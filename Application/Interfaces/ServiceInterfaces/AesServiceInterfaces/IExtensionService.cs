using AES.Domain.Definitions;
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
    public interface IExtensionService
    {
        public IExtension GetExtensionsList(DateTime startDate, DateTime endDate, int page, int pageSize, List<string> lstStatus,
            List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role
            , string orderingColumn = orderingDefinitions.displayUpdatedAt, int orderingType = (Int32)orderingModes.descending);

        public ExtensionResultsDTO CreateExtension(DateTime effectiveDate, TimeSpan startTime, TimeSpan endTime, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string role);

        public ExtensionResultsDTO EditExtension(long id, string extensionID, DateTime newDate, TimeSpan newStartTime, TimeSpan newEndTime, long createdBy, string description, List<ExtensionSavingHelperDTO> lstUnits, string role);

        public ExtensionResultsDTO TerminateExtension(long id, string extensionID, long createdBy, string role);

        public ExtensionResultsDTO AuthorizeExtension(int mode, List<BulkApprovalHelperDTO> lstExtensionIds, long requestedby, string role);

        public IExtension GetExtensionsByID(int id, string requestType);

        public void UpdateExtensionState(ExtensionCommonDTO extension);

        public void UpdateEditedExtensionDetails(ExtensionCommonDTO extension);

        public void UpdateTerminateExtensionDetails(ExtensionCommonDTO extension);

        public void SaveExtensionHistory(string extensionId, string action, int actionBy, DateTime actionAt, DateTime? date, TimeSpan? startTime, TimeSpan? endTime, List<ExtensionSavingHelperDTO>? units, string? discription, DateTime createdAt, DateTime updatedAt);

        public IExtension GetExtensionsNeedActivating(int days);

        public void ActivateExtension(long id);
    }
}