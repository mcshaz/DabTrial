using System;
using System.Net;
using System.Collections.Generic;
using System.Web.Mvc;
using DabTrial.Models;
using DabTrial.Utilities;
using AutoMapper;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize]
    public class ProtocolViolationController : ParticipantRelatedController, IDisposable
    {
        private ProtocolViolationService _violationService;
        private ProtocolViolationService ViolationService {get {return _violationService ?? (_violationService = new ProtocolViolationService(ValidationDictionary, dbContext));}}

        //
        // GET: /ProtocolViolation/
        [AutoMapModel(typeof(IEnumerable<ProtocolViolation>),typeof(IEnumerable<ProtocolViolationListItem>))]
        public ActionResult Index()
        {
            IEnumerable<ProtocolViolation> model;
            switch (CurrentUser.InvestigatorRole())
            {
                case InvestigatorRole.PrincipleInvestigator:
                    model = ViolationService.GetAllViolations(); 
                    break;
                case InvestigatorRole.SiteInvestigator:
                    model = ViolationService.GetViolationsByCentre(CurrentUser.StudyCentreId.Value);
                    break;
                default:
                    return RedirectToAction("LogOn", "Account");
            }
            return View(model);
        }

        //
        // GET: /ProtocolViolation/Details/5
        [AutoMapModel(typeof(ProtocolViolation), typeof(ProtocolViolationDetails))]
        public ActionResult Details(int id)
        {
            var model = ViolationService.GetViolation(id);
            if (Request.IsAjaxRequest()) { return PartialView(model); }
            return View(model); 
        }

        //
        // GET: /ProtocolViolation/Create

        public ActionResult Create(int id)
        {
            var participant = GetParticipant(id);
            var model = new ProtocolViolationCreate()
            {
                ParticipantId = id,
                TrialParticipantLocalTimeRandomised = participant.LocalTimeRandomised
            };
            if (IsAjax)
            {
                return PartialView(model);
            }
            else
            {
                model.TrialParticipantHospitalId = participant.HospitalId;
                return View(model);
            }
        } 

        //
        // POST: /ProtocolViolation/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProtocolViolationCreate model)
        {

            if (ModelState.IsValid)
            {
                ViolationService.CreateViolation(model.ParticipantId, model.TimeOfViolation.Value, model.MajorViolation.Value, model.Details, CurrentUserName);
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        return new EmptyResult();
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            return View(model);
        }
        
        //
        // GET: /ProtocolViolation/Edit/5
        [AutoMapModel(typeof(ProtocolViolation), typeof(ProtocolViolationEdit))]
        public ActionResult Edit(int id)
        {
            var model = ViolationService.GetViolation(id, CurrentUserName);
            if (IsAjax)
            {
                return PartialView(model);
            }
            return View(model);
        }

        //
        // POST: /ProtocolViolation/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ProtocolViolationEdit model)
        {
            if (ModelState.IsValid)
            {
                var violationEvent = ViolationService.UpdateViolation(model.ViolationId, model.TimeOfViolation, model.MajorViolation, model.Details, CurrentUserName);
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        return PartialView("_IndexRow", Mapper.Map<ProtocolViolationListItem>(violationEvent));
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            return View(model);
        }

        //
        // GET: /ProtocolViolation/Delete/5
        [AutoMapModel(typeof(ProtocolViolation), typeof(ProtocolViolationDetails))]
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        public ActionResult Delete(int id)
        {
            return View(ViolationService.GetViolation(id));
        }

        //
        // POST: /ProtocolViolation/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int response = Response.StatusCode = (int)ViolationService.Delete(id, CurrentUserName);
            if (Request.IsAjaxRequest())
            {
                if (response == (int)HttpStatusCode.OK) { return Json(id); }
                return new EmptyResult();
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_violationService != null) _violationService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
