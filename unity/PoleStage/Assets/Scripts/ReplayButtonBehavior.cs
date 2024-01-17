using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ReplayButtonBehavior : MonoBehaviour
{
    public Button ReplayButton;
    public VideoPlayer VideoPlayer;
    public Sprite PlaySprite;

    private bool isPaused = true;

    // Start is called before the first frame update
    void Start()
    {
        VideoPlayer.loopPointReached += OnVideoEnded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnReplayClick()
    {
        if (isPaused)
        {
            VideoPlayer.Play();
            isPaused = false;
        } else
        {
            VideoPlayer.Pause();
            isPaused = true;
        }
    }

    void OnVideoEnded(VideoPlayer vp)
    {
        Image buttonImage = ReplayButton.GetComponent<Image>();
        Debug.Log(buttonImage);
        if (buttonImage != null)
        {
            buttonImage.sprite = PlaySprite;
            isPaused = true;
        }
    }
}
