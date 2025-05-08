using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plant : MonoBehaviour
{
    [SerializeField] GameObject seedModel;
    [SerializeField] GameObject youngPlantModel;
    [SerializeField] GameObject prematurePlantModel;
    [SerializeField] GameObject maturePlantModel;

    public List<GameObject> plantProduceSpawns;

    public GameObject producePrefab;

    public int dayOfPlanting;
    public int plantAge = 0; //depends on the watering frequency  /+
    [SerializeField] int ageForYoungModel;
    [SerializeField] int ageForPrematureModel;
    public int ageForMatureModel;
    [SerializeField] int ageForFirstProduceBatch;

    public int daysForNewProduce; // Days it takes for new fruit to grow after the initial batch; /+
    public int daysRemainingForNewProduce; 

    public bool isWatered = false; 
    public bool isOneTimeHarvest = false; 


    private void OnEnable()
    {
        TimeManager.Instance.OnDayPass.AddListener(DayPass);
    }

    private void OnDisable()
    {
        TimeManager.Instance.OnDayPass.RemoveListener(DayPass);
    }

    private void OnDestroy()
    {
        //the parent is the dirt pile, and the children is the soil
        GetComponentInParent<Soil>().isEmpty = true;
        GetComponentInParent<Soil>().plantName = "";
        GetComponentInParent<Soil>().currentPlant = null;
    }

    //this will be called each day at midnight 0:00
    private void DayPass()
    {
        
        if(isWatered)
        {
            plantAge++;
            isWatered = false;

            CheckGrowth();

            if (!isOneTimeHarvest)
            {
                CheckProduce();
            }

        }
        GetComponentInParent<Soil>().MakeSoilDefault();
    }

    public void CheckGrowth()
    {
        if(plantAge<ageForYoungModel)
        {
            seedModel.SetActive(true);
        }

        if(plantAge>=ageForYoungModel && plantAge<ageForPrematureModel)
        {
            seedModel.SetActive(false);
            youngPlantModel.SetActive(true);
        }

        if(plantAge>=ageForPrematureModel && plantAge<ageForMatureModel)
        {
            youngPlantModel.SetActive(false);
            prematurePlantModel.SetActive(true);
        }

        if(plantAge>=ageForMatureModel)
        {
            prematurePlantModel.SetActive(false); 
            maturePlantModel.SetActive(true);
            
        }

        if(plantAge>=ageForMatureModel && isOneTimeHarvest)
        {
            MakePlantPickable();
        }

    }

    private void MakePlantPickable()
    {
        GetComponent<InteractableObject>().enabled = true;
    }

    

    public void CheckProduce()
    {
        if (plantAge == ageForFirstProduceBatch)
        {
            GenerateProduceForEmptySpawns();
        }

        if(plantAge >= ageForFirstProduceBatch)
        {
            if(daysRemainingForNewProduce>0)
            {
                daysRemainingForNewProduce--;
            }
            else
            {
                GenerateProduceForEmptySpawns();
                daysRemainingForNewProduce = daysForNewProduce;
            }
        }
    }

    private void GenerateProduceForEmptySpawns()
    {
        foreach (GameObject spawn in plantProduceSpawns)
        {
            if(spawn.transform.childCount == 0)
            {
                GameObject produce = Instantiate(producePrefab, spawn.transform.position, Quaternion.identity);
                produce.transform.SetParent(spawn.transform);
            }
        }
    }
}
