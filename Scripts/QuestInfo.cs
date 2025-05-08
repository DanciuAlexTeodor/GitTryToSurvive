using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/QuestInfo", order = 1)]
[System.Serializable]
public class QuestInfo : ScriptableObject
{
    [TextArea(5,10)]
    public List<string> initialDialog;
    public List<AudioClip> initialDialogClips;

    [Header("Options")]
    [TextArea(5, 10)]
    public string acceptOption;
    [TextArea(5, 10)]
    public string acceptAnswer;
    public AudioClip acceptAnswerClip;

    [TextArea(5, 10)]
    public string declineOption;
    [TextArea(5, 10)]
    public string declineAnswer;
    public AudioClip declineAnswerClip;
    [TextArea(5, 10)]
    public string comeBackAfterDecline;
    public AudioClip comeBackAfterDeclineClip;
    [TextArea(5, 10)]
    public string comeBackInProgress;
    public AudioClip comeBackInProgressClip;
    [TextArea(5, 10)]
    public string comeBackCompleted;
    public AudioClip comeBackCompletedClip;
    [TextArea(5, 10)]
    public string finalWords;

    [Header("Rewards")]
    public int coinReward;
    public string rewardItem1; 
    public int rewardItem1Amount;

    public string rewardItem2; 
    public int rewardItem2Amount;

    [Header("Requirements")]
    public string firstRequirementItem;//stone
    public int firstRequirementAmount;//5

    public string secondRequirementItem;
    public int secondRequirementAmount;

    public int questNumber;
    public bool hasToSubmitItems;

    [Header("Checkpoints")]
    public bool hasCheckpoints;
    public List<Checkpoint> checkpoints;
}
