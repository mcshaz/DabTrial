using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
namespace DabTrial.Domain.Services
{
    public class ScreenedPatientService : PatientServiceLayer
    {
        public ScreenedPatientService(IValidationDictionary validationDictionary, IDataContext context = null) : base(validationDictionary, context)
        {
        }
        public IEnumerable<ScreenedPatient> GetAllScreenedPatients(string currentUserName, int? skip=null, int? take=null)
        {
            var usr = _db.Users.Where(u => u.UserName == currentUserName).FirstOrDefault();
            var studyCentreId = usr.RestrictedToCentre();
            IQueryable<ScreenedPatient> returnVar;
            if (studyCentreId.HasValue)
            {
                 returnVar = (from s in _db.ScreenedPatients.Include("NoConsentReason")
                                                        where s.StudyCentreId == studyCentreId
                                                        orderby s.IcuAdmissionDate
                                                        select s);
                if (skip.HasValue)
                {
                    returnVar = returnVar.Skip(skip.Value);
                }
                if (take.HasValue)
                {
                    returnVar = returnVar.Take(take.Value);
                }
                return returnVar.ToList();
            }
            returnVar = (from s in _db.ScreenedPatients
                        orderby s.IcuAdmissionDate
                        select s);
            if (skip.HasValue)
            {
                returnVar = returnVar.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                returnVar = returnVar.Take(take.Value);
            }
            return DecryptHospitalId(returnVar.ToList(),usr).Cast<ScreenedPatient>();
        }
        public ScreenedPatient GetScreenedPatient(int Id, string currentUserName)
        {
            return (ScreenedPatient)DecryptHospitalId(_db.ScreenedPatients.Find(Id), _db.Users.Where(u => u.UserName == currentUserName).FirstOrDefault());
        }
        public IEnumerable<NoConsentAttempt> GetNoConsentReasons()
        {
            return _db.NoConsentAttempts.ToList();
        }
        public ScreenedPatient Update(int screenedPatientId, 
                                        string hospitalId, 
                                        DateTime icuAdmissionDate,
                                        DateTime dob,
                                        DateTime screeningDate,
                                        bool allInclusionCriteriaPresent,
                                        bool allExclusionCriteriaAbsent,
                                        int? noConsentAttemptId,
                                        bool consentRefused,
                                        string noConsentDetails, 
                                        string userName)
        {
            var sp = _db.ScreenedPatients.Include("StudyCentre").Where(s=>s.ScreenedPatientId==screenedPatientId).FirstOrDefault();
            if (sp == null)
            {
                _validatonDictionary.AddError("ScreenedPatientId", "Could not find this patient to update");
                return null;
            }
            hospitalId = hospitalId.ToUpper();
            validateScreenedPatient(sp.StudyCentre, hospitalId, icuAdmissionDate, dob, screeningDate, allInclusionCriteriaPresent, allExclusionCriteriaAbsent, noConsentAttemptId, consentRefused, noConsentDetails, screenedPatientId);
            if (!_validatonDictionary.IsValid) { return null; }
            sp.NoConsentFreeText = noConsentDetails;
            sp.HospitalId = CryptoProvider.Encrypt(hospitalId);
            sp.IcuAdmissionDate = icuAdmissionDate;
            sp.ScreeningDate = screeningDate;
            sp.Dob = dob;
            sp.AllInclusionCriteriaPresent = allInclusionCriteriaPresent;
            sp.AllExclusionCriteriaAbsent = allExclusionCriteriaAbsent;
            sp.NoConsentAttemptId = noConsentAttemptId;
            sp.ConsentRefused = consentRefused;
            _db.SaveChanges(userName);
            sp.HospitalId = hospitalId;//Ok to serve this back, as they obviously entered it - by rights should detach from entity state manager
            return sp;
        }
        public ScreenedPatient Create(
            string hospitalId, 
            DateTime icuAdmissionDate,
            DateTime dob,
            DateTime screeningDate,
            bool allInclusionCriteriaPresent,
            bool allExclusionCriteriaAbsent,
            int? noConsentAttemptId,
            bool consentRefused,
            string noConsentDetails, 
            string userName)
        {
            User researcher = _db.Users.Include("StudyCentre").Where(u => u.UserName == userName).FirstOrDefault();
            hospitalId = hospitalId.ToUpper();
            validateScreenedPatient(researcher.StudyCentre, hospitalId, icuAdmissionDate, dob, screeningDate, allInclusionCriteriaPresent, allExclusionCriteriaAbsent, noConsentAttemptId, consentRefused, noConsentDetails);
            if (!_validatonDictionary.IsValid) { return null; }
            var sp = new ScreenedPatient()
            {
                NoConsentFreeText = noConsentDetails,
                HospitalId = CryptoProvider.Encrypt(hospitalId),
                IcuAdmissionDate = icuAdmissionDate,
                ScreeningDate = screeningDate,
                Dob = dob,
                StudyCentreId = researcher.StudyCentre.StudyCentreId,
                NoConsentAttemptId = noConsentAttemptId,
                ConsentRefused = consentRefused,
                AllExclusionCriteriaAbsent = allExclusionCriteriaAbsent,
                AllInclusionCriteriaPresent = allInclusionCriteriaPresent,
                RegisteredBy = researcher
            };
            _db.ScreenedPatients.Add(sp);
            _db.SaveChanges(userName);
            sp.HospitalId = hospitalId; //Ok to serve this back, as they obviously entered it- by rights should detach from entity state manager
            return sp;
        }
        public void validateScreenedPatient(StudyCentre centre, 
            string hospitalId, 
            DateTime icuAdmissionDate,
            DateTime dob,
            DateTime screeningDate,
            bool allInclusionCriteriaPresent,
            bool allExclusionCriteriaAbsent,
            int? noConsentAttemptId,
            bool consentRefused,
            string noConsentDetails,
            int screeningLogId=-1)
        {
            if (centre.LocalTime() < icuAdmissionDate) { _validatonDictionary.AddError("IcuAdmission", "Must be on or before today"); }
            var priorRecord = PriorRecords(centre, hospitalId, icuAdmissionDate, screeningLogId);
            if (priorRecord == PatientRecord.SameScreeningDate)
            { 
                _validatonDictionary.AddError("HospitalId", "The ICU admission for this patient has already been logged"); 
            } 
            else if (priorRecord == PatientRecord.SameEnrollmentDate)
            {
                _validatonDictionary.AddError("HospitalId", "This patient has already been enrolled. if consent has been withdrawn, please register details on this participants case report form");
            }
            if (allInclusionCriteriaPresent && allExclusionCriteriaAbsent && noConsentAttemptId==null && !consentRefused) 
            {
                _validatonDictionary.AddError("", "The patient must be either enrolled in the trial, or have a reason for not being enrolled");
            }

            if (!centre.RecordSystem.IsValidHospitalNo(hospitalId))
            {
                _validatonDictionary.AddError("HospitalId", "The patient's hospital identification must be " + centre.RecordSystem.NotationDescription);
            }

            if (!(allInclusionCriteriaPresent || allExclusionCriteriaAbsent))
            {
                if (noConsentAttemptId != null) { _validatonDictionary.AddError("noConsentAttemptId", "Should not be given if not meeting other study criteria"); }
                if (consentRefused) { _validatonDictionary.AddError("ConsentRefused", "Cannot refuse consent if not meeting all study criteria"); }
            }
            if (string.IsNullOrWhiteSpace(noConsentDetails) && allInclusionCriteriaPresent && allExclusionCriteriaAbsent && // attempt to minimise unnecessary database lookups
                ( consentRefused || (noConsentAttemptId.HasValue && _db.NoConsentAttempts.Find(noConsentAttemptId).RequiresDetail)))
            {
                    _validatonDictionary.AddError("NoConsentFreeText", "Please include details of why consent was not attempted or was refused"); 
            }
            if (noConsentAttemptId != null && consentRefused) { _validatonDictionary.AddError("ConsentRefused", "Cannot refuse consent if consent was not attempted"); }

            if (dob > icuAdmissionDate) { _validatonDictionary.AddError("Dob", "Must be on or before the date of icu admission"); }
            if (screeningDate < icuAdmissionDate) { _validatonDictionary.AddError("ScreeningDate", "Must be on or after the date of icu admission"); }
            
        }
        private enum PatientRecord { NoRecord, SameEnrollmentDate, SameScreeningDate }
        private PatientRecord PriorRecords(StudyCentre centre, string hospitalId, DateTime icuAdmissionDate, int screeningLogId)
        {
            var saltPossibilities = CryptoProvider.SaltingCombinations();
            DateTime nextDay = icuAdmissionDate.AddDays(1);
            //note for enrollment - record system provider, for screening site (ie patient can be enrolled if transferred from 1 centre to another)
            var sameEnrollmentCriteria = (from p in _db.TrialParticipants
                                          where p.StudyCentreId == centre.StudyCentreId &&
                                                p.IcuAdmission >= icuAdmissionDate && p.IcuAdmission < nextDay
                                          select p);
            var sameScreeningCriteria = (from sp in _db.ScreenedPatients
                                         where sp.StudyCentreId == centre.StudyCentreId &&
                                               sp.IcuAdmissionDate == icuAdmissionDate && 
                                               sp.ScreenedPatientId != screeningLogId
                                         select sp);
            if (saltPossibilities==1 || (saltPossibilities < (sameEnrollmentCriteria.Count() + sameScreeningCriteria.Count())))
            {
                string[] possibleEncryptedIds = CryptoProvider.PossibleEncryptionValues(hospitalId);
                if (sameEnrollmentCriteria.Any(p => possibleEncryptedIds.Contains(p.HospitalId)))
                {
                    return PatientRecord.SameEnrollmentDate;
                }
                if (sameScreeningCriteria.Any(sp => possibleEncryptedIds.Contains(sp.HospitalId)))
                {
                    return PatientRecord.SameScreeningDate;
                }
            }
            else
            {
                var allHospitalIds = sameEnrollmentCriteria.Select(p => p.HospitalId).ToArray();
                if (allHospitalIds.Any(h => CryptoProvider.Decrypt(h) == hospitalId))
                {
                    return PatientRecord.SameEnrollmentDate;
                }
                allHospitalIds = sameScreeningCriteria.Select(sp => sp.HospitalId).ToArray();
                if (allHospitalIds.Any(h => CryptoProvider.Decrypt(h) == hospitalId))
                {
                    return PatientRecord.SameScreeningDate;
                }
            }
            return PatientRecord.NoRecord;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Resopnse status code</returns>
        public HttpStatusCode Delete(int id, string userName)
        {
            var sp = _db.ScreenedPatients.Find(id);
            if (sp == null) { return HttpStatusCode.NotFound; }
            _db.ScreenedPatients.Remove(sp);
            _db.SaveChanges(userName);
            return HttpStatusCode.OK;
        }
    }
}
