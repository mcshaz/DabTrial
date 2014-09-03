using DabTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DabTrial.Domain.Tables
{
    public class TrialParticipant : IDescribableEntity, IPatient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParticipantId { get; set; }
        [Display(Name = "Hospital Id", Description = "Medical record number or health index used by your institution")]
        [Required]
        [StringLength(44, MinimumLength=44)]
        public string HospitalId { get; set; }
        [ForeignKey("StudyCentre")]
        [Display(Name = "Study Site")]
        public int StudyCentreId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth", Description = "day/month/year")]
        [DisplayFormat(DataFormatString="{0:d/M/yyyy}", ApplyFormatInEditMode=true)]
        public DateTime Dob { get; set; }
        [Required]
        [Range(2, 20)]
        public double Weight { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Date of ICU admission", Description = "Date and time of admission to ICU involved in study")]
        public DateTime IcuAdmission { get; set; }
        [Required]
        [ForeignKey("RespiratorySupportAtRandomisation")]
        [Display(Name = "Respiratory support", Description = "At time of randomisation")]
        public int RespSupportTypeId { get; set; }
        public bool IsMaleGender { get; set; }
        public bool HasChronicLungDisease { get; set; }
        public bool HasCyanoticHeartDisease { get; set; }
        [Range(23, 42)]
        public int WeeksGestationAtBirth { get; set; }
        public DateTime? ReadyForIcuDischarge { get; set; }
        public DateTime? ActualIcuDischarge { get; set; }
        public DateTime? HospitalDischarge { get; set; }
        public int? NumberOfSteroidDoses { get; set; }
        public int? NumberOfAdrenalineNebulisers { get; set; }
        public bool? IsRsvPositive { get; set; }
        public bool? IsHmpvPositive { get; set; }
        public DateTime? FirstAdrenalineNebAt { get; set; }
        public DateTime? FifthAdrenalineNebAt { get; set; }
        public bool? SteroidsForPostExtubationStridor { get; set; }
        public bool? AdrenalineForPostExtubationStridor { get; set; }
        [ForeignKey("InitialSteroidRoute")]
        public int? InitialSteroidRouteId { get; set; }
        
        [ForeignKey("EnrollingClinician")]
        public int EnrollingClinicianId { get; set; }
        [DataType(DataType.DateTime)]
        //[DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime LocalTimeRandomised { get; set; }
        public bool IsInterventionArm { get; set; }
        public int BlockNumber { get; set; }

        public virtual ParticipantWithdrawal Withdrawal { get; set; }
        public virtual ParticipantDeath Death { get; set; }
        public virtual User EnrollingClinician { get; set; }
        public virtual RespiratorySupportType RespiratorySupportAtRandomisation { get; set; }
        public virtual DrugRoute InitialSteroidRoute { get; set; }
        public virtual ICollection<AdverseEvent> AdverseEvents { get; set; }
        public virtual ICollection<RespiratorySupportChange> RespiratorySupportChanges { get; set; }
        public virtual StudyCentre StudyCentre { get; set; }
        public virtual ICollection<ProtocolViolation> Violations { get; set; }

        //public int BlockSize { get; set; } // only neccessary because of random blocksize
        //public bool InterventionArm { get;  set; } - in a blinded study, ?keep this in a 1:1 table

        public string Describe()
        {
            return "HospitalRecord:\"" + HospitalId + '"';
        }
    }
    public class ParticipantWithdrawal : OneTo1DiscrepancyReport
    {
        public bool OngoingDataOk { get; set; }
    }
    public class ParticipantDeath : OneTo1DiscrepancyReport
    {
    }
    public static class ParticipantExtensions
    {
        /// <summary>
        /// All respiratory supports, including support at randomisation in descending order
        /// </summary>
        /// <param name="participant"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<RespiratorySupportChange> AllPriorRespSupports(this TrialParticipant participant)
        {
            if (participant.RespiratorySupportChanges == null)
            {
                throw new ArgumentNullException("participant was passed without RespiratorySupportChanges collection - try using .Include");
            }
            return participant.RespiratorySupportChanges
                .Concat(new RespiratorySupportChange[] 
                { 
                    new RespiratorySupportChange 
                    { 
                        ParticipantId = participant.ParticipantId,
                        RespiratorySupportType = participant.RespiratorySupportAtRandomisation,
                        ChangeTime = participant.LocalTimeRandomised
                    }
                }).OrderByDescending(p=>p.ChangeTime);
        }
    }
}