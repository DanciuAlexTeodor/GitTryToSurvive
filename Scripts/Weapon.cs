using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public int weaponDamage;
    public bool isLootWeapon;
    public bool destroyWeaponWhenBroken;
    public bool placeableOnAnvil;
    public float weaponHealth, maximumWeaponHealth;
    public Slider slider;

    private void Awake()
    {
        if(slider!=null)
        {
            slider = GetComponentInChildren<Slider>();
            slider.interactable = false;
            slider.value = weaponHealth / maximumWeaponHealth;
        }
        
    }

    public void IncreaseHealth()
    {
        weaponHealth = maximumWeaponHealth;
        slider.value = weaponHealth / maximumWeaponHealth;

    }

    public void TakeDamage(int damage)
    {
        weaponHealth -= damage;
        if(slider!=null)
        {
            slider.value = weaponHealth / maximumWeaponHealth;
        }
        

        if(destroyWeaponWhenBroken)
        {
            if (slider.value > 0.5f)
            {
                slider.fillRect.GetComponent<Image>().color = Color.green;
            }
            else if (slider.value <= 0.5f && slider.value >= 0.25f)
            {
                slider.fillRect.GetComponent<Image>().color = Color.yellow;
            }
            else if (slider.value < 0.25f)
            {
                slider.fillRect.GetComponent<Image>().color = Color.red;
            }

            if (weaponHealth <= 0)
            {
                EquipSystem.Instance.DestroyWeaponInHand();
              
                if(!GetComponent<InventoryItem>().isEatble)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Instance.weaponDestroy);
                }
               
                Destroy(gameObject);
            }
        }
    }
}
