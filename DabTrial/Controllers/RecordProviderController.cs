using System;
using System.Web.Mvc;
using DabTrial.Models;
using DabTrial.Utilities;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Services;

namespace DabTrial.Controllers
{
    [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
    public class RecordProviderController : DataContextController, IDisposable
    {
        private RecordProviderService _recordService;
        private RecordProviderService RecordService { get { return _recordService ?? (_recordService = new RecordProviderService(ValidationDictionary, dbContext)); } }
        //
        // GET: /HospitalMrn/Details/5
        [AutoMapModel(typeof(LocalRecordProvider), typeof(RecordProviderModel))]
        public ActionResult Details(int id)
        {
            return View(RecordService.GetRecordProvider(id));
        }

        //
        // GET: /HospitalMrn/Create

        public ActionResult Create()
        {
            var model = new RecordProviderModel();
            if (Request.IsAjaxRequest())
            {
                return PartialView(model);
            }
            return View(model);
        } 

        //
        // POST: /HospitalMrn/Create

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Create(RecordProviderModel model)
        {
            if (ModelState.IsValid)
            {
                LocalRecordProvider newRecordSystem = RecordService.CreateRecordProvider(model.Name, model.HospitalNoRegEx, model.NotationDescription, CurrentUserName);
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        return Json(new { selectItem = new { value = newRecordSystem.Id, text = newRecordSystem.Name }, target = "RecordSystemId" });
                    }
                    else
                    {
                        //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        return ModelState.JsonValidation();
                    }
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Create", "StudyCentre", new { recordProviderId = newRecordSystem.Id }); 
                }
            }
            return View(model);
        }
        
        //
        // GET: /HospitalMrn/Edit/5
        [AutoMapModel(typeof(LocalRecordProvider), typeof(RecordProviderModel))]
        public ActionResult Edit(int id)
        {
            return View(RecordService.GetRecordProvider(id));
        }

        //
        // POST: /HospitalMrn/Edit/5

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Edit(int id, RecordProviderModel model)
        {

                // TODO: Add update logic here
            if (ModelState.IsValid)
            {
                RecordService.UpdateRecordProvider(id, model.Name, model.HospitalNoRegEx, model.NotationDescription, WebSecurity.User.Identity.Name);
                if (ModelState.IsValid) { return RedirectToAction("Details","StudyCentre",new { id = id }); }
            }
            return View(model);
        }

        //
        // GET: /HospitalMrn/Delete/5
        [AutoMapModel(typeof(LocalRecordProvider), typeof(RecordProviderModel))]
        public ActionResult Delete(int id)
        {
            return View(RecordService.GetRecordProvider(id));
        }

        //
        // POST: /HospitalMrn/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, RecordProviderModel model)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_recordService != null) _recordService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
