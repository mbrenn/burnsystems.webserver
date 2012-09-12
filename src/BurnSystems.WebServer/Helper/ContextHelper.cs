using System;
using System.Globalization;
using System.Net;
using BurnSystems.WebServer.Dispatcher;

namespace BurnSystems.WebServer.Helper
{
    /// <summary>
    /// Some methods for context
    /// </summary>
    public static class ContextHelper
    {
        /// <summary>
        /// Disables the browser cache, so page will be 
        /// always refreshed
        /// </summary>
        public static void DisableBrowserCache(this HttpListenerContext context)
        {
            context.Response.Headers["Last-Modified"] = "no-cache";
            context.Response.Headers["Expires"] = "Mon, 26 Jul 1997 05:00:00 GMT";
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.AddHeader("Cache-Control", "post-check=0, pre-check=0");
        }

        public static bool CheckForCache(this ContextDispatchInformation info, DateTime fileDate)
        {
            var done = false;

            var isModifiedSince = info.Context.Request.Headers["If-Modified-Since"];
            if (isModifiedSince != null)
            {
                var cacheDate = DateTime.Parse(isModifiedSince);

                if ((fileDate - TimeSpan.FromSeconds(2)) < cacheDate)
                {
                    info.Context.Response.StatusCode = 304;
                    done = true; ;
                }
            }

            // Fügt den Header hinzu
            info.Context.Response.AddHeader(
                "Last-Modified",
                fileDate.ToString("r"));
            info.Context.Response.AddHeader(
                "ETag",
                StringManipulation.Sha1(fileDate.Ticks.ToString(CultureInfo.InvariantCulture)));
            return done;
        }
    }
}
