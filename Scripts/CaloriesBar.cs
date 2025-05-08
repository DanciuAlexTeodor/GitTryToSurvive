using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaloriesBar : MonoBehaviour
{
    private Image fillImage;
    public GameObject playerState;

    private float currentCalories, maxCalories;

    void Awake()
    {
        fillImage = GetComponent<Image>();

    }


    void Update()
    {
        currentCalories = playerState.GetComponent<PlayerState>().currentCalories;
        maxCalories = playerState.GetComponent<PlayerState>().maxCalories;

        fillImage.fillAmount = currentCalories / maxCalories;
    }
}
