using System;
using System.Net;
using System.Threading;
using BurnSystems.Logging;
using System.Collections.Generic;
using System.Text;
using RazorEngine;
using BurnSystems.WebServer.Parser;

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
        /// Stores the template parser
        /// </summary>
        private TemplateParser templateParser = new TemplateParser();

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
                    try
                    {
                        this.ExecuteHttpRequest(context);

                        //ThreadPool.QueueUserWorkItem(new WaitCallback(this.ExecuteHttpRequest), context);
                    }
                    catch (Exception exc)
                    {
                        Log.TheLog.LogEntry(
                            new LogEntry(
                                String.Format(
                                    Localization_WebServer.ExceptionDuringListening,
                                    exc.Message),
                                LogLevel.Message));
                        Log.TheLog.LogEntry(
                            new LogEntry(
                                String.Format(
                                    Localization_WebServer.ExceptionDuringListening,
                                    exc.ToString()),
                                LogLevel.Verbose));
                    }
                }
            }
            catch (HttpListenerException)
            {
                // Listener has been stopped.
            }
        }        

        /// <summary>
        /// Executes the http request itself
        /// </summary>
        /// <param name="context"></param>
        private void ExecuteHttpRequest(object value)
        {
            var context = value as HttpListenerContext;
            if (context == null)
            {
                throw new ArgumentException("value is not HttpListenerContext");
            }

            var content404 = Localization_WebServer.Error404;

            var model = new
            {
                Title = "File Not Found",
                Message = context.Request.Url.ToString(),
                Code = 404.ToString()
            };

            var template = this.templateParser.Parse(content404, model, "__Content404");

            context.Response.StatusCode = 404;
            using (var response = context.Response.OutputStream)
            {
                var bytes = Encoding.UTF8.GetBytes(template);
                response.Write(bytes, 0, bytes.Length);
            }

            context.Response.Close();

        }
    }
}
