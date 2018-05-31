#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.WebSockets;
using Math2.Enums;
using Math2.Interfaces;
using Math2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;
#endregion

namespace Math2.Controllers
{
    public class GameController : ApiController
    {
        #region public properties
        [Inject]
        public IGameBusinessLogics GameLogicHandler { get; set; }
        #endregion

        #region public methods
        [HttpGet]
        [Route("api/Game/Register")]
        public HttpResponseMessage Get(string playerName)
        {
            var pairs = Request.GetQueryNameValuePairs().ToList();

            if (!pairs.Any(p => p.Key.Equals("playerName")))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (HttpContext.Current.IsWebSocketRequest)
            {
                HttpContext.Current.AcceptWebSocketRequest(ProcessWebSocketRequest);
                return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
            }

            return new HttpResponseMessage(HttpStatusCode.SeeOther);
        }
        #endregion

        #region private methods
        private async Task ProcessWebSocketRequest(AspNetWebSocketContext context)
        {
            var pairs = Request.GetQueryNameValuePairs().ToList();
            if (pairs.Any())
            {
                var player = pairs.First();
                var webSocket = context.WebSocket;
                GameLogicHandler.AddPlayer(player.Value, webSocket);
                await GameLogicHandler.NewGame();

                byte[] receiveBuffer = new byte[1024];
                while (webSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                        if (receiveResult.Count > 0)
                        {
                            string s = Encoding.UTF8.GetString(receiveBuffer, 0, receiveBuffer.Length).Replace("\0", string.Empty).ToString();
                            var answer = JsonConvert.DeserializeObject<Answer>(s);
                            await GameLogicHandler.EvaluateAnswer(answer);
                        }
                    }
                    catch (Exception e)
                    {
                        // To do
                    }
                }
            }
        }
        #endregion
    }
}
