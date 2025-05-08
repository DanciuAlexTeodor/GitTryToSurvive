using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FurnacePieceData
{
    public Furnace furnace;          // Reference to the campfire GameObject
    public List<string> storedItems;   // Items inside the campfire
    public string furnaceName;        // Name of the campfire
    public Vector3 position;           // Position of the campfire
    public Quaternion rotation;        // Rotation of the campfire
    public string parentName;          // If the campfire has a parent
    public CookableInput inputBeingCooked1, inputBeingCooked2, inputBeingCooked3;    // Input being cooked
    public bool isCooking;             // Whether the campfire is cooking
    public float cookingTimer;         // Cooking timer value
    public float cookingTimerAux;      // Auxiliary cooking timer

    // Updated constructor to initialize all required properties
    public FurnacePieceData(Furnace furnace, string furnaceName, Vector3 position, Quaternion rotation, string parentName)
    {
        this.furnace = furnace;
        this.storedItems = new List<string>(furnace.items); // Store all items in the campfire
        this.furnaceName = furnaceName;
        this.position = position;
        this.rotation = rotation;
        this.parentName = parentName;
        this.inputBeingCooked1 = furnace.inputBeingCooked1;
        this.inputBeingCooked2 = furnace.inputBeingCooked2;
        this.inputBeingCooked3 = furnace.inputBeingCooked3;

        this.isCooking = furnace.isCooking;
        this.cookingTimer = furnace.cookingTimer;
        this.cookingTimerAux = furnace.cookingTimerAux;
    }
}
