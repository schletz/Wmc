using System.Collections.Concurrent;
using System.Net.WebSockets;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using Microsoft.Extensions.Logging;

namespace WebsocketDemo.Services
{
    public class WebsocketService
    {
        private readonly ILogger _logger;

        public WebsocketService(ILogger logger)
        {
            _logger = logger;
        }
        // Holds all instances of the connected clients, identified by their guid.
        private readonly ConcurrentDictionary<Guid, WebSocket> _clients = new();


        /// <summary>
        /// Sends a message asynchronously to all clients. At the end we will wait for completition.
        /// </summary>
        private async Task SendToAll(Memory<byte> buffer)
        {
            var clients = _clients.Values;
            Task[] tasks = new Task[clients.Count];
            int i = 0;
            foreach (var socket in clients)
            {
                tasks[i++] = socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None).AsTask();
            }
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Readingloop for one client. Every client is in its own reading loop. This
        /// method is executed in parallel, so it has to be thread safe!
        /// </summary>
        public async Task DoWork(Guid clientGuid, WebSocket webSocket)
        {
            _logger.LogInformation($"Wait for messages from {clientGuid}...");
            _clients.TryAdd(clientGuid, webSocket);

            var buffer = new byte[1024 * 4].AsMemory();
            // Waiting for the first data packet.
            var receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            while (!webSocket.CloseStatus.HasValue)
            {
                var receivedMessage = buffer.Slice(0, receiveResult.Count);
                _logger.LogInformation($"{clientGuid} sent {Encoding.UTF8.GetString(receivedMessage.Span)}");
                // Send the received message to all clients.
                await SendToAll(receivedMessage.Slice(0, receiveResult.Count));
                // Wait for subsequent packets.
                receiveResult = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
            }
            await webSocket.CloseAsync(webSocket.CloseStatus.Value, webSocket.CloseStatusDescription, CancellationToken.None);
            _clients.TryRemove(clientGuid, out _);
            _logger.LogInformation($"Connection to {clientGuid} closed.");
        }
    }
}
