using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageBoxPieceData
{

    public StorageBox storageBox;
    public List<string> storedItems;
    public string name;
    public Vector3 position;
    public Quaternion rotation;
    public string parentName;
 



    public StorageBoxPieceData(StorageBox _storageBox,string _name, Vector3 _position, Quaternion _rotation, string _parentName = null)
    {
        storageBox = _storageBox;
        storedItems = new List<string>(_storageBox.items);
        name = _name;
        position = _position;
        rotation = _rotation;
        parentName = _parentName;
       
    }
}
