using UMS.Api.Domain.DTO;

namespace UMS.Api.Interfaces
{
    public interface ISystemTokensService
    {
        List<TokenDTO> getTokens();
        void postToken(TokenPostDTO token, long createdBy);
       // List<TokenDTO> searchedTokens(DateTime firstDateString, DateTime lastDateString);
        void updateToken(TokenPutDTO token, long updatedBy);

        void deleteToken(long tokenId, long deletedBy);
    }
}
