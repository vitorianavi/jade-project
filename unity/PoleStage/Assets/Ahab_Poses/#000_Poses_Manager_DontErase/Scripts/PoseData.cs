using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NecessaryAvatars
{
    ONE,
    TWO,
    THREE,
    FOUR
}
[CreateAssetMenu(fileName = "PoseData", menuName = "AhabDev/PoseData")]
public class PoseData : ScriptableObject
{
    public bool AlreadySet;
    public RuntimeAnimatorController animatorController;
    public Vector3 AvatarPosition;
    public Quaternion AvatarRotation;
    public string[] Tags;
    [Header("Prop info")]
    public bool UsesAProp;
    public GameObject poseProp;
    public Vector3 PropPosition;
    public Quaternion PropRotation;
    public Vector3 PropSize;
    [Header("Avatars' transforms")]
    public NecessaryAvatars currentNecessaryAvatars;
    public PoseData PoseData2;
    public PoseData PoseData3;
    public PoseData PoseData4;

}
