using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DabTrial.Infrastructure.Validation;
using MvcHtmlHelpers;
using Foolproof;


namespace DabTrial.Models
{
    public class ScreenedPatientListItem
    {
        public Int32 ScreenedPatientId { get; set; }
        public string StudyCentreAbbreviation { get; set; }
        public string HospitalId {get;set;}
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime IcuAdmissionDate {get; set;}
        public DateTime ScreeningDate { get; set; }
        public string ExclusionReasonAbbreviation {get;set;}
        public string NoConsentFreeText { get; set; }
        public bool IsRowInEditor { get; set; }
    }
    public class ScreenedPatientDetails
    {
        public Int32 ScreenedPatientId { get; set; }
        public string StudyCentreName { get; set; }
        public string HospitalId { get; set; }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}")]
        public DateTime IcuAdmissionDate { get; set; }
        public DateTime Dob { get; set; }
        public DateTime ScreeningDate { get; set; }
        public bool AllInclusionCriteriaPresent { get; set; }
        public bool AllExclusionCriteriaAbsent { get; set; }
        public bool NoConsentReasonDescription { get; set; }
        public bool ConsentRefused { get; set; }
        public string NoConsentFreeText { get; set; }
    }

    public class CreateEditScreenedPatient
    {
        public Int32 ScreenedPatientId { get; set; }
        [Display(Name = "Hospital Id", Description = "Medical record number or health index used by your institution")]
        [Required]
        public String HospitalId { get; set; }
        
        [ComesBeforeNowAtClient]
        [ComesAfter("CentreData.CommencedEnrollingOn", ErrorMessage="This date is BEFORE your centre began enrolling patients")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of ICU admission", Description = "Date of admission to ICU involved in study")]
        public DateTime? IcuAdmissionDate { get; set; }

        [ComesBefore("IcuAdmissionDate")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth", Description = "Date of Birth")]
        public DateTime? Dob { get; set; }

        [ComesAfter("IcuAdmissionDate")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Screening")]
        public DateTime? ScreeningDate { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Further Details")]
        [RequiredIf("ConsentRefused", true)]
        public string NoConsentFreeText { get; set; }

        [Display(Name = "Inclusion Criteria 1-4 Present", Description = "ALL inclusion criteria EXCEPT recruitment within 4 hours")]
        public bool AllInclusionCriteriaPresent { get; set; }
        public bool AllExclusionCriteriaAbsent { get; set; }
        public int? NoConsentAttemptId { get; set; }
        [Display(Name = "Consent Refused", Description = "(Include Details)")]
        public bool ConsentRefused { get; set; }

        public CentreSpecificPatientValidationInfo CentreData { get; set; }
        public ILookup<bool?,SelectListItem> NoConsentAttemptReasons { get; set; }

        public IEnumerable<ScreenedPatientListItem> ScreeningList { get; set; }
        public IEnumerable<int> NoConsentAttemptRequiresDetail { get; set; }
    }
}