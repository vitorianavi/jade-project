using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTagList1Button : MonoBehaviour
{
    [SerializeField]
    private Text myText;
    public InputField InputFieldText;

    public void SetText1(string textString)
    {
        myText.text = textString;
    }

    public void OnClick()
    {

        InputFieldText.interactable = true;
        InputFieldText.text = myText.text;
        EventSystem.current.SetSelectedGameObject(null);
    }
}
