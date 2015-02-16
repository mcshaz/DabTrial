using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Utilities;
using DabTrial.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using DabTrial.Utilities;
using System.Reflection;
using System.Net.Mime;
using System.Text;
using HtmlAgilityPack;
using System.Net.Mail;
using Hangfire;

namespace DabTrial.Domain.Services
{
    public class CreateEmailService
    {
        #region Fields
        IEmailService _emailService;
        string _viewsPath;
        #endregion
        #region Constructors
        public CreateEmailService()
        {

        }
        public CreateEmailService(IEmailService emailService)
        {
            _emailService = emailService;
        }
        #endregion
        #region Properties
        public string ViewsPath
        {
            get
            {
                return _viewsPath ?? (_viewsPath = HostingEnvironment.MapPath("~/Views/Emails"));
            }
            set
            {
                _viewsPath = value;
            }
        }
        IEmailService MailService
        {
            get
            {
                if (_emailService==null)
                {
                    // Prepare Postal classes to work outside of ASP.NET request
                    //return HostingEnvironment.MapPath(@"~/Views/Emails");
                    //AppDomain.CurrentDomain.BaseDirectory
                    //Assembly.GetAssembly(typeof(CreateEmailService)).Location
                    //Path.GetPath

                    var engines = new ViewEngineCollection();
                    engines.Add(new FileSystemRazorViewEngine(ViewsPath));
                    _emailService = new EmailService(engines);
                }
                return _emailService;
            }
        }
        #endregion
        public void NotifyAdverseEvent(int adverseEventId)
        {
            using (var db = new DataContext())
            {
                var adEvt = GetEventLog(db.AdverseEvents, adverseEventId);
                adEvt.EventType = "Adverse event";
                adEvt.To = AllPIAndSIFromSiteEmails(adEvt.StudyCentreId, db);

                Send(adEvt);
            }
        }
        public void NotifyParticipantWithdrawn(int withdrawalId)
        {
            using (var db = new DataContext())
            {
                var withdraw = GetEventLog(db.ParticipantWithdrawals, withdrawalId);
                withdraw.EventType = "Participant withdrawal from trial";
                withdraw.To = AllPIAndSIFromSiteEmails(withdraw.StudyCentreId, db);

                Send(withdraw);
            }
        }
        public void NotifyParticipantDeath(int deathId)
        {
            using (var db = new DataContext())
            {
                var death = GetEventLog(db.ParticipantDeaths, deathId);
                
                death.EventType = "Participant death";
                death.To = AllPIAndSIFromSiteEmails(death.StudyCentreId, db);

                Send(death);
            }
        }
        public void NotifyProtocolViolation(int violationId)
        {
            using (var db = new DataContext())
            {
                var violation = GetEventLog(db.ProtocolViolations, violationId);

                violation.EventType = "Protocol violation";
                violation.To = AllPIAndSIFromSiteEmails(violation.StudyCentreId, db);

                Send(violation);
            }
        }
        public void ForwardWebMessage(string enquirerEmail, string recipientEmail,string subject, string message)
        {
            
            var model = new ForwardMailInvestigator
            {
                To = recipientEmail,
                From = enquirerEmail,
                Subject = subject,
                Message = message,
                ViewName = "ForwardMail"
            };
            Send(model);
        }
        public void WelcomeNewUser(string userName, string userEmail,string plainTextPassword, PasswordPresentations passwordDisplay, string from = null)
        {
            var user = new UserPasswordEmailModel
            {
                ViewName = "NewUserConfirmation",
                UserName = userName,
                PlainTextPassword = plainTextPassword,
                PasswordDisplay = passwordDisplay,
                From = from,
                To = userEmail
            };
            Send(user);
        }
        public void NotifyResetUserPassword(string userName, string userEmail,string plainTextPassword, PasswordPresentations passwordDisplay)
        {
            var user = new UserPasswordEmailModel
            {
                ViewName = "PasswordChanged",
                UserName = userName,
                PlainTextPassword = plainTextPassword,
                PasswordDisplay = passwordDisplay,
                To = userEmail
            };
            Send(user);
        }
        public void NotifyNewParticipant(int participantId)
        {
            using (var db = new DataContext())
            {
                var participant = (from u in db.TrialParticipants
                                    let ec = u.EnrollingClinician
                                    where u.ParticipantId == participantId
                                    select new NewParticipantEmailModel
                                    {
                                        StudyCentreId = u.StudyCentreId, 
                                        DateTimeRandomised = u.LocalTimeRandomised, 
                                        UserName=ec.FirstName + " " + ec.LastName
                                    }).First();
                participant.ParticipantId = participantId;
                participant.To = PIAndSIFromSiteEmails(participant.StudyCentreId, db);
                participant.ViewName = "NewParticipant";
                Send(participant);
            }
        }
        public const int DaysBeforeNotifying = 7;
        public void EmailInvestigatorsReMissingData()
        {
            List<DataUpdateEmailModel> dataUpdateEmailModels = new List<DataUpdateEmailModel>();
            using (var db = new DataContext())
            {
                foreach (var g in (from p in DataSummaryService.GetMissingParticipantData(db)
                                    where p.Stage != DataStageUtilities.TrialStage.Complete && p.DaysSinceEnrolled > DaysBeforeNotifying
                                    select new
                                    { 
                                        p.StudyCentreId, 
                                        p.ParticipantId,
                                        p.DaysSinceEnrolled,
                                        p.Stage
                                    }).ToLookup(p => p.StudyCentreId))
                {
                    dataUpdateEmailModels.Add(new DataUpdateEmailModel
                    {
                        DaysBeforeNotifying = DaysBeforeNotifying, 
                        To = PIAndSIFromSiteEmails(g.Key,db), 
                        ViewName="EmailDataUpdate",
                        Participants4Update=g.Select(p=>new Participant4Update
                        { 
                            ParticipantId = p.ParticipantId, 
                            DaysSinceEnrolled = p.DaysSinceEnrolled, 
                            DataStage = p.Stage.ToString().ToSeparateWords()
                        })
                    });
                }
            }
            //getting out of loop and then reiterating to minimise chance db gets clobbered by app pool recycle
            foreach (var d in dataUpdateEmailModels)
            {
                Send(d);
            }
            
        }
        static EventLoggedEmailModel GetEventLog(IQueryable<DiscrepancyReportBase> reports, int id)
        {
            var returnVar = (from r in reports
                            let p = r.TrialParticipant
                            let ru = r.ReportingUser
                            where r.Id == id
                            select new EventLoggedEmailModel
                            {
                                StudyCentreId = p.StudyCentreId,
                                Details = r.Details,
                                DateTimeLogged = r.ReportingTimeLocal,
                                EventDateTime = r.EventTime,
                                StudyCentreName = p.StudyCentre.Name,
                                UserName = ru.FirstName + " " + ru.LastName,
                                To = ru.Email,
                                ParcipantId = p.ParticipantId
                            }).First();
            returnVar.ViewName = "SignificantEvent";
            return returnVar;
        }
        void Send(Email email)
        {
            using (MailMessage mail = MailService.CreateMailMessage(email))
            {
                const int junkBytes = 3;
                mail.IsBodyHtml = false;
                FileInfo sig = new FileInfo(Path.Combine(ViewsPath, "_Signature.html"));
                byte[] html = Encoding.UTF8.GetBytes(mail.Body);
                using (MemoryStream stream = new MemoryStream(html.Length + (int)sig.Length - junkBytes))
                {
                    stream.Write(html, 0, html.Length);
                    using (FileStream fileStream = sig.OpenRead()) 
                    {
                        fileStream.Position = junkBytes;
                        fileStream.CopyTo(stream);
                    }
                    stream.Position = 0;
                    HtmlDocument doc = new HtmlDocument();
                    doc.Load(stream);
                    mail.Body = HtmlToText.ConvertDoc(doc);
                    stream.Position = 0;
                    mail.AlternateViews.Add(new System.Net.Mail.AlternateView(stream, "text/html; charset=utf-8"));
                    using (var smtp = new SmtpClient())
                    {
                        smtp.Send(mail);
                    }
                }
            }
        }

        static string AllPIAndSIFromSiteEmails(int centreId, DabTrial.Domain.Providers.IDataContext context)
        {
            return String.Join(",", (from u in context.Users
                                     where !u.IsDeactivated &&
                                         (u.StudyCentreId == centreId && u.Roles.Any(r => r.RoleName == RoleExtensions.SiteInvestigator))
                                         || u.Roles.Any(r => r.RoleName == RoleExtensions.PrincipleInvestigator)
                                     select u.Email));
        }

        static string PIAndSIFromSiteEmails(int centreId, DabTrial.Domain.Providers.IDataContext context)
        {
            return string.Join(",", (from u in context.Users
                                     where !u.IsDeactivated &&
                                      u.StudyCentreId == centreId &&
                                      u.Roles.Any(r => r.RoleName == RoleExtensions.PrincipleInvestigator || r.RoleName == RoleExtensions.SiteInvestigator)
                                     select u.Email));
        }

    }
}