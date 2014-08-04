using DabTrial.Domain.Tables;
using DabTrial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.Utilities
{

    public static class DataStageUtilities
    {
        public enum TrialStage { Active, Complete, HospDischRqd, RespRqd, DetailsRqd }
        public static System.Linq.Expressions.Expression<Func<TrialParticipant, TrialStage>> StageExpression()
        {
            return p =>
            !p.ActualIcuDischarge.HasValue
                ? TrialStage.Active
                : (!p.ReadyForIcuDischarge.HasValue ||
                        !p.AdrenalineForPostExtubationStridor.HasValue ||
                        !p.IsHmpvPositive.HasValue ||
                        !p.IsRsvPositive.HasValue ||
                        !p.SteroidsForPostExtubationStridor.HasValue ||
                        p.IsInterventionArm &&
                            (!p.InitialSteroidRouteId.HasValue ||
                             !p.NumberOfSteroidDoses.HasValue ||
                             !p.FirstAdrenalineNebAt.HasValue ||
                             !p.FifthAdrenalineNebAt.HasValue ||
                             !p.NumberOfAdrenalineNebulisers.HasValue))
                    ? TrialStage.DetailsRqd
                    : (p.Death==null && !(p.RespiratorySupportChanges.OrderByDescending(r=>r.ChangeTime).FirstOrDefault().RespiratorySupportType ?? p.RespiratorySupportAtRandomisation).IsWardCompatible)
                        ? TrialStage.RespRqd
                        : (!p.HospitalDischarge.HasValue)
                            ? TrialStage.HospDischRqd
                            : TrialStage.Complete;
            /*
             return p =>
            p.ActualIcuDischarge == null
                ? TrialStage.Active
                : (p.ReadyForIcuDischarge == null ||
                        p.AdrenalineForPostExtubationStridor == null ||
                        p.IsHmpvPositive == null ||
                        p.IsRsvPositive == null ||
                        p.SteroidsForPostExtubationStridor == null ||
                        p.IsInterventionArm &&
                            (p.InitialSteroidRouteId == null ||
                             p.NumberOfSteroidDoses == null ||
                             p.FirstAdrenalineNebAt == null ||
                             p.FifthAdrenalineNebAt == null ||
                             p.NumberOfAdrenalineNebulisers == null))
                    ? TrialStage.DetailsRqd
                    : (p.Death == null && !p.AllPriorRespSupports().First().RespiratorySupportType.IsWardCompatible)
                        ? TrialStage.RespRqd
                        : (p.HospitalDischarge == null)
                            ? TrialStage.HospDischRqd
                            : TrialStage.Complete;
             */
        }
        static Func<TrialParticipant, TrialStage> _compiledStageExpression;
        public static Func<TrialParticipant, TrialStage> GetCompiledStageExpression()
        {
            return _compiledStageExpression ?? (_compiledStageExpression = StageExpression().Compile());
        }
    }
}