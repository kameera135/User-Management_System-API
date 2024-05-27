using AES.Domain.Definitions;
using AES.Domain.Interfaces;
using Application.Interfaces.ServiceInterfaces.AesServiceInterfaces;
using LoggerLibrary.Interface;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;

using LoggerLibrary.Interface;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SITCAMSClientIntegration.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.ApplicationClasses.AesClasses
{
    public class DashboardService : IDashboardService
    {
        private readonly IExtensionService m_ExtService;
        private readonly IStandingOrderService m_SoService;
        private readonly ILogService m_LogService;
        private readonly IConfiguration m_Configuration;

        //private readonly IReportService m_reportService;

        public DashboardService(IExtensionService extService, IStandingOrderService soService, ILogService logger, IConfiguration configuration /*,IReportService reportService*/)
        {
            m_ExtService = extService;
            m_SoService = soService;
            m_LogService = logger;
            m_Configuration = configuration;
        }

        public DashboardResultsDTO GetExtensionsList(int page, int pageSize, List<string> lstStatus,
            List<ExtensionSavingHelperDTO> lstUnits, long userID, string role)
        {
            DashboardResultsDTO extResults = new DashboardResultsDTO();

            ExtensionResultsDTO objExtensionsResults = new ExtensionResultsDTO();

            int totalPendingExt = 0;

            int pendingExtensionsNext15Days = 0;

            int totalActiveExtensions = 0;

            int activegExtensionsNext15Days = 0;

            int totalApprovedExtensions = 0;

            int approvedExtensionsNext15Days = 0;

            int rejectedExtensionsNext15Days = 0;

            try
            {
                m_LogService.Log("api/dashboard/extension end point has called");

                //List<string> lstStatus = new List<string>();

                DateTime startDate = DateTime.Today;

                int maximumExtensionRange = 12;  //Default 12 months

                int dashboardDateRange = 15; //Default 15 days

                var configMaxExtensionRange = m_Configuration.GetSection(configurationKeys.maxDateForExtensionInMonths);

                var configDashboardDateRange = m_Configuration.GetSection(configurationKeys.dateRangeForDashboard);

                if (configMaxExtensionRange != null)
                {
                    if (!int.TryParse(configMaxExtensionRange.Value, out maximumExtensionRange))
                    {
                        maximumExtensionRange = 12;
                    }
                }

                if (configDashboardDateRange != null)
                {
                    if (!int.TryParse(configDashboardDateRange.Value, out maximumExtensionRange))
                    {
                        dashboardDateRange = 15;
                    }
                }

                DateTime endDate = DateTime.Today.AddMonths(maximumExtensionRange);

                List<string> templstStatus = new List<string>();

                List<string> lstRequestTypes = new List<string>();

                lstRequestTypes.Add(RequestTypeDeffinitions.SelectAll);

                //Get active, pending & approved extensions for next 12 months (or specified date range from config)

                templstStatus.Add(approvalStatusDeffinitions.Pending);

                templstStatus.Add(approvalStatusDeffinitions.ApprovedByFM);

                templstStatus.Add(approvalStatusDeffinitions.ApprovedByTM);

                templstStatus.Add(approvalStatusDeffinitions.Active);

                templstStatus.Add(approvalStatusDeffinitions.Approved);

                objExtensionsResults = (ExtensionResultsDTO)m_ExtService.GetExtensionsList(startDate, endDate, page, pageSize, templstStatus, lstRequestTypes, lstUnits, userID, role);

                //Get total number of pending extensions for next 12 months (or specified date range from config)

                int pendingExtensions = objExtensionsResults.lstExtensionList.Where(extResults => extResults.Status == approvalStatusDeffinitions.Pending).ToList().Count;

                //Get total number of approved by FM extensions for next 12 months (or specified date range from config)

                int approvedByFMExtensions = objExtensionsResults.lstExtensionList.Where(extResults => extResults.Status == approvalStatusDeffinitions.ApprovedByFM).ToList().Count;

                //Get total number of approved by TM extensions for next 12 months (or specified date range from config)

                int approvedByTMExtensions = objExtensionsResults.lstExtensionList.Where(extResults => extResults.Status == approvalStatusDeffinitions.ApprovedByTM).ToList().Count;

                //Get total number of pending (all) extensions for next 12 months (or specified date range from config)

                totalPendingExt = pendingExtensions + approvedByFMExtensions + approvedByTMExtensions;

                //Get total number of active extensions for next 12 months (or specified date range from config)

                totalActiveExtensions = objExtensionsResults.lstExtensionList.Where(extResults => extResults.Status == approvalStatusDeffinitions.Active).ToList().Count;

                //Get total number of approved extensions for next 12 months (or specified date range from config)

                totalApprovedExtensions = objExtensionsResults.lstExtensionList.Where(extResults => extResults.Status == approvalStatusDeffinitions.Approved).ToList().Count;

                //Get active, pending, approved & rejected extensions for next 15 days (or specified date range from config)

                DateTime tmpEndDate = DateTime.Today.AddDays(dashboardDateRange);

                templstStatus.Add(approvalStatusDeffinitions.Rejected);

                objExtensionsResults = (ExtensionResultsDTO)m_ExtService.GetExtensionsList(startDate, tmpEndDate, page, pageSize, templstStatus, lstRequestTypes, lstUnits, userID, role);

                //Get total number of pending (all) extensions for next 15 days (or specified date range from config)

                pendingExtensionsNext15Days = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Pending).ToList().Count;

                //Get total number of active extensions for next 15 days (or specified date range from config)

                activegExtensionsNext15Days = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Active).ToList().Count;

                //Get total number of approved extensions for next 15 days (or specified date range from config)

                approvedExtensionsNext15Days = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Approved).ToList().Count;

                //Get total number of rejected extensions for next 15 days (or specified date range from config)

                rejectedExtensionsNext15Days = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Rejected).ToList().Count;

                if (lstStatus == null)
                {
                    lstStatus = new List<string>();
                }

                objExtensionsResults = (ExtensionResultsDTO)m_ExtService.GetExtensionsList(startDate, endDate, page, pageSize, lstStatus, lstRequestTypes, lstUnits, userID, role);

                if (objExtensionsResults != null)
                {
                    extResults.extensionResult = objExtensionsResults;
                }

                extResults.numberOfPendingResults = totalPendingExt;

                extResults.numberOfActiveResults = totalActiveExtensions;

                extResults.numberOfApprovedResults = totalApprovedExtensions;

                extResults.PendingResultsNext15Days = pendingExtensionsNext15Days;

                extResults.ActiveResultsNext15Days = activegExtensionsNext15Days;

                extResults.ApprovedResultsNext15Days = approvedExtensionsNext15Days;

                extResults.RejectedResultsNext15Days = rejectedExtensionsNext15Days;

                //Get extension details for past 15 days (or for time rage specified in config)

                extResults.lstExpiredResultsForPast15days = new List<int>();

                extResults.lstCompletedResultsForPast15days = new List<int>();

                extResults.lstRejectedResultsForPast15days = new List<int>();

                extResults.lstPastDateTime = new List<string>();

                DateTime tmpStartDate = DateTime.Today.AddDays(-dashboardDateRange);

                tmpEndDate = DateTime.Today;

                templstStatus = new List<string>();

                templstStatus.Add(approvalStatusDeffinitions.Expired);

                templstStatus.Add(approvalStatusDeffinitions.Completed);

                templstStatus.Add(approvalStatusDeffinitions.Rejected);

                objExtensionsResults = (ExtensionResultsDTO)m_ExtService.GetExtensionsList(tmpStartDate, tmpEndDate, page, pageSize, templstStatus, lstRequestTypes, lstUnits, userID, role);

                for (int dayCount = dashboardDateRange; dayCount > 0; dayCount--)
                {
                    tmpStartDate = DateTime.Today.AddDays(-dayCount);

                    string currDateTime = tmpStartDate.ToString("dd MMM");

                    extResults.lstPastDateTime.Add(currDateTime);

                    List<ExtensionCommonDTO> lstExpiredExt = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Expired && extResults.Date.ToString("yyyy-mm-dd").Equals(tmpStartDate.ToString("yyyy-mm-dd"))).ToList();

                    extResults.lstExpiredResultsForPast15days.Add(lstExpiredExt.Count);

                    List<ExtensionCommonDTO> lstCompletedExt = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Completed && extResults.Date.ToString("yyyy-mm-dd").Equals(tmpStartDate.ToString("yyyy-mm-dd"))).ToList();

                    extResults.lstCompletedResultsForPast15days.Add(lstCompletedExt.Count);

                    List<ExtensionCommonDTO> lstRejectedExt = objExtensionsResults.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.ExtensionType == ExtensionTypeDeffinitions.EXT && extResults.Status == approvalStatusDeffinitions.Rejected && extResults.Date.ToString("yyyy-mm-dd").Equals(tmpStartDate.ToString("yyyy-mm-dd"))).ToList();

                    extResults.lstRejectedResultsForPast15days.Add(lstRejectedExt.Count);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return extResults;
        }

        public DashboardResultsDTO GetStandingOrdersList(int page, int pageSize, List<string> lstStatus, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role)
        {
            DashboardResultsDTO soResults = new DashboardResultsDTO();

            StandingOrderResultsDTO objStandingOrders = new StandingOrderResultsDTO();

            ExtensionResultsDTO objExtesionsReult = new ExtensionResultsDTO();

            int totalpendingSO = 0;

            int totalActiveSO = 0;

            int totalApprovedSO = 0;

            int pendingSOExtNext15Days = 0;

            int activeSOExtNext15Days = 0;

            int approvedSOExtNext15Days = 0;

            int rejectedSOExtNext15Days = 0;

            try
            {
                m_LogService.Log("api/dashboard/so end point has called");

                DateTime startDate = DateTime.Today;

                int maximumExtensionRange = 12;  //Default 12 months

                int dashboardDateRange = 15; //Default 15 days

                var configMaxExtensionRange = m_Configuration.GetSection(configurationKeys.maxDateForExtensionInMonths);

                var configDashboardDateRange = m_Configuration.GetSection(configurationKeys.dateRangeForDashboard);

                if (configMaxExtensionRange != null)
                {
                    if (!int.TryParse(configMaxExtensionRange.Value, out maximumExtensionRange))
                    {
                        maximumExtensionRange = 12;
                    }
                }

                if (configDashboardDateRange != null)
                {
                    if (!int.TryParse(configDashboardDateRange.Value, out maximumExtensionRange))
                    {
                        dashboardDateRange = 15;
                    }
                }

                DateTime endDate = DateTime.Today.AddMonths(maximumExtensionRange);

                //List<string> lstStatus = new List<string>();

                List<string> tmpLstStatus = new List<string>();

                List<string> lstRequestTypes = new List<string>();

                lstRequestTypes.Add(RequestTypeDeffinitions.SelectAll);

                //Get active, pending & approved SO for next 12 months (or specified date range from config)

                tmpLstStatus.Add(approvalStatusDeffinitions.Pending);

                tmpLstStatus.Add(approvalStatusDeffinitions.ApprovedByFM);

                tmpLstStatus.Add(approvalStatusDeffinitions.ApprovedByTM);

                tmpLstStatus.Add(approvalStatusDeffinitions.Active);

                tmpLstStatus.Add(approvalStatusDeffinitions.Approved);

                objStandingOrders = (StandingOrderResultsDTO)m_SoService.GetStandingOrdersList(startDate, endDate, page, pageSize, tmpLstStatus, lstRequestTypes, lstUnits, userID, role);

                //Get total number of pending SO for next 12 months (or specified date range from config)

                int pendingSO = objStandingOrders.lstStandingOredrs.Where(extResults => extResults.Status == approvalStatusDeffinitions.Pending).ToList().Count;

                //Get total number of approved by FM extensions for next 12 months (or specified date range from config)

                int approvedByFMSO = objStandingOrders.lstStandingOredrs.Where(extResults => extResults.Status == approvalStatusDeffinitions.ApprovedByFM).ToList().Count;

                //Get total number of approved by TM SO for next 12 months (or specified date range from config)

                int approvedByTMSO = objStandingOrders.lstStandingOredrs.Where(extResults => extResults.Status == approvalStatusDeffinitions.ApprovedByTM).ToList().Count;

                //Get total number of pending (all) SO for next 12 months (or specified date range from config)

                totalpendingSO = pendingSO + approvedByFMSO + approvedByTMSO;

                //Get total number of active SO for next 12 months (or specified date range from config)

                totalActiveSO = objStandingOrders.lstStandingOredrs.Where(extResults => extResults.Status == approvalStatusDeffinitions.Active).ToList().Count;

                //Get total number of approved SO for next 12 months (or specified date range from config)

                totalApprovedSO = objStandingOrders.lstStandingOredrs.Where(extResults => extResults.Status == approvalStatusDeffinitions.Approved).ToList().Count;

                //Get active, pending, approved & rejected SO type extensions for next 15 days (or specified date range from config)

                DateTime tmpEndDate = DateTime.Today.AddDays(dashboardDateRange);

                tmpLstStatus.Add(approvalStatusDeffinitions.Rejected);

                objExtesionsReult = (ExtensionResultsDTO)m_ExtService.GetExtensionsList(startDate, tmpEndDate, page, pageSize, tmpLstStatus, lstRequestTypes, lstUnits, userID, role);

                //Get total number of pending (all) SO type extensions for next 15 days (or specified date range from config)

                pendingSOExtNext15Days = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Pending).ToList().Count;

                //Get total number of active SO type extensions for next 15 days (or specified date range from config)

                activeSOExtNext15Days = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Active).ToList().Count;

                //Get total number of approved SO type extensions for next 15 days (or specified date range from config)

                approvedSOExtNext15Days = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Approved).ToList().Count;

                //Get total number of rejected SO type extensions for next 15 days (or specified date range from config)

                rejectedSOExtNext15Days = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Rejected).ToList().Count;

                if (lstStatus == null)
                {
                    lstStatus = new List<string>();
                }

                objStandingOrders = (StandingOrderResultsDTO)m_SoService.GetStandingOrdersList(startDate, endDate, page, pageSize, lstStatus, lstRequestTypes, lstUnits, userID, role);

                if (objStandingOrders != null)
                {
                    soResults.extensionResult = objStandingOrders;
                }

                soResults.numberOfPendingResults = totalpendingSO;

                soResults.numberOfActiveResults = totalActiveSO;

                soResults.numberOfApprovedResults = totalApprovedSO;

                soResults.PendingResultsNext15Days = pendingSOExtNext15Days;

                soResults.ActiveResultsNext15Days = activeSOExtNext15Days;

                soResults.ApprovedResultsNext15Days = approvedSOExtNext15Days;

                soResults.RejectedResultsNext15Days = rejectedSOExtNext15Days;

                //Get so extension details for past 15 days (or for time rage specified in config)

                soResults.lstExpiredResultsForPast15days = new List<int>();

                soResults.lstCompletedResultsForPast15days = new List<int>();

                soResults.lstRejectedResultsForPast15days = new List<int>();

                soResults.lstPastDateTime = new List<string>();

                DateTime tmpStartDate = DateTime.Today.AddDays(-dashboardDateRange);

                tmpEndDate = DateTime.Today;

                tmpLstStatus = new List<string>();

                tmpLstStatus.Add(approvalStatusDeffinitions.Expired);

                tmpLstStatus.Add(approvalStatusDeffinitions.Completed);

                tmpLstStatus.Add(approvalStatusDeffinitions.Rejected);

                objExtesionsReult = (ExtensionResultsDTO)m_ExtService.GetExtensionsList(tmpStartDate, tmpEndDate, page, pageSize, tmpLstStatus, lstRequestTypes, lstUnits, userID, role);

                for (int dayCount = dashboardDateRange; dayCount > 0; dayCount--)
                {
                    tmpStartDate = DateTime.Today.AddDays(-dayCount);

                    string currDateTime = tmpStartDate.ToString("dd MMM");

                    soResults.lstPastDateTime.Add(currDateTime);

                    List<ExtensionCommonDTO> lstExpiredSOExt = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Expired && extResults.Date.ToString("yyyy-mm-dd").Equals(tmpStartDate.ToString("yyyy-mm-dd"))).ToList();

                    soResults.lstExpiredResultsForPast15days.Add(lstExpiredSOExt.Count);

                    List<ExtensionCommonDTO> lstCompletedSOExt = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Completed && extResults.Date.ToString("yyyy-mm-dd").Equals(tmpStartDate.ToString("yyyy-mm-dd"))).ToList();

                    soResults.lstCompletedResultsForPast15days.Add(lstCompletedSOExt.Count);

                    List<ExtensionCommonDTO> lstRejectedSOExt = objExtesionsReult.lstExtensionList.Where(extResults => extResults.ExtensionType == ExtensionTypeDeffinitions.SO && extResults.Status == approvalStatusDeffinitions.Rejected && extResults.Date.ToString("yyyy-mm-dd").Equals(tmpStartDate.ToString("yyyy-mm-dd"))).ToList();

                    soResults.lstRejectedResultsForPast15days.Add(lstRejectedSOExt.Count);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return soResults;
        }
    }
}