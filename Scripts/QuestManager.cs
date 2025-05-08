using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public List<Quest> allActiveQuests;
    public List<Quest> allCompletedQuests;

    [Header("QuestMenu")]
    public GameObject questMenu;
    public bool isQuestMenuOpen;

    public GameObject activeQuestPrefab;
    public GameObject completedQuestPrefab;

    public GameObject questMenuContent;

    [Header("QuestTracker")]
    public GameObject questTrackerContent;

    public GameObject trackerRowPrefab;

    public List<Quest> allTrackedQuests;

    public void TrackQuest(Quest quest)
    {
        allTrackedQuests.Add(quest);
        RefreshTrackerList();
    }

    public void UnTrackQuest(Quest quest)
    {
        allTrackedQuests.Remove(quest);
        RefreshTrackerList();
    }

    public void RefreshTrackerList()
    {
        // Destroying the previous list
        foreach (Transform child in questTrackerContent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Quest trackedQuest in allTrackedQuests)
        {
            GameObject trackerPrefab = Instantiate(trackerRowPrefab, Vector3.zero, Quaternion.identity);
            trackerPrefab.transform.SetParent(questTrackerContent.transform, false);

            TrackerRow tRow = trackerPrefab.GetComponent<TrackerRow>();
            tRow.questName.text = trackedQuest.questName;

            // Initialize requirements text
            tRow.requirements.text = string.Empty;

            // Check if the quest has checkpoints and print them first
            if (trackedQuest.info.hasCheckpoints)
            {
                tRow.requirements.text = PrintCheckpoints(trackedQuest, tRow.requirements.text);
            }

            // Get requirements from the quest
            var req1 = trackedQuest.info.firstRequirementItem;
            var req1Amount = trackedQuest.info.firstRequirementAmount;
            var req2 = trackedQuest.info.secondRequirementItem;
            var req2Amount = trackedQuest.info.secondRequirementAmount;

            // Check if the requirement involves killing animals or hitting the anvil
            //there are more Actions such as: "Hit", "Kill
            if (req1.StartsWith("Kill") || req1.StartsWith("Hit") ||req1.StartsWith("Cultivate"))
            {
                string action = req1.Substring(0, req1.IndexOf(" ")); // Extract the action from the requirement
                int lengthOfFirstWord = req1.IndexOf(" ");
                string enemyType1 = req1.Substring(lengthOfFirstWord + 1); // Extract the enemy type after "Action"

                int count1 = QuestEventSystem.Instance.GetKillCount(enemyType1); // Fetch the number of kills or hits
                tRow.requirements.text += $"{enemyType1} {action}: {count1}/{req1Amount}\n";
            }
            else
            {
                // If it's a regular item requirement
                tRow.requirements.text += $"{req1} {InventorySystem.Instance.CheckItemAmount(req1)}/{req1Amount}\n";
            }

            if (!string.IsNullOrEmpty(req2))
            {
                if (req2.StartsWith("Kill") || req2.StartsWith("Hit") || req2.StartsWith("Cultivate"))
                {
                    string action = req2.Substring(0, req2.IndexOf(" ")); // Extract the action from the requirement
                    int lengthOfFirstWord = req2.IndexOf(" ");
                    string enemyType2 = req2.Substring(lengthOfFirstWord + 1); // Extract the enemy type after "Action"

                    int count2 = QuestEventSystem.Instance.GetKillCount(enemyType2); // Fetch the number of kills or hits
                    tRow.requirements.text += $"{enemyType2} {action}: {count2}/{req2Amount}\n";
                }
                else
                {
                    // If it's a regular item requirement

                    tRow.requirements.text += $"{req2} {InventorySystem.Instance.CheckItemAmount(req2)}/{req2Amount}\n";
                }
            }
        }
    }

    public string PrintCheckpoints(Quest trackedQuest, string existingText)
    {
        string finalText = existingText;

        foreach (Checkpoint cp in trackedQuest.info.checkpoints)
        {
            if (cp.isCompleted)
            {
                finalText += $"{cp.name} [Done]\n"; 
            }
            else
            {
                finalText += $"{cp.name} \n";
            }
        }

        return finalText;
    }


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isQuestMenuOpen && !ConstructionManager.Instance.inConstructionMode)
        {
            questMenu.SetActive(true);

            CursorManager.Instance.FreeCursor();

            isQuestMenuOpen = true;
        }
        else if (Input.GetKeyDown(KeyCode.Q) && isQuestMenuOpen)
        {
            questMenu.SetActive(false);

            isQuestMenuOpen = false;
            CursorManager.Instance.LockCursor();
        }       
    }

    public void AddActiveQuest(Quest quest)
    {
        allActiveQuests.Add(quest);
        TrackQuest(quest);
        RefreshQuestList();
    }
    //Active = 3        => A = 3
    //Completed = 1, 2  => 

    public void MarkQuestAsCompleted(Quest quest)
    {
        allActiveQuests.Remove(quest);
        allCompletedQuests.Add(quest);
        UnTrackQuest(quest);
        RefreshQuestList();
    }

    public void RefreshQuestList()
    {
        // Destroy the previous list
        foreach (Transform child in questMenuContent.transform)
        {
            Debug.Log("Destroy item 18");
            Destroy(child.gameObject);
        }

        // Loop through active quests and add them to the quest menu
        foreach (Quest activeQuest in allActiveQuests)
        {
            GameObject questPrefab = Instantiate(activeQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            if (qRow == null)
            {
                Debug.LogError("qRow is null for the quest prefab.");
                continue;
            }

            if (activeQuest == null)
            {
                Debug.LogError("activeQuest is null.");
                continue;
            }

            if (activeQuest.info == null)
            {
                Debug.LogError($"Quest info is null for quest: {activeQuest.questName}");
                continue;
            }

            qRow.questName.text = activeQuest.questName;
            qRow.questGiver.text = activeQuest.questGiver;

            qRow.isActive = true;
            qRow.isTracking = true;

            if (qRow.coinAmount == null)
            {
                Debug.LogError($"coinAmount is null for quest: {activeQuest.questName}");
            }
            else
            {
                qRow.coinAmount.text = $"{activeQuest.info.coinReward}";
            }

            // Handle first reward item
            if (!string.IsNullOrEmpty(activeQuest.info.rewardItem1))
            {
                qRow.firstReward.sprite = GetSpriteForItem(activeQuest.info.rewardItem1);
                qRow.firstRewardAmount.text = "";
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmount.text = "";
            }

            // Handle second reward item
            if (!string.IsNullOrEmpty(activeQuest.info.rewardItem2))
            {
                qRow.secondReward.sprite = GetSpriteForItem(activeQuest.info.rewardItem2);
                qRow.secondRewardAmount.text = "";
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmount.text = "";
            }
        }


        // Loop through completed quests and add them to the quest menu
        foreach (Quest completedQuest in allCompletedQuests)
        {
            if (completedQuest == null)
            {
                Debug.LogError("completedQuest is null.");
                continue; // Skip this iteration
            }

            if (completedQuest.info == null)
            {
                Debug.LogError($"completedQuest.info is null for quest: {completedQuest.questName}");
                continue; // Skip this iteration
            }
            else
            {
                completedQuest.infoName = completedQuest.info.name;
            }

            GameObject questPrefab = Instantiate(completedQuestPrefab, Vector3.zero, Quaternion.identity);
            questPrefab.transform.SetParent(questMenuContent.transform, false);

            QuestRow qRow = questPrefab.GetComponent<QuestRow>();

            qRow.questName.text = completedQuest.questName;
            qRow.questGiver.text = completedQuest.questGiver;

            qRow.isActive = false;
            qRow.isTracking = false;

            if (qRow.coinAmount == null)
            {
                Debug.LogError($"CoinAmount is null for quest: {completedQuest.questName}");
            }
            else
            {
                if (completedQuest.info.coinReward == 0)
                {
                    qRow.coinAmount.text = "";
                }
                else
                {
                    qRow.coinAmount.text = $"{completedQuest.info.coinReward}";
                }
            }

            // Handle first reward item
            if (!string.IsNullOrEmpty(completedQuest.info.rewardItem1))
            {
                qRow.firstReward.sprite = GetSpriteForItem(completedQuest.info.rewardItem1);
                qRow.firstRewardAmount.text = ""; // Uncomment this if you need to set a specific amount
            }
            else
            {
                qRow.firstReward.gameObject.SetActive(false);
                qRow.firstRewardAmount.text = ""; // Uncomment this if you need to clear the text
            }

            // Handle second reward item
            if (!string.IsNullOrEmpty(completedQuest.info.rewardItem2))
            {
                qRow.secondReward.sprite = GetSpriteForItem(completedQuest.info.rewardItem2);
                qRow.secondRewardAmount.text = ""; // Uncomment this if you need to set a specific amount
            }
            else
            {
                qRow.secondReward.gameObject.SetActive(false);
                qRow.secondRewardAmount.text = ""; // Uncomment this if you need to clear the text
            }
        }

    }


    public Sprite GetSpriteForItem(string item)
    {
        if(item==null)
        {
            Debug.Log("Item is null");
            return null;
        }


        var itemToGet = Resources.Load<GameObject>(item);

        if (itemToGet == null)
        {
            Debug.Log("ItemToGet is null");
            return null;
        }

        if(itemToGet.GetComponent<Image>() == null)
        {
            Debug.Log("ItemToGet.GetComponent<Image>() is null");
            return null;
        }
        
        return itemToGet.GetComponent<Image>().sprite;
    }
       

}
