using DabTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.ComponentModel;
using DabTrial.Utilities;
using System.Net;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.FilesysServices;

namespace DabTrial.Controllers
{
    [Authorize(Roles = (RoleExtensions.PrincipleInvestigator + "," + RoleExtensions.SiteInvestigator))]
    public class ManageFilesController : Controller
    {
        //
        // GET: /ManageFiles/
        public ActionResult Index()
        {
            var model = FileManagementService.GetAllFiles();
            setLists(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult Upload()
        {
            var directoryList = GetDirectoryList(true);
            var model = new UploadFileModel[4];
            for (int i=0;i<model.Length;i++)
            {
                model[i] = new UploadFileModel { DirectoryList = directoryList };
            }
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Upload(IList<UploadFileModel> model)
        {
            if (ModelState.IsValid && model != null)
            {
                foreach (var item in model)
                {
                    if (item.File != null) { FileManagementService.AddFile(item.File, item.SaveDirectory.Value, item.ServerFileName); }
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            setLists(model, true);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(IList<ManageFileDetailsModel> model)
        {
            if (ModelState.IsValid)
            {
                for (int i = 0; i < model.Count; i++ )
                {
                    var dest = model[i];
                    var src = JsonConvert.DeserializeObject<ManageFileModel>(dest.Src);
                    if (dest.FileName != src.FileName || dest.SaveDirectory != src.SaveDirectory)
                    {
                        if (FileManagementService.ModifyFile(src, dest)!=HttpStatusCode.OK)
                        {
                            ModelState.AddModelError(string.Format("[{0}].FileName", i), "Unable to update file");
                        }
                    }
                    
                }
                if (ModelState.IsValid)
                {
                    return RedirectToAction("Index");
                }
            }
            setLists(model);
            return View(model);
        }
        [HttpGet]
        public ActionResult Delete(string id, DirectoryType directory)
        {
            var model = new ManageFileModel{
                FileName = id
            };
            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken, ActionName("Delete")]
        public ActionResult DeleteConfirmed(string fileName, DirectoryType directory)
        {
            int response = Response.StatusCode =(int)FileManagementService.DeleteFile(directory, fileName);
            if (Request.IsAjaxRequest()) 
            {
                if (response == (int)HttpStatusCode.OK) { return Json(fileName); }
                return new EmptyResult();
            }
            return RedirectToAction("Index");
        }
        private void setLists(IEnumerable<IFileModel> model, bool includeNull = false)
        {
            var directories = GetDirectoryList(includeNull);
            foreach (var item in model)
            {
                var itemDir = item.SaveDirectory.ToString();
                item.DirectoryList = directories.Select(d => new SelectListItem
                {
                    Value = d.Value,
                    Text = d.Text,
                    Selected = d.Value == itemDir
                });
            }
        }
        private IEnumerable<SelectListItem> GetDirectoryList(bool includeNull = false)
        {
            Type directoryType = typeof(DirectoryType);
            TypeConverter converter = TypeDescriptor.GetConverter(directoryType);
            var directories = (DirectoryType[])Enum.GetValues(directoryType);
            var returnVar = new List<SelectListItem>(directories.Length+1);
            if (includeNull) {returnVar.Add(new SelectListItem{ Text="", Value=""});}
            foreach (var d in directories)
            {
                returnVar.Add(new SelectListItem
                    {
                        Text = converter.ConvertToString(d).ToSeparateWords(),
                        Value = d.ToString()
                    });
            }
            return returnVar;
        }
    }
}
