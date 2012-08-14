using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.Dispatcher;
using System.IO;

namespace BurnSystems.WebServer.Responses.FileRequests
{
    public class BspxFileRequest :BaseDispatcher, IFileRequestDispatcher
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

        public BspxFileRequest()
            : base(DispatchFilter.All)
        {
        }

        /// <summary>
        /// Dispatches the object
        /// </summary>
        /// <param name="container">Container for activations</param>
        /// <param name="context">Context being used</param>
        public override void Dispatch(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            var viewModel = File.ReadAllText(this.PhysicalPath);

            var result = Encoding.UTF8.GetBytes(viewModel);
            using (var stream = context.Response.OutputStream)
            {
                stream.Write(result, 0, result.Count());
            }
        }
    }
}
