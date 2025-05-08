using UnityEngine;

[System.Serializable]
public class SoilPieceData
{
    public bool isEmpty;                // Whether the soil is empty or not
    public string plantName;            // Name of the plant growing in the soil (e.g., "Tomato")
    public string name;                 // Name of the soil object
    public string parentName;           // Name of the parent object (if any)
    public Vector3 position;            // Position of the soil in the game world
    public Quaternion rotation;         // Rotation of the soil in the game world
    public bool isWatered;              // Whether the soil is watered or not
    public PlantPieceData currentPlantData;  // Data representing the plant currently in the soil (if any)

    // Constructor
    public SoilPieceData(Soil soil)
    {
        this.isEmpty = soil.isEmpty;
        this.name = soil.name;
        this.plantName = soil.plantName;
        this.parentName = soil.transform.parent != null ? soil.transform.parent.name : null;
        this.position = soil.transform.position;
        this.rotation = soil.transform.rotation;
        this.isWatered = soil.GetComponent<Renderer>().material == soil.wateredMaterial;

        if (!isEmpty && soil.currentPlant != null)
        {
            this.currentPlantData = new PlantPieceData(soil.currentPlant);

        }
        else
        {
            this.currentPlantData = null;
        }
        
    }
}
