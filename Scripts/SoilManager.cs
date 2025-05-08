using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoilManager : MonoBehaviour
{
    public static SoilManager Instance { get; private set; }
    public List<SoilPieceData> soilList = new List<SoilPieceData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdateSoilData()
    {
        Soil[] soilsInScene = FindObjectsOfType<Soil>();

        // Iterate through each soil and either update the existing data or add new data to the list
        foreach (Soil soil in soilsInScene)
        {
            // Check if this soil already exists in the soilList
            SoilPieceData existingData = soilList.Find(data => data.position == soil.transform.position);

            if (existingData != null)
            {
                // Update the existing soil data
                existingData.isEmpty = soil.isEmpty;
                existingData.name = soil.name;
                existingData.plantName = soil.plantName;
                existingData.position = soil.transform.position; // Update position
                existingData.rotation = soil.transform.rotation; // Update rotation
                existingData.isWatered = soil.GetComponent<Renderer>().material == soil.wateredMaterial;

                // Handle the plant data in the soil
                if (!soil.isEmpty && soil.currentPlant != null)
                {
                    Debug.Log("Updating plant data for existing soil.");
                    existingData.currentPlantData = new PlantPieceData(soil.currentPlant);
                }
                else
                {
                    existingData.currentPlantData = null;
                }
            }
            else
            {
                // If no existing data is found, create new soil data and add it to the list
                SoilPieceData newData = new SoilPieceData(soil);
                Debug.Log("Adding new soil data: Soil name: " + soil.gameObject.name);
                newData.name = soil.gameObject.name.Replace("(Clone)","");
                newData.plantName = soil.plantName.Replace("(Clone)", ""); // Update plant name without "(Clone)"
                newData.position = soil.transform.position;
                newData.rotation = soil.transform.rotation;
                newData.isEmpty = soil.isEmpty;
                newData.parentName = soil.transform.parent != null ? soil.transform.parent.name : null;

                
                // Handle the plant data in the soil
                if (!soil.isEmpty && soil.currentPlant != null)
                {
                    Debug.Log("Saving plant data for new soil.");
                    newData.currentPlantData = new PlantPieceData(soil.currentPlant);

                    // Set all the plant-related data
                    newData.isWatered = soil.currentPlant.isWatered;
                    newData.currentPlantData.plantAge = soil.currentPlant.plantAge;
                    newData.currentPlantData.isWatered = soil.currentPlant.isWatered;
                    newData.currentPlantData.isOneTimeHarvest = soil.currentPlant.isOneTimeHarvest;
                    newData.currentPlantData.daysRemainingForNewProduce = soil.currentPlant.daysRemainingForNewProduce;
                }
                else
                {
                    newData.currentPlantData = null;
                }

                soilList.Add(newData); // Add new soil data to the list
            }
        }
    }
}
