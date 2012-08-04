using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BurnSystems.WebServer;
using BurnSystems.ObjectActivation;
using BurnSystems.Logging;
using BurnSystems.WebServer.Dispatcher;
using BurnSystems.WebServer.Responses.Tests;
using BurnSystems.WebServer.Dispatcher.Test;

namespace SimpleTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.TheLog.FilterLevel = LogLevel.Verbose;
            Log.TheLog.AddLogProvider(new ConsoleProvider());
            var activationContainer = new ActivationContainer("Server");
            var server = Server.CreateDefaultServer(activationContainer);
            server.AddPrefix("http://127.0.0.1:8081/");
            server.AddPrefix("http://localhost:8081/");
            server.Start();

            server.Add(new ControllerDispatcher<TestController>(DispatchFilter.ByUrl("/controller"), "/controller"));

            server.Add(new ExceptionDispatcher(DispatchFilter.ByUrl("/exception")));
            server.Add(new FileSystemDispatcher(DispatchFilter.All, "htdocs\\"));
            server.Start();


            Console.WriteLine("Press key to stop server");
            Console.ReadKey();
            server.Stop();
        }
    }
}
