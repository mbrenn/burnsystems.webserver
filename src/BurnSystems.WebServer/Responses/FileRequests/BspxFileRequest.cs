using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Dispatcher;
using BurnSystems.WebServer.MVC;

namespace BurnSystems.WebServer.Responses.FileRequests
{
    public class BspxFileRequest :BaseDispatcher, IFileRequestDispatcher
    {
        public string PhysicalPath
        {
            get;
            set;
        }

        public string VirtualPath
        {
            get;
            set;
        }

        public BspxFileRequest()
            : base(DispatchFilter.All)
        {
        }

        /// <summary>
        /// Dispatches the object
        /// </summary>
        /// <param name="container">Container for activations</param>
        /// <param name="context">Context being used</param>
        public override void Dispatch(ObjectActivation.IActivates container, System.Net.HttpListenerContext context)
        {
            var viewModel = File.ReadAllText(this.PhysicalPath);
            string resultText = "";

            // Try to configuration xml file
            var completeFile = viewModel.TrimStart();
            if (completeFile.StartsWith("<%"))
            {
                var endPosition = completeFile.IndexOf("%>");
                if (endPosition != -1)
                {
                    viewModel = completeFile.Substring(endPosition + 2);
                    
                    // <%1234%>
                    // 01234567
                    var configuration = completeFile.Substring(2, endPosition - 2);

                    // Ok, now we got our configuration node
                    var configurationNode = XDocument.Parse(configuration);

                    // Who's your daddy... Controller is your Daddy, now we got the url of the controller
                    var controllerType = configurationNode.Elements("DynamicPage").Attributes("ControllerType").FirstOrDefault();
                    var controllerWebMethod = configurationNode.Elements("DynamicPage").Attributes("WebMethod").FirstOrDefault();

                    if (controllerType == null)
                    {
                        ErrorResponse.Throw404(container, context, "Xml-Node: DynamicPage/@ControllerType not found");
                        return;
                    }

                    if (controllerWebMethod == null)
                    {
                        ErrorResponse.Throw404(container, context, "Xml-Node: DynamicPage/@WebMethod not found");
                        return;
                    }

                    // Find controller
                    var type = EnvironmentHelper.GetTypeByName(controllerType.Value);
                    if (type == null)
                    {
                        ErrorResponse.Throw404(container, context, string.Format("Type for Controller: {0} not found", controllerType.Value));
                        return;
                    }

                    // Now, create me!
                    var controller = container.Create(type) as Controller;
                    
                    resultText = controller.GetType().FullName;
                }
            }

            // Give response
            var result = Encoding.UTF8.GetBytes(resultText);
            using (var stream = context.Response.OutputStream)
            {
                stream.Write(result, 0, result.Count());
            }
        }
    }
}
