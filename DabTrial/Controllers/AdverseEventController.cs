using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using MvcHtmlHelpers;
using DabTrial.Utilities;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize]
    public class AdverseEventController : ParticipantRelatedController, IDisposable
    {
        private AdverseEventService _advEventService;
        private AdverseEventService AdvEventService { get { return _advEventService ?? (_advEventService=new AdverseEventService(ValidationDictionary, dbContext)); } }
        private AdverseEventTypeService _eventTypeService;
        private AdverseEventTypeService EventTypeService { get { return _eventTypeService ?? (_eventTypeService = new AdverseEventTypeService(ValidationDictionary, dbContext)); } }

        [AutoMapModel(typeof(IEnumerable<AdverseEvent>), typeof(IEnumerable<AdverseEventListItem>))]
        public ActionResult Index()
        {
            var restrict = UserService.GetUser(CurrentUserName).RestrictedToCentre();
            if (!restrict.HasValue)
            {
                return View(AdvEventService.GetAllAdverseEvents());
            }
            return View(AdvEventService.GetAdverseEventsByCentre(restrict.Value));
        }
        [AutoMapModel(typeof(AdverseEvent), typeof(AdverseEventDetails))]
        public ActionResult Details(int id)
        {
            var model = AdvEventService.GetAdverseEvent(id);
            if (IsAjax) { return PartialView(model); }
            return View(model); 
        }
        public ActionResult Create(int id, int? severity, bool? fatal)
        {
            var participant = GetParticipant(id);
            var model = new AdverseEventCreateModel()
            {
                SeverityLevelId = severity, 
                ParticipantId = id,
                FatalEvent = fatal ?? false,
                TrialParticipantLocalTimeRandomised = participant.LocalTimeRandomised
            };
            setLists(model);
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
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Create(int id, AdverseEventCreateModel model)
        {
            if (ModelState.IsValid)
            {
                var adverseEvnt = AdvEventService.CreateAdverseEvent(model.ParticipantId,
                                                                model.EventTime.Value,
                                                                model.SeverityLevelId.Value,
                                                                model.AdverseEventTypeId.Value,
                                                                model.Sequelae,
                                                                model.FatalEvent,
                                                                model.Details,
                                                                CurrentUserName);
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        return PartialView("Details", Mapper.Map<AdverseEventDetails>(adverseEvnt) );
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                if (ModelState.IsValid) { return RedirectToAction("Details", new { adverseEventId = adverseEvnt.AdverseEventId }); }
            };
            setLists(model);
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            var model = Mapper.Map<AdverseEventEditModel>(AdvEventService.GetAdverseEvent(id,CurrentUserName));
            setLists(model);
            if (IsAjax)
            {
                return PartialView(model);
            }
            else
            {
                return View(model);
            }
        }
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Edit(int id, AdverseEventEditModel model)
        {
            if (ModelState.IsValid)
            {
                var adverseEvent = AdvEventService.UpdateAdverseEvent(model.AdverseEventId,
                                                            model.EventTime.Value,
                                                            model.SeverityLevelId.Value,
                                                            model.AdverseEventTypeId.Value,
                                                            model.Sequelae,
                                                            model.FatalEvent,
                                                            model.Details,
                                                            CurrentUserName);
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        return PartialView("_IndexRow", Mapper.Map<AdverseEventListItem>(adverseEvent));
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            setLists(model);
            return View(model); 
        }
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        [AutoMapModel(typeof(AdverseEvent), typeof(AdverseEventDetails))]
        public ActionResult Delete(int id)
        {
            return View(AdvEventService.GetAdverseEvent(id,CurrentUserName));
        }

        //
        // POST: /ProtocolViolation/Delete/5
        [Authorize(Roles=RoleExtensions.PrincipleInvestigator)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int response = Response.StatusCode = (int)AdvEventService.Delete(id, CurrentUserName);
            if (Request.IsAjaxRequest())
            {
                if (response == (int)HttpStatusCode.OK) { return Json(id); }
                return new EmptyResult();
            }
            return RedirectToAction("Index");
        }
        private void setLists(IAdverseEventModel model)
        {
            //if (model.TrialParticipantHospitalId == null) { model.TrialParticipantHospitalId = participantRepos.GetParticipant(model.ParticipantId).HospitalId; }
            model.SeverityLevels = Severity.Levels.Select(l => new DetailSelectListItem()
            {
                Detail = l.Definition.Replace(";", Environment.NewLine),
                Text = l.Description,
                Value = l.SeverityId.ToString(),
                Selected = model.SeverityLevelId.HasValue ? model.SeverityLevelId.Value == l.SeverityId
                                                        : false
            });

            model.EventTypes = EventTypeService.GetAllAdverseEventTypes().Select(l => new SelectListItem()
            {
                Text = l.Description,
                Value = l.AdverseEventTypeId.ToString(),
                Selected = model.SeverityLevelId.HasValue ? model.AdverseEventTypeId.Value == l.AdverseEventTypeId
                                                        : false
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_advEventService != null) _advEventService.Dispose();
                if (_eventTypeService != null) _eventTypeService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

