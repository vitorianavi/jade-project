using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public enum AvatarColorInManager
{
    RED,
    BLUE
}

public enum TypeOfSearh
{
    SINGLE,
    DOUBLE
}

public enum AuxQuantity
{
    NONE,
    ONE,
    TWO,
    THREE
}

public class PosesManagerController : MonoBehaviour
{
    [Header("AVATARS")]
    [HideInInspector]
    public AvatarColorInManager SelectedAvatarColorInManager;
    public GameObject AvatarRed;
    public GameObject AvatarBLUE;
    public GameObject AvatarAuxiliar1;
    public GameObject AvatarAuxiliar2;
    public GameObject AvatarAuxiliar3;
    [HideInInspector]
    public bool avatarHasPoseApplied;
    [Header("TAG LIST CONTROLLERS")]
    public TagList1Controller myTagList1Controller;
    public TagList2Controller myTagList2Controller;
    public InputField Tag1InputField;
    public InputField Tag2InputField;
    public InputField AddTagInputField;
    public InputField RemoveTagInputField;
    public InputField SubstituteAllInputField;
    public InputField NewTagInputField;
    [Space(40)]
    public Text LastActionMessage;
    public Text TagsText;
    public Text currentPoseText;
    [Space(40)]
    [SerializeField]
    private GameObject buttonTemplate;

    [SerializeField]
    private GameObject buttonTemplate2;
    [HideInInspector]
    public PoseData currentViewedPoseData;
    private List<GameObject> buttons;
    bool foundtag1 = false;


    [Header("Tag List Build Elements")]
    public object[] Datas;
    [Header("Set all Manually to zero")]
    public string[] tempTags;

    TagMonitor myTagMonitor;
    bool repeated = false;
    PoseData[] Results;
    PoseData[] Results2;
    [HideInInspector]
    public string[] temptags;
    public GameObject TagList1ButtonListContent;
    public GameObject TagList2ButtonListContent;
    public GameObject ButtonListContent;
    [HideInInspector]
    public GameObject[] ButtonsFound;
   [HideInInspector]
    public string[] ButtonsFoundTitles;
    [HideInInspector]
    public int currentButton;
    [HideInInspector]
    public GameObject[] ButtonsFound2;
    [HideInInspector]
    public TypeOfSearh currentTypeOfSearch;
    [HideInInspector]
    public AuxQuantity currentAuxQuantity;
    [HideInInspector]
    public bool FirstSearch = false;
    public GameObject NecessaryProp;
    [HideInInspector]
    public bool PlusRotationOn = false;
    [HideInInspector]
    public float rotationSpeed = 80;
    public RuntimeAnimatorController TPose;

    void Start()
    {
        rotationSpeed = 80;
        avatarHasPoseApplied = false;
        FirstSearch = false;
        BuildTagList();
        ColorAvatarActivatorInManagerRed();
        InitiateTagList();
    }

    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            if (PlusRotationOn == false)
            {
                PlusRotationOn = true;
            }
            else if (PlusRotationOn == true)
            {
                PlusRotationOn = false;
            }
        }
        PlusRotation();
        NavigateFast();
    }

    void PlusRotation()
    {
        if (PlusRotationOn == true)
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

    }


    public void BuildTagList()
    {
        myTagMonitor = Resources.Load<TagMonitor>("AhabDev/PoseDatas/TagMonitor");

        if(myTagMonitor == null)
        {
            print("Please send the contents of <<SendToResources>> folder to your project's <<Resouces>> folder. If you don't know how to follow this step check the Read me doc ");
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
        else
        {
            //EMPTY AL ARRAYS
            Datas = new object[] { };

            if (tempTags.Length > 0)
            {
                tempTags = new string[] { };
            }

            if (myTagMonitor.CurrentTags.Length > 0)
            {
                myTagMonitor.CurrentTags = new string[] { };

            }





            Datas = Resources.LoadAll("AhabDev/PoseDatas", typeof(PoseData));
            for (int i = 0; i < Datas.Length; i++)
            {
                PoseData tempPoseData = (PoseData)Datas[i];
                for (int g = 0; g < tempPoseData.Tags.Length; g++)
                {
                    Array.Resize(ref tempTags, tempTags.Length + 1);
                    tempTags[tempTags.Length - 1] = tempPoseData.Tags[g];
                }
            }

            for (int h = 0; h < tempTags.Length; h++)
            {
                repeated = false;
                for (int j = 0; j < myTagMonitor.CurrentTags.Length; j++)
                {
                    if (myTagMonitor.CurrentTags[j] == tempTags[h])
                    {
                        repeated = true;
                    }
                }
                if (repeated == false)
                {
                    Array.Resize(ref myTagMonitor.CurrentTags, myTagMonitor.CurrentTags.Length + 1);
                    myTagMonitor.CurrentTags[myTagMonitor.CurrentTags.Length - 1] = tempTags[h];
                }
                else
                {
                    repeated = false;
                }
            }
        }
    }

    public void InitiateTagList()
    {
        myTagList1Controller.enabled = true;
        myTagList2Controller.enabled = true;
    }//ACTIVATES ONLY AT START

    public void ColorAvatarActivatorInManagerBlue()
    {
        if(avatarHasPoseApplied == false)
        {
            if (SelectedAvatarColorInManager != AvatarColorInManager.BLUE)
            {
                SelectedAvatarColorInManager = AvatarColorInManager.BLUE;
            }

            if (SelectedAvatarColorInManager == AvatarColorInManager.BLUE)
            {
                if (AvatarBLUE.activeSelf == false)
                {
                    AvatarRed.SetActive(false);
                    AvatarBLUE.SetActive(true);
                }
            }
        }

        else if (avatarHasPoseApplied == true)
        {
            if (SelectedAvatarColorInManager != AvatarColorInManager.BLUE)
            {
                SelectedAvatarColorInManager = AvatarColorInManager.BLUE;
            }

            if (SelectedAvatarColorInManager == AvatarColorInManager.BLUE)
            {
                if (AvatarBLUE.activeSelf == false)
                {
                    RuntimeAnimatorController runAnimConR = AvatarRed.GetComponent<Animator>().runtimeAnimatorController;
                    Vector3 redPos = AvatarRed.transform.position;
                    Quaternion redRot = AvatarRed.transform.rotation;
                    AvatarRed.SetActive(false);
                    AvatarBLUE.SetActive(true);
                    AvatarBLUE.GetComponent<Animator>().runtimeAnimatorController = runAnimConR;
                    AvatarBLUE.transform.position = redPos;
                    AvatarBLUE.transform.rotation = redRot;
                }
            }
        }

    } 


    public void ColorAvatarActivatorInManagerRed()
    {
        if (avatarHasPoseApplied == false)
        {
            if (SelectedAvatarColorInManager != AvatarColorInManager.RED)
            {
                SelectedAvatarColorInManager = AvatarColorInManager.RED;
            }

            if (SelectedAvatarColorInManager == AvatarColorInManager.RED)
            {
                if (AvatarRed.activeSelf == false)
                {
                    AvatarRed.SetActive(true);
                    AvatarBLUE.SetActive(false);
                }
            }
        }

        else if (avatarHasPoseApplied == true)
        {
            if (SelectedAvatarColorInManager != AvatarColorInManager.RED)
            {
                SelectedAvatarColorInManager = AvatarColorInManager.RED;
            }

            if (SelectedAvatarColorInManager == AvatarColorInManager.RED)
            {
                if (AvatarRed.activeSelf == false)
                {
                    RuntimeAnimatorController runAnimConB = AvatarBLUE.GetComponent<Animator>().runtimeAnimatorController;
                    Vector3 bluePos = AvatarBLUE.transform.position;
                    Quaternion blueRot = AvatarBLUE.transform.rotation;
                    AvatarRed.SetActive(true);
                    AvatarBLUE.SetActive(false);
                    AvatarRed.GetComponent<Animator>().runtimeAnimatorController = runAnimConB;
                    AvatarRed.transform.position = bluePos;
                    AvatarRed.transform.rotation = blueRot;
                }
            }
        }

    } 

    public void SingleSearch()
    {
        if (currentAuxQuantity == AuxQuantity.NONE)
        {

        }
        else if (currentAuxQuantity == AuxQuantity.ONE)
        {
            Destroy(ButtonsFound2[0]);
        }
        else if (currentAuxQuantity == AuxQuantity.TWO)
        {
            Destroy(ButtonsFound2[0]);
            Destroy(ButtonsFound2[1]);
        }
        else if (currentAuxQuantity == AuxQuantity.THREE)
        {
            Destroy(ButtonsFound2[0]);
            Destroy(ButtonsFound2[1]);
            Destroy(ButtonsFound2[2]);
        }

        if (NecessaryProp != null)
        {
            Destroy(GameObject.Find(NecessaryProp.name + "(Clone)"));
        }

        if (SelectedAvatarColorInManager == AvatarColorInManager.BLUE)
        {
            AvatarBLUE.GetComponent<Animator>().runtimeAnimatorController = TPose;
            currentViewedPoseData = null;
            AvatarBLUE.transform.position = Vector3.zero;
            AvatarBLUE.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (SelectedAvatarColorInManager == AvatarColorInManager.RED)
        {
            AvatarRed.GetComponent<Animator>().runtimeAnimatorController = TPose;
            currentViewedPoseData = null;
            AvatarRed.transform.position = Vector3.zero;
            AvatarRed.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        currentPoseText.text = "";
        TagsText.text = "";

        AvatarAuxiliar1.SetActive(false);
        AvatarAuxiliar2.SetActive(false);
        AvatarAuxiliar3.SetActive(false);


        if (Tag1InputField.text == "")
        {
            LastActionMessage.text = "PLEASE ADD A TAG IN THE INPUTFIELD BEFORE DOING THE SEARCH";
        }
        else
        {

            if (FirstSearch == true)
            {

                for (int c = 0; c < ButtonsFound.Length; c++)
                {
                    Destroy(ButtonsFound[c]);

                }
                ButtonsFound = new GameObject[] { };

            }

            currentTypeOfSearch = TypeOfSearh.SINGLE;

            Tag1InputField.text = Tag1InputField.text.ToUpper();

            string tag1 = Tag1InputField.text;
            int tempResult = 0;
            foundtag1 = false;
            Results = new PoseData[] { };


            buttons = new List<GameObject>();

            if (buttons.Count > 0)
            {
                print("found something");
                foreach (GameObject button in buttons)
                {
                    Destroy(button.gameObject);
                }
                buttons.Clear();
            }

            //EMPTY AL ARRAYS
            Datas = new object[] { };

            if (tempTags.Length > 0)
            {
                tempTags = new string[] { };
            }

            //GET AL POSEDATAS
            Datas = Resources.LoadAll("AhabDev/PoseDatas", typeof(PoseData));


            for (int i = 0; i < Datas.Length; i++)
            {
                PoseData tempPoseData = (PoseData)Datas[i];
                for (int g = 0; g < tempPoseData.Tags.Length; g++)
                {
                    if(tempPoseData.Tags[g] == tag1)
                    {
                        foundtag1 = true;
                    }
                    
                }

                if(foundtag1 == true)
                    {
                        Array.Resize(ref Results, Results.Length + 1);
                        Results[Results.Length - 1] = tempPoseData;
                    }

                foundtag1 = false;
            }

            //for (int i = 0; i < myTagMonitor.CurrentTags.Length; i++)
            foreach (PoseData i in Results)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);
                button.GetComponent<ButtonTagResults>().ReferencedPoseData = Results[tempResult];
                button.GetComponent<ButtonTagResults>().SetText1(Results[tempResult].animatorController.name);
                button.transform.SetParent(buttonTemplate.transform.parent, false);


                Array.Resize(ref ButtonsFound, ButtonsFound.Length + 1);
                ButtonsFound[ButtonsFound.Length - 1] = button;



                tempResult++;
                FirstSearch = true;
            }


            ButtonsFoundTitles = new string[] { };

            for (int b = 0; b < ButtonsFound.Length; b++)
            {
                Array.Resize(ref ButtonsFoundTitles, ButtonsFoundTitles.Length + 1);
                ButtonsFoundTitles[ButtonsFoundTitles.Length - 1] = ButtonsFound[b].GetComponent<ButtonTagResults>().myText.text;
            }

            currentButton = 0;

            LastActionMessage.text = Results.Length.ToString()+ " RESUTLS OBTAINED FOR TAG '"+ tag1 + "'!";
        }
    }

    public void DoubleSearch()
    {
        if (currentAuxQuantity == AuxQuantity.NONE)
        {
            print("button 2 empty");
        }
        else if (currentAuxQuantity == AuxQuantity.ONE)
        {
            Destroy(ButtonsFound2[0]);
        }
        else if (currentAuxQuantity == AuxQuantity.TWO)
        {
            Destroy(ButtonsFound2[0]);
            Destroy(ButtonsFound2[1]);
        }
        else if (currentAuxQuantity == AuxQuantity.THREE)
        {
            Destroy(ButtonsFound2[0]);
            Destroy(ButtonsFound2[1]);
            Destroy(ButtonsFound2[2]);
        }

        if (NecessaryProp != null)
        {
            Destroy(GameObject.Find(NecessaryProp.name + "(Clone)"));
        }

        if (SelectedAvatarColorInManager == AvatarColorInManager.BLUE)
        {
            AvatarBLUE.GetComponent<Animator>().runtimeAnimatorController = TPose;
            currentViewedPoseData = null;
            AvatarBLUE.transform.position = Vector3.zero;
            AvatarBLUE.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        else if (SelectedAvatarColorInManager == AvatarColorInManager.RED)
        {
            AvatarRed.GetComponent<Animator>().runtimeAnimatorController = TPose;
            currentViewedPoseData = null;
            AvatarRed.transform.position = Vector3.zero;
            AvatarRed.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        currentPoseText.text = "";
        TagsText.text = "";

        AvatarAuxiliar1.SetActive(false);
        AvatarAuxiliar2.SetActive(false);
        AvatarAuxiliar3.SetActive(false);

        if (Tag1InputField.text == "" || Tag2InputField.text == "")
        {
            LastActionMessage.text = "PLEASE ADD A TAG IN THE INPUTFIELDS BEFORE DOING THE SEARCH";
        }
        else
        {
            if(FirstSearch == true)
            {
                for (int c = 0; c < ButtonsFound.Length; c++)
                {
                    Destroy(ButtonsFound[c]);
                }
                ButtonsFound = new GameObject[] { };

            }


            currentTypeOfSearch = TypeOfSearh.DOUBLE;

            Tag1InputField.text = Tag1InputField.text.ToUpper();
            Tag2InputField.text = Tag2InputField.text.ToUpper();


            string tag1 = Tag1InputField.text;
            string tag2 = Tag2InputField.text;
            int tempResult = 0;
            foundtag1 = false;
            Results = new PoseData[] { };
            Results2 = new PoseData[] { };


            buttons = new List<GameObject>();

            if (buttons.Count > 0)
            {
                print("found something");
                foreach (GameObject button in buttons)
                {
                    Destroy(button.gameObject);
                }
                buttons.Clear();
            }

            //EMPTY AL ARRAYS
            Datas = new object[] { };

            if (tempTags.Length > 0)
            {
                tempTags = new string[] { };
            }

            //GET AL POSEDATAS
            Datas = Resources.LoadAll("AhabDev/PoseDatas", typeof(PoseData));


            for (int i = 0; i < Datas.Length; i++)
            {
                PoseData tempPoseData = (PoseData)Datas[i];
                for (int g = 0; g < tempPoseData.Tags.Length; g++)
                {
                    if (tempPoseData.Tags[g] == tag1)
                    {
                        foundtag1 = true;
                    }

                }

                if (foundtag1 == true)
                {
                    Array.Resize(ref Results, Results.Length + 1);
                    Results[Results.Length - 1] = tempPoseData;
                }

                foundtag1 = false;
            }

            //STEP TWO OF DOUBLE SEARCH++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
            for (int r = 0; r < Results.Length; r++)
            {
                for (int t = 0; t < Results[r].Tags.Length; t++)
                {
                    if (Results[r].Tags[t] == tag2)
                    {
                        foundtag1 = true;
                    }

                }

                if (foundtag1 == true)
                {
                    Array.Resize(ref Results2, Results2.Length + 1);
                    Results2[Results2.Length - 1] = Results[r];
                }

                foundtag1 = false;
            }


            //for (int i = 0; i < myTagMonitor.CurrentTags.Length; i++)
            foreach (PoseData i in Results2)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);
                button.GetComponent<ButtonTagResults>().ReferencedPoseData = Results2[tempResult];
                button.GetComponent<ButtonTagResults>().SetText1(Results2[tempResult].animatorController.name);
                button.transform.SetParent(buttonTemplate.transform.parent, false);


                Array.Resize(ref ButtonsFound, ButtonsFound.Length + 1);
                ButtonsFound[ButtonsFound.Length - 1] = button;


                tempResult++;
                FirstSearch = true;
            }


            ButtonsFoundTitles = new string[] { };


            for (int b = 0; b < ButtonsFound.Length; b++)
            {
                Array.Resize(ref ButtonsFoundTitles, ButtonsFoundTitles.Length + 1);
                ButtonsFoundTitles[ButtonsFoundTitles.Length - 1] = ButtonsFound[b].GetComponent<ButtonTagResults>().myText.text;
            }

            currentButton = 0;


            LastActionMessage.text = Results2.Length.ToString() + " RESUTLS OBTAINED FOR TAG '" + tag1 + "' AND TAG '" + tag2 +   "'!";
        }
    }

    public void LoadAux1(PoseData AuxPoseData1)
    {
        
        GameObject button = Instantiate(buttonTemplate2) as GameObject;
        button.SetActive(true);
        button.GetComponent<ButtonAvatarAux>().ReferencedPoseData = AuxPoseData1;
        button.GetComponent<ButtonAvatarAux>().SetText2(AuxPoseData1.animatorController.name);
        button.transform.SetParent(buttonTemplate2.transform.parent, false);

        ButtonsFound2 = new GameObject[1];
        ButtonsFound2[0] = button; 
    }

    public void LoadAux2(PoseData AuxPoseData1)
    {

        GameObject button = Instantiate(buttonTemplate2) as GameObject;
        button.SetActive(true);
        button.GetComponent<ButtonAvatarAux>().ReferencedPoseData = AuxPoseData1;
        button.GetComponent<ButtonAvatarAux>().SetText2(AuxPoseData1.animatorController.name);
        button.transform.SetParent(buttonTemplate2.transform.parent, false);

        Array.Resize(ref ButtonsFound2, ButtonsFound2.Length + 1);
        ButtonsFound2[ButtonsFound2.Length - 1] = button;
    }

    public void LoadAux3(PoseData AuxPoseData1)
    {

        GameObject button = Instantiate(buttonTemplate2) as GameObject;
        button.SetActive(true);
        button.GetComponent<ButtonAvatarAux>().ReferencedPoseData = AuxPoseData1;
        button.GetComponent<ButtonAvatarAux>().SetText2(AuxPoseData1.animatorController.name);
        button.transform.SetParent(buttonTemplate2.transform.parent, false);

        Array.Resize(ref ButtonsFound2, ButtonsFound2.Length + 1);
        ButtonsFound2[ButtonsFound2.Length - 1] = button;
    }

    public void AddTagToCurrentPoseData()
    {
        if(AddTagInputField.text == "")
        {
            LastActionMessage.text = "Please add a Tag to use <<Add Tag Function>>";
        }
        else
        {
            if(currentViewedPoseData == null)
            {
                LastActionMessage.text = "Please view a Pose first in order to use <<Add Tag Function>>";
            }
            else
            {
                AddTagInputField.text = AddTagInputField.text.ToUpper();
                Array.Resize(ref currentViewedPoseData.Tags, currentViewedPoseData.Tags.Length + 1);
                currentViewedPoseData.Tags[currentViewedPoseData.Tags.Length - 1] = AddTagInputField.text;
                LastActionMessage.text = "Tag '" + AddTagInputField.text + "' succesfully added to current viewed pose!";
                TagsText.text = "";
                for (int i = 0; i < currentViewedPoseData.Tags.Length; i++)
                {
                    TagsText.text = TagsText.text + currentViewedPoseData.Tags[i] + ",";
                }
                AddTagInputField.interactable = true;
                AddTagInputField.text = "";
                #if UNITY_EDITOR
                    UnityEditor.EditorUtility.SetDirty(currentViewedPoseData);
                    UnityEditor.AssetDatabase.SaveAssets();
                    UnityEditor.AssetDatabase.Refresh();
                #endif
                BuildTagList();
                myTagList1Controller.enabled = false;
                myTagList2Controller.enabled = false;
                myTagList1Controller.enabled = true;
                myTagList2Controller.enabled = true;

            }
            
        }
        
    }

    public void RemoveTagCurrentPoseData()
    {
        if (RemoveTagInputField.text == "")
        {
            LastActionMessage.text = "Please add a Tag to use <<Remove Tag Function>>";
        }
        else
        {
            if (currentViewedPoseData == null)
            {
                LastActionMessage.text = "Please view a Pose first in order to use <<Remove Tag Function>>";
            }
            else
            {
                RemoveTagInputField.text = RemoveTagInputField.text.ToUpper();
                bool tagIsWellWriten = false;
                int tempResult = 0;
                bool alreadychanged = false;
                for (int i = 0; i < currentViewedPoseData.Tags.Length; i++)
                {
                    if (currentViewedPoseData.Tags[i] == RemoveTagInputField.text)
                    {
                        tagIsWellWriten = true;
                    }
                }

                if (tagIsWellWriten == true)
                {
                    temptags = new string[] { };

                    for (int i = 0; i < currentViewedPoseData.Tags.Length; i++)
                    {
                        if (currentViewedPoseData.Tags[i] != RemoveTagInputField.text && alreadychanged == false)
                        {
                            Array.Resize(ref temptags, temptags.Length + 1);
                            temptags[temptags.Length - 1] = currentViewedPoseData.Tags[i];
                        }
                        else if(currentViewedPoseData.Tags[i] == RemoveTagInputField.text && alreadychanged == false)
                        {
                        alreadychanged = true;
                        }
                        else if(alreadychanged == true)
                        {
                            Array.Resize(ref temptags, temptags.Length + 1);
                            temptags[temptags.Length - 1] = currentViewedPoseData.Tags[i];
                        }

                    }
                    tempResult = 0;
                    currentViewedPoseData.Tags = new string[] { };
                    for (int c = 0; c < temptags.Length; c++)
                    {
                        Array.Resize(ref currentViewedPoseData.Tags, currentViewedPoseData.Tags.Length + 1);
                        currentViewedPoseData.Tags[currentViewedPoseData.Tags.Length - 1] = temptags[c];
                    }


                    LastActionMessage.text = "Tag '" + RemoveTagInputField.text + "' succesfully erased from current viewed pose!"; TagsText.text = "";
                    for (int i = 0; i < currentViewedPoseData.Tags.Length; i++)
                    {
                        TagsText.text = TagsText.text + currentViewedPoseData.Tags[i] + ",";
                    }
                    RemoveTagInputField.interactable = true;
                    RemoveTagInputField.text = "";
                    #if UNITY_EDITOR
                        UnityEditor.EditorUtility.SetDirty(currentViewedPoseData);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                    #endif
                    BuildTagList();
                    myTagList1Controller.enabled = false;
                    myTagList2Controller.enabled = false;
                    myTagList1Controller.enabled = true;
                    myTagList2Controller.enabled = true;
                }
                else
                {
                    LastActionMessage.text = "You tried to erase a non existent tag from the current viewed pose!";
                }
            }
            
        }
        
    }

    public void SubstituteTagInAll()
    {
        if (SubstituteAllInputField.text == "" || NewTagInputField.text == "")
        {
            LastActionMessage.text = "Please add fill Tag to change and New Tag fields in order to use <<Remove Tag Function>>";
        }
        else
        {
            SubstituteAllInputField.interactable = true;
            SubstituteAllInputField.text = SubstituteAllInputField.text.ToUpper(); ;
            NewTagInputField.interactable = true;
            NewTagInputField.text = NewTagInputField.text.ToUpper();

            bool foundsimilar = false;

            //EMPTY AL ARRAYS
            Datas = new object[] { };

            //GET AL POSEDATAS
            Datas = Resources.LoadAll("AhabDev/PoseDatas", typeof(PoseData));


            for (int i = 0; i < Datas.Length; i++)
            {
                PoseData tempPoseData = (PoseData)Datas[i];
                for (int g = 0; g < tempPoseData.Tags.Length; g++)
                {
                    if (tempPoseData.Tags[g] == SubstituteAllInputField.text)
                    {
                        tempPoseData.Tags[g] = NewTagInputField.text;

                    #if UNITY_EDITOR
                        UnityEditor.EditorUtility.SetDirty(tempPoseData);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                    #endif




                        foundsimilar = true;
                    }

                }
            }

            //EMPTY AL ARRAYS
            Datas = new object[] { };


            //ACTULIZAR LISTA DE CURRENT TAGS
            if(currentViewedPoseData != null)
            {
                TagsText.text = "";
                for (int i = 0; i < currentViewedPoseData.Tags.Length; i++)
                {
                    TagsText.text = TagsText.text + currentViewedPoseData.Tags[i] + ",";
                }
            }

            if(foundsimilar == true)
            {
                LastActionMessage.text = "You changed tag '" + SubstituteAllInputField.text + "' for tag '" + NewTagInputField.text + "'sucessfully!";
            }
            else
            {

                LastActionMessage.text = "Seems like tag '" + SubstituteAllInputField.text + "' doesn't even exists...";
            }

            SubstituteAllInputField.interactable = true;
            SubstituteAllInputField.text = "";
            NewTagInputField.interactable = true;
            NewTagInputField.text = "";
            BuildTagList();
            myTagList1Controller.enabled = false;
            myTagList2Controller.enabled = false;
            myTagList1Controller.enabled = true;
            myTagList2Controller.enabled = true;
        }
        

    }

    public void NavigateFast()
    {
        if(currentViewedPoseData != null)
        {
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                if(currentButton < ButtonsFound.Length-1)
                {
                    currentButton++;
                    ButtonsFound[currentButton].GetComponent<ButtonTagResults>().OnClick();
                }
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentButton > 0)
                {
                    currentButton--;
                    ButtonsFound[currentButton].GetComponent<ButtonTagResults>().OnClick();
                }
            }
        }
    }
}
