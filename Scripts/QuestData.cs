using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public List<Quest> allCompletedQuestsContent;
    public bool firstTimeInteractionWithNPC;
    public int questIndex;
    public int coinAmount;

    public QuestData( List<Quest> _allCompletedQuestsContent, 
       bool _firstTimeInteractionWithNPC, int _questIndex, int _coinAmount)
    {
       
        allCompletedQuestsContent = _allCompletedQuestsContent;
        this.firstTimeInteractionWithNPC = _firstTimeInteractionWithNPC;
        questIndex = _questIndex;
        coinAmount = _coinAmount;
    }
}
