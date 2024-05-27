using AES.Application.Interfaces.RepositoryInterfaces.ActivityLogInterfaces;
using AES.Application.Interfaces.RepositoryInterfaces.UserOperationInterfaces;
using AES.Domain.Definitions;
using Application.Interfaces.ServiceInterfaces.AesServiceInterfaces;
using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.ConfigurationModels;
using CommonInsfrastructure.Models.EmailModels;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using Domain.DataTransferringObjects.UserDTOs;
using LoggerLibrary.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SITCAMSClientIntegration.Configurations;
using System.Text;

namespace Application.BackgroudWokers.AESBackgroundWokers
{
    public class BackgroundStatusManagingService : BackgroundService
    {
        private readonly ILogService m_logService;
        private readonly IServiceProvider m_serviceProvider;
        private readonly IConfiguration m_configuration;

        //private readonly IEmailConfigurations m_emailService;
        private string? emailBody;

        public BackgroundStatusManagingService(ILogService logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            m_logService = logger;
            m_serviceProvider = serviceProvider;
            m_configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            m_logService.Log("Extensions managing service is starting");

            //looping interval
            int interval = (int)(m_configuration.GetValue<double>("BGTasks:ExtensionApprovalInterval") * 60 * 1000);

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = m_serviceProvider.CreateScope();
                var m_extService = scope.ServiceProvider.GetRequiredService<IExtensionService>();
                var m_soService = scope.ServiceProvider.GetRequiredService<IStandingOrderService>();
                var m_activityLog = scope.ServiceProvider.GetRequiredService<IActivityLogRepository>();
                var m_userInfo = scope.ServiceProvider.GetRequiredService<IUserOperationsRepository>();
                var m_emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var m_configRepo = scope.ServiceProvider.GetRequiredService<ICommonConfigRepository>();

                //get background task log status
                bool logBackgrondTask = m_configuration.GetValue<bool>("BGTasks:logBackgrondTask");

                string[] keys = { "NETM", "NEFM", "EETM", "EEFM", "TETM", "TEFM", "NSTM", "NSFM", "ESTM", "ESFM", "TSTM", "TSFM", "NEA", "EEA", "TEA", "NSA", "ESA", "TSA", "NEE", "EEE", "TEE", "NSE", "ESE", "TSE" };
                var settings = m_configRepo.GetAESCommonSettingBulk(keys);

                //approval parameters - read AES configuration parameters from the config database
                bool isNewExtTMapprovalAuto = GetSettingValue(settings, "NETM");
                bool isNewExtFMapprovalAuto = GetSettingValue(settings, "NEFM");
                bool isEditExtTMapprovalAuto = GetSettingValue(settings, "EETM");
                bool isEditExtFMapprovalAuto = GetSettingValue(settings, "EEFM");
                bool isTermExtTMapprovalAuto = GetSettingValue(settings, "TETM");
                bool isTermExtFMapprovalAuto = GetSettingValue(settings, "TEFM");
                bool isNewSOTMapprovalAuto = GetSettingValue(settings, "NSTM");
                bool isNewSOFMapprovalAuto = GetSettingValue(settings, "NSFM");
                bool isEditSOTMapprovalAuto = GetSettingValue(settings, "ESTM");
                bool isEditSOFMapprovalAuto = GetSettingValue(settings, "ESFM");
                bool isTermSOTMapprovalAuto = GetSettingValue(settings, "TSTM");
                bool isTermSOFMapprovalAuto = GetSettingValue(settings, "TSFM");

                //email config parameters - they will read from the database in future
                bool isNewExtApproveEmail = GetSettingValue(settings, "NEA");
                bool isEditExtApproveEmail = GetSettingValue(settings, "EEA");
                bool isTermExtApproveEmail = GetSettingValue(settings, "TEA");
                bool isNewSOApproveEmail = GetSettingValue(settings, "NSA");
                bool isEditSOApproveEmail = GetSettingValue(settings, "ESA");
                bool isTermSOApproveEmail = GetSettingValue(settings, "TSA");
                bool isNewExtExpiredEmail = GetSettingValue(settings, "NEE");
                bool isEditExtExpiredEmail = GetSettingValue(settings, "EEE");
                bool isTermExtExpiredEmail = GetSettingValue(settings, "TEE");
                bool isNewSOExpiredEmail = GetSettingValue(settings, "NSE");
                bool isEditSOExpiredEmail = GetSettingValue(settings, "ESE");
                bool isTermSOExpiredEmail = GetSettingValue(settings, "TSE");

                ExtensionResultsDTO extensions = new();
                StandingOrderResultsDTO standing_orders = new();
                UserNameDTO userDetail = new();

                List<ExtensionSavingHelperDTO> lstUnits = new();
                var reques_type_new = new List<string>() { RequestTypeDeffinitions.New };// { "New" };
                var reques_type_edit = new List<string>() { RequestTypeDeffinitions.Edit }; // { "Edit" };
                var reques_type_terminate = new List<string>() { RequestTypeDeffinitions.Terminate }; // { "Terminate" };
                var status_pending = new List<string>() { approvalStatusDeffinitions.Pending }; // { "pending" };
                var status_tm_approved = new List<string>() { approvalStatusDeffinitions.ApprovedByTM }; // { "Approved by TM" };
                var status_fm_approved = new List<string>() { approvalStatusDeffinitions.ApprovedByFM }; // { "Approved by FM" };
                var status_approved = new List<string>() { approvalStatusDeffinitions.Approved }; // { "Accepted" };
                var status_active = new List<string>() { approvalStatusDeffinitions.Active }; // { "Active" };
                var status_all = new List<string>() { approvalStatusDeffinitions.SelectAll }; // { "All" };

                //read email templete file
                using (var streamReader = new StreamReader(m_configuration.GetValue<string>("EmailTemplatePath"), Encoding.UTF8))
                {
                    emailBody = streamReader.ReadToEnd();
                }

                try
                {
                    // check for pending new extensions and auto approve them for TM
                    if (isNewExtTMapprovalAuto)
                    {
                        // pending status - extensions posted by Tenant, and waiting for TM approval
                        m_logService.Log("getting all pending extensions", null, logBackgrondTask);
                        extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_pending, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataExt(extensions))
                        {
                            m_logService.Log("Found " + extensions.dataCount + " pending extensions", null, logBackgrondTask);

                            foreach (var extension in extensions.lstExtensionList)
                            {
                                if (extension.ExtensionType == ExtensionTypeDeffinitions.EXT)
                                {
                                    m_logService.Log("Processing extension " + extension.ExtensionId);
                                    extension.TmApproval = true;
                                    extension.Status = approvalStatusDeffinitions.ApprovedByTM;
                                    m_extService.UpdateExtensionState(extension);
                                }
                            }
                        }
                        else m_logService.Log("No pending extensions found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto TM approval for new extensions is not enabled", null, logBackgrondTask);

                    // check for TM approved new extensions and auto approve them for FM
                    if (isNewExtFMapprovalAuto)
                    {
                        // tm_approved status - extensions approved by TM / Posted by TM, and waiting for FM approval
                        m_logService.Log("getting all tm approved extensions", null, logBackgrondTask);
                        extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_tm_approved, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataExt(extensions))
                        {
                            m_logService.Log("Found " + extensions.dataCount + " TM approved extensions", null, logBackgrondTask);

                            foreach (var extension in extensions.lstExtensionList)
                            {
                                if (extension.ExtensionType == ExtensionTypeDeffinitions.EXT)
                                {
                                    m_logService.Log("Processing extension " + extension.ExtensionId);
                                    extension.FmApproval = true;
                                    extension.Status = approvalStatusDeffinitions.ApprovedByFM;
                                    m_extService.UpdateExtensionState(extension);
                                }
                            }
                        }
                        else m_logService.Log("No TM approved extensions found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto FM approval for new extensions is not enabled", null, logBackgrondTask);

                    // check for pending edit extensions requests and auto approve them for TM
                    if (isEditExtTMapprovalAuto)
                    {
                        // pending status - extensions edit posted by Tenant, and waiting for TM approval
                        m_logService.Log("getting all pending edit extensions", null, logBackgrondTask);
                        extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_pending, reques_type_edit, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataExt(extensions))
                        {
                            m_logService.Log("Found " + extensions.dataCount + " pending edit extensions", null, logBackgrondTask);

                            foreach (var extension in extensions.lstExtensionList)
                            {
                                if (extension.Extension.ExtensionType == ExtensionTypeDeffinitions.EXT)
                                {
                                    m_logService.Log("Processing extension " + extension.ExtensionId);
                                    extension.TmApproval = true;
                                    extension.Status = approvalStatusDeffinitions.ApprovedByTM;
                                    m_extService.UpdateExtensionState(extension);
                                }
                            }
                        }
                        else m_logService.Log("No pending edit extensions found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto TM approval for edit extensions is not enabled", null, logBackgrondTask);

                    // check for TM approved edit extensions and auto approve them for FM
                    if (isEditExtFMapprovalAuto)
                    {
                        // tm_approved status - extensions edit approved by TM / Posted by TM, and waiting for FM approval
                        m_logService.Log("getting all tm approved edit extensions", null, logBackgrondTask);
                        extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_tm_approved, reques_type_edit, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataExt(extensions))
                        {
                            m_logService.Log("Found " + extensions.dataCount + " TM approved edit extensions", null, logBackgrondTask);

                            foreach (var extension in extensions.lstExtensionList)
                            {
                                if (extension.Extension.ExtensionType == ExtensionTypeDeffinitions.EXT)
                                {
                                    m_logService.Log("Processing extension " + extension.ExtensionId);
                                    extension.FmApproval = true;
                                    extension.Status = approvalStatusDeffinitions.ApprovedByFM;
                                    m_extService.UpdateExtensionState(extension);
                                }
                            }
                        }
                        else m_logService.Log("No TM approved edit extensions found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto FM approval for edit extensions is not enabled", null, logBackgrondTask);

                    // check for pending terminate extensions requests and auto approve them for TM
                    if (isTermExtTMapprovalAuto)
                    {
                        // pending status - extensions terminate posted by Tenant, and waiting for TM approval
                        m_logService.Log("getting all pending delete extensions", null, logBackgrondTask);
                        extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_pending, reques_type_terminate, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataExt(extensions))
                        {
                            m_logService.Log("Found " + extensions.dataCount + " pending delete extensions", null, logBackgrondTask);

                            foreach (var extension in extensions.lstExtensionList)
                            {
                                if (extension.Extension.ExtensionType == ExtensionTypeDeffinitions.EXT)
                                {
                                    m_logService.Log("Processing extension " + extension.ExtensionId);
                                    extension.TmApproval = true;
                                    extension.Status = approvalStatusDeffinitions.ApprovedByTM;
                                    m_extService.UpdateExtensionState(extension);
                                }
                            }
                        }
                        else m_logService.Log("No pending delete extensions found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto TM approval for delete extensions is not enabled", null, logBackgrondTask);

                    // check for TM approved terminate extensions and auto approve them for FM
                    if (isTermExtFMapprovalAuto)
                    {
                        // tm_approved status - extensions terminate approved by TM / Posted by TM, and waiting for FM approval
                        m_logService.Log("getting all tm approved delete extensions", null, logBackgrondTask);
                        extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_tm_approved, reques_type_terminate, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataExt(extensions))
                        {
                            m_logService.Log("Found " + extensions.dataCount + " TM approved delete extensions", null, logBackgrondTask);

                            foreach (var extension in extensions.lstExtensionList)
                            {
                                if (extension.Extension.ExtensionType == ExtensionTypeDeffinitions.EXT)
                                {
                                    m_logService.Log("Processing extension " + extension.ExtensionId);
                                    extension.FmApproval = true;
                                    extension.Status = approvalStatusDeffinitions.ApprovedByFM;
                                    m_extService.UpdateExtensionState(extension);
                                }
                            }
                        }
                        else m_logService.Log("No TM approved delete extensions found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto FM approval for delete extensions is not enabled", null, logBackgrondTask);

                    // check for pending new standing orders and auto approve them for TM
                    if (isNewSOTMapprovalAuto)
                    {
                        // pending status - standing orders posted by Tenant, and waiting for TM approval
                        m_logService.Log("getting all pending new standing orders", null, logBackgrondTask);
                        standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_pending, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataSO(standing_orders))
                        {
                            m_logService.Log("Found " + standing_orders.dataCount + " pending new standing orders", null, logBackgrondTask);

                            foreach (var standing_order in standing_orders.lstStandingOredrs)
                            {
                                m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                                standing_order.TmApproval = true;
                                standing_order.Status = approvalStatusDeffinitions.ApprovedByTM;
                                m_soService.UpdateStandingOrderStatus(standing_order);
                            }
                        }
                        else m_logService.Log("No pending new standing orders found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto TM approval for new standing orders is not enabled", null, logBackgrondTask);

                    // check for TM approved new standing orders and auto approve them for FM
                    if (isNewSOFMapprovalAuto)
                    {
                        // tm_approved status - standing orders new approved by TM / Posted by TM, and waiting for FM approval
                        m_logService.Log("getting all tm approved new standing orders", null, logBackgrondTask);
                        standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_tm_approved, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataSO(standing_orders))
                        {
                            m_logService.Log("Found " + standing_orders.dataCount + " TM approved new standing orders", null, logBackgrondTask);

                            foreach (var standing_order in standing_orders.lstStandingOredrs)
                            {
                                m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                                standing_order.FmApproval = true;
                                standing_order.Status = approvalStatusDeffinitions.ApprovedByFM;
                                m_soService.UpdateStandingOrderStatus(standing_order);
                            }
                        }
                        else m_logService.Log("No TM approved new standing orders found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto FM approval for new standing orders is not enabled", null, logBackgrondTask);

                    // check for pending edit standing orders and auto approve them for TM
                    if (isEditSOTMapprovalAuto)
                    {
                        // pending status - standing orders edit posted by Tenant, and waiting for TM approval
                        m_logService.Log("getting all pending edit standing orders", null, logBackgrondTask);
                        standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_pending, reques_type_edit, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataSO(standing_orders))
                        {
                            m_logService.Log("Found " + standing_orders.dataCount + " pending edit standing orders", null, logBackgrondTask);

                            foreach (var standing_order in standing_orders.lstStandingOredrs)
                            {
                                m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                                standing_order.TmApproval = true;
                                standing_order.Status = approvalStatusDeffinitions.ApprovedByTM;
                                m_soService.UpdateStandingOrderStatus(standing_order);
                            }
                        }
                        else m_logService.Log("No pending edit standing orders found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto TM approval for edit standing orders is not enabled", null, logBackgrondTask);

                    // check for TM approved edit standing orders and auto approve them for FM
                    if (isEditSOFMapprovalAuto)
                    {
                        // tm_approved status - standing orders edit approved by TM / Posted by TM, and waiting for FM approval
                        m_logService.Log("getting all tm approved edit standing orders", null, logBackgrondTask);
                        standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_tm_approved, reques_type_edit, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataSO(standing_orders))
                        {
                            m_logService.Log("Found " + standing_orders.dataCount + " TM approved edit standing orders", null, logBackgrondTask);

                            foreach (var standing_order in standing_orders.lstStandingOredrs)
                            {
                                m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                                standing_order.FmApproval = true;
                                standing_order.Status = approvalStatusDeffinitions.ApprovedByFM;
                                m_soService.UpdateStandingOrderStatus(standing_order);
                            }
                        }
                        else m_logService.Log("No TM approved edit standing orders found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto FM approval for edit standing orders is not enabled", null, logBackgrondTask);

                    // check for pending terminate standing orders and auto approve them for TM
                    if (isTermSOTMapprovalAuto)
                    {
                        // pending status - standing orders terminate posted by Tenant, and waiting for TM approval
                        m_logService.Log("getting all pending terminate standing orders", null, logBackgrondTask);
                        standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_pending, reques_type_terminate, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataSO(standing_orders))
                        {
                            m_logService.Log("Found " + standing_orders.dataCount + " pending terminate standing orders", null, logBackgrondTask);

                            foreach (var standing_order in standing_orders.lstStandingOredrs)
                            {
                                m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                                standing_order.TmApproval = true;
                                standing_order.Status = approvalStatusDeffinitions.ApprovedByTM;
                                m_soService.UpdateStandingOrderStatus(standing_order);
                            }
                        }
                        else m_logService.Log("No pending terminate standing orders found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto TM approval for terminate standing orders is not enabled", null, logBackgrondTask);

                    // check for TM approved terminate standing orders and auto approve them for FM
                    if (isTermSOFMapprovalAuto)
                    {
                        // tm_approved status - standing orders terminate approved by TM / Posted by TM, and waiting for FM approval
                        m_logService.Log("getting all tm approved terminate standing orders", null, logBackgrondTask);
                        standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_tm_approved, reques_type_terminate, lstUnits, -1, RoleConfig.aesAdmin);

                        if (HasDataSO(standing_orders))
                        {
                            m_logService.Log("Found " + standing_orders.dataCount + " TM approved terminate standing orders", null, logBackgrondTask);

                            foreach (var standing_order in standing_orders.lstStandingOredrs)
                            {
                                m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                                standing_order.FmApproval = true;
                                standing_order.Status = approvalStatusDeffinitions.ApprovedByFM;
                                m_soService.UpdateStandingOrderStatus(standing_order);
                            }
                        }
                        else m_logService.Log("No TM approved terminate standing orders found", null, logBackgrondTask);
                    }
                    else m_logService.Log("Auto FM approval for terminate standing orders is not enabled", null, logBackgrondTask);

                    // this section is for the fm approved new extension
                    // check to FM approved new extensions and accept them
                    // fm_approved status - extensions approved by FM / Posted by FM, and waiting for acceptance
                    m_logService.Log("getting all fm approved extensions", null, logBackgrondTask);
                    extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_fm_approved, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataExt(extensions))
                    {
                        m_logService.Log("Found " + extensions.dataCount + " FM approved extensions", null, logBackgrondTask);

                        foreach (var extension in extensions.lstExtensionList)
                        {
                            m_logService.Log("Processing extension " + extension.ExtensionId);
                            extension.Status = approvalStatusDeffinitions.Approved;
                            m_extService.UpdateExtensionState(extension);
                            m_extService.SaveExtensionHistory(extension.ExtensionId, "accepted", -1, DateTime.Now, null, null, null, null, null, DateTime.Now, DateTime.Now);
                            m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "accepted", extension.ExtensionId.ToString(), "", DateTime.Now, DateTime.Now);

                            //getting user details
                            userDetail = m_userInfo.GetUserInfo(extension.RequestBy);
                            if (userDetail != null && userDetail.Email != null && isNewExtApproveEmail)
                            {
                                string name = userDetail.Fname;
                                string subject = "Your Extension has Accepted";
                                string line1 = "Your new Extension has Accepted";
                                string line2 = "Extension ID :" + extension.ExtensionId;
                                string company = m_configuration.GetValue<string>("CompanyName");
                                TblEmailList email = new TblEmailList();
                                email.ToList = userDetail.Email;
                                email.Subject = subject;
                                email.Status = "N";
                                email.GeneratedTime = DateTime.Now;
                                email.Body = emailBody.Replace("@subject", subject).Replace("@name_of_the_user", name).Replace("@message_body_line1", line1).Replace("@message_body_line2", line2).Replace("@company_name", company);
                                m_emailService.SendEmail(email);
                                m_logService.Log(string.Format("Send an email to {0}", userDetail.Email));
                            }
                        }
                    }
                    else m_logService.Log("No FM approved extensions found", null, logBackgrondTask);

                    // this section is for the fm approved edit extension
                    // check to FM approved edit extensions and accept them
                    // fm_approved status - extensions approved by FM / Posted by FM, and waiting for acceptance
                    // here extensions mean edit extensions
                    m_logService.Log("getting all fm approved edit extensions", null, logBackgrondTask);
                    extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_fm_approved, reques_type_edit, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataExt(extensions))
                    {
                        m_logService.Log("Found " + extensions.dataCount + " FM approved edit extensions", null, logBackgrondTask);

                        foreach (var extension in extensions.lstExtensionList)
                        {
                            m_logService.Log("Processing extension " + extension.ExtensionId);
                            extension.Status = approvalStatusDeffinitions.Approved;
                            m_extService.UpdateExtensionState(extension);
                            m_extService.UpdateEditedExtensionDetails(extension);
                            m_extService.SaveExtensionHistory(extension.Extension.ExtensionId, "accepted", -1, DateTime.Now, null, null, null, null, null, DateTime.Now, DateTime.Now);
                            m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "accepted", extension.Extension.ExtensionId.ToString(), "", DateTime.Now, DateTime.Now);

                            //getting user details
                            userDetail = m_userInfo.GetUserInfo(extension.RequestBy);
                            if (userDetail != null && userDetail.Email != null && isEditExtApproveEmail)
                            {
                                string name = userDetail.Fname;
                                string subject = "Your Extension has Accepted";
                                string line1 = "Your Extension edit request has Accepted";
                                string line2 = "Extension ID :" + extension.ParentExtensionID;
                                string company = m_configuration.GetValue<string>("CompanyName");
                                TblEmailList email = new TblEmailList();
                                email.ToList = userDetail.Email;
                                email.Status = "N";
                                email.GeneratedTime = DateTime.Now;
                                email.Subject = subject;
                                email.Body = emailBody.Replace("@subject", subject).Replace("@name_of_the_user", name).Replace("@message_body_line1", line1).Replace("@message_body_line2", line2).Replace("@company_name", company);
                                m_emailService.SendEmail(email);
                                m_logService.Log(string.Format("Send an email to {0}", userDetail.Email));
                            }
                        }
                    }
                    else m_logService.Log("No FM approved edit extensions found", null, logBackgrondTask);

                    // this section is for the fm approved terminate extension
                    // check to FM approved terminate extensions and accept them
                    // fm_approved status - extensions approved by FM / Posted by FM, and waiting for acceptance
                    // here extensions mean terminate extensions
                    m_logService.Log("getting all fm approved terminate extensions", null, logBackgrondTask);
                    extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_fm_approved, reques_type_terminate, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataExt(extensions))
                    {
                        m_logService.Log("Found " + extensions.dataCount + " FM approved terminate extensions", null, logBackgrondTask);

                        foreach (var extension in extensions.lstExtensionList)
                        {
                            m_logService.Log("Processing extension " + extension.ExtensionId);
                            extension.Status = approvalStatusDeffinitions.Approved;
                            m_extService.UpdateExtensionState(extension);
                            m_extService.UpdateTerminateExtensionDetails(extension);
                            m_extService.SaveExtensionHistory(extension.Extension.ExtensionId, "accepted", -1, DateTime.Now, null, null, null, null, null, DateTime.Now, DateTime.Now);
                            m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "accepted", extension.Extension.ExtensionId.ToString(), "", DateTime.Now, DateTime.Now);

                            //getting user details
                            userDetail = m_userInfo.GetUserInfo(extension.RequestBy);
                            if (userDetail != null && userDetail.Email != null && isTermExtApproveEmail)
                            {
                                string name = userDetail.Fname;
                                string subject = "Your Extension has Accepted";
                                string line1 = "Your Extension termination request has Accepted";
                                string line2 = "Extension ID :" + extension.ParentExtensionID;
                                string company = m_configuration.GetValue<string>("CompanyName");
                                TblEmailList email = new TblEmailList();
                                email.ToList = userDetail.Email;
                                email.Status = "N";
                                email.GeneratedTime = DateTime.Now;
                                email.Subject = subject;
                                email.Body = emailBody.Replace("@subject", subject).Replace("@name_of_the_user", name).Replace("@message_body_line1", line1).Replace("@message_body_line2", line2).Replace("@company_name", company);
                                m_emailService.SendEmail(email);
                                m_logService.Log(string.Format("Send an email to {0}", userDetail.Email));
                            }
                        }
                    }
                    else m_logService.Log("No FM approved terminate extensions found", null, logBackgrondTask);

                    // this section is for the fm approved new standing orders
                    // check to FM approved new standing orders and accept them
                    // fm_approved status - standing orders approved by FM / Posted by FM, and waiting for acceptance
                    m_logService.Log("getting all fm approved new standing orders", null, logBackgrondTask);
                    standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_fm_approved, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataSO(standing_orders))
                    {
                        m_logService.Log("Found " + standing_orders.dataCount + " FM approved new standing orders", null, logBackgrondTask);

                        foreach (var standing_order in standing_orders.lstStandingOredrs)
                        {
                            m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                            standing_order.Status = approvalStatusDeffinitions.Approved;
                            m_soService.UpdateStandingOrderStatus(standing_order);
                            m_soService.SaveStandingOrderHistory(standing_order.StandingOrderId, "accepted", -1, DateTime.Now, null, null, null, null, null, null, DateTime.Now, DateTime.Now);
                            m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "accepted", "", standing_order.StandingOrderId.ToString(), DateTime.Now, DateTime.Now);

                            //getting user details
                            userDetail = m_userInfo.GetUserInfo(standing_order.RequestBy);
                            if (userDetail != null && userDetail.Email != null && isNewSOApproveEmail)
                            {
                                string name = userDetail.Fname;
                                string subject = "Your Standing Order has Accepted";
                                string line1 = "Your new Standing Order has Accepted";
                                string line2 = "Standing Order ID :" + standing_order.StandingOrderId;
                                string company = m_configuration.GetValue<string>("CompanyName");
                                TblEmailList email = new TblEmailList();
                                email.ToList = userDetail.Email;
                                email.Status = "N";
                                email.GeneratedTime = DateTime.Now;
                                email.Subject = subject;
                                email.Body = emailBody.Replace("@subject", subject).Replace("@name_of_the_user", name).Replace("@message_body_line1", line1).Replace("@message_body_line2", line2).Replace("@company_name", company);
                                m_emailService.SendEmail(email);
                                m_logService.Log(string.Format("Send an email to {0}", userDetail.Email));
                            }
                        }
                    }
                    else m_logService.Log("No FM approved new standing orders found", null, logBackgrondTask);

                    // this section is for the fm approved edit standing orders
                    // check to FM approved edit standing orders and accept them
                    // fm_approved status - standing orders approved by FM / Posted by FM, and waiting for acceptance
                    m_logService.Log("getting all fm approved edit standing orders", null, logBackgrondTask);
                    standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_fm_approved, reques_type_edit, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataSO(standing_orders))
                    {
                        m_logService.Log("Found " + standing_orders.dataCount + " FM approved edit standing orders", null, logBackgrondTask);

                        foreach (var standing_order in standing_orders.lstStandingOredrs)
                        {
                            m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                            standing_order.Status = approvalStatusDeffinitions.Approved;
                            m_soService.UpdateStandingOrderStatus(standing_order);
                            m_soService.UpdateEditedStandingOrderDetails(standing_order);
                            m_soService.SaveStandingOrderHistory(standing_order.StandingOrderId, "accepted", -1, DateTime.Now, null, null, null, null, null, null, DateTime.Now, DateTime.Now);
                            m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "accepted", "", standing_order.parentStandingOrderID.ToString(), DateTime.Now, DateTime.Now);

                            //getting user details
                            userDetail = m_userInfo.GetUserInfo(standing_order.RequestBy);
                            if (userDetail != null && userDetail.Email != null && isEditSOApproveEmail)
                            {
                                string name = userDetail.Fname;
                                string subject = "Your Standing Order has Accepted";
                                string line1 = "Your Standing Order edit request has Accepted";
                                string line2 = "Standing Order ID :" + standing_order.parentStandingOrderID;
                                string company = m_configuration.GetValue<string>("CompanyName");
                                TblEmailList email = new TblEmailList();
                                email.ToList = userDetail.Email;
                                email.Status = "N";
                                email.GeneratedTime = DateTime.Now;
                                email.Subject = subject;
                                email.Body = emailBody.Replace("@subject", subject).Replace("@name_of_the_user", name).Replace("@message_body_line1", line1).Replace("@message_body_line2", line2).Replace("@company_name", company);
                                m_emailService.SendEmail(email);
                                m_logService.Log(string.Format("Send an email to {0}", userDetail.Email));
                            }
                        }
                    }
                    else m_logService.Log("No FM approved edit standing orders found", null, logBackgrondTask);

                    // this section is for the fm approved terminate standing orders
                    // check to FM approved terminate standing orders and accept them
                    // fm_approved status - standing orders approved by FM / Posted by FM, and waiting for acceptance
                    m_logService.Log("getting all fm approved terminate standing orders", null, logBackgrondTask);
                    standing_orders = (StandingOrderResultsDTO)m_soService.GetStandingOrdersList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_fm_approved, reques_type_terminate, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataSO(standing_orders))
                    {
                        m_logService.Log("Found " + standing_orders.dataCount + " FM approved terminate standing orders", null, logBackgrondTask);

                        foreach (var standing_order in standing_orders.lstStandingOredrs)
                        {
                            m_logService.Log("Processing standing order " + standing_order.StandingOrderId);
                            standing_order.Status = approvalStatusDeffinitions.Approved;
                            m_soService.UpdateStandingOrderStatus(standing_order);
                            m_soService.UpdateTerminateStandingOrderDetails(standing_order);
                            m_soService.SaveStandingOrderHistory(standing_order.StandingOrderId, "accepted", -1, DateTime.Now, null, null, null, null, null, null, DateTime.Now, DateTime.Now);
                            m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "accepted", "", standing_order.parentStandingOrderID.ToString(), DateTime.Now, DateTime.Now);

                            //getting user details
                            userDetail = m_userInfo.GetUserInfo(standing_order.RequestBy);
                            if (userDetail != null && userDetail.Email != null && isTermSOApproveEmail)
                            {
                                string name = userDetail.Fname;
                                string subject = "Your Standing Order has Accepted";
                                string line1 = "Your Standing Order termination request has Accepted";
                                string line2 = "Standing Order ID :" + standing_order.parentStandingOrderID;
                                string company = m_configuration.GetValue<string>("CompanyName");
                                TblEmailList email = new TblEmailList();
                                email.ToList = userDetail.Email;
                                email.Status = "N";
                                email.GeneratedTime = DateTime.Now;
                                email.Subject = subject;
                                email.Body = emailBody.Replace("@subject", subject).Replace("@name_of_the_user", name).Replace("@message_body_line1", line1).Replace("@message_body_line2", line2).Replace("@company_name", company);
                                m_emailService.SendEmail(email);
                                m_logService.Log(string.Format("Send an email to {0}", userDetail.Email));
                            }
                        }
                    }
                    else m_logService.Log("No FM approved terminate standing orders found", null, logBackgrondTask);

                    // this section is for activate extensions which already started
                    // check for approved and activated extensions and mark them activate
                    // approved status - extensions accepted by system
                    m_logService.Log("getting all accepted extensions", null, logBackgrondTask);
                    extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_approved, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataExt(extensions))
                    {
                        m_logService.Log("Found " + extensions.dataCount + " accepted extensions", null, logBackgrondTask);

                        foreach (var extension in extensions.lstExtensionList)
                        {
                            //check if the extensions is activated and started
                            if (extension.IsActivated == true && extension.Date.Add(extension.StartTime) >= DateTime.Now)
                            {
                                m_logService.Log("Processing extension " + extension.ExtensionId);
                                extension.Status = approvalStatusDeffinitions.Active;
                                m_extService.UpdateExtensionState(extension);
                                m_extService.SaveExtensionHistory(extension.ExtensionId, "activated", -1, DateTime.Now, null, null, null, null, null, DateTime.Now, DateTime.Now);
                                m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "activated", extension.ExtensionId.ToString(), "", DateTime.Now, DateTime.Now);
                            }
                        }
                    }
                    else m_logService.Log("No accepted extensions found", null, logBackgrondTask);

                    // this section is for complete extensions which already completed
                    // check for active extensions and mark them completed
                    // active status - extensions active by system
                    m_logService.Log("getting all active extensions", null, logBackgrondTask);
                    extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now, DateTime.Now.AddYears(1), 1, 1000, status_active, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);
                    if (HasDataExt(extensions))
                    {
                        m_logService.Log("Found " + extensions.dataCount + " active extensions", null, logBackgrondTask);

                        foreach (var extension in extensions.lstExtensionList)
                        {
                            //check if the extensions is completed
                            if (extension.Date.Add(extension.EndTime) <= DateTime.Now)
                            {
                                m_logService.Log("Processing extension " + extension.ExtensionId);
                                extension.Status = approvalStatusDeffinitions.Completed;
                                m_extService.UpdateExtensionState(extension);
                                m_extService.SaveExtensionHistory(extension.ExtensionId, "completed", -1, DateTime.Now, null, null, null, null, null, DateTime.Now, DateTime.Now);
                                m_activityLog.InsertActivityLog(-1, RoleConfig.aesAdmin, "completed", extension.ExtensionId.ToString(), "", DateTime.Now, DateTime.Now);
                            }
                        }
                    }
                    else m_logService.Log("No active extensions found", null, logBackgrondTask);

                    // this section is for expire extensions which never accepted and passed the time
                    // check for pending extensions and mark them expired
                    // pending status - extensions posted by Tenant, and waiting for acceptance
                    //m_logService.Log("getting all time passed pending extensions", null, logBackgrondTask);
                    //extensions = (ExtensionResultsDTO)m_extService.GetExtensionsList(DateTime.Now.AddYears(-1), DateTime.Now, 1, 1000, status_pending, reques_type_new, lstUnits, -1, RoleConfig.aesAdmin);
                }
                catch (Exception ex)
                {
                    m_logService.Log("Error while running the extension managing service", "error");
                    m_logService.Log(ex.ToString(), "error");
                    m_logService.Log("Restarting the service", "error");
                    await Task.Delay(interval, stoppingToken);
                    continue;
                }

                await Task.Delay(interval, stoppingToken);
            }
        }

        private static bool HasDataExt(ExtensionResultsDTO extensions)
        {
            if (extensions == null || extensions.lstExtensionList == null || extensions.dataCount == 0)
            {
                return false;
            }
            return true;
        }

        private static bool HasDataSO(StandingOrderResultsDTO standing_orders)
        {
            if (standing_orders == null || standing_orders.lstStandingOredrs == null || standing_orders.dataCount == 0)
            {
                return false;
            }
            return true;
        }

        private static bool GetSettingValue(List<AescommonSetting> settings, string key)
        {
            if (settings == null || settings.Count == 0)
            {
                return false;
            }
            else
            {
                var setting = settings.Where(x => x.Setting == key).FirstOrDefault();
                if (setting == null)
                {
                    return false;
                }
                else
                {
                    return Convert.ToBoolean(setting.Value);
                }
            }
        }
    }
}