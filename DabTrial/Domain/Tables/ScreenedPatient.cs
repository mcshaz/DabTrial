using DabTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    public class ScreenedPatient :IDescribableEntity, IPatient
    {
        [Key]
        public int ScreenedPatientId { get; set; }
        [Display(Name = "Hospital Id", Description = "Medical record number or health index used by your institution")]
        [Required]
        [StringLength(44, MinimumLength = 44)]
        public string HospitalId { get; set; }
        [ForeignKey("StudyCentre")]
        [Display(Name = "Study Site")]
        public int StudyCentreId { get; set; }

        public DateTime IcuAdmissionDate { get; set; }
        public DateTime Dob { get; set; }
        public DateTime ScreeningDate { get; set; }
        public string NoConsentFreeText { get; set; }
        public bool AllInclusionCriteriaPresent { get; set; }
        public bool AllExclusionCriteriaAbsent { get; set; }
        [ForeignKey("NoConsentReason")]
        public int? NoConsentAttemptId { get; set; }
        public bool ConsentRefused { get; set; }
        [ForeignKey("TrialParticipant")]
        public int? ParticipantId { get; set; }
        [ForeignKey("RegisteredBy")]
        public int UserId { get; set; }

        public virtual NoConsentAttempt NoConsentReason { get; set; }
        public virtual StudyCentre StudyCentre { get; set; }
        public virtual TrialParticipant TrialParticipant { get; set; }
        public virtual User RegisteredBy { get; set; }

        public string Describe() 
        { 
            return "HospitalId:\"" + HospitalId + '"'; 
        }
    }

    public class NoConsentAttempt :IDescribableEntity
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="Clinician fully aware of trial")]
        public bool? IsFullyAware { get; set; }
        public bool RequiresDetail { get; set; }
        [StringLength(50, MinimumLength=3)]
        public string Abbreviation { get; set; }
        [StringLength(200, MinimumLength = 5)]
        public string Description { get; set; }

        public virtual ICollection<ScreenedPatient> ScreenedPatients { get; set; }

        public string Describe()
        {
            return String.Format("Id:\"{0}\" Abbrev:\"{1:d}\"", Id, Abbreviation);
        }
    }
}