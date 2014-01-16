using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using DabTrial.Utilities;
using DabTrial.Domain.Services;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Domain.Providers;

namespace DabTrial.Controllers
{
    public class ContactUsController : Controller, IDisposable
    {
        //
        // GET: /ContactUs/
        private ContactService _contactUsService;
        private ContactService ContactUsService
        {
            get { return _contactUsService ?? (_contactUsService = new ContactService(new ModelStateWrapper(this.ModelState), new  DataContext())); }
        }

        public ActionResult Index()
        {
            var model = new InvestigatorContactList();
            model.ContactList = Mapper.Map <IEnumerable<InvestigatorContact>>(ContactUsService.GetAdministrators());
            return View(model);
        }
        public ActionResult MailTo(int id, string name, string role, string hospital)
        {
            var model = new MailInvestigator { InvestigatorId = id, 
                                               DisplayInfo = new MailInvestigator.InvestigatorInfo { Name = name, 
                                               Role = role, 
                                               Hospital = hospital }
                                             };
            if (Request.IsAjaxRequest()) { return PartialView(model); }
            return View(model);
        }
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult MailTo(MailInvestigator model, int id, string name, string role, string hospital)
        {
            if (ModelState.IsValid)
            {
                ContactUsService.sendMail(model.InvestigatorId, model.Email, model.Subject, model.Message);
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid) {
                        string redirectUrl =  Url.Action("Success", new { recipientName = name });
                        return Json(new { redirectUrl = redirectUrl });
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                return RedirectToAction("Success", new {recipientName = name});
            }
            model.DisplayInfo = new MailInvestigator.InvestigatorInfo { Name = name, Role = role, Hospital = hospital };
            return View(model);
        }
        public ActionResult Success(string recipientName)
        {
            ViewBag.Recipient = recipientName;
            return View();
        }
        protected override void Dispose(bool disposing)
        {
            if (_contactUsService!=null) _contactUsService.Dispose();
            base.Dispose(disposing);
        }
    }
}
