using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Models;
using DabTrial.Infrastructure.Utilities;
using LinqKit;

namespace DabTrial.Domain.Services
{
    public class StudyCentreService :ServiceLayer
    {
        public StudyCentreService(IValidationDictionary valDictionary, IDataContext DBcontext = null)
            : base(valDictionary, DBcontext)
        {
        }
        public StudyCentre GetCentreByPassword(String siteSpecificPassword)
        {
            return _db.StudyCentres.Where(c => c.SiteRegistrationPwd == siteSpecificPassword).FirstOrDefault();
        }
        public StudyCentre GetCentreByUser(String userName)
        {
            return (from u in _db.Users.Include("StudyCentre").Include("StudyCentre.RecordSystem")
                    where u.UserName == userName
                    select u.StudyCentre).FirstOrDefault();
        }
        public IEnumerable<StudyCentre> GetCentres()
        {
            return _db.StudyCentres.ToList();
        }
        public string[] GetCentreAbbreviations()
        {
            return (from s in _db.StudyCentres
                    select s.Abbreviation).ToArray();
        }
        public StudyCentre GetCentre(Int32 studyCentreId)
        {
            return _db.StudyCentres.Find(studyCentreId);
        }
        public void DeleteCentre(Int32 studyCentreId)
        {
            var centre = _db.StudyCentres.Find(studyCentreId);
            _db.StudyCentres.Remove(centre);
        }
        public void UpdateCentre(Int32 studyCentreId,
                                 String name,
                                 String abbreviation,
                                 String siteRegistrationPwd,
                                 String timeZoneId,
                                 String[] Domains,
                                 String publicPhoneNumber,
                                 Boolean isUsing1pcAdrenaline,
                                 DateTime commencedEnrollingOn,
                                 Int32 maxWardRespSupportId,
                                 String currentUserName)
        {

            validateCentre(name, abbreviation, siteRegistrationPwd, timeZoneId, studyCentreId);
            string domainCSL = domainsToString(Domains);
            var record = _db.StudyCentres.Find(studyCentreId);
            if (record == null) { throw new ArgumentException("Study Centre with Id:" + studyCentreId +" not found"); }
            if (_validatonDictionary.IsValid)
            {
                record.Name = name;
                record.SiteRegistrationPwd = siteRegistrationPwd;
                record.TimeZoneId = timeZoneId;
                record.PublicPhoneNumber = publicPhoneNumber;
                record.ValidEmailDomains = domainCSL;
                record.IsUsing1pcAdrenaline = isUsing1pcAdrenaline;
                record.CommencedEnrollingOn = commencedEnrollingOn;
                record.MaxWardSupportId = maxWardRespSupportId;
                _db.SaveChanges(currentUserName);
            }
        }

        public const string MrnDescriptionDefault = "Unique {0} record number";
        public void CreateCentre(String name,
                                 String abbreviation,
                                 String siteRegistrationPwd,
                                 Int32 recordProviderId,
                                 String timeZoneId,
                                 String[] Domains,
                                 String publicPhoneNumber,
                                 Boolean isUsing1pcAdrenaline,
                                 DateTime commencedEnrollingOn,
                                 Int32 maxWardRespSupportId,
                                 String currentUserName)
        {
            validateCentre(name, abbreviation, siteRegistrationPwd, timeZoneId);
            string domainCSL = domainsToString(Domains);
            if (_validatonDictionary.IsValid)
            {
                var centre = new StudyCentre()
                {
                    Name = name, 
                    Abbreviation = abbreviation,
                    RecordSystemProviderId = recordProviderId,
                    SiteRegistrationPwd = siteRegistrationPwd,
                    TimeZoneId = timeZoneId,
                    ValidEmailDomains = domainCSL,
                    RecordSystem = _db.LocalRecordProviders.Find(recordProviderId),
                    PublicPhoneNumber = publicPhoneNumber,
                    IsUsing1pcAdrenaline = isUsing1pcAdrenaline,
                    CommencedEnrollingOn = commencedEnrollingOn,
                    MaxWardSupportId = maxWardRespSupportId
                };
                _db.StudyCentres.Add(centre);
                _db.SaveChanges(currentUserName);
            }
        }
        private String domainsToString(String[] Domains)
        {
            string domainRegEx = null;
            Regex validDomain = new Regex(@"^@(\w+([-.]?\w+)){1,}\.\w{2,4}$");
            foreach (string d in Domains)
            {
                string domain = d.Trim();
                if (String.IsNullOrWhiteSpace(d))
                {
                    _validatonDictionary.AddError("Domains", "Domain names cannot be empty");
                }
                else if (!validDomain.IsMatch(domain))
                {
                    _validatonDictionary.AddError("Domains", domain + " is not a valid domain name");
                }
                domainRegEx += (domainRegEx == null) ? domain : ("," + domain);
            }
            return domainRegEx;
        }
        private void validateCentre(String name,
                                    String abbreviation,
                                    String siteRegistrationPwd,
                                    String timeZoneId,
                                    int studyCentreId=-1)
        {
            if ((from s in _db.StudyCentres
                 where s.StudyCentreId != studyCentreId && s.Name == name
                 select s).FirstOrDefault() != null) { _validatonDictionary.AddError("Name", "The name of the study centre must be unique. Another centre with this name already exists"); }
            if ((from s in _db.StudyCentres
                 where s.StudyCentreId != studyCentreId && s.Abbreviation == abbreviation
                 select s).FirstOrDefault() != null) { _validatonDictionary.AddError("Abbreviation", "The study centre abbreviation must be unique. Another centre with this abbreviation already exists"); }
            if ((from s in _db.StudyCentres
                 where s.StudyCentreId != studyCentreId && s.SiteRegistrationPwd == siteRegistrationPwd
                 select s).FirstOrDefault() != null) { _validatonDictionary.AddError("SiteRegistrationPwd", "The password must be unique. Another centre is already using this password"); }
            if (!IsValidTimeZoneId(timeZoneId)) { _validatonDictionary.AddError("TimeZoneId", String.Format("Cannot find timezone matching Id:'{0}'", timeZoneId)); }
        }

        private bool IsValidTimeZoneId(string timeZoneId)
        {
            try
            {
                var dummy = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                return false;
            }
            return true;
        }
    }
}