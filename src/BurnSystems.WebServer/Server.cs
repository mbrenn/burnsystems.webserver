using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer
{
    /// <summary>
    /// Server responsible to start up server, close it and offer the dependency framework
    /// </summary>
    public class Server
    {
        /// <summary>
        /// Stores list of prefixes
        /// </summary>
        private List<string> prefixes = new List<string>();

        /// <summary>
        /// Stores value whether webserver is running
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Stores the listener
        /// </summary>
        private Listener listener;

        /// <summary>
        /// Adds a certain prefix
        /// </summary>
        /// <param name="prefix">Prefix to be added</param>
        public void AddPrefix(string prefix)
        {
            if (this.isRunning)
            {
                throw new InvalidOperationException(Localization_WebServer.ServerAlreadyStarted);
            }

            this.prefixes.Add(prefix);
        }

        /// <summary>
        /// Starts the webserver
        /// </summary>
        public void Start()
        {
            if (this.isRunning)
            {
                throw new InvalidOperationException(Localization_WebServer.ServerAlreadyStarted);
            }

            this.listener = new Listener(this.prefixes);
            this.listener.StartListening();
            this.isRunning = true;
        }

        /// <summary>
        /// Stops the webserver
        /// </summary>
        public void Stop()
        {
            if (!this.isRunning)
            {
                throw new InvalidOperationException(Localization_WebServer.ServerAlreadyStarted);
            }

            this.listener.StopListening();
            this.isRunning = false;
        }
    }
}
