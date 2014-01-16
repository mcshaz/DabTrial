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
        [AutoMapModel(typeof(IEnumerable<ScreenedPatient>),typeof(IEnumerable<ScreenedPatientListItem>))]
        public ViewResult Index()
        {
            return View(ScreenService.GetAllScreenedPatients(CurrentUserName));
        }

        //
        // GET: /ScreeningLog/Details/5
        [AutoMapModel(typeof(ScreenedPatient), typeof(ScreenedPatientDetails))]
        public ActionResult Details(int id)
        {
            var model = ScreenService.GetScreenedPatient(id,CurrentUserName);
            if (Request.IsAjaxRequest()) { return PartialView(model); }
            return View(model);
        }

        //
        // GET: /ScreeningLog/Create
        
        [HttpGet]
        [Ajax(false)]
        public ActionResult CreateEdit(int id=-1)
        {
            var model = Mapper.Map<CreateEditScreenedPatient>(ScreenService.GetScreenedPatient(id, CurrentUserName)) 
                ?? new CreateEditScreenedPatient 
                { 
                    ScreenedPatientId = -1
                    //ScreeningDate = CurrentUser.StudyCentre.LocalTime().Date
                };
            model.ScreeningList = Mapper.Map<IEnumerable<ScreenedPatientListItem>>(ScreenService.GetAllScreenedPatients(CurrentUserName));
            var selectedRow = model.ScreeningList.FirstOrDefault(s => s.ScreenedPatientId == id);
            if (selectedRow != null) { selectedRow.IsRowInEditor = true; }
            setLists(model);
            return View(model);
        }
        [HttpGet]
        [Ajax(true)]
        [ActionName("CreateEdit")]
        //[AutoMapModel(typeof(ScreenedPatient), typeof(CreateEditScreenedPatient))]
        public ActionResult CreateEditAjax(int id = -1)
        {
            var CreateEditPatient = Mapper.Map<CreateEditScreenedPatient>(ScreenService.GetScreenedPatient(id, CurrentUserName)) 
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
                if (isAjax && ModelState.IsValid)
                {
                    return PartialView("_IndexRow", Mapper.Map<ScreenedPatientListItem>(dbSp));
                }
                if (!isAjax && ModelState.IsValid) { return RedirectToAction("CreateEdit"); }
            }
            if (isAjax)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return ModelState.JsonValidation();
            }
            setLists(patient);
            patient.ScreeningList= Mapper.Map<IEnumerable<ScreenedPatientListItem>>(ScreenService.GetAllScreenedPatients(CurrentUserName));
            return View(patient);
        }
 
        //
        // GET: /ScreeningLog/Edit/5
 
        public ActionResult Edit(int id)
        {
            var model = Mapper.Map<CreateEditScreenedPatient>(ScreenService.GetScreenedPatient(id, CurrentUserName));
            setLists(model);
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
            setLists(model);
            return View(model);
        }

        //
        // GET: /ScreeningLog/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(ScreenService.GetScreenedPatient(id, CurrentUserName));
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
        private void setLists(CreateEditScreenedPatient model)
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