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
using SITCAMSClientIntegration.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AES.Application.ApplicationClasses.AesClasses
{
    public class Extensions : IExtensionService
    {
        private readonly IExtensionRepository _extensionRepository;

        private StringBuilder m_validationErrorLog = new StringBuilder();

        private IConfiguration m_configuration;

        private readonly ICommonUtilitiesRepository m_commonUtilities;

        private IStandingOrderRepository m_SoRepository;

        private readonly IValidationRepository m_ValidationRepo;

        public Extensions(IExtensionRepository extRepository, IConfiguration configuration, ICommonUtilitiesRepository commonUtilities,
            IStandingOrderRepository soRepository, IValidationRepository valRepo)
        {
            _extensionRepository = extRepository;

            m_configuration = configuration;

            m_commonUtilities = commonUtilities;

            m_SoRepository = soRepository;

            m_ValidationRepo = valRepo;
        }

        public ExtensionResultsDTO CreateExtension(DateTime effectiveDate, TimeSpan startTime, TimeSpan endTime, string description,
            List<ExtensionSavingHelperDTO> lstUnits, long createdBy, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            //Validates the date and times
            ValidateExtensionInputs(effectiveDate, startTime, endTime, lstUnits);

            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)//If Validation issues are available
            {
                objResult.isValidated = false;

                objResult.isSucceded = false;

                objResult.lstValidationError = m_validationErrorLog.ToString();
            }
            else
            {
                objResult.extensionID = _extensionRepository.CreateExtension(effectiveDate, startTime, endTime, description, lstUnits, createdBy, role);

                objResult.isValidated = true;

                objResult.isSucceded = true;
            }

            return objResult;
        }

        //This method handleds the common alu=idations for the extension. Both for Insert and Updating
        private void CommonValidationsForExtension(DateTime effectiveDate, TimeSpan startTime, TimeSpan endTime, List<ExtensionSavingHelperDTO> lstUnits)
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

                DateTime timeToBeStarted = effectiveDate.Add(startTime);

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
            if (effectiveDate > DateTime.Now)
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

                if (effectiveDate > DateTime.Now.AddMonths(maxDateForExtensionInMonths))
                {
                    m_validationErrorLog.AppendLine(commonMessages.extensionDateNotSupportedMessage);
                }
            }

            //****Can’t have overlapping existing or edit extension for a unit. also not with standing orders. (need to check for all the units in the extension for the specified date)

            if (_extensionRepository.CheckOverLappingExtensionsAvailable(lstUnits, effectiveDate, startTime, endTime))
            {
                m_validationErrorLog.AppendLine(commonMessages.overlappingExtensionsAvailable);
            }

            List<ExtensionCommonDTO> tempExtensions = m_SoRepository.GetVirtualExtensionsForStandingOrder(effectiveDate, effectiveDate, startTime, endTime, soFrequencyDefinitions.daily, null);

            if (m_ValidationRepo.HaveOverlapsWithEditedRequests(tempExtensions, lstUnits))
            {
                m_validationErrorLog.AppendLine(commonMessages.overlappingExtensionsAvailable);
            }
        }

        //This method validates the inputs provided
        private void ValidateExtensionInputs(DateTime effectiveDate, TimeSpan startTime, TimeSpan endTime, List<ExtensionSavingHelperDTO> lstUnits)
        {
            //Date must be tomorrow or after
            if (effectiveDate <= DateTime.Now.AddDays(-1))
            {
                m_validationErrorLog.AppendLine(commonMessages.effectiveDateValidationMessage);
            }

            //Start time < Ending time
            if (startTime >= endTime)
            {
                m_validationErrorLog.AppendLine(commonMessages.invalidTimeRangeValidationMessage);
            }

            if (lstUnits == null || lstUnits.Count == 0)
            {
                m_validationErrorLog.AppendLine(commonMessages.noUnitsValidationMessage);
            }

            CommonValidationsForExtension(effectiveDate, startTime, endTime, lstUnits);

            //****Can’t have overlapping existing or edit extension for a unit. also not with standing orders. (need to check for all the units in the extension for the specified date)

            /*if (_extensionRepository.CheckOverLappingExtensionsAvailable(lstUnits, effectiveDate, startTime, endTime))
            {
                m_validationErrorLog.AppendLine(commonMessages.overlappingExtensionsAvailable);
            }*/

            //Get all active,

            //*****All units must belong to one tenant
        }

        private void FindOverlappings(List<ExtensionSavingHelperDTO> lstUnits)
        {
        }

        //This method returns a list of extensions
        public IExtension GetExtensionsList(DateTime startDate, DateTime endDate, int page, int pageSize, List<string> lstStatus,
            List<string> lstRequestTypes, List<ExtensionSavingHelperDTO> lstUnits, long userID,
            string role, string orderingColumn = orderingDefinitions.displayUpdatedAt, int orderingType = (Int32)orderingModes.descending)
        {
            List<ExtensionSavingHelperDTO> filteredUnits = new List<ExtensionSavingHelperDTO>();

            filteredUnits = m_commonUtilities.loadUnitsAccordingToUser(userID, lstUnits, role);

            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            if (filteredUnits != null && filteredUnits.Count >= 0)//If null receives, not authorize to read the data
            {
                return _extensionRepository.GetExtensionsList(startDate, endDate, page, pageSize, lstStatus, lstRequestTypes, filteredUnits, userID, role, orderingColumn, orderingType);
            }

            return objResult;
        }

        //This Method Validate and Make Edit Request to an Extension
        public ExtensionResultsDTO EditExtension(long id, string extensionID, DateTime newDate, TimeSpan newStartTime, TimeSpan newEndTime,
            long createdBy, string description, List<ExtensionSavingHelperDTO> lstUnits, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            ValidateUpdateRequestInputs(id, newDate, newStartTime, newEndTime, lstUnits);

            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)//If Validation issues are available
            {
                objResult.isValidated = false;

                objResult.isSucceded = false;

                objResult.lstValidationError = m_validationErrorLog.ToString();
            }
            else
            {
                objResult.extensionID = _extensionRepository.EditExtension(id, extensionID, newDate, newStartTime, newEndTime, createdBy, description, lstUnits, role);

                objResult.isValidated = true;

                objResult.isSucceded = true;
            }

            return objResult;
        }

        //This Method validate the Extension and inputs for Update request
        private void ValidateUpdateRequestInputs(long id, DateTime newDate, TimeSpan startTime, TimeSpan endTime, List<ExtensionSavingHelperDTO> lstUnits)
        {
            ExtensionResultsDTO result = _extensionRepository.GetDependanciesForExtension(id, "ExtensionEditRequests,ExtensionTerminateRequests");

            if (result != null)
            {
                //Check if Pending Edit/Terminate requests are available with the extension
                if (result.havePendingTerminateRequests)
                {
                    m_validationErrorLog.AppendLine(commonMessages.pendingTerminateOrEditRequests);
                }
                else if (result.havePendingEditRequests)
                {
                    m_validationErrorLog.AppendLine(commonMessages.pendingTerminateOrEditRequests);
                }

                //Active, Completed, Expired or Rejected extensions can’t have edit requests.
                if (result.extensionStatus.Equals(approvalStatusDeffinitions.Active) ||
                   result.extensionStatus.Equals(approvalStatusDeffinitions.Completed) ||
                   result.extensionStatus.Equals(approvalStatusDeffinitions.Expired) ||
                  result.extensionStatus.Equals(approvalStatusDeffinitions.Rejected))
                {
                    m_validationErrorLog.AppendLine(string.Format(commonMessages.extensionEditNotAllowableForStatus, result.extensionStatus));
                }

                if (result.extensionType != null)
                {
                    if (result.extensionType.ToUpper().Equals(ExtensionTypeDeffinitions.SO))
                    {
                        m_validationErrorLog.AppendLine(commonMessages.extensionTypeNotAllowableForEditing);
                    }
                }
            }

            //Validate the new date
            if (newDate <= DateTime.Now.AddDays(-1))
            {
                m_validationErrorLog.AppendLine(commonMessages.effectiveDateValidationMessage);
            }

            //Validate the new time
            if (startTime > endTime)
            {
                m_validationErrorLog.AppendLine(commonMessages.invalidTimeRangeValidationMessage);
            }

            CommonValidationsForExtension(newDate, startTime, endTime, lstUnits);

            //****Can’t have overlapping existing or edit extension for a unit. also not with standing orders. (need to check for all the units in the extension for the specified date)

            /*if (_extensionRepository.CheckOverLappingExtensionsAvailable(lstUnits, newDate, startTime, endTime))
            {
                m_validationErrorLog.AppendLine(commonMessages.overlappingExtensionsAvailable);
            }*/
        }

        //This method validates and terminate an existing extension
        public ExtensionResultsDTO TerminateExtension(long id, string extensionID, long createdBy, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            ValidateTerminateRequest(id);

            if (m_validationErrorLog != null && m_validationErrorLog.Length > 0)
            {
                objResult.isValidated = false;

                objResult.isSucceded = false;

                objResult.lstValidationError = m_validationErrorLog.ToString();
            }
            else
            {
                objResult.extensionID = _extensionRepository.TerminateExtension(id, extensionID, createdBy, role);

                objResult.isValidated = true;

                objResult.isSucceded = true;
            }

            return objResult;
        }

        //This Method validate the Extension and inputs for Terminate request
        private void ValidateTerminateRequest(long id)
        {
            ExtensionResultsDTO result = _extensionRepository.GetDependanciesForExtension(id, "ExtensionEditRequests,ExtensionTerminateRequests");

            if (result != null)
            {
                //Check if Pending Edit/Terminate requests are available with the extension
                if (result.havePendingTerminateRequests)
                {
                    m_validationErrorLog.AppendLine(commonMessages.pendingTerminateOrEditRequests);
                }
                else if (result.havePendingEditRequests)
                {
                    m_validationErrorLog.AppendLine(commonMessages.pendingTerminateOrEditRequests);
                }

                //Active, Completed, Expired or Rejected extensions can’t have edit requests
                if (result.extensionStatus.Equals(approvalStatusDeffinitions.Active) ||
                   result.extensionStatus.Equals(approvalStatusDeffinitions.Completed) ||
                   result.extensionStatus.Equals(approvalStatusDeffinitions.Expired) ||
                  result.extensionStatus.Equals(approvalStatusDeffinitions.Rejected))
                {
                    m_validationErrorLog.AppendLine(string.Format(commonMessages.extensionEditNotAllowableForStatus, result.extensionStatus));
                }

                if (result.extensionType != null)
                {
                    if (result.extensionType.ToUpper().Equals(ExtensionTypeDeffinitions.SO))
                    {
                        m_validationErrorLog.AppendLine(commonMessages.extensionTypeNotAllowableForTerminate);
                    }
                }
            }
        }

        //This method authorize Extension, EditExtension Request, Terminate Extension Request
        public ExtensionResultsDTO AuthorizeExtension(int mode, List<BulkApprovalHelperDTO> lstExtensionIds, long requestedby, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            objResult = AuthorizeBulkOfExtension(mode, lstExtensionIds, requestedby, role);

            return objResult;
        }

        private ExtensionResultsDTO AuthorizeBulkOfExtension(int mode, List<BulkApprovalHelperDTO> lstExtensionIds, long requestedby, string role)
        {
            ExtensionResultsDTO objResult = new ExtensionResultsDTO();

            if (lstExtensionIds != null && lstExtensionIds.Count > 0)
            {
                foreach (var item in lstExtensionIds)
                {
                    switch (mode)
                    {
                        case (int)approvalModes.approve://mode = 1

                            ValidateApprovalRequest(item.extensionID, item.requesttypeString, role);

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
                                objResult.isSucceded = HandleRejectingProcess(mode, int.Parse(item.requesttype), long.Parse(item.extensionID), requestedby, role);

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

        private void ValidateApprovalRequest(string extensionID, string requesttypeString, string role)
        {
            //This validation is to avoid fake approvals.

            IExtension objExtension = _extensionRepository.GetExtensionsByID(int.Parse(extensionID), requesttypeString);

            if (objExtension != null)
            {
                //if role is "Tenant Manager" and Status is "Approved by TM" no need to approve the record again
                /* if (role.Equals(Roles.TM))
                 {
                     if (objExtension.selectedExtension != null && objExtension.selectedExtension.Status != null)
                     {
                         if (objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.ApprovedByTM))
                         {
                             m_validationErrorLog.AppendLine(string.Format(commonMessages.extensionIsAlreadyApprovedBySameRole, role));
                         }
                     }
                 }*/

                if (objExtension.selectedExtension != null && objExtension.selectedExtension.Status != null)
                {
                    if (objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Expired) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Rejected) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Completed) ||
                       objExtension.selectedExtension.Status.Equals(approvalStatusDeffinitions.Active)
                       )
                    {
                        m_validationErrorLog.AppendLine(string.Format(commonMessages.extensionApproveNotAllowableForStatus, objExtension.selectedExtension.Status));
                    }
                }

                if (objExtension.selectedExtension != null && objExtension.selectedExtension.ExtensionType != null)
                {
                    if (objExtension.selectedExtension.ExtensionType.ToUpper().Equals(ExtensionTypeDeffinitions.SO))
                    {
                        m_validationErrorLog.AppendLine(commonMessages.extensionTypeNotAllowableForApproval);
                    }
                }
            }
        }

        private void ValidateRejectionRequest(string extensionID, string requesttypeString)
        {
            //This validation is to avoid fake rejections.

            IExtension objExtension = _extensionRepository.GetExtensionsByID(int.Parse(extensionID), requesttypeString);

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
                        m_validationErrorLog.AppendLine(string.Format(commonMessages.extensionRejectNotAllowableForStatus, objExtension.selectedExtension.Status));
                    }
                }
                if (objExtension.selectedExtension != null && objExtension.selectedExtension.ExtensionType != null)
                {
                    if (objExtension.selectedExtension.ExtensionType.ToUpper().Equals(ExtensionTypeDeffinitions.SO))
                    {
                        m_validationErrorLog.AppendLine(commonMessages.extensionTypeNotAllowableForApproval);
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
                        return _extensionRepository.EditRequestApproval_TenantManager(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return _extensionRepository.EditRequestApproval_FacilityManager(extensionId, role, requestedby, mode);
                    }

                    break;

                case (int)approvingTypes.terminateRequest://type = 2

                    if (role.Equals(Roles.TM))
                    {
                        return _extensionRepository.TerminateRequestApproval_TenantManager(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return _extensionRepository.TerminateRequestApproval_FacilityManager(extensionId, role, requestedby, mode);
                    }

                    break;

                case (int)approvingTypes.normalExtensionRequest://type = 3

                    if (role.Equals(Roles.TM))
                    {
                        return _extensionRepository.ExtensionApproval_TenantManager(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return _extensionRepository.ExtensionApproval_FacilityManager(extensionId, role, requestedby, mode);
                    }

                    break;
            }

            return false;
        }

        private bool HandleRejectingProcess(int mode, int type, long extensionId, long requestedby, string role)
        {
            switch (type)
            {
                case (int)approvingTypes.editRequest://type = 1

                    if (role.Equals(Roles.TM))
                    {
                        return _extensionRepository.EditRequestApproval_TenantManager(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return _extensionRepository.EditRequestApproval_FacilityManager(extensionId, role, requestedby, mode);
                    }
                    break;

                case (int)approvingTypes.terminateRequest://type = 2

                    if (role.Equals(Roles.TM))
                    {
                        return _extensionRepository.TerminateRequestApproval_TenantManager(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return _extensionRepository.TerminateRequestApproval_FacilityManager(extensionId, role, requestedby, mode);
                    }
                    break;

                case (int)approvingTypes.normalExtensionRequest://type = 3

                    if (role.Equals(Roles.TM))
                    {
                        return _extensionRepository.ExtensionApproval_TenantManager(extensionId, role, requestedby, mode);
                    }
                    else if (role.Equals(Roles.FM))
                    {
                        return _extensionRepository.ExtensionApproval_FacilityManager(extensionId, role, requestedby, mode);
                    }
                    break;
            }

            return false;
        }

        //Get Extension by ID
        public IExtension GetExtensionsByID(int id, string requestType)
        {
            return _extensionRepository.GetExtensionsByID(id, requestType);
        }

        public void UpdateExtensionState(ExtensionCommonDTO extension)
        {
            _extensionRepository.UpdateExtensionStatus(extension);
        }

        public void UpdateEditedExtensionDetails(ExtensionCommonDTO extension)
        {
            _extensionRepository.UpdateEditedExtensionDetails(extension);
        }

        public void UpdateTerminateExtensionDetails(ExtensionCommonDTO extension)
        {
            _extensionRepository.UpdateTerminateExtensionDetails(extension);
        }

        public void SaveExtensionHistory(string extensionId, string action, int actionBy, DateTime actionAt, DateTime? date, TimeSpan? startTime, TimeSpan? endTime, List<ExtensionSavingHelperDTO>? units, string? discription, DateTime createdAt, DateTime updatedAt)
        {
            _extensionRepository.SaveExtensionHistory(extensionId, action, actionBy, actionAt, date, startTime, endTime, units, discription, createdAt, updatedAt);
        }

        public IExtension GetExtensionsNeedActivating(int days)
        {
            return _extensionRepository.GetExtensionsNeedActivating(days);
        }

        public void ActivateExtension(long id)
        {
            _extensionRepository.ActivateExtension(id);
        }
    }
}