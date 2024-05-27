using CommonInsfrastructure.Models.ConfigurationModels;
using Domain.DataTransferringObjects.CommonUtilities;
using Domain.DataTransferringObjects.ConfigDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInsfrastructure.Interfaces
{
    public interface ICommonConfigRepository
    {
        public List<AssertUnitUserMappingDTO> GetUnitsForUser(long userID);

        public Boolean GetAESCommonSettingValue(string key);

        public String GetAESCommonSettingData(string key);

        public Boolean GetCommonSettingValue(string key);

        public String GetCommonSettingData(string key);

        public bool CheckIfHoliday(DateTime date);

        public List<string> GetDefaultWorkingHours(string day, List<ExtensionSavingHelperDTO> lstUnits);

        public List<AescommonSetting> GetAESCommonSettingBulk(string[] keys);

        public void PutCommonSettingData(string key, string data, long updatedBy);
    }
}