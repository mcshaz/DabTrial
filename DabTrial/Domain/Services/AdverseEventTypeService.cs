using System.Collections.Generic;
using System.Linq;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
namespace DabTrial.Domain.Services
{
    public class AdverseEventTypeService : ServiceLayer
    {
        public AdverseEventTypeService(IValidationDictionary valDictionary, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public IEnumerable<AdverseEventType> GetAllAdverseEventTypes()
        {
            return _db.AdverseEventTypes.ToList();
        }
        public AdverseEventType GetAdverseEventType(int Id)
        {
            return _db.AdverseEventTypes.Find(Id);
        }
        public void Update( int Id, string description,string userName)
        {
            var rs = _db.AdverseEventTypes.Find(Id);
            rs.Description = description;
            _db.SaveChanges(userName);
        }
        public AdverseEventType Create(string description, string userName)
        {
            var rs = new AdverseEventType()
            {
                Description = description,
            };
            _db.AdverseEventTypes.Add(rs);
            _db.SaveChanges(userName);
            return rs;
        }
        public void Delete(int id, string userName)
        {
            var rs = _db.AdverseEventTypes.Find(id);
            _db.AdverseEventTypes.Remove(rs);
            _db.SaveChanges(userName);
        }
    }
}
