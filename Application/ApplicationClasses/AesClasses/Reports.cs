using AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces;
using AES.Application.Interfaces.RepositoryInterfaces.CommonUtilityInterfaces;
using AES.Domain.Interfaces;
using Application.Interfaces.ServiceInterfaces.AesServiceInterfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.ApplicationClasses.AesClasses
{
    public class Reports : IReportService
    {

        private readonly IExtensionReportRepository m_extensionReportRepository;

        private StringBuilder m_validationErrorLog = new StringBuilder();

        private IConfiguration m_configuration;

        private IStandingOrderReportRepository m_stodRepository;

        private ICommonUtilitiesRepository m_commonUtilities;

        public Reports(IExtensionReportRepository extReportRepository,
            IStandingOrderReportRepository stodRepository,
            IConfiguration configuration,
            ICommonUtilitiesRepository commonUtilities)
        {

            m_extensionReportRepository = extReportRepository;

            m_configuration = configuration;

            m_stodRepository = stodRepository;

            m_commonUtilities = commonUtilities;

        }

        public ExtensionResultsDTO GetExtensionsListForReport(DateTime startDate, DateTime endDate, int page, int pageSize,
            List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role)
        {

            List<ExtensionSavingHelperDTO> filteredUnits = new List<ExtensionSavingHelperDTO>();

            filteredUnits = m_commonUtilities.loadUnitsAccordingToUser(userID, lstUnits, role);

            return m_extensionReportRepository.GetExtensionsListForReport(startDate, endDate, page, pageSize, lstStatus, lstRequestTypes, filteredUnits, userID);

        }

        public IExtension GetStandingOrdersListForReport(DateTime startDate, DateTime endDate, int page, int pageSize, List<string> lstStatus,
            List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role)
        {

            List<ExtensionSavingHelperDTO> filteredUnits = new List<ExtensionSavingHelperDTO>();

            filteredUnits = m_commonUtilities.loadUnitsAccordingToUser(userID, lstUnits, role);

            return m_stodRepository.GetStandingOrdersListForReport(startDate, endDate, page, pageSize, lstStatus, lstRequestTypes, filteredUnits);

        }

        public IExtension GetExtensionForReport(int id)
        {

            return m_extensionReportRepository.GetExtensionForReport(id);

        }

        public IExtension GetStandingOrderForReport(int id)
        {

            return m_stodRepository.GetStandingOrderForReport(id);

        }

    }
}
