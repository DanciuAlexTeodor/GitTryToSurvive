using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public Button LoadGameBTN;
    public GameObject keyboardMapText;

    private void OnEnable()
    {
        NPC.ActivateKeyboardMap += ActivateKeyboardMap;
        
    }

    private void ActivateKeyboardMap()
    {
        Debug.Log("Keyboard Map Activated");
        keyboardMapText.SetActive(true);
    }

    private void OnDisable()
    {
        NPC.ActivateKeyboardMap -= ActivateKeyboardMap;
    }

    public void NewGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void ExitGame()
    {
        Debug.Log("Exiting Game");
        Application.Quit();
    }
}
