using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.MVC;

namespace BurnSystems.WebServer.Responses.Tests
{
    public class TestController : Controller
    {
        public void Today()
        {
            this.Html(DateTime.Now.ToString());
        }

        public void Greet(string name)
        {
            this.Html("Hello " + name);
        }

        public void Add(int a, int b)
        {
            this.Html(string.Format("{0} + {1} = {2}", a, b, a + b));
        }
    }
}
