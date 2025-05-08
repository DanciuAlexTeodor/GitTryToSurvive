using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CookingData", menuName = "ScriptableObjects/CookingData")]
public class CookingData : ScriptableObject
{
    public List<string> validFuels = new List<string>();  // Corrected syntax
    public List<CookableInput> validInputs = new List<CookableInput>();  // Corrected syntax
}

[System.Serializable]
public class CookableInput
{
    public string name;
    public float timeToCook;
    public string cookedInputName;
}
