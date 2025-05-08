using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentData
{
    public TimeData timeData;
    public List<CampfirePieceData> campfireList;
    public List<FurnacePieceData> furnaceList;
    public List<string> itemsPickedUpFromTheGround;
    public List<BuildingPieceData> placeables;
    public List<BuildingPieceData> itemsBuilded;
    public List<BuildingPieceData> ghostItemsBuilded;
    public List<AnimalData> animals;
    public List<EnemyData> enemies;
    public List<DeadCreature> deadCreatures;
    public List<SoilPieceData> soils;
    public List<RockData> rocks;
    public List<TreeData> trees;

    public EnvironmentData(TimeData _timeData,
        List<CampfirePieceData> _campfireList,
        List<FurnacePieceData> _furnaceList,
        List<string> _itemsPickedUpFromTheGround, 
        List<BuildingPieceData> _placeables,
        List<BuildingPieceData> _construction,
        List<BuildingPieceData> _ghostConstruction,
        List<AnimalData> _animalList,
        List<EnemyData> _enemies,
        List<DeadCreature> _deadCreatures,
        List<SoilPieceData> _soils,
        List<RockData> _rocks,
         List<TreeData> _trees)
        
    {
        timeData = _timeData;
        campfireList = _campfireList;
        furnaceList = _furnaceList;
        itemsPickedUpFromTheGround = _itemsPickedUpFromTheGround;
        placeables = _placeables;
        itemsBuilded = _construction;
        ghostItemsBuilded = _ghostConstruction;
        animals = _animalList;
        enemies = _enemies;
        deadCreatures = _deadCreatures;
        soils = _soils;
        rocks = _rocks;
        trees = _trees;
    }
}