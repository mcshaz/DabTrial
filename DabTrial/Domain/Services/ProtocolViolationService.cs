using System;
using System.Collections.Generic;
using System.Linq;
using DabTrial.Utilities;
using System.Net;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
namespace DabTrial.Domain.Services
{
    public class ProtocolViolationService : PatientServiceLayer
    {
        public ProtocolViolationService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public IEnumerable<ProtocolViolation> GetViolationsByCentre(int studyCentreId)
        {
            return (from a in _db.ProtocolViolations.Include("TrialParticipant")
                        where a.TrialParticipant.StudyCentreId == studyCentreId
                        select a).ToList();
        }
        public IEnumerable<ProtocolViolation> GetViolationsByParticipant(int participantId)
        {
            return _db.ProtocolViolations.Include("TrialParticipant").Where(a => a.ParticipantId == participantId).OrderBy(v => v.TimeOfViolation).ToList();
        }
        public IEnumerable<ProtocolViolation> GetAllViolations(string userName)
        {
            var returnVar = _db.ProtocolViolations.Include("TrialParticipant").ToList();
            DecryptHospitalId(returnVar.Select(pv => pv.TrialParticipant));
            return returnVar;
        }
        public IEnumerable<ProtocolViolation> GetAllViolations()
        {
            return _db.ProtocolViolations.ToList();
        }
        public ProtocolViolation GetViolation(int violationId)
        {
            var returnVar = _db.ProtocolViolations.Include("TrialParticipant").Include("TrialParticipant.StudyCentre").FirstOrDefault(v => v.ViolationId == violationId);
            returnVar.TrialParticipant.HospitalId = null;         
            return returnVar;
        }
        public ProtocolViolation GetViolation(int violationId, string userName)
        {
            var returnVar = _db.ProtocolViolations.Include("TrialParticipant").Include("TrialParticipant.StudyCentre").FirstOrDefault(v => v.ViolationId == violationId);
            DecryptHospitalId(returnVar.TrialParticipant, userName);
            return returnVar;
        }
        public ProtocolViolation CreateViolation(Int32 participantId, DateTime timeOfViolation, Boolean majorViolation, String details, string currentUser)
        {
            var participant = _db.TrialParticipants.Include("StudyCentre").FirstOrDefault(p => p.ParticipantId == participantId);
            var usr = _db.Users.FirstOrDefault(u => u.UserName == currentUser);
            ValidateViolation(participant, details, timeOfViolation);
            if (!_validatonDictionary.IsValid) { return null; }

            var newEvent = new ProtocolViolation()
            {
                ParticipantId = participantId,
                TimeOfViolation = timeOfViolation,
                Details = details,
                ReportingUserId = usr.UserId,
                MajorViolation = majorViolation,
                ReportingTimeLocal = participant.StudyCentre.LocalTime()
            };
            SendEventEmail(participant, usr, newEvent);
            _db.ProtocolViolations.Add(newEvent);
            _db.SaveChanges(currentUser);
            return newEvent;
        }
        public HttpStatusCode Delete(int id, string userName)
        {
            var pv = _db.ProtocolViolations.Find(id);
            if (pv == null) { return HttpStatusCode.NotFound; }
            _db.ProtocolViolations.Remove(pv);
            _db.SaveChanges(userName);
            return HttpStatusCode.OK;
        }
        private void ValidateViolation(TrialParticipant participant, String details, DateTime timeOfViolation)
        {
            if (participant == null) { _validatonDictionary.AddError("ParticipantId", "Participant does not exist"); }
            if (String.IsNullOrWhiteSpace(details)) { _validatonDictionary.AddError("Details", "details of the adverse event must be provided"); }
            TimeSpan timeFromEnrollment = timeOfViolation - participant.LocalTimeRandomised;
            if (timeFromEnrollment.Days > 365 || timeFromEnrollment.Days < 0) { _validatonDictionary.AddError("TimeOfViolation", "must be after the participant was randomised and before 12 months has elapsed"); }
            if (participant.StudyCentre.LocalTime() < timeOfViolation) { _validatonDictionary.AddError("TimeOfViolation", "must be before current date and time"); }
        }
        public ProtocolViolation UpdateViolation(Int32 violationId, DateTime timeOfViolation, Boolean majorViolation, String details, string currentUser)
        {
            var eventToUpdate = _db.ProtocolViolations.Include("TrialParticipant").FirstOrDefault(e => e.ViolationId == violationId);
            ValidateViolation(eventToUpdate.TrialParticipant, details, timeOfViolation);
            if (!_validatonDictionary.IsValid) { return null; }
            eventToUpdate.TimeOfViolation = timeOfViolation;
            eventToUpdate.MajorViolation = majorViolation;
            eventToUpdate.Details = details;
            _db.SaveChanges(currentUser);
            return eventToUpdate;
        }
        private void SendEventEmail(TrialParticipant participant, User usr, ProtocolViolation violation)
        {
            var emailList = RoleExtensions.GetInvestigatorEmails(participant.StudyCentreId, _db);
            string violationSeverity = violation.MajorViolation?"Major":"Minor";
            Email.Send(emailList,
                String.Format("Protocol violation (Classed {0})", violationSeverity),
                "SignificantEvent.txt",
                Email.EventDetails(violationSeverity + " Protocol Violation",
                                   usr.FirstName + " " + usr.LastName,
                                   usr.StudyCentre.Name,
                                   DateTime.Now.ToString(),
                                   participant.ParticipantId,
                                   violation.TimeOfViolation,
                                   violation.Details),
                usr.Email);
        }
    }
}