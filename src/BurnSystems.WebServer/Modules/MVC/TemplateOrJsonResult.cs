﻿using BurnSystems.WebServer.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.Modules.MVC
{
    /// <summary>
    /// Returns an object or a created template, 
    /// dependign whether whether we have a template
    /// </summary>
    public class TemplateOrJsonResult<T> : BaseActionResult, IActionResult
    {
        /// <summary>
        /// Gets or sets the return object
        /// </summary>
        public T ReturnObject
        {
            get;
            set;
        }

        public TemplateOrJsonResult(T resultObject)
        {
            this.ReturnObject = resultObject;
        }

        public void Execute(System.Net.HttpListenerContext listenerContext, IActivates container)
        {
            listenerContext.Response.ContentEncoding = Encoding.UTF8;;
         
            var template = container.GetByName("PageTemplate");
            if (template == null)
            {
                new JsonActionResult(this.ReturnObject).Execute(listenerContext, container);
            }
            else
            {
                listenerContext.Response.ContentType = "text/html";
                var templateParser = container.Get<ITemplateParser>();
                if (templateParser == null)
                {
                    throw new InvalidOperationException("ITemplateParser not set");
                }

                new HtmlActionResult(
                    templateParser.Parse<T>(template.ToString(), this.ReturnObject, null))
                    .Execute(listenerContext, container);
            }
        }
    }
}
