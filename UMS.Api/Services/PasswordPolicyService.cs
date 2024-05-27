using CommonInsfrastructure.Interfaces;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;

namespace UMS.Api.Services
{
    public class PasswordPolicyService : IPasswordPolicyService
    {
        private readonly IPasswordPolicyRepository m_passwordPolicyRepository;
        private readonly ICommonConfigRepository m_commonConfigRepository;

        public PasswordPolicyService(IPasswordPolicyRepository passwordPolicyRepository, ICommonConfigRepository commonConfigRepository)
        {
            m_passwordPolicyRepository = passwordPolicyRepository;
            m_commonConfigRepository = commonConfigRepository;
        }

        public void updatePasswordPolicy(string passwordPolicy, long UpdatedBy)
        {
            try
            {
                m_commonConfigRepository.PutCommonSettingData("UMS_PP", passwordPolicy, UpdatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
