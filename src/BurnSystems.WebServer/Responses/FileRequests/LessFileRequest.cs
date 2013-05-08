using BurnSystems.WebServer.Dispatcher;
using dotless.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Responses.FileRequests
{
    class LessFileRequest : BaseDispatcher, IFileRequestDispatcher
    {
        public string PhysicalPath
        {
            get;
            set;
        }

        public string VirtualPath
        {
            get;
            set;
        }

        public LessFileRequest()
            : base(DispatchFilter.All)
        {
        }

        /// <summary>
        /// Dispatches the object
        /// </summary>
        /// <param name="container">Container for activations</param>
        /// <param name="info">Context being used</param>
        public override void Dispatch(ObjectActivation.IActivates container, ContextDispatchInformation info)
        {
            var lessFile = File.ReadAllText(this.PhysicalPath);
            var cssFile = Less.Parse(lessFile);

            var bytes = Encoding.UTF8.GetBytes(cssFile);
            info.Context.Response.ContentType = "text/css; charset=utf-8";

            using ( var stream = info.Context.Response.OutputStream)
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
