using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerState : MonoBehaviour
{
   public static PlayerState Instance { get; set; }

    public int numberOfAnimalsActive = 0;

    #region ----------   Player State Variables   ----------

   public Vector3 spawnPosition;
    public Vector3 currentPosition;

    // ---- Player Health ---- ///
    public float currentHealth;
    public float maxHealth;

    float timeToZeroHealth = 40f; // Time in seconds to reach 0 health
    float healthDecreaseTimer = 0f; // Tracks the time elapsed
    bool isHealthDecreasing = false;
    float initialHealth; // To store the player's health when the decrease starts
    public bool isBandageApplied = false;

    // ---- Player Calories ---- ///
    public float currentCalories;
    public float maxCalories;

    float distanceTravelled = 0;
    Vector3 lastPosition;
    public GameObject playerBody;

    // ---- Player Hydration ---- ///
    public float currentHydration;
    public float maxHydration;

    // ---- Player Energy ---- ///
    public float currentEnergy;
    public float maxEnergy;

    // ---- Player Body Temperature  ---- ///
    public float currentBodyTemperature;
    public float maxBodyTemperature;

    public bool isEating=false;
    public float armorProtectionInPercent = 0;

    // ---- Player Voice ---- ///
    public Queue<string> audioQueue = new Queue<string>();
    private Coroutine voiceCoroutine;

    int distanceCounterPerFramework = 0; //counter for the distance travelled
    #endregion


   

    private void Awake()
    {
        //if there is already an instance of the inventory system
        //then destroy this instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //set the instance to this
            Instance = this;
        }
    }

    private void Start()
    {
        spawnPosition = playerBody.transform.position;
        currentPosition = spawnPosition;

        currentHealth = maxHealth;
        currentCalories = maxCalories;
        currentHydration = maxHydration;
        currentEnergy = maxEnergy;
        currentBodyTemperature = maxBodyTemperature;

        StartCoroutine(decreaseHydration());
        StartCoroutine(decreaseCalories());

        InvokeRepeating(nameof(CheckIfThePlayerIsInBadCondition), 0f, 1f);
    }


    void Update()
    {
        currentPosition = playerBody.transform.position;
        distanceCounterPerFramework++;

        /*if (currentHealth <= 0)
        {
            Debug.Log("Player is dead");
        }*/

        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
        }

        distanceTravelled += Vector3.Distance(playerBody.transform.position, lastPosition);
        lastPosition = playerBody.transform.position;


        if (distanceTravelled >= 5)
        {
            currentCalories -= 5;
            distanceTravelled = 0;
        }

        int damage = 0;
        if (currentCalories <= 0)
        {
            damage += 1;
        }
        if (currentHydration <= 0)
        {
            damage += 1;
        }


        if (currentBodyTemperature <= 29)
        {
            damage += 1;
        }

        if (currentCalories <= 0 || currentHydration <= 0 || currentBodyTemperature <= 29)
        {
            if (distanceCounterPerFramework >= 150)
            {
                currentHealth -= damage;
                distanceCounterPerFramework = 0;
            }
        }

        if (currentCalories <= 0)
        {
            currentCalories = 0;
        }


        //bandage 
        if (isBandageApplied)
        {
            BandageIsNotAppliedAnymore();
        }

        //health decrease to zero
        if (currentHealth < 25 && !isBandageApplied)
        {
            DecreaseHealthToZero();

        }

        //Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (currentEnergy > -10)
            {
                currentEnergy -= 0.15f;
                currentHydration -= 0.02f;
            }

        }
        else
        {
            if (currentEnergy < maxEnergy)
            {
                currentEnergy += 0.15f;
            }
        }
    }




    #region ----------   Restart Player State   ----------

    public void RestartPlayerPosition()
    {
        // Freeze movement (for CharacterController or Rigidbody)
        CharacterController controller = playerBody.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // Disable CharacterController
        }

        Rigidbody rb = playerBody.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero; // Stop Rigidbody motion
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false; // Disable gravity temporarily
        }

        // Teleport player to spawn position
        Debug.Log("Spawning player at: " + spawnPosition);
        playerBody.transform.position = spawnPosition;
        Debug.Log("Player teleported to: " + playerBody.transform.position);

        // Re-enable movement
        if (rb != null)
        {
            rb.useGravity = true; // Re-enable gravity
        }

        if (controller != null)
        {
            controller.enabled = true; // Re-enable CharacterController
        }
    }



    public void RestartPlayerStatus()
    {
        // Reset health
        currentHealth = maxHealth;
        isHealthDecreasing = false;
        healthDecreaseTimer = 0f;

        // Reset calories
        currentCalories = maxCalories;

        // Reset hydration
        currentHydration = maxHydration;

        // Reset energy
        currentEnergy = maxEnergy;

        // Reset body temperature
        currentBodyTemperature = maxBodyTemperature;

        // Clear any status effects like bandages
        isBandageApplied = false;

        // Reset voice/audio queue
        audioQueue.Clear();
        if (voiceCoroutine != null)
        {
            StopCoroutine(voiceCoroutine);
            voiceCoroutine = null;
        }

        // Reset distance traveled for calorie tracking
        distanceTravelled = 0;
        lastPosition = playerBody.transform.position;

        // Log or debug to confirm reset
        Debug.Log("Player status has been reset.");
    }

    #endregion


    private void CheckIfThePlayerIsInBadCondition()
    {
        if (currentHealth <= 50 && currentHealth > 25 && !audioQueue.Contains("lowHealth"))
        {
            audioQueue.Enqueue("lowHealth");
        }

        if (currentCalories <= 1000 && !audioQueue.Contains("lowCalories"))
        {
            audioQueue.Enqueue("lowCalories");
        }

        if (currentHydration <= 25 && !audioQueue.Contains("lowWater"))
        {
            audioQueue.Enqueue("lowWater");
        }

        currentBodyTemperature  = BodyTemperatureBar.Instance.currentBodyTemperature;
        if (currentBodyTemperature < 32 && !audioQueue.Contains("lowTemperature"))
        {
            audioQueue.Enqueue("lowTemperature");
        }

        // Start the coroutine if it's not already running
        if (audioQueue.Count > 0 && voiceCoroutine == null)
        {
            voiceCoroutine = StartCoroutine(PlayAudios());
        }
    }


    #region ---------- Audios For Player ----------
    private IEnumerator PlayAudios()
    {
        while (audioQueue.Count > 0)
        {
                string condition = audioQueue.Dequeue();
                switch (condition)
                {
                    case "lowHealth":
                        if (currentHealth <= 50 && currentHealth > 25)
                        {
                            PlayerVoice.Instance.PlayVoice(PlayerVoice.Instance.lowHealth);
                        }
                        break;
                    case "lowCalories":
                        if(currentCalories <= 1000)
                        {
                            PlayerVoice.Instance.PlayVoice(PlayerVoice.Instance.lowCalories);
                        }
                        break;
                    case "lowWater":
                        if(currentHydration <= 25)
                        {
                            PlayerVoice.Instance.PlayVoice(PlayerVoice.Instance.lowWater);
                        }
                        break;
                    case "lowTemperature":
                        if(currentBodyTemperature < 32)
                        {
                            PlayerVoice.Instance.PlayVoice(PlayerVoice.Instance.lowTemperature);
                        }
                        break;
                }

                // Wait for 30 seconds before playing the next sound
                yield return new WaitForSeconds(30f);
            
        }
        voiceCoroutine = null;
    }


    public void PlaySpecificShieldHitSound()
    {
        string shieldName = InventorySystem.Instance.GetShieldName();
        if (shieldName == "WoodShield")
        {
            ShieldSystem.Instance.PlayWoodShieldHitSound();
        }
        else if (shieldName == "Iron Shield")
        {
            ShieldSystem.Instance.PlayIronShieldHitSound();
        }
    }
    #endregion



    #region ----------   Setters for player status   ----------

    public async void setHealth(float health)
    {
        if (currentHealth < health)
        {
            await Task.Delay(1000);
            currentHealth++;
            setHealth(health);
        }
    }

    public async void setDecreasingHealth(float health)
    {
        if (currentHealth > health)
        {
            await Task.Delay(1000);
            currentHealth--;
            setDecreasingHealth(health);
        }
    }

    public void setCalories(float calories)
    {
        currentCalories = calories;
    }

    public void setHydration(float hydration)
    {
        currentHydration = hydration;
    }

    #endregion


    #region ----------  Functions to decrease the player status   ----------

    //function to decrease the hydration of the player over time
    IEnumerator decreaseHydration()
    {
        while (currentHydration>0)
        {
            currentHydration -= 1;
            yield return new WaitForSeconds(10f);

        }
    }

    //function to decrease the calories of the player over time
    IEnumerator decreaseCalories()
    {
        while (currentCalories > 0)
        {
            currentCalories -= 1;
            yield return new WaitForSeconds(5f);
        }
    }

    //function to decrease the health of the player when hit
    public void DecreaseHealth(float damage)
    {
        //decrease the health of the player when is attacked
        //   => if the player has armor, the damage will be reduced
        //       => the armor health will be decreased
        bool shieldWasHit = false;
        //adjust the damage with armor
        if (ShieldSystem.Instance != null && ShieldSystem.Instance.isDefending)
        {
            //deal damage to the shield
            shieldWasHit = true;
            damage = damage / 2;

            //play the sound of the shield hit
            Debug.Log("Shield was hit.");
            PlaySpecificShieldHitSound();
            InventorySystem.Instance.DecreaseShieldHealth(damage);
        }


        armorProtectionInPercent = InventorySystem.Instance.GetArmorProtection();

        //bear attack damage 30 => with protection 100% => 15
        //bear attack damage 30 => with protection 50% => 20
        //bear attack damage 30 => with protection 0% => 30

        if (armorProtectionInPercent > 0 && shieldWasHit == false)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.armorHit);
            InventorySystem.Instance.DecreaseArmorHealth(damage);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.playerGetHit);
        }

        damage = damage - (damage * armorProtectionInPercent / 100) / 2;
        damage = Mathf.Round(damage);

        Debug.Log("Damage: " + damage);

        currentHealth -= damage;

        if (currentHealth < 0)
        {

            currentHealth = 0;
            PlayerDies();
        }

        // Reset the initial health for DecreaseHealthToZero if it's already decreasing
        if (isHealthDecreasing)
        {
            initialHealth = currentHealth;
        }
    }

    //when the player's health is < 25, the health will decrease to zero over time
    public void DecreaseHealthToZero()
    {
        if (!isHealthDecreasing)
        {
            // Start the health decrease process
            isHealthDecreasing = true;
            healthDecreaseTimer = 0f; // Reset the timer
            initialHealth = currentHealth; // Store the starting health
        }

        if (isHealthDecreasing)
        {
            // Increment the timer by the time passed since the last frame
            healthDecreaseTimer += Time.deltaTime;

            // Calculate the total amount of health to decrease
            float healthToDecrease = initialHealth / timeToZeroHealth * Time.deltaTime;

            // Decrease health gradually over time
            currentHealth -= healthToDecrease;

            // Ensure the health doesn't drop below 0
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                isHealthDecreasing = false;
            }
            else if (healthDecreaseTimer >= timeToZeroHealth)
            {
                currentHealth = 0;
                isHealthDecreasing = false;
            }
        }
    }

    #endregion



    public void PlayerDies()
    {
        //play the sound of the player death
        //SoundManager.Instance.PlaySound(SoundManager.Instance.playerDeath);
        //show the game over screen
        RestartPlayerStatus();
        RestartPlayerPosition();
        GameOver.Instance.PlayerDies();
    }



    #region ----------   Consuming Functions   ----------

    private async void BandageIsNotAppliedAnymore()
    {
        await Task.Delay(3000);
        isBandageApplied = false;
    }


    public async void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect, string thisName)
    {
        int waitForConsuming = 5000;

        if (thisName == "Bandage")
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.bandage);
            isBandageApplied = true;
            waitForConsuming = 3000;
        }

        if (thisName.Contains("Steak"))
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.mushroomEating); //the sounds are similar
        }

        if (thisName == "Water")
        {
            //SoundManager.Instance.PlaySound(SoundManager.Instance.collectWater);
            waitForConsuming = 3000;
        }



        isEating = true;

        // Lock the cursor and make it invisible
        //CursorManager.Instance.LockCursor();

        // Wait asynchronously for the duration of eating

        await Task.Delay(waitForConsuming);

        //CursorManager.Instance.FreeCursor();

        isEating = false;

        // Apply the effects after the delay
        healthEffectCalculation(healthEffect);
        caloriesEffectCalculation(caloriesEffect);
        hydrationEffectCalculation(hydrationEffect);

        Debug.Log(thisName + " consumed.");
    }


    public void healthEffectCalculation(float healthEffect)
    {
        // --- Health --- //

        float healthBeforeConsumption = currentHealth;
        

        if (healthEffect != 0)
        {
            if ((healthBeforeConsumption + healthEffect) > maxHealth)
            {
                setHealth(maxHealth);
            }
            else
            {
                if (healthEffect > 0)
                {
                    setHealth(healthBeforeConsumption + healthEffect);
                }
                else
                {
                    //there is an item that decreases the health
                    setDecreasingHealth(healthBeforeConsumption + healthEffect);
                }
            }
        }
    }


    public void caloriesEffectCalculation(float caloriesEffect)
    {
        // --- Calories --- //

        float caloriesBeforeConsumption = currentCalories;
        

        if (caloriesEffect != 0)
        {
            if ((caloriesBeforeConsumption + caloriesEffect) > maxCalories)
            {
                setCalories(maxCalories);
            }
            else
            {
                setCalories(caloriesBeforeConsumption + caloriesEffect);
            }
        }
    }


    public void hydrationEffectCalculation(float hydrationEffect)
    {
        // --- Hydration --- //

        float hydrationBeforeConsumption = currentHydration;
        

        if (hydrationEffect != 0)
        {
            if ((hydrationBeforeConsumption + hydrationEffect) > maxHydration)
            {
                setHydration(maxHydration);
            }
            else
            {
                setHydration(hydrationBeforeConsumption + hydrationEffect);
            }
        }
    }

    #endregion
}
