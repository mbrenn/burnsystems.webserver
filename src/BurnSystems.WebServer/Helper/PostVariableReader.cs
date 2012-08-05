using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Helper
{
    public class PostVariableReader
    {
        [Inject]
        public HttpListenerContext ListenerContext
        {
            get;
            set;

        }
    }
}
