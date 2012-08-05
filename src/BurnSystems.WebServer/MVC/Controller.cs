using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;
using System.Net;

namespace BurnSystems.WebServer.MVC
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
        /// Returns an html result to browser
        /// </summary>
        /// <param name="result">Result to be returned</param>
        public void Html(string result)
        {
            var bytes = Encoding.UTF8.GetBytes(result);
            this.Context.Response.ContentEncoding = Encoding.UTF8;
            this.Context.Response.ContentType = "text/html";

            using (var stream = this.Context.Response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
