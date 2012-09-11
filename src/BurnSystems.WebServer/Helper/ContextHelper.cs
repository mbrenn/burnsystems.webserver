using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

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
    }
}
