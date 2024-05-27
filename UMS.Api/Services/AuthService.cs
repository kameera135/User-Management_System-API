using Microsoft.AspNetCore.Identity;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Repositories;

namespace UMS.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository m_authRepository;
       // private readonly User tempUser;

        public AuthService(IAuthRepository authRepository)
        {
            m_authRepository = authRepository;
        }

        public string createToken(string sessionToken, long platformId)
        {
            try
            {
                var token = m_authRepository.createToken(sessionToken, platformId);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public RefreshTokenDTO generateRefreshToken()
        {
            try
            {
                var token = m_authRepository.generateRefreshToken();
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<RegisterUserPostDTO> registerUser(RegisterUserDTO users, long createdBy)
        {
            try
            {
                var tempUser =await m_authRepository.registerUser(users, createdBy);
                return tempUser;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        bool IAuthService.loginUser(UserLoginDTO user)
        {
            try
            {
                var tempUser = m_authRepository.loginUser(user);
                return tempUser;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public CookieOptions setRefreshToken(RefreshTokenDTO refreshToken)
        {
            try
            {
                var options = m_authRepository.setRefreshToken(refreshToken);
                return options;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<User> findByEmail(ForgotPasswordDTO forgotPassword)
        {
            try
            {
                var user = m_authRepository.findByEmail(forgotPassword);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<ResetPasswordTokenDTO> generatePasswordResetToken()
        {
            try
            {
                var token = m_authRepository.generatePasswordResetToken();
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<User> findByEmail(ResetPasswordDTO forgotPassword)
        {
            try
            {
                var user = m_authRepository.findByEmail(forgotPassword);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<IdentityResult> resetPassword(User user, ResetPasswordDTO resetPassword)
        {
            try
            {
                var option = m_authRepository.resetPassword(user,resetPassword);
                return option;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string createSessionToken(string username)
        {
            try
            {
                var token = m_authRepository.createSessionToken(username);
                return token;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool validateSessionToken(long userId, string sessionToken)
        {
            try
            {
                if(m_authRepository.validateSessionToken(userId, sessionToken)) { return true; }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void deleteSessionToken(long userId, string sessionToken)
        {
            try
            {
                m_authRepository.deleteSessionToken(userId, sessionToken);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
