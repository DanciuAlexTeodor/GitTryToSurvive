using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CampfireData
{
    public List<CampfirePieceData> campfireList;

    public CampfireData(List<CampfirePieceData> campfireList)
    {
        this.campfireList = campfireList;
    }
}
