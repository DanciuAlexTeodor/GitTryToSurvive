using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Quest
{
    public string questName;
    public string questGiver;
    public string questDescription;

   
    [Header("Bools")]
    public bool accepted;
    public bool declined;
    public bool initialDialogCompleted;
    public bool isCompleted;

    public bool hasNoRequirements;


    [Header("Quest Info")]
    public string infoName; // Name of the ScriptableObject
    

    public QuestInfo info; // The actual ScriptableObject

}
