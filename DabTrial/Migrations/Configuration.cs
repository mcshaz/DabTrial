namespace BronchiolitisTrial.Migrations
{
    using BronchiolitisTrial.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    //using System.Data.Entity.Validation;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BronchiolitisTrial.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BronchiolitisTrial.DataContext context)
        {
            //if other properties must be unique, add:
            //needed in the bootstrap
            //Database.SetInitializer<MyContext>(new MyInitializer());
            /*
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_StudyCentre_Name ON StudyCentres (Name)");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_StudyCentre_SiteRegistrationPwd ON StudyCentres (SiteRegistrationPwd)");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_LocalRecordProvider_Name ON LocalRecordProviders (Name)");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_RespiratorySupportType_Description ON RespiratorySupportTypes (Description)");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_User_UserName ON Users (UserName)");
            context.Database.ExecuteSqlCommand("CREATE UNIQUE INDEX IX_Role_RoleName ON Roles (RoleName)");
             * */
            if (!context.Roles.Any())
            {
                context.Roles.Add(new Role() { RoleName = "PrincipleInvestigator", Description = "Principle Investigator" });
                context.Roles.Add(new Role() { RoleName = "SiteInvestigator", Description = "Study Site Investigator" });
                context.Roles.Add(new Role() { RoleName = "EnrollingClinician", Description = "Enroling Clinician" });

                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "None", RandomisationCategory = 1 });
                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "Simple oxygen therapy", Explanation = "Includes head box, nasal prongs or catheter", RandomisationCategory = 1 });
                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "High flow nasal prong oxygen", Explanation = "Includes any flows greater than 1 Litre/Kg/minute", RandomisationCategory = 1 });
                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "Non-invasive CPAP", Explanation = "Includes nasopharyngeal, nasal prong or mask CPAP", RandomisationCategory = 2 });
                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "Invasive respiratory support", Explanation = "Endotracheal tube with mechanical ventilation or CPAP", RandomisationCategory = 3 });
                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "HFOV", Explanation = "High frequency oscillation ventilation", RandomisationCategory = null });
                context.RespiratorySupportTypes.Add(new RespiratorySupportType() { Description = "ECMO", Explanation = "Extra-Corporeal Membrane Oxygenation", RandomisationCategory = null });

                var nhi = context.LocalRecordProviders.Add(new LocalRecordProvider() { HospitalNoRegEx = @"[A-Za-z]{3}[0-9]{3,4}", NotationDescription = "3 Letters followed by 3 to 4 numbers", Name = "NZ National Health Index (NHI)" });
                var rchMrn = context.LocalRecordProviders.Add(new LocalRecordProvider() { HospitalNoRegEx = @"\d{7,8}", NotationDescription = "An 8 digit number", Name = "Unique Record (UR) number" });

                var rch = context.StudyCentres.Add(new StudyCentre() { Name = "Royal Children's Hospital Melbourne", Abbreviation = "RCH", SiteRegistrationPwd = "P@ssword2", TimeZoneId = "AUS Eastern Standard Time", ValidEmailDomains = @"@rch.org.au", RecordSystem = rchMrn, PublicPhoneNumber = "(+61) 3 9345 5211" });
                var ss = context.StudyCentres.Add(new StudyCentre() { Name = "Starship Children's Hospital", Abbreviation = "Starship", SiteRegistrationPwd = "P@ssword1", TimeZoneId = "New Zealand Standard Time", ValidEmailDomains = @"@adhb.govt.nz,@auckland.ac.nz", RecordSystem = nhi, PublicPhoneNumber = "(+64) 9 307 4903" });
                context.StudyCentres.Add(new StudyCentre() { Name = "Middlemore Hospital", Abbreviation = "MMH", SiteRegistrationPwd = "P@ssword3", TimeZoneId = "New Zealand Standard Time", ValidEmailDomains = @"@cmdhb.govt.nz", RecordSystem = nhi, PublicPhoneNumber = "(+64) 9 276 0000" });

                string pwd = "AHUdWQbWUVMCf1sR4jJEdHuvyPz0ZO+WSMNcJpohPjs15gO938sZ8J9XhbOz08H8Dg=="; //Password = P@ssword1

                context.SaveChanges();

                context.Users.Add(new User()
                {
                    Roles = context.Roles.Where(r => r.RoleName == "PrincipleInvestigator").ToList(),
                    UserName = "Beng",
                    Email = "Ben.Gelbart@rch.org.au",
                    IsApproved = true,
                    Password = pwd,
                    FirstName = "Ben",
                    LastName = "Gelbart",
                    StudyCentre = rch,
                    PasswordFailuresSinceLastSuccess = 0,
                    ProfessionalRoleID = 4,
                    CreateDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    IsPublicContact = true
                });
                context.Users.Add(new User()
                {
                    Roles = context.Roles.Where(r => r.RoleName == "PrincipleInvestigator").ToList(),
                    UserName = "Brentm",
                    Email = "brentm@adhb.govt.nz",
                    IsApproved = true,
                    Password = pwd,
                    FirstName = "Brent",
                    LastName = "McSharry",
                    StudyCentre = ss,
                    PasswordFailuresSinceLastSuccess = 0,
                    ProfessionalRoleID = 4,
                    CreateDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    IsPublicContact = true
                });
                context.Users.Add(new User()
                {
                    Roles = context.Roles.Where(r => r.RoleName == "SiteInvestigator").ToList(),
                    UserName = "ClaireS",
                    Email = "claire@adhb.govt.nz",
                    IsApproved = true,
                    Password = pwd, //Password = P@ssword1
                    FirstName = "Claire",
                    LastName = "Sherring",
                    StudyCentre = ss,
                    PasswordFailuresSinceLastSuccess = 0,
                    ProfessionalRoleID = 0,
                    CreateDate = DateTime.Now,
                    LastActivityDate = DateTime.Now,
                    LastLoginDate = DateTime.Now,
                    IsPublicContact = true
                });
                context.SaveChanges();
            }
            if (!context.AdverseEventTypes.Any())
            {
                context.AdverseEventTypes.Add(new AdverseEventType { Description = "arrhythmia" });
                context.AdverseEventTypes.Add(new AdverseEventType { Description = "hypertension" });
                context.AdverseEventTypes.Add(new AdverseEventType { Description = "tremor" });
                context.AdverseEventTypes.Add(new AdverseEventType { Description = "other" });
                context.SaveChanges();
            }
        }
    }
}
