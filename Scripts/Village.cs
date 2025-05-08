using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    public Checkpoint checkPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            checkPoint.isCompleted = true;
            QuestManager.Instance.RefreshTrackerList();
        }
    }
}
