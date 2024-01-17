using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Server;

// Pose states
public static class PoseState
{
    public const string NotPose = "not_pose";
    public const string SadGirl = "sad_girl";
    public const string RemiSeat = "remi_seat";
    public const string RemiLayback = "remi_layback";
    public const string Seat = "seat";
    public const string Invert = "invert";
    public const string Superman = "superman";
    public const string Cupid = "cupid";
    public const string Butterfly = "butterfly";
    public const string MermaidSeat = "mermaid_seat";
    public const string Vortex = "vortex";
    public const string Embrace = "embrace";
}

public class ServerControl : MonoBehaviour
{
    // constants
    private const int NProjections = 1;

    // Websocket to connect to Arduino
    private WebSocket wsDmx;
    private const string WsDmxAddress = "ws://10.72.91.146:8081/dmx";

    // Websocket to connect to ML module
    private WebSocket wsPose;
    private const string WsPoseAddress = "ws://localhost:8080/prediction";

    // Light and video wall objects
    private Light sideBackLight;
    private Light sideFrontLight;
    private Light topLight;
    private Renderer videoWall;

    // Projection textures
    private RenderTexture[] projections = new RenderTexture[NProjections];

    // Light setups
    // R, G, B, intensity, pulse:1=true/0=false, pulse delay in ms
    private byte[] pinkConstant = { 199, 21, 133, 255, 0, 100, 0, 200 };
    private byte[] blueConstant = { 30, 144, 255, 255, 0, 100, 0, 200 };
    private byte[] off = { 0, 0, 0, 0, 0, 0, 0 };

    // States
    private string currentPoseState = PoseState.NotPose;

    void Start()
	{
        topLight = GameObject.Find("TopSpotLight").GetComponent<Light>();
        sideBackLight = GameObject.Find("SideBackSpotLight").GetComponent<Light>();
        sideFrontLight = GameObject.Find("SideFrontSpotLight").GetComponent<Light>();
        videoWall = GameObject.Find("Screen").GetComponent<Renderer>();

        for (int i=0; i < NProjections; i++)
        {
            string projPath = "Textures/ProjectionRender" + (i+1).ToString();
            projections[i] = Resources.Load<RenderTexture>(projPath);
        }

        topLight.enabled = false;
        sideBackLight.enabled = false;
        sideFrontLight.enabled = false;
        SwitchVideoWall(false);
        SwitchLight(topLight, false, null, true);

        // Connect to ML API
        wsPose = new WebSocket(WsPoseAddress);
        wsPose.OnMessage += (sender, e) =>
        {
            UnityMainThreadDispatcher.ExecuteOnMainThread(() =>
            {
                // This function will run on the main thread
                UpdateLightsAndVideoWall(e.Data);
            });
        };
        wsPose.Connect();

        // Connect to Arduino ESP32
        wsDmx = new WebSocket(WsDmxAddress);

        wsDmx.Connect();
    }

    void UpdateLightsAndVideoWall(string data)
    {
        Debug.Log(data);
        switch (data)
        {
            case PoseState.SadGirl:
                if (currentPoseState != PoseState.SadGirl)
                {
                    SwitchVideoWall(true);
                    SwitchLight(topLight, false, null, true);
                    currentPoseState = PoseState.SadGirl;
                }
                break;
            case PoseState.RemiSeat:
                if (currentPoseState != PoseState.RemiSeat)
                {
                    SwitchLight(topLight, true, pinkConstant, true);
                    //SetProjection(2);
                    SwitchVideoWall(true);
                    currentPoseState = PoseState.RemiSeat;
                }
                break;
            case PoseState.RemiLayback:
                if (currentPoseState != PoseState.RemiLayback)
                {
                    SwitchLight(topLight, false, null, true);
                    SwitchLight(sideBackLight, true, pinkConstant, false);
                    SwitchLight(sideFrontLight, true, blueConstant, false);
                    currentPoseState = PoseState.RemiLayback;
                }
                break;
            case PoseState.Seat:
                if (currentPoseState != PoseState.Seat)
                {
                    SwitchLight(topLight, true, blueConstant, true);
                    SwitchLight(sideBackLight, false, null, false);
                    SwitchLight(sideFrontLight, false, null, false);
                    SwitchVideoWall(true);
                    currentPoseState = PoseState.Seat;
                }
                break;
            case PoseState.Cupid:
                if (currentPoseState != PoseState.Cupid)
                {
                    SwitchLight(topLight, false, null, true);
                    SwitchLight(sideBackLight, false, null, false);
                    SwitchLight(sideFrontLight, false, null, false);
                    currentPoseState = PoseState.Cupid;
                }
                break;
        }
        
    }

    void SwitchLight(Light light, bool status, byte[] lightSetup, bool sendDmx)
    {
        byte[] lightCmd = off;

        light.enabled = status;
        if (status == true && lightSetup != null)
        {
            light.color = new Color32(lightSetup[0], lightSetup[1], lightSetup[2], lightSetup[3]);
            lightCmd = lightSetup;
        }

        if (sendDmx && wsDmx != null) {
            wsDmx.Send(lightCmd);
        }
    }

    void SwitchVideoWall(bool status)
    {
        if (status == true)
        {
            videoWall.material.EnableKeyword("_EMISSION");
        } else
        {
            videoWall.material.DisableKeyword("_EMISSION");
        }
    }

    void SetProjection(int id)
    {
        if (id>0)
        {
            videoWall.material.SetTexture("_MainTex", projections[id - 1]);
            videoWall.material.SetTexture("_EmissionMap", projections[id - 1]);
        }   
    }

    void OnDestroy()
    {
        if (wsPose != null)
        {
            wsPose.Close();
        }

        if (wsDmx != null)
        {
            wsDmx.Close();
        }
    }

    void Update()
	{
		
	}
}
