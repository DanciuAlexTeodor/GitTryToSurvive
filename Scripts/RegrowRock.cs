using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// /THIS SCRIPT IS USED FOR THE MINED ROCKS
/// </summary>
public class RegrowRock : MonoBehaviour
{
    // Start is called before the first frame update
    public int dayOfRegrowth;

    public bool growthLocked = false;
    //if the tree is chopped down, this will be set to true

    private void Start()
    {
        SubscribeToDayPassEvent();
        //Debug.Log("RegrowRock script is working");
    }

    public void SubscribeToDayPassEvent()
    {
        //Debug.Log("Subscribing to the OnDayPass event");
        TimeManager.Instance.OnDayPass.AddListener(CheckForRegrowth);
    }

    private void CheckForRegrowth()
    {
        //Debug.Log("Checking for regrowth: current day:" + TimeManager.Instance.dayInGame + "| day for revive: " + dayOfRegrowth);
        if (TimeManager.Instance.dayInGame == dayOfRegrowth &&
            growthLocked == false)
        {
            growthLocked = true;
            RegrowRockFunction();
        }
        else if (TimeManager.Instance.dayInGame == dayOfRegrowth - 1)
        {
            DeleteRemainingItems();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnDayPass event to prevent memory leaks
        TimeManager.Instance.OnDayPass.RemoveListener(CheckForRegrowth);
    }

    private void RegrowRockFunction()
    {
        GameObject newRock = Instantiate(Resources.Load<GameObject>("Rock"),
            new Vector3(transform.position.x, transform.position.y, transform.position.z),
            Quaternion.Euler(0, 0, 0));

        //we set the new rock to be a child of the parent of the old rock
        newRock.transform.SetParent(transform.parent);

        //destroy the remaining stones and iron which are on the ground
        Destroy(gameObject);

    }

    private void DeleteRemainingItems()
    {
        //we need to displace the iron and stones to another parent
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Contains("Iron")  || child.gameObject.name.Contains("Stone"))
            {
                Destroy(child.gameObject);
            }
        }
    }
}
