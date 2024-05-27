using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.RepositoryInterfaces.AesInterfaces
{
    public  interface IValidationRepository
    {

        public Boolean CheckForExtensionOverlaps(List<ExtensionSavingHelperDTO> lstUnits, DateTime effectiveDate,
     TimeSpan newExtStartTime, TimeSpan newExtEndTime);

        public Boolean CheckForStandingOrderOverlaps(List<ExtensionSavingHelperDTO> lstUnits, DateTime startDate, DateTime endDate,
         TimeSpan newExtStartTime, TimeSpan newExtEndTime, string frequency, List<ExtensionCommonDTO> tempExtensions);

        public Boolean HaveOverlapsWithEditedRequests(List<ExtensionCommonDTO> proposedExtensions, List<ExtensionSavingHelperDTO> lstUnits);

    }
}
