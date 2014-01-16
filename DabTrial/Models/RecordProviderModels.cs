using System;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using DabTrial.Infrastructure.Validation;

namespace DabTrial.Models
{
    public class RecordProviderModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Name", Description = "A description of the hospital record numbering system")]
        [StringLength(50, MinimumLength = 2)]
        public String Name { get; set; }
        [Required]
        [IsValidRegEx]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Regular Expression must be between 2 and 50 characters")]
        public String HospitalNoRegEx { get; set; }
        [Required]
        [Display(Name = "Medical Record No Description", Description = "A description of the acceptable hospital medical record notation")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "The description must be between 2 and 100 characters")]
        public String NotationDescription { get; set; }
    }
}
