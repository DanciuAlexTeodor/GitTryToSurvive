using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; set; }

    #region || --------- Variables --------- ||
    //Json Project Save Path
    string jsonPathProject;
    //Json External/Real Save Path
    string jsonPathPersistant;
    //Binary Save Path
    string binaryPath;
    string fileName = "SaveGame";
    public bool isSavingToJson;
    public bool isLoading;

    public Canvas loadingScreen;

    #endregion

    private void Start()
    {
        jsonPathProject = Application.dataPath + Path.AltDirectorySeparatorChar;
        jsonPathPersistant = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
        binaryPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            //Debug.LogError("Another instance of SaveManager exists! Destroying this one.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //Debug.Log("SaveManager instance created.");
        }
        DontDestroyOnLoad(gameObject);
    }

    #region || --------- General Section --------- ||

    #region || --------- Saving --------- ||

    public void SaveGame(int slotNumber)
    {
        AllGameData data = new AllGameData();

        data.playerData = GetPlayerData();
        data.environmentData = GetEnvironmentData();
        data.questData = GetQuestData();


        StorageBoxData storageBoxData = GetStorageBoxData();

        if (storageBoxData != null && storageBoxData.storageBoxList.Count > 0)
        {
            data.storageBoxData = storageBoxData;
        }

        /*CampfireData campfireData = GetCampfireData();
        if (campfireData != null && campfireData.campfireList.Count > 0)
        {
            data.campfireData = campfireData; // Store campfire data
        }*/


        SavingTypeSwitch(data, slotNumber);
    }

    private PlayerData GetPlayerData()
    {
        float[] playerStats = new float[4];
        playerStats[0] = PlayerState.Instance.currentHealth;
        playerStats[1] = PlayerState.Instance.currentCalories;
        playerStats[2] = PlayerState.Instance.currentHydration;
        playerStats[3] = PlayerState.Instance.currentBodyTemperature;

        float[] playerPosAndRot = new float[6];
        playerPosAndRot[0] = PlayerState.Instance.playerBody.transform.position.x;
        playerPosAndRot[1] = PlayerState.Instance.playerBody.transform.position.y;
        playerPosAndRot[2] = PlayerState.Instance.playerBody.transform.position.z;

        playerPosAndRot[3] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.x;
        playerPosAndRot[4] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.y;
        playerPosAndRot[5] = PlayerState.Instance.playerBody.transform.rotation.eulerAngles.z;

        //aici stochez in variabila auxiliara inventory: continutul de iteme din inventar
        List<string> inventory = InventorySystem.Instance.itemList;
        List<float> inventoryHealth = InventorySystem.Instance.itemHealthList;
        string[] quickSlots = GetQuickSlotsContent();
        List<float>quickSlotsHealth = GetQuickSlotsHealthList();

        return new PlayerData(playerStats, playerPosAndRot, inventory, inventoryHealth, quickSlots, quickSlotsHealth);
    }

    private void CleanDeadBodies()
    {
        //iterate through all the dead bodies and remove them from the scene
        Transform animals = EnvironmentManager.Instance.animalsParent.transform;
        foreach(Transform child in animals)
        {          
            if(child.GetComponent<Animal>() != null && child.GetComponent<Animal>().isDead == true)
            {
                Destroy(child.gameObject);
            }
        }

        Transform enemies = GameObject.Find("Enemies").transform;
        foreach (Transform child in enemies)
        {
            if (child.GetComponent<Enemy>() != null && child.GetComponent<Enemy>().isDead == true)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private EnvironmentData GetEnvironmentData()
    {
        TimeData timeData = TimeManager.Instance.GetTimeData();
        CampfireUIManager.Instance.UpdateCampfireListData();
        List<CampfirePieceData> campfireList = CampfireUIManager.Instance.campfireList;

        FurnaceUIManager.Instance.UpdateFurnaceListData();
        List<FurnacePieceData> furnaceList = FurnaceUIManager.Instance.furnaceList;

        List<string> itemsPickedUpFromTheGround = InventorySystem.Instance.itemsPickedUp; 

        List<BuildingPieceData> itemsPlaced = EnvironmentManager.Instance.getAllPlacedItems();
        List<BuildingPieceData> itemsBuilded = ConstructionManager.Instance.itemsBuilded;
        List<BuildingPieceData> ghostsItemsBuilded = ConstructionManager.Instance.CollectAllGhostsInExistence();


        List<AnimalData> animals = EnvironmentManager.Instance.getAllAnimals();
        List<EnemyData> enemies = EnvironmentManager.Instance.getAllEnemies();
        List<DeadCreature> deadCreatures = RevivalManager.Instance.deadCreatures;
        List<RockData> rocks = EnvironmentManager.Instance.getAllRocks();
        List<TreeData> trees = EnvironmentManager.Instance.getAllTrees();
        
        CleanDeadBodies();

        List<SoilPieceData> soils = new List<SoilPieceData>();
        if (SoilManager.Instance != null)
        {
            SoilManager.Instance.UpdateSoilData();
            soils = SoilManager.Instance.soilList;
        }
        else
        {
            Debug.LogError("SoilManager.Instance is null!");
        }
        

        return new EnvironmentData(timeData,campfireList, furnaceList, itemsPickedUpFromTheGround, itemsPlaced, itemsBuilded, ghostsItemsBuilded,
            animals, enemies, deadCreatures, soils, rocks, trees);
    }

    private QuestData GetQuestData()
    {
        List<Quest> allCompletedQuestsContent = QuestManager.Instance.allCompletedQuests;
        bool firstTimeInteractionWithNPC = NPC.Instance.firstTimeInteraction;

        int questIndex = NPC.Instance.activeQuestIndex;
        int coinAmount = InventorySystem.Instance.CheckCoins();

        return new QuestData(allCompletedQuestsContent,
             firstTimeInteractionWithNPC,questIndex, coinAmount);
    }

    private StorageBoxData GetStorageBoxData()
    {
        List<StorageBoxPieceData> storageBoxList = StorageManager.Instance.storageBoxList;

        //display the items in the storage box, the position and the rotation

        return new StorageBoxData(storageBoxList);
    }

    private List<float> GetQuickSlotsHealthList()
    {
        List<float> healthList = new List<float>();
        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                Weapon weapon = slot.transform.GetChild(0).GetComponent<Weapon>();
                if (weapon != null)
                {
                    healthList.Add(weapon.weaponHealth);
                }
                else
                {
                    healthList.Add(0);
                }
            }
        }
        return healthList;
    }


    private string[] GetQuickSlotsContent()
    {
        List<string> temp = new List<string>();

        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                string name = slot.transform.GetChild(0).name;
                string str2 = "(Clone)";
                string cleanName = name.Replace(str2, "");
                temp.Add(cleanName);
            }
        }
        return temp.ToArray();
    }

    public void SavingTypeSwitch(AllGameData gameData, int slotNumber)
    {
        if (isSavingToJson)
        {
            SaveGameDataToJsonFile(gameData, slotNumber);
        }
        else
        {
            SaveGameDataToBinaryFile(gameData, slotNumber);
        }
    }

    #endregion

    #region || --------- Loading --------- ||

    public AllGameData LoadingTypeSwitch(int slotNumber)
    {
        if (isSavingToJson)
        {
            AllGameData gameData = LoadGameDataFromJsonFile(slotNumber);
            return gameData;
        }
        else
        {   //binary file
            AllGameData gameData = LoadGameDataFromBinaryFile(slotNumber);
            return gameData;
        }
    }

    public void LoadGame(int slotNumber)
    {
        //Player Data
        SetPlayerData(LoadingTypeSwitch(slotNumber).playerData);

        //Environment Data
        SetEnvironmentData(LoadingTypeSwitch(slotNumber).environmentData);

        SetQuestData(LoadingTypeSwitch(slotNumber).questData);

        SetStorageBoxData(LoadingTypeSwitch(slotNumber).storageBoxData);

       
       //SetCampfireData(LoadingTypeSwitch(slotNumber).campfireData);
        

        isLoading = false;

        DisableLoadingScreen();
    }

    private void SetPlayerData(PlayerData playerData)
    {
        //Setting Player Stats

        PlayerState.Instance.currentHealth = playerData.playerStats[0];
        PlayerState.Instance.currentCalories = playerData.playerStats[1];
        PlayerState.Instance.currentHydration = playerData.playerStats[2];
        PlayerState.Instance.currentBodyTemperature = playerData.playerStats[3];

        //Setting Player Position

        Vector3 loadedPosition;
        loadedPosition.x = playerData.playerPositionAndRotation[0];
        loadedPosition.y = playerData.playerPositionAndRotation[1];
        loadedPosition.z = playerData.playerPositionAndRotation[2];

        PlayerState.Instance.playerBody.transform.position = loadedPosition;

        //Setting Player Rotation

        Vector3 loadedRotation;
        loadedRotation.x = playerData.playerPositionAndRotation[3];
        loadedRotation.y = playerData.playerPositionAndRotation[4];
        loadedRotation.z = playerData.playerPositionAndRotation[5];

        PlayerState.Instance.playerBody.transform.rotation = Quaternion.Euler(loadedRotation);

        //Setting Inventory
        InventorySystem.Instance.AddSavedData(playerData.inventoryContent, playerData.itemHealthList);

        //Setting Quick Slots
        int i = 0;
        foreach (string item in playerData.quickSlotsContent)
        {
            GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();

            var itemToAdd = Instantiate(Resources.Load<GameObject>(item));

            if(itemToAdd.GetComponent<Weapon>() != null)
            {
                Weapon weapon = itemToAdd.GetComponent<Weapon>();
                if (weapon != null)
                {
                    weapon.weaponHealth = playerData.quickSlotsHealth[i];
                    if(weapon.slider != null)
                    {
                        weapon.slider.value = weapon.weaponHealth / weapon.maximumWeaponHealth;
                    }
                  
                    weapon.TakeDamage(0);
                }
                
            }
            itemToAdd.transform.SetParent(availableSlot.transform, false);
            i++;
        }

        isLoading = false;
    }

    private void SetEnvironmentData(EnvironmentData environmentData)
    {
        // Step 0: Set the time of day
        SetTimeOfDay(environmentData.timeData);

        // Step 1: Place the campfires and restore their stored items
        RestoreCampfires(environmentData.campfireList);

        RestoreFurnaces(environmentData.furnaceList);

        // Step 2: Remove picked-up items from the environment
        RemovePickedUpItems(environmentData.itemsPickedUpFromTheGround);

        // Step 3: Place saved animals in the environment
        RestoreAnimals(environmentData.animals);

        RestoreEnemies(environmentData.enemies);

        RestoreDeadCreatures(environmentData.deadCreatures);

        RestorePlaceables(environmentData.placeables);

        // Step 4: Place saved building pieces in the environment
        RestoreBuildingPieces(environmentData.itemsBuilded, environmentData.ghostItemsBuilded);

        //Step 5: Place saved plantables in the environment
        RestoreSoils(environmentData.soils);

        // Step 6: Place saved rocks in the environment
        RestoreRocks(environmentData.rocks);

        // Step 7: Place saved trees in the environment
        RestoreTrees(environmentData.trees);

        // Perform a ghost deletion scan to clean up any unnecessary ghosts
        ConstructionManager.Instance.PerformGhostDeletionScan();
    }

    // Step 0: Set the time of day
    private void SetTimeOfDay(TimeData timeData)
    {
        TimeManager.Instance.LoadTimeData(timeData);
    }

    // Step 1: Restore Campfires
    private void RestoreCampfires(List<CampfirePieceData> campfireList)
    {
        foreach (var campfirePiece in campfireList)
        {
            if (campfirePiece.campfireName.Contains("Clone"))
            {
                campfirePiece.campfireName = campfirePiece.campfireName.Replace("(Clone)", "");
            }

            GameObject newCampfire = Instantiate(Resources.Load<GameObject>(campfirePiece.campfireName), campfirePiece.position, campfirePiece.rotation);

            if (!string.IsNullOrEmpty(campfirePiece.parentName))
            {
                Transform parentTransform = GameObject.Find(campfirePiece.parentName)?.transform;
                if (parentTransform != null)
                {
                    newCampfire.transform.SetParent(parentTransform);
                }
            }
            newCampfire.name = campfirePiece.campfireName;
            newCampfire.GetComponent<Outline>().enabled = false;

            Campfire campfireComponent = newCampfire.GetComponent<Campfire>();
            if (campfireComponent != null)
            {
                RestoreCampfireProperties(campfireComponent, campfirePiece);
            }
        }
    }

    private void RestoreCampfireProperties(Campfire campfireComponent, CampfirePieceData campfirePiece)
    {
        campfireComponent.inputBeingCooked = campfirePiece.inputBeingCooked;
        campfireComponent.items = new List<string>(campfirePiece.storedItems);
        campfireComponent.isCooking = campfirePiece.isCooking;
        campfireComponent.cookingTimer = campfirePiece.cookingTimer;
        campfireComponent.cookingTimerAux = campfirePiece.cookingTimerAux;

        if (campfireComponent.items != null && campfireComponent.items.Count > 0)
        {
            foreach (string itemName in campfireComponent.items)
            {
                GameObject itemPrefab = Resources.Load<GameObject>(itemName);
                if (itemPrefab != null)
                {
                    GameObject instantiatedItem = Instantiate(itemPrefab, campfireComponent.transform.position, Quaternion.identity);
                    instantiatedItem.transform.SetParent(campfireComponent.transform);

                    CampfireUIManager.Instance.LoadCampfireItems(campfireComponent);
                }
                else
                {
                    Debug.LogWarning("Item prefab could not be found for: " + itemName);
                }
            }
        }

        campfireComponent.fire.SetActive(campfireComponent.isCooking);
        
    }

    private void RestoreFurnaces(List<FurnacePieceData> furnaceList)
    {
        foreach (var furnacePiece in furnaceList)
        {
            // Înlăturăm sufixul "(Clone)" din numele furnalului dacă este necesar
            if (furnacePiece.furnaceName.Contains("Clone"))
            {
                furnacePiece.furnaceName = furnacePiece.furnaceName.Replace("(Clone)", "");
            }

            // Instanțiem un nou furnace cu datele stocate (poziție, rotație, nume)
            GameObject newFurnace = Instantiate(Resources.Load<GameObject>(furnacePiece.furnaceName), furnacePiece.position, furnacePiece.rotation);

            // Setăm părintele, dacă există
            if (!string.IsNullOrEmpty(furnacePiece.parentName))
            {
                Transform parentTransform = GameObject.Find(furnacePiece.parentName)?.transform;
                if (parentTransform != null)
                {
                    newFurnace.transform.SetParent(parentTransform);
                }
            }

            // Restabilim proprietățile pentru furnace folosind `FurnacePieceData`
            Furnace furnaceComponent = newFurnace.GetComponent<Furnace>();

            newFurnace.name = furnacePiece.furnaceName;
            newFurnace.GetComponent<Outline>().enabled = false;
            if (furnaceComponent != null)
            {
                RestoreFurnaceProperties(furnaceComponent, furnacePiece);
            }
        }
    }

    private void RestoreFurnaceProperties(Furnace furnaceComponent, FurnacePieceData furnacePiece)
    {
        // Setăm valorile input-urilor în proces de gătit
        furnaceComponent.inputBeingCooked1 = furnacePiece.inputBeingCooked1;
        furnaceComponent.inputBeingCooked2 = furnacePiece.inputBeingCooked2;
        furnaceComponent.inputBeingCooked3 = furnacePiece.inputBeingCooked3;

        // Setăm lista de obiecte stocate în furnace
        furnaceComponent.items = new List<string>(furnacePiece.storedItems);

        // Setăm starea de gătire și timerele
        furnaceComponent.isCooking = furnacePiece.isCooking;
        furnaceComponent.cookingTimer = furnacePiece.cookingTimer;
        furnaceComponent.cookingTimerAux = furnacePiece.cookingTimerAux;

        // Dacă există obiecte stocate, le adăugăm în furnace
        if (furnaceComponent.items != null && furnaceComponent.items.Count > 0)
        {
            foreach (string itemName in furnaceComponent.items)
            {
                GameObject itemPrefab = Resources.Load<GameObject>(itemName);
                if (itemPrefab != null)
                {
                    GameObject instantiatedItem = Instantiate(itemPrefab, furnaceComponent.transform.position, Quaternion.identity);
                    instantiatedItem.transform.SetParent(furnaceComponent.transform);

                    FurnaceUIManager.Instance.LoadFurnaceItems(furnaceComponent);
                }
                else
                {
                    Debug.LogWarning("Item prefab could not be found for: " + itemName);
                }
            }
        }

        // Setăm starea focului
        furnaceComponent.fire.SetActive(furnaceComponent.isCooking);

       
    }


    // Step 2: Remove picked-up items from the environment
    private void RemovePickedUpItems(List<string> itemsPickedUp)
    {
        foreach (Transform itemType in EnvironmentManager.Instance.allItems.transform)
        {
            foreach (Transform item in itemType)
            {
                if (itemsPickedUp.Contains(item.name))
                {
                    //Debug.Log("Destroy item 9");
                    Destroy(item.gameObject);
                }
            }
        }

        InventorySystem.Instance.itemsPickedUp = itemsPickedUp;
    }

    // Step 3: Restore animals in the environment
    private void RestoreAnimals(List<AnimalData> animals)
    {
        if (EnvironmentManager.Instance.animalsParent != null)
        {
            // Clear existing animals in the scene
            foreach (Transform child in EnvironmentManager.Instance.animalsParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        

        foreach (var animalData in animals)
        {

            if (animalData.animalName.Contains("Clone"))
            {
                animalData.animalName = animalData.animalName.Replace("(Clone)", "");
            }

            // Instantiate the animal using the saved data

            GameObject animalPrefab = Resources.Load<GameObject>("Animals/" + animalData.animalName);
            if(animalPrefab == null)
            {
                Debug.LogError("Animal prefab not found for: " + animalData.animalName);
                continue;
            }

            GameObject newAnimal = Instantiate(
                animalPrefab,
                animalData.position,
                animalData.rotation
            );

            

            newAnimal.name = animalData.animalName;

            if (EnvironmentManager.Instance.animalsParent != null)
            {
                newAnimal.transform.SetParent(EnvironmentManager.Instance.animalsParent.transform, true);
            }

            Animal animalComponent = newAnimal.GetComponent<Animal>();
            if (animalComponent != null)
            {
                animalComponent.health = animalData.health;
                animalComponent.hasBeenFed = animalData.hasBeenFed;

               
            }
        }
    }

    private void RestoreEnemies(List<EnemyData> enemies)
    {
        if (EnvironmentManager.Instance.enemiesParent != null)
        {
            // Clear existing enemies in the scene
            foreach (Transform child in EnvironmentManager.Instance.enemiesParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (var enemyData in enemies)
        {
            if (enemyData.enemyName.Contains("Clone"))
            {
                enemyData.enemyName = enemyData.enemyName.Replace("(Clone)", "");
            }

            // Instantiate the enemy using the saved data
            GameObject newEnemy = Instantiate(
                Resources.Load<GameObject>("Enemy/" + enemyData.enemyName),
                enemyData.position,
                enemyData.rotation
            );
            newEnemy.name = enemyData.enemyName;

            if (EnvironmentManager.Instance.enemiesParent != null)
            {
                newEnemy.transform.SetParent(EnvironmentManager.Instance.enemiesParent.transform, true);
            }

            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.health = enemyData.health;
            }
        }
    }


    private void RestoreDeadCreatures(List<DeadCreature> deadCreatures)
    {
        foreach (var deadCreature in deadCreatures)
        {
            RevivalManager.Instance.deadCreatures.Add(deadCreature);
        }
    }


    private void RestorePlaceables(List<BuildingPieceData> placeables)
    {
        foreach (var buildingPiece in placeables)
        {
            buildingPiece.pieceType = buildingPiece.pieceType.Replace("(Clone)", "");
            GameObject prefabToInstantiate = Resources.Load<GameObject>(buildingPiece.pieceType);
            if(prefabToInstantiate!=null)
            {
                GameObject item = Instantiate(prefabToInstantiate, buildingPiece.position, buildingPiece.rotation);
                item.name = buildingPiece.pieceType;


                Transform placeableParent = GameObject.Find("[Placeables]").transform;
                item.transform.SetParent(placeableParent);

                if (item.GetComponent<Outline>() != null)
                {
                    //disable this component
                    item.GetComponent<Outline>().enabled = false;

                    //how can i disable a material?

                }
            }
            else
            {
                Debug.Log("Cannot instantiate prefab: " + buildingPiece.pieceType);
            }
        }

    }


    // Step 4.1: Restore building pieces
    private void RestoreBuildingPieces(List<BuildingPieceData> buildingPieces, List<BuildingPieceData> ghostsList)
    { 

        foreach (var buildingPiece in buildingPieces)
        {
            //Debug.Log("Building piece: " + buildingPiece.pieceType);
            GameObject item = Instantiate(Resources.Load<GameObject>(buildingPiece.pieceType), buildingPiece.position, buildingPiece.rotation);

            item.name = buildingPiece.pieceType;

            //we should destroy the ghosts because when we load the game, we will instantiate the old ghosts(the free and occupied ones)
            item.GetComponent<Constructable>().DestroyGhosts();
            //item.GetComponent<Constructable>().ExtractGhostMembers();

            item.tag = "placedFoundation";
            Transform constructablesParent = GameObject.Find("[Constructables]").transform;
            item.transform.SetParent(constructablesParent);

            ConstructionManager.Instance.itemsBuilded.Add(new BuildingPieceData(item.name, item.transform.position, item.transform.rotation));
            //Debug.Log($"Handled loaded item and extracted ghosts for {item.name}");
        }

        RestoreGhosts(ghostsList);
    }

    //Step 4.2: Restore ghosts for building pieces

    private void RestoreGhosts(List<BuildingPieceData> ghosts)
    {
       
        foreach (var ghost in ghosts)
        {
            GameObject itemToInstantiate = Resources.Load<GameObject>("GhostsForBuilding/" + ghost.pieceType);

            if(itemToInstantiate == null)
            {
                Debug.LogError("Ghost prefab not found for: " + ghost.pieceType);
                continue;
            }

            // Correct rotation for walls only ONCE and update the data
            /*if (ghost.pieceType.Contains("W"))
            {
                Quaternion correctedRotation = Quaternion.Euler(ghost.rotation.eulerAngles.x, ghost.rotation.eulerAngles.y + 90, ghost.rotation.eulerAngles.z);
                ghost.rotation = correctedRotation; // Save the corrected rotation back
            }
            */
           
            

            GameObject item = Instantiate(itemToInstantiate, ghost.position, ghost.rotation);
            
            item.name = ghost.pieceType;
            if (ghost.pieceType.Contains("W"))
            {
                item.tag = "wallGhost";
            }
            else
            {
                item.tag = "ghost";
            }

            Transform constructablesParent = GameObject.Find("[Constructables]").transform;

            item.transform.SetParent(constructablesParent);

            item.transform.rotation = ghost.rotation;

        }
        
    }

    // Step 5: Restore plantables
    private void RestoreSoils(List<SoilPieceData> soils)
    {
        foreach(SoilPieceData soil in soils)
        {
            if(soil.name.Contains("Clone"))
            {
                soil.name = soil.name.Replace("(Clone)", "");
            }

            GameObject newSoil = Instantiate(Resources.Load<GameObject>(soil.name), soil.position, soil.rotation);

            //set the parent

            if (!string.IsNullOrEmpty(soil.parentName))
            {
                Transform parentTransform = GameObject.Find(soil.parentName)?.transform;
                if (parentTransform != null)
                {
                    newSoil.transform.SetParent(parentTransform);
                }
            }

            Soil soilComponent = newSoil.GetComponent<Soil>();
            if (soilComponent != null)
            {
                RestoreSoilProperties(soilComponent, soil);
            }
        }
    }

    private void RestoreSoilProperties(Soil soilComponent, SoilPieceData soilPiece)
    {
        soilComponent.isEmpty = soilPiece.isEmpty;
        soilComponent.plantName = soilPiece.plantName;
        soilComponent.name = soilPiece.name;
        soilComponent.parentName = soilPiece.parentName;
       

        if (!soilComponent.isEmpty)
        {
            GameObject instantiatedPlant = Instantiate(Resources.Load<GameObject>($"{soilPiece.plantName}Plant"));
            instantiatedPlant.transform.parent = soilComponent.transform;

            Vector3 plantPosition = Vector3.zero;
            plantPosition.y = 0f;
            instantiatedPlant.transform.localPosition = plantPosition;

            soilComponent.currentPlant = instantiatedPlant.GetComponent<Plant>();
            soilComponent.currentPlant.isWatered = soilPiece.currentPlantData.isWatered;
            soilComponent.currentPlant.plantAge= soilPiece.currentPlantData.plantAge;
            soilComponent.currentPlant.isOneTimeHarvest = soilPiece.currentPlantData.isOneTimeHarvest;

            List<bool> produceSpawnStates = soilPiece.currentPlantData.produceSpawnStates;
            for (int i = 0; i < soilComponent.currentPlant.plantProduceSpawns.Count; i++)
            {
                GameObject spawn = soilComponent.currentPlant.plantProduceSpawns[i];

                // If the saved state indicates there should be a produce, instantiate it
                if (produceSpawnStates[i])
                {
                    GameObject produce = Instantiate(soilComponent.currentPlant.producePrefab, spawn.transform.position, Quaternion.identity);
                    produce.transform.SetParent(spawn.transform);
                }
            }

            soilComponent.currentPlant.CheckGrowth();

           

        }

        soilComponent.GetComponent<Renderer>().material = soilPiece.isWatered ? soilComponent.wateredMaterial : soilComponent.defaultMaterial;
    }

    // Step 6: Restore rocks
    public void RestoreRocks(List<RockData> rocks)
    {
        //clean the rocks from the scene
        if(EnvironmentManager.Instance.rocksParent != null)
        {
            foreach(Transform child in EnvironmentManager.Instance.rocksParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        //restore the saved ones
        foreach (var rock in rocks)
        {
            if (rock.rockName.Contains("Clone"))
            {
                rock.rockName = rock.rockName.Replace("(Clone)", "");
            }

 
            GameObject newRock = Instantiate(Resources.Load<GameObject>(rock.rockName), rock.position, rock.rotation);
            newRock.name = rock.rockName;

            if (EnvironmentManager.Instance.rocksParent != null)
            {
                newRock.transform.SetParent(EnvironmentManager.Instance.rocksParent.transform, true);
            }

            if (rock.rockName == "MinedRock")
            {
                newRock.GetComponent<RegrowRock>().dayOfRegrowth = rock.dayOfRegrowth;
                //if this rock was mined and we load the game, we want to disable it until it "regrows"
                newRock.gameObject.SetActive(false);
                newRock.GetComponent<RegrowRock>().SubscribeToDayPassEvent(); 
            }
        }

    }

    // Step 7: Restore trees
    private void RestoreTrees(List<TreeData> trees)
    {
        // Clear existing trees in the scene
        foreach (Transform child in EnvironmentManager.Instance.treesParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var treeData in trees)
        {
            if (treeData.treeName.Contains("Clone"))
            {
                treeData.treeName = treeData.treeName.Replace("(Clone)", "");
            }

            // Instantiate the tree using the saved data
            GameObject newTree = Instantiate(
                               Resources.Load<GameObject>(treeData.treeName),
                                              treeData.position,
                                              treeData.rotation);

            newTree.name = treeData.treeName;

            if (EnvironmentManager.Instance.treesParent != null)
            {
                newTree.transform.SetParent(EnvironmentManager.Instance.treesParent.transform, true);
            }

            if(treeData.treeName == "DeadTree")
            {
                newTree.GetComponent<RegrowTree>().dayOfRegrowth = treeData.dayOfRegrowth;
                //if this tree was chopped down and we load the game, we want to disable it until it "regrows"
                newTree.gameObject.SetActive(false);
                newTree.GetComponent<RegrowTree>().SubscribeToDayPassEvent();
            }
        }
    }



    private void SetQuestData(QuestData questData)
    {
        QuestManager.Instance.allCompletedQuests = questData.allCompletedQuestsContent;

        //Set the questInfo for each quest

        foreach(Quest quest in QuestManager.Instance.allCompletedQuests)
        {
            quest.info = Resources.Load<QuestInfo>("Quests/Maria/" + quest.infoName);
            if (quest.info == null)
            {
                Debug.LogError($"QuestInfo not found for {quest.infoName}.");
            }
        }

        // Set the first time interaction with NPC
        NPC.Instance.firstTimeInteraction = questData.firstTimeInteractionWithNPC;

        // Set the active quest index
        NPC.Instance.activeQuestIndex = questData.questIndex;

        // Set the current active quest based on the loaded index
        if (NPC.Instance.activeQuestIndex < NPC.Instance.quests.Count)
        {
            NPC.Instance.currentActiveQuest = NPC.Instance.quests[NPC.Instance.activeQuestIndex];
        }
        else
        {
            // Handle the case where the index might be out of range
            NPC.Instance.currentActiveQuest = null;
            Debug.LogError("Loaded quest index is out of range!");
        }

        // Set the coin amount
        InventorySystem.Instance.SetCoins(questData.coinAmount);

        QuestManager.Instance.RefreshTrackerList();
        QuestManager.Instance.RefreshQuestList(); 
    }

    private void SetStorageBoxData(StorageBoxData storageBoxData)
    {
        // Find the Placeables object inside Environment
        GameObject placeablesParent = GameObject.Find("[Placeables]");

        if (placeablesParent == null)
        {
            Debug.LogError("Could not find [Placeables] inside Environment.");
            return;
        }

        //int index = 0;
        foreach (var storagePiece in storageBoxData.storageBoxList)
        {
            // Instantiate storage box at the saved position and rotation
            GameObject newStorageBox = Instantiate(Resources.Load<GameObject>("StorageBoxModel"), storagePiece.position, storagePiece.rotation);
            newStorageBox.name = "StorageBoxModel";
            // Set the parent to [Placeables] to ensure it's inside the correct hierarchy
            newStorageBox.transform.SetParent(placeablesParent.transform, true);


           

            // Load items into the storage box (could be empty if nothing was added)
            StorageBox storageBoxComponent = newStorageBox.GetComponent<StorageBox>();
            storageBoxComponent.items = new List<string>(storagePiece.storedItems);

            StorageManager.Instance.storageBoxList.Add(new StorageBoxPieceData(storageBoxComponent, newStorageBox.name, newStorageBox.transform.localPosition, newStorageBox.transform.localRotation, newStorageBox.transform.parent?.name));


        }
        

        
    }


    private void SetCampfireData(CampfireData campfireData)
    {
        // Find the Placeables parent object inside Environment
        GameObject placeablesParent = GameObject.Find("[Placeables]");

        if (placeablesParent == null)
        {
            Debug.LogError("Could not find [Placeables] inside Environment.");
            return;
        }

        // Iterate through all the campfires in the saved data
        foreach (var campfirePiece in campfireData.campfireList)
        {
            // Instantiate the campfire at the saved position and rotation
            GameObject newCampfire = Instantiate(Resources.Load<GameObject>(campfirePiece.campfireName), campfirePiece.position, campfirePiece.rotation);

            // Set the parent to [Placeables] to ensure it's inside the correct hierarchy
            if (!string.IsNullOrEmpty(campfirePiece.parentName))
            {
                Transform parentTransform = GameObject.Find(campfirePiece.parentName)?.transform;
                if (parentTransform != null)
                {
                    newCampfire.transform.SetParent(parentTransform);
                }
                else
                {
                    newCampfire.transform.SetParent(placeablesParent.transform, true);
                }
            }
            else
            {
                newCampfire.transform.SetParent(placeablesParent.transform, true);
            }

            newCampfire.name = campfirePiece.campfireName;
            newCampfire.GetComponent<Outline>().enabled = false;

            // Get the Campfire component of the instantiated object
            Campfire campfireComponent = newCampfire.GetComponent<Campfire>();


            // Restore the stored items inside the campfire
            campfireComponent.items = new List<string>(campfirePiece.storedItems);
        }
    }


    private void HandleLoadedItem(GameObject item)
    {
        // Assuming that the item is now "loaded" and should be treated as if it were freshly placed
        ConstructionManager.Instance.itemToBeConstructed = item;

        // Directly call the method that handles freestyle placement
        ConstructionManager.Instance.PlaceItemFreeStyle();
    }

    public void StartLoadedGame(int slotNumber)
    {
        isLoading = true;
        ActivateLoadingScreen();
        SceneManager.LoadScene("GameScene");
        StartCoroutine(DelayedLoading(slotNumber));
    }

    private IEnumerator DelayedLoading(int slotNumber)
    {
        yield return new WaitForSeconds(1f);
        LoadGame(slotNumber);
    }

    #endregion

    #endregion

    #region || --------- To Binary Section --------- ||

    public void SaveGameDataToBinaryFile(AllGameData gameData, int slotNumber)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        FileStream stream = new FileStream(binaryPath + fileName + slotNumber
            + ".bin", FileMode.Create);

        formatter.Serialize(stream, gameData);
        stream.Close();

        print("Data saved to: " + binaryPath + fileName + slotNumber
            + ".bin");
    }

    public AllGameData LoadGameDataFromBinaryFile(int slotNumber)
    {
        if (File.Exists(binaryPath + fileName + slotNumber + ".bin"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(binaryPath + fileName + slotNumber
            + ".bin", FileMode.Open);

            AllGameData data = formatter.Deserialize(stream) as AllGameData;
            stream.Close();

            print("Data loaded from: " + binaryPath + fileName + slotNumber
            + ".bin");

            return data;
        }
        else
        {
            print("Save file not found in: " + binaryPath);
            return null;
        }
    }

    #endregion

    #region || --------- To Json Section --------- ||

    public void SaveGameDataToJsonFile(AllGameData gameData, int slotNumber)
    {
        string json = JsonUtility.ToJson(gameData);

        //string encrypted = EcryptionDecryption(json);

        using (StreamWriter writer = new StreamWriter(jsonPathProject + fileName +
            slotNumber + ".json"))
        {
            writer.Write(json);
            print("Data saved to: " + jsonPathProject + fileName +
            slotNumber + ".json");
        }
    }

    public AllGameData LoadGameDataFromJsonFile(int slotNumber)
    {
        using (StreamReader reader = new StreamReader(jsonPathProject + fileName +
            slotNumber + ".json"))
        {
            string json = reader.ReadToEnd();

            //string decrypted = EcryptionDecryption(json);

            AllGameData data = JsonUtility.FromJson<AllGameData>(json);
            return data;
        }
    }

    #endregion

    #region || --------- Commented Settings Section --------- ||

    /*
    // Save and load separately
    // Music Volume
    public void SaveMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public float LoadMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1f); // 1f is the default value if the key does not exist
    }

    // Effects Volume
    public void SaveEffectsVolume(float volume)
    {
        PlayerPrefs.SetFloat("EffectsVolume", volume);
        PlayerPrefs.Save();
    }

    public float LoadEffectsVolume()
    {
        return PlayerPrefs.GetFloat("EffectsVolume", 1f); // 1f is the default value if the key does not exist
    }

    // Master Volume
    public void SaveMasterVolume(float volume)
    {
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
    }

    public float LoadMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 1f); // 1f is the default value if the key does not exist
    }
    */
    #endregion

    #region || --------- Settings Section --------- ||


    #region || --------- Mouse Sensivity Settings --------- ||

    public void SaveMouseSensivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensivity", value);
        PlayerPrefs.Save();
    }

    public float LoadMouseSensivity()
    {
        string json = PlayerPrefs.GetString("MouseSensitivity", null);

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("Mouse sensitivity data is missing. Returning default value.");
            return 1f; // Default sensitivity
        }

        try
        {
            float data = JsonUtility.FromJson<float>(json);
            return data != null ? data : 1f; // Default value if deserialization fails
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to load mouse sensitivity: {ex.Message}");
            return 1f; // Default sensitivity
        }

    }

    #endregion

    #region || --------- Volume Settings --------- ||

    [System.Serializable]
    public class VolumeSettings
    {
        public float master;  // the music that runs in the background
    }

    public void SaveVolumeSettings(float master)
    {
        VolumeSettings volumeSettings = new VolumeSettings()
        { 
            master = master
        };

        PlayerPrefs.SetString("Volume", JsonUtility.ToJson(volumeSettings));
        PlayerPrefs.Save();
    }

    public VolumeSettings LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey("Volume"))
        {
            return JsonUtility.FromJson<VolumeSettings>(PlayerPrefs.GetString("Volume"));
        }
        else
        {
            return new VolumeSettings() { master = 1f }; // return default values if no settings are saved
        }
    }

    #endregion

    #endregion

    #region || --------- Encryption --------- ||

    public string EcryptionDecryption(string data)
    {
        string keyword = "1234567";
        string result = "";

        for (int i = 0; i < data.Length; i++)
        {
            result += (char)(data[i] ^ keyword[i % keyword.Length]);
        }

        return result;
    }

    #endregion

    #region || --------- ActivateLoadingScreen --------- ||

    public void ActivateLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(true);

        //Cursor.lockState = CursorLockMode.None;
        //animation
    }

    public void DisableLoadingScreen()
    {
        loadingScreen.gameObject.SetActive(false);
    }

    #endregion

    #region || --------- Utility --------- ||

    public bool DoesFileExist(int slotNumber)
    {
        if (isSavingToJson)
        {
            if (System.IO.File.Exists(jsonPathProject + fileName + slotNumber + ".json"))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            if (System.IO.File.Exists(binaryPath + fileName + slotNumber + ".bin"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsSlotEmpty(int slotNumber)
    {
        if (DoesFileExist(slotNumber))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void DeselectButton()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(null);
    }

    #endregion
}
