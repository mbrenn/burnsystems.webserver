using System.Net;
using BurnSystems.WebServer.Dispatcher;
using BurnSystems.WebServer.Modules.MVC;
using BurnSystems.WebServer.Responses.Tests;
using NUnit.Framework;

namespace BurnSystems.WebServer.UnitTests
{
    [TestFixture]
    public class ServerTests
    {
        public static Server CreateServer()
        {
            var server = Server.Default;
            server.AddPrefix("http://localhost:8081/");

            server.Add(new ControllerDispatcher<TestController>(DispatchFilter.ByUrl("/controller"), "/controller/"));
            server.Add(new FileSystemDispatcher(DispatchFilter.ByUrl("/file"), "htdocs/", "/file/"));
            server.Add(new RelocationDispatcher("/", "/file/test.txt"));
            server.Start();

            return server;
        }

        /// <summary>
        /// Tests the server itself. Just opening and closing
        /// </summary>
        [Test]
        public void TestServer()
        {
            using (var server = CreateServer())
            {
                var webClient = new WebClient();
                webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                try
                {
                    var data = webClient.DownloadString("http://localhost:8081/NotFound");

                    // Shall not get reached
                    Assert.IsTrue(false);
                }
                catch (WebException exc)
                {
                    Assert.That(exc.Status == WebExceptionStatus.ProtocolError);
                }
            }
        }

        [Test]
        public void TestFileSystemDownload()
        {
            using (var server = CreateServer())
            {
                var webClient = new WebClient();
                webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                var data = webClient.DownloadString("http://localhost:8081/file/test.txt");

                // Shall not get reached
                Assert.That(data.Trim(), Is.EqualTo("This is a test."));
            }
        }

        [Test]
        public void TestRelocation()
        {
            using (var server = CreateServer())
            {
                var webClient = new WebClient();
                webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

                var data = webClient.DownloadString("http://localhost:8081/");

                // Shall not get reached
                Assert.That(data.Trim(), Is.EqualTo("This is a test."));
            }
        }
    }
}
