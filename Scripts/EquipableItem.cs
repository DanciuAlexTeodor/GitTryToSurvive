using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

//if we place this script on a weapon, its going to place an animation
//to the weapon
[RequireComponent(typeof(Animator))]
public class EquipableItem : MonoBehaviour
{

    public static EquipableItem Instance { get; set; }

    private bool canDealDamage = true;
    public int hitMomentDuration;
    public bool canHit = true;
    public Animator animator;
    public GameObject specialEffects;
    public bool isHitingRock = false;

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


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }


    private IEnumerator DamageCooldown(float cooldown)
    {
        canDealDamage = false;
        yield return new WaitForSeconds(cooldown);
        canDealDamage = true;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if(other.gameObject.tag=="Player") return;

        //Debug.Log("hit something" + other.gameObject.name);

        //in order to avoid damaging the enemy multiple times with the same hit
        if (!canDealDamage) return;

        //i want to store the parent of other in a variable

       
        //The problem is that the Collier is either the prefab itself or the parent of the prefab
        //so we need to check who is the collider 
        GameObject creature = other.gameObject;
        int colliderWasFoundInPrefab = 0;
        if (creature.GetComponent<Enemy>() != null || creature.GetComponent<Animal>() != null)
        {
            colliderWasFoundInPrefab = 1;
        }

        if(colliderWasFoundInPrefab==0 && other.gameObject.transform.parent !=null)
        {
            creature = other.gameObject.transform.parent.gameObject;
        }

        Enemy enemy = creature.GetComponent<Enemy>();
        Animal animal = creature.GetComponent<Animal>();
        int damage = EquipSystem.Instance.GetWeaponDamage();

        if (animal!=null)
        {
            Debug.Log("animal hit");
            animal.TakeDamage(damage);
            StartCoroutine(DamageCooldown(1f));
        }
        else
        {
            if(enemy!=null)
            {   
                Debug.Log("enemy hit");
                enemy.TakeDamage(damage);
                StartCoroutine(DamageCooldown(1f));
            }
        }
        
    }



    
    public async Task waitForHitToFinish()
    {
        int waitTime = hitMomentDuration;
        await Task.Delay(waitTime);
        //Debug.Log("can hit again");
        canHit = true;
    }

    private void consumeFood()
    {
        animator.SetTrigger("eat");
     
        GameObject food = EquipSystem.Instance.selectedItem;
        float healthEffect = food.GetComponent<InventoryItem>().healthEffect;
        float caloriesEffect = food.GetComponent<InventoryItem>().caloriesEffect;
        float hydrationEffect = food.GetComponent<InventoryItem>().hydrationEffect;
        string thisName = EquipSystem.Instance.selectedItemName;
        PlayerState.Instance.consumingFunction(healthEffect, caloriesEffect, hydrationEffect, thisName);
        waitForHitToFinish();
    }


    // Update is called once per frame
    void Update()
    {
        //if we click the left mouse button
        if (Input.GetMouseButtonDown(0) && 
            InventorySystem.Instance.isOpen==false && 
            CraftingSystem.Instance.isOpen==false &&
            SelectionManager.Instance.handIconIsVisible==false &&
            !ConstructionManager.Instance.inConstructionMode &&
            !EquipSystem.Instance.isBackpackOpen
            ) 
        {

            if (EquipSystem.Instance.isEatble() && canHit)
            {
                canHit = false;
                //Debug.Log("consume food");
                consumeFood();
                return;

            }

            //Debug.Log("can hit: ");
            if (canHit == true)
            {
                GameObject selectedTree = SelectionManager.Instance.selectedTree;
                GameObject selectedRock = SelectionManager.Instance.selectedRock;

              
                if (selectedTree != null)
                {
                    
                    selectedTree.GetComponent<ChoppableTree>().GetHit();
                }

                if (selectedRock != null)
                {
                   
                    selectedRock.GetComponent<MiningRock>().GetHit();
                }

                

                if (SelectionManager.Instance.chopHolderText.text == "Loot" && EquipSystem.Instance.GetWeaponName().Contains("Knife"))
                {
                    animator.SetTrigger("loot");
                }
                else if(SelectionManager.Instance.interaction_text.text == "Collect water" && EquipSystem.Instance.GetWeaponName().Contains("Bottle"))
                {
                    animator.SetTrigger("collect");
                }
                

                else if(SelectionManager.Instance.interaction_text.text == "Drink")
                {
                    animator.SetTrigger("drink");
                }
                else if (EquipSystem.Instance.GetWeaponName().Contains("Bow"))
                {
                    if (Bow.Instance.isArrowLoaded)
                    {
                        canHit = false;
                        animator.SetTrigger("hit");
                        Bow.Instance.LaunchArrow();
                        waitForHitToFinish();
                    }
                }
                else
                {
                    canHit = false;
                    //Debug.Log("Start hitting");
                    animator.SetTrigger("hit");
                    waitForHitToFinish();
                    
                }
            }
        }
    }

    public void PlayWateringSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.watering);
    }

    public void PlayMeatCuttingSound()
    {
        if(SelectionManager.Instance.isLooting)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.meatCutting);
        }
       
    }


    //These functions are called from the animation
    // so i can't make them more generic
    public void PlaySwingHitSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.swing);
    }

    public void PlayLootAnimation()
    {
        animator.SetTrigger("loot");

    }

    public void PlayDigSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.dig);
    }

    public void PlayDrinkWaterSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.drink);
    }

    public void PlayHammerHitIronSound()
    {
        if(SelectionManager.Instance.canHitWithHammer)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.hammerHitIron);
        }
        
    }


    public void PlayHammerSpecialEffects()
    {
        if (SelectionManager.Instance.canHitWithHammer)
        {
            if (specialEffects != null)
            {
                //play the special effects
                specialEffects.SetActive(true);

            }
        }

    }

    public void StopHammerSpecialEffects()
    {
        if(specialEffects!=null)
        {
            Debug.Log("stop hammer special effects");
            specialEffects.SetActive(false);
        }
    }

    public void PlayPickaxeSpecialEffects()
    {

        if (specialEffects != null && isHitingRock)
        {
            
            specialEffects.SetActive(true);

        }
    }

    public void StopPickaxeSpecialEffects()
    {
        if (specialEffects != null)
        {
            specialEffects.SetActive(false);
        }
    }

    public void BowLoad()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.bowLoad);
    }

    public void BowShoot()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.bowShoot);
    }

    public void PlayEatSound()
    {
        string foodName = EquipSystem.Instance.GetWeaponName();
        List<string> crancyFood = new List<string> { "Apple", "Turnip", "Carrot" };
        List<string> softFood = new List<string> { "Mushroom", "Eggplant", "Tomato", "Pumpkin" };

        //if the foodName is a crancy food =>> play the crancy food sound
        if (crancyFood.Contains(foodName))
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.appleEating);
        }
        else if (softFood.Contains(foodName))
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.mushroomEating);
        }

        EquipSystem.Instance.decreaseWeaponHealth(5);
    }

    public void PlayFireSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.fire);
    }



    

   
}
