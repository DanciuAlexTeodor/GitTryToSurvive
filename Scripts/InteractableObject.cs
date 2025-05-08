using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public static event Action<string> ItemCollected;

    public string ItemName;
    public bool playerInRange;
    public bool isHoveringDoor;
    private bool isDoorOpen=false;

    [SerializeField] float detectionRange = 4f;

    public string GetItemName()
    {
        return ItemName;
        //return ItemName;
    }


    void Update()
    {

        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if(distance <= detectionRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }



        //if i click on the object
        if (playerInRange && Input.GetKeyDown(KeyCode.Mouse0) && SelectionManager.Instance.onTarget && SelectionManager.Instance.selectedObject == gameObject && 
            !gameObject.CompareTag("interactableBuilding") && !gameObject.CompareTag("placedFoundation"))
        {
            if (InventorySystem.Instance.CkeckSlotsAvailable(1))
            {

                InventorySystem.Instance.AddToInventory(ItemName);
                
                ItemCollected?.Invoke(ItemName);
                
                InventorySystem.Instance.itemsPickedUp.Add(gameObject.name);
                //destroy the object
                //Debug.Log("Destroy item 5");
                Destroy(gameObject);
            }
            else
            {
                EquipSystem.Instance.PopUpMessage("Inventory is full!");
            }
        }
        else if(playerInRange && Input.GetKeyDown(KeyCode.Mouse0) &&  
                       gameObject.CompareTag("interactableBuilding") && checkIfIsHoveringOverTheDoor())
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.woodDoor);
            //trigger an animation
            if(isDoorOpen)
            {
                gameObject.GetComponent<Animator>().SetTrigger("close");
                isDoorOpen = false;
            }
            else
            {
                gameObject.GetComponent<Animator>().SetTrigger("open");
                isDoorOpen = true;
            }
        }
        
    }

    private bool checkIfIsHoveringOverTheDoor()
    {
       
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isHoveringDoor = true;
                return true;
            }
        }
        isHoveringDoor = false;
        return false;
    }

    
}