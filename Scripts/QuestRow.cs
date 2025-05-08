using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestRow : MonoBehaviour
{
    public TextMeshProUGUI questName;
    public TextMeshProUGUI questGiver;

    public Button trackingButton;

    public bool isActive;
    public bool isTracking;

    public Text coinAmount;

    public Image firstReward;
    public Text firstRewardAmount;

    public Image secondReward;
    public Text secondRewardAmount;

    public Quest thisQuest;

    private void Start()
    {
        trackingButton.onClick.AddListener(() => {

            if (isActive)
            {
                if (isTracking)
                {
                    isTracking = false;
                    trackingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Not Tracking";
                    QuestManager.Instance.UnTrackQuest(thisQuest);
                }
                else
                {
                    isTracking = true;
                    trackingButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tracking";
                    QuestManager.Instance.TrackQuest(thisQuest);
                }
            }



        });
    }
  

}
