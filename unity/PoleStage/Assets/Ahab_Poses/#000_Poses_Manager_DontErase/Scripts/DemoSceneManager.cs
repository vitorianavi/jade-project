using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class DemoSceneManager : MonoBehaviour
{
    [Header("Set manually")]
    public string packTitle;
    public GameObject[] Views;
    float rotationSpeed = 80;
    public Text PosesNames;
    public Text titleText;
    [Header("MINUS 1 (VIEWS NOT POSES)")]
    public int TotalNumberPoses;

    [HideInInspector]
    public int ActualPose;

    [HideInInspector]
    public bool PlusRotationOn;

    [HideInInspector]
    public GameObject[] FramesChildren;

    [HideInInspector]
    public bool firstSet;


    
    
    [Header("Name of the series (f.ex Elders_2_")]
    [Header("++++++++++++ADD+++++++++++++++++++")]
    [Header("CONTROLLERS STUFF")]
    [Space(80)]
    [Header("Used by author. Better not mess with any of the following")]
    [Space(80)]
    public string actualSeries = "Elders_2_";
    [Header("Total number of individual poses")]
    public int totalInThisSeries;
    [Space(10)]
    public bool SetControllers = false;
    [HideInInspector]
    public RuntimeAnimatorController[] ToDistribute;
    [HideInInspector]
    public Animator[] avatars;
    int actualSeriesNumber;
    int currentViewToExtractAvatar;

    [Header("DELETE AVATAR LIST FIRST")]
    [Header("++++++++++++ADD+++++++++++++++++++")]
    [Header("Be sure to put right number")]
    [Space(80)]
    public bool DistributePoseDatas = false;
    public int PoseDataDigitToStartWith;
    [Header("+++++++++++++++++REMOVE++++++++++++++++++++")]
    [Header("be sure to have assigned tags manually and other info")]
    [Header("Finally write info in Pose Datas")]
    [Space(80)]
    public bool WriteInfIntoPoseData = false;
    public GameObject[] avatars2;
    int currentViewToExtractAvatar2;

    void Start()
    {
        
        LoadControllersToDistribute();
        DistributePoseDatasToViews();

        titleText.text = packTitle;
        ActualPose = 0;
        for (int i = 0; i < Views.Length; i++)
        {
            if (i != ActualPose)
            {
                Views[i].SetActive(false);
            }
            else if (i == ActualPose)
            {
                Views[i].SetActive(true);
            }
        }
        ChildrenList();
    }

    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            if(PlusRotationOn == false)
            {
                PlusRotationOn = true;
            }
            else if (PlusRotationOn == true)
            {
                PlusRotationOn = false;
            }
        }
        else if (Input.GetKeyUp("right"))
        {
            ChangePoseForward();
        }
        else if (Input.GetKeyUp("left"))
        {
            ChangePoseBackward();
        }

        PlusRotation();
        WritePoseData();

    }

    void PlusRotation()
    {
        if(PlusRotationOn == true)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        
    }
    public void ChangePoseForward()
    {
        if(ActualPose < TotalNumberPoses)
        {
            ActualPose++;
        }
        else if (ActualPose == TotalNumberPoses)
        {
            ActualPose=0;
        }
        for (int i = 0; i < Views.Length; i++)
        {
            if (i != ActualPose)
            {
                Views[i].SetActive(false);
            }
            else if (i == ActualPose)
            {
                Views[i].SetActive(true);
            }
        }
        ChildrenList();
    }

    public void ChangePoseBackward()
    {
        if (ActualPose > 0)
        {
            ActualPose--;
        }
        else if (ActualPose == 0)
        {
            ActualPose = TotalNumberPoses;
        }
        for (int i = 0; i < Views.Length; i++)
        {
            if (i != ActualPose)
            {
                Views[i].SetActive(false);
            }
            else if (i == ActualPose)
            {
                Views[i].SetActive(true);
            }
        }
        ChildrenList();
    }

    void ChildrenList()
    {
        firstSet = false;
        PosesNames.text = "";
        FramesChildren = new GameObject[Views[ActualPose].transform.childCount];
        for (int i = 0; i < Views[ActualPose].transform.childCount; i++)
        {
            FramesChildren[i] = Views[ActualPose].transform.GetChild(i).gameObject;
        }

        for (int a = 0; a < FramesChildren.Length; a++)
        {
            if(FramesChildren[a].tag != "MainCamera")
            {
                Animator anim = FramesChildren[a].GetComponent<Animator>();
                string temp = anim.runtimeAnimatorController.name;

                if (firstSet == false)
                {
                    firstSet = true;
                    PosesNames.text = temp;
                }

                else if (firstSet == true)
                {
                    PosesNames.text = PosesNames.text + " & " + temp;
                }
            }
            

        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    void LoadControllersToDistribute()
    {
        if (SetControllers == true)
        {
            ToDistribute = new RuntimeAnimatorController[] { };
            avatars = new Animator[] { };
            //REMEMBER TO PUT THEM FIRST AT "RESOURCES/CONTROLLERS" -> NOTE FOR MYSELF
            actualSeriesNumber = 1;
            currentViewToExtractAvatar = 0;
            for (int i = 0; i < totalInThisSeries; i++)
            {
                Array.Resize(ref ToDistribute, ToDistribute.Length + 1);
                ToDistribute[ToDistribute.Length - 1] = Resources.Load<RuntimeAnimatorController>("Controllers/" + actualSeries + actualSeriesNumber.ToString());
                actualSeriesNumber++;

            }

            //

            //RECOLLECTING AVATARS
            for (int j = 0; j < Views.Length; j++)
            {
                for (int i = 0; i < Views[currentViewToExtractAvatar].transform.childCount; i++)
                {
                    if (Views[currentViewToExtractAvatar].transform.GetChild(i).gameObject.tag != "MainCamera")
                    {
                        Array.Resize(ref avatars, avatars.Length + 1);
                        avatars[avatars.Length - 1] = Views[currentViewToExtractAvatar].transform.GetChild(i).gameObject.GetComponent<Animator>();
                    }
                }
                currentViewToExtractAvatar++;
            }

            //SETTING NEW AVATARS
            for (int g = 0; g < avatars.Length; g++)
            {
                avatars[g].runtimeAnimatorController = ToDistribute[g];
            }

            SetControllers = false;
        }
    }

    void DistributePoseDatasToViews()
    {
        if(DistributePoseDatas == true)
        {
            currentViewToExtractAvatar2 = 0;

            for (int j = 0; j < Views.Length; j++)
            {
                for (int i = 0; i < Views[currentViewToExtractAvatar2].transform.childCount; i++)
                {
                    if (Views[currentViewToExtractAvatar2].transform.GetChild(i).gameObject.tag != "MainCamera")
                    {
                        Array.Resize(ref avatars2, avatars2.Length + 1);
                        avatars2[avatars2.Length - 1] = Views[currentViewToExtractAvatar2].transform.GetChild(i).gameObject;
                        avatars2[avatars2.Length - 1].GetComponent<PoseDataSetter>().AssignedPoseData = Resources.Load<PoseData>("AhabDev/PoseDatasTEMP/PoseData"+ PoseDataDigitToStartWith.ToString());
                        PoseDataDigitToStartWith++;
                    }
                }
                currentViewToExtractAvatar2++;
            }

            currentViewToExtractAvatar2 = 0;
            DistributePoseDatas = false;
        }
    }

    void WritePoseData()
    {
        if(WriteInfIntoPoseData == true)
        {

            for (int i = 0; i < avatars2.Length; i++)
            {
                if(avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.AlreadySet == false)//ONLY WORKS IF HAD NOT BEEN WRITEN BEFORE->NOTE FOR MYSELF
                {
                    //AVATAR BASIC DATA
                    avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.AvatarPosition = avatars2[i].transform.position;
                    avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.AvatarRotation = avatars2[i].transform.rotation;
                    avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.animatorController = avatars2[i].GetComponent<Animator>().runtimeAnimatorController;

                    //TAGS DATA
                    #region settags
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag1 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag1;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag2 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag2;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag3 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag3;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag4 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag4;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag5 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag5;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag6 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag6;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag7 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag7;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().tag8 != "")
                    {

                        Array.Resize(ref avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags, avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length + 1);
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags[avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.Tags.Length - 1] = avatars2[i].GetComponent<PoseDataSetter>().tag8;
                    }
                    #endregion

                    //PROP DATA
                    if (avatars2[i].GetComponent<PoseDataSetter>().usesprop == true)
                    {
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.UsesAProp = true;
                        avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.poseProp = Resources.Load<GameObject>("AhabDev/PoseDatas/" + avatars2[i].GetComponent<PoseDataSetter>().propname);
                        GameObject parentObject = avatars2[i].transform.parent.gameObject;
                        for (int g = 0; g < parentObject.transform.childCount; g++)
                        {
                            if (parentObject.transform.GetChild(g).gameObject.tag == "MainCamera")
                            {
                                avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.PropPosition = parentObject.transform.GetChild(g).gameObject.transform.position;
                                avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.PropRotation = parentObject.transform.GetChild(g).gameObject.transform.rotation;
                                avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.PropSize = parentObject.transform.GetChild(g).gameObject.transform.localScale;
                            }
                        }

                    }

                    //SET ASSOCIATED POSEDATAS
                    if (avatars2[i].GetComponent<PoseDataSetter>().two == true)
                    {
                        PoseDataSetter posDatSet = avatars2[i].GetComponent<PoseDataSetter>();
                        posDatSet.AssignedPoseData.PoseData2 = posDatSet.AssociatePoseDataSetter2.AssignedPoseData;
                        posDatSet.AssignedPoseData.currentNecessaryAvatars = NecessaryAvatars.TWO;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().three == true)
                    {
                        PoseDataSetter posDatSet = avatars2[i].GetComponent<PoseDataSetter>();
                        posDatSet.AssignedPoseData.PoseData2 = posDatSet.AssociatePoseDataSetter2.AssignedPoseData;
                        posDatSet.AssignedPoseData.PoseData3 = posDatSet.AssociatePoseDataSetter3.AssignedPoseData;
                        posDatSet.AssignedPoseData.currentNecessaryAvatars = NecessaryAvatars.THREE;
                    }
                    if (avatars2[i].GetComponent<PoseDataSetter>().four == true)
                    {
                        PoseDataSetter posDatSet = avatars2[i].GetComponent<PoseDataSetter>();
                        posDatSet.AssignedPoseData.PoseData2 = posDatSet.AssociatePoseDataSetter2.AssignedPoseData;
                        posDatSet.AssignedPoseData.PoseData3 = posDatSet.AssociatePoseDataSetter3.AssignedPoseData;
                        posDatSet.AssignedPoseData.PoseData4 = posDatSet.AssociatePoseDataSetter4.AssignedPoseData;
                        posDatSet.AssignedPoseData.currentNecessaryAvatars = NecessaryAvatars.FOUR;
                    }

                    avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData.AlreadySet = true;

                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(avatars2[i].GetComponent<PoseDataSetter>().AssignedPoseData);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                #endif



                }//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ONLY WORKS IF HAD NOT BEEN WRITEN BEFORE->NOTE FOR MYSELF

            }
            WriteInfIntoPoseData = false;

            print("done");
        }
        
    }

    
}
