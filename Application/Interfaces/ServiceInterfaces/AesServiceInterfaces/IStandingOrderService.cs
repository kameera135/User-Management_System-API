using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AES.Domain.Definitions;
using AES.Domain.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;

namespace Application.Interfaces.ServiceInterfaces.AesServiceInterfaces
{
    public interface IStandingOrderService
    {
        public IExtension GetStandingOrdersList(DateTime startDate, DateTime endDate, int page, int pageSize,
            List<string> lstStatus, List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits,
            long userID, string role, string sortby = orderingDefinitions.displayUpdatedAt, int ordering = (Int32)orderingModes.descending);

        public IExtension GetExtensionsByID(int id, string requestType);

        public IExtension AuthorizeExtension(int mode, List<BulkApprovalHelperDTO> lstExtensionIds, long requestedby, string role);

        public ExtensionResultsDTO CreateStandingOrder(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime,
            List<string> lstWeekDays, string frequency, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string role);

        public IExtension TerminateStandingOrder(long id, string extensionID, long createdBy, string role);

        public IExtension GetStandingOrderByExtensionID(string extensionDisplayID);

        public ExtensionResultsDTO EditStandingOrder(long id, DateTime newStartDate, DateTime newEndDate, TimeSpan newStartTime,
            TimeSpan newEndTime, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBymode, string mode, string role);

        public void UpdateStandingOrderStatus(StandingOrderCommonDTO standing_order);

        public void SaveStandingOrderHistory(string standingOrderId, string action, int actionBy, DateTime actionAt, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime, List<ExtensionSavingHelperDTO>? units, string? discription, DateTime createdAt, DateTime updatedAt);

        public void UpdateEditedStandingOrderDetails(StandingOrderCommonDTO standing_order);

        public void UpdateTerminateStandingOrderDetails(StandingOrderCommonDTO standing_order);
    }
}