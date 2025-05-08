using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Checkpoint", menuName = "ScriptableObjects/Checkpoint", order = 1)]
public class Checkpoint : ScriptableObject
{
    public string name; //name of the checkpoint
    public bool isCompleted=false;

    private void Start()
    {
        isCompleted = false;
    }
}
