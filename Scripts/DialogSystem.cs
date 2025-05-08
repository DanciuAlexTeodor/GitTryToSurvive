using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogSystem : MonoBehaviour
{

    public static DialogSystem Instance { get; set; }

    //reference for text mesh pro
    public TMPro.TextMeshProUGUI dialogText;
    public Button option1BTN;
    public Button option2BTN;

    public Canvas dialogUI;
    public bool dialogUIActive;

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
    
    public void OpenDialogUI()
    {
        dialogUIActive = true;
        dialogUI.gameObject.SetActive(true);

        CursorManager.Instance.FreeCursor();
    }

    public void CloseDialogUI()
    {
        dialogUIActive = false;
        dialogUI.gameObject.SetActive(false);

        CursorManager.Instance.LockCursor();
    }
   
}
