using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.MVC;

namespace BurnSystems.WebServer.UnitTests.Controller
{
    /// <summary>
    /// Demo controller for Post
    /// </summary>
    public class PostController : MVC.Controller
    {
        [WebMethod]
        public void PostTest()
        {
            this.Html("Hallo Welt");
        }
    }
}
