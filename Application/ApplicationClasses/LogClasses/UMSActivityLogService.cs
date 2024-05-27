using Application.Interfaces.ServiceInterfaces.LoggerServiceInterfaces;
using LoggerLibrary.Interface;
using SITCAMSClientIntegration.ActivityLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.ApplicationClasses.LogClasses
{
    public class UMSActivityLogService : IUMSActivityLogService
    {
        private readonly ISystemLogger m_systemLogger;

        public UMSActivityLogService(ISystemLogger systemLogger)
        {
            m_systemLogger = systemLogger;
        }

        //This method is to log informtion in the UMS. In "extraData" field any kind of object can be passes
        public Task<bool> CreateLog(string type, string description, object extraData, int userId)
        {
            return m_systemLogger.CreateLog(type, description, extraData, userId);
        }
    }
}