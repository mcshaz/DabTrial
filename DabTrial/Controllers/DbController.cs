using DabTrial.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.FilesysServices;
using DabTrial.Infrastructure.Interfaces;

namespace DabTrial.Controllers
{
    [Authorize(Roles = RoleExtensions.DbAdministrator)]
    public class DbController : Controller
    {
        //
        // GET: /Db/
        private const string fileNotFoundName = "No Backups Found";
        private DbBackupService _dbService;
        private DbBackupService DbService
        {
            get
            {
                return _dbService ?? (_dbService = new DbBackupService(new ModelStateWrapper(ModelState)));
            }
        }
        /*If you set a path in web.config AppSettings AutomatedDbBakFolder
         * The application will assume your DB Server DOES NOT have write permissions to the directory
         * the application is running in, and that a scripting service is writing scheduled
         * backups to the path specified. The difference in the view relates to the number of 
         * .bak files displayed, whether date information is displayed, and the ability to
         * both create and delete the .bak files
        */
        public ActionResult Index()
        {
            var bakFolder = ConfigurationManager.AppSettings["AutomatedDbBakFolder"];
            if (string.IsNullOrEmpty(bakFolder)) {
                RedirectToActionPermanent("VersionBak");
            }
            DbService.DbBakDirectory = bakFolder;
            return View(DbService.GetAllBackupNames()
                .OrderByDescending(f => f.LastModified)
                .FirstOrDefault() ?? new AutomatedBakInfo { FileName = fileNotFoundName, LastModified = DateTime.MinValue, Size = 0, CanPostBack=false });
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(string fileName)
        {
            if (ModelState.IsValid & !string.IsNullOrWhiteSpace(fileName) & fileName != fileNotFoundName)
            {
                DbService.DbBakDirectory = ConfigurationManager.AppSettings["AutomatedDbBakFolder"];
                string fullPath = DbService.GetFullPath(fileName);
                if (ModelState.IsValid)
                {
                    string suggestedName = "Dabtrial Downloaded " + DbBackupService.Version() + ".zip";
                    if (fileName.EndsWith(".zip"))
                    {
                        return File(fullPath, "application/zip", suggestedName);
                    }
                    else
                    {
                        var stream = DbService.zipDbFiles(new string[] { fullPath });
                        stream.Position = 0;
                        return File(stream, "application/zip", suggestedName);
                    }
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult VersionBak()
        {
            var model = new DbVersionBakModel();
            setLists(model);
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VersionBak(DbVersionBakModel model)
        {
            if (ModelState.IsValid)
            {
                var stream = DbService.zipDbFiles(model.SelectedFileNames);
                if (ModelState.IsValid) { 
                    stream.Position = 0;
                    return File(stream, "application/zip", "Dabtrial Downloaded " + DbBackupService.Version() + ".zip");
                }
            }
            setLists(model);
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CreateBackup(DbVersionBakModel model)
        {
            if (ModelState.IsValid)
            {
                DbService.CreateDbBackup(model.DifferentialOnly);
                if (ModelState.IsValid) { return RedirectToAction("Index"); }
            }
            return View("/Db", model);
        }
        //no non ajax here - db admin should not require non js support!!!
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(string id)
        {
            int response = Response.StatusCode = (int)DbService.DeleteDbBackup(id);
            if (Request.IsAjaxRequest())
            {
                if (response == (int)HttpStatusCode.OK) { return Json(id); }
                return new EmptyResult();
            }
            return Json(id);
        }
        private void setLists(DbVersionBakModel model)
        {
            model.BackupFiles = DbService.GetAllBackupNames()
                .OrderByDescending(f => f.FileName);
            if (model.SelectedFileNames == null)
            {
                model.SelectedFileNames = new string[0];
            }
        }
    }
}