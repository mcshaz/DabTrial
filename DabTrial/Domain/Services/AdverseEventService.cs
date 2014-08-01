using System;
using System.Collections.Generic;
using DabTrial.Utilities;
using System.Net;
using System.Linq;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Providers;
using DabTrial.Infrastructure.Interfaces;
namespace DabTrial.Domain.Services
{
    public class AdverseEventService : PatientServiceLayer
    {
        public AdverseEventService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public IEnumerable<AdverseEvent> GetAdverseEventsByCentre(int studyCentreId)
        {
            return (from a in _db.AdverseEvents.Include("TrialParticipant")
                    where a.TrialParticipant.StudyCentreId == studyCentreId
                    select a).ToList();
        }
        public IEnumerable<AdverseEvent> GetAdverseEventsByCentre(int studyCentreId, string userName)
        {
            var returnVar = (from a in _db.AdverseEvents.Include("TrialParticipant")
                            where a.TrialParticipant.StudyCentreId == studyCentreId
                            select a).ToList();
            DecryptHospitalId(returnVar.Select(ae => ae.TrialParticipant));
            return returnVar;
        }
        public IEnumerable<AdverseEvent> GetAdverseEventsByParticipant(int participantId)
        {
            return _db.AdverseEvents.Include("Drugs").Where(a => a.ParticipantId == participantId).OrderBy(e=>e.EventTime).ToList();
        }
        public IEnumerable<AdverseEvent> GetAllAdverseEvents()
        {
            return _db.AdverseEvents.Include("TrialParticipant").ToList();
        }
        public IEnumerable<AdverseEvent> GetAllAdverseEvents(string userName)
        {
            var returnVar = _db.AdverseEvents.Include("TrialParticipant").ToList();
            DecryptHospitalId(returnVar.Select(ae => ae.TrialParticipant));
            return returnVar;
        }
        public Drug GetDrug(int drugId)
        {
            return (from d in _db.AdverseEventDrugs.Include("AdverseEvent").Include("AdverseEvent.TrialParticipant").Include("AdverseEvent.Drugs")
                        where d.DrugId==drugId
                        select d).FirstOrDefault();
        }
        public AdverseEvent GetAdverseEvent(int adverseEventId)
        {
            var returnVar = _db.AdverseEvents.Include("Drugs").Include("TrialParticipant").Include("ReportingUser").FirstOrDefault(a => a.AdverseEventId == adverseEventId);
            returnVar.TrialParticipant.HospitalId = null;
            return returnVar;
        }
        public AdverseEvent GetAdverseEvent(int adverseEventId, string userName)
        {
            var returnVar = _db.AdverseEvents.Include("Drugs").Include("TrialParticipant").Include("ReportingUser").FirstOrDefault(a => a.AdverseEventId == adverseEventId);
            DecryptHospitalId(returnVar.TrialParticipant, userName);
            return returnVar;
        }
        public AdverseEvent CreateAdverseEvent(Int32 participantId, DateTime eventTime, Int32 severityLevelId, Int32 adverseEventTypeId,Boolean sequelae, Boolean fatalEvent, String details, string currentUser)
        {
            var participant = _db.TrialParticipants.Include("StudyCentre").Include("Death").FirstOrDefault(p => p.ParticipantId == participantId);
            var usr = _db.Users.FirstOrDefault(u => u.UserName == currentUser);
            ValidateAdverseEvent(participant, details, eventTime);
            if (!_validatonDictionary.IsValid) { return null; }

            var newEvent = new AdverseEvent()
            {
                ParticipantId = participantId,
                EventTime = eventTime,
                SeverityLevelId = severityLevelId,
                AdverseEventTypeId = adverseEventTypeId,
                Sequelae = sequelae,
                FatalEvent = fatalEvent,
                Details = details,
                ReportingUserId = usr.UserId,
                ReportingTimeLocal = participant.StudyCentre.LocalTime()
            };
            SendEventEmail(participant, usr, newEvent);
            _db.AdverseEvents.Add(newEvent);
            if (fatalEvent && participant.Death==null) 
            {
                var deathDetails = new ParticipantDeath() { Id=participantId, Details = "Fatal Adverse Event Logged: "+ details, Time = eventTime };
                participant.Death = deathDetails;
            }
            _db.SaveChanges(currentUser);
            return newEvent;
        }
        private void ValidateAdverseEvent(TrialParticipant participant, String details, DateTime eventTime)
        {
            if (participant == null) { _validatonDictionary.AddError("ParticipantId", "Participant does not exist"); }
            if (String.IsNullOrWhiteSpace(details)) { _validatonDictionary.AddError("Details", "details of the adverse event must be provided"); }
            TimeSpan timeFromEnrollment = eventTime - participant.LocalTimeRandomised;
            if (timeFromEnrollment.Days > 365 || timeFromEnrollment.Days < 0) { _validatonDictionary.AddError("EventTime", "must be after the participant was randomised and before 12 months has elapsed"); }
            if (participant.StudyCentre.LocalTime() < eventTime) { _validatonDictionary.AddError("EventTime", "must be before current date and time"); }
        }
        public AdverseEvent UpdateAdverseEvent(Int32 adverseEventId, DateTime eventTime, Int32 severityLevelId, Int32 adverseEventTypeId, Boolean sequelae, Boolean fatalEvent, String details, string currentUser)
        {
            var eventToUpdate = _db.AdverseEvents.Include("TrialParticipant").FirstOrDefault(e=> e.AdverseEventId == adverseEventId);
            ValidateAdverseEvent(eventToUpdate.TrialParticipant, details, eventTime);
            if (!_validatonDictionary.IsValid) { return null; }
            eventToUpdate.EventTime = eventTime;
            eventToUpdate.SeverityLevelId = severityLevelId;
            eventToUpdate.AdverseEventTypeId = adverseEventTypeId;
            eventToUpdate.Sequelae = sequelae;
            eventToUpdate.FatalEvent = fatalEvent;
            eventToUpdate.Details = details;
            _db.SaveChanges(currentUser);
            return eventToUpdate;
        }
        public HttpStatusCode Delete(int id, string userName)
        {
            var pv = _db.AdverseEvents.Find(id);
            if (pv == null) { return HttpStatusCode.NotFound; }
            _db.AdverseEvents.Remove(pv);
            _db.SaveChanges(userName);
            return HttpStatusCode.OK;
        }
        private void SendEventEmail(TrialParticipant participant, User usr, AdverseEvent adverseEvent)
        {
            var emailList = RoleExtensions.GetInvestigatorEmails(participant.StudyCentreId, _db);

            Email.Send(emailList,
                String.Format("Adverse Event (Category {0})",adverseEvent.SeverityLevelId),
                "SignificantEvent.txt",
                Email.EventDetails(adverseEvent.Severity.Description,
                                   usr.FirstName + " " + usr.LastName,
                                   usr.StudyCentre.Name,
                                   DateTime.Now.ToString(),
                                   participant.ParticipantId,
                                   adverseEvent.EventTime,
                                   adverseEvent.Details),
                usr.Email);
        }
    }
}