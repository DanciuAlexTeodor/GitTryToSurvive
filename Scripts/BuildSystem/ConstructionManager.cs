using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; set; }

    public GameObject itemToBeConstructed;
    public bool inConstructionMode = false;
    public GameObject constructionHoldingSpot;

    public bool isValidPlacement;

    public bool selectingAGhost;
    public GameObject selectedGhost;

    // Materials we store as refereces for the ghosts
    public Material fullTransparentMaterial;
    public Material ghostSelectedMat;
    public Material ghostSemiTransparentMat;

    // We keep a reference to all ghosts currently in our world,
    // so the manager can monitor them for various operations
    public List<GameObject> allGhostsInExistence = new List<GameObject>();

    public GameObject itemToBeDestroyed;

    public GameObject ConstructionUI;

    public GameObject player;

    public List<BuildingPieceData> itemsBuilded = new List<BuildingPieceData>();

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


    public void ActivateConstructionPlacement(string itemToConstruct)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>(itemToConstruct));

        //change the name of the gameobject so it will not be (clone)
        item.name = itemToConstruct;

        item.transform.SetParent(constructionHoldingSpot.transform, false);
        itemToBeConstructed = item;
        itemToBeConstructed.gameObject.tag = "activeConstructable";

        // Disabling the non-trigger collider so our mouse can cast a ray
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = false;

        // Actiavting Construction mode
        inConstructionMode = true;
    }


    public void GetAllGhosts(GameObject itemToBeConstructed)
    {
        List<GameObject> ghostlist = itemToBeConstructed.gameObject.GetComponent<Constructable>().ghostList;

        foreach (GameObject ghost in ghostlist)
        {
            //Debug.Log(ghost);
            allGhostsInExistence.Add(ghost);
        }
    }

    public void PerformGhostDeletionScan()
    {

        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition == false) // if we did not already add a flag
                {
                    foreach (GameObject ghostX in allGhostsInExistence)
                    {
                        // First we check that it is not the same object
                        if (ghost.gameObject != ghostX.gameObject)
                        {
                            // If its not the same object but they have the same position
                            if (XPositionToAccurateFloat(ghost) == XPositionToAccurateFloat(ghostX) && ZPositionToAccurateFloat(ghost) == ZPositionToAccurateFloat(ghostX))
                            {
                                if (ghost != null && ghostX != null)
                                {
                                    // setting the flag
                                    ghostX.GetComponent<GhostItem>().hasSamePosition = true;
                                    break;
                                }

                            }

                        }

                    }

                }
            }
        }

        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null)
            {
                if (ghost.GetComponent<GhostItem>().hasSamePosition)
                {
                    DestroyImmediate(ghost);
                }
            }

        }
    }

    private float XPositionToAccurateFloat(GameObject ghost)
    {
        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost.gameObject.transform.position;
            float pos = targetPosition.x;
            float xFloat = Mathf.Round(pos * 100f) / 100f;
            return xFloat;
        }
        return 0;
    }

    private float ZPositionToAccurateFloat(GameObject ghost)
    {

        if (ghost != null)
        {
            // Turning the position to a 2 decimal rounded float
            Vector3 targetPosition = ghost.gameObject.transform.position;
            float pos = targetPosition.z;
            float zFloat = Mathf.Round(pos * 100f) / 100f;
            return zFloat;

        }
        return 0;
    }

    private void ModifyFromKeyboard()
    {
       

        if (Input.GetKey(KeyCode.R))
        {
            itemToBeConstructed.transform.Rotate(0, 1f, 0);
        }
    }

    private void Update()
    {
        if (inConstructionMode)
        {
            ConstructionUI.SetActive(true);
        }
        else
        {
            ConstructionUI.SetActive(false);
        }

        if (itemToBeConstructed != null && inConstructionMode)
        {
            ModifyFromKeyboard();
            if (itemToBeConstructed.name == "FoundationModel" 
                || itemToBeConstructed.name=="StairsModel" 
                || itemToBeConstructed.name=="FenceModel")
            {
                if (CheckValidConstructionPosition())
                {
                    isValidPlacement = true;
                    itemToBeConstructed.GetComponent<Constructable>().SetValidColor();
                }
                else
                {
                    isValidPlacement = false;
                    itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
                }
            }

            


            //we are using the camera to get the ghost object
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //selectionTransform is the object that we are pointing at
                var selectionTransform = hit.transform;
                if (selectionTransform.gameObject.CompareTag("ghost")//fundatiile fantoma au acest tag
                    && itemToBeConstructed.name=="FoundationModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else if(selectionTransform.gameObject.CompareTag("wallGhost")//peretii fantoma au acest tag
                    && itemToBeConstructed.name == "WallModel")
                {
                    //Debug.Log("Wall ghost");
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else if (itemToBeConstructed.name == "DoorModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                    
                    
                }
                else if (selectionTransform.gameObject.CompareTag("wallGhost")
                    && itemToBeConstructed.name == "WindowModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }

                else
                {
                    itemToBeConstructed.SetActive(true);
                    selectedGhost = null;
                    selectingAGhost = false;
                }

            }
        }

        // Left Mouse Click to Place item
        if (Input.GetMouseButtonDown(0) && inConstructionMode)
        {
           
            if (isValidPlacement && selectedGhost == false && itemToBeConstructed != null &&
                (itemToBeConstructed.name == "FoundationModel" ||
                itemToBeConstructed.name=="FenceModel" ||
                itemToBeConstructed.name == "StairsModel"))
            {
                
                SoundManager.Instance.PlaySound(SoundManager.Instance.construction);

                
                PlaceItemFreeStyle();
                if (itemToBeDestroyed != null)
                {
                    DestroyItem(itemToBeDestroyed);
                }
                else
                {
                    Debug.LogWarning("itemToBeDestroyed is null");
                }
            }


            if (selectingAGhost && selectedGhost != null)
            {
                Debug.Log("Placed item in ghost position");
                SoundManager.Instance.PlaySound(SoundManager.Instance.construction);
                PlaceItemInGhostPosition(selectedGhost);
                if (itemToBeDestroyed != null)
                {
                    DestroyItem(itemToBeDestroyed);
                }
                else
                {
                    Debug.LogWarning("itemToBeDestroyed is null");
                }
            }
            else if (selectingAGhost && selectedGhost == null)
            {
                Debug.LogWarning("selectedGhost is null");
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && inConstructionMode)
        {     
            itemToBeDestroyed.SetActive(true);
            //Destroy(itemToBeDestroyed);
            itemToBeDestroyed = null;

            //if we have dragged a construction item over a ghost and we want to exit the construction mode
            //we have to change the material renderer back to fullTransparent
            selectedGhost = null;



            DestroyItem(itemToBeConstructed);
            //Debug.Log("Destroy item 11");
            itemToBeConstructed = null;
            inConstructionMode = false;

        }
    }


    public List<BuildingPieceData> CollectAllGhostsInExistence()
    {
        List<BuildingPieceData> allGhostsInExistence = new List<BuildingPieceData>();

        foreach (Transform ghost in GameObject.Find("[Constructables]").transform)
        {
            if(ghost.gameObject.CompareTag("ghost") || ghost.gameObject.CompareTag("wallGhost"))
            {
                if(ghost.gameObject.activeSelf==true)
                {
                    allGhostsInExistence.Add(new BuildingPieceData
                    (
                        ghost.name,                             // _pieceType
                        ghost.transform.position,               // _position
                        ghost.transform.rotation                // _rotation

                    ));

                    //Debug.Log("Ghost name: " + ghost.name + " Ghost position: " + ghost.transform.position + " Ghost rotation: " + ghost.transform.rotation.eulerAngles);
                }


                
            }
        }

        return allGhostsInExistence;
    }



    private void PlaceItemInGhostPosition(GameObject copyOfGhost)
    {

        Vector3 ghostPosition = copyOfGhost.transform.position;
        Quaternion ghostRotation = copyOfGhost.transform.rotation;

        //Debug.Log("Rotation: " + ghostRotation.eulerAngles);

        selectedGhost.gameObject.SetActive(false);

        // Setting the item to be active again (after we disabled it in the ray cast)
        itemToBeConstructed.gameObject.SetActive(true);
        GameObject constructablesFolder = GameObject.Find("[Constructables]");
        itemToBeConstructed.transform.SetParent(constructablesFolder.transform);


        // We add a random offset to the position so the item is not perfectly aligned with the ghost
        //facem asta pentru a opri bug ul cand 2 walls sunt puse unul langa altul
        //the flickering bug
        var randomOffset = UnityEngine.Random.Range(0.01f, 0.03f);


        itemToBeConstructed.transform.position = new Vector3(ghostPosition.x, ghostPosition.y,
            ghostPosition.z + randomOffset);
        itemToBeConstructed.transform.rotation = ghostRotation;

        //Debug.Log("Rotation after seting the parrent to constructable: " + ghostRotation.eulerAngles);

        // Enabling back the solider collider that we disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;
        // Setting the default color/material
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();

        if(itemToBeConstructed.name=="FoundationModel")
        {
            // Making the Ghost Children to no longer be children of this item
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();


            itemToBeConstructed.tag = "placedFoundation";

            //Adding all the ghosts of this item into the manager's ghost bank
            GetAllGhosts(itemToBeConstructed);

            //delete all the unneeded ghosts
            PerformGhostDeletionScan();
        }
        else if(itemToBeConstructed.name=="WallModel" || itemToBeConstructed.name=="WindowModel")
        {
            //we don't have ghost for the wall, therefore we
            //don't need to extract the ghost members
            itemToBeConstructed.tag = "placedWall";
            //we need to destroy it because the Manager won't do it
            DestroyItem(selectedGhost);
        }
        else if(itemToBeConstructed.name=="DoorModel")
        {
            //we don't have ghost for the wall, therefore we
            //don't need to extract the ghost members
            itemToBeConstructed.tag = "placedWall";
            //we need to destroy it because the Manager won't do it
            DestroyItem(selectedGhost);
        }

        /////
        itemsBuilded.Add(new BuildingPieceData
    (
        itemToBeConstructed.name,                             // _pieceType
        itemToBeConstructed.transform.position,               // _position
        itemToBeConstructed.transform.rotation                // _rotation
    ));
        /////////////


        itemToBeConstructed = null;

        inConstructionMode = false;
    }
 
    public void PlaceItemFreeStyle()
    {
        // Setting the parent to be the root of our scene
        GameObject constructablesFolder = GameObject.Find("[Constructables]");
        itemToBeConstructed.transform.SetParent(constructablesFolder.transform);

        // Making the Ghost Children to no longer be children of this item

        // Setting the default color/material
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
        
        itemToBeConstructed.GetComponent<Constructable>().enabled = false;
        // Enabling back the solider collider that we disabled earlier
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;

        itemsBuilded.Add(new BuildingPieceData
        (
            itemToBeConstructed.name,                             // _pieceType
            itemToBeConstructed.transform.position,          // _position
            itemToBeConstructed.transform.rotation          // _rotation 
        ));

        itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();

        //Adding all the ghosts of this item into the manager's ghost bank
        GetAllGhosts(itemToBeConstructed);
        PerformGhostDeletionScan();

        itemToBeConstructed = null;

        inConstructionMode = false;
    }

    void DestroyItem(GameObject item)
    {
        Debug.Log("Constructable");
        DestroyImmediate(item);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeededItems();

    }

    private bool CheckValidConstructionPosition()
    {
        if (itemToBeConstructed != null)
        {
            return itemToBeConstructed.GetComponent<Constructable>().isValidToBeBuilt;
        }

        return false;
    }
}