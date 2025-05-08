using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CampfirePieceData
{
    public Campfire campfire;          // Reference to the campfire GameObject
    public List<string> storedItems;   // Items inside the campfire
    public string campfireName;        // Name of the campfire
    public Vector3 position;           // Position of the campfire
    public Quaternion rotation;        // Rotation of the campfire
    public string parentName;          // If the campfire has a parent
    public CookableInput inputBeingCooked;    // Input being cooked
    public bool isCooking;             // Whether the campfire is cooking
    public float cookingTimer;         // Cooking timer value
    public float cookingTimerAux;      // Auxiliary cooking timer

    // Updated constructor to initialize all required properties
    public CampfirePieceData(Campfire campfire, string campfireName, Vector3 position, Quaternion rotation, string parentName)
    {
        this.campfire = campfire;
        this.storedItems = new List<string>(campfire.items); // Store all items in the campfire
        this.campfireName = campfireName;
        this.position = position;
        this.rotation = rotation;
        this.parentName = parentName;
        this.inputBeingCooked = campfire.inputBeingCooked;
        this.isCooking = campfire.isCooking;
        this.cookingTimer = campfire.cookingTimer;
        this.cookingTimerAux = campfire.cookingTimerAux;
    }
}
