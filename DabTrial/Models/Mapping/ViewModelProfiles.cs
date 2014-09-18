using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System;
using DabTrial.Utilities;
using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Utilities;

namespace DabTrial.Models
{
    public class HospitalMrnProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<LocalRecordProvider, RecordProviderModel>();
        }
    }
    public class ProtocolViolationProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ProtocolViolation, ProtocolViolationCreate>()
                .ForMember(dest=>dest.Violation, opt=>opt.Ignore())
                .ForMember(dest => dest.TrialParticipantHospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo));

            Mapper.CreateMap<ProtocolViolation, ProtocolViolationDetails>()
                .ForMember(dest=>dest.ViolationClass,opt=>opt.Ignore())
                .ForMember(dest=>dest.ReportingUserFullName, opt=>opt.MapFrom(src=>src.ReportingUser.FirstName + " " +src.ReportingUser.LastName))
                .ForMember(dest => dest.TrialParticipantHospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo));
            Mapper.CreateMap<ProtocolViolation, ProtocolViolationListItem>()
                .ForMember(dest => dest.ViolationClass, opt => opt.Ignore());
            Mapper.CreateMap<ProtocolViolation, ProtocolViolationEdit>()
                .ForMember(dest=>dest.ViolationSeverity, opt=>opt.Ignore())
                .ForMember(dest => dest.TrialParticipantHospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo));
        }
    }
    public class ScreeningLogProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<ScreenedPatient, CreateEditScreenedPatient>()
                .ForMember(dest=>dest.CentreData, opt=>opt.Ignore())
                //.ForMember(dest => dest.ScreeningList, opt => opt.Ignore())
                .ForMember(dest => dest.HospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo))
                .ForMember(dest => dest.NoConsentAttemptReasons, opt=>opt.Ignore())
                .ForMember(dest => dest.NoConsentAttemptRequiresDetail, opt => opt.Ignore())
                .ForMember(dest=>dest.StudyCentreAbbreviations, opt=>opt.Ignore());

            Mapper.CreateMap<ScreenedPatient, ScreenedPatientListItem>()
                //.ForMember(dest=>dest.IsRowInEditor, opt=>opt.Ignore())
                .ForMember(dest => dest.ExclusionReasonAbbreviation, opt => opt.MapFrom(GetExclusionReason()))
                .ForMember(dest => dest.HospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo))
                .ForMember(dest=>dest.NoConsentFreeText, opt => opt.MapFrom(src=>src.NoConsentFreeText.ToBriefString()));

            Mapper.CreateMap<ScreenedPatient, ScreenedPatientDetails>()
                .ForMember(dest => dest.HospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo));
        }

        internal static System.Linq.Expressions.Expression<Func<ScreenedPatient, string>> GetExclusionReason()
        {
            return src =>
                    !src.AllInclusionCriteriaPresent ? "Inclusions"
                        : !src.AllExclusionCriteriaAbsent ? "Exclusions"
                            : src.NoConsentAttemptId.HasValue ? src.NoConsentReason.Abbreviation
                                : src.ConsentRefused ? "Refused" 
                                    : "?";
        }
    }
    public class RespSupportChangeMapProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<RespiratorySupportChange, RespSupportChangeItem>();

            Mapper.CreateMap<RespiratorySupportChange, CreateEditRespSupportChange>()
                .ForMember(dest => dest.RespSupportTypes, opt => opt.Ignore())
                .ForMember(dest => dest.TimeRandomised, opt => opt.Ignore());
        }
    }
    public class ParticipantMapProfile : Profile
    {
        internal const string NullHospNo = "Encrypted";
        protected override void Configure()
        {

            Mapper.CreateMap<TrialParticipant, ParticipantListItem>()
                .ForMember(dest => dest.TrialArm, opt => opt.MapFrom(src => src.IsInterventionArm ? "Intervention Arm" : "Control Arm"))
                .ForMember(dest=>dest.EnrollingClinicianFullName, opt=>opt.MapFrom(src=>src.EnrollingClinician.FirstName + " " + src.EnrollingClinician.LastName))
                .ForMember(dest => dest.HospitalId, opt => opt.NullSubstitute(NullHospNo))
                .ForMember(dest=>dest.DataStage, opt=>opt.ResolveUsing<ParticipantToDataStageResolver>());

            Mapper.CreateMap<TrialParticipant, ParticipantDetails>()
                .ForMember(dest => dest.EnrollingClinicianFullName, opt => opt.MapFrom(src => src.EnrollingClinician.FirstName + " " + src.EnrollingClinician.LastName))
                .ForMember(dest => dest.Gender, opt=>opt.MapFrom(src => src.IsMaleGender?"Male":"Female"))
                .ForMember(dest=>dest.TrialArm,opt=>opt.MapFrom(src=>src.IsInterventionArm?"Intervention Arm":"Control Arm"))
                .ForMember(dest=>dest.RespiratorySupportChanges,opt=>opt.MapFrom(src=>src.RespiratorySupportChanges))
                .ForMember(dest=>dest.HospitalId,opt=>opt.NullSubstitute(NullHospNo))
                .ForMember(dest => dest.DrugDoses, opt => opt.ResolveUsing <ParticipantToDosingModelResolver>());

            Mapper.CreateMap<TrialParticipant, ParticipantUpdate>()
                .ApplyParticipantUpdateMapping();
                
            Mapper.CreateMap<TrialParticipant, ParticipantInterventionUpdate>()
                .ForMember(dest => dest.InitialSteroidRoutes, opt => opt.Ignore())
                .ApplyParticipantUpdateMapping();
        }
    }
    //http://stackoverflow.com/questions/6212516/automapper-inheritance-reusing-maps
    public static class MappingExtensions
    {
        public static IMappingExpression<S, D> ApplyParticipantUpdateMapping<S, D>(this IMappingExpression<S, D> iMappingExpression)
            where S : TrialParticipant
            where D : ParticipantUpdate
        {
            iMappingExpression
                .ForMember(dest => dest.Died, opt => opt.MapFrom(src => src.Death != null))
                .ForMember(dest => dest.WithdrawnFromStudy, opt => opt.MapFrom(src => src.Withdrawal != null))
                .ForMember(dest => dest.MostRecentLoggedEvent, opt => opt.MapFrom(src => src.RespiratorySupportChanges.Any()
                    ? src.RespiratorySupportChanges.OrderByDescending(r => r.ChangeTime).First().ChangeTime
                    : src.LocalTimeRandomised));
            return iMappingExpression;
        }
    }
    public class SimpleTypeProfiles : Profile
    {
        protected override void Configure()
        {
            //CreateMap<string, DateTime>().ConvertUsing(Convert.ToDateTime);
            CreateMap<DateTime, string>().ConvertUsing(d=>d.ToString("s"));
            CreateMap<DateTime?, string>().ConvertUsing(d => d.GetValueOrDefault().ToString("s"));
        }
    }
    public class AdverseEventProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Drug, DrugListItem>();
            Mapper.CreateMap<AdverseEvent, AdverseEventCreateModel>()
                .ForMember(dest => dest.SeverityLevels, opt => opt.Ignore())
                .ForMember(dest => dest.EventTypes, opt => opt.Ignore())
                .ForMember(dest => dest.TrialParticipantHospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo));
            Mapper.CreateMap<AdverseEvent, AdverseEventEditModel>()
                .ForMember(dest => dest.SeverityLevels, opt => opt.Ignore())
                .ForMember(dest => dest.EventTypes, opt => opt.Ignore())
                .ForMember(dest => dest.TrialParticipantHospitalId, opt => opt.NullSubstitute(ParticipantMapProfile.NullHospNo));
            Mapper.CreateMap<AdverseEvent, AdverseEventDetails>()
                .ForMember(dest=>dest.ReportingUserFullName, opt=>opt.MapFrom(src=>src.ReportingUser.FirstName + " " + src.ReportingUser.LastName));
            Mapper.CreateMap<AdverseEvent, AdverseEventListItem>();
            Mapper.CreateMap<Drug, DrugCreateModify>();
            Mapper.CreateMap<Drug, DrugListItem>();
            Mapper.CreateMap<Drug, ConfirmDeleteDrugModel>();
        }
    }
    public class StudyCentreMapProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<StudyCentre, StudyCentreListItem>()
                .ForMember(dest => dest.ValidDomainList, opt => opt.MapFrom(src => src.ValidEmailDomains.Replace(",",", ")));

            Mapper.CreateMap<StudyCentre, StudyCentreEdit>()
                .ForMember(dest => dest.TimeZones, opt => opt.Ignore())
                .ForMember(dest => dest.ValidDomainList, opt => opt.MapFrom(src => src.ValidEmailDomains.Replace(",",", ")))
                .ForMember(dest => dest.PublicPhoneNumber, opt=>opt.MapFrom(src=> new PhoneNumber{Formatted = src.PublicPhoneNumber}))
                .ForMember(dest=>dest.RespSupports, opt=>opt.Ignore());

            Mapper.CreateMap<StudyCentre, StudyCentreDetails>()
                .ForMember(dest => dest.ValidDomainList, opt => opt.MapFrom(src => src.ValidEmailDomains.Replace(",", ", "))); ;

                //.ForMember(dest => dest.ValidDomainList, opt => opt.ResolveUsing<emailRegexToList>().FromMember(src => src.ValidDomainsRegEx));
            Mapper.CreateMap<StudyCentre, CentreSpecificPatientValidationInfo>();
            //.ForMember(dest => dest.Abbreviation, opt=>opt.MapFrom(src=>src.Abbreviation + " Id"));

            string[] total = new string[] { "Total" };
            Mapper.CreateMap<ILookup<string, DabTrial.Domain.Services.DataSummaryService.StageCount>, CentreDataStageMatrixModel>()
                .ForMember(dest => dest.RowCentreNames, opt => opt.MapFrom(src => src.Select(s => s.Key).Concat(total)))
                .ForMember(dest => dest.ColDataStages, opt => opt.MapFrom(src => Enum.GetValues(typeof(DataStageUtilities.TrialStage)).Cast<DataStageUtilities.TrialStage>().Select(s => s.ToString().ToSeparateWords()).Concat(total)))
                .ForMember(dest => dest.Counts, opt => opt.ResolveUsing<StageLookupToMatrixResolver>());
        }
    }
    internal class EmailRegexToList : ValueResolver<string, string>
    {
        protected override string ResolveCore(string source)
        {
            return source.Replace(@"\","").Replace('|',',');
        }
    }
    public class UserMapProfile : Profile
    {
        protected override void Configure()
        {
            //ForSourceType<Name>().AddFormatter<NameFormatter>();
            //ForSourceType<decimal>().AddFormatExpression(context => 
            //    ((decimal) context.SourceValue).ToString("c"));
            Mapper.CreateMap<User, InvestigatorEditBrief>()
                .Include<User, InvestigatorCreateEdit>();

            Mapper.CreateMap<User, InvestigatorCreateEdit>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(u => u.Roles.Select(r => r.RoleName).FirstOrDefault(r => r != "DatabaseAdmin")))
                .ForMember(dest => dest.RolesSelectList, opt => opt.Ignore())
                .ForMember(dest => dest.StudyCentresSelectList, opt => opt.Ignore())
                .ForMember(dest =>dest.IsDbAdmin, opt=>opt.MapFrom(u=>u.Roles.Any(r=>r.RoleName=="DatabaseAdmin")))
                .ForMember(dest=>dest.CanAssignDbAdmin, opt=>opt.Ignore());

            Mapper.CreateMap<User, InvestigatorDetailsBrief>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .Include<User,InvestigatorDetailsFull>();

            Mapper.CreateMap<User, InvestigatorDetailsFull>()
                .ForMember(dest=>dest.Role, opt=>opt.MapFrom(src=>src.Roles.First().RoleName.ToSeparateWords()));

            Mapper.CreateMap<User, InvestigatorListItem>()
                .ForMember(dest => dest.FullName, opts => opts.MapFrom(usr => usr.FirstName + " " + usr.LastName));

            Mapper.CreateMap<ICollection<Role>, String>()
                .ConvertUsing<RolesToStringConverter>();

            Mapper.CreateMap<ProfessionalRoles?, string>()
                .ConvertUsing<ProfessionalRoleToStringConverter>();
            /*
            Mapper.CreateMap<MailInvestigator, ForwardMailInvestigator>()
                .ForMember(dest => dest.To, opts => opts.MapFrom(src => src.Email))
                .ForMember(dest => dest.Attachments, opts => opts.Ignore())
                .ForMember(dest => dest.ViewData, opts => opts.Ignore())
                .ForMember(dest => dest.ViewName, opts => opts.Ignore());
             * */
        }
    }
    public class AuditLogProfile : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<AuditLogEntry, AuditLogListItem>()
                .ForMember(d => d.EventDate, opts => opts.MapFrom(e => e.EventDateUTC));
        }
    }
    internal class  ProfessionalRoleToStringConverter : ITypeConverter<ProfessionalRoles?, String>
    {
        public String Convert(ResolutionContext context)
        {
            if (context.SourceValue == null) { return string.Empty; }
            var professionalRole = ((ProfessionalRoles?)context.SourceValue).Value;
            return Enum.GetName(typeof(ProfessionalRoles), professionalRole).ToSeparateWords();
        }
    }

    internal class RolesToStringConverter : ITypeConverter<ICollection<Role>, String>
    {
        public String Convert(ResolutionContext context)
        {
            var role = (ICollection<Role>)context.SourceValue;
            if (role == null || !role.Any()) { return string.Empty; }
            return role.Select(r => r.RoleName.ToSeparateWords())
                        .Aggregate((current, next) => current + "/" + next)
                        .Replace("database", "DB", StringComparison.OrdinalIgnoreCase);
        }
    }

    internal class StageLookupToMatrixResolver : ValueResolver<ILookup<string, DabTrial.Domain.Services.DataSummaryService.StageCount>, IList<IList<int>>>
    {
        protected override IList<IList<int>> ResolveCore(ILookup<string, DabTrial.Domain.Services.DataSummaryService.StageCount> stats)
        {
            Array vals = Enum.GetValues(typeof(DataStageUtilities.TrialStage));
            int minDataSage = (int)vals.GetValue(0);
            int len = vals.Length - minDataSage + 1;
            int[] colTotal = new int[len];
            var matrix = new int[stats.Count+1][];
            int i = 0;
            foreach (var g in stats)
            {
                int[] rowCounts = matrix[i] = new int[len];
                int rowTotal = 0;
                foreach (var r in g)
                {
                    int colIndx = (int)r.Stage - minDataSage;
                    rowCounts[colIndx] = r.Count;
                    rowTotal += r.Count;
                    colTotal[colIndx] += r.Count;
                    rowCounts[len - 1] = rowTotal;
                }
                i++;
            }
            colTotal[colTotal.Length - 1] = colTotal.Sum();
            matrix[matrix.Length-1] = colTotal;
            return matrix;
        }
    }

    internal class ParticipantToDataStageResolver : ValueResolver<TrialParticipant, DabTrial.Infrastructure.Utilities.DataStageUtilities.TrialStage>
    {
        Func<TrialParticipant, DataStageUtilities.TrialStage> _getStage = DataStageUtilities.StageExpression().Compile();
        protected override DabTrial.Infrastructure.Utilities.DataStageUtilities.TrialStage ResolveCore(TrialParticipant participant)
        {
            return _getStage(participant);
        }
    }
    internal class ParticipantToDosingModelResolver : ValueResolver<TrialParticipant, DosingModel>
    {
        protected override DosingModel ResolveCore(TrialParticipant participant)
        {
            if (!participant.IsInterventionArm) { return null; }
            return (GetDoses(participant.Weight, participant.StudyCentre.IsUsing1pcAdrenaline, participant.LocalTimeRandomised));
        }
        public static DosingModel GetDoses(double weightInKg, bool adrenalineIs1pc, DateTime enrollmentTime)
        {
            DateTime doseGiven = FirstDoseApproximation(enrollmentTime);
            var returnVar = new DosingModel
            {
                PredMg = weightInKg,
                MethyPredMg = weightInKg,
                DexamethasoneMg = 0.6 * weightInKg,
                AdrenalineMl = (adrenalineIs1pc ? 0.05 : 0.5) * weightInKg,
                AdrenalineIs1pc = adrenalineIs1pc,
                LastDoseAdrenaline = DateApproximationString(doseGiven.AddHours(72.0)),
                LastHighDoseSteroid = DateApproximationString(doseGiven.AddHours(72.0)),
                LastDoseSteroid = DateApproximationString(doseGiven.AddHours(144.0))
            };
            if (returnVar.AdrenalineMl > 6) { returnVar.AdrenalineMl = 6; }
            return returnVar;
        }
        private static DateTime FirstDoseApproximation(DateTime enrollmentTime)
        {
            int min = enrollmentTime.Minute;
            if (min > 40)
            {
                return new DateTime(enrollmentTime.Year, enrollmentTime.Month, enrollmentTime.Day, enrollmentTime.Hour + 1, 0, 0);
            }
            return new DateTime(enrollmentTime.Year, enrollmentTime.Month, enrollmentTime.Day, enrollmentTime.Hour, 30, 0);
        }
        private static string DateApproximationString(DateTime dateApprox)
        {
            switch (dateApprox.Minute)
            {
                case 0:
                    string hrDescr = HourDescription(dateApprox);
                    if (hrDescr == "midnight") { MidnightDescription(dateApprox); }
                    return string.Format("around {0} on {1:ddd} {1:d}", hrDescr, dateApprox);
                case 30:
                    DateTime endApprox = dateApprox.AddHours(1);
                    string startDescr = HourDescription(dateApprox);
                    string endDescribe = HourDescription(endApprox);
                    if (startDescr.Substring(startDescr.Length - 2) == endDescribe.Substring(endDescribe.Length - 2))
                    {
                        startDescr = startDescr.Substring(0, startDescr.Length - 2).TrimEnd();
                    }
                    else if (endDescribe == "midnight")
                    {
                        return string.Format("between {0} & {1}", startDescr, MidnightDescription(dateApprox));
                    }
                    return string.Format("between {0} & {1} on {2:ddd} {2:d}", startDescr, endDescribe, dateApprox);
                default:
                    throw new ArgumentException("The minute value for a supplied date must be either 0 or 30");
            }
        }
        private static string MidnightDescription(DateTime date)
        {
            return string.Format("midnight seperating {0:ddd} {1} & {2:ddd} {2:d}", date.AddDays(-1), date.Day.ToOrdinal(), date);
        }
        private static string HourDescription(DateTime time)
        {
            switch (time.Hour)
            {
                case 0:
                    return "midnight";
                case 12:
                    return "12 noon";
                default:
                    return time.ToString("h tt");
            }
        }
    }
}