using System.Collections.Generic;
using UnityEngine;

public class PlaceableItem : MonoBehaviour
{
    // Validation
    public bool toBeSavedForLoading;
    public float multiplier = 0;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isOverlappingItems;
    public bool isValidToBeBuilt;
    public bool isRotationFixed;
    [SerializeField] BoxCollider solidCollider;
    //[SerializeField] MeshCollider meshCollider;
    //some items require a better collider, that's why i have implmeneted the mesh collider
    private Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    void Update()
    {
        

        if (isGrounded && isOverlappingItems == false)
        {
            isValidToBeBuilt = true;
        }
        else
        {
            isValidToBeBuilt = false;
        }

        // Raycast from the box's position towards its center

        float boxHeight = GetBoxHeightNumber();
        RaycastHit groundHit;
        Vector3 colliderBottom = solidCollider.bounds.center - new Vector3(0, solidCollider.bounds.extents.y, 0);
        if (Physics.Raycast(colliderBottom, Vector3.down, out groundHit, boxHeight, LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }

    private float GetBoxHeightNumber()
    {
        float boxHeight = 0;
        
        if (solidCollider != null)
        {
            boxHeight = transform.lossyScale.y;
        }
        
        boxHeight = boxHeight * multiplier;
        return boxHeight;
    }
    

    #region || --- On Triggers --- |
    private void OnTriggerEnter(Collider other)
    {

        if ((other.CompareTag("Ground") || other.CompareTag("placedFoundation")) && PlacementSystem.Instance.inPlacementMode)
        {
            //Debug.Log(gameObject.name);
            // Making sure the item is parallel to the ground
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                // Align the box's rotation with the ground normal
                //Debug.Log(gameObject.name);
                if(gameObject.name!="FurnaceModel" && gameObject.name!="WellModel" && gameObject.name!="GateModel")
                {
                    Quaternion newRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
                    transform.rotation = newRotation;
                }

                //Debug.Log("Rotation fixed");
                isRotationFixed = true;
                isGrounded = true;
            }
        }


        if (other.CompareTag("Tree") || other.CompareTag("pickable") || other.CompareTag("activeConstructable"))
        {
            isOverlappingItems = true;
        }
    }
    #endregion

    private void OnTriggerExit(Collider other)
    {
        if ((other.CompareTag("Ground") || other.CompareTag("placedFoundation") || other.CompareTag("placedWall"))
            && PlacementSystem.Instance.inPlacementMode)
        {
            isGrounded = false;
            isRotationFixed = false;
        }

        if ((other.CompareTag("Tree") || other.CompareTag("pickable") || other.CompareTag("activeConstructable")) && PlacementSystem.Instance.inPlacementMode)
        {
            isOverlappingItems = false;
        }
    }

    #region || --- Set Outline Colors --- |
    public void SetInvalidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.red;
        }

    }

    public void SetValidColor()
    {
        if (outline != null)
        {
            outline.enabled = true;
            outline.OutlineColor = Color.green;
        }
    }

    public void SetDefaultColor()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
    #endregion
}