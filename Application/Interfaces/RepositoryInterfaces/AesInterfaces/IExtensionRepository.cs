using AES.Domain.Definitions;
using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using SITCAMSClientIntegration.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces
{
    public interface IExtensionRepository
    {
        //Page : Clicked page from the pagination. pagesize : Number of records should be displayed on the screen
        public IExtension GetExtensionsList(DateTime startDate, DateTime endDate, int page, int pageSize, List<string> lstStatus,
            List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role, string sortby = orderingDefinitions.displayUpdatedAt, int ordering = (Int32)orderingModes.descending);

        public IExtension GetExtensionsByID(long id, string requestType);

        public string CreateExtension(DateTime effectiveDate, TimeSpan startTime, TimeSpan endTime, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string role);

        public string EditExtension(long id, string extensionID, DateTime newDate, TimeSpan newStartTime, TimeSpan newEndTime, long createdBy, string description, List<ExtensionSavingHelperDTO> lstUnits, string role);

        public string TerminateExtension(long id, string extensionID, long createdBy, string role);

        public ExtensionResultsDTO GetDependanciesForExtension(long id, string includeProperties);

        public bool EditRequestApproval_FacilityManager(long extensionID, string role, long editedBy, int actionMode);

        public bool TerminateRequestApproval_FacilityManager(long extensionID, string role, long editedBy, int actionMode);

        public bool EditRequestApproval_TenantManager(long extensionID, string role, long editedBy, int actionMode);

        public bool TerminateRequestApproval_TenantManager(long extensionID, string role, long editedBy, int actionMode);

        public bool ExtensionApproval_TenantManager(long extensionID, string role, long editedBy, int actionMode);

        public bool ExtensionApproval_FacilityManager(long extensionID, string role, long editedBy, int StatusCode);

        public void UpdateExtensionStatus(ExtensionCommonDTO extension);

        public void UpdateEditedExtensionDetails(ExtensionCommonDTO extension);

        public void UpdateTerminateExtensionDetails(ExtensionCommonDTO extension);

        public Boolean CheckOverLappingExtensionsAvailable(List<ExtensionSavingHelperDTO> lstUnits, DateTime effectiveDate, TimeSpan newExtStartTime, TimeSpan newExtEndTime);

        public Boolean CheckOverLappingExtensionsAvailableInRange(List<ExtensionSavingHelperDTO> lstUnits, DateTime startDate, DateTime endDate, TimeSpan newExtStartTime, TimeSpan newExtEndTime, string frequency, List<ExtensionCommonDTO> tempExtensions);

        public void SaveExtensionHistory(string extensionId, string action, int actionBy, DateTime actionAt, DateTime? date, TimeSpan? startTime, TimeSpan? endTime, List<ExtensionSavingHelperDTO>? units, string? discription, DateTime createdAt, DateTime updatedAt);

        public IExtension GetExtensionsNeedActivating(int days);

        public void ActivateExtension(long id);
    }
}