using System;
using System.Web.Mvc;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;

namespace DabTrial.Controllers
{
    [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
    public class AdverseEventTypeController : DataContextController, IDisposable
    {
        private AdverseEventTypeService _eventTypeService;
        private AdverseEventTypeService EventTypeService { get { return _eventTypeService ?? (_eventTypeService = new AdverseEventTypeService(ValidationDictionary, dbContext)); } }

        //
        // GET: /RespSupport/

        public ViewResult Index()
        {
            return View(EventTypeService.GetAllAdverseEventTypes());
        }

        //
        // GET: /RespSupport/Details/5

        public ViewResult Details(int id)
        {
            return View(EventTypeService.GetAdverseEventType(id));
        }

        //
        // GET: /RespSupport/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /RespSupport/Create

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Create(AdverseEventType adverseEventType)
        {
            if (ModelState.IsValid)
            {
                EventTypeService.Create(adverseEventType.Description, CurrentUserName);
            }

            return View(adverseEventType);
        }

        //
        // GET: /RespSupport/Edit/5

        public ActionResult Edit(int id)
        {
            return View(EventTypeService.GetAdverseEventType(id));
        }

        //
        // POST: /RespSupport/Edit/5

        [HttpPost][ValidateAntiForgeryToken]
        public ActionResult Edit(AdverseEventType adverseEventType)
        {
            if (ModelState.IsValid)
            {
                EventTypeService.Update(adverseEventType.AdverseEventTypeId, adverseEventType.Description, CurrentUserName);
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            return View(adverseEventType);
        }

        //
        // GET: /RespSupport/Delete/5

        public ActionResult Delete(int id)
        {
            return View(EventTypeService.GetAdverseEventType(id));
        }

        //
        // POST: /RespSupport/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EventTypeService.Delete(id, CurrentUserName);
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (_eventTypeService!=null) _eventTypeService.Dispose();
            base.Dispose(disposing);
        }
    }
}