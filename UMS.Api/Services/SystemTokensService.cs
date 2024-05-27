using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Repositories;

namespace UMS.Api.Services
{
    public class SystemTokensService: ISystemTokensService
    {
        private readonly ISystemTokensRepository m_systemTokensRepository;

        public SystemTokensService(ISystemTokensRepository systemTokensRepository)
        {
            m_systemTokensRepository = systemTokensRepository;
        }

        public List<TokenDTO> getTokens()
        {
            try
            {
                List<TokenDTO> lstTokens = m_systemTokensRepository.getTokens();
                return lstTokens;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /*public List <TokenDTO> searchedTokens(DateTime firstDate, DateTime lastDate) {

            try
            {
                List<TokenDTO> lstTokens = m_systemTokensRepository.getTokens(firstDate, lastDate);
                return lstTokens;
            }
            catch (Exception ex)
            {
                throw;
            }
        }*/

        public void postToken(TokenPostDTO token, long createdBy)
        {
            try
            {
                m_systemTokensRepository.postToken(token, createdBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void updateToken(TokenPutDTO token, long updatedBy)
        {
            try
            {
                m_systemTokensRepository.updateToken(token, updatedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

         public void deleteToken(long tokenId, long deletedBy)
        {
            try
            {
                m_systemTokensRepository.deleteToken(tokenId, deletedBy);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
