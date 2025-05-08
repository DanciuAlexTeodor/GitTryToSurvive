using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuManager : MonoBehaviour
{
      
     //make singleton
     public static MenuManager Instance { get; set; }

    public GameObject menuCanvas;
    public GameObject uiCanvas;

    public GameObject saveMenu;
    public GameObject settingsMenu;
    public GameObject menu;

    public bool isOpen=false;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (isOpen == false)
            {
                menuCanvas.SetActive(true);
                uiCanvas.SetActive(false);
                isOpen = true;


                CursorManager.Instance.FreeCursor();
            }
            else
            {
                CloseMenu();
                
            }
        }
    }

    public void CloseMenu()
    {
        menuCanvas.SetActive(false);
        uiCanvas.SetActive(true);
        isOpen = false;

        CursorManager.Instance.LockCursor();
    }
}
