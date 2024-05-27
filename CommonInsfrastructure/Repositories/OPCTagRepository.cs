using AES.Domain.Definitions;
using AES.Domain.Interfaces;
using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.ConfigurationModels;
using CommonInsfrastructure.Models.EmailModels;
using CommonInsfrastructure.Models.OPCTagModels;
using Domain.DataTransferringObjects.AesDTOs;
using LoggerLibrary.Interface;

namespace CommonInsfrastructure.Repositories
{
    public class OPCTagRepository : IOPCTagService
    {
        private readonly ILogService m_logService;
        private readonly BacnetDbContext m_bacnetDbContext;
        private readonly ConfigContext m_ConfigContext;

        public OPCTagRepository(ILogService logService, BacnetDbContext bacnetDbContext, ConfigContext ConfigContext)
        {
            m_logService = logService;
            m_bacnetDbContext = bacnetDbContext;
            m_ConfigContext = ConfigContext;
        }

        public void DeployOPCTagForExtension(ExtensionCommonDTO extension, LocationMapUnitControlTagMapping tag, string onAction, string offAction)
        {
            TblSchedule record = new();
            record.OpcTag = tag.ControlTag;
            record.OnActionTime = extension.Date.Add(extension.StartTime);
            record.OnActionValue = "1";
            record.OnActionStatus = onAction;
            record.OffActionTime = extension.Date.Add(extension.EndTime);
            record.OffActionValue = "0";
            record.OffActionStatus = offAction;
            record.ReferenceCode1 = extension.ExtensionId;
            record.ReferenceCode2 = tag.UnitId;
            record.ReferenceCode3 = tag.Description;
            record.InsertTime = DateTime.Now;
            m_bacnetDbContext.TblSchedules.Add(record);
            m_bacnetDbContext.SaveChanges();

            m_logService.Log(string.Format("OPC Tag '{0}' was deployed to the table successfully", tag.ControlTag));
        }

        public List<LocationMapUnitControlTagMapping> GetUnitExtensionOPCTags(string UnitId)
        {
            List<LocationMapUnitControlTagMapping> tags = m_ConfigContext.LocationMapUnitControlTagMappings.Where(
                x => x.UnitId == UnitId &&
                x.DeletedAt == null &&
                x.ExtensionReady == true
                ).ToList();

            return tags;
        }
    }
}