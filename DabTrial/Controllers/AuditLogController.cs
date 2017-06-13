using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DabTrial.Models;
using Mvc.JQuery.DataTables;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Services;
using System.Linq;

namespace DabTrial.Controllers
{
    [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
    public class AuditLogController : DataContextController, IDisposable
    {
        private AuditLogService _logService;
        private AuditLogService LogService { get { return _logService ??  (_logService = new AuditLogService(ValidationDictionary, dbContext));}}

        //
        // GET: /AuditLog/
        [AutoMapModel(typeof(IEnumerable<AuditLogEntry>), typeof(IEnumerable<AuditLogListItem>))]
        public ViewResult Index()
        {
            return View();
        }

        //
        // GET: /AuditLog/Details/5

        public ViewResult Details(int id)
        {
            return View(LogService.getAuditLog(id));
        }
        [HttpPost, ValidateAntiForgeryToken]
        public DataTablesResult<AuditLogListItem> GetAuditLogEntries(DataTablesParam dataTableParam)
        {
            return DataTablesResult.Create(
                dbContext.AuditLog.Select(src => new AuditLogListItem
                {
                    UserName = src.UserName,
                    EventDate = src.EventDateUTC,
                    EventType = src.EventType,
                    TableName = src.TableName,
                    RecordId = src.RecordId,
                    ColumnName = src.ColumnName,
                    OriginalValue = src.OriginalValue ?? string.Empty,
                    NewValue = src.NewValue ?? string.Empty
                }),
                dataTableParam);
        }

        protected override void Dispose(bool disposing)
        {
            if (_logService !=null) _logService.Dispose();
            base.Dispose(disposing);
        }

    }
}