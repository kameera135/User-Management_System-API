using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.CommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces
{
    public interface IStandingOrderReportRepository
    {

        public IExtension GetStandingOrdersListForReport(DateTime startDate, DateTime endDate, int page, int pageSize, List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits);

        public IExtension GetStandingOrderForReport(int id);

    }
}
