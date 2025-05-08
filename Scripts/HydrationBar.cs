using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HydrationBar : MonoBehaviour
{
    private Image fillImage;  // Reference to the Image component that controls the fill amount.
    public GameObject playerState;

    private float currentHydration, maxHydration;

    void Awake()
    {
        // Get the Image component that will control the fill amount
        fillImage = GetComponent<Image>();
    }

    void Update()
    {
        // Access the player’s current and max hydration from the PlayerState script
        currentHydration = playerState.GetComponent<PlayerState>().currentHydration;
        maxHydration = playerState.GetComponent<PlayerState>().maxHydration;

        // Set the Image fillAmount property based on the hydration percentage
        fillImage.fillAmount = currentHydration / maxHydration;
    }
}
