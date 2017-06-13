using DabTrial.Infrastructure.FilesysServices;
using DabTrial.Models;
using System;
using System.Net.Mail;
using System.Web.Mvc;

namespace DabTrial.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View(FileManagementService.GetAllLinks(WebSecurity.User.Identity.IsAuthenticated));
        }

        public ActionResult Contact()
        {
            return View();
        }
        public ActionResult Download(DirectoryType id, string fileName)
        {
            if (id == DirectoryType.ForInvestigators && !WebSecurity.User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("LogOn", "Account", new { returnUrl = Url.Action("About", "Home") });
                //could get the file itself with returnUrl = ViewContext.HttpContext.Request.Url.PathAndQuery but this will leave the patient on the login page
            }
            var documentName = FileManagementService.Combine(id, fileName);
            string mime = FileManagementService.MimeType(fileName);
            if (!System.IO.File.Exists(documentName) || mime == null)
            {
                return RedirectToAction("NotFound","Error");
            }
            //http://www.softwire.com/blog/2011/08/23/content-disposition-headers-in-net/
            return File(documentName, mime, fileName);
        }
        public ActionResult TestEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult TestEmailResult()
        {
            using (var m = new MailMessage())
            {
                m.To.Add("brent@focused-light.net");
                m.Subject = "Test";
                m.Body = "Test message from dabtrial sent " + DateTime.Now;
                try
                {
                    using (var c = new SmtpClient())
                    {
                        c.Send(m);
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Message = ex.ToString();
                }
            }
            
            ViewBag.Message = ViewBag.Message ?? "Success";
            return View();
        }
    }
}
