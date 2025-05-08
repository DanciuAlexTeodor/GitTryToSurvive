using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitPositioner : MonoBehaviour
{
    void Start()
    {
        PositionOnGround();
    }

    void PositionOnGround()
    {
        RaycastHit hit;
        // Cast a ray downward from the rabbit's position
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            // Move the rabbit to the hit point of the ground
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }
        else
        {
            Debug.LogWarning("Rabbit is not above the ground!");
        }
    }
}
