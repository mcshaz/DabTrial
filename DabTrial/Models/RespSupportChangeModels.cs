using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DabTrial.Infrastructure.Validation;
using MvcHtmlHelpers;

namespace DabTrial.Models
{
    public class CreateEditRespSupportChange
    {
        private int _respSupportChangeId = -1;
        public Int32 RespSupportChangeId { get { return _respSupportChangeId; } set { _respSupportChangeId = value; } }
        public Int32 ParticipantId { get; set; }

        public string TimeRandomised { get; private set; }
        [Display(Name = "Time therapy was instituted")]
        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [ComesAfter("TimeRandomised")]
        [ComesBeforeNowAtClient]
        public DateTime? ChangeTime { get; set; }
        [Required]
        [Display(Name = "Type of support")]
        public Int32? RespiratorySupportTypeId { get; set; }
        private ParticipantDetails _trialParticipant;
        public ParticipantDetails TrialParticipant
        {
            get { return _trialParticipant; }
            set
            {
                _trialParticipant = value;
                ParticipantId = _trialParticipant.ParticipantId;
                TimeRandomised = _trialParticipant.LocalTimeRandomised.ToString("s");
            }
        }
        public IEnumerable<DetailSelectListItem> RespSupportTypes { get; set; }
    }
    public class RespSupportChangeItem
    {
        public Int32 RespSupportChangeId { get; set; }
        [DisplayFormat(DataFormatString="{0:d/M/yyyy HH:mm}", ApplyFormatInEditMode=true)]
        [DataType(DataType.DateTime)]
        [Display(Name = "Time therapy was instituted")]
        public DateTime ChangeTime {get; set;}
        [Display(Name = "Type of support")]
        public string RespiratorySupportTypeDescription { get; set; }
    }
    public class RespSupportChangeDetails : RespSupportChangeItem
    {
        public int ParticipantId { get; set; }
    }
}