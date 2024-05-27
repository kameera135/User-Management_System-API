using Microsoft.AspNetCore.Identity;
using UMS.Api.Domain.DTO;
using UMS.Api.Models;

namespace UMS.Api.Interfaces
{
    public interface IAuthService
    {
        Task<RegisterUserPostDTO> registerUser(RegisterUserDTO users, long createdBy);

        bool loginUser(UserLoginDTO user);
        string createToken(string sessionToken, long platformId);
        RefreshTokenDTO generateRefreshToken();
        CookieOptions setRefreshToken(RefreshTokenDTO refreshToken);
        Task<User> findByEmail(ForgotPasswordDTO forgotPassword);
        Task<User> findByEmail(ResetPasswordDTO forgotPassword);
        Task<ResetPasswordTokenDTO> generatePasswordResetToken();
        Task<IdentityResult> resetPassword(User user, ResetPasswordDTO resetPassword);
        string createSessionToken(string username);
        bool validateSessionToken(long userId, string sessionToken);
        void deleteSessionToken(long userId, string sessionToken);
    }
}
