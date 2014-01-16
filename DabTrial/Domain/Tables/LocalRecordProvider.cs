using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using System.Text.RegularExpressions;

namespace DabTrial.Domain.Tables
{
    public class LocalRecordProvider : IDescribableEntity
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Name", Description = "The name of the hospital record indexing system")]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Regular Expression must be between 2 and 50 characters")]
        public string HospitalNoRegEx { get; set; }
        [Required]
        [Display(Name = "Medical Record No Description", Description = "A description of the acceptable hospital medical record notation")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The description must be between 2 and 100 characters")]
        public string NotationDescription { get; set; }

        [Display(Name="A list of centres using the same medical record numbers")]
        public virtual ICollection<StudyCentre> studyCentres { get; set; }

        public string Describe()
        {
            return "Name:\"" + Name + "\"";
        }
    }
    public static class LocalRecordProviderExtensions
    {
        public static bool IsValidHospitalNo(this LocalRecordProvider recordProvider, String HospitalId)
        {
            var idNoRegEx = new Regex(recordProvider.HospitalNoRegEx);
            return (idNoRegEx.IsMatch(HospitalId));
        }
    }
}
