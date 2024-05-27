using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Requests;
using SITCAMSClientIntegration.Responses;
using SITCAMSClientIntegration.Repositories.Interfaces;
using SITCAMSClientIntegration.MemoryCaches;
using SITCAMSClientIntegration.Models;

namespace SITCAMSClientIntegration.AuthenticationMiddleware
{
    /// <summary>
    /// Class to handle the central token authentication
    /// </summary>
    public class CentralTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private string message;
        private int responseCode;
        private readonly CentralAuthOptions _centralAuthOptions;
        private readonly HttpClient _client;
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CentralTokenAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptionsSnapshot<CentralAuthOptions> centralAuthOptions,
            IUserRepository userRepository,
            HandlerMemoryCache cache,
            IHttpClientFactory clientFactory
            )
            : base(options, logger, encoder, clock)
        {
            _centralAuthOptions = centralAuthOptions.Value;
            _client = clientFactory.CreateClient("CentralTokenAuthenticationHandler");
            _userRepository = userRepository;
            _cache = cache.Cache;
        }

        /// <inheritdoc />
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                string remoteIPAddress = TryGetIP();

                var path = Request.Path;
                Debug.WriteLine(path);

                if (!Request.Headers.ContainsKey("Authorization"))
                {
                    Set("Authorization header was not found");
                    return AuthenticateResult.Fail("Authorization header was not found");
                }

                var authorizationHeaderValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]).ToString();
                if (authorizationHeaderValue.Contains("Token"))
                {
                    var token = authorizationHeaderValue.Replace("Token ", "").Trim();

                    bool isVerified = false;

                    // Look for cache key.
                    CacheData cacheData;
                    if (_cache.TryGetValue(token, out cacheData))
                    {
                        isVerified = true;
                    }
                    else
                    {
                        var userAgent = Request.Headers[HeaderNames.UserAgent].ToString();

                        HttpResponseMessage response;
                        string responseString;

                        var payload = new PlatformAuthVerifyRequest
                        {
                            PlatformId = _centralAuthOptions.PlatformId,
                            Token = token,
                            UserAgent = userAgent,
                            RemoteIPAddress = remoteIPAddress,
                            BehaviorString = "test"
                        };
                        string strPayload = JsonSerializer.Serialize<PlatformAuthVerifyRequest>(payload);
                        HttpContent content = new StringContent(strPayload, Encoding.UTF8, "application/json");

                        var url = _centralAuthOptions.BaseUrl + "/" + _centralAuthOptions.VerifyToken;

                        response = await _client.PostAsync(url, content);
                        responseString = await response.Content.ReadAsStringAsync();
                        var statusCode = (int)response.StatusCode;

                        if (statusCode == 200)
                        {
                            isVerified = true;
                            User userRole;
                            string userName = "";
                            int userId = -1;
                            var verificationResponseMessage = JsonSerializer.Deserialize<TokenVerificationResponseMessage>(responseString);
                            if (_centralAuthOptions.UseUserName)
                            {
                                userName = verificationResponseMessage.UserName;
                                userRole = _userRepository.GetUserWithRole(userName);
                            }
                            else
                            {
                                userId = verificationResponseMessage.UserId;
                                userRole = _userRepository.GetUserWithRole(userId);
                            }


                            if (userRole == null)
                            {
                                Set("Login is required", 409);
                                return AuthenticateResult.Fail("Login is required");
                            }

                            cacheData = new CacheData
                            {
                                Token = token,
                                UserName = userName,
                                UserId = userId,
                                UserRole = userRole.Role.Name,
                                UserRank = userRole.Role.Rank
                            };

                            // Set cache options.
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                // Keep in cache for this time, reset time if accessed.
                                .SetSlidingExpiration(TimeSpan.FromSeconds(_centralAuthOptions.SlidingExpiration))
                                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_centralAuthOptions.AbsoluteExpiration));

                            // Save data in cache.
                            _cache.Set(token, cacheData, cacheEntryOptions);
                        }
                        else
                        {
                            var responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);
                            Set(responseMessage.Message, 409);
                            return AuthenticateResult.Fail(responseMessage.Message);
                        }
                    }

                    if (isVerified)
                    {
                        var claims = new[]
                        {
                            new Claim("Handler", "Token"),
                            new Claim("RemoteIPAddress", remoteIPAddress),
                            new Claim("UserId", cacheData.UserId.ToString()),
                            new Claim("UserName", cacheData.UserName),
                            new Claim("UserRole", cacheData.UserRole.ToString()),
                            new Claim("UserRank", cacheData.UserRank.ToString())
                        };

                        var identity = new ClaimsIdentity(claims, Scheme.Name);
                        var principal = new ClaimsPrincipal(identity);
                        var ticket = new AuthenticationTicket(principal, Scheme.Name);

                        return AuthenticateResult.Success(ticket);
                    }
                }

                Set("Token is invalid");
                return AuthenticateResult.Fail("Token is invalid");
            }
            catch (Exception e)
            {
                Set(e.Message);
                return AuthenticateResult.Fail(e.Message);
            }
        }

        private string TryGetIP()
        {
            var remoteIPAddress = string.Empty;
            try
            {
                IPAddress IP = Request.HttpContext.Connection.RemoteIpAddress;
                if (IP != null)
                {
                    // if (IP.AddressFamily == AddressFamily.InterNetwork)
                    // {
                    //     IP = Dns.GetHostEntry(IP).AddressList
                    //         .First(x => x.AddressFamily == AddressFamily.InterNetwork);
                    // }

                    remoteIPAddress = IP.ToString();
                }
            }
            catch (Exception)
            {
            }

            return remoteIPAddress;
        }

        /// <inheritdoc />
        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {


            if (message != null)
            {
                Response.StatusCode = responseCode;
                var jsonString = JsonSerializer.Serialize<ResponseMessage>(ResponseMessage.Get(false, message));
                byte[] data = Encoding.UTF8.GetBytes(jsonString);
                Response.ContentType = "application/json";
                Response.Body.WriteAsync(data, 0, data.Length);
            }

            return Task.CompletedTask;
        }

        private void Set(string message, int responseCode = 401)
        {
            this.message = message;
            this.responseCode = responseCode;
        }

        private class CacheData
        {
            public string Token { get; set; }
            public string UserName { get; set; }
            public int UserId { get; set; }
            public string UserRole { get; set; }
            public int UserRank { get; set; }
        }
    }
}
