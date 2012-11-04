using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Resources
{
    public static class Files
    {
        public static string JQuery
        {
            get {
#if DEBUG
                return Localization_WebServer.JQueryDebug; 
#else
                return Localization_WebServer.JQuery; 
#endif
            }
        }

        public static string JQueryCookie
        {
            get
            {
                return Localization_WebServer.JQueryCookie;
            }
        }

        public static string JQueryHotkey
        {
            get
            {
                return Localization_WebServer.JQueryHotkeys;
            }
        }

        public static string Require
        {
            get
            {

#if DEBUG
                return Localization_WebServer.RequireJSDebug;
#else
                return Localization_WebServer.RequireJS; 
#endif
            }
        }

        public static string DateFormat
        {
            get
            {
#if DEBUG
                return Localization_WebServer.DateFormatDebug;
#else
                return Localization_WebServer.DateFormat; 
#endif
            }
        }
    }
}
