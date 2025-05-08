using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string enemyName; // Prefab name
    public Vector3 position;
    public Quaternion rotation;
    public float health; // Example property

    public EnemyData(string name, Vector3 pos, Quaternion rot, float hp)
    {
        enemyName = name;
        position = pos;
        rotation = rot;
        health = hp;
    }
}

