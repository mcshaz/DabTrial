using System;
using System.Net;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using DabTrial.Utilities;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize]
    public class AdverseEventDrugController : DataContextController, IDisposable
    {
        private AdverseEventService _advEventService;
        private AdverseEventService AdvEventService { get { return _advEventService ?? (_advEventService = new AdverseEventService(ValidationDictionary, dbContext)); } }
        private AdverseEventDrugService _advEventDrugService;
        private AdverseEventDrugService AdvEventDrugService { get { return _advEventDrugService ?? (_advEventDrugService = new AdverseEventDrugService(ValidationDictionary, dbContext)); } }


        [HttpGet]
        public ActionResult CreateEdit(int id = -1, int adverseEventId = -1)
        {
            if (id == -1 && adverseEventId == -1) { throw new ArgumentException("either a id or adverseEventId must be specified"); }
            var model = (id == -1) ? new DrugCreateModify()
                                    {
                                        AdverseEvent = AdvEventService.GetAdverseEvent(adverseEventId)
                                    }
                                   : Mapper.Map<DrugCreateModify>(AdvEventDrugService.GetDrug(id));
            if (Request.IsAjaxRequest()) 
            {
                return PartialView(model); 
            }
            else 
            { 
                return View(model); 
            }
            /*
            //if user centre not equal to participant centre, redirect to uneditable form
            if (userService.GetUser(currentUserName).StudyCentreId != model.EventDetails.TrialParticipantStudyCentreId)
            {
                return RedirectToActionPermanent("EventDetails", new { id = model.AdverseEventId });
            }
            return View(model);
             * */
        }
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult CreateEdit(DrugCreateModify inputModel, int id = -1, int adverseEventId = -1)
        {
            if (ModelState.IsValid)
            {
                Drug dbDrug;
                if (inputModel.DrugId == -1)
                {
                    if (inputModel.AdverseEventId == -1) { throw new ArgumentException("inputModel", "both drugId and adverseEventId cannot be equal to -1"); }
                    dbDrug = AdvEventDrugService.CreateDrug(inputModel.AdverseEventId,
                                inputModel.DrugName,
                                inputModel.Dosage,
                                inputModel.ReasonsForUse,
                                inputModel.StartDate.Value,
                                inputModel.StopDate,
                                CurrentUserName);
                }
                else
                {
                    dbDrug = AdvEventDrugService.UpdateDrug(inputModel.DrugId,
                                inputModel.DrugName,
                                inputModel.Dosage,
                                inputModel.ReasonsForUse,
                                inputModel.StartDate.Value,
                                inputModel.StopDate,
                                CurrentUserName);
                }
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        return PartialView("_IndexRow", Mapper.Map<DrugListItem>(dbDrug));
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                if (ModelState.IsValid) { return RedirectToAction("Details", "AdverseEvent", new { id = inputModel.AdverseEventId }); }
            }
            return View(inputModel);
        }
        [AutoMapModel(typeof(Drug), typeof(ConfirmDeleteDrugModel))]
        public ActionResult Delete(int id)
        {
            return View(AdvEventDrugService.GetDrug(id));//  .GetScreenedPatient(id));
        }

        //
        // POST: /RespSupportChange/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, ConfirmDeleteDrugModel model)
        {
            int response = Response.StatusCode = (int)AdvEventDrugService.Delete(id, CurrentUserName);
            if (Request.IsAjaxRequest())
            {
                if (response == (int)HttpStatusCode.OK) { return Json(id); }
                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("Details", "AdverseEvent", new { id = model.AdverseEventId });
            }
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_advEventService != null) _advEventService.Dispose();
                if (_advEventDrugService != null) _advEventDrugService.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
