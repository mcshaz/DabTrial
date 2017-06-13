using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using DabTrial.Utilities;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;
using Mvc.JQuery.DataTables;
using System.Data.Entity;
using DabTrial.Infrastructure.Crypto;
using DabTrial.Infrastructure.Helpers;

namespace DabTrial.Controllers
{
    [Authorize]
    public class ScreeningLogController : DataContextController, IDisposable
    {
        private ScreenedPatientService _screenService;
        private ScreenedPatientService ScreenService { get { return _screenService ?? (_screenService = new ScreenedPatientService(ValidationDictionary, dbContext)); } }
        private StudyCentreService _centreService;
        private StudyCentreService CentreService { get { return _centreService ?? (_centreService = new StudyCentreService(ValidationDictionary, dbContext)); } }

        //
        // GET: /ScreeningLog/
        //[AutoMapModel(typeof(IEnumerable<ScreenedPatient>),typeof(IEnumerable<ScreenedPatientListItem>))]
        public ViewResult Index()
        {
            return View(GetDatatableVm());
        }

        //
        // GET: /ScreeningLog/Details/5
        [AutoMapModel(typeof(ScreenedPatient), typeof(ScreenedPatientDetails))]
        public ActionResult Details(int id)
        {
            var model = ScreenService.GetScreenedPatient(id,CurrentUser);
            if (Request.IsAjaxRequest()) { return PartialView(model); }
            return View(model);
        }

        //
        // GET: /ScreeningLog/Create
        
        [HttpGet]
        [Ajax(false)]
        public ActionResult CreateEdit(int id=-1)
        {
            var model = Mapper.Map<CreateEditScreenedPatient>(ScreenService.GetScreenedPatient(id, CurrentUser)) 
                ?? new CreateEditScreenedPatient 
                { 
                    ScreenedPatientId = -1
                    //ScreeningDate = CurrentUser.StudyCentre.LocalTime().Date
                };
            SetLists(model);
            var dataTableVm = GetDatatableVm();
            ViewBag.DatatableVm = dataTableVm;
            return View(model);
        }
        [HttpGet]
        [Ajax(true)]
        [ActionName("CreateEdit")]
        //[AutoMapModel(typeof(ScreenedPatient), typeof(CreateEditScreenedPatient))]
        public ActionResult CreateEditAjax(int id = -1)
        {
            var CreateEditPatient = Mapper.Map<CreateEditScreenedPatient>(ScreenService.GetScreenedPatient(id, CurrentUser)) 
                ?? new CreateEditScreenedPatient 
                {
                    ScreenedPatientId = -1
                    //ScreeningDate = CurrentUser.StudyCentre.LocalTime().Date
                };
            return Json(CreateEditPatient, JsonRequestBehavior.AllowGet);
            //return PartialView("ScreenEdit", CreateEditPatient);
        }

        //
        // POST: /ScreeningLog/CreateEdit
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult CreateEdit(CreateEditScreenedPatient patient, int id=-1)
        {
            bool isAjax = Request.IsAjaxRequest();
            if (ModelState.IsValid)
            {
                ScreenedPatient dbSp;
                if (patient.ScreenedPatientId == -1)
                {
                    dbSp = ScreenService.Create(
                        patient.HospitalId,
                        patient.IcuAdmissionDate.Value, 
                        patient.Dob.Value,
                        patient.ScreeningDate.Value,
                        patient.AllInclusionCriteriaPresent,
                        patient.AllExclusionCriteriaAbsent,
                        patient.NoConsentAttemptId,
                        patient.ConsentRefused,
                        patient.NoConsentFreeText,
                        CurrentUserName);
                }
                else
                {
                    dbSp = ScreenService.Update(patient.ScreenedPatientId, 
                        (patient.HospitalId==ParticipantMapProfile.NullHospNo)?null:patient.HospitalId, 
                        patient.IcuAdmissionDate.Value,
                        patient.Dob.Value,
                        patient.ScreeningDate.Value,
                        patient.AllInclusionCriteriaPresent,
                        patient.AllExclusionCriteriaAbsent,
                        patient.NoConsentAttemptId,
                        patient.ConsentRefused,
                        patient.NoConsentFreeText, 
                        CurrentUserName);
                }
                if (isAjax && ModelState.IsValid)
                {
                    return new EmptyResult(); // new JsonResult { Data = Mapper.Map<ScreenedPatientListItem>(dbSp) };
                }
                if (!isAjax && ModelState.IsValid) { return RedirectToAction("CreateEdit"); }
            }
            if (isAjax)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return ModelState.JsonValidation();
            }
            SetLists(patient);
            return View(patient);
        }
 
        //
        // GET: /ScreeningLog/Edit/5
 
        public ActionResult Edit(int id)
        {
            var model = Mapper.Map<CreateEditScreenedPatient>(ScreenService.GetScreenedPatient(id, CurrentUser));
            SetLists(model);
            return View(model);
        }

        //
        // POST: /ScreeningLog/Edit/5

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Edit(CreateEditScreenedPatient model)
        {
            if (ModelState.IsValid)
            {
                ScreenService.Update(model.ScreenedPatientId, 
                    model.HospitalId, 
                    model.IcuAdmissionDate.Value,
                    model.Dob.Value,
                    model.ScreeningDate.Value,
                    model.AllInclusionCriteriaPresent,
                    model.AllExclusionCriteriaAbsent,
                    model.NoConsentAttemptId,
                    model.ConsentRefused,
                    model.NoConsentFreeText,
                    CurrentUserName);
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            SetLists(model);
            return View(model);
        }

        //
        // GET: /ScreeningLog/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(ScreenService.GetScreenedPatient(id, CurrentUser));
        }

        //
        // POST: /ScreeningLog/Delete/5
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int response = Response.StatusCode = (int)ScreenService.Delete(id, CurrentUserName);
            if (Request.IsAjaxRequest())
            {
                if (response == (int)HttpStatusCode.OK) { return Json(id); }
                return new EmptyResult();
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public DataTablesResult<ScreenedPatientListItem> GetScreenedPatients(DataTablesParam dataTableParam)
        {
            var studyCentreId = CurrentUser.RestrictedToCentre();
            DateTime today = DateTime.Today;

            IQueryable<ScreenedPatient> queryable = null;
            if (dataTableParam.sSearchValues[ScreenedPatientListItem.HosiptalIdOrder] != String.Empty)
            {
                var q = ScreenService.GetScreenedPatientByHospitalId(dataTableParam.sSearchValues[ScreenedPatientListItem.HosiptalIdOrder], CurrentUser);
                dataTableParam.sSearchValues[ScreenedPatientListItem.HosiptalIdOrder] = String.Empty;
                if (q != null)
                {
                    queryable = (new[] { q }).AsQueryable();
                }
            }
            if (queryable == null)
            {
                queryable = dbContext.ScreenedPatients;
            }
            return DataTablesResult.Create(
                queryable.Where(src => !studyCentreId.HasValue || src.StudyCentreId == studyCentreId.Value).Select(src => new ScreenedPatientListItem
                {
                    HospitalId = src.HospitalId,
                    ExclusionReasonAbbreviation = !src.AllInclusionCriteriaPresent ? "Inclusions"
                        : !src.AllExclusionCriteriaAbsent ? "Exclusions"
                            : src.NoConsentAttemptId.HasValue ? src.NoConsentReason.Abbreviation
                                : src.ConsentRefused ? "Refused" 
                                    : "?",
                    IcuAdmissionDate = src.IcuAdmissionDate,
                    ScreenedPatientId = src.ScreenedPatientId, 
                    ScreeningDate = src.ScreeningDate,
                    NoConsentFreeText = (src.NoConsentFreeText == null || src.NoConsentFreeText ==string.Empty || src.NoConsentFreeText.Length <= 15)? src.NoConsentFreeText
                        : DbFunctions.Left(src.NoConsentFreeText, 12) + "...",
                    StudyCentreAbbreviation = src.StudyCentre.Abbreviation,
                    StudyCentreId = src.StudyCentreId
                }),
                dataTableParam,
                src=> new 
                    {
                        HospitalId = (src.StudyCentreId == CurrentUser.StudyCentreId)
                            ? ScreenService.DecryptHospitalId(src.HospitalId)
                            : ParticipantMapProfile.NullHospNo,
                        ScreeningDate = src.ScreeningDate.ToShortDateString(),
                        IcuAdmissionDate = src.IcuAdmissionDate.ToShortDateString() + " (" + DateIntervalHelper.GetIntervalPrior(src.IcuAdmissionDate, today) + " ago)"
                    });
        }

        private void SetLists(CreateEditScreenedPatient model)
        {
            model.CentreData = Mapper.Map<CentreSpecificPatientValidationInfo>(CentreService.GetCentreByUser(CurrentUserName));

            var allReasons = ScreenService.GetNoConsentReasons();
            model.NoConsentAttemptReasons = allReasons.ToLookup(r=>r.IsFullyAware, r => new SelectListItem
            {
                Text = r.Description + (r.RequiresDetail ? " (Include Details)" : ""),
                Selected = model.NoConsentAttemptId == r.Id,
                Value = r.Id.ToString()
            });

            model.NoConsentAttemptRequiresDetail = allReasons.Where(r => r.RequiresDetail).Select(r => r.Id);

            model.StudyCentreAbbreviations = CentreService.GetCentreAbbreviations();
        }

        private DataTableConfigVm GetDatatableVm()
        {
            var getDataUrl = Url.Action(nameof(ScreeningLogController.GetScreenedPatients));
            var datatableVm = DataTablesHelper.DataTableVm<ScreenedPatientListItem>("ScreeningLog", getDataUrl);
            datatableVm.UseColumnFilterPlugin = true;
            datatableVm.FilterOn(nameof(ScreenedPatientListItem.StudyCentreAbbreviation)).Select(CentreService.GetCentreAbbreviations());
            return datatableVm;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_screenService != null) _screenService.Dispose();
                if (_centreService != null) _centreService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}