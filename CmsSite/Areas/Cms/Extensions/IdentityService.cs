using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CmsSite.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CmsSite.Areas.Cms.Extensions
{
    public class IdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private static RoleManager<IdentityRole> _staticRoleManager;
        private static UserManager<ApplicationUser> _staticUserManager;

        public IdentityService(DbContext context)
        {
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            _staticRoleManager = _roleManager;
            _staticUserManager = _userManager;
        }
        public List<ApplicationUser> GetUsers()
        {
            var users = _userManager.Users.Include(u => u.Roles).ToList();
            return users;
        }

        public static List<IdentityRole> GetRoles()
        {
            return _staticRoleManager.Roles.ToList();
        }


        //GetRoleName

        public static string GetRoleName(string id)
        {
            return _staticRoleManager.Roles.First(r => r.Id == id).Name;
        }

        //GetRoleId

        public static string GetRoleId(string name)
        {
            return _staticRoleManager.Roles.First(r => r.Name == name).Id;
        }

        public static bool IsUserInRole(string userid, string rolename)
        {
            return _staticUserManager.IsInRole(userid, rolename);

        }

        public bool RoleExists(string rolename)
        {
            return _roleManager.RoleExists(rolename);
        }


        //Get UserByEmail
        public ApplicationUser GetUserByEmail(string email)
        {
            var user = _userManager.FindByEmail(email);
            return user;
        }


        //Get UserByName
        public bool CreateUser(string username, string email, string password)
        {
           var iResult = _userManager.Create(new ApplicationUser {Email = email, UserName = username.Replace(" ", "") }, password);
            return iResult.Succeeded;
        }

        public bool CreateUser(string username, string email, string password, string phone)
        {
            var iResult = _userManager.Create(new ApplicationUser { Email = email, UserName = username.Replace(" ",""), PhoneNumber = phone}, password);
            return iResult.Succeeded;
        }


        public bool DeleteUser(string id)
        {
            var user = _userManager.FindById(id);
            var iResult = _userManager.Delete(user);
            return iResult.Succeeded;
        }



        public bool CreateRole(string rolename)
        {
            var iResult = _roleManager.Create(new IdentityRole { Name = rolename });
            return iResult.Succeeded;
        }

        public bool AddUserToRole(string userid, string rolename)
        {
          var iResult =   _userManager.AddToRole(userid, rolename);
            return iResult.Succeeded;
        }



        public bool RemoveUserFromRole(string userid, string rolename)
        {
            var iResult = _userManager.RemoveFromRole(userid, rolename);
            return iResult.Succeeded;
        }




        //DeleteRole

        public bool DeleteRole(string roleName)
        {
            var role = _roleManager.FindByName(roleName);
            var users = role.Users;
            foreach (var user in users)
            {
                RemoveUserFromRole(user.UserId, role.Name);
            }
            var iResult = _roleManager.Delete(role);
            return iResult.Succeeded;
        }



        //GetById
    public string GetRoleById(string id)
        {
            var rolename = _roleManager.FindById(id);
            return rolename.Name;
        }

        //ClearUserRoles

        public void ClearUserRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();
            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                _userManager.RemoveFromRole(userId, role.RoleId);
            }
        }


        public static IdentityService Create(DbContext context)
        {
            return new IdentityService(context);
        }
    }
}