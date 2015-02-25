using DabTrial.Domain.Services;
using DabTrial.Domain.Tables;
using DabTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DabTrial.Controllers
{
    [Authorize(Roles = RoleExtensions.PrincipleInvestigator + "," + RoleExtensions.SiteInvestigator)]
    public class DataSummaryController : DataContextController, IDisposable
    {
        private DataSummaryService _summaryService;
        private DataSummaryService SummaryService { get { return _summaryService ?? (_summaryService = new DataSummaryService(dbContext)); } }

        [Authorize(Roles = RoleExtensions.PrincipleInvestigator)]
        [AutoMapModel(typeof(ILookup<string, DabTrial.Domain.Services.DataSummaryService.StageCount>), typeof(CentreDataStageMatrixModel))]
        public ViewResult AllCentreDataStage()
        {
            return View(SummaryService.DataStages());
        }

        public ViewResult CentreStatistics()
        {
            var model = new StudyCentreStats(SummaryService.GetStatistics(), UserService.GetUser(CurrentUserName).RestrictedToCentre());
            return View(model);
        }

        public ViewResult Index()
        {
            int? restrictToCentre = UserService.GetUser(CurrentUserName).RestrictedToCentre();
            return View(restrictToCentre.HasValue
                ?SummaryService.GetMissingParticipantsForCentre(restrictToCentre.Value)
                :SummaryService.GetMissingParticipantsAllCentres());
        }
    }
}