using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.ExternalHelpers.Interfaces;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Requests;
using SITCAMSClientIntegration.Responses;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SITCAMSClientIntegration.ExternalHelpers
{
    public class SystemLogger : ISystemLogger
    {
        protected readonly HttpClient _client;
        private readonly CentralAuthOptions _centralAuthOptions;

        public SystemLogger(IHttpClientFactory clientFactory, IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)
        {
            _client = clientFactory.CreateClient("SystemLogger");
            _centralAuthOptions = centralAuthOptions.Value;
        }

        
        public async Task<bool> CreateLog(string type, string description, object extraData, int userId)
        {
            try
            {

                if (_centralAuthOptions.UseUserName)
                {
                    throw new Exception("Disable UseUserName from appsettings file");
                }

                string url = _centralAuthOptions.BaseUrl + "/" + _centralAuthOptions.CreateActivityLogPath;

                CreatePlatformActivityLogRequest payload = new CreatePlatformActivityLogRequest
                {
                    PlatformId = _centralAuthOptions.PlatformId,
                    Type = type,
                    Description = description,
                    ExtraData = extraData,

                    UserId = userId
                };

                string strPayload = JsonSerializer.Serialize(payload);
                HttpContent content = new StringContent(strPayload, Encoding.UTF8, "application/json");

                var IsHeaderExist = _client.DefaultRequestHeaders.Contains("Authorization");
                if (IsHeaderExist)
                {
                    _client.DefaultRequestHeaders.Remove("Authorization");
                    _client.DefaultRequestHeaders.Add("Authorization", "Token " + _centralAuthOptions.CAMSTokenValue);
                }
                else
                {
                    _client.DefaultRequestHeaders.Add("Authorization", "Token " + _centralAuthOptions.CAMSTokenValue);
                }


                HttpResponseMessage response = await _client.PostAsync(url, content);

                string responseString = await response.Content.ReadAsStringAsync();
                int statusCode = (int)response.StatusCode;

                return true;
            }
            catch (Exception e)
            {
                //return e.ToString();
                return false;
            }
        }
    }
}
