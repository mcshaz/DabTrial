using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;

namespace DabTrial.Models
{
    public class RoleMenuItem: MenuItem
    {
        public RoleMenuItem(){}
        public RoleMenuItem(string linkText, string actionName, string controllerName, string roleNames)
        {
            LinkText = linkText;
            ActionName = actionName;
            ControllerName = controllerName;
            RoleNames = roleNames;
        }
        public string RoleNames { set { Roles = value.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries); } }
        internal string[] Roles;
    }
    public class MenuItem
    {
        public string LinkText { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
    }
    public class RoleMenu : System.Collections.Generic.IEnumerable<MenuItem>
    {
        private readonly List<RoleMenuItem> _roleMenuItems = new List<RoleMenuItem>();
        private readonly string[] _userRoleNames;
        public readonly bool _isAuthenticated;
        public RoleMenu()
        {
            if (_isAuthenticated = WebSecurity.User.Identity.IsAuthenticated)
            {
                _userRoleNames = Roles.GetRolesForUser();
            }
        }
        public RoleMenu(IDataContext context, string userName=null)
        {
            if (_isAuthenticated = WebSecurity.User.Identity.IsAuthenticated)
            {
                if (userName == null) { userName = HttpContext.Current.User.Identity.Name; }
                User usr = context.Users.FirstOrDefault(Usr => Usr.UserName == userName);
                _userRoleNames = (usr == null) ? new string[0] : usr.Roles.Select(r => r.RoleName).ToArray();
            }
        }
        public RoleMenu Add(RoleMenuItem menuItem)
        {
            string[] menuRoles = menuItem.Roles;
            if (
                    menuRoles.Contains("All" ) ||
                    (!_isAuthenticated && menuRoles.Contains("Anonymous")) ||
                    (_isAuthenticated && (menuRoles.Contains("Authenticated") || menuRoles.Any(mr=>_userRoleNames.Contains(mr))))
                )
            {
                _roleMenuItems.Add(menuItem);
            }
            return this;
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public System.Collections.Generic.IEnumerator<MenuItem> GetEnumerator()
        {
            return _roleMenuItems.GetEnumerator();
        } 
        public IEnumerable<MenuItem> ItemsForRole(string roleName)
        {
            return _roleMenuItems.Where(r => r.Roles.Contains(roleName));
        }
        public IEnumerable<MenuItem> ItemsForExclusiveRole(string roleName)
        {
            return _roleMenuItems.Where(r => r.Roles.Length==1 && r.Roles[0]== roleName);
        }
    }
}