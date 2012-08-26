using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer.Modules.MVC;

namespace BurnSystems.WebServer.UnitTests.Controller
{
    /// <summary>
    /// Calculator for webmethods
    /// </summary>
    public class CalcController : Modules.MVC.Controller
    {
        public class SumData
        {
            public double Summand1
            {
                get;
                set;
            }

            public double Summand2
            {
                get;
                set;
            }
        }

        public class SumResult
        {
            public double Sum
            {
                get;
                set;
            }
        }

        [WebMethod()]
        [IfMethodIs("post")]
        public void Sum([PostModel] SumData sum)
        {
            this.Json(new SumResult()
            {
                Sum = sum.Summand1 + sum.Summand2
            });
        }
    }
}
