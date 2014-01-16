using System;
using System.ComponentModel.DataAnnotations;
using DabTrial.Models;


namespace DabTrial.Domain.Tables
{
    public class AuditLogEntry
    {
        [Key]
        public Int32 AuditLogId { get; set; }

        [Required]
        [MaxLength(50)]
        public String UserName { get; set; }
        //public virtual User user { get; set;  }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString="{0:u}")]
        [Required]
        public DateTime EventDateUTC { get; set; }

        [Required]
        [MaxLength(1)]
        public string EventType { get; set; }

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; }

        [Required]
        [MaxLength(100)]
        public string RecordId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }

        //This is a hack to overcome a bug in the current version of mvc.jquery.datatables
        public static explicit operator AuditLogListItem(AuditLogEntry record)
        {
            return new AuditLogListItem
            {
                ColumnName = record.ColumnName,
                EventDate = record.EventDateUTC,
                EventType = record.EventType,
                NewValue = record.NewValue,
                OriginalValue = record.OriginalValue,
                TableName = record.TableName,
                UserName = record.UserName
            };
        }
        //end hack!
    }
}
