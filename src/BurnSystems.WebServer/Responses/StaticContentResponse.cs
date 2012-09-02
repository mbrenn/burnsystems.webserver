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

        /// <summary>
        /// Gets or sets the content encoding
        /// </summary>
        public Encoding ContentEncoding
        {
            get;
            set;
        }

        public StaticContentResponse(Func<ContextDispatchInformation, bool> filter)
            : base(filter)
        {
        }

        public StaticContentResponse(Func<ContextDispatchInformation, bool> filter, string contentType, byte[] content)
            : base(filter)
        {
            this.ContentType = contentType;
            this.Content = content;
        }

        public StaticContentResponse(Func<ContextDispatchInformation, bool> filter, string contentType, string content)
            : base(filter)
        {
            this.ContentType = contentType;
            this.Content = Encoding.UTF8.GetBytes(content);
            this.ContentEncoding = Encoding.UTF8;
        }

        public override void Dispatch(ObjectActivation.IActivates activates, ContextDispatchInformation info)
        {
            info.Context.Response.ContentLength64 = this.Content.Length;
            info.Context.Response.ContentType = this.ContentType;

            if (this.ContentEncoding != null)
            {
                info.Context.Response.ContentEncoding = this.ContentEncoding;
            }

            using (var responseStream = info.Context.Response.OutputStream)
            {
                responseStream.Write(this.Content, 0, this.Content.Length);
            }
        }
    }
}
