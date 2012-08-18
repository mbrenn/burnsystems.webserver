using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.MVC;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Helper;

namespace BurnSystems.WebServer.UnitTests.Controller
{
    /// <summary>
    /// Demo controller for Post
    /// </summary>
    public class PostController : MVC.Controller
    {
        
        public class PostTestModel
        {
            public string Prename
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }
        }

        [WebMethod]
        [IfMethodIs("get")]
        public void PostTest([Inject("PageTemplate")] string template)
        {
            this.Html(template);
        }

        [WebMethod(Name = "PostTest")]
        [IfMethodIs("post")]
        public void PostTestCallback([PostModel] PostTestModel model, [Inject("PageTemplate")] string template)
        {
            this.Html("Prename: " + model.Prename + "<br />Name: " + model.Name);
        }
    }
}
