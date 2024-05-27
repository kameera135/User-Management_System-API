using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SITCAMSClientIntegration.Configurations;
using SITCAMSClientIntegration.Contexts;
using SITCAMSClientIntegration.Models;
using SITCAMSClientIntegration.Options;
using SITCAMSClientIntegration.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SITCAMSClientIntegration.Repositories
{
    /// <summary>
    /// Concrete implementation of the <see cref="IUserRepository" />
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly CAMSDBContext _context;
        private readonly CentralAuthOptions _centralAuthOptions;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context">The CAMS context</param>
        /// <param name="centralAuthOptions">The Central CAMS configuration</param>
        public UserRepository(CAMSDBContext context, IOptionsSnapshot<CentralAuthOptions> centralAuthOptions)

        {
            _context = context;
            _centralAuthOptions = centralAuthOptions.Value;
        }

        /// <inheritdoc />
        public bool IsAbleTo(int id, string role, string permission)
        {
            if (role == RoleConfig.Level1)
            {
                return true;
            }
            User user = _context.Users.Where(x => x.UserId == id)
                .Include(x => x.Role)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefault();
            if (user == null) return false;
            return user.Role.Permissions.Select(p => p.Name).Contains(permission);
        }

        /// <inheritdoc />
        public bool IsAbleTo(string userName, string role, string permission)
        {
            if (role == RoleConfig.Level1)
            {
                return true;
            }
            User user = _context.Users.Where(x => x.UserName == userName.ToLower())
                .Include(x => x.Role)
                    .ThenInclude(r => r.Permissions)
                .FirstOrDefault();
            if (user == null) return false;
            return user.Role.Permissions.Select(p => p.Name).Contains(permission);
        }

        /// <inheritdoc />
        public User GetUserWithRole(int userId)
        {
            return _context.Users.Where(u => u.UserId == userId).Include(u => u.Role).FirstOrDefault();
        }

        /// <inheritdoc />
        public User GetUserWithRole(string username)
        {
            username = username.ToLower();
            return _context.Users.Where(u => u.UserName == username)
                .Include(u => u.Role).FirstOrDefault();
        }

        /// <inheritdoc />
        public void UpdateContext(int userId, string userName, string roleName, int updatedRank, List<string> permissions)
        {
            userName = userName.ToLower();
            List<Permission> newPermissions = new List<Permission>();
            foreach (string permission in permissions)
            {
                Permission existing = _context.Permissions.Where(p => p.Name == permission).FirstOrDefault();
                if (existing == null)
                {
                    existing = new Permission { Name = permission };
                    _context.Permissions.Add(existing);
                    _context.SaveChanges();
                }
                newPermissions.Add(existing);
            }

            // Add or update the role
            Role role = _context.Roles
                .Where(r => r.Name == roleName)
                .Include(r => r.Permissions).FirstOrDefault();

            if (role == null)
            {
                role = new Role
                {
                    Name = roleName,
                    Rank = updatedRank,
                    Permissions = newPermissions
                };
                _context.Add(role);
                _context.SaveChanges();
            }
            else
            {
                if (role.Rank != updatedRank)
                {
                    role.Rank = updatedRank;
                    _context.SaveChanges();
                }
                if (permissions.Count == 0) { }
                else if (!(permissions.OrderBy(p => p)
                    .SequenceEqual(role.Permissions.Select(p => p.Name).OrderBy(p => p))))
                {
                    // Update the permissions
                    role.Permissions = newPermissions;
                    _context.SaveChanges();
                }
            }
            // Update the user
            User user = null;
            if (_centralAuthOptions.UseUserName)
                user = _context.Users.Where(u => u.UserName == userName).FirstOrDefault();
            else user = _context.Users.Where(u => u.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                _context.Add(new User
                {
                    UserId = userId,
                    UserName = userName,
                    Role = role
                });
                _context.SaveChanges();
            }
            else if (user.Role != role)
            {
                user.Role = role;
                _context.SaveChanges();
            }
        }

        /// <inheritdoc />
        public int GetEndPointRank(string endpoint)
        {

            string[] segments = endpoint.Split("/");
            int rank = RankConfig.DefaultRank;
            string subPath = "";
            foreach (string segment in segments)
            {
                if (segment.Length == 0) continue;
                subPath += segment + "/";
                int tmpRank = _context.EndPointRanks
                    .Where(e => e.EndPoint == subPath || e.EndPoint + "/" == subPath).Select(e => e.Rank).FirstOrDefault();
                rank = tmpRank == 0 ? rank : tmpRank;
            }
            return rank;
        }


    }
}
