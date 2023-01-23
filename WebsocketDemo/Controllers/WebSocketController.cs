using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebsocketDemo.Services;

namespace WebsocketDemo.Controllers
{
    // See https://learn.microsoft.com/en-us/aspnet/core/fundamentals/websockets?view=aspnetcore-6.0
    public class WebSocketController : ControllerBase
    {
        private readonly WebsocketService _websocketService;

        public WebSocketController(WebsocketService websocketService)
        {
            _websocketService = websocketService;
        }

        /// <summary>
        /// Will be called when a client tries to connect via websocket with the URL
        /// ws(s)://(server)/ws/(client-guid)
        /// </summary>
        [HttpGet("/ws/{clientGuid:Guid}")]
        public async Task Get(Guid clientGuid)
        {
            // Is it a websocket or an http request?
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _websocketService.DoWork(clientGuid, webSocket);
        }
    }
}
