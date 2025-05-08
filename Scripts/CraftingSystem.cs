using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public static CraftingSystem Instance { get; set; }

    public GameObject craftingScreenUI;
    public GameObject toolsScreenUI, attackScreenUI, survivalScreenUI, utilityScreenUI;
    public GameObject buildScreenUI, furnitureScreenUI, armorScreenUI;

    public List<string> inventoryItemList = new List<string>();

    public bool isOpen;
    public bool isCrafting = false;
    public float hammerHealth = 0;
    public float knifeHealth = 0;

    #region ----------   Button and Text declarations   ----------
    //Category Buttons
    Button toolsBTN, survivalBTN, utilityBTN, buildBTN, attackBTN, furnitureBTN, armorBTN;

    //Crafting Buttons
    //Tools
    Button craftStone_AxeBTN, craftIron_AxeBTN, craftStone_PickaxeBTN, craftIron_PickaxeBTN;
    Button craftStoneShovelBTN, craftIronShovelBTN;

    //Attack
    Button craftStone_KnifeBTN, craftIron_KnifeBTN, craftIronSwordBTN, craftStoneSpearBTN, craftIronSpearBTN, craftBowBTN;

    //Survival
    Button craftLeatherBottleBTN, craftSmallBackpackBTN, craftBigBackpackBTN, craftWoodenTorchBTN;
    Button craftBandageBTN, craftArrowBTN;

    //Utility
    Button craftPlankBTN, craftCampfireBTN, craftFurnaceBTN, craftAnvilBTN, craftHammerBTN, craftWellBTN;

    //Build
    Button craftFoundationBTN, craftWallBTN, craftDoorBTN, craftWindowBTN, craftStairsBTN;
    Button craftFenceBTN, craftGateBNT;

    //Furniture
    Button craftSmallChestBTN, craftBedBTN, wallTorchBTN;
    Button craftCandleBTN;
     //Armor
     Button craftIronHelmetBTN, craftIronChestplateBTN, craftIronLeggsBTN, craftWoodShieldBTN, craftIronShieldBTN;


    //Requirement Text
    //Tools
    Text Stone_AxeReq1, Stone_AxeReq2, Iron_AxeRe1, Iron_AxeReq2;
    Text Stone_PickaxeReq1, Stone_PickaxeReq2;
    Text Iron_PickaxeReq1, Iron_PickaxeReq2;
    Text StoneShovelReq1, IronShovelReq1, StoneShovelReq2, IronShovelReq2;

    //Attack
    Text Stone_KnifeReq1, Stone_KnifeReq2, Iron_KnifeReq1, Iron_KnifeReq2, IronSwordReq1, IronSwordReq2;
    Text StoneSpearReq1, StoneSpearReq2, IronSpearReq1, IronSpearReq2, BowReq1, BowReq2;

    //Survival
    Text LeatherBottleReq1, LeatherBottleReq2;
    Text SmallBackpackReq1, SmallBackpackReq2;
    Text BigBackpackReq1, BigBackpackReq2;
    Text WoodenTorchReq1, WoodenTorchReq2;
    Text BandageReq1;
    Text ArrowReq1, ArrowReq2;

    //Utility
    Text PlankReq1, PlankReq2;
    Text CampfireReq1, CampfireReq2;
    Text FurnaceReq1, FurnaceReq2;
    Text AnvilReq1;
    Text HammerReq1, HammerReq2;
    Text WellReq1, WellReq2;

    //Build
    Text FoundationReq1, WallReq1, DoorReq1, WindowReq1, StairsReq1;
    Text FenceReq1, GateReq1;

    //Furniture
    Text SmallChestReq1, SmallChestReq2, BedReq1, BedReq2, wallTorchReq1, wallTorchReq2;
    Text CandleReq1, CandleReq2;

    //Armor
    Text IronHelmetReq1, IronHelmetReq2, IronChestplateReq1, IronChestplateReq2, IronLeggsReq1, IronLeggsReq2;
    Text WoodShieldReq1, WoodShieldReq2, IronShieldReq1, IronShieldReq2;

    #endregion


    //All Blueprints
    #region ----------   Blueprints   ----------
    //"ItemName", numOfItemsCrafted, "Req1", "Req2", Req1Amount, Req2Amount, numOfRequirements, 
    //Tools
    public Blueprint AxeBLP = new Blueprint("Stone Axe", 1, "Stone", "Stick", 3, 3, 2);
    public Blueprint Iron_AxeBLP = new Blueprint("Iron Axe", 1, "Iron Ingot", "Plank", 4, 3, 2);
    public Blueprint Stone_PickaxeBLP = new Blueprint("Stone Pickaxe", 1, "Stone", "Stick", 4, 5, 2);
    public Blueprint Iron_PickaxeBLP = new Blueprint("Iron Pickaxe", 1, "Iron Ingot", "Plank", 5, 3, 2);
    public Blueprint StoneShovelBLP = new Blueprint("Stone Shovel", 1, "Stone", "Stick", 4, 5, 2); // 6 5
    public Blueprint IronShovelBLP = new Blueprint("Iron Shovel", 1, "Iron Ingot", "Plank", 4, 3, 2); // 4 3

    //Attack
    public Blueprint Stone_KnifeBLP = new Blueprint("Stone Knife", 1, "Stone", "Stick", 2, 2, 2);
    public Blueprint Iron_KnifeBLP = new Blueprint("Iron Knife", 1, "Iron Ingot", "Plank", 2, 2, 2);
    public Blueprint IronSwordBLP = new Blueprint("Iron Sword", 1, "Iron Ingot", "Skin", 6, 2, 2);
    public Blueprint StoneSpearBLP = new Blueprint("Stone Spear", 1, "Plank", "Stone", 3, 3, 2);
    public Blueprint IronSpearBLP = new Blueprint("Iron Spear", 1, "Plank", "Iron Ingot", 3, 4, 2);
    public Blueprint BowBLP = new Blueprint("Bow", 1, "Plank", "Skin", 4, 4, 2);

    //Survival
    public Blueprint LeatherBottleBLP = new Blueprint("LeatherBottle", 1, "Stick", "Skin", 2, 5, 2); // 2 5
    public Blueprint SmallBackpackBLP = new Blueprint("SmallBackpack", 1, "Stick", "Skin", 4, 6, 2); //4 6 
    public Blueprint BigBackpackBLP = new Blueprint("BigBackpack", 1, "Plank", "Skin", 3, 9, 2); // 3 9
    public Blueprint WoodenTorchBLP = new Blueprint("WoodenTorch", 1, "Stick", "Skin", 2, 3, 2); // 2 3
    public Blueprint BandageBLP = new Blueprint("Bandage", 1, "Skin", "", 1, 0, 1); // 1 0
    public Blueprint ArrowBLP = new Blueprint("Arrow", 2, "Stick", "Iron Ingot", 2, 1, 2); // 

    //Utility
    public Blueprint PlankBLP = new Blueprint("Plank", 3, "Log", "", 1, 10, 2);
    public Blueprint CampfireBLP = new Blueprint("Campfire", 1, "Stone", "Stick", 8, 7, 2);
    public Blueprint FurnaceBLP = new Blueprint("Furnace", 1, "Stone", "Iron Ingot", 20,2, 2); // 20 2
    public Blueprint AnvilBLP = new Blueprint("Anvil", 1, "Iron Ingot", "", 10, 0, 1); 
    public Blueprint HammerBLP = new Blueprint("Hammer", 1, "Plank", "Iron Ingot", 1, 4, 2); // 1 4
    public Blueprint WellBLP = new Blueprint("Well", 1, "Stone", "Plank", 10, 5, 2); // 10 5

    //Build
    public Blueprint FoundationBLP = new Blueprint("Foundation", 1, "Plank", "", 4, 0, 1);
    public Blueprint WallBLP = new Blueprint("Wall", 1, "Plank", "", 3, 0, 1);
    public Blueprint DoorBLP = new Blueprint("Door", 1, "Plank", "", 5, 0, 1);
    public Blueprint WindowBLP = new Blueprint("Window", 1, "Plank", "", 3, 0, 1);
    public Blueprint StairsBLP = new Blueprint("Stairs", 1, "Plank", "", 7, 0, 1);
    public Blueprint FenceBLP = new Blueprint("Fence", 1, "Plank", "", 4, 0, 1);
    public Blueprint GateBPL = new Blueprint("Gate", 1, "Plank", "", 3, 0, 1);

    //Furniture
    public Blueprint SmallChestBLP = new Blueprint("StorageBox", 1, "Plank", "Iron Ingot", 3, 3, 2);
    public Blueprint BedBLP = new Blueprint("Bed", 1, "Plank", "Skin", 8, 6, 2);
    //public Blueprint wallTorchBLP = new Blueprint("WallTorch", 1, "Iron Ingot", "", 4, 0, 1);
    public Blueprint CandleBLP = new Blueprint("Candle", 1, "Fat", "Iron Ingot", 1, 1, 2);

    //Armor
    public Blueprint IronHelmetBLP = new Blueprint("IronHelmet", 1, "Iron Ingot", "", 3, 10, 2); 
    public Blueprint IronChestplateBLP = new Blueprint("IronChest", 1, "Iron Ingot", "", 4, 15, 2); 
    public Blueprint IronLeggsBLP = new Blueprint("IronLegs", 1, "Iron Ingot", "", 3, 10, 2);
    public Blueprint WoodShieldBLP = new Blueprint("WoodShield", 1, "Plank", "Skin", 6, 2, 2);
    public Blueprint IronShieldBLP = new Blueprint("Iron Shield", 1, "Iron Ingot", "", 5, 20, 2);


    #endregion


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
        //RefreshNeededItems();
    }

   

    // Start is called before the first frame update
    void Start()
    {
        #region ----------   Initialize UI   ----------

        isOpen = false;
        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenCategory("tools"); });

        attackBTN = craftingScreenUI.transform.Find("AttackButton").GetComponent<Button>();
        attackBTN.onClick.AddListener(delegate { OpenCategory("attack"); });

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(delegate { OpenCategory("survival"); });

        utilityBTN = craftingScreenUI.transform.Find("UtilityButton").GetComponent<Button>();
        utilityBTN.onClick.AddListener(delegate { OpenCategory("utility"); });

        buildBTN = craftingScreenUI.transform.Find("BuildButton").GetComponent<Button>();
        buildBTN.onClick.AddListener(delegate { OpenCategory("build"); });

        furnitureBTN = craftingScreenUI.transform.Find("FurnitureButton").GetComponent<Button>();
        furnitureBTN.onClick.AddListener(delegate { OpenCategory("furniture"); });

        armorBTN = craftingScreenUI.transform.Find("ArmorButton").GetComponent<Button>();
        armorBTN.onClick.AddListener(delegate { OpenCategory("armor"); });


        #endregion


        #region ----------   Initialize Requirement Texts and Buttons   ----------

        //Stone_Axe
        Stone_AxeReq1 = toolsScreenUI.transform.Find("Stone_Axe").transform.Find("req1").GetComponent<Text>();
        Stone_AxeReq2 = toolsScreenUI.transform.Find("Stone_Axe").transform.Find("req2").GetComponent<Text>();

        craftStone_AxeBTN = toolsScreenUI.transform.Find("Stone_Axe").transform.Find("CraftBTN").GetComponent<Button>();
        craftStone_AxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });

        //Iron_Axe
        Iron_AxeRe1 = toolsScreenUI.transform.Find("Iron Axe").transform.Find("req1").GetComponent<Text>();
        Iron_AxeReq2 = toolsScreenUI.transform.Find("Iron Axe").transform.Find("req2").GetComponent<Text>();

        craftIron_AxeBTN = toolsScreenUI.transform.Find("Iron Axe").transform.Find("CraftBTN").GetComponent<Button>();
        craftIron_AxeBTN.onClick.AddListener(delegate { CraftAnyItem(Iron_AxeBLP); });

        //Stone_Pickaxe
        Stone_PickaxeReq1 = toolsScreenUI.transform.Find("Stone_Pickaxe").transform.Find("req1").GetComponent<Text>();
        Stone_PickaxeReq2 = toolsScreenUI.transform.Find("Stone_Pickaxe").transform.Find("req2").GetComponent<Text>();

        craftStone_PickaxeBTN = toolsScreenUI.transform.Find("Stone_Pickaxe").transform.Find("CraftBTN").GetComponent<Button>();
        craftStone_PickaxeBTN.onClick.AddListener(delegate { CraftAnyItem(Stone_PickaxeBLP); });

        //Iron_Pickaxe
        Iron_PickaxeReq1 = toolsScreenUI.transform.Find("Iron Pickaxe").transform.Find("req1").GetComponent<Text>();
        Iron_PickaxeReq2 = toolsScreenUI.transform.Find("Iron Pickaxe").transform.Find("req2").GetComponent<Text>();

        craftIron_PickaxeBTN = toolsScreenUI.transform.Find("Iron Pickaxe").transform.Find("CraftBTN").GetComponent<Button>();
        craftIron_PickaxeBTN.onClick.AddListener(delegate { CraftAnyItem(Iron_PickaxeBLP); });


        //Stone_Knife
        Stone_KnifeReq1 = attackScreenUI.transform.Find("Stone_Knife").transform.Find("req1").GetComponent<Text>();
        Stone_KnifeReq2 = attackScreenUI.transform.Find("Stone_Knife").transform.Find("req2").GetComponent<Text>();
        craftStone_KnifeBTN = attackScreenUI.transform.Find("Stone_Knife").transform.Find("CraftBTN").GetComponent<Button>();
        craftStone_KnifeBTN.onClick.AddListener(delegate { CraftAnyItem(Stone_KnifeBLP); });

        //Iron_Knife
        Iron_KnifeReq1 = attackScreenUI.transform.Find("Iron Knife").transform.Find("req1").GetComponent<Text>();
        Iron_KnifeReq2 = attackScreenUI.transform.Find("Iron Knife").transform.Find("req2").GetComponent<Text>();
        craftIron_KnifeBTN = attackScreenUI.transform.Find("Iron Knife").transform.Find("CraftBTN").GetComponent<Button>();
        craftIron_KnifeBTN.onClick.AddListener(delegate { CraftAnyItem(Iron_KnifeBLP); });

        //SmallChest
        SmallChestReq1 = furnitureScreenUI.transform.Find("StorageBox").transform.Find("req1").GetComponent<Text>();
        SmallChestReq2 = furnitureScreenUI.transform.Find("StorageBox").transform.Find("req2").GetComponent<Text>();
        craftSmallChestBTN = furnitureScreenUI.transform.Find("StorageBox").transform.Find("CraftBTN").GetComponent<Button>();
        craftSmallChestBTN.onClick.AddListener(delegate { CraftAnyItem(SmallChestBLP); });

        //Campfire
        CampfireReq1 = utilityScreenUI.transform.Find("Campfire").transform.Find("req1").GetComponent<Text>();
        CampfireReq2 = utilityScreenUI.transform.Find("Campfire").transform.Find("req2").GetComponent<Text>();
        craftCampfireBTN = utilityScreenUI.transform.Find("Campfire").transform.Find("CraftBTN").GetComponent<Button>();
        craftCampfireBTN.onClick.AddListener(delegate { CraftAnyItem(CampfireBLP); });

        //Furnace
        FurnaceReq1 = utilityScreenUI.transform.Find("Furnace").transform.Find("req1").GetComponent<Text>();
        FurnaceReq2 = utilityScreenUI.transform.Find("Furnace").transform.Find("req2").GetComponent<Text>();
        craftFurnaceBTN = utilityScreenUI.transform.Find("Furnace").transform.Find("CraftBTN").GetComponent<Button>();
        craftFurnaceBTN.onClick.AddListener(delegate { CraftAnyItem(FurnaceBLP); });
        

        //Plank
        PlankReq1 = utilityScreenUI.transform.Find("Plank").transform.Find("req1").GetComponent<Text>();
        PlankReq2 = utilityScreenUI.transform.Find("Plank").transform.Find("req2").GetComponent<Text>();

        craftPlankBTN = utilityScreenUI.transform.Find("Plank").transform.Find("CraftBTN").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(PlankBLP); });

        //Foundation
        FoundationReq1 = buildScreenUI.transform.Find("Foundation").transform.Find("req1").GetComponent<Text>();

        craftFoundationBTN = buildScreenUI.transform.Find("Foundation").transform.Find("CraftBTN").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });

        //Wall
        WallReq1 = buildScreenUI.transform.Find("Wall").transform.Find("req1").GetComponent<Text>();

        craftWallBTN = buildScreenUI.transform.Find("Wall").transform.Find("CraftBTN").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });

        //Door
        DoorReq1 = buildScreenUI.transform.Find("Door").transform.Find("req1").GetComponent<Text>();

        craftDoorBTN = buildScreenUI.transform.Find("Door").transform.Find("CraftBTN").GetComponent<Button>();
        craftDoorBTN.onClick.AddListener(delegate { CraftAnyItem(DoorBLP); });

        //Window
        WindowReq1 = buildScreenUI.transform.Find("Window").transform.Find("req1").GetComponent<Text>();

        craftWindowBTN = buildScreenUI.transform.Find("Window").transform.Find("CraftBTN").GetComponent<Button>();
        craftWindowBTN.onClick.AddListener(delegate { CraftAnyItem(WindowBLP); });

        //Stairs
        StairsReq1 = buildScreenUI.transform.Find("Stairs").transform.Find("req1").GetComponent<Text>();

        craftStairsBTN = buildScreenUI.transform.Find("Stairs").transform.Find("CraftBTN").GetComponent<Button>();
        craftStairsBTN.onClick.AddListener(delegate { CraftAnyItem(StairsBLP); });

        //StoneShovel
        StoneShovelReq1 = toolsScreenUI.transform.Find("Stone_Shovel").transform.Find("req1").GetComponent<Text>();
        StoneShovelReq2 = toolsScreenUI.transform.Find("Stone_Shovel").transform.Find("req2").GetComponent<Text>();
        craftStoneShovelBTN = toolsScreenUI.transform.Find("Stone_Shovel").transform.Find("CraftBTN").GetComponent<Button>();
        craftStoneShovelBTN.onClick.AddListener(delegate { CraftAnyItem(StoneShovelBLP); });

        //IronShovel
        IronShovelReq1 = toolsScreenUI.transform.Find("Iron Shovel").transform.Find("req1").GetComponent<Text>();
        IronShovelReq2 = toolsScreenUI.transform.Find("Iron Shovel").transform.Find("req2").GetComponent<Text>();
        craftIronShovelBTN = toolsScreenUI.transform.Find("Iron Shovel").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronShovelBTN.onClick.AddListener(delegate { CraftAnyItem(IronShovelBLP); });

        //LeatherBottle
        LeatherBottleReq1 = survivalScreenUI.transform.Find("LeatherBottle").transform.Find("req1").GetComponent<Text>();
        LeatherBottleReq2 = survivalScreenUI.transform.Find("LeatherBottle").transform.Find("req2").GetComponent<Text>();
        craftLeatherBottleBTN = survivalScreenUI.transform.Find("LeatherBottle").transform.Find("CraftBTN").GetComponent<Button>();
        craftLeatherBottleBTN.onClick.AddListener(delegate { CraftAnyItem(LeatherBottleBLP); });


        //SmallBackpack
        SmallBackpackReq1 = survivalScreenUI.transform.Find("SmallBackpack").transform.Find("req1").GetComponent<Text>();
        SmallBackpackReq2 = survivalScreenUI.transform.Find("SmallBackpack").transform.Find("req2").GetComponent<Text>();
        craftSmallBackpackBTN = survivalScreenUI.transform.Find("SmallBackpack").transform.Find("CraftBTN").GetComponent<Button>();
        craftSmallBackpackBTN.onClick.AddListener(delegate { CraftAnyItem(SmallBackpackBLP); });

        //BigBackpack
        BigBackpackReq1 = survivalScreenUI.transform.Find("BigBackpack").transform.Find("req1").GetComponent<Text>();
        BigBackpackReq2 = survivalScreenUI.transform.Find("BigBackpack").transform.Find("req2").GetComponent<Text>();
        craftBigBackpackBTN = survivalScreenUI.transform.Find("BigBackpack").transform.Find("CraftBTN").GetComponent<Button>();
        craftBigBackpackBTN.onClick.AddListener(delegate { CraftAnyItem(BigBackpackBLP); });


        //WoodenTorch
        WoodenTorchReq1 = survivalScreenUI.transform.Find("Torch").transform.Find("req1").GetComponent<Text>();
        WoodenTorchReq2 = survivalScreenUI.transform.Find("Torch").transform.Find("req2").GetComponent<Text>();
        craftWoodenTorchBTN = survivalScreenUI.transform.Find("Torch").transform.Find("CraftBTN").GetComponent<Button>();
        craftWoodenTorchBTN.onClick.AddListener(delegate { CraftAnyItem(WoodenTorchBLP); });

        //Bandage
        BandageReq1 = survivalScreenUI.transform.Find("Bandage").transform.Find("req1").GetComponent<Text>();
        craftBandageBTN = survivalScreenUI.transform.Find("Bandage").transform.Find("CraftBTN").GetComponent<Button>();
        craftBandageBTN.onClick.AddListener(delegate { CraftAnyItem(BandageBLP); });

        //Anvil
        AnvilReq1 = utilityScreenUI.transform.Find("Anvil").transform.Find("req1").GetComponent<Text>();
        craftAnvilBTN = utilityScreenUI.transform.Find("Anvil").transform.Find("CraftBTN").GetComponent<Button>();
        craftAnvilBTN.onClick.AddListener(delegate { CraftAnyItem(AnvilBLP); });

        //Hammer 
        HammerReq1 = utilityScreenUI.transform.Find("Hammer").transform.Find("req1").GetComponent<Text>();
        HammerReq2 = utilityScreenUI.transform.Find("Hammer").transform.Find("req2").GetComponent<Text>();
        craftHammerBTN = utilityScreenUI.transform.Find("Hammer").transform.Find("CraftBTN").GetComponent<Button>();
        craftHammerBTN.onClick.AddListener(delegate { CraftAnyItem(HammerBLP); });

        //IronHelmet
        IronHelmetReq1 = armorScreenUI.transform.Find("IronHelmet").transform.Find("req1").GetComponent<Text>();
        IronHelmetReq2 = armorScreenUI.transform.Find("IronHelmet").transform.Find("req2").GetComponent<Text>();
        craftIronHelmetBTN = armorScreenUI.transform.Find("IronHelmet").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronHelmetBTN.onClick.AddListener(delegate { CraftAnyItem(IronHelmetBLP); });

        //IronChestplate
        IronChestplateReq1 = armorScreenUI.transform.Find("IronChest").transform.Find("req1").GetComponent<Text>();
        IronChestplateReq2 = armorScreenUI.transform.Find("IronChest").transform.Find("req2").GetComponent<Text>();
        craftIronChestplateBTN = armorScreenUI.transform.Find("IronChest").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronChestplateBTN.onClick.AddListener(delegate { CraftAnyItem(IronChestplateBLP); });

        //IronLegs
        IronLeggsReq1 = armorScreenUI.transform.Find("IronLegs").transform.Find("req1").GetComponent<Text>();
        IronLeggsReq2 = armorScreenUI.transform.Find("IronLegs").transform.Find("req2").GetComponent<Text>();
        craftIronLeggsBTN = armorScreenUI.transform.Find("IronLegs").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronLeggsBTN.onClick.AddListener(delegate { CraftAnyItem(IronLeggsBLP); });


        //WoodShield
        WoodShieldReq1 = armorScreenUI.transform.Find("WoodShield").transform.Find("req1").GetComponent<Text>();
        WoodShieldReq2 = armorScreenUI.transform.Find("WoodShield").transform.Find("req2").GetComponent<Text>();
        craftWoodShieldBTN = armorScreenUI.transform.Find("WoodShield").transform.Find("CraftBTN").GetComponent<Button>();
        craftWoodShieldBTN.onClick.AddListener(delegate { CraftAnyItem(WoodShieldBLP); });

        //IronShield
        IronShieldReq1 = armorScreenUI.transform.Find("Iron Shield").transform.Find("req1").GetComponent<Text>();
        IronShieldReq2 = armorScreenUI.transform.Find("Iron Shield").transform.Find("req2").GetComponent<Text>();
        craftIronShieldBTN = armorScreenUI.transform.Find("Iron Shield").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronShieldBTN.onClick.AddListener(delegate { CraftAnyItem(IronShieldBLP); });

        //IronSword
        IronSwordReq1 = attackScreenUI.transform.Find("Iron Sword").transform.Find("req1").GetComponent<Text>();
        IronSwordReq2 = attackScreenUI.transform.Find("Iron Sword").transform.Find("req2").GetComponent<Text>();
        craftIronSwordBTN = attackScreenUI.transform.Find("Iron Sword").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronSwordBTN.onClick.AddListener(delegate { CraftAnyItem(IronSwordBLP); });
        

        //StoneSpear
        StoneSpearReq1 = attackScreenUI.transform.Find("StoneSpear").transform.Find("req1").GetComponent<Text>();
        StoneSpearReq2 = attackScreenUI.transform.Find("StoneSpear").transform.Find("req2").GetComponent<Text>();
        craftStoneSpearBTN = attackScreenUI.transform.Find("StoneSpear").transform.Find("CraftBTN").GetComponent<Button>();
        craftStoneSpearBTN.onClick.AddListener(delegate { CraftAnyItem(StoneSpearBLP); });

        //IronSpear
        IronSpearReq1 = attackScreenUI.transform.Find("Iron Spear").transform.Find("req1").GetComponent<Text>();
        IronSpearReq2 = attackScreenUI.transform.Find("Iron Spear").transform.Find("req2").GetComponent<Text>();
        craftIronSpearBTN = attackScreenUI.transform.Find("Iron Spear").transform.Find("CraftBTN").GetComponent<Button>();
        craftIronSpearBTN.onClick.AddListener(delegate { CraftAnyItem(IronSpearBLP); });

        //Bed
        BedReq1 = furnitureScreenUI.transform.Find("Bed").transform.Find("req1").GetComponent<Text>();
        BedReq2 = furnitureScreenUI.transform.Find("Bed").transform.Find("req2").GetComponent<Text>();
        craftBedBTN = furnitureScreenUI.transform.Find("Bed").transform.Find("CraftBTN").GetComponent<Button>();
        craftBedBTN.onClick.AddListener(delegate { CraftAnyItem(BedBLP); });

        //wallTorch
        //wallTorchReq1 = furnitureScreenUI.transform.Find("WallTorch").transform.Find("req1").GetComponent<Text>();
        //wallTorchBTN = furnitureScreenUI.transform.Find("WallTorch").transform.Find("CraftBTN").GetComponent<Button>();
        //wallTorchBTN.onClick.AddListener(delegate { CraftAnyItem(wallTorchBLP); });

        //Bow
        BowReq1 = attackScreenUI.transform.Find("Bow").transform.Find("req1").GetComponent<Text>();
        BowReq2 = attackScreenUI.transform.Find("Bow").transform.Find("req2").GetComponent<Text>();
        craftBowBTN = attackScreenUI.transform.Find("Bow").transform.Find("CraftBTN").GetComponent<Button>();
        craftBowBTN.onClick.AddListener(delegate { CraftAnyItem(BowBLP); });

        //Arrow
        ArrowReq1 = survivalScreenUI.transform.Find("Arrow").transform.Find("req1").GetComponent<Text>();
        ArrowReq2 = survivalScreenUI.transform.Find("Arrow").transform.Find("req2").GetComponent<Text>();
        craftArrowBTN = survivalScreenUI.transform.Find("Arrow").transform.Find("CraftBTN").GetComponent<Button>();
        craftArrowBTN.onClick.AddListener(delegate { CraftAnyItem(ArrowBLP); });

        //Well
        WellReq1 = utilityScreenUI.transform.Find("Well").transform.Find("req1").GetComponent<Text>();
        WellReq2 = utilityScreenUI.transform.Find("Well").transform.Find("req2").GetComponent<Text>();
        craftWellBTN = utilityScreenUI.transform.Find("Well").transform.Find("CraftBTN").GetComponent<Button>();
        craftWellBTN.onClick.AddListener(delegate { CraftAnyItem(WellBLP); });


        //Fence
        FenceReq1 = buildScreenUI.transform.Find("Fence").transform.Find("req1").GetComponent<Text>();
        craftFenceBTN = buildScreenUI.transform.Find("Fence").transform.Find("CraftBTN").GetComponent<Button>();
        craftFenceBTN.onClick.AddListener(delegate { CraftAnyItem(FenceBLP); });

        //Gate
        GateReq1 = buildScreenUI.transform.Find("Gate").transform.Find("req1").GetComponent<Text>();
        craftGateBNT = buildScreenUI.transform.Find("Gate").transform.Find("CraftBTN").GetComponent<Button>();
        craftGateBNT.onClick.AddListener(delegate { CraftAnyItem(GateBPL); });

        //Candle
        CandleReq1 = furnitureScreenUI.transform.Find("Candle").transform.Find("req1").GetComponent<Text>();
        CandleReq2 = furnitureScreenUI.transform.Find("Candle").transform.Find("req2").GetComponent<Text>();
        craftCandleBTN = furnitureScreenUI.transform.Find("Candle").transform.Find("CraftBTN").GetComponent<Button>();
        craftCandleBTN.onClick.AddListener(delegate { CraftAnyItem(CandleBLP); });

        #endregion
    }

    #region ----------   open categories   ----------
    public void CloseAllCategories()
    {
        OpenCategory("nothing");

        isOpen = false;

        CursorManager.Instance.LockCursor();

        
    }

    public void returnToMainCategory()
    {
        OpenCategory("main");

    }

    void OpenCategory(string category)
    {
        craftingScreenUI.SetActive(category=="main");
        toolsScreenUI.SetActive(category == "tools");
        attackScreenUI.SetActive(category == "attack");
        survivalScreenUI.SetActive(category == "survival");
        utilityScreenUI.SetActive(category == "utility");
        buildScreenUI.SetActive(category == "build");
        armorScreenUI.SetActive(category == "armor");
        furnitureScreenUI.SetActive(category == "furniture");
    }


    
    #endregion 

    void CraftAnyItem(Blueprint blueprintToCraft)
    {

        // Check if there is an empty slot in the inventory
        if (InventorySystem.Instance.CkeckSlotsAvailable(blueprintToCraft.numOfItemsCrafted) && isCrafting==false)
        {
            // Remove the required items from the inventory
            if (blueprintToCraft.numOfRequirements == 1)
            {
                InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1Amount);
            }
            else if (blueprintToCraft.numOfRequirements == 2)
            {
                Debug.Log("Removing Req1: " + blueprintToCraft.Req1 + " Req1Amount: " + blueprintToCraft.Req1Amount);
                InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1Amount);
                // Debug.Log(blueprintToCraft.Req2Amount);
                if(blueprintToCraft.Req2 != "")
                {
                    Debug.Log("Removing Req2: " + blueprintToCraft.Req2 + " Req2Amount: " + blueprintToCraft.Req2Amount);
                    InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2Amount);
                }
                else
                {
                    Debug.Log(blueprintToCraft.Req2Amount);
                    //we have to deal with armor, so we have to decrease the health of the hammer
                    EquipSystem.Instance.decreaseWeaponHealth(blueprintToCraft.Req2Amount);
                }
                
            }
            calculate();

            // Add the crafted item to the inventory
            StartCoroutine(CraftItemsWithTime(blueprintToCraft));
        }
        else
        {
            if (isCrafting == true)
            {
                Debug.Log("The player is crafting another item at the moment");
            }
            
            if(!InventorySystem.Instance.CkeckSlotsAvailable(blueprintToCraft.numOfItemsCrafted))
            {
                EquipSystem.Instance.PopUpMessage("Not enough space in the inventory");
            }
            // Optionally, you could show a UI message here to inform the player
        }

    }


    
    public IEnumerator CraftItemsWithTime(Blueprint blueprintToCraft)
    {
        if(blueprintToCraft.itemName.Contains("Iron"))
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.creatingIronItems);
        }
        else
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.creating_planks);
        }
        
        isCrafting = true;
        yield return new WaitForSeconds(6);
        for (int i = 0; i < blueprintToCraft.numOfItemsCrafted; i++)
        {
            InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName);
        }

        if (blueprintToCraft.itemName.Contains("Iron"))
        {
            SoundManager.Instance.StopSound(SoundManager.Instance.creatingIronItems);
        }
        else
        {
            SoundManager.Instance.StopSound(SoundManager.Instance.creating_planks);
        }

        isCrafting = false;
        // Recalculate the inventory list and refresh UI
        StartCoroutine(calculate());
    }

   

    public IEnumerator calculate()
    {
        yield return 0;
        
        InventorySystem.Instance.ReCalculateList();
        RefreshNeededItems();

    }

    // Update is called once per frame
    void Update()
    {
        //RefreshNeededItems();
        if (Input.GetKeyDown(KeyCode.C) && !isOpen &&
            !ConstructionManager.Instance.inConstructionMode &&
            !PlacementSystem.Instance.inPlacementMode)
        {
            craftingScreenUI.SetActive(true);
            isOpen = true;

            CursorManager.Instance.FreeCursor();
            
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            CloseAllCategories();
        }

        if (EquipSystem.Instance.GetWeaponName().Contains("Hammer"))
        {
          
            hammerHealth = EquipSystem.Instance.GetWeaponHealth();
            //Debug.Log("Hammer health: " + hammerHealth);
        }
        else
        {
            hammerHealth = 0;
        }
        IronHelmetReq2.text = "10% Hammer[" + hammerHealth + "%]";
        IronChestplateReq2.text = "15% Hammer[" + hammerHealth + "%]";
        IronLeggsReq2.text = "10% Hammer[" + hammerHealth + "%]";
        IronShieldReq2.text = "20% Hammer[" + hammerHealth + "%]";


        if(EquipSystem.Instance.GetWeaponName().Contains("Knife"))
        {
            knifeHealth = EquipSystem.Instance.GetWeaponHealth();
        }
        else
        {
            
            knifeHealth = 0;
        }
       

        PlankReq2.text = "10% Knife[" + knifeHealth + "%]";

        RefreshNeededItems();

    }

    public void RefreshNeededItems()
    {
        int stone_count = 0, stick_count = 0;
        int ironIngot_count = 0, log_count = 0;
        int plank_count = 0, skin_count = 0, fat_count=0;
        inventoryItemList = InventorySystem.Instance.itemList;

        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Stone":
                    stone_count++;
                    break;
                case "Stick":
                    stick_count++;
                    break;
                case "Iron Ingot":
                    ironIngot_count++;
                    break;
                case "Log":
                    log_count++;
                    break;
                case "Plank":
                    plank_count++;
                    break;
                case "Skin":
                    skin_count++;
                    break;
                case "Fat":
                    fat_count++;
                    break;
            }
        }


        #region ----------   Update Requirement Texts   ----------

        //Stone_Axe   
        Stone_AxeReq1.text = "3 Stone [" + stone_count + "]";
        Stone_AxeReq2.text = "3 Stick [" + stick_count + "]";

        //Iron_Axe
        Iron_AxeRe1.text = "4 Iron Ingot[" + ironIngot_count + "]";
        Iron_AxeReq2.text = "3 Plank [" + plank_count + "]";

        //Stone_Pickaxe
        Stone_PickaxeReq1.text = "4 Stone [" + stone_count + "]";
        Stone_PickaxeReq2.text = "5 Stick [" + stick_count + "]";

        //Iron_Pickaxe
        Iron_PickaxeReq1.text = "5 Iron Ingot[" + ironIngot_count + "]";
        Iron_PickaxeReq2.text = "3 Plank [" + plank_count + "]";

        //Stone_Knife
        Stone_KnifeReq1.text = "2 Stone [" + stone_count + "]";
        Stone_KnifeReq2.text = "2 Stick [" + stick_count + "]";

        //Iron_Knife
        Iron_KnifeReq1.text = "2 Iron Ingot[" + ironIngot_count + "]";
        Iron_KnifeReq2.text = "1 Log [" + log_count + "]";

        //SmallChest
        SmallChestReq1.text = "3 Plank [" + plank_count + "]";
        SmallChestReq2.text = "3 Iron Ingot[" + ironIngot_count + "]";

        //Campfire
        CampfireReq1.text = "8 Stone [" + stone_count + "]";
        CampfireReq2.text = "7 Stick [" + stick_count + "]";

        //Furnace
        FurnaceReq1.text = "20 Stone [" + stone_count + "]";
        FurnaceReq2.text = "2 Iron Ingot[" + ironIngot_count + "]";

        //Plank
        PlankReq1.text = "1 Log [" + log_count + "]";

        //Foundation
        FoundationReq1.text = "4 Plank [" + plank_count + "]";

        //Wall
        WallReq1.text = "3 Plank [" + plank_count + "]";

        //Door
        DoorReq1.text = "5 Plank [" + plank_count + "]";

        //Window
        WindowReq1.text = "3 Plank [" + plank_count + "]";

        //Stairs
        StairsReq1.text = "7 Plank [" + plank_count + "]";

        //StoneShovel
        StoneShovelReq1.text = "6 Stone [" + stone_count + "]";
        StoneShovelReq2.text = "5 Stick [" + stick_count + "]";

        //IronShovel
        IronShovelReq1.text = "4 Iron Ingot[" + ironIngot_count + "]";
        IronShovelReq2.text = "3 Plank [" + plank_count + "]";

        //LeatherBottle
        LeatherBottleReq1.text = "2 Stick [" + stick_count + "]";
        LeatherBottleReq2.text = "5 Skin [" + skin_count + "]";
        
        //SmallBackpack
        SmallBackpackReq1.text = "4 Stick [" + stick_count + "]";
        SmallBackpackReq2.text = "6 Skin [" + skin_count + "]";

        //BigBackpack
        BigBackpackReq1.text = "3 Plank [" + plank_count + "]";
        BigBackpackReq2.text = "9 Skin [" + skin_count + "]";

        //WoodenTorch
        WoodenTorchReq1.text = "2 Stick [" + stick_count + "]";
        WoodenTorchReq2.text = "3 Skin [" + skin_count + "]";

        //Bandage
        BandageReq1.text = "1 Skin [" + skin_count + "]";

        //Anvil
        AnvilReq1.text = "10 Iron Ingot[" + ironIngot_count + "]";

        //Hammer
        HammerReq1.text = "1 Plank [" + plank_count + "]";
        HammerReq2.text = "4 Iron Ingot[" + ironIngot_count + "]";



        //THE SECOND REQUEST IS INSIDE UPDATE FUNCTIN BECAUSE THE HAMMER DOES NOT DEPENT ON THE CRAFTING SYSTEM
        //IronHelmet
        IronHelmetReq1.text = "3 Iron Ingot[" + ironIngot_count + "]";
       
        //IronChestplate
        IronChestplateReq1.text = "4 Iron Ingot[" + ironIngot_count + "]";

        //IronLegs
        IronLeggsReq1.text = "3 Iron Ingot[" + ironIngot_count + "]";

        //WoodShield
        WoodShieldReq1.text = "6 Plank [" + plank_count + "]";
        WoodShieldReq2.text = "2 Skin [" + skin_count + "]";

        //IronShield
        IronShieldReq1.text = "5 Iron Ingot[" + ironIngot_count + "]";

        //IronSword
        IronSwordReq1.text = "5 Iron Ingot[" + ironIngot_count + "]";
        IronSwordReq2.text = "2 Skin [" + skin_count + "]";

        //StoneSpear
        StoneSpearReq1.text = "3 Plank [" + plank_count + "]";
        StoneSpearReq2.text = "3 Stone [" + stone_count + "]";

        //IronSpear
        IronSpearReq1.text = "3 Plank [" + plank_count + "]";
        IronSpearReq2.text = "4 Iron Ingot[" + ironIngot_count + "]";
      
        //Bed
        BedReq1.text = "8 Plank [" + plank_count + "]";
        BedReq2.text = "6 Skin [" + skin_count + "]";


        //Bow
        BowReq1.text = "4 Plank [" + plank_count + "]";
        BowReq2.text = "4 Skin [" + skin_count + "]";

        //Arrow
        ArrowReq1.text = "2 Stick [" + stick_count + "]";
        ArrowReq2.text = "1 Iron Ingot[" + ironIngot_count + "]";
        #endregion

        //Well
        WellReq1.text = "10 Stone [" + stone_count + "]";
        WellReq2.text = "5 Plank [" + plank_count + "]";


        //Fence 
        FenceReq1.text = "4 Plank [" + plank_count + "]";

        //Gate
        GateReq1.text = "3 Plank [" + plank_count + "]";

        //Candle
        CandleReq1.text = "1 Fat [" + fat_count + "]";
        CandleReq2.text = "1 Iron Ingot[" + ironIngot_count + "]";
        //modify all the below conditions with conditions like this one: craftStone_AxeBTN.gameObject.SetActive(stone_count>=3 && stick_count>=3);






        #region ----------   Update Buttons   ----------

        //change to 3 and 3
        craftStone_AxeBTN.gameObject.SetActive(stone_count >= 3 && stick_count >= 3);

        //CHANGE TO 4 AND 7
        craftIron_AxeBTN.gameObject.SetActive(ironIngot_count >= 4 && plank_count >= 3);

        //CHANGE TO 4 AND 5
        craftStone_PickaxeBTN.gameObject.SetActive(stone_count >= 4 && stick_count >= 5);

        //CHANGE TO 5 AND 9
        craftIron_PickaxeBTN.gameObject.SetActive(ironIngot_count >= 5 && plank_count >= 3);

        ///////////////////////////////////////////////////////////////
        craftPlankBTN.gameObject.SetActive(log_count >= 1 && knifeHealth>=10);

        craftFoundationBTN.gameObject.SetActive(plank_count >= 4);

        craftWallBTN.gameObject.SetActive(plank_count >= 3);

        craftDoorBTN.gameObject.SetActive(plank_count >= 5);

        craftWindowBTN.gameObject.SetActive(plank_count >= 3);

        craftStairsBTN.gameObject.SetActive(plank_count >= 7);

        craftStone_KnifeBTN.gameObject.SetActive(stone_count >= 2 && stick_count >= 2);

        craftIron_KnifeBTN.gameObject.SetActive(ironIngot_count >= 2 && log_count >= 1);

        craftSmallChestBTN.gameObject.SetActive(plank_count >= 3 && ironIngot_count >= 3);

        craftCampfireBTN.gameObject.SetActive(stone_count >= 8 && stick_count >= 7);

        //20 and 2
        craftFurnaceBTN.gameObject.SetActive(stone_count >= 20 && ironIngot_count >= 2);

        // 6 and 5
        craftStoneShovelBTN.gameObject.SetActive(stone_count >= 6 && stick_count >= 5);

        // 4 and 3
        craftIronShovelBTN.gameObject.SetActive(ironIngot_count >= 4 && plank_count >= 3);

        //2 stick and 4 skin
        craftLeatherBottleBTN.gameObject.SetActive(stick_count >= 2 && skin_count >= 4);

        //4 stick and 6 skin
        craftSmallBackpackBTN.gameObject.SetActive(stick_count >= 4 && skin_count >= 6);

        //2 and 3 skin
        craftWoodenTorchBTN.gameObject.SetActive(stick_count >= 2 && skin_count >= 3);

        //3 and 9 skin
        craftBigBackpackBTN.gameObject.SetActive(plank_count >= 3 && skin_count >= 9);

        //1 skin
        craftBandageBTN.gameObject.SetActive(skin_count >= 1);

        //10 iron ingot
        craftAnvilBTN.gameObject.SetActive(ironIngot_count >= 10);

        //1 plank and 4 iron ingot
        craftHammerBTN.gameObject.SetActive(plank_count >= 1 && ironIngot_count >= 4);

        //3 iron ingot
        craftIronHelmetBTN.gameObject.SetActive(ironIngot_count >= 3 && hammerHealth >= 10);

        //4 iron ingot
        craftIronChestplateBTN.gameObject.SetActive(ironIngot_count >= 4 && hammerHealth >= 15);

        //3 iron ingot
        craftIronLeggsBTN.gameObject.SetActive(ironIngot_count >= 3 && hammerHealth >= 10);

        craftWoodShieldBTN.gameObject.SetActive(plank_count >= 6 && skin_count >= 2);

        craftIronShieldBTN.gameObject.SetActive(ironIngot_count >= 5 && hammerHealth >= 20);

        craftIronSwordBTN.gameObject.SetActive(ironIngot_count >= 5 && skin_count >= 2);

        craftStoneSpearBTN.gameObject.SetActive(plank_count >= 3 && stone_count >= 3);

        craftIronSpearBTN.gameObject.SetActive(plank_count >= 3 && ironIngot_count >= 4);

        craftBedBTN.gameObject.SetActive(plank_count >= 8 && skin_count >= 6);

        //wallTorchBTN.gameObject.SetActive(ironIngot_count >= 4);

        craftBowBTN.gameObject.SetActive(plank_count >= 4 && skin_count >= 4);

        craftArrowBTN.gameObject.SetActive(stick_count >= 2 && ironIngot_count >= 1);

        craftWellBTN.gameObject.SetActive(stone_count>=10 && plank_count >= 5);

        craftGateBNT.gameObject.SetActive(plank_count >= 3);

        craftFenceBTN.gameObject.SetActive(plank_count >= 4);

        craftCandleBTN.gameObject.SetActive(fat_count >= 1 && ironIngot_count >= 1);
        #endregion
    }
}
