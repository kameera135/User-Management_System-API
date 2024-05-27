using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using SITCAMSClientIntegration.MemoryCaches;

namespace SITCAMSClientIntegration.AuthenticationMiddleware
{
    /// <summary>
    /// Authentication handler for any platform microservice requests
    /// </summary>
    public class CentralPlatformTokenAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private string message;
        private int responseCode;
        private readonly CentralAuthOptions _centralAuthOptions;
        /// <summary>
        /// The HTTP Client
        /// </summary>
        protected readonly HttpClient _client;
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CentralPlatformTokenAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IOptionsSnapshot<CentralAuthOptions> centralAuthOptions,
            HandlerMemoryCache cache,
            IHttpClientFactory clientFactory
            )
            : base(options, logger, encoder, clock)
        {
            _centralAuthOptions = centralAuthOptions.Value;
            _client = clientFactory.CreateClient("CentralPlatformTokenAuthenticationHandler");
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
                        HttpResponseMessage response;
                        string responseString;

                        var payload = new CentralPlatformAuthVerifyRequest
                        {
                            PlatformId = _centralAuthOptions.PlatformId,
                            Token = token
                        };
                        string strPayload = JsonSerializer.Serialize<CentralPlatformAuthVerifyRequest>(payload);
                        HttpContent content = new StringContent(strPayload, Encoding.UTF8, "application/json");

                        var url = _centralAuthOptions.BaseUrl + "/" + _centralAuthOptions.VerifyPlatformToken;

                        response = await _client.PostAsync(url, content);
                        responseString = await response.Content.ReadAsStringAsync();
                        var statusCode = (int)response.StatusCode;

                        if (statusCode == 200)
                        {
                            isVerified = true;

                            cacheData = new CacheData
                            {
                                Token = token
                            };

                            // Set cache options.
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                 // Keep in cache for this time, reset time if accessed.
                                 .SetSlidingExpiration(TimeSpan.FromSeconds(_centralAuthOptions.PlatformSlidingExpiration))
                                .SetAbsoluteExpiration(TimeSpan.FromSeconds(_centralAuthOptions.PlatformAbsoluteExpiration));

                            // Save data in cache.
                            _cache.Set(token, cacheData, cacheEntryOptions);
                        }
                        else
                        {
                            var responseMessage = JsonSerializer.Deserialize<ResponseMessage>(responseString);
                            Set(responseMessage.Message);
                            return AuthenticateResult.Fail(responseMessage.Message);
                        }
                    }

                    if (isVerified)
                    {
                        var claims = new[]
                        {
                            new Claim("Handler", "SystemToken"),
                            new Claim("RemoteIPAddress", remoteIPAddress)
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
        }
    }
}
