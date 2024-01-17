using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagList1Controller : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;


    public List<GameObject> buttons;

    public TagMonitor  myTagMonitor;

    public void OnEnable()
    {
        BuildTagList1();
    }

    public void BuildTagList1()
    {
        int tempInt = 0;

        myTagMonitor = Resources.Load<TagMonitor>("AhabDev/PoseDatas/TagMonitor");

        if (myTagMonitor == null)
        {
            print("Please send the contents of <<SendToResources>> folder to your project's <<Resouces>> folder. If you don't know how to follow this step check the Read me doc ");
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
        else
        {
            if (buttons.Count > 0)
            {
                foreach (GameObject button in buttons)
                {
                    Destroy(button.gameObject);
                }
                buttons.Clear();
            }

            //for (int i = 0; i < myTagMonitor.CurrentTags.Length; i++)
            foreach (string i in myTagMonitor.CurrentTags)
            {
                GameObject button = Instantiate(buttonTemplate) as GameObject;
                button.SetActive(true);
                button.GetComponent<ButtonTagList1Button>().SetText1(myTagMonitor.CurrentTags[tempInt]);
                button.transform.SetParent(buttonTemplate.transform.parent, false);
                buttons.Add(button);
                tempInt++;
            }
        }

            
    }
}
