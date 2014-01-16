using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DabTrial.Infrastructure.Validation;
using System.Web.Mvc;
using MvcHtmlHelpers;
using Foolproof;

namespace DabTrial.Models
{
    public enum ViolationClass { Minor, Major }
    internal static class ViolationUtils
    {
        internal static ViolationClass? GetViolationClass(bool? value)
        {
            switch (value)
            {
                case true:
                    return ViolationClass.Major;
                case false:
                    return ViolationClass.Minor;
                default:
                    return null;
            }
        }
        internal static bool? GetBool(ViolationClass? value)
        {
            switch (value)
            {
                case ViolationClass.Major:
                    return true;
                case ViolationClass.Minor:
                    return false;
                default:
                    return null;
            }
        }
    }
    public class ProtocolViolationCreate
    {
        public int ParticipantId { get; set; }
        [Display(Name = "Hospital Id")]
        public string TrialParticipantHospitalId { get; set; }
        [UIHint("HiddenDate")]
        [DisplayFormat(DataFormatString = "{0:s}", ApplyFormatInEditMode = true)]
        public DateTime TrialParticipantLocalTimeRandomised { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode=true)]
        [ComesAfter("TrialParticipantLocalTimeRandomised", AnnotationArgumentType.PropertyName)]
        [ComesBeforeNowAtClient]
        [DataType(DataType.DateTime)]
        public DateTime? TimeOfViolation { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        public Boolean? MajorViolation { get; set; }
        [Display(Name="Violation Severity")]
        [Required]
        public ViolationClass? Violation
        {
            get {return ViolationUtils.GetViolationClass(MajorViolation);}
            set{ MajorViolation = ViolationUtils.GetBool(value);}
        }
    }
    public class ProtocolViolationEdit
    {
        [HiddenInput]
        public int ViolationId { get; set; }
        public int ParticipantId { get; set; }
        [Display(Name = "Hospital Id")]
        public string TrialParticipantHospitalId { get; set; }
        [UIHint("HiddenDate")]
        [DisplayFormat(DataFormatString = "{0:s}", ApplyFormatInEditMode = true)]
        public DateTime TrialParticipantLocalTimeRandomised { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ComesAfter("TrialParticipantLocalTimeRandomised", AnnotationArgumentType.PropertyName)]
        [ComesBeforeNowAtClient]
        public DateTime TimeOfViolation { get; set; }
        [DataType(DataType.MultilineText)]
        public string Details { get; set; }
        public Boolean MajorViolation { get; set; }

        [Display(Name = "Violation Severity")]
        [Required]
        public ViolationClass ViolationSeverity
        {
            get { return ViolationUtils.GetViolationClass(MajorViolation).Value; }
            set { MajorViolation = ViolationUtils.GetBool(value).Value; }
        }
    }
    public class ProtocolViolationListItem
    {
        public int ViolationId { get; set; }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public DateTime TimeOfViolation { get; set; }
        public string Details { get; set; }
        public Boolean MajorViolation { get; set; }
        public String ViolationClass
        {
            get
            {
                return (MajorViolation) ? "Major" : "Minor";
            }
        }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public DateTime ReportingTimeLocal { get; set; }
        public string ParticipantId { get; set; }
        public string TrialParticipantStudyCentreAbbreviation { get; set; }
    }
    public class ProtocolViolationDetails
    {
        public int ViolationId { get; set; }
        public int ParticipantId { get; set; }
        [Display(Name = "Hospital Id")]
        public string TrialParticipantHospitalId { get; set; }
        [DisplayFormat(DataFormatString="{0:d/M/yyyy HH:mm}")]
        public DateTime TimeOfViolation { get; set; }
        public string Details { get; set; }
        public Boolean MajorViolation { get; set; }
        public String ViolationClass
        {
            get
            {
                return (MajorViolation) ? "Major" : "Minor";
            }
        }
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}")]
        public DateTime ReportingTimeLocal { get; set; }
        public string TrialParticipantStudyCentreName { get; set; }
        public string ReportingUserFullName { get; set; }
        public string ReportingUserEmail { get; set; }
    }
}