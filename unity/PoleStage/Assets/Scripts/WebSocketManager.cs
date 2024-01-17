using UnityEngine;
using System.Collections;
using WebSocketSharp;
using System.Threading.Tasks;

public class WebSocketManager
{
    private WebSocket webSocket;

    public WebSocketManager(string url)
    {
        webSocket = new WebSocket(url);
        webSocket.OnOpen += (sender, e) => Debug.Log("Connection opened");
        webSocket.ConnectAsync();
    }

    public Task SendAsync(byte[] message)
    {
        var tcs = new TaskCompletionSource<bool>();

        webSocket.SendAsync(message, completed =>
        {
            if (completed)
            {
                tcs.SetResult(true);
            }
            else
            {
                tcs.SetResult(false);
            }
        });

        return tcs.Task;
    }

    public void Close()
    {
        if (webSocket != null)
        {
            webSocket.CloseAsync();
            webSocket = null;
        }
    }
}