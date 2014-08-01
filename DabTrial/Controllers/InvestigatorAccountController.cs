using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize(Roles=RoleExtensions.PrincipleInvestigator + "," + RoleExtensions.SiteInvestigator)]
    public class InvestigatorAccountController : DataContextController
    {
        //
        // GET: /InvestigatorAccount/
        [AutoMapModel(typeof(IEnumerable<User>), typeof(IEnumerable<InvestigatorListItem>))]
        public ActionResult Index()
        {
            IEnumerable<User> model;
            switch (CurrentUser.InvestigatorRole())
            {
                case InvestigatorRole.PrincipleInvestigator:
                    model = UserService.GetAllUsers();
                    break;
                case InvestigatorRole.SiteInvestigator:
                    model = UserService.GetUsersFromSameCentre(CurrentUser);
                    break;
                default:
                    return RedirectToAction("LogOn", "Account");
            }
            return View(model);
        }

        //
        // GET: /InvestigatorAccount/Details/5
        [AutoMapModel(typeof(User), typeof(InvestigatorDetailsFull))]
        public ActionResult Details(string id)
        {
            var model = UserService.GetUser(id);
            return View(model);
        }

        //
        // GET: /InvestigatorAccount/Create
        public ActionResult Create()
        {
            var model = new InvestigatorCreateEdit();
            SetLists(model);
            return View(model);
        } 

        //
        // POST: /InvestigatorAccount/Create

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Create(InvestigatorCreateEdit model)
        {
            if (ModelState.IsValid)
            {
                UserService.CreateUser(userNameMakingChanges: CurrentUserName,
                    userName:model.UserName,
                    email:model.Email,
                    firstName:model.FirstName,
                    lastName:model.LastName,
                    studyCentreId:model.StudyCentreId.Value,
                    comment:model.Comment,
                    professionalRole:model.ProfessionalRole.Value,
                    dbAdmin: model.IsDbAdmin,
                    roleName: model.Role,
                    isPublicContact:model.IsPublicContact,
                    password:null,
                    isApproved:true);
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            SetLists(model);
            return View(model);
        }
        
        //
        // GET: /InvestigatorAccount/Edit/BrentM
        public ActionResult Edit(string id)
        {
            var model = Mapper.Map<InvestigatorCreateEdit>(UserService.GetUser(id));
            SetLists(model);
            return View(model);
        }

        //
        // POST: /InvestigatorAccount/Edit/5

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Edit(InvestigatorCreateEdit model)
        {
            if (ModelState.IsValid) {
                UserService.UpdateUser(
                    userNameMakingChanges: CurrentUserName,
                    userName:model.UserName,
                    email:model.Email,
                    firstName:model.FirstName,
                    lastName:model.LastName,
                    dbAdmin: model.IsDbAdmin,
                    comment:model.Comment,
                    professionalRole:model.ProfessionalRole.Value,
                    roleName: model.Role,
                    isPublicContact: model.IsPublicContact,
                    isLockedOut:model.IsLockedOut,
                    isDeactivated:model.IsDeactivated);
                if (ModelState.IsValid) { return RedirectToAction("Index");}
            } 
            SetLists(model);
            return View(model);
        }

        //
        // GET: /InvestigatorAccount/Delete/5
        [AutoMapModel(typeof(User), typeof(InvestigatorDetailsFull))]
        public ActionResult Delete(string id)
        {
            return View(UserService.GetUser(id));
        }

        //
        // POST: /InvestigatorAccount/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, InvestigatorListItem model)
        {
            if (ModelState.IsValid) {
                UserService.DeleteUser(CurrentUserName, id);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index"); 
                }
            }
            return View(model);
        }
        private void SetLists(InvestigatorCreateEdit model)
        {
            // create your menu with all roles : for multi select
            var allRoles = UserService.GetAllRoles().Where(r=>r.RoleName!=RoleExtensions.DbAdminName);
            var currentUser = UserService.GetUser(CurrentUserName);
            var pi = currentUser.IsPrincipleInvestigator();
            model.CanAssignDbAdmin = currentUser.IsDbAdmin();
            if (allRoles!=null) {
                if (!pi) 
                {
                    if (model.Role == RoleExtensions.PrincipleInvestigator)
                    {
                        allRoles = allRoles.Where(r => r.RoleName == RoleExtensions.PrincipleInvestigator);
                    }
                    else
                    {
                        allRoles = allRoles.Where(r => r.RoleName != RoleExtensions.PrincipleInvestigator);
                    }
                }
                model.RolesSelectList = allRoles.Select(
                    r => new SelectListItem()
                    {
                        Value=r.RoleName,
                        Text=r.GetRoleDescription(),
                        Selected = (model.Role==null)?false:model.Role.Contains(r.RoleName)
                    }
                );
            }
            List<StudyCentre> centresForList;
            if (pi) { 
                centresForList = UserService.GetAllCentres().ToList();  
            }
            else { 
                centresForList = new List<StudyCentre>();
                centresForList.Add(currentUser.StudyCentre);
            }
            model.StudyCentresSelectList = new SelectList(centresForList, "StudyCentreId", "Name", model.StudyCentreId ?? currentUser.StudyCentreId);
            
        }
    }
}
