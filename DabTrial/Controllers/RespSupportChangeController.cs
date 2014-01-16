using System;
using System.Net;
using System.Linq;
using System.Web.Mvc;
using DabTrial.Models;
using AutoMapper;
using DabTrial.Utilities;
using MvcHtmlHelpers;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize]
    public class RespSupportChangeController : DataContextController, IDisposable
    {
        private RespSupportTypesService _respiratorySupportTypesService;
        private RespSupportTypesService RespiratorySupportTypesService { get { return _respiratorySupportTypesService ?? (_respiratorySupportTypesService = new RespSupportTypesService(ValidationDictionary, dbContext)); } }
        private RespSupportChangeService _respiratorySupportChangeService;
        private RespSupportChangeService RespiratorySupportChangeService { get { return _respiratorySupportChangeService ?? (_respiratorySupportChangeService = new RespSupportChangeService(ValidationDictionary, dbContext)); } }

        //
        [HttpGet]
        public ActionResult CreateEdit(int id =-1, int participantId = -1)
        {
            if (id == -1 && participantId == -1) { throw new ArgumentNullException("Either id or respSupportChangeId must be specified"); }
            var model = (id == -1) ? new CreateEditRespSupportChange 
                                    { 
                                        TrialParticipant = Mapper.Map<ParticipantDetails>(ParticipantService.GetParticipant(participantId,CurrentUserName)), 
                                        RespSupportChangeId = -1 
                                    }
                                :Mapper.Map<CreateEditRespSupportChange>(RespiratorySupportChangeService.GetRespChange(id)) ;
            setLists(model);
            if (Request.IsAjaxRequest()) 
            {
                return PartialView(model); 
            }
            else 
            { 
                return View(model); 
            }
        }

        //
        // POST: /ScreeningLog/Create
        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult CreateEdit(CreateEditRespSupportChange respChange, int id = -1, int participantId = -1)
        {
            if (ModelState.IsValid)
            {
                RespiratorySupportChange dbRSC;
                if (respChange.RespSupportChangeId == -1)
                {
                    if (respChange.ParticipantId == -1) { throw new ArgumentException("respChange", "both ParticipantId and RespSupportId cannot be equal to -1"); }
                    dbRSC = RespiratorySupportChangeService.Create(respChange.ParticipantId, respChange.ChangeTime.Value, respChange.RespiratorySupportTypeId.Value, CurrentUserName);
                }
                else
                {
                    dbRSC = RespiratorySupportChangeService.Update(respChange.RespSupportChangeId ,respChange.ChangeTime.Value, respChange.RespiratorySupportTypeId.Value, CurrentUserName);
                }
                if (Request.IsAjaxRequest())
                {
                    if (ModelState.IsValid)
                    {
                        dbRSC.RespiratorySupportType = RespiratorySupportTypesService.GetRespSupportType(dbRSC.RespiratorySupportTypeId);
                        return PartialView("_IndexRow", Mapper.Map<RespSupportChangeItem>(dbRSC));
                    }
                    //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return ModelState.JsonValidation();
                }
                if (ModelState.IsValid) { return RedirectToAction("CreateEdit"); }
            }
            setLists(respChange);
            return View(respChange);
        }
        // GET: /RespSupportChange/Delete/5

        public ActionResult Delete(int id)
        {
            return View(RespiratorySupportChangeService.GetRespChange(id));//  .GetScreenedPatient(id));
        }

        //
        // POST: /RespSupportChange/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            int response = Response.StatusCode = (int)RespiratorySupportChangeService.Delete(id, CurrentUserName);
            if (Request.IsAjaxRequest())
            {
                if (response == (int)HttpStatusCode.OK) { return Json(id); }
                return new EmptyResult();
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        private void setLists(CreateEditRespSupportChange model)
        {
            model.RespSupportTypes = RespiratorySupportTypesService.GetAllRespSupportTypes().Select(rst => new DetailSelectListItem()
            {
                Value = rst.RespSupportTypeId.ToString(),
                Detail = rst.Explanation,
                Text = rst.Description,
                Selected = model.RespiratorySupportTypeId.HasValue ? model.RespiratorySupportTypeId.Value == rst.RespSupportTypeId
                                                           : false
            });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_respiratorySupportChangeService != null) _respiratorySupportChangeService.Dispose();
                if (_respiratorySupportTypesService != null) _respiratorySupportTypesService.Dispose();
            }
            base.Dispose(disposing);
        } 
    }
}
