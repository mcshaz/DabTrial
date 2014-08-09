using DabTrial.Domain.Tables;
using DabTrial.Models;

namespace DabTrial.Infrastructure.RazorUtilities
{
    public static class NavigationItems
    {
        public static void AssignMenu(RoleMenu menu)
        {
            string PIorSI = RoleExtensions.PrincipleInvestigator + ',' + RoleExtensions.SiteInvestigator;
            menu.Add(new RoleMenuItem("Home", "Index", "Home", "All"))
                .Add(new RoleMenuItem("About", "About", "Home", "All"))
                .Add(new RoleMenuItem("Contact Us", "Index", "ContactUs", "All"))
                .Add(new RoleMenuItem("Enrol", "Enrol", "Participant", "Authenticated"))
                .Add(new RoleMenuItem("Participants", "Index", "Participant", "Authenticated"))
                .Add(new RoleMenuItem("Site Stats", "CentreStatistics", "DataSummary", PIorSI))
                .Add(new RoleMenuItem("Adverse", "Index", "AdverseEvent", PIorSI))
                .Add(new RoleMenuItem("Violations", "Index", "ProtocolViolation", PIorSI))
                .Add(new RoleMenuItem("Screening", "CreateEdit", "ScreeningLog", "Authenticated"))
                .Add(new RoleMenuItem("Manage Users", "Index", "InvestigatorAccount", PIorSI))
                .Add(new RoleMenuItem("View log", "Index", "AuditLog", RoleExtensions.PrincipleInvestigator + ',' +RoleExtensions.DbAdministrator))
                .Add(new RoleMenuItem("Sites", "Index", "StudyCentre", PIorSI))
                .Add(new RoleMenuItem("Data Required", "AllCentreDataStage", "DataSummary", RoleExtensions.PrincipleInvestigator))
                .Add(new RoleMenuItem("Missing Variables", "Index", "DataSummary", PIorSI))
                .Add(new RoleMenuItem("No Consent Reasons", "Index", "NoConsentAttempt", RoleExtensions.PrincipleInvestigator))
                .Add(new RoleMenuItem("Manage Files", "Index", "ManageFiles", RoleExtensions.PrincipleInvestigator))
                .Add(new RoleMenuItem("My Account", "Details", "Account", "Authenticated"))
                .Add(new RoleMenuItem("Backup", "Index", "Db", RoleExtensions.DbAdministrator));
        }
    }
}