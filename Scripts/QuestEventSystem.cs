using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventSystem : MonoBehaviour
{
    public static QuestEventSystem Instance { get; private set; }

    // Quest 6 tracking
    public bool isQuest7TrackingActive = false;
    public bool isQuest12TrackingActive = false;
    public bool isQuest11TrackingActive=false;
    public bool isQuest13TrackingActive = false;
    public bool isQuest15TrackingActive = false;

    public bool isQuest7Completed = false;
    public bool isQuest12Completed = false;
    public bool isQuest11Completed = false;
    public bool isQuest13Completed = false;
    public bool isQuest15Completed = false;

    private Dictionary<string,int> enemyKillCounts = new Dictionary<string, int>();

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


    #region --------- Quest 7 Tracking ---------

    // Start tracking for Quest 7
    public void StartQuest7Tracking()
    {
        if (!isQuest7TrackingActive)
        {
            enemyKillCounts.Add("Wolf", 0);
            enemyKillCounts.Add("Bear", 0);
            isQuest7TrackingActive = true;
            Enemy.OnEnemyKilled += TrackQuest7Kills;
            Debug.Log("Started tracking Quest 7 kills.");
        }
    }

    private void TrackQuest7Kills(string enemyType)
    {
        if (enemyType == "Wolf")
        {
            enemyKillCounts["Wolf"]++;
            QuestManager.Instance.RefreshTrackerList();
        }
        else if (enemyType == "Bear")
        {

            enemyKillCounts["Bear"]++;
            QuestManager.Instance.RefreshTrackerList();
        }

        // Check if the quest requirements are met
        int wolvesKilled = enemyKillCounts["Wolf"];
        int bearsKilled = enemyKillCounts["Bear"];
        if (wolvesKilled >= 2 && bearsKilled >= 1)
        {
            isQuest7Completed = true;
            Debug.Log("Quest7 completed!");
            StopQuest7Tracking();
        }
    }

    // Stop tracking for Quest 7
    public void StopQuest7Tracking()
    {
        if (isQuest7TrackingActive)
        {
            isQuest7TrackingActive = false;
            Enemy.OnEnemyKilled -= TrackQuest7Kills;
            Debug.Log("Stopped tracking Quest7 kills.");

        }
    }

    public void ResetQuest7Tracking()
    {
        enemyKillCounts.Clear();
    }

    #endregion

    #region --------- Quest 11 Tracking ---------

    public void StartQuest11Tracking()
    {
        if (!isQuest11TrackingActive)
        {
            enemyKillCounts.Add("Anvil", 0);
            Debug.Log("Started tracking Quest 11 hits.");
            isQuest11TrackingActive = true;
            SelectionManager.OnHammerHit += TrackQuest11Hits;
        }
    }

    private void TrackQuest11Hits()
    {
        Debug.Log("One hit on Anvil!");
        enemyKillCounts["Anvil"]++;
        QuestManager.Instance.RefreshTrackerList();

        // Check if the quest requirements are met
        int hammerHits = enemyKillCounts["Anvil"];
        if (hammerHits >= 5)
        {
            isQuest11Completed = true;
            Debug.Log("Quest11 completed!");
            StopQuest11Tracking();
        }
    }

    public void StopQuest11Tracking()
    {
        if (isQuest11TrackingActive)
        {
            isQuest11TrackingActive = false;
            SelectionManager.OnHammerHit -= TrackQuest11Hits;
        }
    }

    public void ResetQuest11Tracking()
    {
        enemyKillCounts.Clear();
    }
    #endregion

    #region --------- Quest 12 Tracking ---------

    public void StartQuest12Tracking()
    {
        if (!isQuest12TrackingActive)
        {
            enemyKillCounts.Add("Pig", 0);
            enemyKillCounts.Add("Goat", 0);
            isQuest12TrackingActive = true;
            Animal.OnEnemyKilledWithBow += TrackQuest12Kills;
            Debug.Log("Started tracking Quest 12 kills.");
        }
    }


    private void TrackQuest12Kills(string enemyType)
    {
        if (enemyType == "Pig")
        {
            enemyKillCounts["Pig"]++;
            QuestManager.Instance.RefreshTrackerList();
        }
        else if (enemyType == "Goat")
        {
            enemyKillCounts["Goat"]++;
            QuestManager.Instance.RefreshTrackerList();
        }

        // Check if the quest requirements are met
        int pigsKilled = enemyKillCounts["Pig"];
        int goatsKilled = enemyKillCounts["Goat"];
        if (pigsKilled >= 2 && goatsKilled >= 1)
        {
            isQuest12Completed = true;
            Debug.Log("Quest12 completed!");
            StopQuest12Tracking();
        }
    }


    public void StopQuest12Tracking()
    {
        if (isQuest12TrackingActive)
        {
            isQuest12TrackingActive = false;
            Animal.OnEnemyKilledWithBow -= TrackQuest12Kills;
            Debug.Log("Stopped tracking Quest12 kills.");
        }
    }

    public void ResetQuest12Tracking()
    {
        enemyKillCounts.Clear();
    }

    #endregion

    #region --------- Quest 13 Tracking ---------

    public void StartQuest13Tracking()
    {
        if (!isQuest13TrackingActive)
        {
            enemyKillCounts.Add("Soulreeper", 0);
            isQuest13TrackingActive = true;
            Enemy.OnEnemyKilled += TrackQuest13Kills;
            Debug.Log("Started tracking Quest 13.");
        }
    }

    private void TrackQuest13Kills(string enemyType)
    {
        if (enemyType == "Soulreeper")
        {
            enemyKillCounts["Soulreeper"]++;
            QuestManager.Instance.RefreshTrackerList();
        }

        // Check if the quest requirements are met
        int soulreepersKilled = enemyKillCounts["Soulreeper"];
        if (soulreepersKilled >= 5)
        {
            isQuest13Completed = true;
            Debug.Log("Quest13 completed!");
            StopQuest13Tracking();
        }
    }

    public void StopQuest13Tracking()
    {
        if (isQuest13TrackingActive)
        {
            isQuest13TrackingActive = false;
            Enemy.OnEnemyKilled -= TrackQuest13Kills;
            Debug.Log("Stopped tracking Quest13.");
        }
    }

    #endregion

    #region --------- Quest 15 Tracking ---------

    public void StartQuest15Tracking()
    {
        if(!isQuest15TrackingActive)
        {
            enemyKillCounts.Add("Tomato", 0);
            enemyKillCounts.Add("Carrot", 0);
            isQuest15TrackingActive = true;
            InteractableObject.ItemCollected += TrackQuest15Kills;
            Debug.Log("Started tracking Quest 15.");
        }
    }

    private void TrackQuest15Kills(string itemName)
    {
        if (itemName == "Tomato")
        {
            enemyKillCounts["Tomato"]++;
            QuestManager.Instance.RefreshTrackerList();
        }
        else if (itemName == "Carrot")
        {
            enemyKillCounts["Carrot"]++;
            QuestManager.Instance.RefreshTrackerList();
        }

        // Check if the quest requirements are met
        int tomatoesCollected = enemyKillCounts["Tomato"];
        int carrotsCollected = enemyKillCounts["Carrot"];
        if (tomatoesCollected >= 3 && carrotsCollected >= 3)
        {
            isQuest15Completed = true;
            Debug.Log("Quest15 completed!");
            StopQuest15Tracking();
        }
    }

    public void StopQuest15Tracking()
    {
        if (isQuest15TrackingActive)
        {
            isQuest15TrackingActive = false;
            InteractableObject.ItemCollected -= TrackQuest15Kills;
            Debug.Log("Stopped tracking Quest15.");
        }
    }

    public void ResetQuest15Tracking()
    {
        enemyKillCounts.Clear();
    }

    #endregion
    
    

    

    public int GetKillCount(string enemyType)
    {
        if (enemyKillCounts.ContainsKey(enemyType))
        {
            return enemyKillCounts[enemyType];
        }
        return 0;
        
    }
}
