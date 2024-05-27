using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.CommonUtilityInterfaces
{
    public interface ICommonUtilitiesRepository
    {
        public ExtIDGenerationResultsDTO GetNewExtensionID(bool useExistingTransaction = false);

        public ExtIDGenerationResultsDTO GetNewEditExtensionID(string oldExtensionID, int editCount);

        public ExtIDGenerationResultsDTO GetNewTerminateExtensionID(string oldExtensionID, int editCount);

        public ExtIDGenerationResultsDTO GetNewStandingOrderID(bool useExistingTransaction = false);

        public ExtIDGenerationResultsDTO GetNewTerminateSOID(string oldStdoID, int editCount);

        public ExtIDGenerationResultsDTO GetNewEditSOID(string oldStdoID, int editCount);

        public List<ExtensionSavingHelperDTO> LoadUnitsByUser(long userID);

        public List<ExtensionSavingHelperDTO> loadUnitsAccordingToUser(long userID, List<ExtensionSavingHelperDTO> lstUnits, string role);

        public List<string> GetRecurrentDays(string soID);

        public List<ExtensionUnitsDTO> LoadExtensionUnits(string extensionID);

        public void DeactivateExtension(string extensionId);
    }
}