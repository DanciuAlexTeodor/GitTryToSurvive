using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DeadCreature
{
    public string name;
    public int dayOfRevival;
    public Vector3 spawningLocation;
}

public class RevivalManager : MonoBehaviour
{
    public static RevivalManager Instance { get; set; }

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
    }

    public List<DeadCreature> deadCreatures = new List<DeadCreature>();

    private IEnumerator WaitForTimeManager()
    {
        // Wait until TimeManager.Instance is not null
        while (TimeManager.Instance == null)
        {
            yield return null;
        }

        //Debug.Log("Subscribing to OnDayPass event");
        TimeManager.Instance.OnDayPass.AddListener(CheckIfTimeToInstantiate);
    }

    private void Start()
    {
        StartCoroutine(WaitForTimeManager());
    }


    public void AddDeadCreature(string name, int dayOfRevival, Vector3 spawningLocation)
    {
        //Debug.Log("Adding dead creature " + name + "with day of revival: " + dayOfRevival + " to the list");
        DeadCreature deadCreature = new DeadCreature();
        deadCreature.name = name;
        deadCreature.dayOfRevival = dayOfRevival;
        deadCreature.spawningLocation = spawningLocation;
        deadCreatures.Add(deadCreature);
    }

    public void CheckIfTimeToInstantiate()
    {
        //Debug.Log(deadCreatures.Count + " dead creatures in the list");

        // Use a temporary list to collect creatures that need to be revived
        List<DeadCreature> creaturesToRevive = new List<DeadCreature>();

        foreach (DeadCreature deadCreature in deadCreatures)
        {
            Debug.Log("Checking if it's time to instantiate " + deadCreature.name +
                      " at " + deadCreature.spawningLocation +
                      " on day " + deadCreature.dayOfRevival +
                      " current day is " + TimeManager.Instance.dayInGame);

            if (deadCreature.dayOfRevival <= TimeManager.Instance.dayInGame)
            {
                InstantiateCreature(deadCreature.name, deadCreature.spawningLocation);
                creaturesToRevive.Add(deadCreature); // Mark for removal
            }
        }

        // Remove revived creatures from the original list
        foreach (DeadCreature creature in creaturesToRevive)
        {
            deadCreatures.Remove(creature);
        }
    }


    public void InstantiateCreature(string name, Vector3 spawningLocation)
    {
        //Instantiate creature
        string folderName = "Animals/";
        if(name.Contains("Soul") || name.Contains("Wolf") || name.Contains("Bear"))
        {
            folderName = "Enemy/";
        }

        GameObject creaturePrefab = Resources.Load<GameObject>(folderName + name);

        if(creaturePrefab != null)
        {
            GameObject creature = Instantiate(creaturePrefab);

            creature.transform.position = spawningLocation;

            creature.name = creature.name.Replace("(Clone)", "");

            creature.transform.SetParent(EnvironmentManager.Instance.animalsParent.transform);
        }
        else
        {
            Debug.LogError("Could not find creature prefab: " + name + " in: " + folderName+name);
        }
        
        
    }
    

}
