using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.MVC;
using BurnSystems.ObjectActivation;
using BurnSystems.WebServer.Helper;
using BurnSystems.WebServer.Parser;

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

        [Inject]
        public ITemplateParser TemplateParser
        {
            get;
            set;
        }

        [WebMethod]
        public void PostTest([PostModel] PostTestModel model, [Inject("PageTemplate")] string template)
        {
            if (model == null)
            {
                this.Html(this.TemplateParser.Parse<PostTestModel>(template, null, null, "PostController"));
            }
            else
            {
                this.Html(this.TemplateParser.Parse(template, model, null, "PostController"));
            }
        }
    }
}
