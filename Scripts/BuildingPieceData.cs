using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BuildingPieceData
{
    public string pieceType;
    public Vector3 position;
    public Quaternion rotation;

    public BuildingPieceData(string _pieceType, Vector3 _position, Quaternion _rotation)
    {
        pieceType = _pieceType;
        position = _position;
        rotation = _rotation;
    }
}
