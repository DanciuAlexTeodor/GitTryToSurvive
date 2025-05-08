using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private Image fillImage;
    public GameObject playerState;

    private float currentHealth, maxHealth;
    
    void Awake()
    {
        fillImage = GetComponent<Image>();
    }

    
    void Update()
    {
        currentHealth = playerState.GetComponent<PlayerState>().currentHealth;
        maxHealth = playerState.GetComponent<PlayerState>().maxHealth;

        fillImage.fillAmount = currentHealth / maxHealth;
    }
}
