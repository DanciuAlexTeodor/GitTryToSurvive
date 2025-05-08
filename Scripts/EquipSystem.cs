using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    // -- UI -- //

    public GameObject quickSlotsPanel;
    public GameObject numbersHolder;


    //items to reset when the player dies
    public List<GameObject> quickSlotsList = new List<GameObject>();
    public int selectedOldNumber = -1;
    public GameObject selectedItem;
    public GameObject toolHolder;
    public GameObject leftHolder;
    public GameObject WoodenTorch;

    public bool isLeftHolderEmpty = true;

    public GameObject selectedItemModel;
    public GameObject selectedLeftItemModel;
    public string selectedItemName;

    public GameObject messagePopUp;
    public bool isBackpackOpen=false;
    public bool isArrowLoadedOnBow = false;

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

    private void Start()
    {
        WoodenTorch.SetActive(false);
        PopulateSlotList();
    }

    void Update()
    {
        if (PlacementSystem.Instance.inPlacementMode ||
            ConstructionManager.Instance.inConstructionMode ||
            InventorySystem.Instance.isOpen)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectQuickSlot(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectQuickSlot(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectQuickSlot(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SelectQuickSlot(4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SelectQuickSlot(5);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SelectQuickSlot(6);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SelectQuickSlot(7);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("T pressed");
            CheckToEquipOnOfTheTorch();
        }

    }

    public string GetWeaponName()
    {
        if (selectedItem != null)
        {
            return selectedItem.name;
        }
        else
        {
            return "";
        }
    }

    public bool isFoodInPlayersHand()
    {
        List<string> food = new List<string> { "Apple", "Carrot", "Mushroom", "Turnip",
            "Pumpkin", "Tomato", "Eggplant" };

        if(selectedItem != null)
        {
            if(food.Contains(selectedItem.name))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public float GetWeaponHealth()
    {
        if(selectedItem != null)
        {
            Weapon weapon = selectedItem.GetComponent<Weapon>();
            return weapon.weaponHealth;
        }
        else
        {
            return 0;
        }
    }

    public bool isHoldingShovel()
    {
        if(selectedItem != null)
        {
            Debug.Log(selectedItem.name);
            if(selectedItem.name == "Stone_Shovel" ||
                selectedItem.name == "Iron_Shovel")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void EquipItemInLeftHolder(string objectToInstantiate)
    {
        GameObject prefabToInstantiate = Resources.Load<GameObject>(objectToInstantiate + "_Model");
        prefabToInstantiate.name = prefabToInstantiate.name.Replace("(Clone)", "");
        if(prefabToInstantiate!=null)
        {
            Debug.Log("Prefab found in Resources: " + prefabToInstantiate.name  + ". Prefab was instantiated");

            selectedLeftItemModel = Instantiate(prefabToInstantiate);
            selectedLeftItemModel.transform.SetParent(leftHolder.transform, false);
            selectedLeftItemModel.transform.SetSiblingIndex(0);
            isLeftHolderEmpty = false;
            selectedLeftItemModel.name = objectToInstantiate + "_Model";

        }
        else
        {
            Debug.LogError("Prefab not found in Resources: " + prefabToInstantiate.name);
        }

       
    }


   
    private void CheckToEquipOnOfTheTorch()
    {
        //how to check if an item is active

        if (CheckIfLeftHolderIsEmpty() && !WoodenTorch.activeSelf)
        {
            Debug.Log("Left holder is empty");
            if (InventorySystem.Instance.CheckItemAmount("WoodenTorch") != 0)
            {
                Debug.Log("You have torches in your inventory");
                WoodenTorch.SetActive(true);
                isLeftHolderEmpty = false;
            }
            else
            {
                PopUpMessage("You don't have any torches in your inventory!");
            }

        }
        else if (WoodenTorch.activeSelf)
        {
            Debug.Log("Unequip the torch");
            WoodenTorch.SetActive(false);
            isLeftHolderEmpty = true;
        }
        else
        {
            PopUpMessage("You already have an item in your left hand!");
        }
    }

    public void PopUpMessage(string message)
    {
        messagePopUp.SetActive(true);
        messagePopUp.transform.Find("Text").GetComponent<Text>().text = message;
        StartCoroutine(HideMessage());
    }

    private IEnumerator HideMessage()
    {
        yield return new WaitForSeconds(5);
        messagePopUp.SetActive(false);
    }



    public bool CheckIfLeftHolderIsEmpty()
    {
        if(leftHolder.transform.childCount == 1 && !WoodenTorch.activeSelf)
        {
            isLeftHolderEmpty = true;
        }
        else
        {
            isLeftHolderEmpty = false;
        }
        

        return isLeftHolderEmpty ;
    }

    public void decreaseWeaponHealth(int amount)
    {
        //get the weapon script of the selected item
        if(selectedItem == null)
        {
            return;
        }
        Weapon weapon = selectedItem.GetComponent<Weapon>();

        if(weapon != null)
        {
            weapon.TakeDamage(amount);
        }
        //decrease the health of the weapon
        
    }

    public void increaseWeaponHealth()
    {
        //get the weapon script of the selected item
        Weapon weapon = selectedItem.GetComponent<Weapon>();
        //decrease the health of the weapon
        weapon.IncreaseHealth();
    }

    public bool isEatble()
    {
        if(selectedItem != null)
        {
            if(selectedItem.GetComponent<InventoryItem>() != null && selectedItem.GetComponent<InventoryItem>().isEatble)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void SelectQuickSlot(int number)
    {
        if (checkIfSlotIsFull(number) == true)
        {

            if (isBackpackOpen == true)
            {
                CloseUI();
            }

            if (selectedOldNumber != number)
            {

                selectedOldNumber = number;
                if (selectedItem != null)
                {
                    //if there is already a selected item then set the previous to false
                    selectedItem.GetComponent<InventoryItem>().isSelected = false;
                }

                // Check if in placement mode and deactivate it before switching items
                /*if (PlacementSystem.Instance.inPlacementMode)
                {
                    PlacementSystem.Instance.DeactivatePlacementMode();  // Exit placement mode
                }*/

                selectedItem = GetSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem);

                //changing color of all numbers
                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = new Color32(0xFF, 0xDE, 0x00, 0xFF);
                }

                Text toBeChanged = numbersHolder.transform.Find("number" + number).transform.Find("Text").GetComponent<Text>();
                //03FF03
                toBeChanged.color = new Color32(0x03, 0xFF, 0x03, 0xFF);
            }
            else //we are trying to select the same slot
            {
                
                
                // Check if in placement mode and deactivate it before switching items
                /*if (PlacementSystem.Instance.inPlacementMode)
                {
                    PlacementSystem.Instance.DeactivatePlacementMode();  // Exit placement mode
                }*/

                selectedOldNumber = -1;
                if(selectedItem != null)
                {
                    selectedItem.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                //destroy the model in our hand
                if(selectedItemModel != null)
                {
                    //Debug.Log("Destroy item 6");
                    DestroyImmediate(selectedItemModel.gameObject);
                    selectedItemModel = null;
                }

                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = new Color32(0xFF, 0xDE, 0x00, 0xFF);
                }
            }
        }
        


    }

    private void SetEquippedModel(GameObject selectedItem)
    {
        // Remove "(Clone)" from the selected item's name if it exists
        selectedItemName = selectedItem.name.Replace("(Clone)", "");
        //Debug.Log("Selected item name: " + selectedItemName);

        // Load the corresponding model from the Resources folder
        //if selectedItemName contains "Seed" then we load a hand
        if(selectedItemName.Contains("Seed"))
        {
            selectedItemName = "Hand";
        }

        if (selectedItemName.Contains("Shovel") && !PlacementSystem.Instance.inPlacementMode)
        {
            PlacementSystem.Instance.ActivatePlacementMode("Dirt_PileModel");
        }

        if(selectedItemName.Contains("Backpack"))
        {
            OpenUI(selectedItemName);
        }

        
            GameObject itemModelPrefab = Resources.Load<GameObject>(selectedItemName + "_Model");
            
            if (itemModelPrefab == null)
            {
                Debug.LogError("Item model not found in Resources: " + selectedItemName + "_Model");
                return; // Exit the method if the model is not found
            }



            //destroy the model in our hand
            if (selectedItemModel != null)
            {
                Debug.Log("Destroy item 7");
                DestroyImmediate(selectedItemModel.gameObject);
                selectedItemModel = null;
            }

        selectedItemModel = Instantiate(itemModelPrefab);
            
        selectedItemModel.transform.SetParent(toolHolder.transform, false);

        if(selectedItemName.Contains("Bow"))
        {
            if (isArrowLoadedOnBow)
            {
                Debug.Log("There is an arrow loaded:");
            }
            else
            {
                Debug.Log("There is no arrow loaded:");
            }

            if (isArrowLoadedOnBow)
            {
                Bow.Instance.InstantiateArrow();
            }
        }
        
        
    }

    //aici
    private void OpenUI(string backpackName)
    {

        if(backpackName.Contains("Big"))
        {
            //activate the big backpack slots
            InventorySystem.Instance.OnOfBigBackpackSlots(true);

        }
        else
        {
            //deactivate the big backpack slots
            Debug.Log("Open small backpack");
            InventorySystem.Instance.OnOfBigBackpackSlots(false);
        }

        InventorySystem.Instance.backpackScreenUI.SetActive(true);

        CursorManager.Instance.FreeCursor();
        isBackpackOpen = true;
    }

    private void CloseUI()
    {
        InventorySystem.Instance.backpackScreenUI.SetActive(false);

        
        isBackpackOpen = false;

        CursorManager.Instance.LockCursor();

    }

    GameObject GetSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.GetChild(0).gameObject;
    }
     
    bool checkIfSlotIsFull(int slotNumber)
    {
        //Debug.Log("Slot number: " + slotNumber);
        if (quickSlotsList[slotNumber-1].transform.childCount > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public async void DestroyWeaponInHand()
    {
        Debug.Log("Destroy item 8");
        
        
        Destroy(selectedItemModel.gameObject);
        await Task.Delay(3000);

        CursorManager.Instance.LockCursor();
    }

    public bool IsPlayerHoldingBottle()
    {
        if(selectedItemModel != null)
        {
            if(selectedItemModel.gameObject.name == "LeatherBottle_Model(Clone)" ||
                selectedItemModel.gameObject.name == "LeatherBottle_Model")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    public void AddToQuickSlots(GameObject itemToEquip)
    {
        // Find next free slot
        GameObject availableSlot = FindNextEmptySlot();
        // Set transform of our object
        itemToEquip.transform.SetParent(availableSlot.transform, false);
        // Getting clean name

        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }


    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject();
    }

    public bool CheckIfFull()
    {

        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        if (counter == 7)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public int GetWeaponDamage()
    {
        if (selectedItem != null)
        {
            return selectedItem.GetComponent<Weapon>().weaponDamage;
        }
        else
        {
            return 0;
        }
    }

    public bool IsHoldingWeapon()
    {
        if (selectedItem != null)
        {
            //we check if the selected item has a "Weapon" script
            if(selectedItem.GetComponent<Weapon>() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }

    public bool IsPlayerHoldingSeed()
    {
        if(selectedItemModel!=null)
        {
            switch(selectedItemModel.gameObject.name)
            {
                case "Hand_Model(Clone)":
                    return true;
                case "Hand_Model":
                    return true;
                default:
                    return false;

            }
        }
        else
        {
            return false;
        }
    }

    public bool IsHoldingLootWeapon()
    {
        if(selectedItem!= null)
        {
            if(selectedItem.GetComponent<Weapon>() != null &&
                selectedItem.GetComponent<Weapon>().isLootWeapon == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void DestroyAllItemsInsideQuickslots()
    {

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }

        selectedOldNumber = -1;
      
        if(selectedItemModel != null)
        {
            Destroy(selectedItemModel.gameObject);
        }

        if(toolHolder.transform.childCount > 0)
        {
            Destroy(toolHolder.transform.GetChild(0).gameObject);
        }

        if(leftHolder.transform.childCount > 0)
        {
            Destroy(leftHolder.transform.GetChild(0).gameObject);
        }
      

        isLeftHolderEmpty = true;

        if(selectedItemModel != null)
        {
            Destroy(selectedItemModel.gameObject);
        }

        isBackpackOpen = false;
        isArrowLoadedOnBow = false;


        
    }

}