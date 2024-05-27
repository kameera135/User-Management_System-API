using CommonInsfrastructure.DTOs;
using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.ConfigurationModels;
using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInsfrastructure.Controllers
{
    [Route("api/common/settings/")]
    [ApiController]
    [Authorize]
    public class CommonSettingsController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly ICommonConfigRepository commonConfig;

        public CommonSettingsController(ILogService logService, ICommonConfigRepository commonConfig)
        {
            m_logService = logService;
            this.commonConfig = commonConfig;
        }

        [HttpGet]
        [Route("system-data/{settingId}")]
        public string GetSystemSettingData(string settingId)
        {
            string data = String.Empty;

            data = commonConfig.GetCommonSettingData(settingId);

            m_logService.Log(String.Format("Common setting {0} was readed", settingId));

            return data;
        }

        [HttpGet]
        [Route("system-value/{settingId}")]
        public bool GetSystemSettingVlue(string settingId)
        {
            bool value = false;

            value = commonConfig.GetCommonSettingValue(settingId);

            m_logService.Log(String.Format("Common setting {0} was readed", settingId));

            return value;
        }

        /*[HttpGet]
        [Route("aes-data/{settingId}")]
        public string GetAESSettingData(string settingId)
        {
            string data = String.Empty;

            data = commonConfig.GetAESCommonSettingData(settingId);

            m_logService.Log(String.Format("Common setting {0} was readed", settingId));

            return data;
        }

        [HttpGet]
        [Route("aes-value/{settingId}")]
        public bool GetAESSettingVlue(string settingId)
        {
            bool value = false;

            value = commonConfig.GetAESCommonSettingValue(settingId);

            m_logService.Log(String.Format("Common setting {0} was readed", settingId));

            return value;
        }*/
    }
}