using AES.Domain.Interfaces;
using CommonInsfrastructure.Models.ConfigurationModels;
using Domain.DataTransferringObjects.AesDTOs;

namespace CommonInsfrastructure.Interfaces
{
    public interface IOPCTagService
    {
        public void DeployOPCTagForExtension(ExtensionCommonDTO extension, LocationMapUnitControlTagMapping tag, string onAction, string offAction);

        public List<LocationMapUnitControlTagMapping> GetUnitExtensionOPCTags(string UnitId);
    }
}