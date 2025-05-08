using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherSystem : MonoBehaviour
{
    [Range(0f, 1f)]
    public float chanceToRainSpring = 0.5f;
    [Range(0f, 1f)]
    public float chanceToRainSummer = 0.3f;
    [Range(0f, 1f)]
    public float chanceToRainAutumn = 0.5f;
    [Range(0f, 1f)]
    public float chanceToRainWinter = 0.7f;

    public GameObject rainEffect;
    public Material rainSkyBox;

    public bool isSpecialWeather;
    public AudioSource rainChannel;
    public AudioClip rainSound;
    

    public enum WeatherCondition
    {
        Sunny,
        Rainy
    }

    public WeatherCondition currentWeather = WeatherCondition.Sunny;

    private void Start()
    {
        //we register the method GenerateRandomWeather to the event OnDayPass
        TimeManager.Instance.OnDayPass.AddListener(GenerateRandomWeather);
    }

    public void GenerateRandomWeather()
    {
        float chanceToRain = 0f;

        switch (TimeManager.Instance.currentSeason)
        {
            case TimeManager.Season.Spring:
                chanceToRain = chanceToRainSpring;
                break;
            case TimeManager.Season.Summer:
                chanceToRain = chanceToRainSummer;
                break;
            case TimeManager.Season.Autumn:
                chanceToRain = chanceToRainAutumn;
                break;
            case TimeManager.Season.Winter:
                chanceToRain = chanceToRainWinter;
                break;
        }

        float randomValue = Random.Range(0f, 1f);

        if (randomValue <= chanceToRain)
        {
            currentWeather = WeatherCondition.Rainy;
            isSpecialWeather = true;
         
           
            Invoke("StartRain",1f);
            
        }
        else
        {
            currentWeather = WeatherCondition.Sunny;
            isSpecialWeather = false;

            StopRain();
        }
    }

    private void StartRain()
    {
        if(rainChannel.isPlaying == false)
        {
            rainChannel.clip = rainSound;
            rainChannel.loop = true;
            rainChannel.Play();
            //activate the fog
            RenderSettings.fog = true;
            RenderSettings.fogColor = Color.gray;
            RenderSettings.fogDensity = 0.1f;

        }

        RenderSettings.skybox = rainSkyBox;
        rainEffect.SetActive(true);
        MakeAllSoilsWet();
    }

    private void MakeAllSoilsWet()
    {
        Soil[] soils = FindObjectsOfType<Soil>();

        foreach(Soil soil in soils)
        {
            soil.MakeSoilWatered();
        }
    }

    private void StopRain()
    {
        if(rainChannel.isPlaying == true)
        {
            rainChannel.Stop();
        }


        rainEffect.SetActive(false);
        RenderSettings.fog = false;
    }

}
