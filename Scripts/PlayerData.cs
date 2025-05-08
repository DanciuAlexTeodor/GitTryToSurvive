using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    public float[] playerStats;
    //[0] = health, [1] = calories, [2] = hydration

    public float[] playerPositionAndRotation;
    //[0] = x, [1] = y, [2] = z, [3] = xRot, [4] = yRot, [5] = zRot

    public List<string> inventoryContent;
    public List<float> itemHealthList;

    public string[] quickSlotsContent;
    public List<float> quickSlotsHealth;

    public List<Quest> allActiveQuestsContent;
    public List<Quest> allCompletedQuestsContent;

    public PlayerData(float[] _playerStats, float[] _playerPosAndRot, 
       List<string> _inventoryContent, List<float> _itemHealthList, 
       string[] _quickSlotsContent, List<float> _quickSlotsHealth
    )
       
    {
        playerStats = _playerStats;
        playerPositionAndRotation = _playerPosAndRot;
        inventoryContent = _inventoryContent;
        itemHealthList = _itemHealthList;
        quickSlotsContent = _quickSlotsContent;
        quickSlotsHealth = _quickSlotsHealth;
    }


}
