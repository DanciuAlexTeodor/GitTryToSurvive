using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollisionHandler : MonoBehaviour
{
    private Rigidbody arrowRb;
    private bool hasHit = false;

    public void Initialize(Rigidbody rb)
    {
        arrowRb = rb;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasHit) return;

        if (arrowRb == null)
        {
            Debug.LogError("Rigidbody is not assigned to the arrow! Make sure Initialize() is called.");
            return;
        }

        hasHit = true;
        // Stop the arrow on collision
        //arrowRb.velocity = Vector3.zero;
        //arrowRb.angularVelocity = Vector3.zero;
        arrowRb.useGravity = false; // Disable gravity
        arrowRb.isKinematic = true; // Stop further physics interactions

        // Check if the collision was with an animal (tagged as "Animal")
        GameObject hitObject = collision.gameObject;
        if (hitObject.CompareTag("Animal"))
        {
            hitObject.GetComponent<Animal>().TakeDamage(45);
            hitObject.GetComponent<Animal>().calmDownDistance = 100f;
            // Stick the arrow to the animal
            transform.SetParent(collision.transform);

            //Debug.Log("Arrow hit an animal and is now sticking to it.");
        }
        else if(hitObject.CompareTag("Enemy"))
        {
            hitObject.GetComponent<Enemy>().TakeDamage(30f);
            hitObject.GetComponent<Enemy>().followRange = hitObject.GetComponent<Enemy>().GetDistanceFromPlayerToEnemy()+20f;
            hitObject.GetComponent<Enemy>().distanceToLosePlayer = hitObject.GetComponent<Enemy>().GetDistanceFromPlayerToEnemy() + 50f;
            // Stick the arrow to the enemy
            transform.SetParent(collision.transform);


            //Debug.Log("Arrow hit an enemy and is now sticking to it.");
        }
        else
        {
            // If it’s not an animal, just freeze the arrow
            Debug.Log("Arrow hit a non-animal surface and stopped.");
        }
    }
}
