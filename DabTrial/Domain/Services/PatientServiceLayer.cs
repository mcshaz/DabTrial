using DabTrial.Infrastructure.Crypto;
using System.Collections.Generic;
using System.Linq;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;

namespace DabTrial.Domain.Services
{
    public abstract class PatientServiceLayer : ServiceLayer
    {
        #region instantiation
        public PatientServiceLayer(IValidationDictionary valDictionary = null, IDataContext DBcontext = null) : base(valDictionary, DBcontext)
        {}
        #endregion
        #region members
        private ICryptoProvider _cryptoProvider;
        #endregion
        #region properties
        protected ICryptoProvider CryptoProvider
        {
            get
            {
                return _cryptoProvider ?? (_cryptoProvider = new SimpleAES(ProjectUniqueKeys.key, ProjectUniqueKeys.vector, ProjectUniqueKeys.possibleSalts));
            }
        }
        #endregion
        #region methods
        protected IPatient DecryptHospitalId(IPatient patient, string userName)
        {
            return DecryptHospitalId(patient, _db.Users.FirstOrDefault(u => u.UserName == userName));
        }
        protected IPatient DecryptHospitalId(IPatient patient, User usr)
        {
            if (patient == null) { return null; }
            return DecryptHospitalId(new IPatient[] { patient }, usr).First();
        }
        protected IEnumerable<IPatient> DecryptHospitalId(IEnumerable<IPatient> patients, string userName)
        {
            return DecryptHospitalId(patients, _db.Users.FirstOrDefault(u => u.UserName == userName));
        }
        protected IEnumerable<IPatient> DecryptHospitalId(IEnumerable<IPatient> patients, User usr)
        { 
            foreach (var patient in patients)
            {
                if (usr==null || usr.StudyCentreId != patient.StudyCentreId)
                {
                    patient.HospitalId = null;
                }
                else
                {
                    patient.HospitalId = CryptoProvider.Decrypt(patient.HospitalId);
                }
            }
            return patients;
        }

        /// <summary>
        /// Bypasses security checks  - BE SURE ONLY users belonging to the same centre as the user are used
        /// </summary>
        /// <param name="patients"></param>
        /// <returns></returns>
        protected IEnumerable<IPatient> DecryptHospitalId(IEnumerable<IPatient> patients)
        {
            foreach (var patient in patients)
            {
                patient.HospitalId = CryptoProvider.Decrypt(patient.HospitalId);
            }
            return patients;
        }
        #endregion
    }
}