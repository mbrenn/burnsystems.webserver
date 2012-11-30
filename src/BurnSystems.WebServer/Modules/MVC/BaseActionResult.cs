using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BurnSystems.WebServer.Modules.MVC
{
    /// <summary>
    /// Defines some basic methods which are used by the specific Action Result instances
    /// </summary>
    public class BaseActionResult
    {
        /// <summary>
        /// Gets or sets a value indicating whether the sending already has been finished
        /// </summary>
        public bool HasFinishedSending
        {
            get;
            set;
        }

        /// <summary>
        /// Sends the result to webserver
        /// </summary>
        /// <param name="result">Result to be sent</param>
        protected void SendResult(HttpListenerContext listenerContext, string result)
        {
            this.CheckForSending();

            var bytes = Encoding.UTF8.GetBytes(result);
            listenerContext.Response.ContentEncoding = Encoding.UTF8;
            listenerContext.Response.ContentLength64 = bytes.LongLength;

            using (var stream = listenerContext.Response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.HasFinishedSending = true;
        }

        /// <summary>
        /// Checks, whether an additional sending is allowed
        /// </summary>
        private void CheckForSending()
        {
            if (this.HasFinishedSending)
            {
                throw new InvalidOperationException(
                    Localization_WebServer.FinishedSending);
            }
        }
    }
}
