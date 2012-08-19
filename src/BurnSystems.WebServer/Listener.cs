using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using BurnSystems.Logging;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Parser;
using BurnSystems.WebServer.Responses;
using BurnSystems.WebServer.Dispatcher;

namespace BurnSystems.WebServer
{
    /// <summary>
    /// Listener receiving the http requests and dispatching them to the correct place
    /// </summary>
    internal class Listener
    {
        /// <summary>
        /// Defines the class logger
        /// </summary>
        private ClassLogger logger = new ClassLogger(typeof(Listener));

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
        /// Stoers the activation container
        /// </summary>
        private ActivationContainer activationContainer;

        /// <summary>
        /// Initializes a new instance of the Listener instance
        /// </summary>
        /// <param name="container">Defines the activation container</param>
        /// <param name="prefixes">Prefixes to be listened to</param>
        internal Listener(ActivationContainer container, IEnumerable<string> prefixes)
        {
            this.activationContainer = container;
            this.httpListener = new HttpListener();
            foreach (var prefix in prefixes)
            {
                logger.LogEntry(new LogEntry(
                    string.Format(Localization_WebServer.AddedPrefix, prefix),
                    LogLevel.Notify));

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
                        logger.LogEntry(
                            new LogEntry(
                                String.Format(
                                    Localization_WebServer.ExceptionDuringListening,
                                    exc.Message),
                                LogLevel.Message));
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

            var webRequestContainer = new ActivationContainer("WebRequest", this.activationContainer);
            webRequestContainer.Bind<HttpListenerContext>().ToConstant(context);

            using (var block = new ActivationBlock("WebRequest", webRequestContainer))
            {
                try
                {
                    var info = new ContextDispatchInformation(context);
                    try
                    {
                        if (context == null)
                        {
                            throw new ArgumentException("value is not HttpListenerContext");
                        }

                        foreach (var dispatcher in block.GetAll<IRequestDispatcher>())
                        {
                            if (dispatcher.IsResponsible(block, info))
                            {
                                dispatcher.Dispatch(block, info);
                            }
                        }

                        // Throw 404
                        var errorResponse = this.activationContainer.Create<ErrorResponse>();
                        errorResponse.Set(HttpStatusCode.NotFound);
                        errorResponse.Dispatch(block, info);

                    }
                    catch (Exception exc)
                    {
                        var errorResponse = this.activationContainer.Create<ErrorResponse>();
                        errorResponse.Title = "Server error";
                        errorResponse.Message = exc.ToString();
                        errorResponse.Code = 500;
                        errorResponse.Dispatch(block, info);
                    }
                    finally
                    {
                        context.Response.Close();
                    }
                }
                catch (Exception exc)
                {
                    // Default, can't do anything
                    logger.LogEntry(new LogEntry(exc.Message, LogLevel.Message));
                }
            }
        }
    }
}
