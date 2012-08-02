using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BurnSystems.WebServer.Responses
{
    public class PhysicalFileResponse : IRequestDispatcher
    {
        public string PhysicalPath
        {
            get;
            set;
        }

        public bool IsResponsible(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            return File.Exists(this.PhysicalPath);
        }

        public void Dispatch(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
