using System.Net.WebSockets;
using System.Text;

var clientHttp = new HttpClient();

using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5182/"), CancellationToken.None);

var buffer = new byte[256];
while (ws.State == WebSocketState.Open)
{

    Console.Write("Insira a requisição ab: [GET]: ");
    var request = Console.ReadLine();
    HttpResponseMessage response = await clientHttp.GetAsync(request);
    response.EnsureSuccessStatusCode();
    string responseBody = await response.Content.ReadAsStringAsync();

    await ws.SendAsync(
        Encoding.ASCII.GetBytes(responseBody),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None);

    var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
    if (result.MessageType == WebSocketMessageType.Close)
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    else
        Console.WriteLine($"request result body: {Encoding.ASCII.GetString(buffer, 0, result.Count)}");
}