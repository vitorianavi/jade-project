using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTagResults : MonoBehaviour
{
    public PoseData ReferencedPoseData;
    public PosesManagerController myPosesManagerController;
    public Text TagsText;
    public Text AnimationName;
    public Text UpdateMessage;
    public Text myText;

    public void SetText1(string textString)
    {
        myText.text = textString;
    }

    public void OnClick()
    {
        for (int i = 0; i < myPosesManagerController.ButtonsFoundTitles.Length; i++)
        {
            if(myText.text == myPosesManagerController.ButtonsFoundTitles[i])
            {
                myPosesManagerController.currentButton = i;
            }
        }

        myPosesManagerController.currentViewedPoseData = ReferencedPoseData;

        if (myPosesManagerController.currentAuxQuantity == AuxQuantity.NONE)
        {

        }
        else if (myPosesManagerController.currentAuxQuantity == AuxQuantity.ONE)
        {
            Destroy(myPosesManagerController.ButtonsFound2[0]);
        }
        else if (myPosesManagerController.currentAuxQuantity == AuxQuantity.TWO)
        {
            Destroy(myPosesManagerController.ButtonsFound2[0]);
            Destroy(myPosesManagerController.ButtonsFound2[1]);
        }
        else if (myPosesManagerController.currentAuxQuantity == AuxQuantity.THREE)
        {
            Destroy(myPosesManagerController.ButtonsFound2[0]);
            Destroy(myPosesManagerController.ButtonsFound2[1]);
            Destroy(myPosesManagerController.ButtonsFound2[2]);
        }

        if (myPosesManagerController.NecessaryProp != null)
        {
            Destroy(GameObject.Find(myPosesManagerController.NecessaryProp.name+"(Clone)"));
        }
        myPosesManagerController.avatarHasPoseApplied = true;

        myPosesManagerController.AvatarAuxiliar1.SetActive(false);
        myPosesManagerController.AvatarAuxiliar2.SetActive(false);
        myPosesManagerController.AvatarAuxiliar3.SetActive(false);

        if (myPosesManagerController.SelectedAvatarColorInManager == AvatarColorInManager.BLUE)
        {
            myPosesManagerController.AvatarBLUE.GetComponent<Animator>().runtimeAnimatorController = ReferencedPoseData.animatorController;
            myPosesManagerController.AvatarBLUE.transform.position = ReferencedPoseData.AvatarPosition;
            myPosesManagerController.AvatarBLUE.transform.rotation = ReferencedPoseData.AvatarRotation;
        }
        else if (myPosesManagerController.SelectedAvatarColorInManager == AvatarColorInManager.RED)
        {
            myPosesManagerController.AvatarRed.GetComponent<Animator>().runtimeAnimatorController = ReferencedPoseData.animatorController;
            myPosesManagerController.AvatarRed.transform.position = ReferencedPoseData.AvatarPosition;
            myPosesManagerController.AvatarRed.transform.rotation = ReferencedPoseData.AvatarRotation;
        }


        if(ReferencedPoseData.UsesAProp == true)
        {
            if(myPosesManagerController.NecessaryProp != null)
            {
                myPosesManagerController.NecessaryProp.SetActive(true);

            }
            myPosesManagerController.NecessaryProp = ReferencedPoseData.poseProp;
            Instantiate(myPosesManagerController.NecessaryProp, ReferencedPoseData.PropPosition, ReferencedPoseData.PropRotation);
            myPosesManagerController.NecessaryProp.transform.localScale = ReferencedPoseData.PropSize;
        }


        EventSystem.current.SetSelectedGameObject(null);
        UpdateMessage.text = "Loaded animation '" + ReferencedPoseData.animatorController.name + "'!";
        AnimationName.text = ReferencedPoseData.animatorController.name;
        TagsText.text = "";
        for (int i = 0; i < ReferencedPoseData.Tags.Length; i++)
        {
            TagsText.text = TagsText.text + ReferencedPoseData.Tags[i] + ",";
        }

        myPosesManagerController.currentAuxQuantity = AuxQuantity.NONE;

        if (ReferencedPoseData.currentNecessaryAvatars == NecessaryAvatars.TWO)
        {
            SetAux1();
            myPosesManagerController.currentAuxQuantity = AuxQuantity.ONE;
        }
        if (ReferencedPoseData.currentNecessaryAvatars == NecessaryAvatars.THREE)
        {
            SetAux1();
            SetAux2();
            myPosesManagerController.currentAuxQuantity = AuxQuantity.TWO;
        }
        if (ReferencedPoseData.currentNecessaryAvatars == NecessaryAvatars.FOUR)
        {
            SetAux1();
            SetAux2();
            SetAux3();
            myPosesManagerController.currentAuxQuantity = AuxQuantity.THREE;
        }
    }
    public void SetAux1()
    {
        myPosesManagerController.AvatarAuxiliar1.SetActive(true);
        myPosesManagerController.AvatarAuxiliar1.GetComponent<Animator>().runtimeAnimatorController = ReferencedPoseData.PoseData2.animatorController;
        myPosesManagerController.AvatarAuxiliar1.transform.position = ReferencedPoseData.PoseData2.AvatarPosition;
        myPosesManagerController.AvatarAuxiliar1.transform.rotation = ReferencedPoseData.PoseData2.AvatarRotation;
        myPosesManagerController.LoadAux1(ReferencedPoseData.PoseData2);
    }
    public void SetAux2()
    {
        myPosesManagerController.AvatarAuxiliar2.SetActive(true);
        myPosesManagerController.AvatarAuxiliar2.GetComponent<Animator>().runtimeAnimatorController = ReferencedPoseData.PoseData3.animatorController;
        myPosesManagerController.AvatarAuxiliar2.transform.position = ReferencedPoseData.PoseData3.AvatarPosition;
        myPosesManagerController.AvatarAuxiliar2.transform.rotation = ReferencedPoseData.PoseData3.AvatarRotation;
        myPosesManagerController.LoadAux2(ReferencedPoseData.PoseData3);

    }
    public void SetAux3()
    {
        myPosesManagerController.AvatarAuxiliar3.SetActive(true);
        myPosesManagerController.AvatarAuxiliar3.GetComponent<Animator>().runtimeAnimatorController = ReferencedPoseData.PoseData4.animatorController;
        myPosesManagerController.AvatarAuxiliar3.transform.position = ReferencedPoseData.PoseData4.AvatarPosition;
        myPosesManagerController.AvatarAuxiliar3.transform.rotation = ReferencedPoseData.PoseData4.AvatarRotation;
        myPosesManagerController.LoadAux3(ReferencedPoseData.PoseData4);

    }
}
