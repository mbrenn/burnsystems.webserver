using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.MVC
{
    /// <summary>
    /// This attribute is set for webmethods that shall only be called for a certain HTTP-Method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    sealed class IfMethodIsAttribute : Attribute
    {
        public string MethodName
        {
            get;
            private set;
        }

        public IfMethodIsAttribute(string methodName)
        {
            this.MethodName = methodName.ToLower();
        }
    }
}
