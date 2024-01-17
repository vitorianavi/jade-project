using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Video;
using System.Threading;
using WebSocketSharp;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Text;

public class VideoStreamControl : MonoBehaviour
{
    // This will be replaced by a webcam object
    public VideoPlayer VideoPlayer;
    private RenderTexture videoTexture;

    private WebSocketManager webSocketManager;
    private const string WsAddress = "ws://localhost:8080/poseframe";
    private bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        webSocketManager = new WebSocketManager(WsAddress);

        videoTexture = Resources.Load<RenderTexture>("Textures/LiveCameraVideo");

        InvokeRepeating("sendVideoStream", 0f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void sendVideoStream()
    {
        if (VideoPlayer.isPlaying)
        {
            if (!started)
            {
                byte[] array = Encoding.ASCII.GetBytes("start");
                webSocketManager.SendAsync(array);
                started = true;
            }
            captureFrame(videoTexture);
            
        } else
        {
            started = false;
        }
    }

    void captureFrame(RenderTexture renderTexture)
    {
        AsyncGPUReadback.Request(renderTexture, 0, result =>
        {
            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture.Apply();
            RenderTexture.active = null;

            byte[] imgData = texture.EncodeToJPG();
            Destroy(texture);
            webSocketManager.SendAsync(imgData);
        });
    }

    void OnDestroy()
    {
        webSocketManager.Close();
    }
}
