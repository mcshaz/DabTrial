using DabTrial.Domain.Tables;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DabTrial.Controllers
{
    [Authorize(Roles = "PrincipleInvestigator")]
    public class NoConsentAttemptController : DataContextController
    {
        //
        // GET: /NoConsentAttempt/

        public ViewResult Index()
        {
            return View(dbContext.NoConsentAttempts.ToList());
        }

        //
        // GET: /NoConsentAttempt/Details/5

        public ViewResult Details(int id)
        {
            return View(dbContext.NoConsentAttempts.Find(id));
        }

        //
        // GET: /NoConsentAttempt/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /NoConsentAttempt/Create

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(NoConsentAttempt model)
        {
            if (ModelState.IsValid)
            {
                dbContext.NoConsentAttempts.Add(model);
                dbContext.SaveChanges(CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //
        // GET: /NoConsentAttempt/Edit/5

        public ActionResult Edit(int id)
        {
            return View(dbContext.NoConsentAttempts.Find(id));
        }

        //
        // POST: /NoConsentAttempt/Edit/5

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int id, NoConsentAttempt model)
        {
            if (ModelState.IsValid)
            {
                //warning quick and nasty hack!
                ((DbContext)dbContext).Entry(model).State = EntityState.Modified;
                dbContext.SaveChanges(CurrentUserName);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //
        // GET: /NoConsentAttempt/Delete/5

        public ActionResult Delete(int id)
        {
            return View(dbContext.NoConsentAttempts.Find(id));
        }

        //
        // POST: /NoConsentAttempt/Delete/5

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var toDelete = new NoConsentAttempt { Id = id };
            dbContext.NoConsentAttempts.Remove(toDelete);
            dbContext.SaveChanges(CurrentUserName);
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            
            base.Dispose(disposing);
        }
    }
}
