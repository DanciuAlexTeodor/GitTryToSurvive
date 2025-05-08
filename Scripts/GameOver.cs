using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public static GameOver Instance { get; set; }

    public GameObject gameOverScreen;

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

    private void RespawnPlayer()
    {
        InventorySystem.Instance.ResetInventory();
        EquipSystem.Instance.DestroyAllItemsInsideQuickslots();
    }

    public void PlayerDies()
    {

        ShowGameOverScreen();
        RespawnPlayer();
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        StartCoroutine(CloseGameOverScreen());
    }


    private IEnumerator CloseGameOverScreen()
    {
        yield return new WaitForSeconds(3f);
        gameOverScreen.SetActive(false);
    }



}
