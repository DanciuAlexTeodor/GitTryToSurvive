using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Degree symbol: °  
//A year has 4 seasons: Spring, Summer, Autumn, Winter

//Each season has 30 days

//Medium temperature is 15 degrees celsius
// Summer temperature is 25 degrees celsius
// Winter temperature is 0 degrees celsius
// Spring and Autumn temperature is 15 degrees celsius

//for each day there will be a medium temperature based on the time of season, the exact temperature will be calculated based on this
// medium temperature and the current day of the season and the time of day (morning, afternoon, evening, night)(with a random interval [-10,10])

//The temperature will be displayed in the UI



public class TemperatureSystem : MonoBehaviour
{
    public static TemperatureSystem Instance { get; set; }

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

    public TextMeshProUGUI temperature;
    public int mediumTemperaturePerSeason;
    public string currentSeason;
    public int partOfDayIndex=0;
    //there are 9 parts of the day       //0  1  2  3  4  5   6   7   8
    //give me the temperature for each hour of the day

    List<int> partOfDay = new List<int> {
        -2,    // 8 AM: Approaching medium temperature
        0,  // 9 AM: Morning, cooler
        2,  // 10 AM: Morning, warming up
        4,   // 11 AM: Closer to medium temperature
        6,   // 12 PM: Starting to peak
        10,   // 1 PM: Peak temperature of the day
        12,   // 2 PM: Warmest part of the day
        10,   // 3 PM: Slightly cooling
        6,   // 4 PM: Late afternoon, cooling down
        3,   // 5 PM: Evening, medium temperature
        -1,  // 6 PM: Cooling down
        -1,  // 7 PM: Night cooling
        -3,  // 8 PM: Cooler as night sets in
        -5,  // 9 PM: Cold night
        -9,  // 10 PM: Getting colder
        -11,  // 11 PM: Very cold night
        -11, // 12 AM: Coldest point of the night
        -11, // 1 AM: Still very cold
        -10,  // 2 AM: Cold, starting to rise soon
        -10,  // 3 AM: Gradually rising
        -10,  // 4 AM: Pre-dawn cold
        -9,  // 5 AM: Early dawn, starting to warm
        -7,  // 6 AM: Morning warming up
        -6  // 7 AM: Continued warming
    };

    public void ChangeTemperatureBasedOnSeason()
    {
        currentSeason = TimeManager.Instance.currentSeason.ToString();

        switch (currentSeason)
        {
            case "Spring":
                mediumTemperaturePerSeason = 17; //fac foc o data pe zi (special noaptea) - 40 minute
                break;
            case "Summer":
                mediumTemperaturePerSeason = 23;
                break;
            case "Autumn":
                mediumTemperaturePerSeason = 15;
                break;
            case "Winter":
                mediumTemperaturePerSeason = 10;
                break;
        }

    }

    public void ChangeTemperatureBasedOnHourOfDay()
    {
       
        //the temperature will be chosen from [-10,10] interval based on the part of the day
        //morning: -3 (with a random interval of [-3,+3], afternoon: 7 [-3,3], evening: 3 [-3,3], night -7: [-3,3]
        

        int randomInterval = Random.Range(-1, 2); //random temperature variations
        //how can i make the randomInterval to increase and decrease gradually?

        int temp  = mediumTemperaturePerSeason + partOfDay[partOfDayIndex] + randomInterval;
        temperature.text = temp.ToString() + "°C";

        BodyTemperatureBar.Instance.ChangeBodyTemperatureBasedOnOutsideTemperature(temp);

        partOfDayIndex++;
        //Debug.Log("Part of day index: " + partOfDayIndex);

        if (partOfDayIndex == 24)
        {
            partOfDayIndex = 0;
        }

    }
}
