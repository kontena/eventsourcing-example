using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Kontena.EventSourcing;
using Kontena.Dashboard.Models;

namespace Kontena.Dashboard.Middleware
{
    public class WebsocketPubSubMiddleware
    {
        private readonly RequestDelegate _next;

        public WebsocketPubSubMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path == "/ws")
            {
                if (httpContext.WebSockets.IsWebSocketRequest)
                {
                    var customerSubscriber = httpContext.RequestServices.GetService<PersistingEventSubscriber<Customer, EventBusConfig<Customer>>>();
                    var productSubscriber = httpContext.RequestServices.GetService<PersistingEventSubscriber<Product, EventBusConfig<Product>>>();
                    var purchaseSubscriber = httpContext.RequestServices.GetService<PersistingEventSubscriber<Purchase, EventBusConfig<Purchase>>>();

                    var buffer = new byte[1024 * 4];
                    var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();
                    var result = (WebSocketReceiveResult)null;

                    EventHandler<ReceivedRawEventArgs> eventReceived = async (sender, args) =>
                    {
                        await NotifySocket(webSocket, args.Event);
                    };

                    try
                    {
                        customerSubscriber.RawEventReceived += eventReceived;
                        productSubscriber.RawEventReceived += eventReceived;
                        purchaseSubscriber.RawEventReceived += eventReceived;

                        do
                        {
                            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                        }
                        while (!result.CloseStatus.HasValue);
                    }
                    finally
                    {
                        customerSubscriber.RawEventReceived -= eventReceived;
                        productSubscriber.RawEventReceived -= eventReceived;
                        purchaseSubscriber.RawEventReceived -= eventReceived;

                        if (result != null)
                        {
                            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        }
                    }
                }
                else
                {
                    httpContext.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(httpContext);
            }
        }

        private async Task NotifySocket(WebSocket webSocket, string payload)
        {
            var buffer = Encoding.UTF8.GetBytes(payload);

            await webSocket.SendAsync(
                new ArraySegment<byte>(buffer, 0, buffer.Length),
                WebSocketMessageType.Binary,
                true,
                CancellationToken.None);
        }
    }

    public static class WebsocketPubSubMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebsocketPubSub(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebsocketPubSubMiddleware>();
        }
    }
}