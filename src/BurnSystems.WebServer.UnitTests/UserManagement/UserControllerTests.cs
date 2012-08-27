using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using BurnSystems.WebServer.Modules.UserManagement;

namespace BurnSystems.WebServer.UnitTests.UserManagement
{
    [TestFixture]
    public class UserControllerTests
    {
        [Test]
        public void TestWrongLogin()
        {
            using (var server = ServerTests.CreateServer())
            {
                var cookie = BspxTests.GetCookie();
                var webRequest = BspxTests.GetSessionRequest(
                    cookie,
                    "http://localhost:8081/controller/Users/Login", 
                    "Username=no&Password=Yes");

                var responseValue = BspxTests.GetResponseObject<UserManagementController.LoginResult>(webRequest);
                Assert.That(responseValue.Success, Is.False);                
            }
        }

        [Test]
        public void TestCorrectLogin()
        {
            using (var server = ServerTests.CreateServer())
            {
                var cookie = BspxTests.GetCookie();
                var webRequest = BspxTests.GetSessionRequest(
                    cookie,
                    "http://localhost:8081/controller/Users/Login",
                    "Username=Karl&Password=Heinz");

                var responseValue = BspxTests.GetResponseObject<UserManagementController.LoginResult>(webRequest);
                Assert.That(responseValue.Success, Is.True);
            }
        }

        [Test]
        public void TestUserRetrieval()
        {
        }

        [Test]
        public void TestLogout()
        {
        }
    }
}
