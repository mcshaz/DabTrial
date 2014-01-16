using AutoMapper;

namespace DabTrial.Models
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x => { x.AddProfile<UserMapProfile>();
                                     x.AddProfile<ParticipantMapProfile>();
                                     x.AddProfile<StudyCentreMapProfile>();
                                     x.AddProfile<HospitalMrnProfile>();
                                     x.AddProfile<AdverseEventProfile>();
                                     x.AddProfile<ProtocolViolationProfile>();
                                     x.AddProfile<ScreeningLogProfile>();
                                     x.AddProfile<RespSupportChangeMapProfile>();
                                     x.AddProfile<SimpleTypeProfiles>();
                                     x.AddProfile<AuditLogProfile>();
            });
#if DEBUG
            Mapper.AssertConfigurationIsValid();
#endif
        }
    }
}