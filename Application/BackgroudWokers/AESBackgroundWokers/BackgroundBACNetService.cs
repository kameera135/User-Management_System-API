using AES.Domain.Definitions;
using Application.Interfaces.ServiceInterfaces.AesServiceInterfaces;
using CommonInsfrastructure.Interfaces;
using Domain.DataTransferringObjects.AesDTOs;
using Domain.DataTransferringObjects.CommonUtilities;
using LoggerLibrary.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Application.BackgroudWokers.AESBackgroundWokers
{
    public class BackgroundBACNetService : BackgroundService
    {
        private readonly ILogService m_logService;
        private readonly IServiceProvider m_serviceProvider;
        private readonly IConfiguration m_configuration;

        public BackgroundBACNetService(ILogService logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            m_logService = logger;
            m_serviceProvider = serviceProvider;
            m_configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            m_logService.Log("OPCTag deploying service is starting");

            //get background task log status
            bool logBackgrondTask = m_configuration.GetValue<bool>("BGTasks:logBackgrondTask");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = m_serviceProvider.CreateScope();
                var m_extService = scope.ServiceProvider.GetRequiredService<IExtensionService>();
                var m_opcTagService = scope.ServiceProvider.GetRequiredService<IOPCTagService>();

                //looping interval
                int interval = (int)(m_configuration.GetValue<double>("BGTasks:BACnetTableUpdateInterval") * 60 * 1000);

                ExtensionResultsDTO extensions = new();

                List<ExtensionSavingHelperDTO> lstUnits = new();
                var reques_type_new = new List<string>() { RequestTypeDeffinitions.New };// { "New" };
                var status_approved = new List<string>() { approvalStatusDeffinitions.Approved }; // { "accepted" };

                try
                {
                    //get all extensions
                    m_logService.Log("getting all approved extensions", null, logBackgrondTask);

                    ExtensionResultsDTO extensionList = (ExtensionResultsDTO)m_extService.GetExtensionsNeedActivating(100);
                    if (HasDataExt(extensionList))
                    {
                        m_logService.Log("Found " + extensions.dataCount + " approved extensions", null, logBackgrondTask);
                        foreach (var extension in extensionList.lstExtensionList)
                        {
                            m_logService.Log("Processing extension " + extension.Id, null, logBackgrondTask);
                            //activate extension
                            m_extService.ActivateExtension(extension.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_logService.Log("Error while running the opc tag deploying service", "error");
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
            if (extensions == null || extensions.lstExtensionList == null)
            {
                return false;
            }
            return true;
        }
    }
}