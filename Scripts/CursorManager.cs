using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; set; }

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

    public void FreeCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SelectionManager.Instance.DisableSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
    }

    public void LockCursor()
    {
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
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            SelectionManager.Instance.EnableSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
        }
    }
}
