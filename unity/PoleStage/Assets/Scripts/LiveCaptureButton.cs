using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LiveCaptureButton : MonoBehaviour
{
    public Button CaptureButton;
    public Button StopButton;
    public VideoPlayer VideoPlayer;
    public GameObject LiveCameraFrame;

    private Image liveCameraFrameBorder;

    // Start is called before the first frame update
    void Start()
    {
        liveCameraFrameBorder = LiveCameraFrame.GetComponentInChildren<Image>();
    }

    public void OnCaptureClick()
    {
        CaptureButton.gameObject.SetActive(false);
        StopButton.gameObject.SetActive(true);
        LiveCameraFrame.SetActive(true);
        liveCameraFrameBorder.gameObject.SetActive(true);

        VideoPlayer.Prepare();
        VideoPlayer.Play();
    }

    public void OnStopClick()
    {
        StopButton.gameObject.SetActive(false);
        CaptureButton.gameObject.SetActive(true);
        LiveCameraFrame.SetActive(false);
        liveCameraFrameBorder.gameObject.SetActive(false);

        VideoPlayer.Stop();
    }
}
