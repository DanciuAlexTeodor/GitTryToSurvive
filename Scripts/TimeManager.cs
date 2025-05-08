using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[System.Serializable]
public class TimeData
{
    public TimeManager.Season currentSeason;
    public TimeManager.DayOfWeek currentDayOfWeek;
    public int daysInCurrentSeason;
    public int dayInGame;
    public int currentHour;
    public float currentTimeOfDay;
    public int partOfDayIndex;


    public TimeData(TimeManager.Season _currentSeason, TimeManager.DayOfWeek _currentDayOfWeek,
        int _daysInCurrentSeason, int _dayInGame, float _currentTimeOfDay, int _partOfDayIndex)
    {
        this.currentSeason = _currentSeason;
        this.currentDayOfWeek = _currentDayOfWeek;
        this.daysInCurrentSeason = _daysInCurrentSeason;
        this.dayInGame = _dayInGame;
        this.currentTimeOfDay = _currentTimeOfDay;
        this.partOfDayIndex = _partOfDayIndex;
    }
}

public class TimeManager : MonoBehaviour
{ 

    public static TimeManager Instance { get; set; }
   
    public UnityEvent OnDayPass = new UnityEvent();

    public enum Season
    {
        Spring,
        Summer,
        Autumn,
        Winter
    }

    public enum DayOfWeek
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }

    public Season currentSeason = Season.Summer;
    public DayOfWeek currentDayOfWeek = DayOfWeek.Monday;

    private int daysPerSeason = 30;
    public int daysInCurrentSeason = 1;

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

    private void Start()
    {
        UpdateUI();
        TemperatureSystem.Instance.ChangeTemperatureBasedOnSeason();
    }

    public int dayInGame = 1;

    public TextMeshProUGUI dayUI;

    public void TriggerNextDay()
    {
        dayInGame++;
        daysInCurrentSeason++;


        currentDayOfWeek=GoToNextDayInWeek();
        if(daysInCurrentSeason > daysPerSeason)
        {
            daysInCurrentSeason = 1;
            currentSeason=GoToNextSeason();
        }
        UpdateUI();
        OnDayPass.Invoke();
    }

    private DayOfWeek GoToNextDayInWeek()
    {
        if (currentDayOfWeek == DayOfWeek.Sunday)
        {
            return DayOfWeek.Monday;
        }
        else
        {
            return currentDayOfWeek + 1;
        }
    }

    private Season GoToNextSeason()
    {
        TemperatureSystem.Instance.ChangeTemperatureBasedOnSeason();
        if (currentSeason == Season.Winter)
        {
            return Season.Spring;
        }
        else
        {
            return currentSeason + 1;
        }
        
    }

    private void UpdateUI()
    {
        dayUI.text = $"{currentDayOfWeek} {daysInCurrentSeason}\n{currentSeason}";
    }

    public TimeData GetTimeData()
    {
        int partOfDayIndex = TemperatureSystem.Instance.partOfDayIndex;
        float currentTimeOfDay = DayNightSystem.Instance.currentTimeOfDay;
        TimeData timeData = new TimeData(currentSeason, currentDayOfWeek, daysInCurrentSeason, dayInGame, currentTimeOfDay, partOfDayIndex);

        //Debug.Log("Day in game:" + dayInGame + "clock: ");

        return timeData;
        
    }

    public void LoadTimeData(TimeData data)
    {
        if(data == null)
        {
            return;
        }
        if(TemperatureSystem.Instance != null)
        {
            Debug.Log("Temperature system is not null: index is:" + data.partOfDayIndex);
            TemperatureSystem.Instance.partOfDayIndex = data.partOfDayIndex;
        }
        
        currentSeason = data.currentSeason;
        currentDayOfWeek = data.currentDayOfWeek;
        daysInCurrentSeason = data.daysInCurrentSeason;
        dayInGame = data.dayInGame;
        DayNightSystem.Instance.currentTimeOfDay = data.currentTimeOfDay;
        UpdateUI();
    }
}
