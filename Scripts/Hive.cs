using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour
{
    public Checkpoint checkPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkPoint!=null)
        {
            checkPoint.isCompleted = true;
            QuestManager.Instance.RefreshTrackerList();
        }
    }
}
