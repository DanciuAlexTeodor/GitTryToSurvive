using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Animal : MonoBehaviour
{
    public static event Action<string> OnEnemyKilledWithBow;

    public bool isActivated=false;
    public float zoneToActivateAnimal=200;

    public string animalName;
    public UnityEngine.AI.NavMeshAgent animal;
    //this types are for the audio 
    public enum AnimalType
    {
        Rabbit,
        Chicken,
        Goat,
        Deer,
        Pig,
        Cow,
        Wolf,
        Bear
    }
    
    public AnimalType thisAnimalType;

    //this type is for saving
    public string animalType;
    //positio
    
    public Vector3 position
    {
        get { return transform.position; }
    }

    // Getter to access the current rotation 
    public Quaternion rotation
    {
        get { return transform.rotation; }
    }
 
    public Vector3 spawningLocation;
    public bool isFarmAnimal;
    public float runSpeed;
    public float walkSpeed;
    public float health;
    public float maxHealth;
    public int dayOfRevival;
    //when the player can attack the animal
    public int attackZone;
    public bool isInAttackZone;
    public bool isInFeedZone;
    public bool hasBeenFed;

    private Animator animator;

    private Vector3 stopPosition;
    private Vector3 moveDirection;

    private float walkTime;
    private float walkCounter;
    private float waitTime; 
    private float waitCounter;

    private bool isFleeing = false;
    private bool isIdle=false;


    public bool isDead = false;
    public bool canBeLooted = false;
    public bool looted=false;
    public int currentLootHealth;
    public int maxLootHealth;

    public float lastTimeDirectionChanged = 0f;


    [Header("Flee Settings")]
    public Transform hunter;
    public float runTriggerZone;
    public float foodDetectionRange;
    public bool isFoodInPlayersHand;
    public float calmDownDistance;
    public float minFleeDistance;
    public float maxFleeDistance;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip normalBehaviourSound;

    [SerializeField] AudioClip chickenHitSound;
    [SerializeField] AudioClip chickenDieSound;

    [SerializeField] AudioClip rabbitHitSound;
    [SerializeField] AudioClip rabbitDieSound;

    [SerializeField] AudioClip pigHitSound;
    [SerializeField] AudioClip pigDieSound;

    [SerializeField] AudioClip cowHitSound;
    [SerializeField] AudioClip cowDieSound;

    [SerializeField] AudioClip goatHitSound;
    [SerializeField] AudioClip goatDieSound;

    [SerializeField] AudioClip deerHitSound;
    [SerializeField] AudioClip deerDieSound;

    [SerializeField] AudioSource stabSoundChannel;
    [SerializeField] AudioClip getHit;

    [SerializeField] AudioSource walkRun; //pig, cow, goat, deer
    [SerializeField] AudioClip walk;
    [SerializeField] AudioClip run;

    private bool isWalking;
    private bool isRunning;
    private bool isPlayingWalkSound = false;
    private bool isPlayingRunSound = false;


    private ResourceHealthBar resourceHealthBar;



    [Header("Particles")]
    [SerializeField] public GameObject bloodSplashParticles;
    [SerializeField] public GameObject bloodPuddle;
    //animalul va fugi doar cand intra o amenintare in runTriggerZone
    //animalul va fi in stagiul de walk si idle cand nu este o amenintare (random)

    // iepurele se spawneaza => start =>  idle

    //idle:
    //   - isIdle = true (cat timp is idle e true, in functia update nu se va intampla nimic cat timp e iddle si nu apare un hunter)
    //   - Walk si Run sunt false
    //   -asteapta un timp random inainte de a alege o noua directie in pozitia de idle
    // is idle e fals cand waitCounter <= 0 


    //la choose direction: walktime= walkCounter = 8, waitTime = WaitCounter = 5
    //                     alegem un unghi random pentru a merge in directia respectiva
    //                     rotim obiectul pentru a se indrepta in directia aleasa
    //                     setam directia de miscare in fata



    //c3: nimic nu l deranjeaza pe iepure => idle

    

    private void Start()
    {
        spawningLocation = transform.position;
        animal = GetComponent<UnityEngine.AI.NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
        isIdle = true;
        ChooseRandomTimeToPlaySound();
        health = maxHealth;

        if(bloodPuddle!=null)
        {
            bloodPuddle.SetActive(false);
        }

        animalName = gameObject.name;
        animalName.Replace("(Clone)", "");
        //Debug.Log("Animal name: " + animalName);

    }

    private void ChooseRandomTimeToPlaySound()
    {
        if (!isDead)
        {
            float randomTime = UnityEngine.Random.Range(7f, 10f);
            Invoke("PlayNormalBehaviourSound", randomTime);
        }
       
    }

    private void PlayNormalBehaviourSound()
    {
        if (isDead || soundChannel == null || normalBehaviourSound == null)
        {
            return;
        }

        soundChannel.PlayOneShot(normalBehaviourSound);
        ChooseRandomTimeToPlaySound();

    }

    private void HandleWalkRunSounds()
    {
        if (isDead)
        {
            // Check if walkRun is null before accessing it
            if (walkRun != null && walkRun.isPlaying)
            {
                walkRun.Stop();
            }
            return;
        }

        if (animal.speed == walkSpeed)
        {
            isWalking = true;
            isRunning = false;
        }
        else if (animal.speed == runSpeed)
        {
            isRunning = true;
            isWalking = false;
        }
        else
        {
            isWalking = false;
            isRunning = false;
        }

        if (isWalking && !isPlayingWalkSound)
        {
            isPlayingWalkSound = true;
            isPlayingRunSound = false;
            if (walkRun != null)
            {
                walkRun.clip = walk;
                walkRun.Play();
            }
        }
        else if (isRunning && !isPlayingRunSound)
        {
            isPlayingRunSound = true;
            isPlayingWalkSound = false;
            if (walkRun != null)
            {
                walkRun.clip = run;
                walkRun.Play();
            }
        }
        else
        {
            if (!isWalking && !isRunning)
            {
                isPlayingRunSound = false;
                isPlayingWalkSound = false;
                if (walkRun != null && walkRun.isPlaying)
                {
                    walkRun.Stop();
                }
            }
        }
    }


    private void ChooseFleeDirection()
    {
        //Debug.Log("Flee direction chosen");
        //alegem un unghi random pentru a merge in directia respectiva
        //Debug.Log("Choosing flee direction");
        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        //rotim obiectul pentru a se indrepta in directia aleasa
        transform.localRotation = Quaternion.Euler(0f, randomAngle, 0f);
        //setam directia de miscare in fata 
        moveDirection = transform.forward;
    }

    
    private void AactivateDezactivateAnimal(float distanceFromPlayerToAnimal)
    {
        if (distanceFromPlayerToAnimal <= zoneToActivateAnimal)
        {
            if (isActivated == false)
            {
                gameObject.SetActive(true);
                PlayerState.Instance.numberOfAnimalsActive++;
            }
            isActivated = true;

        }
        else
        {
            if (isActivated == true)
            {
                gameObject.SetActive(false);
                PlayerState.Instance.numberOfAnimalsActive--;
            }

            isActivated = false;
        }
    }


    private void Update()
    {
        GameObject player = PlayerState.Instance.playerBody;
        Vector3 playerPosition = player.transform.position;
        float distanceFromPlayerToAnimal = Vector3.Distance(transform.position, playerPosition);

        /*AactivateDezactivateAnimal(distanceFromPlayerToAnimal);

        if(isActivated==false)
        {
            return;
        }
        */
        isInFeedZone = distanceFromPlayerToAnimal<=8f;
        
        isFoodInPlayersHand = EquipSystem.Instance.isFoodInPlayersHand();

        //float weaponRangeOfAttack = EquipSystem.Instance.getWeaponRangeOfAttack(); //THE EQUIPED WEAPON

        //cand hunterul intra in zona de lovire a animalului
        isInAttackZone = distanceFromPlayerToAnimal <= 5f;

        if (isDead) return;

        HandleWalkRunSounds();


       //cand hunterul intra in zona de food detection range a animalului, acesta se va plimba spre player
        if (distanceFromPlayerToAnimal <= foodDetectionRange && !isFleeing && isFoodInPlayersHand && isFarmAnimal)
        {
 
            if(distanceFromPlayerToAnimal > 6f)
            {
                Debug.Log("Walking towards player");
                // Trigger walking behavior
                Walk();
                //rotate the animal to face the player
                transform.LookAt(playerPosition);
                moveDirection = (playerPosition - transform.position).normalized;
                transform.position += moveDirection * walkSpeed * Time.deltaTime;
            }
            else if(distanceFromPlayerToAnimal <= 6f)
            {
                Idle();
            }
           

        }
        else if (isFleeing && distanceFromPlayerToAnimal > calmDownDistance && Time.time - lastTimeDirectionChanged >= 1f) // e ceva neinregula la asta
        {
            // If the animal has fled far enough, it calms down and resumes normal behavior
            Idle();
            ChooseDirection();
        }
        //if the animal is running and encounter an object, it will change it's direction
        else if (isFleeing)
        {
            Vector3 rayOrigin = transform.position + moveDirection * 0.5f; // Offset by 0.5 units forward
            RaycastHit hit;
           
            if (Physics.Raycast(rayOrigin, moveDirection, out hit, 1f) && Time.time-lastTimeDirectionChanged>=1f)
            {
                //Debug.Log("Hitting" + hit.collider.name);
                // If anything is detected, choose a new direction to flee
                ChooseFleeDirection();
                // Code to handle hitting obstacles
                lastTimeDirectionChanged = Time.time;
            }
            else
            {
                // Continue fleeing in the current direction if no obstacle is detected
                transform.position += moveDirection * runSpeed * Time.deltaTime;
            }
            
        }
        //cand hunter ul intra in zona de trigger a animalului si nu are mancare in mana, animalul fuge
        else if (distanceFromPlayerToAnimal <= runTriggerZone && !isFleeing && !hasBeenFed)
        {
            // Trigger fleeing behavior
            Run();
        }
        else if (!isFleeing)
        {
            if (isIdle)
            {
                waitCounter -= Time.deltaTime;

                if (waitCounter <= 0)
                {
                    //cand timpul de astepare a trecut, animalul va alege o noua directie si se va plimba
                    Walk();
                    ChooseDirection();
                }
            }
            else if (isWalking)
            {
                walkCounter -= Time.deltaTime;

                transform.position += moveDirection * walkSpeed * Time.deltaTime;

                if (walkCounter <= 0)
                {
                    Idle();
                    waitCounter = waitTime;
                }
            }

        }

   
    }

    public void ChooseDirection()
    {
        lastTimeDirectionChanged = Time.time;
        //Debug.Log("Choosing direction");
        // Randomize walk and wait times to add variability
        walkTime = UnityEngine.Random.Range(7, 10);
        waitTime = UnityEngine.Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        //alegem un unghi random pentru a merge in directia respectiva
        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        //rotim obiectul pentru a se indrepta in directia aleasa
        transform.localRotation = Quaternion.Euler(0f, randomAngle, 0f);
        //setam directia de miscare in fata 
        moveDirection = transform.forward;
    }

    private void Run()
    {
        isFleeing = true;
        isIdle = false;
        isWalking = false;
        animator.SetBool("Walk", false);
        animator.SetBool("Run", true);
        animal.speed = runSpeed;
        // Choose a random direction to flee to
        ChooseDirection();
    }

    public void Walk()
    {
        isIdle = false;
        isWalking = true;
        isFleeing = false;
        animator.SetBool("Walk", true);
        animator.SetBool("Run", false);
        animal.speed = walkSpeed;
    }

    public void Idle()
    {
        isIdle = true;
        isWalking = false;
        isFleeing = false;
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        //stop the animal from moving
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        animal.speed = 0;
    }

    public void FeedAnimal()
    {
        hasBeenFed = true;
    }


    public async Task TakeLootDamage(int damage)
    {
        SelectionManager.Instance.isLooting = true;

        await Task.Delay(2000); 
        currentLootHealth -= damage;

        GlobalState.Instance.resourceHealth = currentLootHealth;
        GlobalState.Instance.resourceMaxHealth = maxLootHealth;
     
        if(resourceHealthBar==null)
        {
            resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
        }

        if(resourceHealthBar!=null)
        {
            resourceHealthBar.UpdateHealthBar();
        }

        SelectionManager.Instance.isLooting = false;


        if (currentLootHealth <= 0)
        {
            looted = true;
        }
    }



    public async Task TakeDamage(int damage)
    {

        if (isDead) return;

        soundChannel.Stop();
        PlayHitSound(); //The animal's scream
        stabSoundChannel.PlayOneShot(getHit);
        
        Run();//Change the position of the animal and make it run

        bloodSplashParticles.SetActive(false);
        bloodSplashParticles.SetActive(true);


        //Debug.Log($"Initial Health: {health}");
        health -= damage;
        //Debug.Log($"Health after damage: {health}");
        health = Mathf.Clamp(health, 0, maxHealth);

        if (health <= 0)
        {
            //Debug.Log("Health is zero or less. Animal should die.");
            Die();

            //Daca pun aceste comenzi de mai jos deasupra lui Die(), Die() nu se va executa
            // cand soundChannel sau walkRun nu au audio clip-uri !!!!!!!!!!!!!!!!!
            soundChannel.Stop();
            walkRun.Stop();
            
        }
        
    }

    public async Task Die()
    {
        PlayDieSound();
        canBeLooted = true;
        resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
        //stop all the chanels
        isDead = true;

        if (soundChannel != null) soundChannel.Stop();
        if (walkRun != null) walkRun.Stop();

        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
        animator.SetTrigger("Die");

        Rigidbody rigidbody= GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        ActivateGravityAndDisableKinematicForStuckArrows();

        string enemyType = thisAnimalType.ToString();

        if(EquipSystem.Instance.GetWeaponName().Contains("Bow"))
        {
            OnEnemyKilledWithBow?.Invoke(enemyType);
        }

        await Task.Delay(2000);
        bloodPuddle.SetActive(true);

        dayOfRevival = TimeManager.Instance.dayInGame + 2;
        RevivalManager.Instance.AddDeadCreature(name, dayOfRevival, spawningLocation);
        TimeManager.Instance.OnDayPass.AddListener(DestroyBody);
        
    }

    
    public void DestroyBody()
    {
        if(TimeManager.Instance.dayInGame == dayOfRevival)
        {
            Destroy(gameObject);   
        }
    }

    private void ActivateGravityAndDisableKinematicForStuckArrows()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.Contains("Arrow"))
            {
                Rigidbody arrowRb = child.GetComponent<Rigidbody>();
                if (arrowRb != null)
                {
                    // Enable gravity and disable kinematic mode
                    arrowRb.isKinematic = false;
                    arrowRb.useGravity = true;

                    arrowRb.constraints = RigidbodyConstraints.None;
                }
            }
        }
    }

    public void HoverOverAnimal()
    {
        GlobalState.Instance.resourceHealth = currentLootHealth;
        GlobalState.Instance.resourceMaxHealth = maxLootHealth;

        if (resourceHealthBar == null)
        {
            resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
        }

        if (resourceHealthBar != null)
        {
            resourceHealthBar.UpdateHealthBar();
        }
    }

    private void PlayHitSound()
    {
        switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitHitSound);
                break;
            case AnimalType.Chicken:
                soundChannel.PlayOneShot(chickenHitSound);
                break;
            case AnimalType.Pig:
                soundChannel.PlayOneShot(pigHitSound);
                break;
            case AnimalType.Cow:
                soundChannel.PlayOneShot(cowHitSound);
                break;
            case AnimalType.Goat:
                soundChannel.PlayOneShot(goatHitSound);
                break;
            case AnimalType.Deer:
                soundChannel.PlayOneShot(deerHitSound);
                break;
            }
        }

    private void PlayDieSound()
    {
        switch(thisAnimalType)
        {
            case AnimalType.Rabbit:
                soundChannel.PlayOneShot(rabbitDieSound);
                break;
            case AnimalType.Chicken:
                soundChannel.PlayOneShot(chickenDieSound);
                break;
            case AnimalType.Pig:
                soundChannel.PlayOneShot(pigDieSound);
                break;
            case AnimalType.Cow:
                soundChannel.PlayOneShot(cowDieSound);
                break;
            case AnimalType.Goat:
                soundChannel.PlayOneShot(goatDieSound);
                break;
            case AnimalType.Deer:
                soundChannel.PlayOneShot(deerDieSound);
                break;
        }
    }
}
