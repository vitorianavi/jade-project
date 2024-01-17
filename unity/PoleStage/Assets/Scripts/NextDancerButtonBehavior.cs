using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class NextDancerButtonBehavior : MonoBehaviour
{
    public Button NextButton;
    public VideoPlayer VideoPlayer;
    public VideoClip[] VideoClips;
    public Button RecordingButton;
    public Button PlayButton;
    public Sprite DancerSprite1;
    public Sprite DancerSprite2;

    private Image dancerIcon;
    private Text dancerIconText;
    private Text recordBtnText;
    private int nextVideoIdx = 0;
    private int currentDancer = 1;

    // Start is called before the first frame update
    void Start()
    {
        recordBtnText = RecordingButton.GetComponentInChildren<Text>();

        dancerIcon = GameObject.Find("PlayerIconAvatar").GetComponent<Image>();
        dancerIconText = GameObject.Find("PlayerIconText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        changeVideo();
        PlayButton.gameObject.SetActive(false);
        if (recordBtnText != null)
        {
            recordBtnText.text = "START RECORDING";
        }

        RecordingButton.gameObject.SetActive(true);
        NextButton.gameObject.SetActive(false);

        if (currentDancer==1)
        {
            dancerIcon.sprite = DancerSprite2;
            dancerIconText.text = "DANCER 2";
            currentDancer = 2;
        } else
        {
            dancerIcon.sprite = DancerSprite1;
            dancerIconText.text = "DANCER 1";
            currentDancer = 1;
        }
    }

    private void changeVideo()
    {
        if (VideoPlayer != null && nextVideoIdx < VideoClips.Length)
        {
            VideoPlayer.clip = VideoClips[nextVideoIdx];
            VideoPlayer.Play();
            VideoPlayer.Pause();
            nextVideoIdx++;
        }
    }
}
