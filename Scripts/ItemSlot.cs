using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    // Public field to store the slot number
    public int slotNumber;


    public GameObject Item
    {
        get
        {
            // Return the item in the slot if one exists (if it has any child).
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("Action called");
        if (Item==null && (gameObject.name.Contains("Slot") || gameObject.name.Contains("Zone")))
        {
            //Debug.Log("Action2 called");
            SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemOnSlot);
            PlaceItemInSlot();

        }
        else
        {
            if (DragDrop.itemBeingDragged == null)
            {
                return;
            }

            Debug.Log("Slot is occupied. Returning item to its original position.");
            DragDrop.itemBeingDragged.GetComponent<DragDrop>().ReturnToOriginalPosition(); // Return to original position
            return; // Do nothing further, don't place the item

        }


    }

    private void PlaceItemInSlot()
    {
        // Set the item being dragged to be a child of this slot
        if(DragDrop.itemBeingDragged != null)
        {
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = Vector2.zero; // Center the item in the slot
        }
        
    }
}

