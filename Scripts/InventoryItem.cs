using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading.Tasks;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    //DOAMNE FERESTE SA ACTIVEZ ASTA!!!
    //public static InventoryItem Instance { get; set; }

    // --- Is this item trashable --- //
    public bool isTrashable;

    // --- Item Info UI --- //
    private GameObject itemInfoUI;

    private Text itemInfoUI_itemName;
    private Text itemInfoUI_itemDescription;
    

    public string thisName, thisDescription, thisFunctionality;

    // --- Consumption --- //
    private GameObject itemPendingConsumption;
    public bool isConsumable;
    public bool isEatble;
    public bool isUsedToAtrractFarmAnimals;

    // --- Effects --- //
    //cu cat imi creste viata, ....
    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;
    public float armorValue;


    //--- Equipping ---//
    public bool isEquippable;
    public bool isInsideQuickSlot; //in inside a quickslot
    private GameObject itemPendingEquipping;

    public bool isSelected;
    public bool isUsable;
    public bool isArrow;

   

    private void Start()
    {
        itemInfoUI = InventorySystem.Instance.ItemInfoUi;
        itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<Text>();
        itemInfoUI_itemDescription = itemInfoUI.transform.Find("itemDescription").GetComponent<Text>();
    }

    void Update()
    {
        if (isSelected)
        {
            gameObject.GetComponent<DragDrop>().enabled = false;
        }
        else
        {
            gameObject.GetComponent<DragDrop>().enabled = true;
        }
    }

    // Triggered when the mouse enters into the area of the item that has this script.
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        //itemInfoUI_itemFunctionality.text = thisFunctionality;
    }

    // Triggered when the mouse exits the area of the item that has this script.
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    // Triggered when the mouse is clicked over the item that has this script.
    public void OnPointerDown(PointerEventData eventData)
    {
        //Right Mouse Button Click on
        if (eventData.button == PointerEventData.InputButton.Right && PlayerState.Instance.isEating==false)
        {
            if(isConsumable)
            {
                itemPendingConsumption = gameObject;
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);
            }


            if (gameObject.name == "Arrow" &&  EquipSystem.Instance.GetWeaponName().Contains("Bow") && Bow.Instance.isArrowLoaded == false)
            {
                isArrow = true;
                //Debug.Log("Arrow loaded");
                InventorySystem.Instance.ReCalculateList();
                Bow.Instance.InstantiateArrow();
                
            }
            else
            {
                isArrow = false;
            }

            if (isEquippable && isInsideQuickSlot == false && EquipSystem.Instance.CheckIfFull() == false)
            {
                EquipSystem.Instance.AddToQuickSlots(gameObject);

                isInsideQuickSlot = true;
            }

            if(isUsable)
            {
                //ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                gameObject.SetActive(false);
                UseItem();
            }
        }
    }

    private void UseItem()
    {
        itemInfoUI.SetActive(false);

        
        InventorySystem.Instance.inventoryScreenUI.SetActive(false);

        CraftingSystem.Instance.isOpen=false;
        CraftingSystem.Instance.craftingScreenUI.SetActive(false);
        CraftingSystem.Instance.toolsScreenUI.SetActive(false);
        CraftingSystem.Instance.survivalScreenUI.SetActive(false);
        CraftingSystem.Instance.utilityScreenUI.SetActive(false);
        CraftingSystem.Instance.buildScreenUI.SetActive(false);
        CraftingSystem.Instance.attackScreenUI.SetActive(false);

        InventorySystem.Instance.isOpen = false;
        CursorManager.Instance.LockCursor();

        if(gameObject.name.Contains("Clone"))
        {
            gameObject.name = gameObject.name.Replace("(Clone)", "");
        }

        switch (gameObject.name)
        {

            case "Foundation":
                ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("FoundationModel");
                break;
            case "Wall":           
                ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("WallModel");
                break;
           
            case "Door":
                ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("DoorModel");
                break;
            
            case "Window":
                ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("WindowModel");
                break;
            
            case "Stairs":
                ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("StairsModel");
                break;
            
            case "Fence":
                ConstructionManager.Instance.itemToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("FenceModel");
                break;
            
            case "Gate":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("GateModel");
                break;
            case "StorageBox":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("StorageBoxModel");
                //ConstructionManager.Instance.ActivateConstructionPlacement("StorageBoxModel");
                break;
            
            case "Campfire":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("CampfireModel"); 
                break;
            
            case "Furnace":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("FurnaceModel");
                break;
            ;
            case "Anvil":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("AnvilModel");
                break;
            
            case "Bed":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("BedModel");
                break;
            
            case "WallTorch":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("WallTorchModel");
                break;
            case "Well":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("WellModel");
                break;
            
            case "Candle":
                PlacementSystem.Instance.inventoryItemToDestroy = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("CandleModel");
                break;
            
            default:
                break;
        }
       

    }

    // Triggered when the mouse button is released over the item that has this script.
    public void OnPointerUp(PointerEventData eventData)
    {
        
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            
            if ((isConsumable && itemPendingConsumption == gameObject) || isArrow)
            {
                //Debug.Log("Destroy item 4");
                DestroyImmediate(gameObject);
                InventorySystem.Instance.ReCalculateList();
                CraftingSystem.Instance.RefreshNeededItems();
            }

        }
    }

    private void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        PlayerState.Instance.consumingFunction(healthEffect, caloriesEffect, hydrationEffect, thisName);
        // Hide the item info UI immediately.
        itemInfoUI.SetActive(false);
        
    }

    
}