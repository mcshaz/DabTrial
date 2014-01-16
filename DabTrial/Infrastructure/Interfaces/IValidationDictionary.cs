using System.Web.Mvc;
namespace DabTrial.Infrastructure.Interfaces
{
    public interface IValidationDictionary
    {
        void AddError(string key, string errorMessage);
        bool IsValid { get; }
        //public void Merge(IValidationDictionary dictionary);
    }
    public class EmptyValidationShell : IValidationDictionary
    {
        public EmptyValidationShell() { _IsValid = true; }
        public void AddError(string key, string errorMessage) { _IsValid = false; }
        private bool _IsValid;
        public bool IsValid { get { return _IsValid; } }
        //public void Merge(IValidationDictionary dictionary) { _IsValid = _IsVallid && dictionary.IsValid; }
    }
    public class ModelStateWrapper : IValidationDictionary
    {
        private ModelStateDictionary _modelState;

        public ModelStateWrapper(ModelStateDictionary modelState)
        {
            _modelState = modelState;
        }

        #region IValidationDictionary Members
        /*
        public void Merge(IValidationDictionary dictionary)
        {
            var mergeWith = dictionary as ModelStateDictionary;
            if (mergeWith == null) { throw new System.TypeAccessException("Cannot explicitly convert to ModelStateDictionary"); }
            _modelState.Merge(mergeWith);
        }
         * */
        public void AddError(string key, string errorMessage)
        {
            _modelState.AddModelError(key, errorMessage);
        }

        public bool IsValid
        {
            get { return _modelState.IsValid; }
        }

        #endregion
    }
}
