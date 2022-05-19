using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseWebSockets();


var buffer = new Byte[256];

app.Map("/", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    else
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        while (true)
        {
            var receiveResultBody = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            var body = Encoding.ASCII.GetString(buffer, 0, receiveResultBody.Count);

            await webSocket.SendAsync(
                Encoding.ASCII.GetBytes(body),
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);

            await Task.Delay(1000);
        }
    }
});



await app.RunAsync();
