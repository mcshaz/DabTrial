
using System.ComponentModel.DataAnnotations;
namespace DabTrial.Models
{
    public class DosingModel
    {
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double DexamethasoneMg { get; set; }
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double MethyPredMg { get; set; }
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double PredDailyMg { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double PredLoadMg { get; set; }
        [DisplayFormat(DataFormatString = "{0:G2}")]
        public double AdrenalineMl { get; set; }
        public bool AdrenalineIs1pc { get; set; }
        public string LastDoseAdrenaline { get; set; }
        public string LastHighDoseSteroid { get; set; }
        public string LastDoseSteroid { get; set; }
    }
}