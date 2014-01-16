using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
namespace DabTrial.Domain.Services
{
    public class RecordProviderService : ServiceLayer
    {
        public RecordProviderService(IValidationDictionary valDictionary, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public IEnumerable<LocalRecordProvider> GetAllRecordProviders()
        {
            return _db.LocalRecordProviders.ToList();
        }
        public LocalRecordProvider GetRecordProvider(int MrnId)
        {
            return _db.LocalRecordProviders.Find(MrnId);
        }
        public LocalRecordProvider CreateRecordProvider(String name, String hospitalNoRegEx, String notationDescription,String currentUserName ) 
        {
            Validate(name,  hospitalNoRegEx,  notationDescription);
            if (_validatonDictionary.IsValid)
            {
                LocalRecordProvider returnVal = new LocalRecordProvider() { Name = name, HospitalNoRegEx = hospitalNoRegEx, NotationDescription = notationDescription };
                _db.LocalRecordProviders.Add(returnVal);
                _db.SaveChanges(currentUserName);
                return returnVal;
            }
            return null;
        }
        public void UpdateRecordProvider( int mrnId, String name, String hospitalNoRegEx, String notationDescription,String currentUserName)
        {
            Validate(name, hospitalNoRegEx, notationDescription, mrnId);
            if (_validatonDictionary.IsValid)
            {
                var MR = _db.LocalRecordProviders.Find(mrnId);
                MR.Name = name;
                MR.HospitalNoRegEx = hospitalNoRegEx;
                MR.NotationDescription = notationDescription;
                _db.SaveChanges(currentUserName);
            }
        }
        private void Validate(String name, String hospitalNoRegEx, String notationDescription, int id=-1)
        {
            if (_db.LocalRecordProviders.Where(r => r.Id != id && r.Name == name).Any()) { _validatonDictionary.AddError("Name", "Name must be unique"); }
            if (!IsValidRegex(hospitalNoRegEx)) { _validatonDictionary.AddError("HospitalNoRegEx", "Is not a valid regular expression"); }
        }
        private static bool IsValidRegex(string pattern)
        {
            if (String.IsNullOrEmpty(pattern)) return false;

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }
    }
}
