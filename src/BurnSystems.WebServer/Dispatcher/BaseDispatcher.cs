using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.Test;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Dispatcher
{
    /// <summary>
    /// The base dispatcher, offering filter possibility for responsibility check
    /// </summary>
    public abstract class BaseDispatcher: IRequestDispatcher
    {
        /// <summary>
        /// Stores the filter
        /// </summary>
        private Func<HttpListenerContext, bool> filter;

        public BaseDispatcher(Func<HttpListenerContext, bool> filter)
        {
            Ensure.That(filter != null);
            this.filter = filter;
        }

        public bool IsResponsible(IActivates container, HttpListenerContext context)
        {
            return this.filter(context);
        }

        public abstract void Dispatch(IActivates container, HttpListenerContext context);
    }
}
