using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace BurnSystems.WebServer.Dispatcher
{
    /// <summary>
    /// Offers some default filters
    /// </summary>
    public static class DispatchFilter
    {
        public static Func<HttpListenerContext, bool> All
        {
            get { return (x) => true; }
        }

        public static Func<HttpListenerContext, bool> None
        {
            get { return (x) => false; }
        }

        /// <summary>
        /// Checks, if the host matches (case-insensitive)
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Func<HttpListenerContext, bool> ByHost(string host)
        {
            return (x) =>
               x.Request.Url.Host.ToLower() == host.ToLower();
        }

        /// <summary>
        /// Checks, if the host matches (case-insensitive)
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static Func<HttpListenerContext, bool> ByUrl(string url)
        {
            return (x) =>
               x.Request.Url.ToString().ToLower().StartsWith(url);
        }

        public static Func<HttpListenerContext, bool> And(Func<HttpListenerContext, bool> f1, Func<HttpListenerContext, bool> f2)
        {
            return (x) => f1(x) && f2(x);
        }

        public static Func<HttpListenerContext, bool> Or(Func<HttpListenerContext, bool> f1, Func<HttpListenerContext, bool> f2)
        {
            return (x) => f1(x) && f2(x);
        }
    }
}
