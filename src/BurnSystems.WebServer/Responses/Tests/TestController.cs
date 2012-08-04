using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.MVC;

namespace BurnSystems.WebServer.Responses.Tests
{
    public class TestController : Controller
    {
        [WebMethod]
        public void Today()
        {
            this.Html(DateTime.Now.ToString());
        }

        [WebMethod]
        public void Greet(string name)
        {
            this.Html("Hello " + name);
        }

        [WebMethod]
        public void Add(int a, int b)
        {
            this.Html(string.Format("{0} + {1} = {2}", a, b, a + b));
        }
    }
}
