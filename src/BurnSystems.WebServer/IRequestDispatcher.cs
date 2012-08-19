using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Dispatcher;

namespace BurnSystems.WebServer
{
    /// <summary>
    /// Dispatches the request, if responsible
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Checks, if the dispatcher is responsible for the request
        /// </summary>
        /// <param name="context">Context of Http Request</param>
        /// <returns>true, if this dispatcher shall dispatch the request</returns>
        bool IsResponsible(IActivates container, ContextDispatchInformation information);

        /// <summary>
        /// Dispatches the request
        /// </summary>
        /// <param name="context">Context of the request</param>
        void Dispatch(IActivates container, ContextDispatchInformation information);
    }
}
