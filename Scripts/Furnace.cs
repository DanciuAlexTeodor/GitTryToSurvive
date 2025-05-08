using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furnace : MonoBehaviour
{
    public bool playerInRange;
    public bool isCooking;
    public float cookingTimer, cookingTimerAux;
    public CookableInput inputBeingCooked1, inputBeingCooked2, inputBeingCooked3;
    public GameObject fuelObject;
    public string output1, output2, output3;
    public GameObject fire;

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
            Debug.Log("Health bar is updating");
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

            cookingTimer -= Time.deltaTime;
            fire.SetActive(true);

            if (cookingTimer <= 0)
            {
                isCooking = false;
                
                if(inputBeingCooked1!=null)
                {
                    output1 = GetCookedInput(inputBeingCooked1);
                }
                if(inputBeingCooked2!=null)
                {
                    output2 = GetCookedInput(inputBeingCooked2);
                }

                if(inputBeingCooked3!=null)
                {
                    output3 = GetCookedInput(inputBeingCooked3);
                }
                inputBeingCooked1 = null;
                inputBeingCooked2 = null;
                inputBeingCooked3 = null;
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
        // Load stored items into UI when the Furnace UI opens
        FurnaceUIManager.Instance.LoadFurnaceItems(this);

        FurnaceUIManager.Instance.OpenUI();
        FurnaceUIManager.Instance.selectedFurnace = this;

        if (output1 != "")
        {
            GameObject outputItem1 = Instantiate(Resources.Load<GameObject>(output1),
                FurnaceUIManager.Instance.outputSlot1.transform.position,
                FurnaceUIManager.Instance.outputSlot1.transform.rotation);

            outputItem1.transform.SetParent(FurnaceUIManager.Instance.outputSlot1.transform);

            output1 = "";
        }

        if (output2 != "")
        {
            GameObject outputItem2 = Instantiate(Resources.Load<GameObject>(output2),
                FurnaceUIManager.Instance.outputSlot2.transform.position,
                FurnaceUIManager.Instance.outputSlot2.transform.rotation);

            outputItem2.transform.SetParent(FurnaceUIManager.Instance.outputSlot2.transform);

            output2 = "";
        }

        if (output3 != "")
        {
            GameObject outputItem3 = Instantiate(Resources.Load<GameObject>(output3),
                FurnaceUIManager.Instance.outputSlot3.transform.position,
                FurnaceUIManager.Instance.outputSlot3.transform.rotation);

            outputItem3.transform.SetParent(FurnaceUIManager.Instance.outputSlot3.transform);

            output3 = "";
        }
    }

    public void StartCooking(InventoryItem input1, InventoryItem input2, InventoryItem input3)
    {
        // Convert the inputs into cookable objects to find the exact cooking time
        float cookTime1=0, cookTime2=0, cookTime3=0;
        bool cookingToWarmUp = true;
        if(input1!=null)
        {
            inputBeingCooked1 = ConvertIntoCookable(input1);
            cookTime1 = TimeToCookInput(inputBeingCooked1);
            cookingToWarmUp = false;
        }

        if(input2!=null)
        {
            inputBeingCooked2 = ConvertIntoCookable(input2);
            cookTime2 = TimeToCookInput(inputBeingCooked2);
            cookingToWarmUp = false; 
        }

        if(input3!=null)
        {
            inputBeingCooked3 = ConvertIntoCookable(input3);
            cookTime3 = TimeToCookInput(inputBeingCooked3);
            cookingToWarmUp = false;
        }
       

        isCooking = true;

        // Set the cooking timers for each input

        // Find the maximum cook time across all three inputs
        float maximumCookingTimer;
        if(cookingToWarmUp)
        {
            maximumCookingTimer = 30f;
        }
        else
        {
            maximumCookingTimer = Mathf.Max(cookTime1, cookTime2, cookTime3);
        }
            

        // Use the maximum cooking timer for tracking progress
        cookingTimer = maximumCookingTimer;
        cookingTimerAux = maximumCookingTimer;


    }


    private CookableInput ConvertIntoCookable(InventoryItem input)
    {
        foreach (CookableInput cookable in FurnaceUIManager.Instance.cookingData.validInputs)
        {
            if (cookable.name == input.thisName)
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
