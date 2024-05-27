using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Responses;
using SITCAMSClientIntegration.Requests;
using SITCAMSClientIntegration.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace SITCAMSClientIntegration.Controllers
{
    /// <summary>
    /// Controller to handle authentication calls to CAMS Core.
    /// </summary>
    [Route("api/central-auth")]
    [ApiController]
    public class CentralAuthController : ControllerBase
    {
        protected readonly HttpClient _client;
        protected readonly IUserRepository _userRepository;
        private readonly CentralAuthOptions _centralAuthOptions;

        public CentralAuthController(IOptionsSnapshot<CentralAuthOptions> centralAuthOptions, IUserRepository userRepository, IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("CentralAuthController");
            _centralAuthOptions = centralAuthOptions.Value;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Get platform specific user token using login information.
        /// </summary>
        /// <param name="request">A request contianing username and password for login</param>
        /// <returns>A token response message from the CAMS Core with 
        /// platform specific user token and authentication information</returns>
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseMessage>> GetTokenAsync(AuthRequest request)
        {
            try
            {
                string remoteIPAddress = TryGetIPAddress();
                string userAgent = Request.Headers[HeaderNames.UserAgent].ToString();
                string url = _centralAuthOptions.BaseUrl + "/" + _centralAuthOptions.Login;
                PlatformAuthRequest payload = new PlatformAuthRequest
                {
                    PlatformId = _centralAuthOptions.PlatformId,
                    Username = request.Username,
                    Password = request.Password,
                    RemoteIPAddress = remoteIPAddress,
                    UserAgent = userAgent,
                    BehaviorString = request.BehaviorString
                };
                return await CAMSVerify<PlatformAuthRequest>(url, payload);
            }
            catch (Exception e)
            {
                //return BadRequest(ResponseMessage.Get(false, e.ToString()));
                return BadRequest(ResponseMessage.Get(false, e.Message));
            }
        }

        /// <summary>
        /// Get platform specific user token using Single auth token.
        /// </summary>
        /// <param name="request">A request contianing single-sign-on token</param>
        /// <returns>A token response message from the CAMS Core with 
        /// platform specific user token and authentication information</returns>
        [HttpPost("single-auth/verify")]
        public async Task<ActionResult<TokenResponseMessage>> TokenAuthAsync(AuthVerifyRequest request)
        {
            try
            {
                string remoteIPAddress = TryGetIPAddress();
                string userAgent = Request.Headers[HeaderNames.UserAgent].ToString();
                string url = _centralAuthOptions.BaseUrl + "/" + _centralAuthOptions.SingleAuthVerify;
                PlatformAuthVerifyRequest payload = new PlatformAuthVerifyRequest
                {
                    PlatformId = _centralAuthOptions.PlatformId,
                    Token = request.Token,
                    RemoteIPAddress = remoteIPAddress,
                    UserAgent = userAgent,
                    BehaviorString = request.BehaviorString
                };
                return await CAMSVerify<PlatformAuthVerifyRequest>(url, payload);
            }
            catch (Exception e)
            {
                return BadRequest(ResponseMessage.Get(false, e.ToString()));
            }
        }

        /// <summary>
        /// Log user out of platform
        /// </summary>
        /// <param name="request">Logout request containing the user token</param>
        /// <returns>A token response message indicating the success of the logout.</returns>
        [HttpPost("logout")]
        public async Task<ActionResult<TokenResponseMessage>> LogoutAsync(LogoutRequest request)
        {
            try
            {
                string remoteIPAddress = TryGetIPAddress();
                string userAgent = Request.Headers[HeaderNames.UserAgent].ToString();
                string url = _centralAuthOptions.BaseUrl + "/" + _centralAuthOptions.Logout;
                PlatformAuthVerifyRequest payload = new PlatformAuthVerifyRequest
                {
                    PlatformId = _centralAuthOptions.PlatformId,
                    Token = request.Token,
                    RemoteIPAddress = remoteIPAddress,
                    UserAgent = userAgent,
                    BehaviorString = request.BehaviorString
                };
                return await CAMSVerify<PlatformAuthVerifyRequest>(url, payload, update: false);
            }
            catch (Exception e)
            {
                return BadRequest(ResponseMessage.Get(false, e.ToString()));
            }
        }

        private string TryGetIPAddress()
        {
            string remoteIPAddress = string.Empty;
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
        private async Task<ActionResult<TokenResponseMessage>> CAMSVerify<Req>(string url, Req payload, bool update = true)
        {
            string strPayload = JsonSerializer.Serialize<Req>(payload);
            HttpContent content = new StringContent(strPayload, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _client.PostAsync(url, content);
            string responseString = await response.Content.ReadAsStringAsync();
            int statusCode = (int)response.StatusCode;
            TokenResponseMessage tokenResponse = JsonSerializer.Deserialize<TokenResponseMessage>(responseString);

            if (statusCode == 200)
            {
                if (update) UpdateContext(tokenResponse);
                return Ok(tokenResponse);
            }
            else return BadRequest(tokenResponse);
        }

        private void UpdateContext(TokenResponseMessage tokenResponse)
        {
            int userId = tokenResponse.UserId;
            string updatedRole = tokenResponse.Role;
            int updatedRank = tokenResponse.Rank;
            string userName = "";
            if (_centralAuthOptions.UseUserName) userName = tokenResponse.UserName;
            List<string> updatedPermissions = tokenResponse.Permissions;

            // Add or Update User
            _userRepository.UpdateContext(userId, userName, updatedRole, updatedRank, updatedPermissions);
        }

        
    }
}
