using AutoMapper;
using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.ConfigurationModels;
using Domain.DataTransferringObjects.CommonUtilities;
using Domain.DataTransferringObjects.ConfigDTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonInsfrastructure.Repositories
{
    public class CommonConfigRepository : ICommonConfigRepository
    {
        private readonly ConfigContext m_configDBContext;

        private readonly IMapper m_mapper;

        public CommonConfigRepository(ConfigContext ConfigContext, IMapper mapper)
        {
            m_configDBContext = ConfigContext;

            m_mapper = mapper;
        }

        //This method return a list of unis for the given user. Data is taken from the configuration database
        public List<AssertUnitUserMappingDTO> GetUnitsForUser(long userID)
        {
            List<AssertUnitUserMappingDTO> lstResult = new List<AssertUnitUserMappingDTO>();

            lstResult = m_configDBContext.LocationMapUnitUserMappings.Where(q => q.UserId == userID && q.DeletedAt == null)
                .Include(e => e.Unit)
                .Select(ext => m_mapper.Map<AssertUnitUserMappingDTO>(ext)).ToList();

            return lstResult;
        }

        public Boolean GetAESCommonSettingValue(string key)
        {
            var res = m_configDBContext.AescommonSettings.Where(q => q.Setting == key).FirstOrDefault();
            if (res != null)
            {
                return res.Value;
            }
            return false;
        }

        public String GetAESCommonSettingData(string key)
        {
            var res = m_configDBContext.AescommonSettings.Where(q => q.Setting == key).FirstOrDefault();
            if (res != null && res.Data != null)
            {
                return res.Data;
            }
            return String.Empty;
        }

        public List<AescommonSetting> GetAESCommonSettingBulk(string[] keys)
        {
            List<AescommonSetting> lstConf = new();
            lstConf = m_configDBContext.AescommonSettings.Where(q => keys.Contains(q.Setting)).ToList();
            return lstConf;
        }

        public Boolean GetCommonSettingValue(string key)
        {
            var res = m_configDBContext.CommonSettings.Where(q => q.Setting == key).FirstOrDefault();
            if (res != null)
            {
                return res.Value;
            }
            return false;
        }

        public String GetCommonSettingData(string key)
        {
            var res = m_configDBContext.CommonSettings.Where(q => q.Setting == key).FirstOrDefault();
            if (res != null && res.Data != null)
            {
                return res.Data;
            }
            return String.Empty;
        }

        public bool CheckIfHoliday(DateTime date)
        {
            var res = m_configDBContext.MasterDataHolidays.Where(q => q.Date == date.Date && q.Status == true && q.DeletedAt == null).FirstOrDefault();

            if (res != null)
            {
                return true;
            }
            return false;
        }

        public List<string> GetDefaultWorkingHours(string day, List<ExtensionSavingHelperDTO> lstUnits)
        {
            List<string> lstResult = new List<string>();
            lstResult.Add("00:00");
            lstResult.Add("00:00");

            List<TimeSpan> startHours = new List<TimeSpan>();
            List<TimeSpan> endHours = new List<TimeSpan>();

            foreach (var unit in lstUnits)
            {
                var res = m_configDBContext.LocationMapUnitWorkingHours.Where(q => q.Day == day && q.UnitId == unit.id && q.DeletedAt == null).FirstOrDefault();

                if (res != null)
                {
                    if (res.StartTime != TimeSpan.Parse("00:00")) startHours.Add(res.StartTime);
                    endHours.Add(res.EndTime);
                }
                else
                {
                    var nres = m_configDBContext.MasterDataDefaultOfficeHours.Where(q => q.Day == day && q.DeletedAt == null).FirstOrDefault();
                    if (nres != null)
                    {
                        if (nres.StartTime != TimeSpan.Parse("00:00")) startHours.Add(nres.StartTime);
                        endHours.Add(nres.EndTime);
                    }
                }
            }

            if (startHours.Count > 0)
            {
                lstResult[0] = startHours.Min().Hours.ToString() + ":" + startHours.Min().Minutes.ToString();
            }
            if (endHours.Count > 0)
            {
                lstResult[1] = endHours.Max().Hours.ToString() + ":" + endHours.Max().Minutes.ToString();
            }

            return lstResult;
        }

        public void PutCommonSettingData(string key, string data, long updatedBy)
        {

            try
            {
                CommonSetting tempSetting = new CommonSetting();

                tempSetting = m_configDBContext.CommonSettings.Where(q => q.Setting == key).ToList()[0];

                if (tempSetting != null)
                {
                    tempSetting.Data = data;
                    tempSetting.UpdatedBy = updatedBy;
                    tempSetting.UpdatedAt = DateTime.Now;

                    m_configDBContext.Update<CommonSetting>(tempSetting);
                    m_configDBContext.SaveChanges();
                }
                else
                {
                    throw new Exception("Password policy setting not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            
        }
    }
}