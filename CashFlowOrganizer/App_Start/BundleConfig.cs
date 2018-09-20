using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace CashFlowOrganizer
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {   
            bundles.Add(new ScriptBundle("~/bundles/EveryPageScript").Include(
                        "~/Scripts/Spin.js",
                        "~/Scripts/AllPages.js",
                        "~/Scripts/jquery-1.12.4.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/jquery-ui-1.12.1.js",
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/modernizr-2.6.2.js",
                        "~/Scripts/bootstrap-confirmation.js",                      
                        "~/Scripts/jquery.validate.js"));            

            bundles.Add(new StyleBundle("~/Content/EveryPageContent").Include(
              "~/Content/font-awesome.css",
              "~/Content/bootstrap.css",
              "~/Content/bootstrap-theme.css",
              "~/Content/Site.css"));            
        }
    }
}