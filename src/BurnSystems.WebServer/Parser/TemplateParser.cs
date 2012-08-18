using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RazorEngine.Templating;
using RazorEngine;
using RazorEngine.Compilation;

namespace BurnSystems.WebServer.Parser
{
    /// <summary>
    /// Stores the template parser for the webservice
    /// </summary>
    public class TemplateParser : ITemplateParser
    {
        /// <summary>
        /// Stores the template service
        /// </summary>
        private TemplateService templateService = new TemplateService();

        /// <summary>
        /// Initializes a new instance of the template parser instance
        /// </summary>
        public TemplateParser()            
        {
            this.templateService.AddNamespace("BurnSystems");
        }

        public string Parse(string template, object model, DynamicViewBag bag, string cacheName)
        {
            return this.templateService.Parse(template, model, bag, cacheName);
        }
    }
}
