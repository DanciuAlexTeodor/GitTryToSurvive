using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageBoxData
{
    public List<StorageBoxPieceData> storageBoxList;

    public StorageBoxData(List<StorageBoxPieceData> _storageBoxList)
    {
        storageBoxList = _storageBoxList;
    }
}
