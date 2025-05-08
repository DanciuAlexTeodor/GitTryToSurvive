using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using static SaveManager;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; set; }

    public Button saveBTN;

    public Slider masterSlider;
    public Slider mouseSlider;
    public GameObject masterValue;
    public GameObject mouseValue;
    public float mouseValueFloat;

    public UnityEvent mouseSensivityChanged = new UnityEvent();


    private void Start()
    {
        if(SaveManager.Instance == null)
        {
            Debug.Log("SaveManager.Instance is null!");
            return;
        }
        else
        {
            Debug.Log("SaveManager.Instance is not null!");

            StartCoroutine(LoadAndApplySettings());
        }
        
    }

    private void LoadAndSetMouseSensivity()
    {
        if(SaveManager.Instance == null)
        {
            Debug.Log("SaveManager.Instance is null!");
            return;
        }
        mouseSlider.value = SaveManager.Instance.LoadMouseSensivity();
        SetMouseSensivity(mouseSlider.value);
    }

    private void LoadAndSetVolume()
    {
        if(SaveManager.Instance != null)
        {
            VolumeSettings volumeSettings = SaveManager.Instance.LoadVolumeSettings();
            if (volumeSettings == null)
            {
                Debug.Log("VolumeSettings is null!");
                return;
            }
            masterSlider.value = volumeSettings.master;
            SetGameVolume(masterSlider.value);
        }

        
    }

    private IEnumerator LoadAndApplySettings()
    {
       
        LoadAndSetVolume();
        LoadAndSetMouseSensivity();
        //Load Graphics settings
        //Load Keybinds

        yield return new WaitForSeconds(0.1f);
    }

    //this function is called when the user clicks the save button
    public void SaveSettings()
    {
        SetGameVolume(masterSlider.value);
        SetMouseSensivity(mouseSlider.value);
        if (SaveManager.Instance == null)
        {
            Debug.Log("SaveManager.Instance is null!");
            return;
        }
        SaveManager.Instance.SaveVolumeSettings(masterSlider.value);
        SaveManager.Instance.SaveMouseSensivity(mouseSlider.value);
    }

    private void SetGameVolume(float volume)
    {
        AudioListener.volume = volume/50;
    }

    private void SetMouseSensivity(float sensivity)
    {
        mouseValueFloat = sensivity*2;
        mouseSensivityChanged.Invoke();
    }


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
        masterValue.GetComponent<TextMeshProUGUI>().text = masterSlider.value.ToString();
        mouseValue.GetComponent<TextMeshProUGUI>().text = mouseSlider.value.ToString();
    }

}
