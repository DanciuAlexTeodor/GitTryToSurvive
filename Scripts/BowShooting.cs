using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShooting : MonoBehaviour
{
    public bool isArrowLoaded = false;
   
    public void LaunchArrow()
    {
        if (isArrowLoaded)
        {
            
            isArrowLoaded = false;
        }
    }
    


}
