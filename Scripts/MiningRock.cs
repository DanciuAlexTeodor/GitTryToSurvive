using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//!!!!!!
//trebuie sa maresc baza sa cuprinda tot obiectul, astfel nu il gaseste

[RequireComponent(typeof(BoxCollider))]
public class MiningRock : MonoBehaviour
{
    public bool playerInRange;
    public bool canBeMined;

    private ResourceHealthBar resourceHealthBar;

    public float rockMaxHealth;
    public float rockHealth;

    public float caloriesSpentMiningRock = 50;

    public Animator animator;
    public GameObject stonePrefab;

    private void Start()
    {
        //i want to change the text of the chop holder to the rock health
        rockHealth = rockMaxHealth;
        animator = transform.parent.transform.parent.GetComponent<Animator>();
        SpawnRocksNearby();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private float GetRandomPosition()
    {
        float randomValue = Random.Range(-4f, 4f);

        // Exclude the range between -1 and 1
        while (randomValue > -1f && randomValue < 1f)
        {
            randomValue = Random.Range(-4f, 4f);
        }

        return randomValue;
    }

    private void SpawnRocksNearby()
    {
        int randomNumberOfRocks = Random.Range(2, 4);
        for(int i=0; i<randomNumberOfRocks; i++)
        {
            //i want a random position between -4 and 4 but not between -1 and 1
            float randomX = GetRandomPosition();
            float randomZ = GetRandomPosition();
            Vector3 randomPosition = new Vector3(transform.position.x + randomX, transform.position.y+1, transform.position.z + randomZ);
            GameObject stone = Instantiate(stonePrefab, randomPosition, Quaternion.Euler(0, 0, 0));
            stone.transform.SetParent(transform.parent.transform.parent);
            stone.name = "Stone";

        }
    }

    public void GetHit()
    {   
        EquipableItem.Instance.isHitingRock = true;
        StartCoroutine(hit());

    }

    private IEnumerator hit()
    {
        yield return new WaitForSeconds(1f);

        SoundManager.Instance.PlaySound(SoundManager.Instance.rock_hit);
        //CHANGE TO 4
        if (EquipSystem.Instance.selectedItemName == "Stone Pickaxe")
            rockHealth -= 3;
        else if (EquipSystem.Instance.selectedItemName == "Iron Pickaxe")
            rockHealth -= 4;

        EquipSystem.Instance.decreaseWeaponHealth(5);
        //animator.SetTrigger("shake_hit");
        PlayerState.Instance.currentCalories -= caloriesSpentMiningRock;
        if (rockHealth <= 0)
        {
            RockIsDead();
        }
        yield return new WaitForSeconds(0.5f);
        EquipableItem.Instance.isHitingRock = false;
    }

    void RockIsDead()
    {
        Vector3 rockPosition = transform.position;
        Destroy(transform.parent.transform.parent.gameObject);
        canBeMined = false;

        SelectionManager.Instance.selectedRock = null;
        SelectionManager.Instance.chopHolder.gameObject.SetActive(false);

        GameObject brokenRock = Instantiate(Resources.Load<GameObject>("MinedRock"),
            new Vector3(rockPosition.x, rockPosition.y, rockPosition.z), Quaternion.Euler(0, 0, 0));


        brokenRock.GetComponent<RegrowRock>().dayOfRegrowth = TimeManager.Instance.dayInGame + 2;
        brokenRock.name = "MinedRock";
        //search for objects of name [Rocks] and set them
        //as the parent of the broken rock

        Transform parent = GameObject.Find("[Rocks]").transform;

        brokenRock.transform.SetParent(parent);


        SoundManager.Instance.PlaySound(SoundManager.Instance.log_hit_ground);

    }

    public void HoverOverRock()
    {
        GlobalState.Instance.resourceHealth = rockHealth;
        GlobalState.Instance.resourceMaxHealth = rockMaxHealth;

        if(resourceHealthBar== null)
        {
            resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
        }

        if (resourceHealthBar != null)
        {
            
            resourceHealthBar.UpdateHealthBar();
        }
    }

}
