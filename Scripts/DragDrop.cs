using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public static GameObject itemBeingDragged;
    public static GameObject itemBeingClicked;
    Vector3 startPosition;
    Transform startParent;
    public bool wasTeleported;


    private void Awake()
    {

        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

    }

    public void OnPointerClick(PointerEventData eventData)
    {

        

        gameObject.name.Replace("(Clone)", "");
        itemBeingClicked = gameObject;
        itemBeingClicked.name = itemBeingClicked.name.Replace("(Clone)", "");
        //Debug.Log("Item clicked with right: " + itemBeingClicked.name);

        // Check if the item has a parent that represents the slot.
        Transform slotTransform = transform.parent;

        //get the parent of the slot ( the UI screen type)
        Transform screen = slotTransform.parent;

        // If the slot has a child (which is an item), teleport it to the other screen.
        if (slotTransform != null && slotTransform.childCount > 0)
        {
            // Assuming that the item game object has a name that describes it.
            GameObject itemInSlot = slotTransform.GetChild(0).gameObject;
            wasTeleported = false;

            //we check if the item we clicked is in inventory or backpack
            if (screen.name.Contains("Backpack") || screen.name.Contains("Inventory"))
            {
                //if the storage UI is open, we want to teleport the item to the storage screen
                if(StorageManager.Instance.storageUIOpen)
                {
                    //DestroyImmediate(slotTransform.GetChild(0).gameObject);
                    TeleportItemInOtherUIScreen(itemBeingClicked, StorageManager.Instance.GetRelevantUI(StorageManager.Instance.selectedStorage));
                }

                //if the backpack UI is open, we want to teleport the item to the inventory screen
                if(wasTeleported==false && EquipSystem.Instance.isBackpackOpen && screen.name.Contains("Inventory") && InventorySystem.Instance.CkeckSlotsAvailable(1))
                {
                    TeleportItemInOtherUIScreen(itemBeingClicked, InventorySystem.Instance.backpackScreenUI);
                }

                //if the inventory UI is open, we want to teleport the item to the backpack screen
                if(wasTeleported==false && InventorySystem.Instance.isOpen && screen.name.Contains("Backpack"))
                {
                    TeleportItemInOtherUIScreen(itemBeingClicked, InventorySystem.Instance.inventoryScreenUI);
                }

                if(wasTeleported==false && CampfireUIManager.Instance.isUIOpen)
                {
                    TeleportItemInOtherUIScreen(itemBeingClicked, CampfireUIManager.Instance.campfirePanel);
                }

                if(wasTeleported==false && FurnaceUIManager.Instance.isUIOpen)
                {
                    TeleportItemInOtherUIScreen(itemBeingClicked, FurnaceUIManager.Instance.furnacePanel);
                }
            }
            else 
            {
                if (screen.name.Contains("Storage") || screen.name.Contains("Cooking") || screen.name.Contains("Furnace"))
                {
                    //if the storage UI is open, we want to teleport the item to the inventory screen
                    if (InventorySystem.Instance.isOpen && InventorySystem.Instance.CkeckSlotsAvailable(1))
                    {

                        TeleportItemInOtherUIScreen(itemBeingClicked, InventorySystem.Instance.inventoryScreenUI);
                    }

                    if (wasTeleported == false && EquipSystem.Instance.isBackpackOpen)
                    {
                        TeleportItemInOtherUIScreen(itemBeingClicked, InventorySystem.Instance.backpackScreenUI);
                    }
                }

            }
                 
        }
        else
        {
            Debug.Log("Slot is empty");
        }
    }

    private void TeleportItemInOtherUIScreen(GameObject itemToBeTeleported, GameObject newUI)
    {
        if(newUI!=null)
        {
            foreach(Transform child in newUI.transform)
            {
                //if the new UI is a campfire, we want to teleport the inputItem to the input slot
                //and the fuelItem to the fuel slot


                if (child.childCount == 0 && child.CompareTag("Slot") && child.gameObject.activeSelf == true)
                {
                    if (newUI.name.Contains("Cooking") || newUI.name.Contains("Furnace"))
                    {
                        //Debug.Log("Item beeing teleported: " + itemToBeTeleported.name);

                        if (CampfireUIManager.Instance.IsInputItem(itemToBeTeleported.name))
                        {
                            if (child.name.Contains("InputSlot"))
                            {
                                Teleport(itemToBeTeleported, child);
                                //break;
                            }
                        }
                        else if (CampfireUIManager.Instance.IsFuelItem(itemToBeTeleported.name))
                        {
                            //Debug.Log("Fuel item");
                            if (child.name.Contains("FuelSlot"))
                            {
                                Teleport(itemToBeTeleported, child);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Teleport(itemToBeTeleported, child);
                        break;
                    }
                }

            }
        }    
    }

    public void Teleport(GameObject itemToBeTeleported, Transform child)
    {
        wasTeleported = true;
        SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemOnSlot);
        itemToBeTeleported.transform.SetParent(child);
        itemToBeTeleported.transform.position = child.position;

        InventorySystem.Instance.ReCalculateList();
        //CampfireUIManager.Instance.ReCalculateList();
        if(StorageManager.Instance.storageUIOpen)
        {
            StorageManager.Instance.UpdateStorageBoxData(StorageManager.Instance.selectedStorage);
        }
        CraftingSystem.Instance.RefreshNeededItems();
    }


    public void OnBeginDrag(PointerEventData eventData)
    {

        //Debug.Log("OnBeginDrag");
        //So the item will be transparent when we drag it.
        canvasGroup.alpha = .6f;
        //So the ray cast will ignore the item itself.
        canvasGroup.blocksRaycasts = false;
        //So the item will go back to its original position if we drop it in a wrong place.
        startPosition = transform.position;
        //So the item will be on top of everything.
        startParent = transform.parent;
        transform.SetParent(transform.root);
        //So we can access the item from other scripts.
        itemBeingDragged = gameObject;

        //Debug.Log("Item being dragged: " + itemBeingDragged.name);

    }

    public void OnDrag(PointerEventData eventData)
    {
        //So the item will move with our mouse (at same speed)  and so it will be consistant if the canvas has a different scale (other then 1);
        rectTransform.anchoredPosition += eventData.delta;

    }



    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging on :" + transform.parent.name);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        if (transform.parent == startParent || transform.parent == transform.root)
        {
            Debug.Log("Parent is the same");
            ReturnToOriginalPosition();
            return;
        }

       

        string objectName = itemBeingDragged.name; //for example : IronHelmet(Clone)


        //Checking if the item is armor
        List<string> objectNameList = new List<string>();
        objectNameList.Add("Helmet");
        objectNameList.Add("Chest");
        objectNameList.Add("Legs");
        objectNameList.Add("Shield");
        bool itemIsEquipment = false;



        for(int i=0; i<objectNameList.Count; i++)
        {
            if (objectName.Contains(objectNameList[i])) // if( IronHelmet(Clone).Contains("Helmet"))
            {
                itemIsEquipment = true;
                //if(HelmetZone.Contains("Helmet") || Slot.Contains("Slot"))
                if (transform.parent.name.Contains(objectNameList[i] + "Zone") || transform.parent.name.Contains("Slot"))
                {
                    

                    if(objectName.Contains("Shield") && transform.parent.name.Contains(objectNameList[i] + "Zone"))
                    {
                        if(EquipSystem.Instance.CheckIfLeftHolderIsEmpty())
                        {
                            if(objectName.Contains("Iron"))
                            {
                                EquipSystem.Instance.EquipItemInLeftHolder("Iron Shield");
                            }
                            else if(objectName.Contains("Wood"))
                            {
                                EquipSystem.Instance.EquipItemInLeftHolder("WoodShield");
                            }
                            
                        }
                        else
                        {
                            EquipSystem.Instance.PopUpMessage("You already have an item in your left hand!");
                            ReturnToOriginalPosition();
                            return;
                        }


                    }

                    SoundManager.Instance.PlaySound(SoundManager.Instance.armorEquip);
                    //Debug.Log("Parent is " + objectNameList[i] + " slot");
                    transform.SetParent(transform.parent);
                }
                else
                {
                    ReturnToOriginalPosition();
                }
            } 
        }

        if (itemIsEquipment==false && transform.parent.name.Contains("Zone"))
        {
            Debug.Log("Item is not armor");
            ReturnToOriginalPosition();
        }

        /*if(itemBeingDragged!=null && itemBeingDragged.GetComponent<InventoryItem>()!=null)
        {
            if(itemBeingDragged.GetComponent<InventoryItem>().isEquippable==false)
            {
                ReturnToOriginalPosition();
            }
        }*/

        itemBeingDragged = null;

       
        InventorySystem.Instance.ReCalculateList();

        //Debug.Log("OnEndDrag");
       
        CraftingSystem.Instance.RefreshNeededItems();
    }

    public void ReturnToOriginalPosition()
    {
        transform.position = startPosition;
        transform.SetParent(startParent);
    }
    


}