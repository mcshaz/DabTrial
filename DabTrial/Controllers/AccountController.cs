using System;
using System.Web.Mvc;
using System.Web.Security;
using DabTrial.Models;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize]
    public class AccountController : DataContextController
    {
        //
        // GET: /Account/LogOn
        [AllowAnonymous]
        //[InitializeSimpleMembership]
        public ActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                int attempts;
                if (WebSecurity.Login(model.UserName, model.Password, out attempts, persistCookie: model.RememberMe))
                {
                    return RedirectToLocal(returnUrl);
                }
                if (attempts >= DabTrial.CustomMembership.CodeFirstMembershipProvider.FixedMaxInvalidPasswordAttempts)
                {
                    ModelState.AddModelError("", "You have been locked out. Please contact an investigator from your site to be unlocked.");
                    return View();
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                if (model.Password == model.Password.ToUpperInvariant())
                {
                    ModelState.AddModelError("", "PASSWORD PROVIDED IS ALL CAPITALS - please check you do not have the caps lock key on by mistake.");
                }
                if (DabTrial.CustomMembership.CodeFirstMembershipProvider.FixedMaxInvalidPasswordAttempts-1 == attempts)
                {
                    ModelState.AddModelError("", "You have only 1 more attempt before being locked out. If you have forgotten your password, please click the link to have a new one emailed before being locked out.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            //check if siteRegistrationPwd is valid
            // Attempt to register the user
            if (!UserService.CanRegister())
            {
                ModelState.AddModelError("", "Unable to register from this location");
            }
            if (ModelState.IsValid)
            {
                UserService.SelfRegisterClinician(model.UserName, model.Email, model.Password, model.SiteSpecificPassword, model.FirstName, model.LastName, model.ProfessionalRole);
                if (ModelState.IsValid)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Enrol", "Participant");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                /*
                catch (Exception e)
                {
                    changePasswordSucceeded = false;
                }
                */
                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AllowAnonymous]
        public ActionResult EmailPasswordSuccess()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult EmailPassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EmailPassword(EmailPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                UserService.EmailNewPassword(model.Email);
                if (ModelState.IsValid)
                {
                    return RedirectToAction("EmailPasswordSuccess");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        [AutoMapModel(typeof(User), typeof(InvestigatorEditBrief))]
        public ActionResult Edit()
        {
            var user = UserService.GetUser(CurrentUserName);
            var role = user.InvestigatorRole();
            if (role == InvestigatorRole.EnrollingClinician)
            {
                return View(user);
            }
            return RedirectToAction("Edit", "InvestigatorAccount", new { id = CurrentUserName });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(InvestigatorEditBrief model)
        {
            if (ModelState.IsValid)
            {
                UserService.UpdateSelf(CurrentUserName,
                    model.Email,
                    model.FirstName,
                    model.LastName,
                    model.ProfessionalRole.Value);
                return RedirectToAction("Details");
            }
            return View(model);
        }

        [AutoMapModel(typeof(User), typeof(InvestigatorDetailsBrief))]
        public ActionResult Details()
        {
            return View(UserService.GetUser(CurrentUserName));
        }
        //
        // GET: /Account/ChangePasswordSuccess
        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
