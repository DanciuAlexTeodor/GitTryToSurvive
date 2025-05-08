using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Bow : MonoBehaviour
{
    public static Bow Instance { get; set; }

    public bool isArrowLoaded;
    public GameObject arrow;


    //functia de awake este apelata mereu cand dau equip la arc
    public void Awake()
    {
        isArrowLoaded = false;
        arrow.SetActive(false);
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void InstantiateArrow()
    {
        isArrowLoaded = true;
        EquipSystem.Instance.isArrowLoadedOnBow = true;
        arrow.SetActive(true);
    }

    public void LaunchArrow()
    {
        if (isArrowLoaded)
        {
            ShootArrow();
            isArrowLoaded = false;
            EquipSystem.Instance.isArrowLoadedOnBow = false;
        }
    }

    public async void ShootArrow()
    {
        await Task.Delay(1000);
        arrow.SetActive(false);

       

        // Load the prefab for the shot arrow
        GameObject ShotedArrowPrefab = Resources.Load<GameObject>("ShotedArrow_Model");

        if (ShotedArrowPrefab != null)
        {
            // Find the parent object where the shot arrows should be instantiated
            GameObject ShotedArrowParent = GameObject.Find("[ShotedArrows]");

            // Instantiate the shot arrow at the current arrow's position and rotation
            GameObject shotedArrow = Instantiate(ShotedArrowPrefab, arrow.transform.position, arrow.transform.rotation);

            // Set the parent of the instantiated shot arrow
            shotedArrow.transform.SetParent(ShotedArrowParent.transform, true);

            // Optionally: Apply the prefab's original local position, rotation, and scale if necessary
            // (Not needed if you want to maintain the current arrow's position/rotation as it is)
            shotedArrow.transform.localPosition = ShotedArrowPrefab.transform.localPosition;
            shotedArrow.transform.localRotation = ShotedArrowPrefab.transform.localRotation;
            shotedArrow.transform.localScale = ShotedArrowPrefab.transform.localScale;

            

            //Debug.Log("Successfully instantiated shot arrow: " + shotedArrow.name);

            //Decrease the weapon health of the bow and arrow (arrow can be shoted 4 times, bow can shoot 30 times)
            //arrow.GetComponent<Weapon>().TakeDamage(5);
            EquipSystem.Instance.decreaseWeaponHealth(5);

            CalculateDistanceForArrow(shotedArrow);
        }
        else
        {
            Debug.LogError("ShotedArrow_Model could not be loaded from Resources.");
        }
        
    }

    public async void CalculateDistanceForArrow(GameObject shotedArrow)
    {
        // Get the Rigidbody of the shot arrow
        Rigidbody arrowRb = shotedArrow.GetComponent<Rigidbody>();

        if (arrowRb == null)
        {
            arrowRb = shotedArrow.AddComponent<Rigidbody>(); // Add Rigidbody if not present
        }

        // Temporarily set the arrow to be kinematic to prevent unwanted collisions
        arrowRb.isKinematic = true;
        arrowRb.useGravity = false; // Temporarily disable gravity

        // Initialize the ArrowCollisionHandler
        ArrowCollisionHandler collisionHandler = shotedArrow.AddComponent<ArrowCollisionHandler>();
        collisionHandler.Initialize(arrowRb); // Call Initialize and pass the Rigidbody

        // Wait briefly to ensure initialization completes before enabling physics
        await Task.Delay(100); // Short delay (100 milliseconds)

        // Now enable physics interactions
        arrowRb.isKinematic = false; // Enable physics
        arrowRb.useGravity = true; // Enable gravity

        // Apply force to move the arrow forward
        float shootingForce = 60f; // Adjust this value to control the speed
        arrowRb.AddForce(shotedArrow.transform.up * shootingForce, ForceMode.Impulse);

        // Remove the parent so the arrow flies independently
        shotedArrow.transform.SetParent(null);
    }







}
