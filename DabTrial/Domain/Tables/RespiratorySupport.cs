using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DabTrial.Domain.Tables
{
    public class RespiratorySupportType : IDescribableEntity
    {
        [Key]
        public int RespSupportTypeId { get; set; }
        [StringLength(40, MinimumLength = 3)]
        [Display(Name = "Brief Description")]
        public string Description { get; set; }
        [StringLength(100)]
        [Display(Description="Including sub categories which should be included")]
        public string Explanation { get; set; }
        [Range(1,8)]
        [Display(Description="Blank equates to not applicable")]
        public int? RandomisationCategory { get; set; }
        [StringLength(12)]
        public string Abbrev { get; set; }

        public virtual ICollection<TrialParticipant> TrialParticipants { get; set; }
        public virtual ICollection<RespiratorySupportChange> RespiratorySupportChanges { get; set; }

        public string Describe()
        {
            return String.Format("Description:\"{0}\" RandomisationCategory:\"{1}\"", Description, RandomisationCategory);
        }
    }
    public class RespiratorySupportChange : IDescribableEntity
    {
        [Key]
        public int RespSupportChangeId { get; set; }
        [ForeignKey("TrialParticipant")]
        public int ParticipantId { get; set; }
        [Display(Name="Date and time new therapy was instituted")]
        public DateTime ChangeTime { get; set; }
        [ForeignKey("RespiratorySupportType")]
        public int RespiratorySupportTypeId { get; set; }

        public virtual TrialParticipant TrialParticipant { get; set; }
        public virtual RespiratorySupportType RespiratorySupportType { get; set; }

        public string Describe()
        {
            return String.Format("ParticipantId:\"{0}\" EventTime:\"{1:d/M/yyyy HH:mm}\"", ParticipantId, ChangeTime); 
        }
    }
}