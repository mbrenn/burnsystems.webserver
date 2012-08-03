using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.Dispatcher;
using System.Net;

namespace BurnSystems.WebServer.Responses
{
    public class StaticContentResponse : BaseDispatcher
    {
        public string ContentType
        {
            get;
            set;
        }

        public byte[] Content
        {
            get;
            set;
        }

        public StaticContentResponse(Func<HttpListenerContext, bool> filter)
            : base(filter)
        {
        }

        public StaticContentResponse(Func<HttpListenerContext, bool> filter, string contentType, byte[] content)
            : base(filter)
        {
            this.ContentType = contentType;
            this.Content = content;
        }

        public override void Dispatch(ObjectActivation.IActivates activates, System.Net.HttpListenerContext context)
        {
            context.Response.ContentLength64 = this.Content.Length;
            context.Response.ContentType = this.ContentType;

            using (var responseStream = context.Response.OutputStream)
            {
                responseStream.Write(this.Content, 0, this.Content.Length);
            }
        }
    }
}
