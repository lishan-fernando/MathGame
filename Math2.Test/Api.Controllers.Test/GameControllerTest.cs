using System.Net;
using System.Net.Http;
using System.Web.Http;
using Math2.Controllers;
using NUnit.Framework;

namespace Math2.Test.Api.Controllers.Test
{
    [TestFixture]
    public class GameControllerTest
    {
        private readonly string _playerName = "Lishan";

        [Test]
        public void GameRegisterWithInvalidPlayerRequestTest()
        {
            var controller = GetController("http://localhost:3276/api/Game/Register?wrong=wrong");
            var response = controller.Get("");

            Assert.NotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Test]
        public void GameRegisterWithEmptyPlayerRequestTest()
        {
            var controller = GetController("http://localhost:3276/api/Game/Register");
            var response = controller.Get("");

            Assert.NotNull(response);
            Assert.AreEqual(response.StatusCode, HttpStatusCode.BadRequest);
        }

        private GameController GetController(string url)
        {
            var controller = new GameController
            {
                Request = new HttpRequestMessage(HttpMethod.Get, url),
                Configuration = new HttpConfiguration()
            };
            return controller;
        }
    }
}
