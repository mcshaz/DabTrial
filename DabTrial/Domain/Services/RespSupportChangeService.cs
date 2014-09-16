using System;
using System.Linq;
using System.Net;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;

namespace DabTrial.Domain.Services
{
    public class RespSupportChangeService : ServiceLayer
    {
        public RespSupportChangeService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public RespiratorySupportChange GetRespChange(int changeId)
        {
            return _db.RespiratorySupportChanges.Include("TrialParticipant").Include("TrialParticipant.RespiratorySupportChanges").Where(r => r.RespSupportChangeId == changeId).FirstOrDefault();
        }
        public RespiratorySupportChange Create(Int32 participantId, DateTime changeTime, Int32 respiratorySupportTypeId, string currentUser)
        {
            var participant = _db.TrialParticipants.Include("StudyCentre").Include("RespiratorySupportChanges").Where(p => p.ParticipantId == participantId).FirstOrDefault();
            Validate(changeTime, _db.RespiratorySupportTypes.Find(respiratorySupportTypeId), participant);
            if (!_validatonDictionary.IsValid) { return null; }
            var respChange = new RespiratorySupportChange() { ParticipantId = participantId, ChangeTime = changeTime, RespiratorySupportTypeId = respiratorySupportTypeId };
            _db.RespiratorySupportChanges.Add(respChange);
            _db.SaveChanges(currentUser);
            return respChange;
        }
        public RespiratorySupportChange Update(Int32 respSupportChangeId, DateTime changeTime, Int32 respiratorySupportTypeId, string currentUser)
        {
            var respChange = (from r in _db.RespiratorySupportChanges.Include("TrialParticipant")
                                  .Include("TrialParticipant.StudyCentre")
                                  .Include("TrialParticipant.RespiratorySupportChanges")
                                  .Include("TrialParticipant.RespiratorySupportChanges.RespiratorySupportType")
                              where r.RespSupportChangeId == respSupportChangeId
                              select r).FirstOrDefault();
            if (respChange == null)
            {
                _validatonDictionary.AddError("RespSupportChangeId", "Not a valid Id");
            }
            Validate(changeTime, _db.RespiratorySupportTypes.Find(respiratorySupportTypeId), respChange.TrialParticipant, respSupportChangeId);
            if (!_validatonDictionary.IsValid) { return null; }
            respChange.ChangeTime = changeTime;
            respChange.RespiratorySupportTypeId = respiratorySupportTypeId;
            _db.SaveChanges(currentUser);
            return respChange;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>response status code</returns>
        public HttpStatusCode Delete(Int32 respSupportChangeId, string currentUser)
        {
            
            var respChange = _db.RespiratorySupportChanges.Find(respSupportChangeId);
            if (respChange == null) { return HttpStatusCode.NotFound; }
            _db.RespiratorySupportChanges.Remove(respChange);
            _db.SaveChanges(currentUser);
            return HttpStatusCode.OK;
        }
        private void Validate(DateTime changeTime,RespiratorySupportType respSupportType, TrialParticipant participant, int respSupportChangeId=-1)
        {
            if (participant.RespiratorySupportChanges == null) { throw new ArgumentException("participant", "participant object must include ICollection RespiratorySupportChanges"); }
            if (changeTime > participant.StudyCentre.LocalTime()) 
            { 
                _validatonDictionary.AddError("ChangeTime", "any changes must be before current date and time"); 
            }
            if (changeTime < participant.LocalTimeRandomised) 
            { 
                _validatonDictionary.AddError("ChangeTime", "any changes must be after the participant was randomised"); 
            }
            var changeList = participant.RespiratorySupportChanges.OrderBy(r=>r.ChangeTime);
            var firstBefore = changeList.LastOrDefault(r => r.RespSupportChangeId != respSupportChangeId && r.ChangeTime < changeTime);
            if (firstBefore == null)
            {
                if (participant.RespiratorySupportAtRandomisation == respSupportType)
                {
                    _validatonDictionary.AddError("RespiratorySupportTypeId", String.Format("This is no diferent to the respiratory support at randomisation ({0:d/M/yyyy HH:mm})", participant.LocalTimeRandomised));
                }
            }
            else if (firstBefore.RespiratorySupportType == respSupportType)
            {
                _validatonDictionary.AddError("RespiratorySupportTypeId", String.Format("This is no diferent to the previous respiratory support logged on {0:d/M/yyyy HH:mm}", firstBefore.ChangeTime));
            }
            var firstAfter = changeList.FirstOrDefault(r => r.RespSupportChangeId != respSupportChangeId && r.ChangeTime > changeTime);
            if (firstAfter != null && firstAfter.RespiratorySupportType == respSupportType)
            {
                _validatonDictionary.AddError("RespiratorySupportTypeId", String.Format("This is no diferent to the subsequent respiratory support logged on {0:d/M/yyyy HH:mm}", firstAfter.ChangeTime));
            }
            var sameTime = changeList.Where(r => r.ChangeTime == changeTime && r.RespSupportChangeId != respSupportChangeId).FirstOrDefault();
            if (sameTime != null)
            {
                _validatonDictionary.AddError("ChangeTime", "The time conflicts with an existing entry");
            }
            if (participant.ReadyForIcuDischarge != null && participant.ReadyForIcuDischarge.Value < changeTime && respSupportType.RespSupportTypeId > participant.StudyCentre.MaxWardSupportId)
            {
                _validatonDictionary.AddError("ChangeTime",
                    string.Format("Invasive forms of support cannot be after the time ready for ICU Discharge ({0})", participant.ReadyForIcuDischarge));
            }
            if (participant.ActualIcuDischarge != null && participant.ActualIcuDischarge.Value < changeTime)
            {
                _validatonDictionary.AddError("ChangeTime",
                    string.Format("Respiratory support changes cannot be after the time of ICU Discharge ({0})", participant.ActualIcuDischarge));
            }
        }
    }
}