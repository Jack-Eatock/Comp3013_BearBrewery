using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public event Action<string> OnTimeChanged;
    public event Action<string> OnDayChanged;

    [SerializeField] private int secondsInTenMinutes = 4;

    private int currentTimeInMinutes = 480; // 8 AM in minutes
    private string[] daysOfWeek = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    private int currentDayIndex = 0;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        StartCoroutine(UpdateTime());
    }

    private System.Collections.IEnumerator UpdateTime()
    {
        while (true)
        {
            string timeString = FormatTime(currentTimeInMinutes);
            OnTimeChanged?.Invoke(timeString);
            yield return new WaitForSeconds(secondsInTenMinutes);
            currentTimeInMinutes += 10;

            if (currentTimeInMinutes >= 1080) // 6 PM
            {
                currentTimeInMinutes = 480; // Reset to 8 AM
                UpdateDay();
            }
        }
    }

    private void UpdateDay()
    {
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length;
        OnDayChanged?.Invoke(daysOfWeek[currentDayIndex]);
    }

    private string FormatTime(int timeInMinutes)
    {
        int hours = timeInMinutes / 60;
        int minutes = timeInMinutes % 60;
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
}
