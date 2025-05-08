using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent character;
    public static event Action<string> OnEnemyKilled;

    public enum EnemyType { Soulreeper, Bear, Wolf }
    public EnemyType enemyType;
    public string enemyName;

    //i need the spawning location for revival of the animal (the animal will be revived after a certain time in the
    //spawining location)
    public Vector3 spawningLocation;
    public float health;
    public float damage;
    public float walkSpeed;
    public float runSpeed;

    public Transform player;
    public float followRange;
    public float distanceToLosePlayer;
    public float attackRange; // The distance from which the zombie can attack
    public bool inRangeOfHit;

    public float screamDuration;
    public float attackDuration;
    public float getHitDuration;
    public bool shouldRunAfterPlayer = false;

    public bool isDead;
    public GameObject bloodPuddle;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip normalBehaviourSound;
    [SerializeField] AudioClip scream;
    [SerializeField] AudioClip beingHit;
    [SerializeField] AudioClip dieSound;
    [SerializeField] AudioClip attackSound;

    [SerializeField] AudioSource walkRun;
    [SerializeField] AudioClip walk;
    [SerializeField] AudioClip run;

    private Animator animator;

    public GameObject bloodSplashParticles;

    private float walkTime;
    private float walkCounter;
    private float waitTime;
    private float waitCounter;
    private bool isWalking;
    private bool isRunning;
    private bool isPlayingWalkSound = false;
    private bool isPlayingRunSound = false;
    private int dayOfRevival;

    private Vector3 moveDirection;
    private Vector3 startPosition;  // Store initial position to prevent unwanted teleporting

    private bool hasScreamed = false;
    private bool isAttacking = false; // To prevent multiple attack triggers

    private enum State { Idle, Walking, Running, Attacking, Dead, Screaming, Hit }
    private State currentState;

    void Start()
    {
        enemyName = enemyType.ToString();
        character = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        animator = GetComponent<Animator>();
        startPosition = transform.position;  // Store the initial position
        spawningLocation = startPosition;
        SetState(State.Idle);
        ChooseDurationAndDirection();
        ChooseRandomTimeToPlaySound();
    }

    #region ---------- Sounds ----------
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
            walkRun.Stop(); // Ensure walkRun is stopped
            isPlayingWalkSound = false;
            isPlayingRunSound = false;
            return; // Exit the method early
        }

        if (character.speed == walkSpeed)
        {
            isWalking = true;
            isRunning = false;
        }
        else if (character.speed == runSpeed)
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
            walkRun.clip = walk;
            //Debug.Log("Is walking");
            walkRun.Play();
        }
        else if (isRunning && !isPlayingRunSound)
        {
            isPlayingRunSound = true;
            isPlayingWalkSound = false;
            walkRun.clip = run;
            //Debug.Log("Is running");
            walkRun.Play();
        }
        else
        {
            if (!isWalking && !isRunning)
            {
                isPlayingRunSound = false;
                isPlayingWalkSound = false;
                walkRun.Stop();
            }
        }
    }





    //this function is called from the animation event
    public void PlayAttackSound()
    {
        soundChannel.PlayOneShot(attackSound);
    }



    #endregion

    void Update()
    {

            if (isDead && currentState == State.Dead)
            {
                return;
            }
            else
            {
                if (isDead && currentState != State.Dead)
                {
                    SetState(State.Dead);
                }
            }

            switch (currentState)
            {
                case State.Idle:
                    IdleBehaviour();
                    break;
                case State.Walking:
                    WalkingBehaviour();
                    break;
                case State.Running:
                    RunningBehaviour();
                    break;
                case State.Screaming:
                    break;
                case State.Attacking:
                    // No need to handle Attacking here; it's managed by the coroutine
                    break;
                case State.Hit:
                    break;
                case State.Dead:
                    break;
            }

            HandleWalkRunSounds();

            // Check for attack range first
            if (Vector3.Distance(transform.position, player.position) < attackRange)
            {

                if (!isAttacking) // Only attack if not already attacking
                {
                    isAttacking = true; // Prevent multiple attack triggers
                    SetState(State.Attacking);
                    StartCoroutine(AttackingBehaviour());
                }
            }
            // Check for follow range
            else if (Vector3.Distance(transform.position, player.position) < followRange)
            {
                if (!hasScreamed)
                {
                    hasScreamed = true;
                    SetState(State.Screaming);
                    StartCoroutine(Screaming());
                }
            }
            else
            {
                inRangeOfHit = false;
            }

        }

    public float GetDistanceFromPlayerToEnemy()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    private void SetState(State newState)
    {
        currentState = newState;

        // Reset all animator bools
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isDead", false);
        animator.SetBool("isHit", false);
        animator.SetBool("isIdle", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("isScreaming", false);

        // Set the specific state
        switch (newState)
        {
            case State.Idle:
                animator.SetBool("isIdle", true);
                waitCounter = waitTime;
                break;
            case State.Walking:
                character.speed = walkSpeed;
                animator.SetBool("isWalking", true);
                walkCounter = walkTime;
                break;
            case State.Running:
                character.speed = runSpeed;
                animator.SetBool("isRunning", true);
                break;
            case State.Attacking:
                animator.SetBool("isAttacking", true);
                break;
            case State.Dead:
                animator.SetBool("isDead", true);
                isDead = true;
                break;
            case State.Screaming:
                //Debug.Log("Screaming");
                animator.SetBool("isScreaming", true);
                break;
            case State.Hit:
                animator.SetBool("isHit", true);
                break;
        }
    }

    public void ChooseDurationAndDirection()
    {
        walkTime = UnityEngine.Random.Range(4, 6);
        waitTime = UnityEngine.Random.Range(3, 4);

        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        transform.localRotation = Quaternion.Euler(0f, randomAngle, 0f);
        moveDirection = transform.forward;  // Update the movement direction
    }

    public void AttackAtExactMoment()
    {
        //this function is called from the animation event
        if (Vector3.Distance(transform.position, player.position) < attackRange + 2)
        {
            PlayerState.Instance.DecreaseHealth(damage);
        }
    }

    private IEnumerator AttackingBehaviour()
    {
        // Perform attack logic

        character.speed = 0; // Stop the zombie from moving
        yield return new WaitForSeconds(attackDuration); // Wait for attack duration

        isAttacking = false; // Reset attacking state
        SetState(State.Running); // Transition back to running
    }

    private IEnumerator Screaming()
    {
        if (soundChannel != null && scream != null)
        {
            soundChannel.PlayOneShot(scream);
        }
        // Play the scream sound
        character.speed = 0; // Stop the zombie from moving

        // Rotate the zombie to face the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
        float rotationSpeed = 5f;

        // Smoothly rotate over time
        float elapsedTime = 0f;
        while (elapsedTime < 1f) // Adjust this duration as needed
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(screamDuration); // Delay for the scream animation duration
        character.speed = runSpeed;
        SetState(State.Running); // Transition to running
    }

    public void IdleBehaviour()
    {
        waitCounter -= Time.deltaTime;
        character.speed = 0;
        if (waitCounter < 0)
        {
            SetState(State.Walking);
            ChooseDurationAndDirection();
        }
    }

    public void WalkingBehaviour()
    {
        walkCounter -= Time.deltaTime;
        if (walkCounter <= 0)
        {
            SetState(State.Idle); // Transition to idle after walking is complete
            ChooseDurationAndDirection();
        }
        else
        {
            // Move in the current direction every frame, ensuring we keep position updates consistent
            Vector3 newPosition = transform.position + (moveDirection * walkSpeed * Time.deltaTime);
            transform.position = newPosition;

            // Ensure the Animator stays in the "Walking" state
            if (currentState != State.Walking)
            {
                SetState(State.Walking); // Keep the state in walking if it’s been changed
            }
        }
    }

    public void RunningBehaviour()
    {
        if (currentState != State.Dead && health > 0)
        {

            character.speed = runSpeed;
            character.SetDestination(player.position);
            //if the enemy loses the player, it will be idle
            if (Vector3.Distance(transform.position, player.position) > distanceToLosePlayer)
            {
                //reset the navmesh agent
                if (GetComponent<NavMeshAgent>().enabled == true)
                {
                    GetComponent<NavMeshAgent>().enabled = false;
                    GetComponent<NavMeshAgent>().enabled = true;
                }

                hasScreamed = false;
                ChooseDurationAndDirection();
                SetState(State.Idle);
            }
        }
        else
        {
            if (health <= 0)
            {
                ActivateDead();
            }
        }
    }

    public void ActivateAttacking()
    {
        if (currentState != State.Dead)
            SetState(State.Attacking);
    }

    public async void ActivateHit()
    {
        if (currentState != State.Dead)
        {

            SetState(State.Hit);

            soundChannel.PlayOneShot(beingHit);
            character.speed = 0;
            await Task.Delay((int)getHitDuration * 1000);
            if (health <= 0)
            {
                ActivateDead();
            }
            else
            {
                SetState(State.Running);
            }
        }
    }

    public void ActivateDead()
    {
        if (isDead) return;
        isDead = true;
        SetState(State.Dead);
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        soundChannel.PlayOneShot(dieSound);
        walkRun.Stop();

        OnEnemyKilled?.Invoke(enemyType.ToString());
        ActivateGravityAndDisableKinematicForStuckArrows();

        ActivateBlood();

        dayOfRevival = TimeManager.Instance.dayInGame + 2;
        RevivalManager.Instance.AddDeadCreature(enemyType.ToString(), dayOfRevival, spawningLocation);
        TimeManager.Instance.OnDayPass.AddListener(DestroyBody);
    }

    public void DestroyBody()
    {
        if (TimeManager.Instance.dayInGame == dayOfRevival)
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


    public async void ActivateBlood()
    {
        await Task.Delay(3000);
        bloodPuddle.SetActive(true);
    }

    public void TakeDamage(float damage)
    {
        bloodSplashParticles.SetActive(false);
        Debug.Log("Hit a " + enemyType + " with " + damage + " damage.");
        bloodSplashParticles.SetActive(true);
        health -= damage;
        StopBloodSplash();
        ActivateHit();

    }

    private async void StopBloodSplash()
    {
        await Task.Delay(3000);
        bloodSplashParticles.SetActive(false);
    }

}
