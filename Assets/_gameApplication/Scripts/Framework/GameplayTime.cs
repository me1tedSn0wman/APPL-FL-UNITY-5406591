using System;
using UnityEngine;
using Utils;

public enum DayPeriod { 
    Night,
    Breakfast,
    Lunch,
    Dinner,
    FreeTime,
}

[Serializable]
public struct DayPeriodTime {
    public DayPeriod dayPeriod;
    public Vector2 timePeriod;
}

/*
 1 sec == 1 min in game time
 */

public class GameplayTime : Soliton<GameplayTime>
{
    public static int TIME_PER_DAY = 1440;

    [SerializeField] private float timeFlow = 1.0f;
    [SerializeField] private DayPeriodTime[] DayPeriodTimeTable;
    [SerializeField] private Light skyLight;

    [SerializeField] private Material skyBoxMaterial;
    [SerializeField] private Gradient colorGradient;

    private float crntTime = 0.0f;

    private float normalisedTime {
        get {
            return Mathf.Clamp01(crntTime / TIME_PER_DAY);
        }
    }

    protected void Start()
    {
        crntTime = 0;
        skyBoxMaterial = RenderSettings.skybox;
    }

    protected void Update()
    {
        crntTime = (crntTime + Time.deltaTime * timeFlow) % (TIME_PER_DAY);
        UpdateSkybox();
    }

    public static string GetTimeString(int time)
    {
        time = time % TIME_PER_DAY;
        int hours = time / 60;
        int mins = time % 60;
        string time_str = string.Format("{0:d2}:{1:d2}", hours, mins);
        return time_str;
    }

    public string GetCurrentTimeString() {
        return GetTimeString(Mathf.FloorToInt(crntTime));
    }

    public void AddMinutesToTime(int value) {
        crntTime = (crntTime + value) % (TIME_PER_DAY);
    }

    public DayPeriod GetCurrentDayPerion() {
        return GetDayPeriod(crntTime);
    }

    public DayPeriod GetDayPeriod(float time) {
        for (int i = 0; i < DayPeriodTimeTable.Length; i++) {
            if (time >= DayPeriodTimeTable[i].timePeriod.x && time < DayPeriodTimeTable[i].timePeriod.y) {
                return DayPeriodTimeTable[i].dayPeriod;
            }
        }
        return DayPeriod.FreeTime;
    }

    public void UpdateSkybox() {
        skyBoxMaterial.SetColor("_Tint", colorGradient.Evaluate(normalisedTime));
        skyLight.color = colorGradient.Evaluate(normalisedTime);
    }
}
