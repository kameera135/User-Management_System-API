using Data;
using LicenseLibrary;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;
        private readonly IConfiguration m_cofiguration;
        private readonly ILicense m_license;
        private readonly IServiceProvider m_serviceProvider;
        private readonly IPasswordPolicyService m_passwordPolicyService;

        public AuthRepository(IRepository dbContext, UmsContext umsContext, IConfiguration cofiguration, ILicense license, IServiceProvider serviceProvider, IPasswordPolicyService passwordPolicyService)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
            m_cofiguration = cofiguration;
            m_license = license;
            m_serviceProvider = serviceProvider;
            m_passwordPolicyService = passwordPolicyService;
        }

        public Task<RegisterUserPostDTO> registerUser(RegisterUserDTO user, long createdBy)
        {
            try
            {
                string PasswordSalt = m_passwordPolicyService.generateSalt();
                string PasswordHash = m_passwordPolicyService.getPasswordHash(user.Password, PasswordSalt);

                //create tempUser in User modal
                User tempUser = new User
                {
                    UserName = user.UserName,
                    PasswordHash = PasswordHash,
                    PasswordSalt = PasswordSalt,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = user.Phone,
                    FirstLogin = "Just Added",
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                m_dbContext.Create(tempUser);
                m_dbContext.Save();

                //To return specific parameters from the DTO file
                var RegisterUser = new RegisterUserPostDTO
                {
                    UserName = tempUser.UserName,
                    PasswordHash = PasswordHash,
                    PasswordSalt = PasswordSalt,
                    FirstName = tempUser.FirstName,
                    LastName = tempUser.LastName,
                    Email = tempUser.Email,
                    Phone = tempUser.Phone,
                    FirstLogin = "Just Added",
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                return Task.FromResult(RegisterUser);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool loginUser(UserLoginDTO user)
        {
            try
            {
                User? tempUser = m_dbContext.Get<User>(u => u.UserName == user.UserName && u.DeletedAt == null).FirstOrDefault();

                if (tempUser != null && tempUser.UserName != null)
                {
                    string IncomingPasswordHash = m_passwordPolicyService.getPasswordHash(user.Password, tempUser.PasswordSalt);

                    if (IncomingPasswordHash == tempUser.PasswordHash) return true;
                    else return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string createToken(string sessionToken, long platformId)
        {
            try
            {
                long? userId = m_umsContext.AuthTokens.Where(u => u.Token == sessionToken && u.ExpireDate > DateTime.Now && u.DeletedAt == null).Select(u => u.UserId).FirstOrDefault();

                var platform = m_umsContext.Platforms.Where(p => p.PlatformId == platformId).FirstOrDefault();

                if (platform != null)
                {
                    string? currentPlatform = m_umsContext.Platforms.Where(p => p.PlatformId == platformId).Select(u => u.PlatformName).FirstOrDefault();

                    string? platformCode = m_umsContext.Platforms.Where(p => p.PlatformId == platformId).Select(u => u.PlatformCode).FirstOrDefault();

                    List<Claim> claims = new List<Claim> { };

                    //Get userId from user name
                    var user = m_dbContext.GetById<User>(userId);

                    if (user != null)
                    {
                        claims.Add(new Claim("id", userId.ToString()));
                        claims.Add(new Claim("username", user.UserName.ToString()));
                        claims.Add(new Claim("fName", user.FirstName.ToString()));
                        claims.Add(new Claim("lName", user.LastName.ToString()));
                        claims.Add(new Claim("email", user.Email.ToString()));
                        claims.Add(new Claim("currentPlatformId", platformId.ToString()));
                        claims.Add(new Claim("currentPlatform", currentPlatform));
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }

                    if (userId != 0)
                    {
                        //Retrive platforms
                        var userPlatforms = from up in m_umsContext.UserPlatforms
                                            join p in m_umsContext.Platforms on up.PlatformId equals p.PlatformId
                                            where up.UserId == user.UserId && up.DeletedAt == null && p.DeletedAt == null && p.PlatformId != platformId
                                            select new
                                            {
                                                PlatformID = p.PlatformId,
                                                PlatformName = p.PlatformName,
                                                PlatformURL = p.PlatformUrl,
                                            };

                        if (userPlatforms.Any())
                        {
                            foreach (var pl in userPlatforms)
                            {
                                claims.Add(new Claim("platforms", pl.PlatformName + " || " + pl.PlatformURL + " || " + pl.PlatformID));
                            }
                        }

                        //Retrive roles
                        var userRoles = from ur in m_umsContext.UserRoles
                                        join r in m_umsContext.Roles on ur.RoleId equals r.RoleId
                                        where ur.UserId == userId && r.DeletedAt == null && ur.DeletedAt == null && r.PlatformId == platformId && r.Status == true
                                        select new
                                        {
                                            RoleName = r.Role1
                                        };

                        if (userRoles.Any())
                        {
                            var roles = userRoles.ToList();

                            foreach (var role in roles)
                            {
                                claims.Add(new Claim("roles", role.RoleName));
                            }
                        }

                        //Check license validation for get permissions
                        if (m_license.isLicenseValid(platformCode))
                        {
                            //Retrive permissions
                            var userPermissions = from ur in m_umsContext.UserRoles
                                                  join rp in m_umsContext.RolePermissions on ur.RoleId equals rp.RoleId
                                                  join p in m_umsContext.Permissions on rp.PermissionId equals p.PermissionId
                                                  where ur.UserId == userId && p.DeletedAt == null && rp.DeletedAt == null && ur.DeletedAt == null && p.PlatformId == platformId && p.Status == true && p.Is_licence == true
                                                  select new
                                                  {
                                                      PermissionName = p.Permission1
                                                  };

                            if (userPermissions.Any())
                            {
                                var uniquePermissions = userPermissions.Select(up => up.PermissionName).Distinct().ToList();

                                foreach (var permission in uniquePermissions)
                                {
                                    claims.Add(new Claim("permissions", permission));
                                }
                            }
                            claims.Add(new Claim("license", "full"));
                        }
                        else
                        {
                            var userPermissions = from ur in m_umsContext.UserRoles
                                                  join rp in m_umsContext.RolePermissions on ur.RoleId equals rp.RoleId
                                                  join p in m_umsContext.Permissions on rp.PermissionId equals p.PermissionId
                                                  where ur.UserId == userId && p.DeletedAt == null && rp.DeletedAt == null && ur.DeletedAt == null && p.PlatformId == platformId && p.Status == true && p.Is_licence == false
                                                  select new
                                                  {
                                                      PermissionName = p.Permission1
                                                  };

                            if (userPermissions.Any())
                            {
                                var uniquePermissions = userPermissions.Select(up => up.PermissionName).Distinct().ToList();

                                foreach (var permission in uniquePermissions)
                                {
                                    claims.Add(new Claim("permissions", permission));
                                }
                            }
                            claims.Add(new Claim("license", "trial"));
                        }
                    }

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s: m_cofiguration.GetSection("AppSettings:Token").Value));

                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateTime.Now.AddDays(7), //make this more limit time like minitue
                        signingCredentials: creds);

                    var jwt = new JwtSecurityTokenHandler().WriteToken(token);

                    return jwt;
                }
                else
                {
                    throw new Exception("Platform Id not found");
                }
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
                var refreshToken = new RefreshTokenDTO
                {
                    Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    Expires = DateTime.Now.AddDays(7),
                    Created = DateTime.Now
                };

                return refreshToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CookieOptions setRefreshToken(RefreshTokenDTO refreshToken)
        {
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = refreshToken.Expires,
                };

                return cookieOptions;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<User?> findByEmail(ForgotPasswordDTO forgotPassword)
        {
            try
            {
                User? user = m_dbContext.Get<User>(u => u.Email == forgotPassword.Email && u.DeletedBy == null).FirstOrDefault();

                return Task.FromResult(user);
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
                var expirationDate = DateTime.UtcNow.AddHours(1);

                var tokenBytes = new byte[32]; // 256 bits for a secure token
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(tokenBytes);
                }

                var token = Convert.ToBase64String(tokenBytes);

                // Encode the expiration date to Base64
                string encodedExpireDate = Convert.ToBase64String(Encoding.UTF8.GetBytes(expirationDate.ToString()));

                //var combinedToken = $"{token}.{encodedExpireDate}";

                var signature = CreateTokenSignature(token);

                var finalToken = $"{token}.{encodedExpireDate}.{signature}";

                var resetToken = new ResetPasswordTokenDTO
                {
                    Token = finalToken
                };

                return Task.FromResult(resetToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating password reset token: " + ex.Message);
            }
        }

        public Task<User?> findByEmail(ResetPasswordDTO forgotPassword)
        {
            try
            {
                User? user = m_dbContext.Get<User>(u => u.Email == forgotPassword.Email && u.DeletedBy == null).FirstOrDefault();

                return Task.FromResult(user);
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
                if (!validateToken(resetPassword.Token))
                {
                    return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Invalid or expired reset password token" }));
                }

                User exsistingResult = m_dbContext.GetById<User>(user.UserId);

                if (resetPassword != null && resetPassword.Password != null)
                {
                    exsistingResult.PasswordSalt = m_passwordPolicyService.generateSalt();
                    exsistingResult.PasswordHash = m_passwordPolicyService.getPasswordHash(resetPassword.Password, exsistingResult.PasswordSalt);
                    exsistingResult.UpdatedAt = DateTime.Now;
                    exsistingResult.UpdatedBy = user.UserId;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }

                return Task.FromResult(IdentityResult.Success);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool validateToken(string resetToken)
        {
            // Extract token and signature
            string[] tokenParts = resetToken.Split('.');
            if (tokenParts.Length != 3)
            {
                // Invalid token format
                return false;
            }

            string token = tokenParts[0];
            string expirationDateString = tokenParts[1];
            string signature = tokenParts[2];

            // Decode the expiration date from Base64
            byte[] expireDateBytes = Convert.FromBase64String(expirationDateString);
            string expireDateStr = Encoding.UTF8.GetString(expireDateBytes);
            DateTime expireDate = DateTime.Parse(expireDateStr);

            // Recompute signature for the token
            string computedSignature = CreateTokenSignature(token);

            // Compare computed signature with the extracted signature
            if (computedSignature != signature)
            {
                return false;
            }

            // Check expiration date
            if (expireDate < DateTime.UtcNow)
            {
                return false;
            }

            return true;
        }

        private string CreateTokenSignature(string token)
        {
            // Generate a signature for the token using a secret key
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes("secret_key")))
            {
                byte[] signatureBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
                return Convert.ToBase64String(signatureBytes);
            }
        }

        public string createSessionToken(string username)
        {
            try
            {
                User? tempUser = m_umsContext.Users.FirstOrDefault(u => u.UserName == username && u.DeletedAt == null);
                AuthToken? tempToken = m_umsContext.AuthTokens.FirstOrDefault(t => t.UserId == tempUser.UserId && t.DeletedAt == null);

                if (tempUser != null)
                {
                    var temptoken = GenerateRandomString();

                    AuthToken token = new AuthToken
                    {
                        UserId = tempUser.UserId,
                        Token = temptoken,
                        ExpireDate = DateTime.Now.AddDays(1),
                        CreatedAt = DateTime.Now,
                        CreatedBy = tempUser.UserId,
                    };
                    m_dbContext.Create(token);
                    m_dbContext.Save();

                    return token.Token;
                }
                else
                {
                    throw new Exception("User not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GenerateRandomString()
        {
            int length = 32;
            var crypt = new SHA256Managed();
            string hash = String.Empty;

            // Define the characters allowed in the random string
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // Create a random object
            Random random = new Random();

            // Generate a random string by selecting characters randomly from the allowed characters
            string randomString = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());

            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));

            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            return hash;
        }

        public bool validateSessionToken(long userId, string sessionToken)
        {
            try
            {
                User tempUser = m_dbContext.GetById<User>(userId);
                if (tempUser != null)
                {
                    AuthToken? token = m_umsContext.AuthTokens.FirstOrDefault(u => u.UserId == userId && u.Token == sessionToken && u.ExpireDate != null && u.ExpireDate > DateTime.Now && u.DeletedAt == null);
                    bool isValidCombination = token != null && token.UserId == userId;

                    if (token != null && isValidCombination)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
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
                AuthToken? token = m_umsContext.AuthTokens.FirstOrDefault(t => t.UserId == userId && t.Token == sessionToken);

                if (token != null)
                {
                    token.DeletedAt = DateTime.Now;
                    token.DeletedBy = userId;

                    m_dbContext.Update(token);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Token is invalid");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}