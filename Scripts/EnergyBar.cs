using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Image fillImage;
    public GameObject playerState;

    private float currentEnergy, maxEnergy;

    void Awake()
    {
        fillImage = GetComponent<Image>();
    }


    void Update()
    {
        currentEnergy = playerState.GetComponent<PlayerState>().currentEnergy;
        maxEnergy = playerState.GetComponent<PlayerState>().maxEnergy;

        fillImage.fillAmount = currentEnergy / maxEnergy;

        if (currentEnergy <= 10f)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.lowEnergy);
        }
        else
        {
            if(currentEnergy>40f)
            {
                SoundManager.Instance.StopSound(SoundManager.Instance.lowEnergy);
            }
            
        }
    }
}
