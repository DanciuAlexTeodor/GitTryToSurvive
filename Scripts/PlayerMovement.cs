using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float speed;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public bool isMoving;
    private bool isRunning; // Track whether the player is running
    private bool isPlayingWalkSound = false; // Track if walk sound is playing
    private bool isPlayingRunSound = false; // Track if run sound is playing

    void Update()
    {
        
        if (DialogSystem.Instance.dialogUIActive == false &&
            StorageManager.Instance.storageUIOpen == false &&
            CampfireUIManager.Instance.isUIOpen == false)
        {
            HandleMovementAndSounds();
        }
    }

    

    private void HandleMovementAndSounds()
    {
        // Determine if the player should be running or walking
        if (Input.GetKey(KeyCode.LeftShift) && PlayerState.Instance.currentEnergy > 10)
        {
            speed = runSpeed;
            isRunning = true;
        }
        else
        {
            speed = walkSpeed;
            isRunning = false;
        }

        // If armor is equipped, reduce speed
        CheckIfArmorOn();
        Movement();

        // Play the appropriate sound based on movement and state
        if (isMoving && isGrounded)
        {
            if (isRunning)
            {
                if (!isPlayingRunSound)
                {
                    SoundManager.Instance.StopSound(SoundManager.Instance.grassWalkSound);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.grassRunSound);
                    isPlayingRunSound = true;
                    isPlayingWalkSound = false;
                }
            }
            else
            {
                if (!isPlayingWalkSound)
                {
                    SoundManager.Instance.StopSound(SoundManager.Instance.grassRunSound);
                    SoundManager.Instance.PlaySound(SoundManager.Instance.grassWalkSound);
                    isPlayingWalkSound = true;
                    isPlayingRunSound = false;
                }
            }
        }
        else
        {
            // Stop all sounds if the player is not moving
            SoundManager.Instance.StopSound(SoundManager.Instance.grassWalkSound);
            SoundManager.Instance.StopSound(SoundManager.Instance.grassRunSound);
            isPlayingWalkSound = false;
            isPlayingRunSound = false;
        }
    }

    public void CheckIfArmorOn()
    {
        List<int> armorSlots = InventorySystem.Instance.CheckArmor();
        for (int i = 0; i < armorSlots.Count; i++)
        {
            if (armorSlots[i] == 1)
            {
                speed -= speed * 0.1f;
            }
        }
    }

    public void Movement()
    {
        // Check if we hit the ground to reset falling velocity
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jump logic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Check if the player is moving
        if (lastPosition != gameObject.transform.position && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;
    }
}
