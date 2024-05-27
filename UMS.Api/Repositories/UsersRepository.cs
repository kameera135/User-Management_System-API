using Azure.Identity;
using Data;
using LoggerLibrary.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Data.Entity.Core;
using System.Drawing.Printing;
using System.Security.Cryptography;
using System.Text;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Repositories
{
    public class UsersRepository : IUserRepository
    {
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;
        private readonly ILogService m_logger;

        public UsersRepository(IRepository dbContext, UmsContext umsContext, ILogService logger)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
            m_logger = logger;
        }

        //Get Users for combobox
        public List<ComboboxDTO> getUserList()
        {
            try
            {
                List<User> users = m_dbContext.Get<User>().Where(q => q.DeletedAt == null).ToList();

                List<ComboboxDTO> lstResult = new List<ComboboxDTO>();

                foreach (User user in users)
                {
                    ComboboxDTO tempUser = new()
                    {
                        Id = user.UserId,
                        Value = user.FirstName +" "+ user.LastName,
                    };

                    lstResult.Add(tempUser);
                }


                return lstResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Get Users
        public List<UserDTO> getUsers(int platformId, string? userName)
        {
            try
            {
                List<UserDTO> lstResult = new List<UserDTO>();

                if (platformId != 0) 
                {
                    var allUsersAccordingToThePlatform = ((from users in m_umsContext.Users
                                                           join userPlatform in m_umsContext.UserPlatforms on users.UserId equals userPlatform.UserId
                                                           where (userPlatform.PlatformId == platformId && userPlatform.DeletedAt==null && users.DeletedAt==null)
                                                           select new UserDTO
                                                           {
                                                               UserId = users.UserId,
                                                               UserName = users.UserName,
                                                               FirstName = users.FirstName,
                                                               LastName = users.LastName,
                                                               Email = users.Email,
                                                               Phone = users.Phone,
                                                               PasswordHash = users.PasswordHash,
                                                               PasswordSalt = users.PasswordSalt,
                                                           }).ToList());

                    if (userName != null)
                    {
                        lstResult = (List<UserDTO>)allUsersAccordingToThePlatform.Where(q => q.FirstName.ToLower().Contains(userName.ToLower()) || q.LastName.ToLower().Contains(userName.ToLower())).ToList();
                    }
                    else
                    {
                        lstResult = allUsersAccordingToThePlatform;
                    }
                    
                }
                else
                {
                    List<User> allUsers = new List<User>();

                    if (userName != null)
                    {
                        allUsers = m_dbContext.Get<User>().Where(q => q.DeletedAt==null && (q.FirstName.ToLower().Contains(userName.ToLower()) || q.LastName.ToLower().Contains(userName.ToLower()))).ToList();
                    }
                    else
                    {
                        allUsers = m_dbContext.Get<User>().Where(q => q.DeletedAt == null).ToList();
                    }

                    if(allUsers.Count > 0 && allUsers != null)
                    {
                        foreach (User user in allUsers)
                        {
                            UserDTO tempUser = new()
                            {
                                UserId = user.UserId,
                                UserName = user.UserName,
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                Phone = user.Phone,
                                PasswordHash = user.PasswordHash,
                                PasswordSalt = user.PasswordSalt
                            };

                            lstResult.Add(tempUser);
                        }
                    }                 
                }        
                
                return lstResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void postUser(UserPostDTO user,long createdBy)
        {
            try
            {
                if(m_umsContext.Users.Any(u=> u.UserName == user.UserName || u.Email == user.Email))
                {
                    throw new Exception("Username or email already exist. Please check and change it");
                }

                //for create password hash
                var hmac = new HMACSHA512();

                byte[] PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                byte[] PasswordSalt = hmac.Key;


                User tempUserObj = new User
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
                    CreatedBy = createdBy,
                };

                m_dbContext.Create(tempUserObj);
                m_dbContext.Save();

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void updateUser(UserPutDTO user, long updatedBy) 
        {
            try
            {
                User exsistingResult = m_dbContext.GetById<User>(user.UserId);

                if (exsistingResult != null && exsistingResult.UserId == user.UserId)
                {
                    if(m_umsContext.Users.Any(u => (u.UserId != user.UserId && u.UserName == user.UserName)))
                    {
                        throw new Exception("Username already exists for another user.");
                    }
                    else
                    {
                        exsistingResult.UserName = user.UserName;
                        exsistingResult.FirstName = user.FirstName;
                        exsistingResult.LastName = user.LastName;
                        //exsistingResult.PasswordHash = user.PasswordHash; 
                        exsistingResult.Email = user.Email;
                        exsistingResult.Phone = user.Phone;
                        exsistingResult.UpdatedAt = DateTime.Now;
                        exsistingResult.UpdatedBy = updatedBy;

                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            // Update password only if a new password is provided
                            var hmac = new HMACSHA512();
                            exsistingResult.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password));
                            exsistingResult.PasswordSalt = hmac.Key;
                        }

                        m_dbContext.Update(exsistingResult);
                        m_dbContext.Save();
                    }
                }
                else 
                {
                    throw new Exception("User Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void deleteUser(long userId, long deletedBy)
        {
            try
            {
                User exsistingResult = m_dbContext.GetById<User>(userId);

                if (exsistingResult != null && exsistingResult.UserId == userId)
                {
                    exsistingResult.DeletedAt = DateTime.Now;
                    exsistingResult.DeletedBy = deletedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("User Not found");
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UserPlatformsAndRolesDTO> getUserPlatformsAndRoles(long userId)
        {
            try
            {
                List<UserPlatformsAndRolesDTO> UserPlatformsAndRoles = new List<UserPlatformsAndRolesDTO>();

                //Gives Roles Which have not been assigned to platforms
                var UserPlatformsAndRoles3 = ((from user in m_umsContext.Users
                                                  join userRole in m_umsContext.UserRoles on user.UserId equals userRole.UserId
                                                  join role in m_umsContext.Roles on userRole.RoleId equals role.RoleId
                                                  where user.UserId == userId && role.PlatformId==null
                                                    && user.DeletedAt == null
                                                    && role.DeletedAt == null
                                                    && role.Status == true
                                                    && userRole.DeletedAt == null
                                                  select new UserPlatformsAndRolesDTO
                                                  {
                                                      RoleId = userRole.RoleId,
                                                      RoleName = role.Role1
                                                  }).ToList());

                    //Gives Roles Which have been assigned to platforms
                    var UserPlatformsAndRoles1 = ((from user in m_umsContext.Users
                                                  join userRole in m_umsContext.UserRoles on user.UserId equals userRole.UserId
                                                  join role in m_umsContext.Roles on userRole.RoleId equals role.RoleId
                                                  join platform in m_umsContext.Platforms on role.PlatformId equals platform.PlatformId
                                                   where user.UserId == userId
                                                    && user.DeletedAt == null
                                                    && role.DeletedAt == null
                                                    && role.Status == true
                                                    && platform.DeletedAt == null
                                                    && userRole.DeletedAt == null
                                                  select new UserPlatformsAndRolesDTO
                                                  {
                                                      RoleId = userRole.RoleId,
                                                      RoleName = role.Role1,
                                                      PlatformId = role.PlatformId,
                                                      PlatformName = platform.PlatformName,
                                                  }).ToList());

                //Gives Platforms Which don't have roles
                var UserPlatformsAndRoles2 = ((from user in m_umsContext.Users
                                                   join userPlatform in m_umsContext.UserPlatforms on user.UserId equals userPlatform.UserId
                                                   join platform in m_umsContext.Platforms on userPlatform.PlatformId equals platform.PlatformId
                                                   where user.UserId == userId
                                                    && user.DeletedAt == null
                                                    && platform.DeletedAt == null
                                                    && userPlatform.DeletedAt == null
                                                   select new UserPlatformsAndRolesDTO
                                                   {
                                                       PlatformId = userPlatform.PlatformId,
                                                       PlatformName = platform.PlatformName,
                                                   }).ToList());
                    var platformIdsToRemove = UserPlatformsAndRoles1.Select(x => x.PlatformId).ToHashSet();
                    UserPlatformsAndRoles2 = UserPlatformsAndRoles2.Where(x => !platformIdsToRemove.Contains(x.PlatformId)).ToList();

                UserPlatformsAndRoles = UserPlatformsAndRoles1.Union(UserPlatformsAndRoles2).Union(UserPlatformsAndRoles3).ToList();

                return UserPlatformsAndRoles;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Post Bulk Users

        public void postBulkUsers(List<UserPostDTO> users, List<BulkRoleAssignDTO> userRoles,  long createdBy)
        {
            using (var transaction = m_dbContext.CreateTransAction())
            {
                try
                {
                    //Post Users 
                    if (users.Count > 0 && users != null)
                    {
                        foreach (UserPostDTO user in users)
                        {
                            List<User> tempUserToCkeck = m_dbContext.Get<User>().Where(q => q.UserName == user.UserName && q.DeletedAt == null).ToList();

                            if (tempUserToCkeck.Count() == 0)
                            {
                                User tempUser = new()
                                {
                                    UserName = user.UserName,
                                   // Password = user.Password,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    Email = user.Email,
                                    Phone = user.Phone,
                                    FirstLogin = "Just Added",
                                    CreatedAt = DateTime.Now,
                                    CreatedBy = createdBy,
                                };

                                m_dbContext.Create(tempUser);
                            }
                            else
                            {
                                transaction.Rollback();
                                m_logger.Log("Duplicate usernames detected. Please choose a unique username for each user and try again.");
                                throw new Exception("Duplicate usernames detected. Please choose a unique username for each user and try again.");
                            }                           
                        }
                        m_dbContext.Save();

                        //Post User Roles
                        if (userRoles.Count > 0 && userRoles != null)
                        {
                            foreach (BulkRoleAssignDTO userRole in userRoles)
                            {
                                List<User> tempUserToAssign = m_dbContext.Get<User>().Where(q=> q.UserName==userRole.UserName && q.DeletedAt==null).ToList();

                                if(tempUserToAssign.Count > 1)
                                {
                                    transaction.Rollback();
                                    m_logger.Log("Duplicate usernames detected. Please choose a unique username for each user and try again.");
                                    throw new Exception("Duplicate usernames detected. Please choose a unique username for each user and try again.");
                                }
                                else if (tempUserToAssign.Count == 0)
                                {
                                    transaction.Rollback();
                                    m_logger.Log("Assignment Failed: No users were found for role allocation. Please ensure there are valid users available and try again.");
                                    throw new Exception("Assignment Failed: No users were found for role allocation. Please ensure there are valid users available and try again.");
                                }
                                else
                                {
                                    UserRole tempUserRole = new()
                                    {
                                        UserId = tempUserToAssign[0].UserId,
                                        RoleId = userRole.RoleId,
                                        CreatedAt = DateTime.Now,
                                        CreatedBy = createdBy,
                                    };
                                    m_dbContext.Create(tempUserRole);

                                   Role tempRole = m_dbContext.Get<Role>().Where(q => q.RoleId == userRole.RoleId && q.DeletedAt == null).ToList()[0];
                                    var platformId = tempRole.PlatformId;

                                    if (platformId != null)
                                    {
                                        UserPlatform tempUserPlatform = new()
                                        {
                                            UserId = tempUserToAssign[0].UserId,
                                            PlatformId = (long)platformId,
                                            CreatedAt = DateTime.Now,
                                            CreatedBy = createdBy,
                                        };
                                        m_dbContext.Create(tempUserPlatform);
                                    }                      
                                }                               
                            }
                            m_dbContext.Save();
                        }

                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                        m_logger.Log("No users to post");
                        throw new Exception("No users to post");
                        
                    }                 
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        //Get unassign users for selected platform
        public List<PlatformUsersDTO> getUsersNotInPlatform(long platformId, string? searchedUserName)
        {
            try
            {
                List<PlatformUsersDTO> lstResult = new List<PlatformUsersDTO>();

                if (searchedUserName != null)
                {

                    lstResult = (from users in m_umsContext.Users
                                 where (users.UserName.ToLower().Contains(searchedUserName.ToLower()) || users.FirstName.ToLower().Contains(searchedUserName.ToLower()) || users.LastName.ToLower().Contains(searchedUserName.ToLower())) && users.DeletedAt == null && !m_umsContext.UserPlatforms.Any(up => up.UserId == users.UserId && up.PlatformId == platformId && up.DeletedAt == null)
                                 select new PlatformUsersDTO
                                 {
                                     UserId = users.UserId,
                                     UserName = users.UserName,
                                     FirstName = users.FirstName,
                                     LastName = users.LastName,
                                     PhoneNumber = users.Phone,
                                     Email = users.Email,
                                    // PlatformId = userGroup.PlatformId
                                 }).ToList();
                   
                }
                    
                else
                {
                    lstResult = (from users in m_umsContext.Users
                                 where users.DeletedAt == null && !m_umsContext.UserPlatforms.Any(up=> up.UserId == users.UserId && up.PlatformId == platformId && up.DeletedAt == null)
                                 select new PlatformUsersDTO
                                 {
                                     UserId = users.UserId,
                                     UserName = users.UserName,
                                     FirstName = users.FirstName,
                                     LastName = users.LastName,
                                     PhoneNumber = users.Phone,
                                     Email = users.Email,
                                     //PlatformId = userGroup.PlatformId,
                                 }).ToList();
                }

                return lstResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<UserRolesAndPermissionsDTO> getRolePermissions(long userId, long platformId)
        {
            try
            {
                List<UserRolesAndPermissionsDTO> lstResult = new List<UserRolesAndPermissionsDTO>();

                //Get roles that have permissions
                var rolesPermissions = (from user in m_umsContext.Users
                                        join userRoles in m_umsContext.UserRoles on user.UserId equals userRoles.UserId
                                        join roles in m_umsContext.Roles on userRoles.RoleId equals roles.RoleId
                                        join rolePermissions in m_umsContext.RolePermissions on roles.RoleId equals rolePermissions.RoleId
                                        join permissions in m_umsContext.Permissions on rolePermissions.PermissionId equals permissions.PermissionId 
                                        where (user.DeletedAt == null && userRoles.DeletedAt == null && roles.DeletedAt == null && roles.Status ==true && rolePermissions.DeletedAt == null && user.UserId == userId && roles.PlatformId == platformId)

                                        select new UserRolesAndPermissionsDTO
                                        {
                                            RoleId = rolePermissions.RoleId,
                                            RoleName = roles.Role1,
                                            PermissionId = rolePermissions.PermissionId,
                                            Permission = permissions.Permission1

                                        }).ToList();

                //Get roles that do not have permissions
                var rolesNotPermisions = (from users in m_umsContext.Users
                                          join userRoles in m_umsContext.UserRoles on users.UserId equals userRoles.UserId
                                          join roles in m_umsContext.Roles on userRoles.RoleId equals roles.RoleId
                                          where (users.DeletedAt == null && roles.DeletedAt == null && userRoles.DeletedAt == null && roles.Status == true && users.UserId == userId && roles.PlatformId == platformId)
                                          select new UserRolesAndPermissionsDTO
                                          {
                                              RoleId = roles.RoleId,
                                              RoleName = roles.Role1,

                                          }).ToList();


                var roleIdToRemove = rolesPermissions.Select(x => x.RoleId).ToHashSet();
                rolesNotPermisions = rolesNotPermisions.Where(x => !roleIdToRemove.Contains(x.RoleId)).ToList();

                lstResult = rolesPermissions.Union(rolesNotPermisions).ToList();

                return lstResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void assignRoles(AssignRolesToUserDTO user, long createdBy)
        {
            try
            {
                bool userRolesHaveSamePlatform = checkUserRolesInSamePlatform(user.UserId, user.RoleIds);

                if (userRolesHaveSamePlatform)
                {
                    foreach (long role in user.RoleIds)
                    {
                        UserRole? entryExist = m_umsContext.UserRoles.FirstOrDefault(ur => ur.UserId.Equals(user.UserId) && ur.RoleId.Equals(role));

                        if (entryExist == null)
                        {
                            UserRole userRole = new UserRole()
                            {
                                RoleId = role,
                                UserId = user.UserId,
                                CreatedAt = DateTime.Now,
                                CreatedBy = createdBy
                            };
                            m_dbContext.Create(userRole);
                        }
                        else
                        {
                            entryExist.DeletedAt = null;
                            entryExist.DeletedBy = null;
                        }
                    }
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("User and roles are not in same platform");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //To check roles and user in the same platform
        private bool checkUserRolesInSamePlatform(long userId, List<long> roles)
        {
            if(roles.Any())
            {
                var userRoles = from role in m_umsContext.Roles
                                join userPlatform in m_umsContext.UserPlatforms on role.PlatformId equals userPlatform.PlatformId
                                where roles.Contains(role.RoleId) && userPlatform.UserId == userId && role.DeletedAt == null && userPlatform.DeletedAt == null && role.Status == true
                                select role;

                return userRoles.Count() == roles.Count();
            }
            else
            {
                return false;
            }
        }

        public void unassignRoles(long userId,long roleId, long deletedBy)
        {
            try
            {
                UserRole? existingResult = m_umsContext.UserRoles.FirstOrDefault(ur => ur.RoleId == roleId && ur.UserId == userId);

                if (existingResult != null)
                {
                    existingResult.DeletedAt = DateTime.Now;
                    existingResult.DeletedBy = deletedBy;

                    m_dbContext.Update(existingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("User and role combinations are not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public GetUserDTO getUserDetails(long userId)
        {
            try
            {
                User? user = m_umsContext.Users.FirstOrDefault(ur => ur.UserId == userId && ur.DeletedAt == null);

                if(user != null)
                {
                    GetUserDTO tempUser = new()
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Phone = user.Phone,
                    };

                    return tempUser;
                }
                else
                {
                    throw new Exception("User Id not found");
                }
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
