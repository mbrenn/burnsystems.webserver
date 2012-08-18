using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Sessions
{
    /// <summary>
    /// Implements the session interface
    /// </summary>
    public class SessionInterface : ISessionInterface
    {
        /// <summary>
        /// Stores the http Listener Interface
        /// </summary>
        private HttpListenerContext context;

        /// <summary>
        /// Stores the sessioncontainer
        /// </summary>
        [Inject]
        public SessionContainer sessionContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the SessionInterface class.
        /// </summary>
        /// <param name="context">Http Context to be used</param>
        [Inject]        
        public SessionInterface(HttpListenerContext context)
        {
            this.context = context;
        }


        /// <summary>
        /// Checks, if a session exists with the name in cookie "SessionId". 
        /// If this is not the case, a new session will be created, otherwise
        /// the old session will be restored.
        /// </summary>
        public Session GetSession()
        {            
            var cookie = this.context.Request.Cookies["SessionId"];
            var sessionId = string.Empty;
            var isCookieFresh = false;

            if (cookie == null)
            {
                isCookieFresh = true;
            }
            else
            {
                sessionId = cookie.Value;
            }

            var session = this.sessionContainer[sessionId];

            if (session == null)
            {
                session = this.sessionContainer.CreateNewSession();
                sessionId = session.SessionId;
                this.context.Response.Cookies.Add(
                    new Cookie("SessionId", sessionId, "/"));
                session.IsSessionFresh = true;
            }
            else
            {
                session.IsSessionFresh = false;
            }

            session.IsCookieFresh = isCookieFresh;

            // Räumt vielleicht die Sessions auf
            this.sessionContainer.RemovePerhapsOldSessions();
            
            return session;
        }
    }
}
