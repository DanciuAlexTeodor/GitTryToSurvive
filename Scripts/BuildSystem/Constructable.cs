using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constructable : MonoBehaviour
{
    // Validation
    public bool isGrounded;
    public bool isOverlappingItems;
    public bool isValidToBeBuilt;
    public bool detectedGhostMemeber;

    // Material related
    private Renderer mRenderer;
    public Material redMaterial;
    public Material greenMaterial;
    public Material defaultMaterial;

    public List<GameObject> ghostList = new List<GameObject>();

    public BoxCollider solidCollider; // We need to drag this collider manualy into the inspector

    private void Start()
    {
        mRenderer = GetComponent<Renderer>();

        mRenderer.material = defaultMaterial;
        AddGhostsToList();
    }


    public void AddGhostsToList()
    {
        foreach (Transform child in transform)
        {
            ghostList.Add(child.gameObject);
        }
    }

    void Update()
    {
        if (isGrounded&& isOverlappingItems == false)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //other.CompareTag("placedFoundation") - be able to place the stairs on the foundation 
        if (other.CompareTag("Ground") || other.CompareTag("placedFoundation") && gameObject.CompareTag("activeConstructable"))
        {
            isGrounded = true;
        }

        //if there are trees or pickable object in the way, it won't be possible to build
        if (other.CompareTag("Tree") || other.CompareTag("pickable") && gameObject.CompareTag("activeConstructable"))
        {

            isOverlappingItems = true;
        }

        //if there is a ghost constructable, it will detect it and return true
        if (other.gameObject.CompareTag("ghost") && gameObject.CompareTag("activeConstructable"))
        {
            detectedGhostMemeber = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Ground") || other.CompareTag("placedFoundation"))  && gameObject.CompareTag("activeConstructable"))
        {
            isGrounded = false;
        }

        if (other.CompareTag("Tree") || other.CompareTag("pickable") && gameObject.CompareTag("activeConstructable"))
        {
            isOverlappingItems = false;
        }

        if (other.gameObject.CompareTag("ghost") && gameObject.CompareTag("activeConstructable"))
        {
            detectedGhostMemeber = false;
        }
    }

    public void DestroyGhosts()
    {
        foreach(Transform child in transform)
        {
            if (child.gameObject.CompareTag("ghost") || child.gameObject.CompareTag("wallGhost"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void SetInvalidColor()
    {
        if (mRenderer != null)
        {
            mRenderer.material = redMaterial;
        }
    }

    public void SetValidColor()
    {
        //Debug.Log("Set the color of the piece to green");
        mRenderer.material = greenMaterial;
    }

    public void SetDefaultColor()
    {
        if (mRenderer != null && defaultMaterial != null)
        {
            mRenderer.material = defaultMaterial;
        }
        else
        {
            Debug.LogWarning("mRenderer or defaultMaterial is null.");
        }
    }

  


    //!!!! de verificat aceasta functie
    public void ExtractGhostMembers()
    {
        //I have to clear the list and add the ghosts again, because rarely the list is not updated
        ghostList.Clear();
        AddGhostsToList();



        Debug.Log("All ghosts members are extracted. Number of ghosts: " + ghostList.Count);
        foreach (GameObject item in ghostList)
        {
            item.transform.SetParent(transform.parent, true);
            item.gameObject.GetComponent<GhostItem>().solidCollider.enabled = false;
            item.gameObject.GetComponent<GhostItem>().isPlaced = true;
        }
    }
}