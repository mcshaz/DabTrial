using DabTrial.Domain.Providers;
using DabTrial.Infrastructure.Interfaces;
using System;

namespace DabTrial.Domain.Services
{
    public abstract class ServiceLayer : IDisposable
    {
        protected IDataContext _db;
        protected IValidationDictionary _validatonDictionary; // Not required at present
        public ServiceLayer(IValidationDictionary valDictionary = null, IDataContext DBcontext = null)
        {
            //_validatonDictionary = validationDictionary;
            _db = DBcontext ?? new DataContext();
            _validatonDictionary = valDictionary ?? new EmptyValidationShell();
        }

        //disposing
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
