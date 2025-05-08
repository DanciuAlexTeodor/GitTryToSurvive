using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AllGameData 
{
    public PlayerData playerData;

    public EnvironmentData environmentData;

    public QuestData questData;

    public StorageBoxData storageBoxData;

    public CampfireData campfireData;
    //!!! Ai grija ca clasa sa nu fie MonoBehaviour, altfel nu o sa mearga serializarea

    //public ConstructionData constructionData;
}
