using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Soil : MonoBehaviour
{
      
    public bool isEmpty = true;
    public bool playerInRange;
    public string plantName;
    public string name;
    public string parentName;
    public bool isWatered;

    public Plant currentPlant;

    public Material defaultMaterial;
    public Material wateredMaterial;
    public Material currentMaterial;

    private void Update()
    {
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        if (distance <= 6f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }

    internal void PlantSeed()
    {
        InventoryItem selectedSeed = EquipSystem.Instance.selectedItem.GetComponent<InventoryItem>();
        isEmpty = false;

        string onlyPlantName = selectedSeed.thisName.Split(new string[] { " Seed" }, StringSplitOptions.None)[0];
        plantName = onlyPlantName;

        // Instantiate Plant Prefab
        GameObject instantiatedPlant = Instantiate(Resources.Load($"{onlyPlantName}Plant") as GameObject);

        // Set the instantiated plant to be a child of the soil
        instantiatedPlant.transform.parent = gameObject.transform;

        // Make the plant's position in the middle of the soil
        Vector3 plantPosition = Vector3.zero;
        plantPosition.y = 0f;
        instantiatedPlant.transform.localPosition = plantPosition;

        currentPlant = instantiatedPlant.GetComponent<Plant>();

        if (currentPlant == null)
        {
            Debug.LogError("Failed to find Plant component on instantiated plant object");
        }
    }

    public void MakeSoilWatered()
    {
        GetComponent<Renderer>().material = wateredMaterial;
        currentMaterial = wateredMaterial;
        isWatered = true;
        if(currentPlant != null)
        {
            currentPlant.isWatered = true;
        }
    }

    internal void MakeSoilDefault()
    {
        GetComponent<Renderer>().material = defaultMaterial;
        currentMaterial = defaultMaterial;
        isWatered = false;
    }
}
