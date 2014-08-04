using DabTrial.Domain.Providers;
using DabTrial.Infrastructure.Interfaces;
using DabTrial.Infrastructure.Utilities;
using DabTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqKit;
using System.Data.Entity;
using DabTrial.Domain.Tables;
using DabTrial.Utilities;

namespace DabTrial.Domain.Services
{
    public class DataSummaryService :ServiceLayer
    {
        public DataSummaryService(IDataContext dBcontext = null)
            : base(new EmptyValidationShell(), dBcontext)
        {
        }
        public class StageCount
        {
            public DataStageUtilities.TrialStage Stage { get; set; }
            public int Count { get; set; }
        }
        public ILookup<string, StageCount> DataStages()
        {
            var stageExpression = DataStageUtilities.StageExpression();
            var anon = (from p in _db.TrialParticipants.AsExpandable()
                        let Stage = stageExpression.Invoke(p)
                        group p by new { p.StudyCentre.Name, Stage = Stage } into pGrp
                        select new
                        {
                            pGrp.Key.Name,
                            pGrp.Key.Stage,
                            Count = pGrp.Count()
                        });
            return anon.ToLookup(a => a.Name, a => new StageCount { Stage = a.Stage, Count = a.Count });
        }
        public IEnumerable<StudyCentreStatistic> GetStatistics()
        {
            return (from s in _db.StudyCentres
                    select new StudyCentreStatistic
                    {
                        StudyCentreId = s.StudyCentreId,
                        Abbreviation = s.Abbreviation,
                        TotalParticiapants = s.TrialParticipants.Count,
                        CurrentParticipants = s.TrialParticipants.Count(p => p.ActualIcuDischarge == null),
                        InterventionArmCount = s.TrialParticipants.Count(p => p.IsInterventionArm),
                        DeathCount = s.TrialParticipants.Count(p => p.Death != null),
                        ViolationCount = s.TrialParticipants.SelectMany(p => p.Violations).Count(),
                        AdverseEventCount = s.TrialParticipants.SelectMany(p => p.AdverseEvents).Count(),
                        WithdrawnCount = s.TrialParticipants.Count(p => p.Withdrawal != null),
                        Screened = s.ScreenedPatients.Count,
                        Eligible = s.ScreenedPatients.Count(sp => sp.AllExclusionCriteriaAbsent && sp.AllInclusionCriteriaPresent),
                        MostRecentScreen = (from p in s.ScreenedPatients
                                            orderby p.IcuAdmissionDate descending
                                            select (DateTime?)p.IcuAdmissionDate).FirstOrDefault()
                    }).ToList();
        }
        public IEnumerable<MissingParticipantDataModel> GetMissingParticipantsAllCentres()
        {
            return GetMissingParticipantData(_db).ToList(); ;
        }
        public IEnumerable<MissingParticipantDataModel> GetMissingParticipantsForCentre(int centreId)
        {
            return GetMissingParticipantData(_db).Where(p => p.StudyCentreId == centreId).ToList();
        }
        internal static IQueryable<MissingParticipantDataModel> GetMissingParticipantData(IDataContext db)
        {
            var stageExpression = DataStageUtilities.StageExpression();
            DateTime now = DateTime.Now;
            return (from p in db.TrialParticipants.AsExpandable()
                    let stage = stageExpression.Invoke(p)
                    let anyLoggedChanges = p.RespiratorySupportChanges.Any()
                    let daysSinceEnrolled = DbFunctions.DiffDays(p.LocalTimeRandomised,now).Value
                    where stage != DataStageUtilities.TrialStage.Complete
                    select new MissingParticipantDataModel
                    {
                         ParticipantId = p.ParticipantId,
                         ActualDischarge = !!p.ActualIcuDischarge.HasValue, 
                         AdrenalineForStridor = !p.AdrenalineForPostExtubationStridor.HasValue, 
                         DaysSinceEnrolled = daysSinceEnrolled, 
                         HMPV = !p.IsHmpvPositive.HasValue, 
                         HospDischarge = !p.HospitalDischarge.HasValue, 
                         ReadyForDischarge = !p.ReadyForIcuDischarge.HasValue, 
                         RSV = !p.IsRsvPositive.HasValue, 
                         SteroidsForStridor = !p.SteroidsForPostExtubationStridor.HasValue, 
                         Stage = stage, 
                         StudyCentreAbbreviation=p.StudyCentre.Abbreviation, 
                         StudyCentreId = p.StudyCentre.StudyCentreId,
                         DaysSinceRespSupportLogged = anyLoggedChanges
                            ?DbFunctions.DiffDays(p.RespiratorySupportChanges.OrderByDescending(r => r.ChangeTime).FirstOrDefault().ChangeTime,now).Value
                            :daysSinceEnrolled,
                         LastLoggedSupport = (anyLoggedChanges 
                            ? p.RespiratorySupportChanges.OrderByDescending(r=>r.ChangeTime).FirstOrDefault().RespiratorySupportType
                            : p.RespiratorySupportAtRandomisation).Abbrev,
                         InterventionDetails = p.IsInterventionArm
                             ? new MissingParticipantDataModel.MissingInterventionDetails
                                 {
                                    FifthAdrenalineNebAt = !p.FifthAdrenalineNebAt.HasValue, 
                                    FirstAdrenalineNeb = !p.FirstAdrenalineNebAt.HasValue, 
                                    InitialSteroidRoute = !p.InitialSteroidRouteId.HasValue, 
                                    NumberOfAdrenalineNebulisers = !p.NumberOfAdrenalineNebulisers.HasValue, 
                                    NumberOfSteroidDoses = !p.NumberOfSteroidDoses.HasValue 
                                 }
                             :null
                    });
        }
    }
}