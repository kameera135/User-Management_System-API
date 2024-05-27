using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.DTOs;
using SITCAMSClientIntegration.ExternalHelpers.Interfaces;
using SITCAMSClientIntegration.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SITCAMSClientIntegration.ExternalHelpers
{
    public class UserInfo : IUserInfo
    {
        protected readonly HttpClient _client;
        private readonly CentralAuthOptions _centralAuthOptions;

        public UserInfo(IHttpClientFactory clientFactory, IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)
        {
            _client = clientFactory.CreateClient("UserInfo");
            _centralAuthOptions = centralAuthOptions.Value;
        }

        public async Task<EmployeeDTO> GetUserDetails(int userId, string userToken)
        {
            try
            {
                if (_centralAuthOptions.UseUserName)
                {
                    throw new Exception("Disable UseUserName from appsettings file");
                }

                string url = $"{_centralAuthOptions.BaseUrl}/{_centralAuthOptions.UserProfilePath}/{userId}";

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

                var IsPlatformUserTokenHeaderExist = _client.DefaultRequestHeaders.Contains("Platform-User-Token");
                if (IsPlatformUserTokenHeaderExist)
                {
                    _client.DefaultRequestHeaders.Remove("Platform-User-Token");
                    _client.DefaultRequestHeaders.Add("Platform-User-Token", $"{_centralAuthOptions.PlatformId}_{userToken}");
                }
                else
                {
                    _client.DefaultRequestHeaders.Add("Platform-User-Token", $"{_centralAuthOptions.PlatformId}_{userToken}");
                }


                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<EmployeeDTO>(responseString);

                return dto;
            }
            catch (Exception e)
            {
                //return e.ToString();
                return null;
            }
        }

        public async Task<PaginationDTO<EmployeeWithRoleDTO>> GetAllUsersByPlatform(int page = 0, int limit = 0)
        {
            try
            {
                if (_centralAuthOptions.UseUserName)
                {
                    throw new Exception("Disable UseUserName from appsettings file");
                }

                string url;
                //{platformId}?page=1&limit=1500
                if (page != 0 && limit != 0)
                {
                    url = $"{_centralAuthOptions.BaseUrl}/{_centralAuthOptions.GetAllUsersPath}/{_centralAuthOptions.PlatformId}?page={page}&limit={limit}";
                }
                else
                {
                    url = $"{_centralAuthOptions.BaseUrl}/{_centralAuthOptions.GetAllUsersPath}/{_centralAuthOptions.PlatformId}";
                }

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


                HttpResponseMessage response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string responseString = await response.Content.ReadAsStringAsync();
                var dto = JsonSerializer.Deserialize<PaginationDTO<EmployeeWithRoleDTO>>(responseString);

                return dto;
            }
            catch (Exception e)
            {
                //return e.ToString();
                return null;
            }
        }
    }
}
