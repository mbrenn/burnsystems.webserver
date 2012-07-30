using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.WebServer.Parser;
using BurnSystems.ObjectActivation;
using BurnSystems.Test;

namespace BurnSystems.WebServer.Responses
{
    public class ErrorResponse
    {
        [ByName(Server.TemplateParserBindingName)]
        public TemplateParser TemplateParser
        {
            get;
            set;
        }

        /// <summary>
        /// Gives response to listener context
        /// </summary>
        /// <param name="context">Context to be used</param>
        public void Respond(int code, string message, HttpListenerContext context)
        {
            Ensure.That(this.TemplateParser != null, "this.TemplateParser == null");

            var content = Localization_WebServer.Error;

            var model = new
            {
                Title = message,
                Message = context.Request.Url.ToString(),
                Code = code.ToString()
            };

            var template = this.TemplateParser.Parse(content, model, null, this.GetType().ToString());

            context.Response.StatusCode = 404;
            using (var response = context.Response.OutputStream)
            {
                var bytes = Encoding.UTF8.GetBytes(template);
                response.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
