//-----------------------------------------------------------------------
// <copyright file="SessionContainer.cs" company="Martin Brenn">
//     Alle Rechte vorbehalten. 
// 
//     Die Inhalte dieser Datei sind ebenfalls automatisch unter 
//     der AGPL lizenziert. 
//     http://www.fsf.org/licensing/licenses/agpl-3.0.html
//     Weitere Informationen: http://de.wikipedia.org/wiki/AGPL
// </copyright>
//-----------------------------------------------------------------------

namespace BurnSystems.WebServer.Sessions
{
    using System;
    using System.Collections.Generic;
    using BurnSystems.Logging;

    /// <summary>
    /// Dieser Sessioncontainer speichert die Sessions im Groüen 
    /// und Ganzen und kümmert sich auch darum, dass veraltete Sessions
    /// nach einer bestimmten Zeitspanne entfernt werden. 
    /// </summary>
    [Serializable()]
    public class SessionContainer
    {
        /// <summary>
        /// Speichert das maximal erlaubte Alter von Sessions
        /// </summary>
        private TimeSpan maximumAge = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Wahrscheinlichkeit, ob beim Aufruf der Funktion 
        /// 'RemovePerhapsOldSessions' wirklich aufgerüumt wird. 
        /// </summary>
        private double collectorProbability = 0.01;

        /// <summary>
        /// Liste von Sessions
        /// </summary>
        private List<Session> sessions = new List<Session>();

        /// <summary>
        /// Gets or sets the maximum age of a session
        /// </summary>
        public TimeSpan MaximumAge
        {
            get { return this.maximumAge; }
            set { this.maximumAge = value; }
        }

        /// <summary>
        /// Gets a session with the given id. If session does not exist, null will be returned.
        /// </summary>
        /// <param name="sessionId">Id der angeforderten Session</param>
        /// <returns>Gefundene Session oder null, wenn diese nicht gefunden
        /// wird</returns>
        public Session this[string sessionId]
        {
            get
            {
                lock (this.sessions)
                {
                    var session = this.sessions.Find(
                        delegate(Session otherSession)
                        {
                            return otherSession.SessionId == sessionId;
                        });

                    if (session != null)
                    {
                        session.LastAccess = DateTime.Now;
                    }

                    return session;
                }
            }
        }

        /// <summary>
        /// Erstellt eine neue Session
        /// </summary>
        /// <returns>Recently created session with a unique sessionid</returns>
        public Session CreateNewSession()
        {
            lock (this.sessions)
            {
                var session = new Session();
                session.LastAccess = DateTime.Now;
                session.SessionId = StringManipulation.SecureRandomString(32);

                if (this.sessions.Find(x => x.SessionId == session.SessionId) == null)
                {
                    this.sessions.Add(session);
                    return session;
                }
                else
                {
                    // Es wird solang Sessions erzeugt bis eine wirklich
                    // neue SessionId gefunden wurde. 
                    // Die Wahrscheinlichkeit einer endlosen Rekursion
                    // geht auf 0
                    return this.CreateNewSession();
                }
            }
        }

        /// <summary>
        /// Entfernt alle alte Sessions aus dem Speicher.
        /// </summary>
        public void RemoveOldSessions()
        {
            lock (this.sessions)
            {
                DateTime now = DateTime.Now;

                this.sessions.RemoveAll(
                    x => now - x.LastAccess > this.maximumAge);
            }
        }

        /// <summary>
        /// If this function is called, the garbage collection will be started with
        /// a certain probability. This method should be called after each 
        /// webrequest, so the GC will be executed with the configured probability. 
        /// </summary>
        public void RemovePerhapsOldSessions()
        {
            if (MathHelper.Random.NextDouble() < this.collectorProbability)
            {
                Log.TheLog.LogEntry(
                       new LogEntry(Localization_WebServer.CollectingSessions, LogLevel.Verbose));
                this.RemoveOldSessions();
            }
        }
    }
}
