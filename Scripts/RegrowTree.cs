using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegrowTree : MonoBehaviour
{
    // Start is called before the first frame update
    public int dayOfRegrowth;

    public bool growthLocked = false; 
    //if the tree is chopped down, this will be set to true

    private void Start()
    {
        SubscribeToDayPassEvent();
    }

    public void SubscribeToDayPassEvent()
    {
        TimeManager.Instance.OnDayPass.AddListener(CheckForRegrowth);
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnDayPass event to prevent memory leaks
        TimeManager.Instance.OnDayPass.RemoveListener(CheckForRegrowth);
    }

    private void CheckForRegrowth()
    {

        if (TimeManager.Instance.dayInGame == dayOfRegrowth &&
            growthLocked == false)
        {
            growthLocked = true;
            RegrowTreeFunction();
        }
        else if (TimeManager.Instance.dayInGame == dayOfRegrowth - 1)
        {
            DeleteRemainingItems();
        }
    }

    private void RegrowTreeFunction()
    {
        //there will "radacina copacului" si trebuie sa o stergem
        gameObject.SetActive(false);

        GameObject newTree = Instantiate(Resources.Load<GameObject>("Tree"), 
            new Vector3(transform.position.x+3, transform.position.y-2, transform.position.z+2),
            Quaternion.Euler(0,0,0));

        //we set the new tree to be a child of the parent of the old tree
        //search for the parent of the old tree
       
        newTree.name = "Tree";
        newTree.transform.SetParent(transform.parent);

        //destroy the old tree stump
        Destroy(gameObject);

    }

    private void DeleteRemainingItems()
    {
        foreach(Transform child in transform)
        {
            if(child.gameObject.name.Contains("Log") || child.gameObject.name.Contains("Stick"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
