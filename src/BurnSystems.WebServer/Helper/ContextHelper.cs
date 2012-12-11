using System;
using System.Globalization;
using System.Net;
using BurnSystems.WebServer.Dispatcher;
using BurnSystems.Logging;

namespace BurnSystems.WebServer.Helper
{
    /// <summary>
    /// Some methods for context
    /// </summary>
    public static class ContextHelper
    {
        /// <summary>
        /// Logging for this class
        /// </summary>
        private static ClassLogger logger = new ClassLogger(typeof(ContextHelper));

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

        public static bool CheckForCache(this ContextDispatchInformation info, DateTime localModificationDate, byte[] content)
        {
            try
            {
                var done = false;
                localModificationDate = localModificationDate.ToUniversalTime();

                var isModifiedSince = info.Context.Request.Headers["If-Modified-Since"];
                if (isModifiedSince != null)
                {
                    var cacheDate = DateTime.Parse(isModifiedSince).ToUniversalTime();

                    if ((localModificationDate - TimeSpan.FromSeconds(2)) < cacheDate)
                    {
                        info.Context.Response.StatusCode = 304;
                        done = true;
                    }
                }

                // Fügt den Header hinzu
                info.Context.Response.AddHeader(
                    "Last-Modified",
                    localModificationDate.ToString("r"));
                info.Context.Response.AddHeader(
                    "ETag",
                    string.Format(
                        "\"{0}\"",
                        StringManipulation.Sha1(content)));
                info.Context.Response.AddHeader(
                    "Date",
                    DateTime.Now.ToUniversalTime().ToString("r"));
                info.Context.Response.AddHeader(
                    "Cache-Control",
                    "max-age=0");
                return done;
            }
            catch (Exception)
            {
                logger.LogEntry(LogLevel.Message, "Error during test of cache: If-Modified-Since: " + info.Context.Request.Headers["If-Modified-Since"]);
                return false;
            }
        }
    }
}
