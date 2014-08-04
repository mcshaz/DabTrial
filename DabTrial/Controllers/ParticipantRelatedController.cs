using DabTrial.Domain.Providers;
using DabTrial.Domain.Tables;
using System;
using System.Web.Mvc;

namespace DabTrial.Controllers
{
    public abstract class ParticipantRelatedController : DataContextController
    {
        #region fields
        bool? _isAjax;
        #endregion

        #region instantiation
        public ParticipantRelatedController()
            : base()
        {
        }
        public ParticipantRelatedController(IDataContext context)
            :base(context)
        {
        }
        #endregion

        #region properties
        protected bool IsAjax 
        { 
            get 
            {
                if (_isAjax == null)
                {
                    _isAjax = Request.IsAjaxRequest();
                    ViewBag.IsAjax = _isAjax.Value;
                }
                return _isAjax.Value;
            } 
        }
        #endregion

        #region methods
        protected TrialParticipant GetParticipant(int id)
        {
            return ParticipantService.GetParticipant(id, CurrentUserName);
        }
        #endregion
    }
}