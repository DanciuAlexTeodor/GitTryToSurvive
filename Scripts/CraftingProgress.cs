using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CraftingProgress : MonoBehaviour
{

    //create singleton
    public static CraftingProgress Instance { get; set; }

    public Slider craftSlider; // Reference to the slider
    //public Button craftButton; // Reference to the craft button

    private bool isCrafting = false;

    public void Start()
    {
        //craftButton.onClick.AddListener(StartCrafting);
        craftSlider.gameObject.SetActive(false); // Hide the slider initially
        Start();
        
    }

    public void StartCrafting()
    {
        if (!isCrafting)
        {
            isCrafting = true;
            craftSlider.value = 0; // Reset the slider
            craftSlider.gameObject.SetActive(true); // Show the slider
            StartCoroutine(FillSlider());
        }
    }

    IEnumerator FillSlider()
    {
        float duration = 9f; // Crafting time
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            craftSlider.value = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }

        // Crafting completed
        isCrafting = false;
        craftSlider.gameObject.SetActive(false); // Hide the slider again
        AddItemToInventory();
    }

    void AddItemToInventory()
    {
        // Implement your logic to add the crafted item to the inventory
        Debug.Log("Item added to inventory!");
    }
}
