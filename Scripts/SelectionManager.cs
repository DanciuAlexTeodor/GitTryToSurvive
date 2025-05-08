using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*when the interaction text is not visible, i have to check this condition and implement the necessary code
 * 
 * if (!npc && !interectable && !campfire && !storageBox && !soil && !miningRock
                && !choppableTree && !animal && !furnace && !water && !anvil)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);
            }
*/

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance { get; set; }

    public static event Action OnHammerHit;

    public GameObject selectedObject;
    public bool onTarget;
    public GameObject interaction_Info_UI;
    public Text interaction_text;

    public Image centerDotImage;
    public Image handIcon;

    public bool handIconIsVisible; // Mike a pus handIsVisible
    public bool IsNearWater = false;
    public GameObject selectedTree;
    public GameObject selectedRock;
    public GameObject selectedAnimal;
    public GameObject selectedStorageBox;
    public GameObject selectedCampfire;
    public GameObject selectedFurnace;
    public GameObject selectedSoil;
    public GameObject selectedWater;
    private float weaponRangeOfAttack;

    public string plantName;

    public bool isHittingWithHammer = false;
    public bool canHitWithHammer = false;
    public float healthToBeRestored;


    public GameObject chopHolder;
    public GameObject fireHolder;

    public TextMeshProUGUI chopHolderText;
    public bool isAttacking = false;
    public bool isLooting = false;
    


    private void Start()
    {
        onTarget = false;
        interaction_text = interaction_Info_UI.GetComponent<Text>();
        interaction_Info_UI.SetActive(false); // Ensure the UI is initially hidden

        chopHolderText = chopHolder.transform.Find("Text").GetComponent<TextMeshProUGUI>();
    }

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

    public void EnableSelection()
    {
        // Reset selection states
        onTarget = true;

        // Show the interaction UI if an object is selected
        if (selectedObject != null)
        {
            interaction_Info_UI.SetActive(true);

            if (selectedObject.CompareTag("pickable"))
            {
                centerDotImage.gameObject.SetActive(false);
                handIcon.gameObject.SetActive(true);
                handIconIsVisible = true;
            }
            else
            {
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                handIconIsVisible = false;
            }
        }
    }

    public void DisableSelection()
    {
        // Hide UI and reset states
        onTarget = false;
        interaction_Info_UI.SetActive(false);
        interaction_text.text = "";
        centerDotImage.gameObject.SetActive(true);
        handIcon.gameObject.SetActive(false);
        handIconIsVisible = false;
        selectedObject = null;
    }


    private void Print(string message)
    {
        Debug.Log(message);
    }


    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        


        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;


            //Attention: I have moved this ection of code to the EquipableItem script
            //because the enemy takes damage based on the collision with the weapon
            /*
            #region ------ Enemy Interaction -------
            Enemy enemy = selectionTransform.GetComponent<Enemy>();
            
            if (enemy)
            {
                if (EquipSystem.Instance.IsHoldingWeapon())
                {
                    weaponRangeOfAttack = EquipSystem.Instance.getWeaponRangeOfAttack();
                }

                if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingWeapon() &&
                    EquipableItem.Instance.canHit)
                {
                    float distanceFromPlayerToEnemy = enemy.GetDistanceFromPlayerToEnemy();
                    if (weaponRangeOfAttack+2>distanceFromPlayerToEnemy)
                    {
                        Debug.Log("Weapon range of attack: " + weaponRangeOfAttack + "/ distance from player to enemy: " + distanceFromPlayerToEnemy);
                        int damage = EquipSystem.Instance.GetWeaponDamage();
                        float timeToWaitForDamage = EquipSystem.Instance.TimeToWaitForDamage();
                       
                        enemy.TakeDamageBasedOnWeapon(damage, timeToWaitForDamage);
                        EquipSystem.Instance.decreaseWeaponHealth(5);
                    }
                }
            }
            #endregion
            */

            #region ------ Animal Interaction -------
            Animal animal = selectionTransform.GetComponent<Animal>();

         
            if (animal && animal.isInAttackZone)
            {
                if (animal.isDead)
                {
                    if (animal.looted == false)
                    {
                        chopHolderText.text = "Loot";
                        chopHolder.gameObject.SetActive(true);
                        animal.HoverOverAnimal();
                        selectedAnimal = animal.gameObject;
                        if (Input.GetMouseButtonDown(0) && EquipSystem.Instance.IsHoldingLootWeapon() && !isLooting)
                        {

                            int damage = EquipSystem.Instance.GetWeaponDamage();
                            
                            animal.TakeLootDamage(damage);
                            EquipSystem.Instance.decreaseWeaponHealth(5);
                            
                        }
                    }
                    else
                    {
                        //when the animal was already looted
                        chopHolderText.text = "";
                        chopHolder.gameObject.SetActive(false);
                        Debug.Log("Destroy item 2");
                        animal.gameObject.SetActive(false);
                        Lootable lootable = animal.GetComponent<Lootable>();
                        Loot(lootable);
                    }
                }
                else
                {

                    chopHolderText.text = "";
                    chopHolder.gameObject.SetActive(false);
                       
                }
            }
            else if (animal && animal.isInFeedZone && animal.isFarmAnimal && EquipSystem.Instance.isFoodInPlayersHand())
            {
                //pop up the option to feed the animal
                interaction_Info_UI.SetActive(true);
                interaction_text.text = "Feed " + animal.animalType;
                //right click to feed the animal
                if(Input.GetMouseButtonDown(1))
                {
                    animal.FeedAnimal();
                    EquipSystem.Instance.decreaseWeaponHealth(5);
                }

            }
            else
            {
                if(selectedAnimal != null)
                {
                    selectedAnimal = null;///
                    chopHolderText.text = "";
                    chopHolder.gameObject.SetActive(false);
                }
                
            }

           
            #endregion



            #region-------NPC Interaction ---------
            NPC npc = selectionTransform.GetComponent<NPC>();

            if (npc && npc.playerInRange)
            {
                interaction_text.text = "Talk to Maria";
                interaction_Info_UI.SetActive(true);

                if (Input.GetMouseButtonDown(0) && npc.isTalkingWithPlayer == false)
                {
                    npc.StartConversation();
                }

                if (DialogSystem.Instance.dialogUIActive)
                {
                    interaction_Info_UI.SetActive(false);
                    centerDotImage.gameObject.SetActive(false);
                }

            }

            #endregion


            #region -------- Shop Interaction --------
            SeedShop seedShop = selectionTransform.GetComponent<SeedShop>();
            if(seedShop && seedShop.playerInRange)
            {
                interaction_text.text = "Open Shop";
                interaction_Info_UI.SetActive(true);
                if(Input.GetMouseButtonDown(0))
                { 
                    SeedShopSystem.Instance.OpenShop();
                }
            }

            #endregion

            
            #region------ Tree Interaction--------

            ChoppableTree choppableTree = selectionTransform.GetComponent<ChoppableTree>();
            if (choppableTree && choppableTree.playerInRange)
            {
                choppableTree.HoverOverTree();
                choppableTree.canBeChopped = true;
                
                selectedTree = choppableTree.gameObject;
                chopHolderText.text = "Tree";
                chopHolder.gameObject.SetActive(true);

            }
            else
            {
                if (selectedTree != null)
                {
                    selectedTree.GetComponent<ChoppableTree>().canBeChopped = false;
                    selectedTree = null;


                    chopHolderText.text = "";
                    chopHolder.gameObject.SetActive(false);
                    
                }
                
                
            }

            #endregion
            


            #region ------ Rock Interaction -------
            
            MiningRock miningRock = selectionTransform.GetComponent<MiningRock>();

            if (miningRock && miningRock.playerInRange)
            {
                miningRock.canBeMined = true;
                selectedRock = miningRock.gameObject;
                chopHolderText.text = "IronRock";
                miningRock.HoverOverRock();

                chopHolder.gameObject.SetActive(true);
            }
            else
            {
                if (selectedRock != null)
                {
                    selectedRock.GetComponent<MiningRock>().canBeMined = false;
                    selectedRock = null;
                    chopHolder.gameObject.SetActive(false);
                    chopHolderText.text = "";
                }
               
            }
            #endregion

            #region ------ Interactable Object Interaction -------

            InteractableObject interectable = selectionTransform.GetComponent<InteractableObject>();

            if (interectable && interectable.playerInRange && !InventorySystem.Instance.isOpen && !CraftingSystem.Instance.isOpen &&
                !EquipSystem.Instance.isBackpackOpen && !EquipSystem.Instance.selectedItem)
            {
                onTarget = true;
                selectedObject = interectable.gameObject;
                interaction_text.text = interectable.GetItemName();
                interaction_Info_UI.SetActive(true);
                
                //if the parent of interectable has the Anvil script attached
                if (interectable.transform.parent != null && interectable.transform.parent.GetComponent<Anvil>())
                {
                    healthToBeRestored = interectable.transform.parent.GetComponent<Anvil>().tool2DWeaponHealth;
                }


                if (interectable.CompareTag("pickable"))
                {
                    centerDotImage.gameObject.SetActive(false);
                    handIcon.gameObject.SetActive(true);
                    handIconIsVisible = true;
                }
                else
                {
                    centerDotImage.gameObject.SetActive(true);
                    handIcon.gameObject.SetActive(false);
                    handIconIsVisible = false;
                    handIconIsVisible = false;
                }
            }
            else
            {//if there is a hit but the object is not interactable
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                onTarget = false;
                handIconIsVisible = false;
                healthToBeRestored = 0;
                
            }
            #endregion

            #region -------- Bed interaction -------

            Bed bed = selectionTransform.GetComponent<Bed>();
            if (bed && bed.playerInRange)
            {
                int currentHour = DayNightSystem.Instance.currentHour;
                if (currentHour >= 22 || currentHour <= 6)
                {
                    interaction_text.text = "Sleep";
                    interaction_Info_UI.SetActive(true);
                   
                    if (Input.GetMouseButtonDown(0) && PlacementSystem.Instance.inPlacementMode == false)
                    {
                        bed.Sleep();
                    }
                }
                else
                {
                    interaction_text.text = "Can't sleep now";
                    interaction_Info_UI.SetActive(true);
                }
            }
            #endregion

            #region  ------- Candle Interaction  -------

            Candle candle = selectionTransform.GetComponent<Candle>();
            if (candle && candle.playerInRange)
            {
                if (candle.isLit)
                {
                    interaction_text.text = "Turn off";
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    interaction_text.text = "Light";
                    interaction_Info_UI.SetActive(true);
                }
                
                if (Input.GetMouseButtonDown(0))
                {
                    if(candle.isLit)
                    {
                        candle.TurnOffCandle();
                    }
                    else
                    {
                        candle.LightCandle();
                    }
                 
                }
            }
            #endregion

            #region ------ Storage Box Interaction -------
            StorageBox storageBox  = selectionTransform.GetComponent<StorageBox>();

            if(storageBox && storageBox.playerInRange && PlacementSystem.Instance.inPlacementMode==false)
            {
                interaction_text.text = "Open";
                interaction_Info_UI.SetActive(true);

                selectedStorageBox = storageBox.gameObject;

                if(Input.GetMouseButtonDown(0))
                {
                    StorageManager.Instance.OpenBox(storageBox);
                }
            }
            else
            {
                if(selectedStorageBox != null)
                {
                    selectedStorageBox = null;
                }
                
            }

            #endregion

            
            #region ------ Campfire Interaction -------
            Campfire campfire = selectionTransform.GetComponent<Campfire>();

            if (campfire && campfire.playerInRange && PlacementSystem.Instance.inPlacementMode == false)
            {
                interaction_text.text = "Interact";
                interaction_Info_UI.SetActive(true);
               
                if(campfire.isCooking)
                {
                    campfire.HoverOverFire();
                    BodyTemperatureBar.Instance.IncreaseBodyTemperature();
                    interaction_text.text = "Cooking";
                    fireHolder.gameObject.SetActive(true);
                }

                selectedCampfire = campfire.gameObject;

                if (Input.GetMouseButtonDown(0) && campfire.isCooking ==false)
                {
                    campfire.OpenUI();
                }
            }
            else
            {
                if (selectedCampfire != null)
                {
                    selectedCampfire = null;
                    fireHolder.gameObject.SetActive(false);
                }

            }
            #endregion


            #region ------ Furnace Interaction -------
            Furnace furnace = selectionTransform.GetComponent<Furnace>();

            if(furnace && furnace.playerInRange && PlacementSystem.Instance.inPlacementMode == false)
            {

                interaction_text.text = "Interact";
                interaction_Info_UI.SetActive(true);
                if (furnace.isCooking)
                {
                    furnace.HoverOverFire();
                    BodyTemperatureBar.Instance.IncreaseBodyTemperature();
                    interaction_text.text = "Cooking";
                    fireHolder.gameObject.SetActive(true);
                }

                selectedFurnace = furnace.gameObject;

                if (Input.GetMouseButtonDown(0) && furnace.isCooking == false)
                {
                    furnace.OpenUI();
                }
            }
            else
            {
                if (selectedFurnace != null)
                {
                    selectedFurnace = null;

                    fireHolder.gameObject.SetActive(false);
                }

            }
            #endregion


            #region --------- Anvil Interaction ---------
            Anvil anvil = selectionTransform.GetComponent<Anvil>();
            if (anvil && anvil.playerInRange)
            {
                //Debug.Log("Anvil");
                if (EquipSystem.Instance.selectedItem)
                {
                    Weapon weaponInHand = EquipSystem.Instance.selectedItem.GetComponent<Weapon>();
                    if (weaponInHand != null)
                    {
                        //place the item on the anvil
                        if (weaponInHand.placeableOnAnvil == true && !anvil.isToolOnAnvil)
                        {
                            //Debug.Log("Place item");
                            interaction_text.text = "Place item";
                            anvil.nrOfHits = 0;
                            interaction_Info_UI.SetActive(true);
                            if (Input.GetMouseButtonDown(0))
                            {
                                anvil.PlaceItemForRepairing(weaponInHand.name);
                            }
                        }
                    }
                }

            }

            

            Blade blade = selectionTransform.GetComponentInChildren<Blade>();
            
            if(blade && blade.playerInRange)
            {
                
                if (EquipSystem.Instance.GetWeaponName().Contains("Hammer"))
                {
                    
                    Debug.Log("Blade in sight");
                    canHitWithHammer = true;
                    interaction_text.text = "Hit";
                    //get the parent of the blade
                    anvil = blade.transform.parent.transform.parent.GetComponent<Anvil>();
                    

                    interaction_Info_UI.SetActive(true);
                    if (EquipableItem.Instance.canHit && Input.GetMouseButtonDown(0) && anvil)
                    {
                        isHittingWithHammer = true;
                        //Debug.Log("Repairing");
                        OnHammerHit?.Invoke();
                        EquipSystem.Instance.decreaseWeaponHealth(5);
                        anvil.Repair();
                        
                        
                    }
                }
            }
            else
            {
                //Debug.Log("Blade not in range");
                canHitWithHammer = false; 
            }
            


            #endregion

            
            #region ------ Soil Interaction -------
            Soil soil = selectionTransform.GetComponent<Soil>();
            if (soil && soil.playerInRange)
            {
                
                if (soil.isEmpty && EquipSystem.Instance.IsPlayerHoldingSeed())
                {
                    
                    string seedName = EquipSystem.Instance.selectedItem.GetComponent<InventoryItem>().thisName;
                    string onlyPlantName = seedName.Substring(0, seedName.Length - 4);
                    interaction_text.text = "Plant " + onlyPlantName;
                        
                    interaction_Info_UI.SetActive(true);

                    if(Input.GetMouseButtonDown(0))
                    {
                        soil.PlantSeed();
                        Debug.Log("Destroy item 3");
                        Destroy(EquipSystem.Instance.selectedItemModel);
                        Destroy(EquipSystem.Instance.selectedItem);
                    }
                }
                else if(soil.isEmpty && !EquipSystem.Instance.IsPlayerHoldingBottle())
                {
                    interaction_text.text = "Soil";
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    if(EquipSystem.Instance.IsPlayerHoldingBottle())
                    {
                        Weapon weapon = EquipSystem.Instance.selectedItem.GetComponent<Weapon>();
                        if(weapon.weaponHealth>0)
                        {
                            //Debug.Log("Player is holding bottle && bottleHealth>0");
                            //Debug.Log("Player is holding bottle && looking at soil");
                            GameObject plant = soil.GetComponentInChildren<Plant>().gameObject;
                            if (plant)
                            {
                                if (soil.GetComponentInChildren<Plant>().isWatered)
                                {

                                    interaction_text.text = soil.plantName;
                                    interaction_Info_UI.SetActive(true);
                                }
                                else
                                {
                                    interaction_text.text = "Water";
                                    interaction_Info_UI.SetActive(true);

                                    if (Input.GetMouseButtonDown(0))
                                    {
                                        soil.GetComponentInChildren<Plant>().isWatered = true;
                                        soil.MakeSoilWatered();
                                        EquipSystem.Instance.decreaseWeaponHealth(5);
                                    }
                                }
                            }
                        }
                        
                        
                    }
                    else
                    {
                        
                        interaction_text.text = soil.plantName;
                        interaction_Info_UI.SetActive(true);

                    }
                    
                }
                selectedSoil = soil.gameObject;

            }
            else
            {
                if (selectedSoil != null)
                {
                    selectedSoil = null;
                    interaction_text.text = "";
                    chopHolder.gameObject.SetActive(false);
                }

            }
            #endregion
            
            


            #region ------ Water Interaction -------   
             Water water = selectionTransform.GetComponent<Water>();
            if (water)
            {
                //Debug.Log("The player is looking at water");
                if (water.playerInRange)
                {
                    IsNearWater = true;
                    interaction_Info_UI.SetActive(false);
                    //Debug.Log("The player is near the water");
                    selectedWater = water.gameObject;

                    if (EquipSystem.Instance.IsPlayerHoldingBottle())
                    {
                        //Debug.Log("Holding bootle Water");
                        interaction_text.text = "Collect water";

                        //ACTIVATE ANIMATION

                        if (Input.GetMouseButtonDown(0))
                        {
                            SoundManager.Instance.PlaySound(SoundManager.Instance.collectWater);
                            EquipSystem.Instance.increaseWeaponHealth();
                        }


                    }
                    else
                    {
                        //Debug.Log("not holding bootle Water");
                        interaction_text.text = "Water";


                    }
                    interaction_Info_UI.SetActive(true);
                }
                else
                {
                    IsNearWater = false;
                }
            }

            #endregion


            
            if (!npc && !interectable && !campfire && !storageBox && !soil && !miningRock
                && !choppableTree && !animal && !furnace && !water && !anvil && !blade && !bed && !candle && !seedShop)
            {
                interaction_text.text = "";
                interaction_Info_UI.SetActive(false);

            }

            

            //#endregion


        }
        else
        {   
                // Hide the UI if no object is hit
                centerDotImage.gameObject.SetActive(true);
                handIcon.gameObject.SetActive(false);
                onTarget = false;
                interaction_Info_UI.SetActive(false);
   
        }

        if (EquipSystem.Instance.GetWeaponName().Contains("Bottle") &&
            interaction_text.text != "Water" &&
            interaction_text.text != "Collect water"
            )
        {
            Weapon weaponComponent = EquipSystem.Instance.selectedItem.GetComponent<Weapon>();
            if(weaponComponent.weaponHealth<=0)
            {
                interaction_text.text = "Empty";
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                interaction_text.text = "Drink";
                interaction_Info_UI.SetActive(true);
                if (Input.GetMouseButtonDown(0) && PlayerState.Instance.isEating==false)
                {
                    PlayerState.Instance.consumingFunction(0f,0f,30f,"Water");
                    EquipSystem.Instance.decreaseWeaponHealth(5);
                }
            }
           
            
           
        }
    }


    private void Loot(Lootable lootable)
    {
        if (lootable.wasLootCalculated == false)
        {
            List<LootRecieved> recievedLoot = new List<LootRecieved>();

            foreach (LootPossibility loot in lootable.possibleLoot)
            {
                var lootAmount = UnityEngine.Random.Range(loot.amountMin, loot.amountMax + 1);
                if (lootAmount != 0)
                {
                    LootRecieved lt = new LootRecieved();
                    lt.item = loot.item;
                    lt.amount = lootAmount;

                    recievedLoot.Add(lt);
                }
            }

            lootable.finalLoot = recievedLoot;
            lootable.wasLootCalculated = true;

        }


        //Spawning the loot on the ground
        Vector3 lootSpawnPosition = lootable.gameObject.transform.position;
        float distance;
        foreach (LootRecieved lootRecieved in lootable.finalLoot)
        {
            for (int i = 0; i < lootRecieved.amount; i++)
            {
                //pick a random distance 
                distance = UnityEngine.Random.Range(-0.5f, 0.5f);
                if (lootRecieved.item.name == "Skin")
                {
                    GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootRecieved.item.name + "_Model"),
                   new Vector3(lootSpawnPosition.x + distance, lootSpawnPosition.y + 0.2f, lootSpawnPosition.z + distance),
                   Quaternion.Euler(-90f, 0, 0));
                }
                else
                {
                    GameObject lootSpawn = Instantiate(Resources.Load<GameObject>(lootRecieved.item.name + "_Model"),
                    new Vector3(lootSpawnPosition.x + distance, lootSpawnPosition.y + 0.2f, lootSpawnPosition.z + distance),
                    Quaternion.Euler(0, 0, 0));
                }

            }
        }

        // If we want the blood puddle to stay on the ground

        if (lootable.GetComponent<Animal>())
        {
            lootable.GetComponent<Animal>().bloodPuddle.transform.SetParent(lootable.transform.parent);
        }

        // Destroy Looted body
        //Debug.Log("Item Destroyed 1");
        //Destroy(lootable.gameObject);


    }



}