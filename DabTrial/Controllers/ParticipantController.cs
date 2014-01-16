using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using MvcHtmlHelpers;
using DabTrial.Utilities;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Services;
using DabTrial.Infrastructure.ModelBinders;

namespace DabTrial.Controllers
{
    [Authorize]
    public class ParticipantController : ParticipantRelatedController
    {
        private RespSupportTypesService _respSupportTypesService;
        private RespSupportTypesService RespiratorySupportTypesService { get { return _respSupportTypesService ?? (_respSupportTypesService = new RespSupportTypesService(ValidationDictionary, dbContext)); } }
        private StudyCentreService _centreService;
        private StudyCentreService CentreService { get { return _centreService ?? (_centreService = new StudyCentreService(ValidationDictionary, dbContext)); } }

        [AutoMapModel(typeof(IEnumerable<TrialParticipant>),typeof(IEnumerable<ParticipantListItem>))]
        public ActionResult Index()
        {
            var participants = ParticipantService.GetParticipantsByUserCentre(CurrentUserName);
            return View(participants);
        }
        //
        // GET: /Participant/
        public ActionResult Enrol()
        {
            var model = new ParticipantRegistration();
            SetPage(model);
            return View(model); 
        } 
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Enrol(ParticipantRegistration model)
        {
            if (ModelState.IsValid)
            {
                var newParticipant = ParticipantService.CreateNewParticipant(model.HospitalId,
                                                                            model.Dob.Value,
                                                                            Double.Parse(model.WeightStr),
                                                                            model.IcuAdmission.Value,
                                                                            model.RespSupportTypeId.Value,
                                                                            model.ChronicLungDisease.Value,
                                                                            model.CyanoticHeartDisease.Value,
                                                                            model.WeeksGestationAtBirth.Value,
                                                                            model.MaleGender.Value,
                                                                            CurrentUserName);
                if (newParticipant != null) { return RedirectToAction("Details", new { id = newParticipant.ParticipantId, newlyRandomised = true }); }
            }
            SetPage(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            var participant = ParticipantService.GetParticipant(id,CurrentUserName);
            ParticipantUpdate model;
            if (participant.IsInterventionArm)
            {
                model = (ParticipantUpdate)Mapper.Map<ParticipantInterventionUpdate>(participant);
                SetDropDown(model);
            }
            else
            {
                model = Mapper.Map<ParticipantUpdate>(participant);
            }
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
        public ActionResult Update(int id, [ModelBinder(typeof(ParticipantModelBinder))]ParticipantUpdate model)
        {
            if (ModelState.IsValid)
            {
                var participantIntervention = model as ParticipantInterventionUpdate;
                if (participantIntervention == null)
                {
                    ParticipantService.Update(id,
                        model.ReadyForIcuDischarge,
                        model.ActualIcuDischarge,
                        model.HospitalDischarge,
                        null,
                        null,
                        model.IsRsvPositive,
                        model.IsHmpvPositive,
                        null,
                        null,
                        null,
                        model.SteroidsForPostExtubationStridor,
                        model.AdrenalineForPostExtubationStridor,
                        model.DeathTime,
                        model.DeathDetails,
                        model.WithdrawalTime,
                        model.WithdrawalReason,
                        CurrentUserName);
                }
                else
                {
                    ParticipantService.Update(id,
                        model.ReadyForIcuDischarge,
                        model.ActualIcuDischarge,
                        model.HospitalDischarge,
                        participantIntervention.NumberOfSteroidDoses,
                        participantIntervention.NumberOfAdrenalineNebulisers,
                        model.IsRsvPositive,
                        model.IsHmpvPositive,
                        participantIntervention.InitialSteroidRouteId,
                        participantIntervention.FirstAdrenalineNebAt,
                        participantIntervention.FifthAdrenalineNebAt,
                        model.SteroidsForPostExtubationStridor,
                        model.AdrenalineForPostExtubationStridor,
                        model.DeathTime,
                        model.DeathDetails,
                        model.WithdrawalTime,
                        model.WithdrawalReason,
                        CurrentUserName);
                }
                if (IsAjax)
                {
                    if (ModelState.IsValid)
                    {
                        return new EmptyResult();
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                else if (ModelState.IsValid) { return RedirectToAction("Index", "Participant"); }
            }
            SetDropDown(model);
            return View(model);
        }
        [AutoMapModel(typeof(TrialParticipant), typeof(ParticipantDetails))]
        public ActionResult Details(int id, bool? newlyRandomised)
        {
            ViewBag.newlyRandomised = newlyRandomised ?? false;
            return View(ParticipantService.GetParticipant(id, CurrentUserName));
        }
        private void SetDropDown(ParticipantUpdate model)
        {
            var interventionModel = model as ParticipantInterventionUpdate;
            if (interventionModel == null) { return; }
            interventionModel.InitialSteroidRoutes = ParticipantService.GetAllDrugRoutes().Select(dr => new SelectListItem
            {
                Text = dr.Description,
                Value = dr.RouteId.ToString(),
                Selected = interventionModel.InitialSteroidRouteId == dr.RouteId
            });
        }
        private void SetPage(ParticipantRegistration model)
        {
            model.RespiratorySupportAtRandomisation = RespiratorySupportTypesService.GetAllRespSupportTypes().Select(rst => new DetailSelectListItem{
                 Value = rst.RespSupportTypeId.ToString(),
                 Detail = rst.Explanation,
                 Text = rst.Description,
                 IsDisabled = rst.RandomisationCategory == null,
                 Selected = model.RespSupportTypeId.HasValue?model.RespSupportTypeId.Value == rst.RespSupportTypeId
                                                            :false
            });
            model.ViewData = Mapper.Map<StudyCentre, CentreSpecificPatientValidationInfo>(CentreService.GetCentreByUser(CurrentUserName));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_respSupportTypesService != null) _respSupportTypesService.Dispose();
                if (_centreService != null) _centreService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
