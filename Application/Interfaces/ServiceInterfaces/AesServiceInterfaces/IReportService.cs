using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;

namespace Application.Interfaces.ServiceInterfaces.AesServiceInterfaces
{
    public interface IReportService
    {

        public ExtensionResultsDTO GetExtensionsListForReport(DateTime startDate, DateTime endDate, int page, int pageSize,
            List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role);

        public IExtension GetStandingOrdersListForReport(DateTime startDate, DateTime endDate, int page, int pageSize,
            List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role);

        public IExtension GetExtensionForReport(int id);

        public IExtension GetStandingOrderForReport(int id);

    }
}
