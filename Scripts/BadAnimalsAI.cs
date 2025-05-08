using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading.Tasks;

public class BadAnimalsAI : MonoBehaviour
{
    public Transform target;
    public NavMeshAgent agent;
    private Animator animator;
    public string animalName;
    private bool isAttacking;

    public bool isDead;
    public bool isInAttackZone;
    public int attackZone;

    public float health = 100f;

    [Header("Attack Settings")]
    public float damage;
    public float maxChaseDistance;
    public float minAttackDistance = 1.5f;
    public float maxAttackDistance = 2.5f;

    [Header("Movement")]
    private float currentWanderTime;
    public float wanderWaitTime = 10f;  // Changes direction every 2 seconds
    public bool canMoveWhileAttacking;
    [Space]
    public float walkSpeed = 2f;
    public float runSpeed = 3.5f;
    public float wanderRange = 5f;

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel;
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip dieSound;

    [Header("Particles")]
    [SerializeField] ParticleSystem bloodSplashParticles;
    [SerializeField] GameObject bloodPuddle;



    public bool walk;
    public bool run;


    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentWanderTime = wanderWaitTime;
    }


    private void Update()
    {
        if (isDead) return;

        if (target != null)
        {
            if (Vector3.Distance(target.transform.position, transform.position) <= attackZone)
            {
                isInAttackZone = true;
            }
            else
            {
                isInAttackZone = false;
            }
        }
            

        UpdateAnimations();

        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);

            if (distanceToTarget > maxChaseDistance)
            {
                target = null;
                isAttacking = false;
            }
            else if (distanceToTarget > maxAttackDistance && !isAttacking)
            {
                // Out of attack range but within chase range
                Chase();
            }
            else if (distanceToTarget <= minAttackDistance && !isAttacking)
            {
                // In attack range, start attack
                StartAttack();
            }
        }
        else
        {
            Wander();
        }
    }

    public void UpdateAnimations()
    {
        animator.SetBool("Walk", walk);
        animator.SetBool("Run", run);
    }

    public void Wander()
    {
        // Check if the agent has reached the destination or if enough time has passed
        if (currentWanderTime >= wanderWaitTime || agent.remainingDistance <= agent.stoppingDistance)
        {
            // Pick a new random position to wander to
            Vector3 wanderPos = transform.position;
            wanderPos.x += Random.Range(-wanderRange, wanderRange);
            wanderPos.z += Random.Range(-wanderRange, wanderRange);

            // Reset the timer and set the new destination
            currentWanderTime = 0;

            agent.speed = walkSpeed;
            agent.SetDestination(wanderPos);

            walk = true;
            run = false;
        }
        else
        {
            // If the agent is not stopped and is still moving, update the timer
            if (!agent.isStopped && agent.remainingDistance > agent.stoppingDistance)
            {
                currentWanderTime += Time.deltaTime;

                walk = true;
                run = false;
            }
            else
            {
                // If the agent is stopped or reached its destination, reset movement
                walk = false;
                run = false;
            }
        }
    }

    private int TakeWeaponHitMomentBasedOnName()
    {
        string weaponName = EquipSystem.Instance.GetWeaponName();
        if (weaponName == "Stone_Axe")
        {
            return 2300;
        }
        return 0;
    }

    public async Task TakeDamage(int damage)
    {
        if (isDead) return;

        int weaponHitMoment = TakeWeaponHitMomentBasedOnName();
        SelectionManager.Instance.isAttacking = true;
        await Task.Delay(weaponHitMoment);



        health -= damage;
        bloodSplashParticles.Play();
        PlayHitSound();

        if (health <= 0)
        {
            Die();
        }
        SelectionManager.Instance.isAttacking = false;
    }

    private void Die()
    {
        isDead = true;
        PlayDieSound();
        animator.SetBool("Run", false);
        animator.SetBool("Walk", false);
        animator.SetTrigger("Die");

        bloodPuddle.SetActive(true);
        //Destroy(gameObject, 5f); // Destroy after 5 seconds to give time for animation
    }

    private void PlayHitSound()
    {
        soundChannel.PlayOneShot(hitSound);
    }

    private void PlayDieSound()
    {
        soundChannel.PlayOneShot(dieSound);
    }


    public void Chase()
    {
        agent.SetDestination(target.transform.position);

        walk = false;

        run = true;

        agent.speed = runSpeed;

        if (Vector3.Distance(target.transform.position, transform.position) <= minAttackDistance && !isAttacking)
            StartAttack();
    }

    public void StartAttack()
    {
        isAttacking = true;

        if (!canMoveWhileAttacking)
        {
            agent.SetDestination(transform.position);
        }

        animator.SetTrigger("Attack");
    }

    public void FinishAttack()
    {
        if (Vector3.Distance(target.transform.position, transform.position) > maxAttackDistance)
        {
            // Target moved out of attack range, resume chase
            isAttacking = false;
            Chase();
        }
        else
        {
            // Deal damage if target is still within range
            PlayerState.Instance.DecreaseHealth(damage);

            // Prepare for the next attack
            isAttacking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //cand intru in triggerul lui, ma ataca
        if (other.GetComponent<PlayerMovement>() != null)
            target = other.transform;
    }
}
