using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AnimalData
{
    public string animalName; // Prefab name 
    public Vector3 position;
    public Quaternion rotation;
    public float health; // Example property
    public bool hasBeenFed; // Example property
    // Add other properties like hunger, aggression, etc.

    public AnimalData(string name, Vector3 pos, Quaternion rot, float hp, bool _hasBeenFed)
    {
        animalName = name;
        position = pos;
        rotation = rot;
        health = hp;
        hasBeenFed = _hasBeenFed;
    }
}
