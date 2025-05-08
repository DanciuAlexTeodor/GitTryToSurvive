using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading.Tasks;


public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; set; }

    public GameObject placementHoldingSpot; // Drag our construcionHoldingSpot or a new placementHoldingSpot
    public GameObject enviromentPlaceables;


    public bool inPlacementMode;
    [SerializeField] bool isValidPlacement;

    [SerializeField] GameObject itemToBePlaced;
    public GameObject inventoryItemToDestroy;

    [SerializeField] GameObject placementModelUI;

    public bool itemWasPlaced = false;

 
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

    public void ActivatePlacementMode(string itemToPlace)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>(itemToPlace));

        // Changing the name of the gameobject so it will not be (clone)
        item.name = itemToPlace; //Dirt_PileModel

        // Setting the item to be a child of our placement holding spot
        item.transform.SetParent(placementHoldingSpot.transform, false);

        // Saving a reference to the item we want to place
        itemToBePlaced = item;

        // Activating Construction mode
        inPlacementMode = true;

        placementModelUI.SetActive(true);
    }

    private void ModifyFromKeyboard()
    {
        float x, y, z;

        switch (itemToBePlaced.name)
        {
            
            case "FenceModel":
                x=0; y=0; z=1;
                break;
            case "FurnaceModel": //is fixed
                x = 0; y = 0; z = 1;
                break;
            case "GateModel":
                x = 0; y = 0; z = 1;
                break;
            default:
                x = 0; y = 1; z = 0;
                break;
        }
        
        
        if(Input.GetKey(KeyCode.R))
        {
            itemToBePlaced.transform.Rotate(x,y,z);
        }
        if(Input.GetKey(KeyCode.E))
        {
            itemToBePlaced.transform.Rotate(-x, -y, -z);
        }
    }

    private void Update()
    {


        if (itemToBePlaced != null && inPlacementMode)
        {
            ModifyFromKeyboard();
            if (IsCheckValidPlacement())
            {
                isValidPlacement = true;
                itemToBePlaced.GetComponent<PlaceableItem>().SetValidColor();
            }
            else
            {
                isValidPlacement = false;
                itemToBePlaced.GetComponent<PlaceableItem>().SetInvalidColor();
            }
        }

        // Left Mouse Click to Place item
        if (Input.GetMouseButtonDown(0) && inPlacementMode && isValidPlacement)
        {
            //if the item is a shovel , we need to place it in a specific way

            if(itemToBePlaced==null)
            {
                return;
            }

            if (itemToBePlaced.name == "Dirt_PileModel")
            {
               itemWasPlaced = true;
               Wait();
            }
            else
            {
                itemWasPlaced = true;
                PlaceItemFreeStyle();
                DestroyItem(inventoryItemToDestroy);
                placementModelUI.SetActive(false);
            }
            
        }

        // Cancel Placement                     //TODO - don't destroy the ui item until you actually placed it.
        if (Input.GetKeyDown(KeyCode.X) && inPlacementMode)
        {
            if(inventoryItemToDestroy!=null)
            {
                inventoryItemToDestroy.SetActive(true);
                inventoryItemToDestroy = null;
            }
            
            DestroyItem(itemToBePlaced);
            
            itemWasPlaced = false;
            
            itemToBePlaced = null;
            inPlacementMode = false;
            placementModelUI.SetActive(false);
        }
    }

    private async void Wait()
    {
        await WaitForDigging();
    }

    private async Task WaitForDigging()
    {
        await Task.Delay(2000);  // Wait for 2 seconds (2000 milliseconds)
        EquipSystem.Instance.decreaseWeaponHealth(5);
        PlaceItemFreeStyle();
        Debug.Log("Destroy item 13");
        DestroyItem(inventoryItemToDestroy);
        placementModelUI.SetActive(false);
    }

    private bool IsCheckValidPlacement()
    {
        if (itemToBePlaced != null)
        {
            return itemToBePlaced.GetComponent<PlaceableItem>().isValidToBeBuilt;
        }

        return false;
    }

    public void DeactivatePlacementMode()
    {
        if (itemToBePlaced != null)
        {
            Debug.Log("Destroy item 14");
            itemToBePlaced.SetActive(false);
            DestroyImmediate(itemToBePlaced); // Destroys the current item being placed (e.g., Dirt_PileModel)
            itemToBePlaced = null;
        }

        inPlacementMode = false;
    }



    private void PlaceItemFreeStyle()
    {
        // Store the name before itemToBePlaced becomes null
        string itemName = itemToBePlaced.name;

        // Setting the parent to be the root of our scene
        itemToBePlaced.transform.SetParent(enviromentPlaceables.transform, true);

        // Setting the default color/material
        itemToBePlaced.GetComponent<PlaceableItem>().SetDefaultColor();
        itemToBePlaced.GetComponent<PlaceableItem>().enabled = false;

        if (itemToBePlaced.name == "StorageBoxModel")
        {
            //Debug.Log("StorageBoxModel");
            StorageBox storageBox = itemToBePlaced.GetComponent<StorageBox>();
            StorageManager.Instance.storageBoxList.Add(new StorageBoxPieceData(storageBox, itemToBePlaced.name, itemToBePlaced.transform.localPosition, itemToBePlaced.transform.localRotation, itemToBePlaced.transform.parent?.name));
        }

       
        // Set itemToBePlaced to null after its name is saved
        itemToBePlaced = null;

        StartCoroutine(delay(itemName));
    }

    IEnumerator delay(string itemName)
    {
        if (itemName != "Dirt_PileModel")  // Use the stored name
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.construction);
        }

        yield return new WaitForSeconds(2.5f);
        itemToBePlaced = null;
        inPlacementMode = false;

    }


    private void DestroyItem(GameObject item)
    {
        //Debug.Log("Destroy item 15");
        DestroyImmediate(item);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();
    }
}