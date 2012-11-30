using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BurnSystems.Extensions;
using BurnSystems.Logging;
using BurnSystems.ObjectActivation;
using BurnSystems.Test;
using BurnSystems.WebServer.Dispatcher;
using BurnSystems.WebServer.Modules.PostVariables;
using BurnSystems.WebServer.Responses;

namespace BurnSystems.WebServer.Modules.MVC
{
    /// <summary>
    /// Contains the controller dispatcher for a specific controller class
    /// </summary>
    /// <typeparam name="T">Type of the controller</typeparam>
    public class ControllerDispatcher<T> : ControllerDispatcher where T : Controller, new()
    {
        /// <summary>
        /// Initializes a new instance of the ControllerDispatcher class
        /// </summary>
        /// <param name="filter">Filter being used</param>
        public ControllerDispatcher(Func<ContextDispatchInformation, bool> filter)
            : base(typeof(T), filter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ControllerDispatcher class
        /// </summary>
        /// <param name="filter">Filter being used</param>
        /// <param name="webPath">Virtual web path being used to strip away constant part</param>
        public ControllerDispatcher(Func<ContextDispatchInformation, bool> filter, string webPath)
            : base(typeof(T), filter, webPath)
        {
        }
    }

    public class ControllerDispatcher : BaseDispatcher
    {
        /// <summary>
        /// Stores the type of the controller
        /// </summary>
        private Type controllerType;

        /// <summary>
        /// Logger being used in this class.
        /// </summary>
        private ClassLogger logger = new ClassLogger(typeof(ControllerDispatcher));

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
        /// <param name="controllerType">Type of the controller being used</param>
        /// <param name="filter">Filter being used</param>
        public ControllerDispatcher(Type controllerType, Func<ContextDispatchInformation, bool> filter)
            : base(filter)
        {
            this.controllerType = controllerType;
            this.InitializeWebMethods();
        }
        
        /// <summary>
        /// Initializes a new instance of the ControllerDispatcher class
        /// </summary>
        /// <param name="controllerType">Type of the controller</param>
        /// <param name="filter">Filter being used</param>
        /// <param name="webPath">Path, where controller is stored</param>
        public ControllerDispatcher(Type controllerType, Func<ContextDispatchInformation, bool> filter, string webPath)
            : this(controllerType, filter)
        {
            this.WebPath = webPath;
        }

        /// <summary>
        /// Dispatches the object, depending of activity and context
        /// </summary>
        /// <param name="activates">Container used for activation</param>
        /// <param name="context">Context of http</param>
        public override void Dispatch(ObjectActivation.IActivates activates, ContextDispatchInformation info)
        {
            // Stores the absolute path
            var absolutePath = info.RequestUrl.AbsolutePath;
            if (!string.IsNullOrEmpty(this.WebPath) && !this.WebPath.EndsWith("/"))
            {
                this.WebPath = this.WebPath + "/";
                logger.LogEntry(new LogEntry("this.WebPath did not end with '/'", LogLevel.Verbose));
            }

            if (!absolutePath.StartsWith(this.WebPath))
            {
                // I'm not the real responsible for this task
                ErrorResponse.Throw404(activates, info);
                return;
            }
            
            // Now try to find the correct method and call the function
            var restUrl = absolutePath.Substring(this.WebPath.Length);
            if (string.IsNullOrEmpty(restUrl))
            {
                // Nothing to do here... Default page. Not implemented yet
                ErrorResponse.Throw404(activates, info);
                return;
            }

            // Split segments
            var segments = restUrl.Split(new char[] { '/' });

            // First segment is method name
            var methodName = segments[0];

            this.DispatchForWebMethod(activates, info, methodName);
        }

        /// <summary>
        /// Performs the dispatch for a specific 
        /// </summary>
        /// <param name="activates">Activation Container</param>
        /// <param name="context">WebContext for request</param>
        /// <param name="controller">Controller to be used</param>
        /// <param name="methodName">Requested web method</param>
        public void DispatchForWebMethod(ObjectActivation.IActivates activates, ContextDispatchInformation info, string methodName)
        {
            // Creates controller
            var controller = activates.Create(this.controllerType) as Controller;
            Ensure.That(controller != null, "Not a ControllerType: " + this.controllerType.FullName);

            // Try to find the method
            foreach (var webMethodInfo in this.webMethodInfos
                .Where(x => x.Name == methodName))
            {
                // Check for http Method
                if (!string.IsNullOrEmpty(webMethodInfo.IfMethodIs))
                {
                    if (webMethodInfo.IfMethodIs != info.HttpMethod.ToLower())
                    {
                        // No match, 
                        continue;
                    }
                }

                // Ok, now look for get variables
                var parameters = webMethodInfo.MethodInfo.GetParameters();
                var callArguments = new List<object>();
                foreach (var parameter in parameters)
                {
                    var parameterAttributes = parameter.GetCustomAttributes(false);

                    /////////////////////////////////
                    // Check for POST-Parameter
                    var postParameterAttribute = parameterAttributes.Where(x => x is PostModelAttribute).Cast<PostModelAttribute>().FirstOrDefault();
                    if (postParameterAttribute != null)
                    {
                        if (info.HttpMethod.ToLower() != "post")
                        {
                            callArguments.Add(null);
                        }
                        else
                        {
                            callArguments.Add(
                                this.CreatePostModel(activates, parameter));
                        }
                        continue;
                    }

                    /////////////////////////////////
                    // Check for injection parameter
                    var injectParameterAttribute = parameterAttributes.Where(x => x is InjectAttribute).Cast<InjectAttribute>().FirstOrDefault();
                    if (injectParameterAttribute != null)
                    {
                        if (string.IsNullOrEmpty(injectParameterAttribute.ByName))
                        {
                            // Ok, we are NOT by name, injection by type
                            callArguments.Add(activates.Get(parameter.ParameterType));
                        }
                        else
                        {
                            callArguments.Add(activates.GetByName(injectParameterAttribute.ByName));
                        }

                        continue;
                    }

                    /////////////////////////////////
                    // Check for Url-Parameter
                    var urlParameterAttributes = parameterAttributes.Where(x => x is UrlParameterAttribute).FirstOrDefault();
                    if (urlParameterAttributes != null)
                    {
                        callArguments.Add(null);
                        // Is a url parameter attribute, do not like this
                        continue;
                    }

                    // Rest is Get-Parameter
                    var value = info.Context.Request.QueryString[parameter.Name];
                    if (value == null)
                    {
                        callArguments.Add(this.GetDefaultArgument(parameter));
                    }
                    else
                    {
                        callArguments.Add(this.ConvertToArgument(value, parameter));
                    }
                }

                var result = webMethodInfo.MethodInfo.Invoke(controller, callArguments.ToArray()) as IActionResult;
                if (result == null)
                {
                    logger.LogEntry(LogEntry.Format(LogLevel.Message, "WebMethod '{0}' does not return IActionResult", methodName));
                }
                else
                {
                    result.Execute(info.Context, activates);
                }

                // First hit is success
                return;
            }

            ErrorResponse.Throw404(activates, info);
            return;
        }

        /// <summary>
        /// Creates the post model for a certain parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private object CreatePostModel(IActivates activates,  System.Reflection.ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            var instance = Activator.CreateInstance(parameterType);
            var postVariables = activates.Get<PostVariableReader>();

            foreach (var property in parameterType.GetProperties(BindingFlags.SetField | BindingFlags.Instance | BindingFlags.Public))
            {
                var value = postVariables[property.Name];
                if (value == null)
                {
                    continue;
                }

                property.SetValue(
                    instance,
                    value.ConvertTo(property.PropertyType), 
                    null);
            }

            return instance;
        }
        
        /// <summary>
        /// Initializes the webmethods and stores the webmethods into 
        /// </summary>
        private void InitializeWebMethods()
        {
            // Try to find method
            var foundMethods = this.controllerType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
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

                var methods = info.Method.GetCustomAttributes(typeof(IfMethodIsAttribute), false);
                if (methods.Length == 1)
                {
                    code.IfMethodIs = (methods[0] as IfMethodIsAttribute).MethodName;
                }

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
    }
}
