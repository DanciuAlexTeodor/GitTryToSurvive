using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ChoppableTree : MonoBehaviour
{

    public bool playerInRange;
    public bool canBeChopped;

    public float treeMaxHealth;
    public float treeHealth;

    public float caloriesSpentChoppingWood = 50;

    public Animator animator;
    public bool firstTimeEnter = false;

    public GameObject applePrefab;
    public GameObject stickPrefab;
    public GameObject mushroomPrefab;

    private ResourceHealthBar resourceHealthBar;

    

    private void Start()
    {
        treeHealth = treeMaxHealth;
        animator = transform.parent.transform.parent.GetComponent<Animator>();
        SpawnEnvironmentAroundTree();
    }

    private void SpawnObject(int changeToBeSpawned, int leftSizeForRandomDistanceFunction, int rightSizeForRandomDistanceFunction, GameObject prefab)
    {
        if (Random.Range(0, 100) < changeToBeSpawned)
        {
            float randomDistanceX = Random.Range(leftSizeForRandomDistanceFunction, rightSizeForRandomDistanceFunction);
            float randomDistanceZ = Random.Range(leftSizeForRandomDistanceFunction, rightSizeForRandomDistanceFunction);
            GameObject objectToBeInstantiated = Instantiate(prefab,
                               new Vector3(transform.position.x + randomDistanceX, transform.position.y + 1, transform.position.z + randomDistanceZ),
                               Quaternion.Euler(0, 0, 0));
            objectToBeInstantiated.name = prefab.name;
            objectToBeInstantiated.transform.SetParent(transform.parent.transform.parent);
        }
    }

    private void SpawnEnvironmentAroundTree()
    {
        SpawnObject(20, -5, 5, applePrefab);

        SpawnObject(50, -5, 5, stickPrefab);

        SpawnObject(20, -2, 2, mushroomPrefab);
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

    public void GetHit()
    {
        StartCoroutine(hit());
    }

    private IEnumerator hit()
    {
        yield return new WaitForSeconds(1.1f);

        //CHANGE TO 4
        if(EquipSystem.Instance.selectedItemName=="Stone Axe")
            treeHealth -= 3;
        else if(EquipSystem.Instance.selectedItemName=="Iron Axe")
            treeHealth -= 4;


        EquipSystem.Instance.decreaseWeaponHealth(5);
        SoundManager.Instance.PlaySound(SoundManager.Instance.wood_hit);
        //animator.SetTrigger("shake");
        PlayerState.Instance.currentCalories -= caloriesSpentChoppingWood;
        if(treeHealth <= 0)
        {
            TreeIsDead();
            
        }
        yield return new WaitForSeconds(0.8f);
    }

    void TreeIsDead()
    {
        Vector3 treePosition = transform.position;
        Destroy(transform.parent.transform.parent.gameObject);
        canBeChopped = false;

        SelectionManager.Instance.selectedTree = null;
        SelectionManager.Instance.chopHolder.gameObject.SetActive(false);

        GameObject brokenTree = Instantiate(Resources.Load<GameObject>("DeadTree"),
            new Vector3(treePosition.x-2.5f, treePosition.y+1.35f, treePosition.z), Quaternion.Euler(0,0,0));

        StartCoroutine(cool());
        SoundManager.Instance.PlaySound(SoundManager.Instance.log_hit_ground);

        brokenTree.name = "DeadTree";
        brokenTree.GetComponent<RegrowTree>().dayOfRegrowth = TimeManager.Instance.dayInGame+2;

        Transform parent = GameObject.Find("[Trees]").transform;
        brokenTree.transform.SetParent(parent);

    }

    IEnumerator cool()
    {
        yield return new WaitForSeconds(3f);
        
    }

    public void HoverOverTree()
    {
        GlobalState.Instance.resourceHealth = treeHealth;
        GlobalState.Instance.resourceMaxHealth = treeMaxHealth;

        if(resourceHealthBar== null)
        {
            Debug.Log("ResourceHealthBar is null");
            resourceHealthBar = FindObjectOfType<ResourceHealthBar>();
        }

        if (resourceHealthBar != null)
        {
            resourceHealthBar.UpdateHealthBar();
        }
    }
}
