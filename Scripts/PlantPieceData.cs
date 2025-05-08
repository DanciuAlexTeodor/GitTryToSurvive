using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantPieceData
{
    public string plantType;                  // The type of plant (e.g., Tomato, Carrot)
    //public Vector3 position;                  // The position of the plant in the game world
    //public Quaternion rotation;               // The rotation of the plant
    public int plantAge;                      // The current age of the plant, affecting its growth stage
    public bool isWatered;                    // Whether the plant is watered or not
    public bool isOneTimeHarvest;             // Whether the plant can be harvested only once
    public int daysRemainingForNewProduce;    // Days remaining for new produce to grow after the initial harvest
    public List<bool> produceSpawnStates;

    // Constructor to initialize a PlantPieceData from a Plant object
    public PlantPieceData(Plant plant)
    {
        this.plantType = plant.gameObject.name.Replace("(Clone)", "");  // Save the plant type without "(Clone)"
        this.plantAge = plant.plantAge;
        this.isWatered = plant.isWatered;
        this.isOneTimeHarvest = plant.isOneTimeHarvest;
        this.daysRemainingForNewProduce = plant.daysRemainingForNewProduce;

        this.produceSpawnStates = new List<bool>();
        foreach (GameObject spawn in plant.plantProduceSpawns)
        {
            bool hasProduce = spawn.transform.childCount > 0;
            produceSpawnStates.Add(hasProduce);
        }
    }
}
