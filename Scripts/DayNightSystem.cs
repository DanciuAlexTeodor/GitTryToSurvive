using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DayNightSystem : MonoBehaviour
{
    public static DayNightSystem Instance { get; set; }
    public Light directionalLight;

    public float dayDurationInSeconds = 24f;
    public float currentTimeOfDay = 0.20f;
    public int currentHour, oldHour;

    public List<SkyboxTimeMapping> timeMappings;
    float blendedValue=0.0f;

    bool lockNextDayTrigger = false;

    public TextMeshProUGUI timeUI;
    public WeatherSystem wheaterSystem;

    // Day and Night Colors
    [SerializeField] private Color daySkyColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color nightSkyColor = new Color(0.05f, 0.05f, 0.15f);
    [SerializeField] private Color dayEquatorColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color nightEquatorColor = new Color(0.05f, 0.05f, 0.05f);
    [SerializeField] private Color dayGroundColor = new Color(0.8f, 0.8f, 0.8f);
    [SerializeField] private Color nightGroundColor = new Color(0.05f, 0.05f, 0.05f);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        // Calculate the current time of day based on the game time
        currentTimeOfDay += Time.deltaTime / dayDurationInSeconds;
        currentTimeOfDay %= 1; // keep the time between 0 and 1

        currentHour = (int)(currentTimeOfDay * 24);
        timeUI.text = $"{currentHour}:00";

        if(currentHour != oldHour)
        {
            TemperatureSystem.Instance.ChangeTemperatureBasedOnHourOfDay();
            UpdateSkybox();
            if(currentHour == 22 && EquipSystem.Instance.CheckIfLeftHolderIsEmpty())
            {
                EquipSystem.Instance.PopUpMessage("Activate your torch by pressing T");
            }
        }

        oldHour = currentHour;

        // Adjust light based on the time of day
        float sunOffset = -0.20f; // Adjust this to control when the sun rises and sets
        float adjustedTimeOfDay = Mathf.Clamp((currentTimeOfDay + sunOffset), 0, 1);
        float sunAngle = Mathf.Lerp(-90f, 270f, adjustedTimeOfDay); // -90 to 270 degrees for a full sun cycle
        directionalLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 170, 0));

        // Determine whether it is day or night
        if (currentHour >= 8 && currentHour <= 21)
        {
            // Daytime settings
            directionalLight.intensity = 1f;

            // Transition to day colors
            RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, daySkyColor, Time.deltaTime);
            RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, dayEquatorColor, Time.deltaTime);
            RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, dayGroundColor, Time.deltaTime);
        }
        else
        {
            // Nighttime settings
            directionalLight.intensity = 0.1f;

            // Transition to night colors
            RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, nightSkyColor, Time.deltaTime);
            RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, nightEquatorColor, Time.deltaTime);
            RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, nightGroundColor, Time.deltaTime);
        }


        // Update the skybox
        /*if (!wheaterSystem.isSpecialWeather)
        {
            UpdateSkybox();
        }*/

        

        // Day pass check
        if (currentHour == 0 && !lockNextDayTrigger)
        {
            TimeManager.Instance.TriggerNextDay();
            lockNextDayTrigger = true;
        }

        if (currentHour != 0)
        {
            lockNextDayTrigger = false;
        }
    }





    private void UpdateSkybox()
    {
        //find the appropriate skybox material for the current hour
        Material currentSkybox = null;
        foreach (SkyboxTimeMapping mapping in timeMappings)
        {

            if (currentHour == mapping.hour)
            {
                currentSkybox = mapping.skyboxMaterial;

                if (currentSkybox.shader != null)
                {
                    if(currentSkybox.shader.name == "Custom/SkyboxTransition")
                    {
                        
                        blendedValue += Time.deltaTime;
                        blendedValue = Mathf.Clamp01(blendedValue);
                        currentSkybox.SetFloat("_TransitionFactor", blendedValue);
                    }
                    else
                    {
                        blendedValue = 0.0f;
                    }
                }
            }
        }

        

        if(currentSkybox != null)
        {
            
            RenderSettings.skybox = currentSkybox;
        }
    }

}

[System.Serializable]
public class SkyboxTimeMapping
{
    public int hour; // 0 - 23
    public string phaseName;
    public Material skyboxMaterial; // Skybox material for this hour
}