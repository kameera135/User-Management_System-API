using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AES.Domain.Definitions;
using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;

namespace AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces
{
    public interface IStandingOrderRepository
    {
        public IExtension GetStandingOrdersList(DateTime startDate, DateTime endDate, int page, int pageSize,
            List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits,
            string role, string sortby = orderingDefinitions.displayUpdatedAt, int ordering = (Int32)orderingModes.descending);

        public IExtension GetStandingOrderByID(long id, string requestType);

        public string CretaeStandingOrder(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, List<string> lstWeekDays,
            string frequency, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string role);

        //approval/Reject methods
        public bool EditRequestApproval(long SoID, string role, long editedBy, int actionMode);

        public bool TerminateRequestApproval(long extensionID, string role, long editedBy, int actionMode);

        public bool ExtensionApproval(long soID, string role, long editedBy, int actionMode);

        public string TerminateStandingOrder(long id, string extensionID, long createdBy, string role);

        public IExtension GetDependanciesForStandingOrders(long id, string includeProperties);

        public IExtension GetStandingOrderByExtensionID(string extensionDisplayID);

        public string EditStandingOrder(long id, string extensionID, DateTime newStartDate, DateTime newEndDate, TimeSpan newStartTime,
            TimeSpan newEndTime, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string mode, string role);

        public void UpdateStandingOrderStatus(StandingOrderCommonDTO standing_order);

        public List<ExtensionCommonDTO> GetVirtualExtensionsForStandingOrder(DateTime startDate, DateTime endDate, TimeSpan startTime,
            TimeSpan endTime, string recurrentType, List<string> lstFrequency);

        public void SaveStandingOrderHistory(string standingOrderId, string action, int actionBy, DateTime actionAt, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime, List<ExtensionSavingHelperDTO>? units, string? discription, DateTime createdAt, DateTime updatedAt);

        public void UpdateEditedStandingOrderDetails(StandingOrderCommonDTO standing_order);

        public void UpdateTerminateStandingOrderDetails(StandingOrderCommonDTO standing_order);
    }
}