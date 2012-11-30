using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.Modules.MVC
{
    /// <summary>
    /// Defines the extension class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Returns an html result to browser
        /// </summary>
        /// <param name="result">Result to be returned</param>
        public static IActionResult Html(this Controller controller, string result)
        {
            return new HtmlActionResult(result);
        }

        /// <summary>
        /// Returns html to browser and uses the Template parser as stored in container and
        /// PageTemplate as stored in container
        /// </summary>
        /// <typeparam name="T">Type of the model</typeparam>
        /// <param name="model">Model to be set</param>
        public static IActionResult TemplateOrJson<T>(this Controller controller, T model)
        {
            return new TemplateOrJsonResult<T>(model);
        }

        /// <summary>
        /// Returns an html result to browser
        /// </summary>
        /// <param name="result">Result to be returned</param>
        public static IActionResult Json(this Controller controller, object result)
        {
            return new JsonActionResult(result);
        }
    }
}
