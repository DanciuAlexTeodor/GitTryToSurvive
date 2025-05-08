using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class FurnaceUIManager : MonoBehaviour
{
    public static FurnaceUIManager Instance { get; set; }

    public List<FurnacePieceData> furnaceList = new List<FurnacePieceData>();

    public GameObject furnacePanel; //the whole UI

    public Button cookButton;
    public Button exitButton;

    public GameObject inputSlot1, inputSlot2, inputSlot3;
    public GameObject outputSlot1, outputSlot2, outputSlot3;
    public GameObject fuelSlot;

    public Furnace selectedFurnace;

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

    public void UpdateFurnaceListData()
    {
        Furnace[] furnacesInScene = FindObjectsOfType<Furnace>();

        foreach(Furnace furnace in  furnacesInScene)
        {
            FurnacePieceData existingData = furnaceList.Find(data => data.furnace == furnace);

            if (existingData != null)
            {
                // Update the existing campfire data
                string name = furnace.gameObject.name;
                name.Replace("(Clone)", ""); // Remove (Clone) from the name
                existingData.furnaceName = furnace.gameObject.name;
                existingData.inputBeingCooked1 = furnace.inputBeingCooked1;
                existingData.inputBeingCooked2 = furnace.inputBeingCooked2;
                existingData.inputBeingCooked3 = furnace.inputBeingCooked3;
                existingData.isCooking = furnace.isCooking;
                existingData.cookingTimer = furnace.cookingTimer;
                existingData.cookingTimerAux = furnace.cookingTimerAux;
                existingData.storedItems = new List<string>(furnace.items); // Update stored items
                existingData.position = furnace.transform.position; // Update position
                existingData.rotation = furnace.transform.rotation; // Update rotation
            }
            else
            {
                // Create new campfire data and add it to the list
                FurnacePieceData newData = new FurnacePieceData(
                    furnace,
                    furnace.gameObject.name,
                    furnace.transform.position,
                    furnace.transform.rotation,
                    furnace.transform.parent != null ? furnace.transform.parent.name : null
                );
                furnaceList.Add(newData);
            }
        }

    }

    private bool FuelAndInputAreValid()
    {
        // Get references to the input items from all 3 input slots
        InventoryItem input1 = inputSlot1.GetComponentInChildren<InventoryItem>();
        InventoryItem input2 = inputSlot2.GetComponentInChildren<InventoryItem>();
        InventoryItem input3 = inputSlot3.GetComponentInChildren<InventoryItem>();

        // Get reference to the fuel item from the fuel slot
        InventoryItem fuel = fuelSlot.GetComponentInChildren<InventoryItem>();

        // Check if fuel is valid and at least one input slot has a valid item
        if (fuel != null)
        {
            // Check if the fuel is valid
            bool isValidFuel = cookingData.validFuels.Contains(fuel.thisName);

            // Check if any of the input items are valid
            bool isValidInput1 = input1 != null && cookingData.validInputs.Any(cookableInput => cookableInput.name == input1.thisName);
            bool isValidInput2 = input2 != null && cookingData.validInputs.Any(cookableInput => cookableInput.name == input2.thisName);
            bool isValidInput3 = input3 != null && cookingData.validInputs.Any(cookableInput => cookableInput.name == input3.thisName);

            // Return true if fuel is valid and at least one input is valid
            if (isValidFuel)
            {
                return true;
            }
        }

        // Return false if fuel or inputs are not valid
        return false;
    }


    public void CookButtonPressed()
    {
        InventoryItem input1 = inputSlot1.GetComponentInChildren<InventoryItem>();
        InventoryItem input2 = inputSlot2.GetComponentInChildren<InventoryItem>();
        InventoryItem input3 = inputSlot3.GetComponentInChildren<InventoryItem>();

        InventoryItem fuel = fuelSlot.GetComponentInChildren<InventoryItem>();
        selectedFurnace.StartCooking(input1,input2,input3);


        

        //this is ok for now
        if (fuel == null)
        {
            Debug.LogError("Fuel is null");
            return;
        }

        if(input1!=null)
        {
            DestroyImmediate(input1.gameObject);
        }

        if(input2!=null)
        {
            DestroyImmediate(input2.gameObject);
        }
        
        if(input3!=null)
        {
            DestroyImmediate(input3.gameObject);
        }
       
        DestroyImmediate(fuel.gameObject);
        Debug.Log("INPUT AND FUEL DESTROYED");

        CloseUI();

    }

    public void OpenUI()
    {
        furnacePanel.SetActive(true);
        isUIOpen = true;

        //CursorManager.Instance.FreeCursor();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        

    }

    public void CloseUI()
    {
        furnacePanel.SetActive(false);
        isUIOpen = false;

        //CursorManager.Instance.LockCursor();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SaveFurnaceItems(selectedFurnace); // Save items before closing the UI

    }

    public void LoadFurnaceItems(Furnace furnace)
    {
        // Clear UI slots
        ClearSlot(inputSlot1);
        ClearSlot(inputSlot2);
        ClearSlot(inputSlot3);
        ClearSlot(fuelSlot);
        ClearSlot(outputSlot1);
        ClearSlot(outputSlot2);
        ClearSlot(outputSlot3);

        int[] input = { 0, 0, 0 };   // Track which input slots are filled
        int[] output = { 0, 0, 0 };  // Track which output slots are filled

        foreach (string itemName in furnace.items)
        {
            // Instantiate the item and place it in the appropriate slot (input, fuel, or output)
            GameObject itemPrefab = Resources.Load<GameObject>(itemName);

            // Check if it's an input item
            if (IsInputItem(itemName))
            {
                if (input[0] == 0)
                {
                    Instantiate(itemPrefab, inputSlot1.transform.position, inputSlot1.transform.rotation, inputSlot1.transform);
                    input[0] = 1;
                }
                else if (input[1] == 0)
                {
                    Instantiate(itemPrefab, inputSlot2.transform.position, inputSlot2.transform.rotation, inputSlot2.transform);
                    input[1] = 1;
                }
                else if (input[2] == 0)
                {
                    Instantiate(itemPrefab, inputSlot3.transform.position, inputSlot3.transform.rotation, inputSlot3.transform);
                    input[2] = 1;
                }
            }
            // Check if it's a fuel item
            else if (IsFuelItem(itemName))
            {
                Instantiate(itemPrefab, fuelSlot.transform.position, fuelSlot.transform.rotation, fuelSlot.transform);
            }
            // Otherwise, it's an output item
            else
            {
                if (output[0] == 0)
                {
                    Instantiate(itemPrefab, outputSlot1.transform.position, outputSlot1.transform.rotation, outputSlot1.transform);
                    output[0] = 1;
                }
                else if (output[1] == 0)
                {
                    Instantiate(itemPrefab, outputSlot2.transform.position, outputSlot2.transform.rotation, outputSlot2.transform);
                    output[1] = 1;
                }
                else if (output[2] == 0)
                {
                    Instantiate(itemPrefab, outputSlot3.transform.position, outputSlot3.transform.rotation, outputSlot3.transform);
                    output[2] = 1;
                }
            }
        }
    }


    public void SaveFurnaceItems(Furnace furnace)
    {
        furnace.items.Clear();

        // Save the item in the input, fuel, and output slots
        SaveItemInSlot(inputSlot1, furnace);
        SaveItemInSlot(inputSlot2, furnace);
        SaveItemInSlot(inputSlot3, furnace);
        SaveItemInSlot(fuelSlot, furnace);
        SaveItemInSlot(outputSlot1, furnace);
        SaveItemInSlot(outputSlot2, furnace);
        SaveItemInSlot(outputSlot3, furnace);
    }

    private void SaveItemInSlot(GameObject slot, Furnace furnace)
    {
        InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            // Add item to campfire's item list
            furnace.items.Add(item.thisName);
        }
    }

    private void ClearSlot(GameObject slot)
    {
        InventoryItem item = slot.GetComponentInChildren<InventoryItem>();
        if (item != null)
        {
            Debug.Log("Destroy item 17");
            Destroy(item.gameObject);
        }
    }

    private bool IsInputItem(string itemName)
    {
        return cookingData.validInputs.Any(input => input.name == itemName);
    }

    private bool IsFuelItem(string itemName)
    {
        return cookingData.validFuels.Contains(itemName);
    }
}
