using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class RecordingButtonBehavior : MonoBehaviour
{
    public Animator RecordAnimator;
    public Button RecordButton;
    public Text RecordButtonText;
    public Button ReplayButton;
    public Button NextPlayerButton;
    public VideoPlayer VideoPlayer;
    public Button TryAgainButton;

    private Text buttonText;
    public Image IconRight;
    public Image IconWrong;

    private bool isRecording = false;
    private bool rightEnabled = false;
    private bool wrongEnabled = false;
    public float delay = 2f; // Set the delay before animation in seconds
    private float globalTimerRight = 0f;
    private float playTimerRight = 0f;
    private float globalTimerWrong = 0f;
    private float playTimerWrong = 0f;
    private float[] rightDelays1 = { 11f, 15f, 21f, 25f};
    private float[] rightDelays2 = { 9f, 16f, 19f, 24f};
    private float wrongDelay = 31f;
    private int rightDelayIdx = 0;
    private int currentPlayer = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isRecording && VideoPlayer.isPlaying) { 
            if (rightEnabled == false)
            {
                if (currentPlayer == 1)
                {
                    if (rightDelayIdx < rightDelays1.Length)
                    {
                        globalTimerRight += Time.deltaTime;

                        if (globalTimerRight >= rightDelays1[rightDelayIdx])
                        {
                            IconRight.gameObject.SetActive(true);
                            rightEnabled = true;
                            playTimerRight = 0f;
                        }
                    }
                } else
                {
                    if (rightDelayIdx < rightDelays2.Length)
                    {
                        globalTimerRight += Time.deltaTime;

                        if (globalTimerRight >= rightDelays2[rightDelayIdx])
                        {
                            IconRight.gameObject.SetActive(true);
                            rightEnabled = true;
                            playTimerRight = 0f;
                        }
                    }
                }
            } else
            {
                playTimerRight += Time.deltaTime;
                if (playTimerRight >= 1)
                {
                    IconRight.gameObject.SetActive(false);
                    rightEnabled = false;
                    playTimerRight = 0f;
                    rightDelayIdx += 1;
                }
            }

            if (wrongEnabled==false)
            {
                globalTimerWrong += Time.deltaTime;
                

                if (globalTimerWrong >= wrongDelay)
                {
                    IconWrong.gameObject.SetActive(true);
                    wrongEnabled = true;
                    playTimerWrong = 0f;
                    TryAgainButton.gameObject.SetActive(true);
                }
            }
            else
            {
                playTimerWrong += Time.deltaTime;
                if (playTimerWrong >= 1)
                {
                    IconWrong.gameObject.SetActive(false);
                    wrongEnabled = false;
                    playTimerWrong = 0f;
                }
            }
        }
    }

    public void OnClick()
    {
        if (isRecording)
        {
            isRecording = false;
            VideoPlayer.Stop();
            RecordButton.gameObject.SetActive(false);
            ReplayButton.gameObject.SetActive(true);
            NextPlayerButton.gameObject.SetActive(true);
        }
        else
        {
            RecordButtonText.text = "STOP RECORDING";
            isRecording = true;
            VideoPlayer.Play();
            globalTimerRight = 0f;
            rightDelayIdx = 0;
            globalTimerWrong = 0f;
            if (currentPlayer==2 || currentPlayer==0)
            {
                currentPlayer = 1;
            }
            else if (currentPlayer==1)
            {
                currentPlayer = 2;
            }
        }
    }
}
