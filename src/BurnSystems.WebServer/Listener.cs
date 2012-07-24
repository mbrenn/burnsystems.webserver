using System;
using System.Net;
using System.Threading;
using BurnSystems.Logging;
using System.Collections.Generic;
using System.Text;

namespace BurnSystems.WebServer
{
    /// <summary>
    /// Listener receiving the http requests and dispatching them to the correct place
    /// </summary>
    internal class Listener
    {
        /// <summary>
        /// Httplistener, which receives the requests
        /// </summary>
        private HttpListener httpListener;

        /// <summary>
        /// Thread, which listens to the http-socket
        /// </summary>
        private Thread httpThread;

        /// <summary>
        /// Flag, ob der Webserver noch laufen soll
        /// </summary>
        private volatile bool running = false;

        /// <summary>
        /// Initializes a new instance of the Listener instance
        /// </summary>
        /// <param name="prefixes"></param>
        internal Listener(IEnumerable<string> prefixes)
        {
            this.httpListener = new HttpListener();
            foreach (var prefix in prefixes)
            {
                this.httpListener.Prefixes.Add(prefix);
            }
        }

        /// <summary>
        /// Starts listening
        /// </summary>
        public void StartListening()
        {
            this.running = true;
            try
            {
                this.httpListener.Start();

                this.httpThread = new Thread(new ThreadStart(this.HttpThreadEntry));
                this.httpThread.Start();
            }
            catch (HttpListenerException exc)
            {
                // Der Benutzer braucht Administorrechte
                string message = string.Format(
                    Localization_WebServer.HttpListenerException,
                    exc.Message);
                Log.TheLog.LogEntry(
                    new LogEntry(message, LogLevel.Critical));

                throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// Stops listening
        /// </summary>
        public void StopListening()
        {
            this.httpListener.Stop();
            this.running = false;
            if (this.httpThread != null)
            {
                this.httpThread.Join();
                this.httpThread = null;
            }
        }

        /// <summary>
        /// Thread entry for receiving http-requests
        /// </summary>
        public void HttpThreadEntry()
        {
            try
            {
                while (this.running)
                {
                    var context = this.httpListener.GetContext();

                    var content404 = Localization_WebServer.Error404;

                    context.Response.StatusCode = 404;
                    using (var response = context.Response.OutputStream)
                    {
                        var bytes = Encoding.UTF8.GetBytes(content404);
                        response.Write(bytes, 0, bytes.Length);
                    }

                    context.Response.Close();

                    //ThreadPool.QueueUserWorkItem(this.ExecuteHttpRequest, context);
                }
            }
            catch (HttpListenerException)
            {
                // Listener has been stopped.
            }
            catch (Exception exc)
            {
                Log.TheLog.LogEntry(
                    new LogEntry(
                        String.Format(
                            Localization_WebServer.ExceptionDuringListening,
                            exc.Message),
                        LogLevel.Message));
            }
        }
    }
}
