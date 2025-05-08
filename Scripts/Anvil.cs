using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anvil : MonoBehaviour
{
    public bool playerInRange;
    public float distance;
    public GameObject tool2D; //the image of the tool
    public GameObject tool3D; // the 3d model of the tool
    public float tool2DWeaponHealth;
    public float tool2DWeaponMaxHealth;
    public bool isToolOnAnvil=false;
    public int nrOfHits = 0;

    private void Update()
    {
        float distanceFromPlayerToAnvil = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);
        if(distanceFromPlayerToAnvil <= distance)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if(transform.childCount==0 && isToolOnAnvil)
        {
            isToolOnAnvil = false;
    
        }
    }

    public void Repair()
    {
        nrOfHits++;
        if(nrOfHits==3)
        {
            tool2DWeaponHealth = tool2DWeaponMaxHealth;
        }
    }

    public void PlaceItemForRepairing(string itemToBePlaced)
    {
        if(!itemToBePlaced.Contains("Iron"))
        {
            return;
        }

        itemToBePlaced = itemToBePlaced.Replace("(Clone)", "");
        Debug.Log(itemToBePlaced); //Iron_Axe
        tool2D = EquipSystem.Instance.selectedItem;
        Weapon tool2DWeaponComponent = tool2D.GetComponent<Weapon>();
        tool2DWeaponHealth = tool2DWeaponComponent.weaponHealth;
        tool2DWeaponMaxHealth = tool2DWeaponComponent.maximumWeaponHealth;


        //Debug.Log(itemToBePlaced);

        // Load the prefab from Resources
        GameObject toolPrefab = Resources.Load<GameObject>("Anvil/" + itemToBePlaced);

        if (toolPrefab != null)
        {
            // Instantiate the prefab at the same position as the anvil (or some temporary location)
            tool3D = Instantiate(toolPrefab, transform.position, Quaternion.identity);
            isToolOnAnvil = true;

            // Get the prefab's original local position, rotation, and scale
            Vector3 originalLocalPosition = toolPrefab.transform.localPosition;
            Quaternion originalLocalRotation = toolPrefab.transform.localRotation;
            Vector3 originalLocalScale = toolPrefab.transform.localScale;

            // Set the instantiated object's parent to the anvil
            tool3D.transform.SetParent(transform, true); // Set parent, but we will reset the coordinates below

            // Apply the original local transform (coordinates) of the prefab
            tool3D.transform.localPosition = originalLocalPosition;   // Restore original local position
            tool3D.transform.localRotation = originalLocalRotation;   // Restore original local rotation
            tool3D.transform.localScale = originalLocalScale;         // Restore original local scale

            Debug.Log("Successfully instantiated tool3D: " + tool3D.name + " with original prefab coordinates.");
        }
        else
        {
            Debug.LogError("Model not found in Resources/Anvil/"+ itemToBePlaced);
        }


        DestroyImmediate(EquipSystem.Instance.selectedItem);
        EquipSystem.Instance.DestroyWeaponInHand();

    }
}
