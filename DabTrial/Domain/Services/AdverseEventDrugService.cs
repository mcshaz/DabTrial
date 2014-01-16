using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
namespace DabTrial.Domain.Services
{
    public class AdverseEventDrugService : ServiceLayer
    {
        public AdverseEventDrugService(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public Drug GetDrug(int drugId)
        {
            return (from d in _db.AdverseEventDrugs.Include("AdverseEvent").Include("AdverseEvent.TrialParticipant")
                        where d.DrugId==drugId
                        select d).FirstOrDefault();
        }
        public IEnumerable<Drug> GetDrugsForEvent(int adverseEventId)
        {
            return _db.AdverseEventDrugs.Where(d => d.AdverseEventId == adverseEventId).ToList();
        }
        public Drug CreateDrug(int adverseEventId, string drugName, string dosage, string reasonsForUse, DateTime startDate, DateTime? stopDate, string currentUser)
        {
            var newDrug = new Drug() { AdverseEventId = adverseEventId, DrugName = drugName, Dosage = dosage, ReasonsForUse = reasonsForUse, StartDate = startDate, StopDate = stopDate };
            _db.AdverseEventDrugs.Add(newDrug);
            _db.SaveChanges(currentUser);
            return newDrug;
        }
        public Drug UpdateDrug(int drugId, string drugName, string dosage, string reasonsForUse, DateTime startDate, DateTime? stopDate, string currentUser)
        {
            var drugToUpdate = _db.AdverseEventDrugs.Find(drugId);
            drugToUpdate.DrugName = drugName;
            drugToUpdate.Dosage = dosage;
            drugToUpdate.ReasonsForUse = reasonsForUse;
            drugToUpdate.StartDate = startDate;
            drugToUpdate.StopDate = stopDate;
            _db.SaveChanges(currentUser);
            return drugToUpdate;
        }
        public HttpStatusCode Delete(Int32 drugId, string currentUser)
        {

            var drugToDelete = _db.AdverseEventDrugs.Find(drugId);
            if (drugToDelete == null) { return HttpStatusCode.NotFound; }//204
            _db.AdverseEventDrugs.Remove(drugToDelete);
            _db.SaveChanges(currentUser);
            return HttpStatusCode.OK; //200
        }
    }
}