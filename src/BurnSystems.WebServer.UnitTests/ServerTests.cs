using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Net;
using System.Threading;

namespace BurnSystems.WebServer.UnitTests
{
    [TestFixture]
    public class ServerTests
    {
        /// <summary>
        /// Tests the server itself. Just opening and closing
        /// </summary>
        [Test]
        public void TestServer()
        {
            var server = new Server();
            server.AddPrefix("http://localhost:8081/");
            server.Start();

            var webClient = new WebClient();
            webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

            try
            {
                var data = webClient.DownloadString("http://localhost:8081/");

                // Shall not get reached
                Assert.IsTrue(false);
            }
            catch (WebException exc)
            {
                Assert.That(exc.Status == WebExceptionStatus.ProtocolError);
            }

            server.Stop();
        }

    }
}
