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
    public class TemplateParser : TemplateService
    {
        /// <summary>
        /// Gets or sets the compiler service factory
        /// </summary>
        private static ICompilerServiceFactory CompilerServiceFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static TemplateParser()
        {
            CompilerServiceFactory = new DefaultCompilerServiceFactory();
        }

        /// <summary>
        /// Initializes a new instance of the template parser instance
        /// </summary>
        public TemplateParser()
            : base()
        {
        }
    }
}
