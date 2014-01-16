using DabTrial.Domain.Providers;
using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Models;
using System;
using System.Web.Mvc;

namespace DabTrial.Controllers
{
    public abstract class DataContextController : Controller, IDisposable
    {
        #region members
        protected readonly IDataContext dbContext;
        private ModelStateWrapper _valDictionary;
        protected readonly String CurrentUserName;
        private User _currentUser;
        private UserService _userService;
        private TrialParticipantService _participantService;
        #endregion
        #region instantiation
        public DataContextController()
            : this(new DataContext())
        {
        }
        public DataContextController(IDataContext context)
        {
            dbContext = context;
            ViewBag.Menu = new RoleMenu(context);
            CurrentUserName = WebSecurity.User.Identity.Name;
        }
        #endregion

        #region properties
        protected UserService UserService { get { return _userService ?? (_userService = new UserService(ValidationDictionary, dbContext)); } }
        protected ModelStateWrapper ValidationDictionary{get {return _valDictionary ?? (_valDictionary = new ModelStateWrapper(this.ModelState));}}
        protected TrialParticipantService ParticipantService { get { return _participantService ?? (_participantService = new TrialParticipantService(ValidationDictionary, dbContext)); } }
        protected User CurrentUser { get { return _currentUser ?? (_currentUser = UserService.GetUser(CurrentUserName));} }
        #endregion

        #region interface impementation
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_participantService != null) _participantService.Dispose();
                if (_userService != null) _userService.Dispose();
                dbContext.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}