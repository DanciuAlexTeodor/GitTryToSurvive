using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance { get; set; }

    [SerializeField] GameObject storageBoxSmallUI;
    public StorageBox selectedStorage;
    public bool storageUIOpen;

    public List<StorageBoxPieceData> storageBoxList;

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

    public void OpenBox(StorageBox storage)
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.openBox);
        SetSelectedStorage(storage);

        PopulateStorage(GetRelevantUI(selectedStorage));

        GetRelevantUI(selectedStorage).SetActive(true);
        storageUIOpen = true;

        CursorManager.Instance.FreeCursor();
    }

    private void PopulateStorage(GameObject storageUI)
    {
        // Get all slots of the ui
        List<GameObject> uiSlots = new List<GameObject>();

        foreach (Transform child in storageUI.transform)
        {
            uiSlots.Add(child.gameObject);
        }

        // Now, instantiate the prefab and set it as a child of each GameObject
        foreach (string name in selectedStorage.items)
        {
            foreach (GameObject slot in uiSlots)
            {
                if (slot.transform.childCount < 1)
                {
                    var itemToAdd = Instantiate(Resources.Load<GameObject>(name), slot.transform.position, slot.transform.rotation);
                    itemToAdd.transform.SetParent(slot.transform);
                    break;
                }
            }
        }
    }

    public void CloseBox()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.closeBox);
        RecalculateStorage(GetRelevantUI(selectedStorage));

        GetRelevantUI(selectedStorage).SetActive(false);
        storageUIOpen = false;

        CursorManager.Instance.LockCursor();
    }

    private void RecalculateStorage(GameObject storageUI)
    {
        // Get all slots of the ui
        List<GameObject> uiSlots = new List<GameObject>();

        foreach (Transform child in storageUI.transform)
        {
            uiSlots.Add(child.gameObject);
        }

        //clear the list of items
        selectedStorage.items.Clear();

        List<GameObject> toBeDeleted = new List<GameObject>();
        Debug.Log("Recalculating storage box");
        //Take the inventory items and convert them intro strings
        foreach(GameObject slot in uiSlots)
        {
            if (slot.transform.childCount > 0)
            {
                //Remove "Clone" from the name
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string result = name.Replace(str2, "");

                Debug.Log("Item added");
                selectedStorage.items.Add(result);
                toBeDeleted.Add(slot.transform.GetChild(0).gameObject);
            }
        }

        UpdateStorageBoxData(selectedStorage);

        foreach (GameObject item in toBeDeleted)
        {
            Debug.Log("Destroy item 16");
            Destroy(item);
        }
    }

    public void UpdateStorageBoxData(StorageBox storage)
    {
        Debug.Log("Updating storage box data");
        for (int i = 0; i < storageBoxList.Count; i++)
        {
            if (storageBoxList[i].storageBox == storage)
            {
                Debug.Log("Storage box found and updated");
                storageBoxList[i].storedItems = new List<string>(storage.items);
                break;
            }
        }
    }

    public void SetSelectedStorage(StorageBox storage)
    {
        selectedStorage = storage;
    }

    public GameObject GetRelevantUI(StorageBox storage)
    {
        // Create a switch for other types
        if(storage.thisBoxType == StorageBox.BoxType.smallBox)
             return storageBoxSmallUI;
        else if(storage.thisBoxType == StorageBox.BoxType.bigBox)
            return null;
        else
        {
            return null;
        }

    }
}