using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Services;
using MvcHtmlHelpers;


namespace DabTrial.Controllers
{
    [Authorize(Roles = RoleExtensions.PrincipleInvestigator + "," + RoleExtensions.SiteInvestigator)]
    public class StudyCentreController : DataContextController, IDisposable
    {
        private StudyCentreService _centreService;
        private StudyCentreService CentreService { get { return _centreService ?? (_centreService = new StudyCentreService(ValidationDictionary, dbContext)); } }
        private RecordProviderService _recordService;
        private RecordProviderService RecordService { get { return _recordService ?? (_recordService = new RecordProviderService(ValidationDictionary, dbContext)); } }
        private RespSupportTypesService _respSupportService;
        private RespSupportTypesService RespSupportService { get { return _respSupportService ?? (_respSupportService = new RespSupportTypesService(ValidationDictionary, dbContext)); } }
        //
        // GET: /StudyCentre/
        [AutoMapModel(typeof(IEnumerable<StudyCentre>),typeof(IEnumerable<StudyCentreListItem>))]
        public ActionResult Index()
        {
            int? restrict = UserService.GetUser(CurrentUserName).RestrictedToCentre();
            if (restrict.HasValue)
            {
                return RedirectToAction("Details", new { id = restrict.Value });
            }
            return View(CentreService.GetCentres());
        }

        //
        // GET: /StudyCentre/Details/5
        public ViewResult Details(int id)
        {
            int? restrict = UserService.GetUser(CurrentUserName).RestrictedToCentre();
            if (restrict.HasValue) { id = restrict.Value; }
            var studyCentre = Mapper.Map<StudyCentreDetails>(CentreService.GetCentre(id));
            return View(studyCentre);
        }

        //
        // GET: /StudyCentre/Create
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        public ActionResult Create(int? recordProviderId)
        {
            var model = new StudyCentreCreate
            {
                RecordSystemId = recordProviderId,
                CommencedEnrollingOn = DateTime.Now
            };
            SetLists(model);
            return View(model);
        } 

        //
        // POST: /StudyCentre/Create
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Create(StudyCentreCreate model)
        {
            if (ModelState.IsValid)
            {
                CentreService.CreateCentre(model.Name,
                    model.Abbreviation,
                    model.SiteRegistrationPwd,
                    model.RecordSystemId.Value,
                    model.TimeZoneId,
                    model.ValidDomainList.Split(','),
                    model.PublicPhoneNumber.Formatted,
                    model.IsUsing1pcAdrenaline,
                    model.CommencedEnrollingOn,
                    model.MaxWardSupportId,
                    CurrentUserName);
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            SetLists(model);
            return View(model);
        }
        
        //
        // GET: /StudyCentre/Edit/5
 
        public ActionResult Edit(int id)
        {
            int? restrict = UserService.GetUser(CurrentUserName).RestrictedToCentre(); 
            if (restrict.HasValue) { id = restrict.Value; }
            StudyCentreEdit studyCentre = Mapper.Map<StudyCentreEdit>(CentreService.GetCentre(id));
            SetLists(studyCentre);
            return View(studyCentre);
        }

        //
        // POST: /StudyCentre/Edit/5

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Edit(StudyCentreEdit model)
        {
            int? restrict = UserService.GetUser(CurrentUserName).RestrictedToCentre(); 
            if (restrict.HasValue) { model.StudyCentreId = restrict.Value; }
            if (ModelState.IsValid)
            {
                CentreService.UpdateCentre(model.StudyCentreId,
                    model.Name,
                    model.Abbreviation,
                    model.SiteRegistrationPwd,
                    model.TimeZoneId,
                    model.ValidDomainList.Split(','),
                    model.PublicPhoneNumber.Formatted,
                    model.IsUsing1pcAdrenaline,
                    model.CommencedEnrollingOn,
                    model.MaxWardSupportId,
                    CurrentUserName);
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            SetLists(model);
            return View(model);
        }

        //
        // GET: /StudyCentre/Delete/5
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        public ActionResult Delete(int id)
        {
            StudyCentre studycentre = CentreService.GetCentre(id);
            return View(studycentre);
        }

        //
        // POST: /StudyCentre/Delete/5
        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CentreService.DeleteCentre(id);
            return RedirectToAction("Index");
        }

        private void SetLists(StudyCentreEdit model)
        {
            model.TimeZones = new SelectList(TimeZoneInfo.GetSystemTimeZones(), "Id", "DisplayName", model.TimeZoneId);
            model.RespSupports = GetRespSupports(model.MaxWardSupportId);
        }
        private void SetLists(StudyCentreCreate model)
        {
            model.TimeZones = new SelectList(TimeZoneInfo.GetSystemTimeZones(), "Id", "DisplayName", "AUS Eastern Standard Time");
            model.RecordSystems = RecordService.GetAllRecordProviders().Select(p => new SelectListItem()
            {
                Value = p.Id.ToString(),
                Text = p.Name,
                Selected = model.RecordSystemId == p.Id
            });
            model.RespSupports = GetRespSupports();
        }
        private IEnumerable<SelectListItem> GetRespSupports(int? selected = null)
        {
            return RespSupportService.GetAllRespSupportTypes().Select(rst => new DetailSelectListItem()
            {
                Value = rst.RespSupportTypeId.ToString(),
                Detail = rst.Explanation,
                Text = rst.Description,
                Selected = selected.HasValue ? selected.Value == rst.RespSupportTypeId
                                                           : false
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_centreService != null) _centreService.Dispose();
                if (_recordService != null) _recordService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}