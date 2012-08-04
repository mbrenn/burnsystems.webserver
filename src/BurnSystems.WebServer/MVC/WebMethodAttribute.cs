using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnSystems.WebServer.MVC
{
    /// <summary>
    /// Used to define a webmethod
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public sealed class WebMethodAttribute : Attribute
    {
    }
}
