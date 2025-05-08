using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject map;
    private bool isOpen = false;
    private bool playerGotTheMap = false;

    private void OnEnable()
    {
        NPC.PlayerGetsTheMap += PlayerGotMap;
    }

    private void OnDisable()
    {
        NPC.PlayerGetsTheMap -= PlayerGotMap;
    }

    private void PlayerGotMap()
    {
        playerGotTheMap = true;
        Debug.Log("Player can press H.");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H) && playerGotTheMap)
        {
            ToggleMap();
        }
    }

    public void ToggleMap()
    {
        isOpen = !isOpen;
        map.SetActive(isOpen);
    }
}
