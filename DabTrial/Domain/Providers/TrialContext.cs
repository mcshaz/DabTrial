using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
using System.Text.RegularExpressions;
using System.ComponentModel.DataAnnotations.Schema;
using DabTrial.Domain.Tables;

namespace DabTrial.Domain.Providers
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() : base("DataContext") { }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdverseEvent>().Map(m =>m.MapInheritedProperties());
            modelBuilder.Entity<ProtocolViolation>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<ParticipantDeath>().Map(m => m.MapInheritedProperties());
            modelBuilder.Entity<ParticipantWithdrawal>().Map(m => m.MapInheritedProperties());

            modelBuilder.Entity<RespiratorySupportType>()
                .HasMany(t => t.RespiratorySupportChanges)
                .WithRequired(c => c.RespiratorySupportType)
                .HasForeignKey(c => c.RespiratorySupportTypeId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TrialParticipant>().HasOptional(p=>p.Death)
                .WithRequired()
                .WillCascadeOnDelete();

            modelBuilder.Entity<TrialParticipant>().HasOptional(p => p.Withdrawal)
                .WithRequired()
                .WillCascadeOnDelete();
                        
            //modelBuilder.Entity<ParticipantAllocation>().HasRequired(p => p.TrialParticipant).WithRequiredDependent();
            //for 1:1 on primary key
        }
        
        public DbSet<StudyCentre> StudyCentres { get; set; }
        public DbSet<TrialParticipant> TrialParticipants { get; set; }
        //public DbSet<ParticipantAllocation> ParticipantAllocations { get; set; }
        //currently merged 1:1 table may be appropriate for blinded study
        public DbSet<RespiratorySupportType> RespiratorySupportTypes { get; set; }
        public DbSet<RespiratorySupportChange> RespiratorySupportChanges { get; set; }
        public DbSet<AdverseEvent> AdverseEvents { get; set; }
        public DbSet<Drug> AdverseEventDrugs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AuditLogEntry> AuditLog { get; set; }
        public DbSet<LocalRecordProvider> LocalRecordProviders { get; set; }
        public DbSet<ScreenedPatient> ScreenedPatients { get; set; }
        public DbSet<ParticipantWithdrawal> ParticipantWithdrawals { get; set; }
        public DbSet<ParticipantDeath> ParticipantDeaths { get; set; }
        public DbSet<ProtocolViolation> ProtocolViolations { get; set; }
        public DbSet<AdverseEventType> AdverseEventTypes { get; set; }
        public DbSet<NoConsentAttempt> NoConsentAttempts { get; set; }
        public DbSet<RegistrationFromIP> RegistrationsFromIP { get; set; }
        public DbSet<DrugRoute> DrugRoutes { get; set; }
        
        //public DbSet<WithdrawalReason> WithdrawalReasons { get; set; }

        //http://jmdority.wordpress.com/2011/07/20/using-entity-framework-4-1-dbcontext-change-tracking-for-audit-logging/
        // This is overridden to prevent someone from calling SaveChanges without specifying the user making the change
        //public override int SaveChanges()
        //{
            //throw new InvalidOperationException("User Id must be provided");
        //    return SaveChanges(WebSecurity.User.Identity.Name);
        //}
        public int SaveChanges(String userId)
        {
#if DEBUG
            try
            {
                CheckValidation();
            }
            catch(DbEntityValidationException)
            {
                throw;
            }
#endif
            foreach (var ent in this.ChangeTracker.Entries())
            {
                if (ent.State == System.Data.Entity.EntityState.Added || ent.State == System.Data.Entity.EntityState.Deleted || ent.State == System.Data.Entity.EntityState.Modified)
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (AuditLogEntry x in GetAuditRecordsForChange(ent, userId))
                    {
                        this.AuditLog.Add(x);
                    }
                }
            }
#if DEBUG
            CheckValidation("AuditRecords");
#endif
            return base.SaveChanges();

        }
        private void CheckValidation(string details="")
        {
            var valCollection = this.GetValidationErrors();
            if (!valCollection.Any()) { return; }
            var valInfo = new System.Text.StringBuilder("The following entities did not meet following validation requirements: " + details  + Environment.NewLine);
            foreach (var valResults in valCollection)
            {
                valInfo.AppendFormat("Class -> {0}{1}", valResults.Entry.Entity.GetType().FullName, Environment.NewLine);
                foreach (var valErr in valResults.ValidationErrors)
                {
                            valInfo.AppendFormat("----Property: {0} Error: {1}{2}",
                            valErr.PropertyName,
                            valErr.ErrorMessage,
                            Environment.NewLine);
                }
                valInfo.AppendLine("------------------------------");
            }
            throw new DbEntityValidationException(valInfo.ToString());
        }
        protected readonly Regex guidTail = new Regex(@"_[0-9A-F]{64}$");
        private List<AuditLogEntry> GetAuditRecordsForChange(DbEntityEntry dbEntry, String user)
        {
            List<AuditLogEntry> result = new List<AuditLogEntry>();

            DateTime changeTime = DateTime.UtcNow;

            // Get the Table() attribute, if one exists
            TableAttribute tableAttr = dbEntry.Entity.GetType().GetCustomAttributes(typeof(TableAttribute), false).SingleOrDefault() as TableAttribute;

            // Get table name (if it has a Table attribute, use that, otherwise get the pluralized name)
            string tableName = tableAttr != null ? tableAttr.Name : dbEntry.Entity.GetType().Name;
            //remove guid if included in table name
            tableName = guidTail.Replace(tableName, "");

            // Get primary key value (If you have more than one key column, this will need to be adjusted)
            //string keyName = dbEntry.Entity.GetType().GetProperties().First(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).Name;

            var entityKeyValues = ((IObjectContextAdapter)this)
                                    .ObjectContext
                                    .ObjectStateManager
                                    .GetObjectStateEntry(dbEntry.Entity)
                                    .EntityKey
                                    .EntityKeyValues;
            string keyValue;
            if (entityKeyValues == null) //will be null if is identity column (autoincrement)
            {
                keyValue = "auto-assign";
            }
            else
            {
                keyValue = entityKeyValues[0].Value.ToString();
                // could try aggregate to get all key values
            }

            if (dbEntry.State == System.Data.Entity.EntityState.Added)
            {
                // For Inserts, just add the whole record
                // If the entity implements IDescribableEntity, use the description from Describe(), otherwise use ToString()
                result.Add(new AuditLogEntry()
                        {
                            UserName = user,
                            EventDateUTC = changeTime,
                            EventType = "C", // Created
                            TableName = tableName,
                            RecordId = keyValue,  // Again, adjust this if you have a multi-column key
                            ColumnName = "*ALL",    // Or make it nullable, whatever you want
                            NewValue = (dbEntry.CurrentValues.ToObject() is IDescribableEntity) ? (dbEntry.CurrentValues.ToObject() as IDescribableEntity).Describe() : dbEntry.CurrentValues.ToObject().ToString()
                        }
                    );
            }
            else if(dbEntry.State == System.Data.Entity.EntityState.Deleted)
            {
                // Same with deletes, do the whole record, and use either the description from Describe() or ToString()
                result.Add(new AuditLogEntry()
                        {
                            UserName = user,
                            EventDateUTC = changeTime,
                            EventType = "D", // Deleted
                            TableName = tableName,
                            RecordId = keyValue,
                            ColumnName = "*ALL",
                            OriginalValue = (dbEntry.OriginalValues.ToObject() is IDescribableEntity) ? (dbEntry.OriginalValues.ToObject() as IDescribableEntity).Describe() : dbEntry.OriginalValues.ToObject().ToString()
                        }
                    );
            }
            else if (dbEntry.State == System.Data.Entity.EntityState.Modified)
            {
                foreach (string propertyName in dbEntry.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntry.OriginalValues.GetValue<object>(propertyName), dbEntry.CurrentValues.GetValue<object>(propertyName)))
                    {
                        result.Add(new AuditLogEntry()
                                {
                                    UserName = user,
                                    EventDateUTC = changeTime,
                                    EventType = "M",    // Modified
                                    TableName = tableName,
                                    RecordId = keyValue,
                                    ColumnName = propertyName,
                                    OriginalValue = dbEntry.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntry.OriginalValues.GetValue<object>(propertyName).ToString(),
                                    NewValue = dbEntry.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntry.CurrentValues.GetValue<object>(propertyName).ToString()
                                }
                            );
                    }
                }
            }
            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities
            return result;
        }
    }
}