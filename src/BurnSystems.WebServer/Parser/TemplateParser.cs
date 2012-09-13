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
    public class TemplateParser : ITemplateParser, IDisposable
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

        public string Parse<T>(string template, T model, Dictionary<string, object> bag, string cacheName)
        {
            if (bag == null)
            {
                return this.templateService.Parse<T>(template, model, null, cacheName);
            }
            else
            {
                var typeBag = new DynamicViewBag();
                typeBag.AddDictionaryValues(bag);
                return this.templateService.Parse<T>(template, model, typeBag, cacheName);
            }
        }

        /// <summary>
        /// Disposes the parser and including templateservice
        /// </summary>
        public void Dispose()
        {
            if (this.templateService != null)
            {
                this.templateService.Dispose();
                this.templateService = null;
            }
        }
    }
}
