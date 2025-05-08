using UnityEngine;

public class ShieldSystem : MonoBehaviour
{
    public static ShieldSystem Instance { get; set; }

    private Animator animator;
    public bool isDefending = false;

    
    public AudioSource soundChannel;
    public AudioClip woodShieldHit;
    public AudioClip ironShieldHit;

    void Start()
    {
        // Get the Animator component from the shield
        animator = GetComponent<Animator>();
    }

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

    void Update()
    {
        // Toggle defending state when 'E' is pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            isDefending = !isDefending;  // Toggle the defending state
            animator.SetBool("isDefending", isDefending);  // Update Animator parameter
        }
    }

    public void PlayWoodShieldHitSound()
    {
        soundChannel.PlayOneShot(woodShieldHit);
    }

    public void PlayIronShieldHitSound()
    {
        Debug.Log("Iron shield hit sound");
        soundChannel.PlayOneShot(ironShieldHit);
    }
}
