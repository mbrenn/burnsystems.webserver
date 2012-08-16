using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.MVC;
using BurnSystems.ObjectActivation;

namespace BurnSystems.WebServer.UnitTests.Controller
{
    /// <summary>
    /// Demo controller for Post
    /// </summary>
    public class PostController : MVC.Controller
    {
        [WebMethod]
        public void PostTest([Inject("PageTemplate")] string template)
        {
            this.Html(template);
        }
    }
}
