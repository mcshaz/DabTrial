
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
        public double PredMg { get; set; }
        [DisplayFormat(DataFormatString = "{0:N1}")]
        public double AdrenalineMl { get; set; }
        public bool AdrenalineIs1pc { get; set; }
        public string LastDoseAdrenaline { get; set; }
        public string LastHighDoseSteroid { get; set; }
        public string LastDoseSteroid { get; set; }
    }
}