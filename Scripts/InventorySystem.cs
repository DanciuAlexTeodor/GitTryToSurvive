using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; }

    public GameObject inventoryScreenUI;
    public GameObject backpackScreenUI;

    public bool isOpen;

    public List<GameObject> slotList = new List<GameObject>();

    //0 is available, 1 is not available
    //public List<int> availableSlots = new List<int>();

    private GameObject itemToAdd;
    private GameObject whatSlotToEquip;

    //public bool isFull;

    //Pickup Popup
    public GameObject pickupAlert;
    public GameObject moneyPopUp;
    public TextMeshProUGUI pickupName;
    public Image pickupImage;
    public GameObject ItemInfoUi;


    //Items to restart when the player dies
    public List<string> itemList = new List<string>();
    public List<float> itemHealthList = new List<float>();
    public List<string> inventoryItems;
    public List<string> itemsPickedUp;
    public GameObject shieldSlot;
    public GameObject helmetSlot;
    public GameObject chestSlot;
    public GameObject legsSlot;
    //restart values to 0
    public float helmetArmorValue;
    public float chestArmorValue;
    public float legArmorValue;


    //declare a text mesh pro text
    public TextMeshProUGUI coinAmount;
    public GameObject helmetVision;
    public Image legsImage;

   
    


    private void Awake()
    {

        ReCalculateList();
        //if there is already an instance of the inventory system
        //then destroy this instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //set the instance to this
            Instance = this;
        }
    }

    void Start()
    {
        coinAmount.text = "10";
        //isFull = false;
        isOpen = false;
        PopulateSlotList();
        Cursor.visible = false;
    }

    //activate and dezactivate the big backpack slots based on the backpack i have in my hand
    public void OnOfBigBackpackSlots(bool isActivating)
    {
        // Loop through slotList and activate slots from 30 to 39
        for (int i = 33; i < 41; i++)  // Array index starts from 0, so slot 30 is at index 29
        {
            if (i < slotList.Count)  // Make sure the index is within the bounds of the list
            {
                slotList[i].SetActive(isActivating);  // Activate the slot
                //if isActivating = 1 => the slot is available
                //if isActivating = 0 => the slot is not available

            }
        }
    }

    public string GetAmountOfMoney()
    {
        return coinAmount.text;
    }

    private void DestroyAllChildrenInSlotsAndZones()
    {
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
    }

    public void ResetInventory()
    {
        itemList = new List<string>();
        itemHealthList = new List<float>();
        inventoryItems = new List<string>();
        itemsPickedUp = new List<string>();
        DestroyAllChildrenInSlotsAndZones();
}


    public void AddCoins(int amount)
    {
        TrigerMoneyPopUp(amount);
        int currentAmount = Int32.Parse(coinAmount.text);
        currentAmount += amount;
        coinAmount.text = currentAmount.ToString();
    }

    public void SetCoins(int amount)
    {
        coinAmount.text = amount.ToString();
    }

    //get the amount of coins in integer format
    public int CheckCoins()
    {
        return Int32.Parse(coinAmount.text);
    }


    public int CheckItemAmount(string name)
    {
        int itemCounter = 0;

        foreach (string item in itemList)
        {
            if (item == name)
            {
                itemCounter++;
            }
        }

        return itemCounter;
    }


    private void PopulateSlotList()
    {
        int i = 0;
        //loop through all the children of the inventory screen
        //and add the slots to the slot list
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                //Debug.Log(i + " Slot found: " + child.name);
                slotList.Add(child.gameObject);
                i++;
            }
        }

        foreach (Transform child in backpackScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {

                //Debug.Log(i + "Slot found: " + child.name);
                slotList.Add(child.gameObject);
                i++;
            }
        }
    }

    public float GetArmorProtection()
    {
        return (helmetArmorValue + chestArmorValue + legArmorValue);
    }

    public string GetShieldName()
    {
        if(shieldSlot.transform.childCount > 0)
        {
            return shieldSlot.transform.GetChild(0).name;
        }
        else
        {
            return "";
        }
    }

    public void DecreaseShieldHealth(float damage)
    {
        if(shieldSlot.transform.childCount > 0)
        {
            shieldSlot.transform.GetChild(0).GetComponent<Weapon>().TakeDamage((int)damage);
        }
    }

    public void DecreaseArmorHealth(float damageTaken)
    {

        int damageToArmor;
        damageToArmor = (int)(damageTaken / 2);

        if(helmetSlot.transform.childCount > 0)
        {
            helmetSlot.transform.GetChild(0).GetComponent<Weapon>().TakeDamage(damageToArmor);
        }
       

        if(chestSlot.transform.childCount > 0)
        {
            chestSlot.transform.GetChild(0).GetComponent<Weapon>().TakeDamage(damageToArmor);
        }

        if(legsSlot.transform.childCount > 0)
        {
            legsSlot.transform.GetChild(0).GetComponent<Weapon>().TakeDamage(damageToArmor);
        }
    }

    public List<int> CheckArmor()
    {
        List<int> armorVectorForSpeedDecrease = new List<int> { 0, 0, 0, 0 };
        if(shieldSlot.transform.childCount == 0 && EquipSystem.Instance.isLeftHolderEmpty==false && EquipSystem.Instance.leftHolder.transform.GetChild(0).gameObject.name.Contains("Shield"))
        {
            armorVectorForSpeedDecrease[0] = 0;
            EquipSystem.Instance.isLeftHolderEmpty=true;
            if (!EquipSystem.Instance.CheckIfLeftHolderIsEmpty())
            {
                Debug.Log("Destroying shield");
                DestroyImmediate(EquipSystem.Instance.leftHolder.transform.GetChild(0).gameObject);
            }
        }
        else
        {
            if(shieldSlot.transform.childCount > 0)
            {
                armorVectorForSpeedDecrease[0] = 1;
                EquipSystem.Instance.isLeftHolderEmpty=false;
            }
        }

        if (legsSlot.transform.childCount > 0)
        {
            armorVectorForSpeedDecrease[1] = 1;
            legArmorValue = legsSlot.transform.GetChild(0).GetComponent<InventoryItem>().armorValue;
            legsImage.enabled = false;
        }
        else
        {
            armorVectorForSpeedDecrease[1] = 0;
            legArmorValue = 0;
            legsImage.enabled = true;
        }

        if (helmetSlot.transform.childCount > 0)
        {
            armorVectorForSpeedDecrease[2] = 1;
            helmetArmorValue = helmetSlot.transform.GetChild(0).GetComponent<InventoryItem>().armorValue;
            helmetVision.SetActive(true);
        }
        else
        {
            armorVectorForSpeedDecrease[2] = 0;
            helmetArmorValue = 0;
            helmetVision.SetActive(false);
        }

        if(chestSlot.transform.childCount > 0)
        {
            armorVectorForSpeedDecrease[3] = 1;
            chestArmorValue = chestSlot.transform.GetChild(0).GetComponent<InventoryItem>().armorValue;
        }
        else
        {
            armorVectorForSpeedDecrease[3] = 0;
            chestArmorValue = 0;
        }

        return armorVectorForSpeedDecrease;
    }

    void Update()
    {
        //if the inventory is not open and the player presses the G key
        //then open the inventory
        if (Input.GetKeyDown(KeyCode.G) && !isOpen &&
            !ConstructionManager.Instance.inConstructionMode &&
            !PlacementSystem.Instance.inPlacementMode)
        {
            OpenUI();
        }
        else if (Input.GetKeyDown(KeyCode.G) && isOpen)
        {
            CloseUI();
        }


        ///i use this function for 2 purposes
        List<int> vectorForNothing = CheckArmor();
    }

    public void OpenUI()
    {

        inventoryScreenUI.SetActive(true);
        isOpen = true;
        CursorManager.Instance.FreeCursor();
       
    }

    public void CloseUI()
    {
        inventoryScreenUI.SetActive(false);
        isOpen = false;

        CursorManager.Instance.LockCursor();

        
    }

    //we load all the data from inventory that was saved in the save manager
    public void AddSavedData(List<string> savedItemList, List<float> savedItemHealthList)
    {
        // Ensure that itemList is properly initialized and matches the size of the slotList
        if (itemList.Count < slotList.Count)
        {
            // Initialize itemList to match the size of slotList
            itemList = new List<string>(new string[slotList.Count]);
            itemHealthList = new List<float>(new float[slotList.Count]);
        }

        // Ensure that the savedItemList has the same count as the slotList
        for (int i = 0; i < savedItemList.Count && i < slotList.Count; i++)
        {
            // Clear the current slot if it has any child items
            if (slotList[i].transform.childCount > 0)
            {
                // Remove the existing item if any
                DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
            }

            // Add the saved item only if it is not null or empty
            if (!string.IsNullOrEmpty(savedItemList[i]))
            {
                // Load the saved item from resources and place it in the slot
                GameObject itemToLoad = Resources.Load<GameObject>(savedItemList[i]);
                itemToLoad.name = itemToLoad.name.Replace("(Clone)", "");

                if (itemToLoad != null)
                {
                    // Instantiate the item and set its parent to the slot
                    GameObject instantiatedItem = Instantiate(itemToLoad, slotList[i].transform);

                    // Reset local position and rotation to make sure it fits correctly in the slot
                    instantiatedItem.transform.localPosition = Vector3.zero;
                    instantiatedItem.transform.localRotation = Quaternion.identity;
                    instantiatedItem.name = instantiatedItem.name.Replace("(Clone)", "");


                    // Update itemList with the saved item
                    itemList[i] = savedItemList[i];
                    itemList[i] = itemList[i].Replace("(Clone)", "");
                    itemHealthList[i] = savedItemHealthList[i];
                    Weapon weaponComponent = instantiatedItem.GetComponent<Weapon>();

                    if (weaponComponent != null)
                    {
                        weaponComponent.weaponHealth = savedItemHealthList[i];
                        if(weaponComponent.slider!=null)
                        {
                            weaponComponent.slider.value = weaponComponent.weaponHealth / weaponComponent.maximumWeaponHealth;
                        }
                        

                    }
                }
                else
                {
                    Debug.LogWarning("Could not find item in resources: " + savedItemList[i]);
                }
            }
            else
            {
                // If there is no item saved in this slot, keep the slot empty in itemList
                itemList[i] = null;
                itemHealthList[i] = 0;
            }
        }

        // After loading all the items, refresh the inventory, crafting system, and quest manager
        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
        QuestManager.Instance.RefreshTrackerList();

        //Debug.Log("Inventory loaded with saved data.");
    }



    public void AddToInventory(string itemName)
    {
        if (itemName == null || itemName == "")
            return;


        whatSlotToEquip = FindNextEmptySlot();

        //cand joc din scene mode trebuie sa dezactivez codul acesta
        /*if (!SaveManager.Instance.isLoading)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUpItem);
        }*/

        //cand joc jocul din mainScene trebuie sa dezactivez codul acesta
        SoundManager.Instance.PlaySound(SoundManager.Instance.pickUpItem);

        Debug.Log("Item picked up: " + itemName);

        
        itemToAdd = Instantiate(Resources.Load<GameObject>(itemName), whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
        itemToAdd.transform.SetParent(whatSlotToEquip.transform);
        itemToAdd.name = itemName;
        itemList.Add(itemName);


        if (SelectionManager.Instance.healthToBeRestored > 0)
        {
            Debug.Log("Health to be restored: " + SelectionManager.Instance.healthToBeRestored);
            itemToAdd.GetComponent<Weapon>().weaponHealth = SelectionManager.Instance.healthToBeRestored;
            itemToAdd.GetComponent<Weapon>().slider.value = itemToAdd.GetComponent<Weapon>().weaponHealth / itemToAdd.GetComponent<Weapon>().maximumWeaponHealth;
        }



        TriggerPickupPopUp(itemName, itemToAdd.GetComponent<Image>().sprite);


        QuestManager.Instance.RefreshTrackerList();
        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();

    }



    private GameObject FindNextEmptySlot()
    {
        int index = 0;
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return new GameObject();

    }

    //check if there are enough empty slots to add the item
    public bool CkeckSlotsAvailable(int emptyNeeded)
    {
        int countEmptySlots = 0, slotIndex = 0;
        foreach (GameObject slot in slotList)
        {
            //if the slot is empty

            if (slot.transform.childCount == 0)
            {
                //Checking the slots of the inventory
                if (slotIndex < 21)
                {
                    countEmptySlots++;
                }
                else
                {
                    //we are trying to access the big backpack slots
                    //so we need to check if the big backpack is open
                    //checking the slots of the backpack
                    if (EquipSystem.Instance.isBackpackOpen)
                    {
                        countEmptySlots++;
                    }
                }
            }



            slotIndex++;
        }
        //if the amount of empty slots is less than the amount of empty slots needed
        //then return false, because we can't add the item
        if (emptyNeeded > countEmptySlots)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void RemoveItem(string nameToRemove, int amountToRemove)
    {
        int counter = amountToRemove;
        for (var i = slotList.Count - 1; i >= 0; i--)
        {
            if (slotList[i].transform.childCount > 0)
            {
                Debug.Log("Checking slot: " + i + " object: " + slotList[i].transform.GetChild(0).name + " == " + nameToRemove + " counter: " + counter);
                if (slotList[i].transform.GetChild(0).name == nameToRemove && counter != 0)
                {
                    Debug.Log("Remove from slot:" + i + " object: " + slotList[i].transform.GetChild(0).name + " == " + nameToRemove + " counter: " + counter);
                    DestroyImmediate(slotList[i].transform.GetChild(0).gameObject);
                    counter--;
                }

            }
        }

        ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void ReCalculateList()
    {
        // Clear the list to make sure we start fresh
        itemList.Clear();
        itemHealthList.Clear();

        // Iterate through each slot in the inventory
        foreach (GameObject slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                // Get the name of the first child (the item in the slot)
                string name = slot.transform.GetChild(0).name;
                name.Replace("Clone", "");
                //check if the slot has a weapon component
                if (slot.transform.GetChild(0).GetComponent<Weapon>() != null)
                {
                    float health = slot.transform.GetChild(0).GetComponent<Weapon>().weaponHealth;
                    itemHealthList.Add(health);
                }
                else
                {
                    itemHealthList.Add(0);
                }


                // Remove "(Clone)" from the name if it exists
                string result = name.Replace("(Clone)", "");

                // Add the cleaned name to the itemList
                itemList.Add(result);

            }
            else
            {
                // Add a placeholder if the slot is empty (e.g., null or "")
                itemList.Add(null);
                itemHealthList.Add(0);
            }
        }

        // Print all items in the list to the console
        for (int i = 0; i < itemList.Count; i++)
        {
            Console.WriteLine(itemList[i]);
        }



        QuestManager.Instance.RefreshTrackerList();
    }

    #region -------- Pickup PopUp ------------

    void TriggerPickupPopUp(string itemName, Sprite itemSprite)
    {
        pickupName.text = " +1 " + itemName;
        pickupImage.sprite = itemSprite;
        pickupAlert.SetActive(true);
        StartCoroutine(DisablePickupAlert());
    }

    void TrigerMoneyPopUp(int amount)
    {
        moneyPopUp.SetActive(true);
        moneyPopUp.GetComponentInChildren<TextMeshProUGUI>().text = "+ " + amount;
        StartCoroutine(DisableMoneyPopUp());
    }

    IEnumerator DisablePickupAlert()
    {
        yield return new WaitForSeconds(2f);
        pickupAlert.SetActive(false);
    }

    IEnumerator DisableMoneyPopUp()
    {
        yield return new WaitForSeconds(2f);
        moneyPopUp.SetActive(false);
    }

    #endregion
}
