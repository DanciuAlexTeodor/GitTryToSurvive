using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint
{
    public string itemName;

    public string Req1;
    public string Req2;

    public int Req1Amount;
    public int Req2Amount;

    public int numOfRequirements;
    public int numOfItemsCrafted;

    public Blueprint(string itemName, int numOfItemsCrafted, string Req1, string Req2,
        int Req1Amount, int Req2Amount, int numOfRequirements)
    {
        this.itemName = itemName;
        this.numOfItemsCrafted = numOfItemsCrafted;
        this.Req1 = Req1;
        this.Req2 = Req2;
        this.Req1Amount = Req1Amount;
        this.Req2Amount = Req2Amount;
        this.numOfRequirements = numOfRequirements;
        
    }
}
