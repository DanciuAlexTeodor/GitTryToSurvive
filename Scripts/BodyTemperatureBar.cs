using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class BodyTemperatureBar : MonoBehaviour
{
    public static BodyTemperatureBar Instance { get; set; }


    public GameObject playerState;

    public float currentBodyTemperature= 36.5f;
    public float maxBodyTemperature;
    public float initialBodyTemperature = 36.5f;
    public float outsideTemperatureValue;

    private Image fillImage;
    private Image tempIcon;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
        tempIcon = transform.Find("Image").GetComponent<Image>();

        currentBodyTemperature = playerState.GetComponent<PlayerState>().currentBodyTemperature;
        maxBodyTemperature = playerState.GetComponent<PlayerState>().maxBodyTemperature;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    //i decided to implement this method here because i have to add collors for the image fill
    // and there is a huge amount of code to implement regarding the temperature system
    public void IncreaseBodyTemperature()
    {
        if(currentBodyTemperature< maxBodyTemperature)
        {
            currentBodyTemperature += 0.01f;
            initialBodyTemperature = currentBodyTemperature;
            fillImage.fillAmount = (currentBodyTemperature - 28) / (38 - 28);
            ChangeColorOfSlider(currentBodyTemperature);
        }
    }

    public void ChangeBodyTemperatureBasedOnOutsideTemperature(int outsideTemperature)
    {
        outsideTemperatureValue = outsideTemperature;

        // Determine how fast the body temperature adjusts to outside conditions based on outside temperature
        float timeToAdjustBodyTemperatureToOutsideConditions;

        //pentru ca temperatura sa scada mai repede, valoarea timeToAdjust trebuie sa fie mai mica
        //aceste temperaturi sunt perfect calculate pentru iarna la timp de 100 sec/zi   
        //functioneaza si pentru 1200 sec/zi
        if (outsideTemperature >= 10 && outsideTemperature < 15)
        {
            timeToAdjustBodyTemperatureToOutsideConditions = 1.2f;
        }
        else if (outsideTemperature >= 0 && outsideTemperature < 10)
        {
            timeToAdjustBodyTemperatureToOutsideConditions = 0.9f;
        }
        else if (outsideTemperature < 0)
        {
            timeToAdjustBodyTemperatureToOutsideConditions = 0.4f;
        }
        else
        {
            timeToAdjustBodyTemperatureToOutsideConditions = 1.5f; // Add a safe default for above 15°C
        }


        
        currentBodyTemperature = Mathf.Lerp(initialBodyTemperature, outsideTemperature+10, Time.deltaTime / timeToAdjustBodyTemperatureToOutsideConditions);
        fillImage.fillAmount = (currentBodyTemperature - 28) / (38 - 28);

        ChangeColorOfSlider(currentBodyTemperature);
        initialBodyTemperature = currentBodyTemperature;
    }

    private void ChangeColorOfSlider(float currentBodyTemperature)
    {
        

        //minimum body temperature is 28 and maximum is 38, i want purple, blue, green, yellow and red colors to represent the body temperature

        if (currentBodyTemperature < 29)
        {
            fillImage.color = Color.magenta;  // Purple color (represented by magenta in Unity)
            tempIcon.color = Color.magenta;
        }
        else if (currentBodyTemperature >= 29 && currentBodyTemperature < 32)
        {
            fillImage.color = Color.blue;  // Blue color
            tempIcon.color = Color.blue;
        }
        else if (currentBodyTemperature >= 32 && currentBodyTemperature < 37)
        {
            fillImage.color = Color.yellow;  // Yellow color
            tempIcon.color = Color.yellow;
        }
        else if (currentBodyTemperature >= 37 && currentBodyTemperature <= 38)
        {
            fillImage.color = Color.red;  // Red color
            tempIcon.color = Color.red;
        }

    }



}
