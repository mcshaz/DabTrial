using System.Collections.Generic;
using System.Linq;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Domain.Tables;
using DabTrial.Domain.Providers;
namespace DabTrial.Domain.Services
{
    public class RespSupportTypesService : ServiceLayer
    {

        public RespSupportTypesService(IValidationDictionary valDictionary, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public IEnumerable<RespiratorySupportType> GetAllRespSupportTypes()
        {
            return _db.RespiratorySupportTypes.ToList();
        }
        public RespiratorySupportType GetRespSupportType(int Id)
        {
            return _db.RespiratorySupportTypes.Find(Id);
        }
        public void Update( int Id, string description, int? randomisationCategory,string userName)
        {
            var rs = _db.RespiratorySupportTypes.Find(Id);
            rs.Description = description;
            rs.RandomisationCategory = randomisationCategory;
            _db.SaveChanges(userName);
        }
        public RespiratorySupportType Create(string description, int? randomisationCategory, string userName)
        {
            var rs = new RespiratorySupportType()
            {
                Description = description,
                RandomisationCategory = randomisationCategory
            };
            _db.RespiratorySupportTypes.Add(rs);
            _db.SaveChanges(userName);
            return rs;
        }
        public void Delete(int id, string userName)
        {
            var rs = _db.RespiratorySupportTypes.Find(id);
            _db.RespiratorySupportTypes.Remove(rs);
            _db.SaveChanges(userName);
        }
    }
}
