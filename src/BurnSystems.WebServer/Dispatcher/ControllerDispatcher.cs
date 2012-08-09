using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.WebServer.MVC;
using System.Linq.Expressions;
using BurnSystems.ObjectActivation;
using BurnSystems.Logging;
using BurnSystems.WebServer.Responses;

namespace BurnSystems.WebServer.Dispatcher
{
    /// <summary>
    /// Contains the controller dispatcher for a specific controller class
    /// </summary>
    /// <typeparam name="T">Type of the controller</typeparam>
    public class ControllerDispatcher<T> : BaseDispatcher where T : Controller, new()
    {
        /// <summary>
        /// Logger being used in this class.
        /// </summary>
        private ClassLogger logger = new ClassLogger(typeof(ControllerDispatcher<T>));

        /// <summary>
        /// Stores the webmethod infos
        /// </summary>
        private List<WebMethodInfo> webMethodInfos = new List<WebMethodInfo>();

        /// <summary>
        /// Gets or sets the webpath, including controller name
        /// </summary>
        public string WebPath
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the ControllerDispatcher class
        /// </summary>
        /// <param name="filter">Filter being used</param>
        public ControllerDispatcher(Func<HttpListenerContext, bool> filter)
            : base(filter)
        {
            this.InitializeWebMethods();
        }
        
        /// <summary>
        /// Initializes a new instance of the ControllerDispatcher class
        /// </summary>
        /// <param name="filter">Filter being used</param>
        /// <param name="webPath">Path, where controller is stored</param>
        public ControllerDispatcher(Func<HttpListenerContext, bool> filter, string webPath)
            : this(filter)
        {
            this.WebPath = webPath;
        }
        
        public override void Dispatch(ObjectActivation.IActivates activates, HttpListenerContext context)
        {
            // Stores the absolute path
            var absolutePath = context.Request.Url.AbsolutePath;
            if (!this.WebPath.EndsWith("/"))
            {
                this.WebPath = this.WebPath + "/";
                logger.LogEntry(new LogEntry("this.WebPath did not end with '/'", LogLevel.Verbose));
            }

            if (!absolutePath.StartsWith(this.WebPath))
            {
                // I'm not the real responsible for this task
                this.Throw404(activates, context);
                return;
            }

            var controller = activates.Create<T>();
            controller.Container = activates;
            controller.Context = context;

            // Now try to find the correct method and call the function
            var restUrl = absolutePath.Substring(this.WebPath.Length);
            if (string.IsNullOrEmpty(restUrl))
            {
                // Nothing to do here... Default page. Not implemented yet
                this.Throw404(activates, context);
                return;
            }

            // Split segments
            var segments = restUrl.Split(new char[] { '/' });
            
            // First segment is method name
            var methodName = segments[0];

            // Try to find method
            var info = this.webMethodInfos
                .Where(x => x.Name == methodName)  
                .FirstOrDefault();

            if (info == null)
            {
                this.Throw404(activates, context);
                return;
            }

            // Ok, now look for get variables
            var parameters = info.MethodInfo.GetParameters();
            var callArguments = new List<object>();
            foreach (var parameter in parameters)
            {
                var urlParameterAttributes = parameter.GetCustomAttributes(typeof(UrlParameterAttribute), false);
                if (urlParameterAttributes.Length > 0)
                {
                    // Is a url parameter attribute, do not like this
                    continue;
                }

                var value = context.Request.QueryString[parameter.Name];
                if (value == null)
                {
                    callArguments.Add(this.GetDefaultArgument(parameter));
                }
                else
                {
                    callArguments.Add(this.ConvertToArgument(value, parameter));
                }
            }

            info.MethodInfo.Invoke(controller, callArguments.ToArray());
        }
        
        /// <summary>
        /// Initializes the webmethods and stores the webmethods into 
        /// </summary>
        private void InitializeWebMethods()
        {
            // Try to find method
            var foundMethods = typeof(T).GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
                .Select (x=> new 
                {
                    Method = x,
                    WebMethodAttribute = (WebMethodAttribute) x.GetCustomAttributes(typeof(WebMethodAttribute), false).FirstOrDefault()
                })
                .Where(x => x.WebMethodAttribute != null);

            // Convert methods into predefined data structures
            foreach (var info in foundMethods)
            {                
                var name = info.Method.Name;
                if (!string.IsNullOrEmpty(info.WebMethodAttribute.Name))
                {
                    name = info.WebMethodAttribute.Name;
                }

                var code = new WebMethodInfo()
                {
                    MethodInfo = info.Method,
                    Name = name
                };

                this.webMethodInfos.Add(code);
            }
        }

        /// <summary>
        /// Gets the default argument, depending on type
        /// </summary>
        /// <param name="parameter">Used parameter info</param>
        /// <returns>Default argument</returns>
        private object GetDefaultArgument(System.Reflection.ParameterInfo parameter)
        {
            if (parameter.ParameterType == typeof(string))
            {
                return null;
            }

            if (parameter.ParameterType == typeof(int))
            {
                return 0;
            }

            if (parameter.ParameterType == typeof(double))
            {
                return 0.0;
            }

            throw new ArgumentException("Unknown Parameter Type: " + parameter.ParameterType);
        }

        /// <summary>
        /// Converts the given string to a parameter
        /// </summary>
        /// <param name="value">Value to be converted</param>
        /// <param name="parameter">Parameter Info</param>
        /// <returns>Converted Argument</returns>
        private object ConvertToArgument(string value, System.Reflection.ParameterInfo parameter)
        {

            if (parameter.ParameterType == typeof(string))
            {
                return value;
            }

            if (parameter.ParameterType == typeof(int))
            {
                return Convert.ToInt32(value);
            }

            if (parameter.ParameterType == typeof(double))
            {
                return Convert.ToDouble(value);
            }

            throw new ArgumentException("Unknown Parameter Type: " + parameter.ParameterType);
        }

        /// <summary>
        /// Throws a 404 page
        /// </summary>
        /// <param name="container">Container to be used</param>
        /// <param name="context">HTTP Context</param>
        private void Throw404(ObjectActivation.IActivates container, HttpListenerContext context)
        {
            var errorResponse = container.Create<ErrorResponse>();
            errorResponse.Set(HttpStatusCode.NotFound);
            errorResponse.Dispatch(container, context);
        }
    }
}
