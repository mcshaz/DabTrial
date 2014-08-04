using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Infrastructure.Utilities;
using DabTrial.Models;
using Postal;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using DabTrial.Utilities;

namespace DabTrial.Domain.Services
{
    public static class CreateEmailService
    {
        public static void NotifyAdverseEvent(int adverseEventId)
        {
            using (var db = new DataContext())
            {
                var adEvt = GetEventLog(db.AdverseEvents, adverseEventId);
                adEvt.EventType = "Adverse event";
                adEvt.To = AllPIAndSIFromSiteEmails(adEvt.StudyCentreId, db);

                GetEmailService().Send(adEvt);
            }
        }
        public static void NotifyParticipantWithdrawn(int withdrawalId)
        {
            using (var db = new DataContext())
            {
                var withdraw = GetEventLog(db.ParticipantWithdrawals, withdrawalId);
                withdraw.EventType = "Participant withdrawal from trial";
                withdraw.To = AllPIAndSIFromSiteEmails(withdraw.StudyCentreId, db);

                GetEmailService().Send(withdraw);
            }
        }
        public static void NotifyParticipantDeath(int deathId)
        {
            using (var db = new DataContext())
            {
                var death = GetEventLog(db.ParticipantDeaths, deathId);
                
                death.EventType = "Participant death";
                death.To = AllPIAndSIFromSiteEmails(death.StudyCentreId, db);

                GetEmailService().Send(death);
            }
        }
        public static void NotifyProtocolViolation(int violationId)
        {
            using (var db = new DataContext())
            {
                var violation = GetEventLog(db.ProtocolViolations, violationId);

                violation.EventType = "Protocol violation";
                violation.To = AllPIAndSIFromSiteEmails(violation.StudyCentreId, db);

                GetEmailService().Send(violation);
            }
        }
        public static void WelcomeNewUser(int newUserId)
        {
            using (var db = new DataContext())
            {
                var user = (from u in db.Users
                            where u.UserId == newUserId
                            select new UserPasswordEmailModel 
                             {
                                 To = u.Email, 
                                 Password=u.Password, 
                                 UserName =u.UserName
                             }).First();
                user.ViewName = "NewUserConfirmation";
                GetEmailService().Send(user);
            }
        }
        public static void NotifyResetUserPassword(int userId, string userEmailMakingChanges)
        {
            UserPasswordEmailModel userModel;
            using (var db = new DataContext())
            {
                userModel = GetUserModel(userId, db);
            }
            userModel.From = userEmailMakingChanges;
            userModel.ViewName = "PasswordChanged";
            GetEmailService().Send(userModel);
        }
        public static void NotifyResetUserPassword(int userId, IDataContext db)
        {
            var userModel = GetUserModel(userId, db);
            userModel.ViewName = "PasswordChanged";
            GetEmailService().Send(userModel);
        }
        static UserPasswordEmailModel GetUserModel(int userId, IDataContext db)
        {
            var user = db.Users.Find(userId);
            return new UserPasswordEmailModel
            {
                To = user.Email,
                Password = user.Password,
                UserName = user.UserName,
                ViewName = "PasswordChanged"
            };
        }
        public static void NotifyNewParticipant(int participantId)
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
                participant.To = PIAndSIFromSiteEmails(participant.StudyCentreId, db);
                participant.ViewName = "NewParticipant";
                GetEmailService().Send(participant);
            }
        }
        public const int DaysBeforeNotifying = 7;
        public static void EmailInvestigatorsReMissingData()
        {
            var emailService = GetEmailService();
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
                                    }).GroupBy(p => p.StudyCentreId))
                {
                    dataUpdateEmailModels.Add(new DataUpdateEmailModel
                    {
                        DaysBeforeNotifying = DaysBeforeNotifying, 
                        To = PIAndSIFromSiteEmails(g.Key,db), 
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
                emailService.Send(d);
            }
            
        }
        static EventLoggedEmailModel GetEventLog(IQueryable<DiscrepancyReport> reports, int id)
        {
            return (from r in reports
                    let ru= r.ReportingUser 
                    where r.Id == id
                    select new EventLoggedEmailModel
                    {
                        StudyCentreId = r.TrialParticipant.StudyCentreId,
                        Details = r.Details,
                        DateTimeLogged = r.ReportingTimeLocal,
                        EventDateTime = r.EventTime,
                        ParcipantId = r.ParticipantId,
                        StudyCentreName = r.TrialParticipant.StudyCentre.Name,
                        UserName = ru.FirstName + " " + ru.LastName
                    }).First();
        }
        static EventLoggedEmailModel GetEventLog(IQueryable<OneTo1DiscrepancyReport> reports, int id)
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
                                UserName = ru.FirstName + " " + ru.LastName
                            }).First();
            returnVar.ParcipantId = id;
            returnVar.ViewName = "SignificantEvent";
            return returnVar;
        }
        static internal EmailService GetEmailService()
        {
            // Prepare Postal classes to work outside of ASP.NET request
            var viewsPath = Path.GetFullPath(HostingEnvironment.MapPath(@"~/Views/Emails"));
            var engines = new ViewEngineCollection();
            engines.Add(new FileSystemRazorViewEngine(viewsPath));

            return new EmailService(engines);
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