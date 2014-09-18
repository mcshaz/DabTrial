using MvcHtmlHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DabTrial.Infrastructure.RazorUtilities
{
    public static class CurrentResources
    {
        public static void AssignAllResources(AssetsHelper assets)
        {
#if DEBUG
            assets.Libraries["jQuery"]
                .AddScript("~/Scripts/jquery-2.1.1.js", BrowserType.W3cCompliant)
                .AddScript("~/Scripts/jquery-1.11.1.js", BrowserType.IeLegacy);
 //               .AddScript("~/Scripts/jquery-migrate-1.2.1.js"); //generate console warnings - must remember to look
            assets.Libraries["jQueryUI"].Requires("jQuery")
                .AddScript("~/Scripts/jquery-ui-1.9.2.js",BrowserType.Ie6)
                .AddScript("~/Scripts/jquery-ui-1.11.1.js", ~BrowserType.Ie6)
                .AddStyle("//code.jquery.com/ui/1.9.2/themes/eggplant/jquery-ui.css", BrowserType.Ie6)
                .AddStyle("//code.jquery.com/ui/1.10.4/themes/eggplant/jquery-ui.css", ~BrowserType.Ie6);
            assets.Libraries["TimePicker"].Requires("jQueryUI","MyUtilityScripts")
                .AddScript("~/Scripts/jquery-ui-sliderAccess.js")
                .AddScript("~/Scripts/jquery-ui-timepicker-addon-1.4.6.js")
                .AddStyle("~/Content/jQueryUI/jquery-ui-timepicker-addon.css");
            assets.Libraries["Validation"].Requires("jQuery")
                .AddScript("~/Scripts/jquery.validate-1.13.0.js")
                .AddScript("~/Scripts/jquery.validate.unobtrusive.js")
                .AddScript("~/Scripts/mvcfoolproof.unobtrusive.js")
                .AddScript("~/Scripts/CustomClientValidation.js");
            assets.Libraries["MyUtilityScripts"].Requires("jQuery")
                .AddScript("~/Scripts/GeneralOnLoad.js").IsImplementingLibrary=true;
            assets.Libraries["FormTools"].Requires("Validation", "MyUtilityScripts");
            assets.Libraries["AjaxFormTools"].Requires("FormTools", "jQueryUI")
                .AddScript("~/Scripts/jquery.unobtrusive-ajax.js");
                //.AddScript("//ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.unobtrusive-ajax.js"); //not compliant with modern jQuery syntax
            assets.Libraries["DataTables"].Requires("jQuery", "MyUtilityScripts")
                .AddScript("~/Scripts/DataTables-1.9.4/media/js/jquery.dataTables.js")
                .AddStyle("~/Content/DataTables-1.9.4/media/css/jquery.dataTables.css")
                .AddStyle("~/Content/DataTables-1.9.4/media/css/jquery.dataTables_themeroller.css");
            assets.Libraries["MvcDataTables"].Requires("DataTables")
                .AddScript("~/Scripts/jquery.dataTables.columnFilter.js");
            assets.Libraries["DummyData"].Requires("jQuery")
                .AddScript("~/Scripts/DummyData.js")
                .AddStyle("~/Content/DummyData.css");
            assets.Libraries["Wt4Age"].Requires("Validation")
                .AddScript("~/Scripts/Wt4Age.js");
            assets.Libraries["DrugDosing"]
                .AddStyle("~/Content/DrugDosing-1.0.css");
#else
            assets.Libraries["jQuery"]
                .AddScript("//ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js",BrowserType.W3cCompliant)
                .AddScript("//ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js", BrowserType.IeLegacy);
            assets.Libraries["jQueryUI"].Requires("jQuery")
                .AddScript("//code.jquery.com/ui/1.9.2/jquery-ui.min.js", BrowserType.Ie6)
                .AddScript("//code.jquery.com/ui/1.11.1/jquery-ui.min.js", ~BrowserType.Ie6)//curently 1.11.1 but does not close timepicker on exit
                .AddStyle("//code.jquery.com/ui/1.9.2/themes/eggplant/jquery-ui.css", BrowserType.Ie6)
                .AddStyle("//code.jquery.com/ui/1.11.1/themes/eggplant/jquery-ui.css", ~BrowserType.Ie6);
            assets.Libraries["TimePicker"].Requires("jQueryUI", "MyUtilityScripts")
                .AddScript("~/Scripts/jquery-ui-sliderAccess.min.js")
                .AddScript("~/Scripts/jquery-ui-timepicker-addon-1.4.6.min.js")
                .AddStyle("~/Content/jQueryUI/jquery-ui-timepicker-addon.css");
            assets.Libraries["Validation"].Requires("jQuery")
                .AddScript("//ajax.aspnetcdn.com/ajax/jquery.validate/1.13.0/jquery.validate.min.js")
                .AddScript("//ajax.aspnetcdn.com/ajax/mvc/5.1/jquery.validate.unobtrusive.min.js")
                .AddScript("~/Scripts/mvcfoolproof.unobtrusive.min.js")
                .AddScript("~/Scripts/CustomClientValidation-1.1.1.min.js");
            assets.Libraries["MyUtilityScripts"].Requires("jQuery")
                .AddScript("~/Scripts/GeneralOnLoad-1.1.3.min.js").IsImplementingLibrary = true;
            assets.Libraries["FormTools"].Requires("Validation", "MyUtilityScripts");
            assets.Libraries["AjaxFormTools"].Requires("FormTools", "jQueryUI")
                .AddScript("~/Scripts/jquery.unobtrusive-ajax.min.js");
                //.AddScript("//ajax.aspnetcdn.com/ajax/mvc/3.0/jquery.unobtrusive-ajax.min.js"); -old version no support modern jquery
            assets.Libraries["DataTables"].Requires("jQuery", "MyUtilityScripts")
                .AddScript("//cdn.datatables.net/1.10.2/js/jquery.dataTables.min.js")
                .AddStyle("//cdn.datatables.net/1.10.2/css/jquery.dataTables.min.css");
                //.AddStyle("//ajax.aspnetcdn.com/ajax/jquery.dataTables/1.9.4/css/jquery.dataTables_themeroller.css");
            assets.Libraries["MvcDataTables"].Requires("DataTables")
                .AddScript("~/Scripts/jquery.dataTables.columnFilter-1.5.6.min.js");
            assets.Libraries["DummyData"].Requires("jQuery")
                .AddScript("~/Scripts/DummyData.js")
                .AddStyle("~/Content/DummyData.css");
            assets.Libraries["Wt4Age"].Requires("Validation")
                .AddScript("~/Scripts/Wt4Age-1.0.0.min.js");
            assets.Libraries["DrugDosing"]
                .AddStyle("~/Content/DrugDosing-1.0.css");

#endif
        }
    }
}