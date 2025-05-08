using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public static NPC Instance { get; set; }

    public static event Action PlayerGetsTheMap;
    public static event Action ActivateKeyboardMap;

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

    #region ----- Variables -----
    public bool playerInRange;
    public bool isTalkingWithPlayer;

    TextMeshProUGUI npcDialogText;
    Button optionButton1;
    TextMeshProUGUI optionButton1Text;

    Button optionButton2;
    TextMeshProUGUI optionButton2Text;

    public List<Quest> quests;
    public Quest currentActiveQuest = null;
    public int activeQuestIndex = 0;
    public bool firstTimeInteraction = true;
    public int currentDialog;

    #endregion

    private void Start()
    {
        npcDialogText = DialogSystem.Instance.dialogText;
        optionButton1 = DialogSystem.Instance.option1BTN;
        optionButton1Text = DialogSystem.Instance.option1BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        optionButton2 = DialogSystem.Instance.option2BTN;
        optionButton2Text = DialogSystem.Instance.option2BTN.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void LookAtPlayer()
    {
        var player = PlayerState.Instance.playerBody.transform;
        Vector3 direction = player.position - transform.position;
        transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = transform.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void StartConversation()
    {
        isTalkingWithPlayer = true;

        LookAtPlayer();

        //curentActiveQuest : .declined, .accepted, .isCompleted, .initialDialogCompleted

        //Interaction with NPC for the first time
        if (firstTimeInteraction)
        {
            Debug.Log("First Time Interaction");
            firstTimeInteraction = false;
            currentActiveQuest = quests[activeQuestIndex]; //0 at start
            StartQuestInitialDialog();
            currentDialog = 0;
        }
        else
        {
            // If we return after declining the quest
            if (currentActiveQuest.declined)
            {
                DialogSystem.Instance.OpenDialogUI();
                npcDialogText.text = currentActiveQuest.info.comeBackAfterDecline;
                SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.comeBackAfterDeclineClip);
                SetAcceptAndDeclineOption();
            }

            // If we return while the quest is still in progress
            if (currentActiveQuest.accepted && currentActiveQuest.isCompleted == false)
            {
                if (AreQuestRequirementsCompleted())
                {
                    if(currentActiveQuest.info.hasToSubmitItems)
                    {
                        SubmitRequiredItems();
                    }
                    

                    DialogSystem.Instance.OpenDialogUI();

                    npcDialogText.text = currentActiveQuest.info.comeBackCompleted;
                    SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.comeBackCompletedClip);


                    optionButton1Text.text = "[Take Reward]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() =>
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.takeMoney);
                        ReceiveRewardAndCompleteQuest();
                    });
                }
                else
                {
                    DialogSystem.Instance.OpenDialogUI();
                    npcDialogText.text = currentActiveQuest.info.comeBackInProgress;
                    SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.comeBackInProgressClip);

                    optionButton1Text.text = "[Close]";
                    optionButton1.onClick.RemoveAllListeners();
                    optionButton1.onClick.AddListener(() =>
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
                        DialogSystem.Instance.CloseDialogUI();
                        isTalkingWithPlayer = false;
                    });
                }
            }

            // If the quest is completed
            if (currentActiveQuest.isCompleted == true)
            {
                DialogSystem.Instance.OpenDialogUI();

                npcDialogText.text = currentActiveQuest.info.finalWords;

                optionButton1Text.text = "[Close]";
                optionButton1.onClick.RemoveAllListeners();
                optionButton1.onClick.AddListener(() =>
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
                    DialogSystem.Instance.CloseDialogUI();
                    isTalkingWithPlayer = false;
                });
            }

            // If there is another quest available
            if (currentActiveQuest.initialDialogCompleted == false)
            {
                StartQuestInitialDialog();
            }


        }
    }

    private void SubmitRequiredItems()
    {
        string firstRequiredItem = currentActiveQuest.info.firstRequirementItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;

        if (firstRequiredItem != "")
        {
            InventorySystem.Instance.RemoveItem(firstRequiredItem, firstRequiredAmount);
        }


        string secondtRequiredItem = currentActiveQuest.info.secondRequirementItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        if (firstRequiredItem != "")
        {
            InventorySystem.Instance.RemoveItem(secondtRequiredItem, secondRequiredAmount);
        }
    }

    private bool AreQuestRequirementsCompleted()
    {
        //check if the quest has checkpoints
        SetQuestsHasCheckpoints(currentActiveQuest);

        bool allCheckpointsCompleted = true;

        //we first check if all the checkpoint of the quest are completed
        if (currentActiveQuest.info.hasCheckpoints)
        {
            allCheckpointsCompleted = false;
            foreach (Checkpoint cp in currentActiveQuest.info.checkpoints)
            {
                if (cp.isCompleted == false)
                {
                    //if at least one is false, return false
                    allCheckpointsCompleted = false;
                    break;
                }
            }
            allCheckpointsCompleted = true;
        }

        #region ------- Particular Quests -------
        int questNumber = currentActiveQuest.info.questNumber;
        switch (questNumber)
        {
            case 7:
                if (QuestEventSystem.Instance.isQuest7Completed && allCheckpointsCompleted)
                {
                    return true;
                }
                return false;
            case 12:
                if (QuestEventSystem.Instance.isQuest12Completed && allCheckpointsCompleted)
                {
                    return true;
                }
                return false;
            case 11:
                if (QuestEventSystem.Instance.isQuest11Completed && allCheckpointsCompleted)
                {
                    return true;
                }
                return false;
            case 13:
                if (QuestEventSystem.Instance.isQuest13Completed && allCheckpointsCompleted)
                {
                    return true;
                }
                return false;
            case 15:
                if (QuestEventSystem.Instance.isQuest15Completed && allCheckpointsCompleted)
                {
                    return true;
                }
                return false;
        }
        
        #endregion



        #region ------- General Quests regarding collecting items -------
        print("Checking Requirements");

        // First Item Requirement

        string firstRequiredItem = currentActiveQuest.info.firstRequirementItem;
        int firstRequiredAmount = currentActiveQuest.info.firstRequirementAmount;
        var firstItemCounter = 0;

        
        foreach (string item in InventorySystem.Instance.itemList)
        {
              if (item == firstRequiredItem)
              {
                   firstItemCounter++;
              }
            
        }


        // Second Item Requirement -- If we don't have a second item, just set it to 0

        string secondRequiredItem = currentActiveQuest.info.secondRequirementItem;
        int secondRequiredAmount = currentActiveQuest.info.secondRequirementAmount;

        var secondItemCounter = 0;
        foreach (string item in InventorySystem.Instance.itemList)
        {
            if (item == secondRequiredItem)
            {
                secondItemCounter++;
            }
        }


        if (firstItemCounter >= firstRequiredAmount && secondItemCounter >= secondRequiredAmount && allCheckpointsCompleted)
        {
            return true;
        }
        else
        {
            return false;
        }

        #endregion

    }

    private void SetQuestsHasCheckpoints(Quest activeQuest)
    {
        activeQuest.info.hasCheckpoints = activeQuest.info.checkpoints.Count > 0;
    }

    private void StartQuestInitialDialog()
    {
        DialogSystem.Instance.OpenDialogUI();

        npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
        SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.initialDialogClips[currentDialog]);
        optionButton1Text.text = "Next";
        
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
            currentDialog++;
            CheckIfDialogDone();
        });

        optionButton2.gameObject.SetActive(false);
    }

    private void CheckIfDialogDone()
    {
        if (currentDialog == currentActiveQuest.info.initialDialog.Count - 1) // If it's the last dialog
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
            SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.initialDialogClips[currentDialog]);


            currentActiveQuest.initialDialogCompleted = true;

            SetAcceptAndDeclineOption();
        }
        else // If there are more dialogs
        {
            npcDialogText.text = currentActiveQuest.info.initialDialog[currentDialog];
            SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.initialDialogClips[currentDialog]);


            optionButton1Text.text = "Next";
            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() => {
                SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
                currentDialog++;
                CheckIfDialogDone();
            });
        }
    }

    private void SetAcceptAndDeclineOption()
    {
        optionButton1Text.text = currentActiveQuest.info.acceptOption;
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
            AcceptedQuest();
        });

        optionButton2.gameObject.SetActive(true);
        optionButton2Text.text = currentActiveQuest.info.declineOption;
        optionButton2.onClick.RemoveAllListeners();
        optionButton2.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
            DeclinedQuest();
        });
    }

    private void DeclinedQuest()
    {
        currentActiveQuest.declined = true;
        npcDialogText.text = currentActiveQuest.info.declineAnswer;
        SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.declineAnswerClip);

        CloseDialogUI();
    }

    private void AcceptedQuest()
    {
        

        // Reset checkpoints explicitly
        if (currentActiveQuest.info.hasCheckpoints)
        {
            // Ensure the checkpoints are correctly initialized
            for (int i = 0; i < currentActiveQuest.info.checkpoints.Count; i++)
            {
                currentActiveQuest.info.checkpoints[i].isCompleted = false;
            }
        }

        QuestManager.Instance.AddActiveQuest(currentActiveQuest);

        currentActiveQuest.accepted = true;
        currentActiveQuest.declined = false;

        if (currentActiveQuest.hasNoRequirements)
        {
            npcDialogText.text = currentActiveQuest.info.comeBackCompleted;
            SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.comeBackCompletedClip);
            optionButton1Text.text = "[Take Reward]";

            optionButton1.onClick.RemoveAllListeners();
            optionButton1.onClick.AddListener(() =>
            {
                SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
                ReceiveRewardAndCompleteQuest();
            });

            optionButton2.gameObject.SetActive(false);
        }
        else
        {
            // Activate tracking for Quest 7 if it's the current quest
            if (currentActiveQuest.info.questNumber == 7)
            {
                QuestEventSystem.Instance.StartQuest7Tracking();
            }

            if(currentActiveQuest.info.questNumber == 12)
            {
                QuestEventSystem.Instance.StartQuest12Tracking();
            }

            if(currentActiveQuest.info.questNumber == 11)
            {
                QuestEventSystem.Instance.StartQuest11Tracking();
            }

            if(currentActiveQuest.info.questNumber == 13)
            {
                QuestEventSystem.Instance.StartQuest13Tracking();
            }

            if (currentActiveQuest.info.questNumber == 15)
            {
                QuestEventSystem.Instance.StartQuest15Tracking();
            }

            npcDialogText.text = currentActiveQuest.info.acceptAnswer;
            SoundManager.Instance.PlayMariaVoice(currentActiveQuest.info.acceptAnswerClip);
            CloseDialogUI();
        }
    }

    private void CloseDialogUI()
    {
        optionButton1Text.text = "[Close]";
        optionButton1.onClick.RemoveAllListeners();
        optionButton1.onClick.AddListener(() => {
            SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        });

        optionButton2.gameObject.SetActive(false);
    }

    //recevie reward and go to next quest
    private void ReceiveRewardAndCompleteQuest()
    {
        if(currentActiveQuest.info == null)
        {
            Debug.Log("Quest info is null");
            return;
        }

        //Debug.Log("Current quest: " + currentActiveQuest.info.questNumber);

        QuestManager.Instance.MarkQuestAsCompleted(currentActiveQuest);

        currentActiveQuest.isCompleted = true;

        var coinsRecieved = currentActiveQuest.info.coinReward;
        print("You recieved " + coinsRecieved + " gold coins");
        InventorySystem.Instance.AddCoins(coinsRecieved);

        if (currentActiveQuest.info.rewardItem1 != "")
        {
            for (int i = 0; i < currentActiveQuest.info.rewardItem1Amount; i++)
            {
                InventorySystem.Instance.AddToInventory(currentActiveQuest.info.rewardItem1);
            }
        }

        if (currentActiveQuest.info.rewardItem2 != "")
        {
            for (int i = 0; i < currentActiveQuest.info.rewardItem2Amount; i++)
            {
                InventorySystem.Instance.AddToInventory(currentActiveQuest.info.rewardItem2);
            }
            
        }


        if (currentActiveQuest.info.questNumber == 8)
        {
            Debug.Log("Player gets the map inside NPC");
            PlayerGetsTheMap?.Invoke();
            ActivateKeyboardMap?.Invoke();

        }

        activeQuestIndex++;

        // Start Next Quest 
        if (activeQuestIndex < quests.Count)
        {
            currentActiveQuest = quests[activeQuestIndex];
            currentDialog = 0;
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
        }
        else
        {
            DialogSystem.Instance.CloseDialogUI();
            isTalkingWithPlayer = false;
            print("No more quests");
        }
    }
}
