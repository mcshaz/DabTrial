using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using DabTrial.Models;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;

namespace DabTrial.CustomMembership
{
    public class CodeFirstRoleProvider : RoleProvider
    {
        public override string ApplicationName
        {
            get
            {
                return this.GetType().Assembly.GetName().Name.ToString();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool RoleExists(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (var context = new DataContext())
            {
                Role role = context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (role != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (var context = new DataContext())
            {
                User user = context.Users.FirstOrDefault(Usr => Usr.UserName == username);
                if (user == null)
                {
                    return false;
                }
                Role role = context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (role == null)
                {
                    return false;
                }
                return user.Roles.Contains(role);
            }
        }

        public override string[] GetAllRoles()
        {
            using (var context = new DataContext())
            {
                return context.Roles.Select(Rl => Rl.RoleName).ToArray();
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }
            using (var context = new DataContext())
            {
                Role role = null;
                role = context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (role != null)
                {
                    return role.Users.Select(Usr => Usr.UserName).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }
            using (var Context = new DataContext())
            {
                User user = Context.Users.FirstOrDefault(Usr => Usr.UserName == username);
                if (user != null)
                {
                    return user.Roles.Select(Rl => Rl.RoleName).ToArray();
                }
                else
                {
                    return null;
                }
            }
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return null;
            }

            if (string.IsNullOrEmpty(usernameToMatch))
            {
                return null;
            }

            using (var Context = new DataContext())
            {
                return (from Rl in Context.Roles from Usr in Rl.Users where Rl.RoleName == roleName && Usr.UserName.Contains(usernameToMatch) select Usr.UserName).ToArray();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                using (var context = new DataContext())
                {
                    Role role = context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                    if (role == null)
                    {
                        Role NewRole = new Role
                        {
                            //RoleId = Guid.NewGuid(), Identity (DB assigned)
                            RoleName = roleName
                        };
                        context.Roles.Add(NewRole);
                        context.SaveChanges();
                    }
                }
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return false;
            }
            using (var context = new DataContext())
            {
                Role role = context.Roles.FirstOrDefault(Rl => Rl.RoleName == roleName);
                if (role == null)
                {
                    return false;
                }
                if (throwOnPopulatedRole)
                {
                    if (role.Users.Any())
                    {
                        return false;
                    }
                }
                else
                {
                    role.Users.Clear();
                }
                context.Roles.Remove(role);
                context.SaveChanges();
                return true;
            }
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (var context = new DataContext())
            {
                List<User> users = context.Users.Where(Usr => usernames.Contains(Usr.UserName)).ToList();
                List<Role> roles = context.Roles.Where(Rl => roleNames.Contains(Rl.RoleName)).ToList();
                foreach (User user in users)
                {
                    foreach (Role role in roles)
                    {
                        if (!user.Roles.Contains(role))
                        {
                            user.Roles.Add(role);
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (var Context = new DataContext())
            {
                foreach (String username in usernames)
                {
                    User user = Context.Users.FirstOrDefault(U => U.UserName == username);
                    if (user != null)
                    {
                        foreach (String roleName in roleNames)
                        {
                            Role role = user.Roles.FirstOrDefault(R => R.RoleName == roleName);
                            if (role != null)
                            {
                                user.Roles.Remove(role);
                            }
                        }
                    }
                }
                Context.SaveChanges();
            }
        }
    }
}