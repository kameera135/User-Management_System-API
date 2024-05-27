using CommonInsfrastructure.Interfaces;
using CommonInsfrastructure.Models.EmailModels;
using LoggerLibrary.Interface;

namespace CommonInsfrastructure.Repositories
{
    public class EmailRepository : IEmailService
    {
        private readonly ILogService m_logService;
        private readonly EmailDbContext m_emailDbContext;

        public EmailRepository(ILogService logService, EmailDbContext emailDbContext)
        {
            m_logService = logService;
            m_emailDbContext = emailDbContext;
        }

        public void SendEmail(TblEmailList email)
        {
            try
            {
                m_emailDbContext.TblEmailLists.Add(email);
                m_emailDbContext.SaveChanges();

                m_logService.Log("Email send successfully");
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
            }
        }
    }
}