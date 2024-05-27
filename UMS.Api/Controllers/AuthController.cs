using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;
using UMS.Api.Services;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Authorization;
using Data;
using LicenseLibrary;

namespace UMS.Api.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogService m_logService;
        private readonly IAuthService m_authService;
        private readonly IEmailSender m_emailSender;
        private readonly UmsContext m_umsContext;
        private readonly IActivityLogsService m_umsActivityLogService;
        private readonly UserRolePlatformService m_userRolePlatformService;


        public AuthController(IAuthService authService, ILogService logger, IEmailSender emailSender, UmsContext umsContext, IActivityLogsService umsActivityLogService, UserRolePlatformService userRolePlatformService)
        {
            m_logService = logger;
            m_authService = authService;
            m_emailSender = emailSender;
            m_umsContext = umsContext;
            m_umsActivityLogService = umsActivityLogService;
            m_userRolePlatformService = userRolePlatformService;
        }

        long GetPlatformId()
        {
            return m_userRolePlatformService.GetPlatformId();
        }

        //USER REGISTRATION
        [HttpPost("user/register")]
        public async Task<IActionResult> RegisterUser(RegisterUserDTO users, long createdBy)
        {
            m_logService.Log("Auth Service -> Authentication has been initiated");


            try
            {
                var userResult = m_authService.registerUser(users, createdBy);
                var result = userResult.Result; // For get the reponse that only DTO file return

                m_logService.Log("Add User: User registered successfully");
                return Ok(result);
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Register user", "UMS", Int32.Parse(createdBy.ToString()), null, null);
            }
        }

        //USER LOGIN
        [HttpPost("user/login")]
        public IActionResult LoginUser(UserLoginDTO users)
        {
            m_logService.Log("Auth Service -> Authentication has been initiated for login");
            var user = m_umsContext.Users.Where(u => u.UserName == users.UserName).FirstOrDefault();

            if (user == null)
            {
                return BadRequest("Wrong userName or password. Kindly check");
            }

            long userId = user.UserId;

            try
            {

                var result = m_authService.loginUser(users);
                var handler = new JwtSecurityTokenHandler();

                if (!result)
                {
                    return BadRequest("Wrong userName or password. Kindly check");
                }

                long platformId = GetPlatformId();

                string sessionToken = m_authService.createSessionToken(users.UserName);

                string token = m_authService.createToken(sessionToken, platformId);
                RefreshTokenDTO refreshToken = m_authService.generateRefreshToken();
                CookieOptions cookieOptions = m_authService.setRefreshToken(refreshToken);

                Response.Cookies.Append("refresh-token", refreshToken.Token, cookieOptions);


                m_logService.Log("Auth Service -> User Loged In Successfully");

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = "Login Successfully",
                    Token = token,
                    Expire = handler.ReadToken(token).ValidTo,
                    session_id = sessionToken,
                });
            }

            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Login user", "UMS", Int32.Parse(userId.ToString()), null,null);
            }
        }

        [HttpPost("user/createJWT")]
        public IActionResult createJWT(string sessionToken,  long platformId)
        {
            m_logService.Log("Auth Service -> Create JWT");

            var user = m_umsContext.AuthTokens.Where(u=> u.Token == sessionToken).FirstOrDefault();

            if (user == null)
            {
                var errorResponse = new
                {
                    Message = "Wrong session token. ",
                };

                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            long userId = user.UserId;

            try
            {
                string token = m_authService.createToken(sessionToken, platformId);

                return Ok(new
                {
                    StatusCode = StatusCodes.Status200OK,
                    message = "JWT has been created successfully",
                    Token = token,
                });
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Create JWT", "UMS", Int32.Parse(userId.ToString()), null, null);
            }
        }

        //FORGOT PASSWORD FEATURE
        [HttpPost("user/ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO forgotPasswordDto)
        {
            m_logService.Log("Auth service -> User Initiate Forgot Password Feature");

            var user1 = m_umsContext.Users.FirstOrDefault(u => u.Email == forgotPasswordDto.Email);

            if (user1 == null)
            {
                var errorResponse = new
                {
                    Message = "Email not found. Check email format or correct email address",
                };

                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            long userId = user1.UserId;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var user = await m_authService.findByEmail(forgotPasswordDto);
                if (user == null)
                    return BadRequest("Invalid Request");

                var token = await m_authService.generatePasswordResetToken();
                var param = new Dictionary<string, string?>
                {
                    {"token", token.Token },
                    {"email", forgotPasswordDto.Email }
                };

                var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);
                var message = new Message(new string[] { user.Email }, "Reset password token", callback, null);



                await m_emailSender.SendEmailAsync(message, user.Email,callback);

                m_logService.Log("Auth service -> Password Reset Link has been sent");

                return Ok(new
                {
                    StatusCode = 200,
                    message = "Password reset link has been sent",
                    token = token

                });
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Forgot password", "UMS", Int32.Parse(userId.ToString()),null,null);
            }
        }

        //RESET PASSWORD
        [HttpPost("user/ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO resetPassword)
        {
            m_logService.Log("Auth service -> User Initiate Reset Password");
            var user1 = m_umsContext.Users.FirstOrDefault(u => u.Email == resetPassword.Email);

            if (user1 == null)
            {
                var errorResponse = new
                {
                    Message = "Email not found. Check email format or correct email address",
                };

                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            long userId = user1.UserId;

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                var user = await m_authService.findByEmail(resetPassword);

                if (user == null)
                {
                    var errorResponse = new
                    {
                        Message = "Email not found. Check email format or correct email address",
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
                }

                if (resetPassword.Password == null || resetPassword.ConfirmPassword == null)
                {
                    var errorResponse = new
                    {
                        Message = "Password and confirm passwords can not be null",
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
                }

                if (resetPassword.Password != resetPassword.ConfirmPassword)
                {
                    var errorResponse = new
                    {
                        Message = "Password is not match. Kindly check the password",
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
                }

                var resetPassResult = await m_authService.resetPassword(user, resetPassword);
                if (!resetPassResult.Succeeded)
                {
                    var errors = resetPassResult.Errors.Select(e => e.Description);

                    var errorResponse = new
                    {
                        Message = errors,
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
                }

                //await _userManager.SetLockoutEndDateAsync(user, new DateTime(2000, 1, 1));

                m_logService.Log("Auth service -> Password has been reset");

                return Ok(new
                {
                    Status_code = 200,
                    Message = "Password has been reset"
                });
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Reset password", "UMS", Int32.Parse(userId.ToString()), null, null);
            }
        }


        [HttpPost("user/createToken")]
        public IActionResult CreateSessionToken(string username)
        {
            m_logService.Log("Auth service -> Session token has been created");
            var user1 = m_umsContext.Users.FirstOrDefault(u => u.UserName == username);

            if (user1 == null)
            {
                var errorResponse = new
                {
                    Message = "Username was not found",
                };

                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            long userId = user1.UserId;

            try
            {
                string token = m_authService.createSessionToken(username);
                return Ok(new
                {
                    status_code = StatusCodes.Status200OK,
                    Token = token,
                    message = "Session token has been created"
                });
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);

            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Create Session Token", "UMS", Int32.Parse(userId.ToString()), null, null);
            }
        }

        [HttpPost("user/validateSessionToken")]
        public IActionResult ValidateSessionToken(long userId, string sessionToken)
        {
            m_logService.Log("Auth service -> Session token has been authenticated");

            var user1 = m_umsContext.Users.FirstOrDefault(u => u.UserId == userId);

            if (user1 == null)
            {
                var errorResponse = new
                {
                    Message = "User was not found",
                };

                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            try
            {
                long platformId = GetPlatformId();

                bool result = m_authService.validateSessionToken(userId, sessionToken);
                if (result == true)
                {
                    User? user = m_umsContext.Users.FirstOrDefault(u => u.UserId == userId);

                    UserLoginDTO tempUser = new UserLoginDTO
                    {
                        UserName = user.UserName,
                    };

                    var token = m_authService.createToken(sessionToken, platformId);

                    return Ok(new
                    {
                        StatusCode = StatusCodes.Status200OK,
                        message = "Session authentication is success",
                        Token = token
                    });
                }
                else
                {
                    var errorResponse = new
                    {
                        Message = "User do not have the valid session",
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
                }
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Validate session token", "UMS", Int32.Parse(userId.ToString()), null, null);
            }
        }

        [HttpDelete("user/deleteSession")]
        public IActionResult DeleteSessionToken(long userId, string sessionToken)
        {
            m_logService.Log("Auth service -> Session token has been deleted");

            var user1 = m_umsContext.Users.FirstOrDefault(u => u.UserId == userId);

            if (user1 == null)
            {
                var errorResponse = new
                {
                    Message = "User was not found",
                };

                return StatusCode(StatusCodes.Status400BadRequest, errorResponse);
            }

            try
            {
                m_authService.deleteSessionToken(userId, sessionToken);
                return Ok();
            }
            catch (Exception ex)
            {
                m_logService.Log(ex.ToString(), "error");
                var errorResponse = new
                {
                    Message = "Internal Server Error",
                    ErrorDetails = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, errorResponse);
            }
            finally
            {
                m_umsActivityLogService.CreateLog("Post", "Delete session token", "UMS", Int32.Parse(userId.ToString()), null, null);
            }
        }
    }
}
