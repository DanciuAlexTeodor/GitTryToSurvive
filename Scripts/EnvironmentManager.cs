using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TreeData
{
    public string treeName; // Prefab name
    public Vector3 position;
    public Quaternion rotation;
    public int dayOfRegrowth; // Example property

    public TreeData(string name, Vector3 pos, Quaternion rot, int day)
    {
        treeName = name;
        position = pos;
        rotation = rot;
        dayOfRegrowth = day;
    }
}


[System.Serializable]
public class RockData
{
    public string rockName;
    public Vector3 position;
    public Quaternion rotation;
    public float rockHealth;
    public int dayOfRegrowth; //for the mined rocks

    public RockData(string rockName, Vector3 position, Quaternion rotation, float rockHealth, int dayOfRegrowth)
    {
        this.rockName = rockName;
        this.position = position;
        this.rotation = rotation;
        this.rockHealth = rockHealth;
        this.dayOfRegrowth = dayOfRegrowth;
    }
}

public class EnvironmentManager : MonoBehaviour
{
    public static EnvironmentManager Instance { get; set; }

    public GameObject allItems;

    public GameObject animalsParent; // Cached reference to "Animals" parent GameObject
    public GameObject enemiesParent; // Cached reference to "Enemies" parent GameObject
    public GameObject rocksParent; // Cached reference to "Rocks" parent GameObject
    public GameObject treesParent;
    public GameObject placeablesParent;

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

        // Ensure animalsParent is set up at the start
        if (animalsParent == null)
        {
            animalsParent = GameObject.Find("Animals");
        }

        // Ensure enemiesParent is set up at the start
        if (enemiesParent == null)
        {
            enemiesParent = GameObject.Find("Enemies");
        }

        // Ensure rocksParent is set up at the start
        if (rocksParent == null)
        {
            rocksParent = GameObject.Find("[Rocks]");
        }

        if(treesParent == null)
        {
            treesParent = GameObject.Find("[Trees]");
        }

        if(placeablesParent == null)
        {
            placeablesParent = GameObject.Find("[Placeables]");
        }
    }





    public List<AnimalData> getAllAnimals()
    {
        
        if (animalsParent == null)
        {
            Debug.LogWarning("Animals parent object not found.");
            return new List<AnimalData>(); // Return an empty list if animalsParent is missing
        }

        List<AnimalData> animalList = new List<AnimalData>();

        foreach (Transform child in animalsParent.transform)
        {
            Animal animalComponent = child.GetComponent<Animal>();
            if (animalComponent != null && animalComponent.isDead==false)
            {
                // Save the animal's data into AnimalData and add to the list
                AnimalData animalData = new AnimalData(animalComponent.animalName,
                                                       child.position,
                                                       child.rotation,
                                                       animalComponent.health,
                                                       animalComponent.hasBeenFed);
                animalList.Add(animalData);
            }
        }

        return animalList; // Return the list of AnimalData
    }

    public List<EnemyData> getAllEnemies()
    {
        if (enemiesParent == null)
        {
            Debug.LogWarning("Enemies parent object not found.");
            return new List<EnemyData>(); // Return an empty list if enemiesParent is missing
        }

        List<EnemyData> enemyList = new List<EnemyData>();

        foreach (Transform child in enemiesParent.transform)
        {
            if (child.GetComponent<Enemy>() != null)
            {
                Enemy enemyComponent = child.GetComponent<Enemy>();
                if (enemyComponent != null && !enemyComponent.isDead)
                {
                    // Save the enemy's data into EnemyData and add to the list
                    EnemyData enemyData = new EnemyData(
                        enemyComponent.enemyName,
                        child.position,
                        child.rotation,
                        enemyComponent.health
                    );
                    enemyList.Add(enemyData);
                }
            }
        }

        return enemyList; // Return the list of EnemyData
    }


    public List<RockData> getAllRocks()
    {
        if (rocksParent == null)
        {
            Debug.LogWarning("Rocks parent object not found.");
            return new List<RockData>(); // Return an empty list if rocksParent is missing
        }

        List<RockData> rockList = new List<RockData>();

        foreach (Transform child in rocksParent.transform)
        {
            if(child.gameObject.name== "MinedRock")
            {
                RegrowRock regrowRockComponent = child.GetComponent<RegrowRock>();
                if (regrowRockComponent != null)
                {
                    // Save the rock's data into RockData and add to the list
                    RockData rockData = new RockData(
                             regrowRockComponent.gameObject.name,
                             child.position,
                             child.rotation,
                             0,
                             regrowRockComponent.dayOfRegrowth);
                                                                                                                                                              
                    rockList.Add(rockData);
                }
            }
            else if (child.gameObject.name == "Rock")
            {
                MiningRock rockComponent = child.GetChild(0).GetChild(0).GetComponent<MiningRock>();
                
                if (rockComponent != null)
                {
                    // Save the rock's data into RockData and add to the list
                    RockData rockData = new RockData(
                        "Rock",
                        child.position,
                        child.rotation,
                        rockComponent.rockHealth,
                        -1);
                                                                                                                                           
                        rockList.Add(rockData);
                }
                
            }
            
        }

        return rockList; // Return the list of RockData
    }



    public List<TreeData> getAllTrees()
    {
        Transform treesParent = GameObject.Find("[Trees]").transform;
        if (treesParent == null)
        {
            Debug.LogWarning("Trees parent object not found.");
            return new List<TreeData>(); // Return an empty list if treeParent is missing
        }

        List<TreeData> treeList = new List<TreeData>();

        foreach (Transform child in treesParent)
        {
            if (child.gameObject.name == "DeadTree")
            {
                RegrowTree regrowTreeComponent = child.GetComponent<RegrowTree>();
                if (regrowTreeComponent != null)
                {
                    // Save the tree's data into TreeData and add to the list
                    TreeData treeData = new TreeData(
                        regrowTreeComponent.gameObject.name,
                        child.position,
                        child.rotation,
                        regrowTreeComponent.dayOfRegrowth
                    );

                    treeList.Add(treeData);
                }
            }
            else if (child.gameObject.name == "Tree")
            {
                ChoppableTree treeComponent = child.GetChild(0).GetChild(0).GetComponent<ChoppableTree>();

                if (treeComponent != null)
                {
                    // Save the tree's data into TreeData and add to the list
                    TreeData treeData = new TreeData(
                        "Tree",
                        child.position,
                        child.rotation,
                        -1
                    );

                    treeList.Add(treeData);
                }
            }
        }

        return treeList; // Return the list of TreeData
    }


    public List<BuildingPieceData> getAllPlacedItems()
    {
        List<BuildingPieceData> placedItemsData = new List<BuildingPieceData>();

 
        foreach (Transform item in placeablesParent.transform)
        {
            if(item.GetComponent<PlaceableItem>().toBeSavedForLoading == true)
            {
                item.name = item.name.Replace("(Clone)", "");
                placedItemsData.Add(new BuildingPieceData(item.name, item.localPosition, item.localRotation));
            }

            
        }

        return placedItemsData;

    }
}
