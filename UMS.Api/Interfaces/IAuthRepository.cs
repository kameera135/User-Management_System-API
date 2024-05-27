using Microsoft.AspNetCore.Identity;
using UMS.Api.Domain.DTO;
using UMS.Api.Models;

namespace UMS.Api.Interfaces
{
    public interface IAuthRepository
    {
        string createSessionToken(string username);
        string createToken(string sessionToken, long platformId);
        void deleteSessionToken(long userId, string sessionToken);
        Task<User> findByEmail(ForgotPasswordDTO forgotPassword);
        Task<User> findByEmail(ResetPasswordDTO forgotPassword);
        Task<ResetPasswordTokenDTO> generatePasswordResetToken();
        RefreshTokenDTO generateRefreshToken();
        bool loginUser(UserLoginDTO user);
        Task<RegisterUserPostDTO> registerUser(RegisterUserDTO user, long createdBy);
        Task<IdentityResult> resetPassword(User user, ResetPasswordDTO resetPassword);
        CookieOptions setRefreshToken(RefreshTokenDTO refreshToken);
        bool validateSessionToken(long userId, string sessionToken);
    }
}
