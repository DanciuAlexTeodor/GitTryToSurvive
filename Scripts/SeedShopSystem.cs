using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SeedShopSystem : MonoBehaviour
{

    public static SeedShopSystem Instance { get; set; }

    public GameObject seedShopUI;
    private TextMeshProUGUI coinAmountText;
    private int coinAmount;
    public bool isOpen;
    //create a <string,int> dictionary to store the seed name and its price
    private Dictionary<string, int> seedPrice = new Dictionary<string, int>();

    Button buyTomatoBTN, buyCornBTN, buyTurnipBTN, buyPumpkinBTN, buyCarrotBTN, buyEggplantBTN;

    private void Start()
    {
        seedShopUI.SetActive(false);
        isOpen = false;

        buyTomatoBTN = seedShopUI.transform.Find("Tomato").transform.Find("TomatoBTN").GetComponent<Button>();
        buyCornBTN = seedShopUI.transform.Find("Corn").transform.Find("CornBTN").GetComponent<Button>();
        buyTurnipBTN = seedShopUI.transform.Find("Turnip").transform.Find("TurnipBTN").GetComponent<Button>();
        buyPumpkinBTN = seedShopUI.transform.Find("Pumpkin").transform.Find("PumpkinBTN").GetComponent<Button>();
        buyCarrotBTN = seedShopUI.transform.Find("Carrot").transform.Find("CarrotBTN").GetComponent<Button>();
        buyEggplantBTN = seedShopUI.transform.Find("Eggplant").transform.Find("EggplantBTN").GetComponent<Button>();

        buyTomatoBTN.onClick.AddListener(() => BuySeed("Tomato"));
        buyCornBTN.onClick.AddListener(() => BuySeed("Corn"));
        buyTurnipBTN.onClick.AddListener(() => BuySeed("Turnip"));
        buyPumpkinBTN.onClick.AddListener(() => BuySeed("Pumpkin"));
        buyCarrotBTN.onClick.AddListener(() => BuySeed("Carrot"));
        buyEggplantBTN.onClick.AddListener(() => BuySeed("Eggplant"));

        seedPrice.Add("Tomato", 5);
        seedPrice.Add("Corn", 4);
        seedPrice.Add("Turnip", 2);
        seedPrice.Add("Pumpkin", 3);
        seedPrice.Add("Carrot", 2);
        seedPrice.Add("Eggplant", 4);
    }

    private void BuySeed(string seedName)
    {
        Debug.Log("Buying seed: " + seedName);
        int price = seedPrice[seedName];

        //convert it to int
        string coinAmountString = InventorySystem.Instance.GetAmountOfMoney();
        coinAmount = int.Parse(coinAmountString);
        if(coinAmount < price)
        {
            Debug.Log("Not enough money");
            return;
        }

        coinAmountText.text = (coinAmount - price).ToString();

        InventorySystem.Instance.SetCoins(coinAmount - price);

        InventorySystem.Instance.AddToInventory(seedName+"Seed");
    }

    private void Awake()
    {
  
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            //set the instance to this
            Instance = this;
        }
    }

    public void OpenShop()
    {
        isOpen = true;

        CursorManager.Instance.FreeCursor();

        coinAmountText = seedShopUI.transform.Find("CoinAmount")?.GetComponent<TextMeshProUGUI>();

        coinAmountText.text = InventorySystem.Instance.GetAmountOfMoney();
        //Debug.Log("Coin amount set to: " + coinAmountText.text);

        seedShopUI.SetActive(true);
    }


    public void CloseShop()
    {
        isOpen = false;
        seedShopUI.SetActive(false);

        CursorManager.Instance.LockCursor();
    }



}
