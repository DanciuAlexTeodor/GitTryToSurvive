using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public bool playerInRange;
    public bool isCooking;
    public float cookingTimer, cookingTimerAux;
    public CookableInput inputBeingCooked;
    public GameObject fuelObject;
    public string output;
    public GameObject fire;
    //DECLARE AUDIO CLIP

    private ResourceHealthBar resourceHealthBar;

    private void Start()
    {
        resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
  
    }

    public List<string> items;

    private void Awake()
    {
        items = new List<string>(); // Initialize items list
    }

    public void HoverOverFire()
    {
        GlobalState.Instance.resourceHealth = cookingTimer;
        GlobalState.Instance.resourceMaxHealth = cookingTimerAux;

        if (resourceHealthBar == null)
        {
            resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
        }

        if (resourceHealthBar != null)
        {
            resourceHealthBar.UpdateHealthBar();
        }


    }

    private void Update()
    {
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if (distance <= 6f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }

        if (isCooking)
        {
            if(inputBeingCooked!=null)
            {
                output = GetCookedInput(inputBeingCooked);
            }

            cookingTimer -= Time.deltaTime;
            fire.SetActive(true);

            if (cookingTimer <= 0)
            {
                isCooking = false;
            }
        }
        else
        {
           
            fire.SetActive(false);
        }
    }

    private string GetCookedInput(CookableInput input)
    {
        
         return input.cookedInputName;
        
    }

    public void OpenUI()
    {
        // Load stored items into UI when the campfire UI opens
        CampfireUIManager.Instance.LoadCampfireItems(this);

        CampfireUIManager.Instance.OpenUI();
        CampfireUIManager.Instance.selectedCampfire = this;

        if(output != "")
        {
            GameObject outputItem = Instantiate(Resources.Load<GameObject>(output),
                CampfireUIManager.Instance.outputSlot.transform.position,
                CampfireUIManager.Instance.outputSlot.transform.rotation);

            outputItem.transform.SetParent(CampfireUIManager.Instance.outputSlot.transform);

            output = "";
        }
    }

    public void StartCooking(InventoryItem input)
    {
        isCooking = true;
        if (input==null)
        {
            //Debug.Log("Is not cooking anyting");
            cookingTimer = 30f;
            cookingTimerAux = cookingTimer;
        }
        else
        {
            inputBeingCooked = ConvertIntoCookable(input);
            
            cookingTimer = TimeToCookInput(inputBeingCooked);
            cookingTimerAux = cookingTimer;
        }
        
    }

    private CookableInput ConvertIntoCookable(InventoryItem input)
    {
        foreach(CookableInput cookable in CampfireUIManager.Instance.cookingData.validInputs)
        {
            if(cookable.name == input.thisName)
            {
                return cookable;
            }
        }
        return new CookableInput();
    }

    private float TimeToCookInput(CookableInput input)
    {
        return input.timeToCook;
    }
}
