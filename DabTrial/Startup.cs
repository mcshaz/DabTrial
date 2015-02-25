using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Utilities;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DabTrial.Startup))]
namespace DabTrial
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseHangfire(config =>
            {
                config.UseSqlServerStorage("DataContext");
                config.UseServer();
                config.UseAuthorizationFilters(new AuthorizationFilter
                {
                    Roles = RoleExtensions.PrincipleInvestigator + ',' + RoleExtensions.DbAdministrator // allow only specified roles
                });
            });
            RecurringJob.AddOrUpdate<DabTrial.Domain.Services.CreateEmailService>("EmailDataStatusUpdate", c=>c.EmailInvestigatorsReMissingData(), Cron.Monthly(15));
            //GlobalJobFilters.Filters.Add(new LogFailureAttribute());
        }
    }
}