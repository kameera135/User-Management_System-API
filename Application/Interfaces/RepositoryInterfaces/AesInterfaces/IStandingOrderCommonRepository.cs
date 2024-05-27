using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces
{
    public interface IStandingOrderCommonRepository
    {

        public List<string> GetExtensionsForUnit(List<ExtensionSavingHelperDTO> lstUnits);

        public List<StandingOrderCommonDTO> LoadExtensionUnitsForStandingOrders(List<StandingOrderCommonDTO> lstTemp);


    }
}
