using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class CampfireUIManager : MonoBehaviour
{
    public static CampfireUIManager Instance { get; set; }

    public List<CampfirePieceData> campfireList = new List<CampfirePieceData>();

    public GameObject campfirePanel; //the whole UI

    public Button cookButton;
    public Button exitButton;

    public GameObject inputSlot;
    public GameObject outputSlot;
    public GameObject fuelSlot;

    public Campfire selectedCampfire;

    public bool isUIOpen;

    public CookingData cookingData;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        if (FuelAndInputAreValid())
        {
            cookButton.interactable = true;
        }
        else
        {
            cookButton.interactable = false;
        }
        
    }

    private bool FuelAndInputAreValid()
    {
        InventoryItem input = inputSlot.GetComponentInChildren<InventoryItem>();
        InventoryItem fuel = fuelSlot.GetComponentInChildren<InventoryItem>();


        if(fuel != null)
        {
            if(input == null && cookingData.validFuels.Contains(fuel.thisName))
            {
                //Debug.Log("input null but valid fuel");
                return true;
            }

            if (cookingData.validFuels.Contains(fuel.thisName) &&
                cookingData.validInputs.Any(cookableInput => cookableInput.name == input.thisName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void CookButtonPressed()
    {
        InventoryItem input = inputSlot.GetComponentInChildren<InventoryItem>();
        InventoryItem fuel = fuelSlot.GetComponentInChildren<InventoryItem>();
        selectedCampfire.StartCooking(input);

        

        if (fuel == null)
        {
            return;
        }

        if (input)
        {
            DestroyImmediate(input.gameObject);
        }
        DestroyImmediate(fuel.gameObject);

        CloseUI();

    }

    public void OpenUI()
    {
        campfirePanel.SetActive(true);
        isUIOpen = true;

        CursorManager.Instance.FreeCursor();

    }

    public void CloseUI()
    {
        campfirePanel.SetActive(false);
        isUIOpen = false;

        CursorManager.Instance.LockCursor();

        SaveCampfireItems(selectedCampfire); // Save items before closing the UI

    }


    public void UpdateCampfireListData()
    {
        // Find all campfire instances currently in the scene
        Campfire[] campfiresInScene = FindObjectsOfType<Campfire>();

        // Iterate through each campfire and either update the existing data or add new data to the list
        foreach (Campfire campfire in campfiresInScene)
        {
            // Check if this campfire already exists in the campfireList
            CampfirePieceData existingData = campfireList.Find(data => data.campfire == campfire);

            if (existingData != null)
            {
                // Update the existing campfire data
                string name = campfire.gameObject.name;
                name.Replace("(Clone)", ""); // Remove (Clone) from the name
                existingData.campfireName = campfire.gameObject.name;
                existingData.inputBeingCooked = campfire.inputBeingCooked;
                existingData.isCooking = campfire.isCooking;
                existingData.cookingTimer = campfire.cookingTimer;
                existingData.cookingTimerAux = campfire.cookingTimerAux;
                existingData.storedItems = new List<string>(campfire.items); // Update stored items
                existingData.position = campfire.transform.position; // Update position
                existingData.rotation = campfire.transform.rotation; // Update rotation
            }
            else
            {
                // Create new campfire data and add it to the list
                CampfirePieceData newData = new CampfirePieceData(
                    campfire,
                    campfire.gameObject.name,
                    campfire.transform.position,
                    campfire.transform.rotation,
                    campfire.transform.parent != null ? campfire.transform.parent.name : null
                );
                campfireList.Add(newData);
            }
        }

        //Debug.Log("Campfire data updated successfully for all campfires in the scene.");
    }




    public void LoadCampfireItems(Campfire campfire)
    {
        // Clear UI slots
        ClearSlot(inputSlot);
        ClearSlot(fuelSlot);
        ClearSlot(outputSlot);

        foreach (string itemName in campfire.items)
        {
            // Instantiate the item and place it in the appropriate slot (input, fuel, or output)
            GameObject itemPrefab = Resources.Load<GameObject>(itemName);

            // Here you can implement logic to place items in the appropriate slots, for example:
            if (IsInputItem(itemName))
            {
                Instantiate(itemPrefab, inputSlot.transform.position, inputSlot.transform.rotation, inputSlot.transform);
            }
            else if (IsFuelItem(itemName))
            {
                Instantiate(itemPrefab, fuelSlot.transform.position, fuelSlot.transform.rotation, fuelSlot.transform);
            }
            else
            {
                Instantiate(itemPrefab, outputSlot.transform.position, outputSlot.transform.rotation, outputSlot.transform);
            }
        }
    }

    public void SaveCampfireItems(Campfire campfire)
    {
        campfire.items.Clear();

        // Save the item in the input, fuel, and output slots
        SaveItemInSlot(inputSlot, campfire);
        SaveItemInSlot(fuelSlot, campfire);
        SaveItemInSlot(outputSlot, campfire);

        for(int i = 0; i < campfireList.Count; i++)
        {
            if (campfireList[i].campfire == campfire)
            {
                campfireList[i].storedItems = campfire.items;
            }
        }
    }

    private void SaveItemInSlot(GameObject slot, Campfire campfire)
    {
        InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            // Add item to campfire's item list
            campfire.items.Add(item.thisName);
        }
    }

    private void ClearSlot(GameObject slot)
    {
        InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            Debug.Log("Destroy item 10");
            Destroy(item.gameObject);
        }
    }

    public bool IsInputItem(string itemName)
    {
        return cookingData.validInputs.Any(input => input.name == itemName);
    }

    public bool IsFuelItem(string itemName)
    {
        return cookingData.validFuels.Contains(itemName);
    }
}
