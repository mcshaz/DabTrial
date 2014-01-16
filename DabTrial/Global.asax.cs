using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Web;
using DabTrial.Models;
using System.Data.Entity;
using UkAdcHtmlAttributeProvider.Infrastructure;
using DabTrial.Infrastructure.Validation;

namespace DabTrial
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.IgnoreRoute("{*allaspx}", new {allaspx=@".*\.aspx(/.*)?"});
            routes.IgnoreRoute("{*robotstxt}", new {robotstxt=@"(.*/)?robots.txt(/.*)?"});
            routes.IgnoreRoute("{*favicon}", new {favicon=@"(.*/)?favicon.ico(/.*)?"});
            //routes.IgnoreRoute("MyLittleAdmin/{*pathInfo}");
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //Database.SetInitializer<DataContext>(new Migrations.InitialiseAndSeedTrialData());

            AutoMapperConfiguration.Configure();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            ModelMetadataProviders.Current = new ConventionProvider();

            //ModelBinders.Binders.Add(
            //ModelBinders.Binders.DefaultBinder = new PerpetuumSoft.Knockout.KnockoutModelBinder();
            if (!ModelBinders.Binders.ContainsKey(typeof(Mvc.JQuery.Datatables.DataTablesParam)))
            {
                ModelBinders.Binders.Add(typeof(Mvc.JQuery.Datatables.DataTablesParam), new Mvc.JQuery.Datatables.DataTablesModelBinder());
            }

            HtmlAttributeProvider.Register((metadata) =>
                {
                    switch (metadata.DataTypeName)
                    {
                        case "Date":
                            return new[] 
                            { 
                                new KeyValuePair<string, object>("class", "date"),
                                new KeyValuePair<string, object>("placeholder", "day/month/year")
                            };
                        case "DateTime":
                            return new[] 
                            { 
                                new KeyValuePair<string, object>("class", "dateTime"),
                                new KeyValuePair<string, object>("placeholder", "day/month/year 24Hr:min")
                            };
                        case "Time":
                            return new[] 
                            { 
                                new KeyValuePair<string, object>("class", "time"),
                                new KeyValuePair<string, object>("placeholder", "24Hr:min")
                            };
                        default:
                            return null;
                    };
                });

            HtmlAttributeProvider.Register((metadata) =>
            {
                if (metadata.AdditionalValues.ContainsKey(DateIntervalClientOnlyAttribute.DateIntervalValProperty))
                {
                    return metadata.AdditionalValues[DateIntervalClientOnlyAttribute.DateIntervalValProperty] as IEnumerable<KeyValuePair<string, object>>;
                }
                return null;
            });
            HtmlAttributeProvider.Register((metadata) =>
            {
                return MultipleValidationAttribute.GetAttributes(metadata);
            });

        }
    }

}