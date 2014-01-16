using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using DabTrial.Domain.Tables;

namespace DabTrial.Domain.Providers
{
    public interface IDataContext
    {
        int SaveChanges();
        int SaveChanges(String user);

        DbSet<StudyCentre> StudyCentres { get; set; }
        DbSet<TrialParticipant> TrialParticipants { get; set; }
        DbSet<RespiratorySupportType> RespiratorySupportTypes { get; set; }
        DbSet<RespiratorySupportChange> RespiratorySupportChanges { get; set; }
        DbSet<AdverseEvent> AdverseEvents { get; set; }
        DbSet<Drug> AdverseEventDrugs { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<AuditLogEntry> AuditLog { get; set; }
        DbSet<LocalRecordProvider> LocalRecordProviders { get; set; }
        DbSet<ScreenedPatient> ScreenedPatients { get; set; }
        DbSet<ParticipantWithdrawal> ParticipantWithdrawals { get; set; }
        DbSet<ParticipantDeath> ParticipantDeaths { get; set; }
        DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        DbSet<AdverseEventType> AdverseEventTypes { get; set; }
        DbSet<NoConsentAttempt> NoConsentAttempts { get; set; }
        DbSet<RegistrationFromIP> RegistrationsFromIP { get; set; }
        DbSet<DrugRoute> DrugRoutes { get; set; }
        void Dispose();

        IEnumerable<DbEntityValidationResult> GetValidationErrors();
    }
}
