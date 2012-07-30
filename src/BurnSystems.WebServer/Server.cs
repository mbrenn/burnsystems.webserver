using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Parser;

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
        /// Stores the activation container
        /// </summary>
        private ActivationContainer activationContainer;

        /// <summary>
        /// Gets the activation container
        /// </summary>
        public ActivationContainer ActivationContainer
        {
            get { return this.activationContainer; }
        }

        /// <summary>
        /// Stores the template parser used for Web
        /// </summary>
        private static TemplateParser parser = new TemplateParser();

        /// <summary>
        /// Defines the binding name for template parser
        /// </summary>
        public const string TemplateParserBindingName = "WebtemplateParser";

        /// <summary>
        /// Stores the name for the default dispatcher
        /// </summary>
        public const string DefaultDispatcherBindingName = "DefaultDispatcher";

        /// <summary>
        /// Creates the default server
        /// </summary>
        public static Server Default
        {
            get
            {
                var activationContainer = new ActivationContainer("BurnSystems.Webserver");
                
                return CreateDefaultServer(activationContainer);
            }
        }

        /// <summary>
        /// Creates the default server with all bindings
        /// </summary>
        /// <param name="activationContainer">Activation container where default binding will be addedd</param>
        /// <returns></returns>
        public static Server CreateDefaultServer(ObjectActivation.ActivationContainer activationContainer)
        {
            activationContainer.BindToName(Server.TemplateParserBindingName).ToConstant(parser);

            return new Server(activationContainer);
        }

        /// <summary>
        /// Initializes a new instance of the Server class.
        /// </summary>
        /// <param name="container">Container to be set</param>
        public Server(ActivationContainer container)
        {
            this.activationContainer = container;
        }

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

            this.listener = new Listener(this.activationContainer, this.prefixes);
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
