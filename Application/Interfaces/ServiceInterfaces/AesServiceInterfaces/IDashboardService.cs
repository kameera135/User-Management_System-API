using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.ServiceInterfaces.AesServiceInterfaces
{
    public interface IDashboardService
    {
        public DashboardResultsDTO GetExtensionsList(int page, int pageSize, List<string> lstStatus, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role);

        public DashboardResultsDTO GetStandingOrdersList(int page, int pageSize, List<string> lstStatus, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role);
    }
}
