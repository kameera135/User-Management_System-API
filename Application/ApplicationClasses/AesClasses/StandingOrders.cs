using AES.Application.Interfaces.RepositoryInterfaces.AesInterfaces;
using AES.Application.Interfaces.RepositoryInterfaces.CommonUtilityInterfaces;
using AES.Domain.Definitions;
using AES.Domain.Interfaces;
using AES.Domain.ListMappers;
using Application.Interfaces.RepositoryInterfaces.AesInterfaces;
using Application.Interfaces.ServiceInterfaces.AesServiceInterfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.ApplicationClasses.AesClasses
{
    public class StandingOrders : IStandingOrderService
    {
        private IStandingOrderRepository m_SoRepository;

        private StringBuilder m_validationErrorLog = new StringBuilder();

        private IConfiguration m_configuration;

        private ICommonUtilitiesRepository m_commonUtilities;

        private readonly IExtensionRepository _extensionRepository;

        private readonly IValidationRepository m_ValidationRepo;

        public StandingOrders(IConfiguration configuration, IStandingOrderRepository soRepository, ICommonUtilitiesRepository commonUtilities,
            IExtensionRepository extRepository, IValidationRepository valRepo)
        {
            m_configuration = configuration;

            m_SoRepository = soRepository;

            m_commonUtilities = commonUtilities;

            _extensionRepository = extRepository;

            m_ValidationRepo = valRepo;
        }

        public IExtension GetExtensionsByID(int id, string requestType)
        {
            return m_SoRepository.GetStandingOrderByID(id, requestType);
        }

        public ExtensionResultsDTO EditStandingOrder(long id, DateTime newStartDate, DateTime newEndDate, TimeSpan newStartTime,
            TimeSpan newEndTime, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string mode, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            ValidateUpdateExtensionInputs(id, newStartDate, newEndDate, newStartTime, newEndTime, description, lstUnits);

            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)//If Validation issues are available
            {
                objResult.isValidated = false;

                objResult.isSucceded = false;

                objResult.lstValidationError = m_validationErrorLog.ToString();
            }
            else
            {
                objResult.extensionID = m_SoRepository.EditStandingOrder(id, "", newStartDate, newEndDate, newStartTime, newEndTime,
                    description, lstUnits, createdBy, mode, role);

                objResult.isValidated = true;

                objResult.isSucceded = true;
            }

            return objResult;
        }

        private void ValidateUpdateExtensionInputs(long id, DateTime newStartDate, DateTime newEndDate, TimeSpan newStartTime, TimeSpan newEndTime, string description, List<ExtensionSavingHelperDTO> lstUnits)
        {
            var result = (StandingOrderResultsDTO)m_SoRepository.GetStandingOrderByID(id, RequestTypeDeffinitions.New);

            if (result != null && result.selectedStandingOrder != null)
            {
                List<string> lstWeekDays = m_commonUtilities.GetRecurrentDays(result.selectedStandingOrder.StandingOrderId);

                ValidateExtensionInputs(newStartDate, newEndDate, newStartTime, newEndTime, lstWeekDays, result.selectedStandingOrder.RecurrentType, description, lstUnits);
            }
        }

        public ExtensionResultsDTO CreateStandingOrder(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime,
            List<string> lstWeekDays, string frequency, string description, List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            //Validates the date and times
            ValidateExtensionInputs(startDate, endDate, startTime, endTime, lstWeekDays, frequency, description, lstUnits);

            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)//If Validation issues are available
            {
                objResult.isValidated = false;

                objResult.isSucceded = false;

                objResult.lstValidationError = m_validationErrorLog.ToString();
            }
            else
            {
                objResult.extensionID = m_SoRepository.CretaeStandingOrder(startDate, endDate, startTime, endTime, lstWeekDays, frequency,
                    description, lstUnits, createdBy, role);

                objResult.isValidated = true;

                objResult.isSucceded = true;
            }

            return objResult;
        }

        private void ValidateExtensionInputs(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, List<string> lstWeekDays, string frequency, string description, List<ExtensionSavingHelperDTO> lstUnits)
        {
            if (startDate == default || endDate == default)
            {
                m_validationErrorLog.AppendLine(commonMessages.soValidityPeriodValidationMessage);
            }
            else if (startDate > endDate)
            {
                m_validationErrorLog.AppendLine(commonMessages.soValidityPeriodValidationMessage);
            }
            /*else if (endDate > startDate.AddYears(1))
            {
                m_validationErrorLog.AppendLine(commonMessages.soValidaityPeriodRangeValidationMessage);
            }*/

            //Start time < Ending time
            if (startTime >= endTime)
            {
                m_validationErrorLog.AppendLine(commonMessages.invalidTimeRangeValidationMessage);
            }

            if (lstUnits == null || lstUnits.Count == 0)
            {
                m_validationErrorLog.AppendLine(commonMessages.noUnitsValidationMessage);
            }

            if (frequency == null || frequency == string.Empty)
            {
                m_validationErrorLog.AppendLine(commonMessages.soFrequencyTypeValidaionMessage);
            }
            else if (frequency.ToUpper().Equals(soFrequencyDefinitions.weekly.ToUpper()))//If weekly frequency selected check for the recurrent days
            {
                if (lstWeekDays == null || lstWeekDays.Count == 0)
                {
                    m_validationErrorLog.AppendLine(commonMessages.soRecurrentDayListValidaionMessage);
                }
            }

            if (description == null || description == string.Empty)
            {
                m_validationErrorLog.AppendLine(commonMessages.soDescriptionValidationMessage);
            }

            //Validate Standing Order for overlaps
            //Create extensions for standing order virtually
            //validate virtual extension with real extensions

            List<ExtensionCommonDTO> tempExtensions = m_SoRepository.GetVirtualExtensionsForStandingOrder(startDate, endDate, startTime, endTime, frequency, lstWeekDays);

            Boolean overlapFound = false;

            if (tempExtensions != null && tempExtensions.Count > 0)
            {
                //Check if overlaps with extensions
                //Check if overlaps with new/edited- approved standing orders
                //Note : Extensions(New and Edited), Standing orders (New and Edited (approved)) have extensions created on the extension table

                if (_extensionRepository.CheckOverLappingExtensionsAvailableInRange(lstUnits, startDate, endDate, startTime, endTime, frequency, tempExtensions))
                {
                    m_validationErrorLog.AppendLine(commonMessages.overlappingExtensionsAvailable);

                    overlapFound = true;
                }

                //Check if overlaps with Edit-Pending standing orders
                if (!overlapFound)
                {
                    if (m_ValidationRepo.HaveOverlapsWithEditedRequests(tempExtensions, lstUnits))
                    {
                        m_validationErrorLog.AppendLine(commonMessages.overlappingExtensionsAvailable);

                        overlapFound = true;
                    }
                }
            }

            CommonValidationsForExtension(startDate, endDate, startTime, endTime, lstUnits);
        }

        //This method handleds the common alu=idations for the extension. Both for Insert and Updating
        private void CommonValidationsForExtension(DateTime startDate, DateTime endDate, TimeSpan startTime, TimeSpan endTime, List<ExtensionSavingHelperDTO> lstUnits)
        {
            //Extension can be added 30 minutes before start time
            //Note : This takes 30 as the default value. If required can change the magine value from thhe configuration file. Value should be mentioned in minutes
            var timeLowerMagine = m_configuration.GetSection(configurationKeys.lowerTimeMargineforExtensionInMinutes);

            if (timeLowerMagine != null)
            {
                int minimumLowerTimeMargineForExtension = 30;

                if (!int.TryParse(timeLowerMagine.Value, out minimumLowerTimeMargineForExtension))
                {
                    minimumLowerTimeMargineForExtension = 30;
                }

                DateTime timeToBeStarted = startDate.Add(startTime);

                if ((timeToBeStarted - DateTime.Now).TotalMinutes < minimumLowerTimeMargineForExtension)
                {
                    m_validationErrorLog.AppendLine(string.Format(commonMessages.extensionLowerTimeMargineValidationMessage, minimumLowerTimeMargineForExtension));
                }
            }

            //Check for the Working Hours

            var workingHourStartingBoundary = m_configuration.GetSection(configurationKeys.workingHourStatingBoundary);

            int startHour = 7;

            int startMinute = 30;

            TimeSpan expectedLowerMargineTimeValue = new TimeSpan(startHour, startMinute, 0);

            if (workingHourStartingBoundary != null && workingHourStartingBoundary.Value.Length >= 5)
            {
                string[] tempStartingHour = workingHourStartingBoundary.Value.Split(':');

                if (tempStartingHour.Length == 2)
                {
                    if (!int.TryParse(tempStartingHour[0], out startHour))
                    {
                        startHour = 7;
                    }

                    if (!int.TryParse(tempStartingHour[1], out startMinute))
                    {
                        startMinute = 30;
                    }

                    expectedLowerMargineTimeValue = new TimeSpan(startHour, startMinute, 0);//Extension should be ended before this time
                }
            }

            var workingHourEndingBoundary = m_configuration.GetSection(configurationKeys.workingHourEndingBoundary);

            int endHour = 17;

            int endMinute = 30;

            TimeSpan expectedUpperMargineTimeValue = new TimeSpan(endHour, endMinute, 0);

            if (workingHourEndingBoundary != null && workingHourEndingBoundary.Value.Length >= 5)
            {
                string[] tempEndingHour = workingHourEndingBoundary.Value.Split(':');

                if (tempEndingHour.Length == 2)
                {
                    if (tempEndingHour.Length == 2)
                    {
                        if (!int.TryParse(tempEndingHour[0], out endHour))
                        {
                            endHour = 7;
                        }

                        if (!int.TryParse(tempEndingHour[1], out endMinute))
                        {
                            endMinute = 30;
                        }

                        expectedUpperMargineTimeValue = new TimeSpan(endHour, endMinute, 0);//Extension should be ended before this time
                    }
                }
            }

            if (endTime > expectedLowerMargineTimeValue && startTime < expectedLowerMargineTimeValue ||
                startTime >= expectedLowerMargineTimeValue && endTime <= expectedUpperMargineTimeValue ||
                startTime < expectedUpperMargineTimeValue && endTime > expectedUpperMargineTimeValue ||
                endTime > expectedUpperMargineTimeValue && startTime < expectedLowerMargineTimeValue)
            {
                m_validationErrorLog.AppendLine(string.Format(commonMessages.operationHourValidationMessgae, expectedLowerMargineTimeValue, expectedUpperMargineTimeValue));
            }

            //Time intervals should be 30 minutes

            if (startTime <= endTime)
            {
                int minimumTimeIntervalForExtension = 30; //Default value is 30 min

                var configvalue = m_configuration.GetSection(configurationKeys.minimumTimeIntervalForExtension);

                if (configvalue != null)
                {
                    if (!int.TryParse(configvalue.Value, out minimumTimeIntervalForExtension))
                    {
                        minimumTimeIntervalForExtension = 30;
                    }
                }

                if (Math.Abs((endTime - startTime).TotalMinutes % minimumTimeIntervalForExtension) > 0)
                {
                    m_validationErrorLog.AppendLine(commonMessages.timeRangeNotSuportedMessage);
                }
            }

            //Date must be less than a year from today. (can be configured)
            if (startDate > DateTime.Now)
            {
                int maxDateForExtensionInMonths = 12; //Default value in 12 mon

                var configvalue = m_configuration.GetSection(configurationKeys.maxDateForExtensionInMonths);

                if (configvalue != null)
                {
                    if (!int.TryParse(configvalue.Value, out maxDateForExtensionInMonths))
                    {
                        maxDateForExtensionInMonths = 12;
                    }
                }

                if (startDate > DateTime.Now.AddMonths(maxDateForExtensionInMonths))
                {
                    m_validationErrorLog.AppendLine(commonMessages.extensionDateNotSupportedMessage);
                }
            }

            //Maximum duration of standing order should be a year(cnfigurable)

            int maxDurationForStandingOrderInMonths = 12; //Default value in 12 mon

            var stdiMaxDuration = m_configuration.GetSection(configurationKeys.maxDurationForStandingOrderInMonths);

            if (stdiMaxDuration != null)
            {
                if (!int.TryParse(stdiMaxDuration.Value, out maxDurationForStandingOrderInMonths))
                {
                    maxDurationForStandingOrderInMonths = 12;
                }
            }

            if (endDate > startDate.AddMonths(maxDurationForStandingOrderInMonths))
            {
                m_validationErrorLog.AppendLine(string.Format(commonMessages.standingOrderDuration, maxDurationForStandingOrderInMonths));
            }
        }

        public IExtension GetStandingOrdersList(DateTime startDate, DateTime endDate, int page, int pageSize, List<string> lstStatus,
            List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID, string role,
            string sortby = orderingDefinitions.displayUpdatedAt, int ordering = (Int32)orderingModes.descending)
        {
            List<ExtensionSavingHelperDTO> filteredUnits = new List<ExtensionSavingHelperDTO>();

            filteredUnits = m_commonUtilities.loadUnitsAccordingToUser(userID, lstUnits, role);

            return m_SoRepository.GetStandingOrdersList(startDate, endDate, page, pageSize, lstStatus,
                lstRequestTypes, filteredUnits, role, sortby, ordering);
        }

        public IExtension AuthorizeExtension(int mode, List<BulkApprovalHelperDTO> lstExtensionIds, long requestedby, string role)
        {
            IExtension objResult = new StandingOrderResultsDTO();

            objResult = AuthorizeBulkOfExtension(mode, lstExtensionIds, requestedby, role);

            return objResult;
        }

        private IExtension AuthorizeBulkOfExtension(int mode, List<BulkApprovalHelperDTO> lstExtensionIds, long requestedby, string role)
        {
            StandingOrderResultsDTO objResult = new StandingOrderResultsDTO();

            if (lstExtensionIds != null && lstExtensionIds.Count > 0)
            {
                foreach (var item in lstExtensionIds)
                {
                    switch (mode)
                    {
                        case (int)approvalModes.approve://mode = 1

                            ValidateApprovalRequest(item.extensionID, item.requesttypeString);

                            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)
                            {
                                objResult.isValidated = false;

                                objResult.isSucceded = false;

                                objResult.lstValidationError = m_validationErrorLog.ToString();
                            }
                            else
                            {
                                objResult.isSucceded = HandleApprovingProcess(mode, int.Parse(item.requesttype), long.Parse(item.extensionID), requestedby, role);

                                objResult.isSucceded = true;
                            }

                            break;

                        case (int)approvalModes.Reject://mode = 2

                            ValidateRejectionRequest(item.extensionID, item.requesttypeString);

                            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)
                            {
                                objResult.isValidated = false;

                                objResult.isSucceded = false;

                                objResult.lstValidationError = m_validationErrorLog.ToString();
                            }
                            else
                            {
                                objResult.isSucceded = HandleApprovingProcess(mode, int.Parse(item.requesttype), long.Parse(item.extensionID), requestedby, role);

                                objResult.isValidated = true;
                            }

                            break;
                    }

                    if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)
                    {
                        m_validationErrorLog.Clear();
                    }
                }
            }

            return objResult;
        }

        private void ValidateRejectionRequest(string extensionID, string requesttypeString)
        {
            //This validation is to avoid fake rejections.

            IExtension objExtension = m_SoRepository.GetStandingOrderByID(int.Parse(extensionID), requesttypeString);

            if (objExtension != null)
            {
                if (objExtension.selectedExtension != null && objExtension.selectedExtension.Status != null)
                {
                    if (objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Expired) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Rejected) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Completed) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Active)
                       )
                    {
                        m_validationErrorLog.AppendLine(string.Format(commonMessages.soRejectNotAllowableForStatus, objExtension.selectedExtension.Status));
                    }
                }
            }
        }

        private void ValidateApprovalRequest(string extensionID, string requesttypeString)
        {
            //This validation is to avoid fake approvals.

            IExtension objExtension = m_SoRepository.GetStandingOrderByID(int.Parse(extensionID), requesttypeString);

            if (objExtension != null)
            {
                if (objExtension.selectedExtension != null && objExtension.selectedExtension.Status != null)
                {
                    if (objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Expired) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Rejected) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Completed) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Active)
                       )
                    {
                        m_validationErrorLog.AppendLine(string.Format(commonMessages.soApproveNotAllowableForStatus, objExtension.selectedExtension.Status));
                    }
                }
            }
        }

        private bool HandleApprovingProcess(int mode, int type, long extensionId, long requestedby, string role)
        {
            switch (type)
            {
                case (int)approvingTypes.editRequest://type = 1

                    if (role.Equals(Roles.TM))
                    {
                        return m_SoRepository.EditRequestApproval(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return m_SoRepository.EditRequestApproval(extensionId, role, requestedby, mode);
                    }

                    break;

                case (int)approvingTypes.terminateRequest://type = 2

                    if (role.Equals(Roles.TM))
                    {
                        return m_SoRepository.TerminateRequestApproval(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return m_SoRepository.TerminateRequestApproval(extensionId, role, requestedby, mode);
                    }

                    break;

                case (int)approvingTypes.normalExtensionRequest://type = 3

                    if (role.Equals(Roles.TM))
                    {
                        return m_SoRepository.ExtensionApproval(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return m_SoRepository.ExtensionApproval(extensionId, role, requestedby, mode);
                    }

                    break;
            }

            return false;
        }

        //This method validates and terminate an existing extension
        public IExtension TerminateStandingOrder(long id, string extensionID, long createdBy, string role)
        {
            StandingOrderResultsDTO objResult = new StandingOrderResultsDTO();

            ValidateTerminateRequest(id);

            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)
            {
                objResult.isValidated = false;

                objResult.isSucceded = false;

                objResult.lstValidationError = m_validationErrorLog.ToString();
            }
            else
            {
                objResult.standingOrderID = m_SoRepository.TerminateStandingOrder(id, extensionID, createdBy, role);

                objResult.isValidated = true;

                objResult.isSucceded = true;
            }

            return objResult;
        }

        //This Method validate the Extension and inputs for Terminate request
        private void ValidateTerminateRequest(long id)
        {
            IExtension result = m_SoRepository.GetDependanciesForStandingOrders(id, "StandingOrderEditRequests,StandingOrderTerminateRequests");

            if (result != null)
            {
                //Check if Pending Edit/Terminate requests are available with the extension
                if (result.havePendingTerminateRequests)
                {
                    m_validationErrorLog.AppendLine(commonMessages.soPendingTerminateOrEditRequests);
                }
                else if (result.havePendingEditRequests)
                {
                    m_validationErrorLog.AppendLine(commonMessages.soPendingTerminateOrEditRequests);
                }

                //Active, Completed, Expired or Rejected extensions can’t have edit requests
                if (result.extensionStatus.Equals(approvalStatusDeffinitions.Active) ||
                   result.extensionStatus.Equals(approvalStatusDeffinitions.Completed) ||
                   result.extensionStatus.Equals(approvalStatusDeffinitions.Expired) ||
                  result.extensionStatus.Equals(approvalStatusDeffinitions.Rejected))
                {
                    m_validationErrorLog.AppendLine(string.Format(commonMessages.soEditNotAllowableForStatus, result.extensionStatus));
                }
            }
        }

        public IExtension GetStandingOrderByExtensionID(string extensionDisplayID)
        {
            return m_SoRepository.GetStandingOrderByExtensionID(extensionDisplayID);
        }

        public void UpdateStandingOrderStatus(StandingOrderCommonDTO standing_order)
        {
            m_SoRepository.UpdateStandingOrderStatus(standing_order);
        }

        public void SaveStandingOrderHistory(string standingOrderId, string action, int actionBy, DateTime actionAt, DateTime? startDate, DateTime? endDate, TimeSpan? startTime, TimeSpan? endTime, List<ExtensionSavingHelperDTO>? units, string? discription, DateTime createdAt, DateTime updatedAt)
        {
            m_SoRepository.SaveStandingOrderHistory(standingOrderId, action, actionBy, actionAt, startDate, endDate, startTime, endTime, units, discription, createdAt, updatedAt);
        }

        public void UpdateEditedStandingOrderDetails(StandingOrderCommonDTO standing_order)
        {
            m_SoRepository.UpdateEditedStandingOrderDetails(standing_order);
        }

        public void UpdateTerminateStandingOrderDetails(StandingOrderCommonDTO standing_order)
        {
            m_SoRepository.UpdateTerminateStandingOrderDetails(standing_order);
        }
    }
}