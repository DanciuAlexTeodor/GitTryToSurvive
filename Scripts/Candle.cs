using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candle : MonoBehaviour
{
    public bool isLit;
    public float distanceForPlayerInRange;
    public bool playerInRange;
    private GameObject light;

    // Start is called before the first frame update
    void Start()
    {
        light = transform.GetChild(0).gameObject;
        light.SetActive(false);
    }

    void Update()
    {
        float distanceFromCandleToPlayer = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);
        if(distanceFromCandleToPlayer <= distanceForPlayerInRange)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }


    public void LightCandle()
    {
        isLit = true;
        light.SetActive(true);
    }

    public void TurnOffCandle()
    {
        isLit = false;
        light.SetActive(false);
    }

}
