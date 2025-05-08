using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgeOfItem : MonoBehaviour
{
    //This script is for items that can decay (food)
    public float age;
    public float maxAge;
    public Image normalImage;
    public Sprite decayedSprite;
    public string decayDescription;
    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    //the age of the item will be afected by the age and maxAge

    private void Start()
    {
        age = 0;
        TimeManager.Instance.OnDayPass.AddListener(UpdateAge);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnDayPass.RemoveListener(UpdateAge);
        }
    }

    private void UpdateAge()
    {
        age++;
        if (age >= maxAge)
        {
            HandleDecay();
        }
    }

    private void HandleDecay()
    {
        //how to choange the photo of the item
        if(decayedSprite != null && normalImage != null)
        {
            normalImage.sprite = decayedSprite;
            InventoryItem item = GetComponent<InventoryItem>();
            item.thisDescription = decayDescription;
            item.healthEffect = healthEffect;
            item.caloriesEffect = caloriesEffect;
            item.hydrationEffect = hydrationEffect;
        }
       
        
    }



}
