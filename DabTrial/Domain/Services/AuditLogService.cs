using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DabTrial.Domain.Services
{
    public class AuditLogService : ServiceLayer
    {
        public AuditLogService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public const int defaultRecords = 100;
        public IEnumerable<AuditLogEntry> getAuditLogs<T>(Func<AuditLogEntry, T> expression, Int32 NoRecords = defaultRecords, Int32 pageNo = 1)
        {
            Int32 skipNo = NoRecords * (pageNo - 1);
            return _db.AuditLog.OrderBy(expression).Skip(skipNo).Take(NoRecords).AsEnumerable();
        }
        public IEnumerable<AuditLogEntry> getAuditLogs(Int32 NoRecords = defaultRecords, Int32 pageNo = 1)
        {
            return getAuditLogs(l => l.EventDateUTC, NoRecords, pageNo);
        }
        public AuditLogEntry getAuditLog(Int32 AuditLogId)
        {
            return _db.AuditLog.Find(AuditLogId);
        }
    }
}