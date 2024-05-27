using Data;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Repositories
{
    public class SystemTokensRepository : ISystemTokensRepository
    {
        private readonly IRepository m_dbContext;

        public SystemTokensRepository(IRepository dbContext)
        {
            m_dbContext = dbContext;
        }

        //GET TOKENS
        public List<TokenDTO> getTokens()
        {
            try
            {
                List<Apitoken> apitokens = new List<Apitoken>();

                apitokens = m_dbContext.Get<Apitoken>().Where(q => q.DeletedAt == null).ToList();

                List<TokenDTO> lstResult = new List<TokenDTO>();


                foreach(Apitoken apiToken in apitokens)
                {
                    TokenDTO tempToken = new()
                    {
                        TokenId = apiToken.TokenId,
                        Token = apiToken.Token,
                        CreatedAt = apiToken.CreatedAt,
                        ExpireDate = apiToken.ExpireDate,
                    };

                    lstResult.Add(tempToken);
                   
                }
                return lstResult;
          
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //POST TOKENS
        public void postToken(TokenPostDTO token, long createdBy)
        {
            try
            {
                Apitoken tempTokenObj = new Apitoken
                {
                    Token = token.Token,
                    CreatedAt = DateTime.Now,
                    ExpireDate = DateTime.Now.AddDays(1),
                    CreatedBy = createdBy,
                };

                m_dbContext.Create(tempTokenObj);
                m_dbContext.Save();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //UPDATE TOKEN
        public void updateToken(TokenPutDTO token, long updatedBy)
        {
            try
            {
                Apitoken exsistingResult = m_dbContext.GetById<Apitoken>(token.TokenId);

                if (exsistingResult != null && exsistingResult.TokenId == token.TokenId)
                {
                    exsistingResult.Token = token.Token;
                    exsistingResult.CreatedAt = DateTime.Now;
                    exsistingResult.ExpireDate = DateTime.Now.AddDays(1);
                    exsistingResult.UpdatedAt = DateTime.Now;
                    exsistingResult.UpdatedBy = updatedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Token Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //DELETE TOKEN
        public void deleteToken(long tokenId, long deletedBy)
        {
            try
            {
                Apitoken exsistingResult = m_dbContext.GetById<Apitoken>(tokenId);

                if (exsistingResult != null && exsistingResult.TokenId == tokenId)
                {
                    exsistingResult.DeletedAt = DateTime.Now;
                    exsistingResult.DeletedBy = deletedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Token Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
