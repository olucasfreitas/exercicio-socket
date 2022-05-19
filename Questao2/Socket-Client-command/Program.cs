using System.Net.WebSockets;
using System.Text;

using var ws = new ClientWebSocket();
await ws.ConnectAsync(new Uri("ws://localhost:5274/"), CancellationToken.None);

var buffer = new byte[256];
while (ws.State == WebSocketState.Open)
{
    Console.Write("Digite um comando:");
    var comando = Console.ReadLine();

    Console.WriteLine($"Comando digitado: {comando}");

    await ws.SendAsync(
        Encoding.ASCII.GetBytes(comando),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None);

    var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
    if (result.MessageType == WebSocketMessageType.Close)
        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
    else
        Console.WriteLine($"[RETORNO SERVER]: {Encoding.ASCII.GetString(buffer, 0, result.Count)}");
}