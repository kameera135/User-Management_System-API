    using Data;
using UMS.Api.Domain.DTO;
using UMS.Api.Interfaces;
using UMS.Api.Models;

namespace UMS.Api.Repositories
{
    public class PlatformConfigurationRepository : IPlatformConfigurationRepository
    {
        private readonly IRepository m_dbContext;
        private readonly UmsContext m_umsContext;

        public PlatformConfigurationRepository(IRepository dbContext, UmsContext umsContext)
        {
            m_dbContext = dbContext;
            m_umsContext = umsContext;
        }

        public List<ComboboxDTO> getPlatformList()
        {
            try
            {
                List<Platform> platforms = m_dbContext.Get<Platform>().Where(q => q.DeletedAt == null).ToList();

                List<ComboboxDTO> lstResult = new List<ComboboxDTO>();

                foreach (Platform platform in platforms)
                {
                    ComboboxDTO tempPlatform = new()
                    {
                        Id = platform.PlatformId,
                        Value = platform.PlatformName,
                    };

                    lstResult.Add(tempPlatform);
                }

                return lstResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<PlatformDTO> getPlatforms(string? searchedPlatformName)
        {
            try
            {
                List<Platform> platforms = new List<Platform>();

                if (searchedPlatformName != null)
                {
                    platforms = m_dbContext.Get<Platform>().Where(q => q.DeletedAt == null && q.PlatformName.ToLower().Contains(searchedPlatformName.ToLower())).ToList();
                }
                else
                {
                    platforms = m_dbContext.Get<Platform>().Where(q => q.DeletedAt == null).ToList();
                }

                List<PlatformDTO> lstResult = new List<PlatformDTO>();

                foreach (Platform platform in platforms)
                {
                    PlatformDTO tempPlatform = new()
                    {
                        PlatformId = platform.PlatformId,
                        PlatformName = platform.PlatformName,
                        PlatformCode = platform.PlatformCode.ToUpper(),
                        Description = platform.Description,
                        PlatformUrl = platform.PlatformUrl,
                    };

                    lstResult.Add(tempPlatform);
                }

                return lstResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void postPlatform(PlatformPostDTO platform, long createdBy)
        {
            try
            {
                Platform tempPlatformObj = new Platform
                {
                    PlatformCode = platform.PlatformCode.ToUpper(),
                    PlatformName = platform.PlatformName,
                    PlatformUrl = platform.PlatformUrl,
                    Description = platform.Description,
                    ExternalLink = platform.ExternalLink,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy,
                };

                m_dbContext.Create(tempPlatformObj);
                m_dbContext.Save();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void updatePlatform(PlatformPutDTO platform, long updatedBy)
        {
            try
            {
                Platform exsistingResult = m_dbContext.GetById<Platform>(platform.PlatformId);

                if (exsistingResult != null && exsistingResult.PlatformId == platform.PlatformId)
                {
                    exsistingResult.PlatformName = platform.PlatformName;
                    exsistingResult.PlatformCode = platform.PlatformCode.ToUpper();
                    exsistingResult.PlatformUrl = platform.PlatformUrl;
                    exsistingResult.Description = platform.Description;
                    exsistingResult.UpdatedAt = DateTime.Now;
                    exsistingResult.UpdatedBy = updatedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Platform Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void deletePlatform(long platformId, long deletedBy)
        {
            try
            {
                Platform exsistingResult = m_dbContext.GetById<Platform>(platformId);

                if (exsistingResult != null && exsistingResult.PlatformId == platformId)
                {
                    exsistingResult.DeletedAt = DateTime.Now;
                    exsistingResult.DeletedBy = deletedBy;

                    m_dbContext.Update(exsistingResult);
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Platform Not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //GET PLATFORMS FOR GIVEN ROLE ID
        public List<PlatformsOfRoleDTO> getPlatformsByRole(long roleId)
        {
            try
            {
                List<PlatformsOfRoleDTO> lstResult = new List<PlatformsOfRoleDTO>();

                var rolesAccordingToPlatforms = ((from roles in m_umsContext.Roles
                                                  join platforms in m_umsContext.Platforms on roles.PlatformId equals platforms.PlatformId
                                                  where roles.RoleId == roleId && roles.DeletedAt == null && platforms.DeletedAt == null
                                                  select new PlatformsOfRoleDTO
                                                  {
                                                      PlatformId = platforms.PlatformId,
                                                      PlatformName = platforms.PlatformName,
                                                  }).ToList());

                lstResult = rolesAccordingToPlatforms.ToList();

                return lstResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<PlatformUsersDTO> getPlatformUsers(long platformId, string? searchedUserName)
        {
            try
            {
                List<PlatformUsersDTO> lstResult = new List<PlatformUsersDTO>();

                if (searchedUserName != null)
                {
                    lstResult = (from users in m_umsContext.Users
                                 join userPlatform in m_umsContext.UserPlatforms on users.UserId equals userPlatform.UserId
                                 join platform in m_umsContext.Platforms on userPlatform.PlatformId equals platform.PlatformId
                                 where (users.DeletedAt == null && userPlatform.DeletedAt == null && platform.DeletedAt == null && platform.PlatformId == platformId && (users.UserName.ToLower().Contains(searchedUserName.ToLower()) || users.FirstName.ToLower().Contains(searchedUserName.ToLower()) || users.LastName.ToLower().Contains(searchedUserName.ToLower())))
                                 select new PlatformUsersDTO
                                 {
                                     UserId = users.UserId,
                                     UserName = users.UserName,
                                     FirstName = users.FirstName,
                                     LastName = users.LastName,
                                     PhoneNumber = users.Phone,
                                     Email = users.Email,
                                     PlatformId = platform.PlatformId,
                                     PlatformName = platform.PlatformName
                                 }).ToList();
                }
                else
                {
                    lstResult = (from users in m_umsContext.Users
                                 join userPlatform in m_umsContext.UserPlatforms on users.UserId equals userPlatform.UserId
                                 join platform in m_umsContext.Platforms on userPlatform.PlatformId equals platform.PlatformId
                                 where (users.DeletedAt == null && userPlatform.DeletedAt == null && platform.DeletedAt == null && platform.PlatformId == platformId)
                                 select new PlatformUsersDTO
                                 {
                                     UserId = users.UserId,
                                     UserName = users.UserName,
                                     FirstName = users.FirstName,
                                     LastName = users.LastName,
                                     PhoneNumber = users.Phone,
                                     Email = users.Email,
                                     PlatformId = platform.PlatformId,
                                     PlatformName = platform.PlatformName
                                 }).ToList();
                }

                return lstResult;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void assignPlatformUsers(AssignPlatformUsersDTO user, long createdBy)
        {
            try
            {
                Platform? platform = m_umsContext.Platforms.FirstOrDefault(p => p.PlatformId == user.PlatformId);

                if (platform != null)
                {
                    foreach (long userId in user.UserIds)
                    {
                        User? tempUser = m_umsContext.Users.FirstOrDefault(u => u.UserId == userId);

                        if (tempUser != null)
                        {
                            // Check if the entry already exists
                            bool entryExists = m_umsContext.UserPlatforms
                                .Any(up => up.UserId == userId && up.PlatformId == user.PlatformId && (up.DeletedAt == null || up.DeletedAt <= DateTime.Now));

                            if (!entryExists)
                            {
                                UserPlatform userPlatform = new UserPlatform
                                {
                                    UserId = userId,
                                    PlatformId = user.PlatformId,
                                    CreatedBy = createdBy,
                                    CreatedAt = DateTime.Now,
                                };

                                m_dbContext.Create(userPlatform);
                            }
                            else
                            {
                                UserPlatform? existingUserPlatform = m_umsContext.UserPlatforms.FirstOrDefault(up => up.UserId == userId && up.PlatformId == user.PlatformId);

                                // If the entry already exists, update DeletedAt to null
                                existingUserPlatform.DeletedAt = null;
                                existingUserPlatform.DeletedBy = null;
                                m_dbContext.Update(existingUserPlatform);
                            }
                        }
                        else
                        {
                            throw new Exception("User not found");
                        }
                    }
                    m_dbContext.Save();
                }
                else
                {
                    throw new Exception("Platform not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void unassignPlatformUsers(AssignPlatformUsersDTO user, long deletedBy)
        {
            try
            {
                Platform? plaform = m_umsContext.Platforms.FirstOrDefault(p => p.PlatformId == user.PlatformId);

                if (plaform != null)
                {
                    foreach (long userId in user.UserIds)
                    {
                        User? tempUser = m_umsContext.Users.FirstOrDefault(u => u.UserId == userId);

                        if (tempUser != null)
                        {
                            UserPlatform? userPlatform = m_umsContext.UserPlatforms.FirstOrDefault(up => up.UserId == userId && up.PlatformId == user.PlatformId);

                            if (userPlatform != null)
                            {
                                userPlatform.DeletedAt = DateTime.Now;
                                userPlatform.DeletedBy = deletedBy;

                                m_dbContext.Update(userPlatform);
                                m_dbContext.Save();
                            }
                            else
                            {
                                throw new Exception("User not assigned to the given platform");
                            }
                        }
                        else
                        {
                            throw new Exception("User not found");
                        }
                    }
                }
                else
                {
                    throw new Exception("There are one or more platforms not in the platform list.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}