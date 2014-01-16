using System;
using System.ComponentModel.DataAnnotations;

namespace DabTrial.Domain.Tables
{
    public class DrugRoute: IDescribableEntity
    {
        [Key]
        public int RouteId { get; set; }
        [StringLength(50)]
        public string Description { get; set; }
        public string Describe()
        {
            return Description;
        }
    }
}