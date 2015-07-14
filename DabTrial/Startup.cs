using DabTrial.Domain.Tables;
using DabTrial.Infrastructure.Utilities;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using System.Collections.Generic;

[assembly: OwinStartup(typeof(DabTrial.Startup))]
namespace DabTrial
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                // Use connection string name defined in `web.config` or `app.config`
                .UseSqlServerStorage("DataContext");
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                AuthorizationFilters = new[] { new MyRestrictiveAuthorizationFilter() }
            });
            RecurringJob.AddOrUpdate<DabTrial.Domain.Services.CreateEmailService>("EmailDataStatusUpdate", c=>c.EmailInvestigatorsReMissingData(), Cron.Monthly(15));
            //GlobalJobFilters.Filters.Add(new LogFailureAttribute());
            app.UseHangfireServer();
        }

        public class MyRestrictiveAuthorizationFilter : IAuthorizationFilter
        {
            public bool Authorize(IDictionary<string, object> owinEnvironment)
            {
                // In case you need an OWIN context, use the next line,
                // `OwinContext` class is the part of the `Microsoft.Owin` package.
                var context = new OwinContext(owinEnvironment);

                // Allow all authenticated users to see the Dashboard (potentially dangerous).
                return context.Authentication.User.IsInRole(RoleExtensions.DbAdministrator);
            }
        }
    }
}