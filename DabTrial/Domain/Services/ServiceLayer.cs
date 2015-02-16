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
            _isContextOwned = DBcontext == null;
            _db = DBcontext ?? new DataContext();
            _validatonDictionary = valDictionary ?? new EmptyValidationShell();
        }

        //disposing
        bool _disposedValue;
        bool _isContextOwned;
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposedValue)
            {
                if (disposing && _isContextOwned)
                {
                    _db.Dispose();
                }
            }
            this._disposedValue = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
