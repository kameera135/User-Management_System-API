using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;

namespace AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces
{
    public interface IExtensionReportRepository
    {

        public ExtensionResultsDTO GetExtensionsListForReport(DateTime startDate, DateTime endDate, int page, int pageSize,
            List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID);

        public IExtension GetExtensionForReport(int id);

    }
}
