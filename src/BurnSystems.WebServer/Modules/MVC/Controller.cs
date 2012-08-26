using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;
using System.Net;
using System.Web.Script.Serialization;

namespace BurnSystems.WebServer.Modules.MVC
{
    /// <summary>
    /// Implements the base class for all controller classes
    /// </summary>
    public class Controller
    {
        public IActivates Container
        {
            get;
            internal set;
        }

        public HttpListenerContext Context
        {
            get;
            internal set;
        }

        /// <summary>
        /// Indicates whether an additional sending is allowed
        /// </summary>
        private bool finishedSending = false;

        /// <summary>
        /// Returns an html result to browser
        /// </summary>
        /// <param name="result">Result to be returned</param>
        public void Html(string result)
        {
            this.CheckForSending();

            var bytes = Encoding.UTF8.GetBytes(result);
            this.Context.Response.ContentEncoding = Encoding.UTF8;
            this.Context.Response.ContentType = "text/html";
            this.Context.Response.ContentLength64 = bytes.LongLength;

            using (var stream = this.Context.Response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.finishedSending = true;
        }

        /// <summary>
        /// Returns an html result to browser
        /// </summary>
        /// <param name="result">Result to be returned</param>
        public void Json(object result)
        {
            this.CheckForSending();

            var serializer = new JavaScriptSerializer();
            
            var bytes = Encoding.UTF8.GetBytes(serializer.Serialize(result));
            this.Context.Response.ContentEncoding = Encoding.UTF8;
            this.Context.Response.ContentType = "application/json";
            this.Context.Response.ContentLength64 = bytes.LongLength;

            using (var stream = this.Context.Response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }

            this.finishedSending = true;
        }

        /// <summary>
        /// Checks, whether an additional sending is allowed
        /// </summary>
        private void CheckForSending()
        {
            if (this.finishedSending)
            {
                throw new InvalidOperationException(
                    Localization_WebServer.FinishedSending);
            }
        }
    }
}
