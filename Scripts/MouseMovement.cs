using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{

    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float YRotation = 0f;

    void Start()
    {
        //Locking the cursor to the middle of the screen and making it invisible
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SettingsManager.Instance.mouseSensivityChanged.AddListener(() =>
        {
            mouseSensitivity = SettingsManager.Instance.mouseValueFloat;
        });
    }

    void Update()
    {

        //if the inventory is open we dont want to move the camera
        if (!InventorySystem.Instance.isOpen &&
            !CraftingSystem.Instance.isOpen && 
            !MenuManager.Instance.isOpen &&
            !DialogSystem.Instance.dialogUIActive &&
            !QuestManager.Instance.isQuestMenuOpen &&
            !StorageManager.Instance.storageUIOpen &&
            !CampfireUIManager.Instance.isUIOpen &&
            !EquipSystem.Instance.isBackpackOpen &&
            !FurnaceUIManager.Instance.isUIOpen &&
            !SeedShopSystem.Instance.isOpen)
        {
           

            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            //control rotation around x axis (Look up and down)
            xRotation -= mouseY;

            //we clamp the rotation so we cant Over-rotate (like in real life)
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //control rotation around y axis (Look up and down)
            YRotation += mouseX;

            //applying both rotations
            transform.localRotation = Quaternion.Euler(xRotation, YRotation, 0f);
        }
        
        

    }
}
