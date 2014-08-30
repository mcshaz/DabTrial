using System;
using System.Collections.Generic;
using System.Linq;
using DabTrial.Utilities;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Providers;
using DabTrial.Infrastructure.Interfaces;
using Hangfire;

namespace DabTrial.Domain.Services
{
    public class TrialParticipantService : PatientServiceLayer
    {
        public TrialParticipantService(IValidationDictionary valDictionary, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        private const int BlockSize = 4;
        protected void CreateAllocation(TrialParticipant participant)
        {
            var currentBlock = GetCurrentBlock(participant);
            int noInBlock = currentBlock.Count();
            if (noInBlock == 0)
            {
                //for random block size add:
                // newAllocation.BlockSize = BlockRandomisation.BlockSize();
                participant.BlockNumber = 1;
            }
            else if (noInBlock == BlockSize)
            {
                participant.BlockNumber = currentBlock.First().BlockNumber + 1;
                currentBlock = (new TrialParticipant[0]).AsQueryable();
            }
            else
            {
                // for Random blocksize need to get the current blocksize 
                // int currentBlock.First().Blocksize;
                participant.BlockNumber = currentBlock.Select(p=>p.BlockNumber).First();

            }
            participant.IsInterventionArm = BlockRandomisation.IsNextAllocationInterventionWithCentralTendancy(currentBlock.Select(p=>p.IsInterventionArm),BlockSize,new RandomAdaptor(),NewBlockProbIntervention);
        }
        public TrialParticipant GetParticipant(int participantId)
        {
            return _db.TrialParticipants.Find(participantId);
        }
        public TrialParticipant GetParticipant(int participantId, string currentUserName)
        {
            return GetParticipant(participantId, _db.Users.Include("StudyCentre").FirstOrDefault(u => u.UserName == currentUserName));
        }
        public TrialParticipant GetParticipant(int participantId, User currentUser)
        {
            return (TrialParticipant)DecryptHospitalId(_db.TrialParticipants.Find(participantId), currentUser);
        }
        public IEnumerable<DrugRoute> GetAllDrugRoutes()
        {
            return _db.DrugRoutes.ToList();
        }
        public IEnumerable<TrialParticipant> GetParticipantsByUserCentre(string userName)
        {
            return DecryptHospitalId((from u in _db.Users.Include("StudyCentre").Include("StudyCentre.TrialParticipants")
                                          .Include("StudyCentre.TrialParticipants.RespiratorySupportChanges")
                                          .Include("StudyCentre.TrialParticipants.RespiratorySupportChanges.RespiratorySupportType")
                                      where u.UserName==userName 
                                      select u.StudyCentre.TrialParticipants).FirstOrDefault()).Cast<TrialParticipant>();
        }
        public double NewBlockProbIntervention()
        {
            var dbSum = (from p in _db.TrialParticipants
                         group p by 0 into g
                         select new 
                         { 
                             x = g.Count(i=>i.IsInterventionArm),
                             n = g.Count()
                         }).FirstOrDefault();
            return 1.0-MathNet.Numerics.Distributions.Binomial.CDF(0.5, dbSum.n+1, dbSum.x);
        }

        protected IQueryable<TrialParticipant> GetCurrentBlock(TrialParticipant newParticipant)
        {
            if (newParticipant.RespiratorySupportAtRandomisation == null)
            {
                newParticipant.RespiratorySupportAtRandomisation = _db.RespiratorySupportTypes.Find(newParticipant.RespSupportTypeId);
            }
            IQueryable<TrialParticipant> partsInGroup = _db.TrialParticipants.Where(p=>p.StudyCentreId == newParticipant.StudyCentreId &&
                         p.HasCyanoticHeartDisease == newParticipant.HasCyanoticHeartDisease &&
                         p.HasChronicLungDisease == newParticipant.HasChronicLungDisease &&
                         p.RespiratorySupportAtRandomisation.RandomisationCategory == newParticipant.RespiratorySupportAtRandomisation.RandomisationCategory);

            int? maxBlockNumberInGrp = partsInGroup.Max(p => (int?)p.BlockNumber);
            if (maxBlockNumberInGrp.HasValue)
            {
                return partsInGroup.Where(p=>p.BlockNumber==maxBlockNumberInGrp.Value);
            }
            return (new TrialParticipant[0]).AsQueryable();
            //.OrderBy(p=>p.LocalTimeRandomised) if wanting to know variable block size assigned to first participant within block
            /*
            (from p in _db.TrialParticipants
                 where p.StudyCentreId == newParticipant.StudyCentreId &&
                         p.HasCyanoticHeartDisease == newParticipant.HasCyanoticHeartDisease &&
                         p.HasChronicLungDisease == newParticipant.HasChronicLungDisease &&
                         p.RespiratorySupportAtRandomisation.RandomisationCategory == newParticipant.RespiratorySupportAtRandomisation.RandomisationCategory
                 group p by p.BlockNumber into g
                 orderby g.Key descending
                 select g).FirstOrDefault()
             */ 
        }
        public TrialParticipant CreateNewParticipant(String hospitalId,
                                                     DateTime Dob,
                                                     double weight,
                                                     DateTime IcuAdmission,
                                                     int respSupportTypeId,
                                                     bool chronicLungDisease, 
                                                     bool cyanoticHeartDisease, 
                                                     int weeksGestationAtBirth, 
                                                     bool maleGender,
                                                     String currentUser)
        {
            User clinician = _db.Users.Include("StudyCentre").Include("StudyCentre.RecordSystem").FirstOrDefault(u => u.UserName == currentUser);
            if (clinician == null) { throw new ArgumentException("registeringUser not found"); }
            hospitalId = hospitalId.ToUpper();
            ValidateCreate(hospitalId,clinician.StudyCentre, IcuAdmission, respSupportTypeId);
            if (_validatonDictionary.IsValid)
            {
                TrialParticipant participant = new TrialParticipant
                {
                    HospitalId = CryptoProvider.Encrypt(hospitalId),
                    Dob = Dob,
                    Weight = weight,
                    IcuAdmission = IcuAdmission,
                    RespSupportTypeId = respSupportTypeId,
                    IsMaleGender = maleGender,
                    WeeksGestationAtBirth = weeksGestationAtBirth,
                    HasChronicLungDisease = chronicLungDisease,
                    HasCyanoticHeartDisease = cyanoticHeartDisease,

                    EnrollingClinicianId = clinician.UserId,
                    StudyCentreId = clinician.StudyCentreId.Value
                };
                CreateAllocation(participant);
                participant.LocalTimeRandomised = clinician.StudyCentre.LocalTime();

                _db.TrialParticipants.Add(participant);
                
                _db.SaveChanges(currentUser);

                BackgroundJob.Enqueue<CreateEmailService>(c => c.NotifyNewParticipant(participant.ParticipantId)); ;

                return participant;

            }
            return null;
        }
        public void Update(int participantId, 
            DateTime? readyForIcuDischarge,
            DateTime? actualIcuDischarge,
            DateTime? hospitalDischarge,
            int? numberOfSteroidDoses,
            int? numberOfAdrenalineNebulisers,
            bool? rsvPositive,
            bool? hmpvPositive,
            int? initialSteroidRouteId,
            DateTime? FirstAdrenalineNebAt,
            DateTime? LastAdrenalineNebAt,
            bool? SteroidsForPostExtubationStridor,
            bool? AdrenalineForPostExtubationStridor,
            DateTime? deathTime, 
            string deathDetails,
            DateTime? withdrawalTime, 
            string withdrawalReason,
            string userName)
        {
            var participant = _db.TrialParticipants.Include("Withdrawal")
                .Include("Death")
                .Include("StudyCentre")
                .Include("RespiratorySupportChanges")
                .Include("RespiratorySupportChanges.RespiratorySupportType")
                .First(p => p.ParticipantId == participantId);
            ValidateUpdate(participant, numberOfSteroidDoses, readyForIcuDischarge, actualIcuDischarge, hospitalDischarge, deathTime, deathDetails, withdrawalTime, withdrawalReason, userName);
            if (!_validatonDictionary.IsValid) { return; }
            participant.ReadyForIcuDischarge = readyForIcuDischarge;
            participant.ActualIcuDischarge = actualIcuDischarge;
            participant.HospitalDischarge = hospitalDischarge;
            participant.NumberOfSteroidDoses = numberOfSteroidDoses;
            participant.NumberOfAdrenalineNebulisers = numberOfAdrenalineNebulisers;
            participant.IsRsvPositive = rsvPositive;
            participant.IsHmpvPositive = hmpvPositive;
            participant.InitialSteroidRouteId = initialSteroidRouteId;
            participant.FirstAdrenalineNebAt = FirstAdrenalineNebAt;
            participant.FifthAdrenalineNebAt = LastAdrenalineNebAt;
            participant.SteroidsForPostExtubationStridor = SteroidsForPostExtubationStridor;
            participant.AdrenalineForPostExtubationStridor = AdrenalineForPostExtubationStridor;
            //deal with withdrawal
            if (participant.Withdrawal != null) //has DB record
            {
                if (withdrawalTime.HasValue) //entered value - update
                {
                    participant.Withdrawal.EventTime = withdrawalTime.Value;
                    participant.Withdrawal.Details = withdrawalReason;
                }
                else //no entered value - delete
                {
                    _db.ParticipantWithdrawals.Remove(participant.Withdrawal);
                }
            }
            else if (withdrawalTime.HasValue) //has no DB record but value entered - create
            {
                var withdrawalDetails = new ParticipantWithdrawal()
                    {
                        Id = participantId,
                        EventTime = withdrawalTime.Value,
                        Details = withdrawalReason,
                        ReportingTimeLocal = participant.StudyCentre.LocalTime(), 
                        ReportingUserId = (from u in _db.Users
                                           where u.UserName == userName
                                           select u.UserId).First()
                    };
                participant.Withdrawal = withdrawalDetails;
                _db.SaveChanges(userName);
                BackgroundJob.Enqueue<CreateEmailService>(c => c.NotifyParticipantWithdrawn(participantId));
            }
            //deal with death
            if (participant.Death != null) //has DB record
            {
                if (deathTime.HasValue) //entered value - update
                {
                    participant.Death.EventTime = deathTime.Value;
                    participant.Death.Details = deathDetails;
                }
                else //no entered value - delete
                {
                    _db.ParticipantDeaths.Remove(participant.Death);
                }
            }
            else if (deathTime.HasValue) //has no DB record but value entered - create
            {
                var deathEvent = new ParticipantDeath()
                {
                    Id = participantId,
                    EventTime = deathTime.Value,
                    Details = deathDetails,
                    ReportingTimeLocal = participant.StudyCentre.LocalTime(),
                    ReportingUserId = (from u in _db.Users
                                       where u.UserName == userName
                                       select u.UserId).First()
                };
                participant.Death = deathEvent;
                _db.SaveChanges(userName);
                BackgroundJob.Enqueue<CreateEmailService>(c => c.NotifyParticipantDeath(participantId));
            }
            _db.SaveChanges(userName);
        }
        private void ValidateUpdate(TrialParticipant participant,
            int? numberOfSteroidDoses,
            DateTime? readyForIcuDischarge,
            DateTime? actualIcuDischarge, 
            DateTime? hospitalDischarge,
            DateTime? deathTime, 
            string deathDetails, 
            DateTime? withdrawalTime, 
            string withdrawalReason, 
            string userName)
        {
            if (readyForIcuDischarge.HasValue || actualIcuDischarge.HasValue || hospitalDischarge.HasValue)
            {
                var localTime = (from u in _db.Users
                                 where u.UserName == userName
                                 select u.StudyCentre).First().LocalTime();
                var respSupports = participant.AllPriorRespSupports().ToArray();
                DateTime IcuTherapyStopped;
                if (deathTime != null || !respSupports.First().RespiratorySupportType.IsWardCompatible)
                {
                    IcuTherapyStopped = respSupports.First().ChangeTime;
                }
                else
                {
                    int i = 0;
                    while (++i < respSupports.Length && respSupports[i].RespiratorySupportType.IsWardCompatible){}
                    IcuTherapyStopped = respSupports[i - 1].ChangeTime;
                }
                DateTime mostRecentLogged = respSupports.First().ChangeTime;
                if (readyForIcuDischarge.HasValue)
                {
                    if (readyForIcuDischarge.Value > localTime) { _validatonDictionary.AddError("ReadyForIcuDischarge", "ready for ICU discharge must be before current date and time"); }
                    if (numberOfSteroidDoses.HasValue) 
                    {
                        int theoreticalDoses = theoreticalSteroidDoses(readyForIcuDischarge.Value - participant.IcuAdmission);
                        if (theoreticalDoses > numberOfSteroidDoses.Value)
                        {
                            _validatonDictionary.AddError("DaysOfSteroids",
                                                           String.Format("the patient should only have received {0} steroid doses, but has {1} doses of steroids recorded", theoreticalDoses, numberOfSteroidDoses.Value));
                        }
                    }
                    if (readyForIcuDischarge.Value < IcuTherapyStopped)
                    {
                        _validatonDictionary.AddError("ReadyForIcuDischarge",
                                                       String.Format("ready ICU discharge must be after commencement of the most recent non invasive respiratory support ({0})", IcuTherapyStopped));
                    }
                }
                if (actualIcuDischarge.HasValue)
                {
                    if (actualIcuDischarge.Value > localTime) { _validatonDictionary.AddError("ActualIcuDischarge", "ICU discharge must be before current date and time"); }
                    if (actualIcuDischarge.Value < mostRecentLogged)
                    {
                        _validatonDictionary.AddError("ActualIcuDischarge",
                                                       String.Format("actual ICU discharge must be after most recent logged respiratory support ({0})", mostRecentLogged));
                    }
                    if (readyForIcuDischarge.HasValue && actualIcuDischarge.Value < readyForIcuDischarge.Value)
                    {
                        _validatonDictionary.AddError("ActualIcuDischarge",
                                                       "Actual ICU discharge must be after the patient was ready for discharge");
                    }
                }
                if (hospitalDischarge.HasValue)
                {
                    if (hospitalDischarge.Value > localTime) { _validatonDictionary.AddError("HospitalDischarge", "Hospital discharge must be before current date and time"); }
                    if (hospitalDischarge.Value < mostRecentLogged.Date)
                    {
                        _validatonDictionary.AddError("HospitalDischarge",
                                                  String.Format("Hospital discharge must be after the most recent logged respiratory support ({0})", mostRecentLogged));
                    }
                    if (actualIcuDischarge.HasValue && actualIcuDischarge.Value > hospitalDischarge.Value)
                    {
                        _validatonDictionary.AddError("HospitalDischarge", "Hospital discharge must be on or after ICU discharge");
                    }
                }
                if (!participant.IsInterventionArm && (participant.FifthAdrenalineNebAt.HasValue || participant.NumberOfAdrenalineNebulisers.HasValue || participant.FirstAdrenalineNebAt.HasValue || participant.NumberOfSteroidDoses.HasValue))
                {
                    _validatonDictionary.AddError("", "Information given relating to intervention, but this patient is in the control arm");
                }
            }
            if (deathTime.HasValue == (String.IsNullOrWhiteSpace(deathDetails))) { _validatonDictionary.AddError("", "Please provide both Date and Details of death"); }
            if (withdrawalTime.HasValue == (String.IsNullOrWhiteSpace(withdrawalReason))) { _validatonDictionary.AddError("", "Please provide both Date and Details of withdrawal from study"); }
        }
        private static int theoreticalSteroidDoses(TimeSpan daysInIcu)
        {
            int hrs = daysInIcu.Hours;
            return 1 + (hrs >= 80 
                ? 9 + (hrs >= 152?3:(hrs-80)/24)
                : hrs / 8);
        }
        private void ValidateCreate(String hospitalId, StudyCentre centre, DateTime IcuAdmission, int respSupportTypeId)
        {
            if (String.IsNullOrWhiteSpace(hospitalId)) {throw new ArgumentNullException("HospitalId");}
            if (centre == null) { throw new ArgumentNullException("Clinician", "Must Include StudyCentre"); }
            if (centre.RecordSystem == null) { throw new ArgumentNullException("Clinician", "Must Include StudyCentre.RecordSystem"); }
            if (!centre.RecordSystem.IsValidHospitalNo(hospitalId))
            {
                _validatonDictionary.AddError("HospitalId", "The patient's hospital identification must be " + centre.RecordSystem.NotationDescription);
            }
            else if (SameHospitalNoForSite(hospitalId, centre))
            {
                _validatonDictionary.AddError("HospitalId", "A patient with the same Hospital ID has been previously enroled in the study");
            }
            if ((centre.LocalTime() - IcuAdmission).TotalMinutes >= 240)
            {
                _validatonDictionary.AddError("IcuAdmission", "The patient must be Enroled within 4 hours of being admitted to ICU");
            }
            if (!_db.RespiratorySupportTypes.Find(respSupportTypeId).RandomisationCategory.HasValue)
            {
                _validatonDictionary.AddError("RespSupportTypeId", "The current level of respiratory support is not appropriate for entry into the study");
            }
        }
        private bool SameHospitalNoForSite(string hospitalId, StudyCentre studyCentre)
        {
            var saltPossibilities = CryptoProvider.SaltingCombinations();
            var recordSystemParticipants = (from p in _db.TrialParticipants
                                    where p.StudyCentre.RecordSystemProviderId == studyCentre.RecordSystemProviderId
                                    select p);
            if (saltPossibilities == 1 || saltPossibilities < recordSystemParticipants.Count())
            {
                string[] possibleEncryptedIds = CryptoProvider.PossibleEncryptionValues(hospitalId);
                return recordSystemParticipants.Any(p => possibleEncryptedIds.Contains(p.HospitalId));
            }
            var allHospitalIds = recordSystemParticipants.Select(p => p.HospitalId).ToArray();
            return allHospitalIds.Any(h => CryptoProvider.Decrypt(h) == hospitalId);
        }
    }
}