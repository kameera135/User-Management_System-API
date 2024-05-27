using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface ISystemTokensRepository
    {
        void deleteToken(long tokenId, long deletedBy);
        public List<TokenDTO> getTokens();
        void postToken(TokenPostDTO token, long createdBy);
        void updateToken(TokenPutDTO token, long updatedBy);
    }
}
