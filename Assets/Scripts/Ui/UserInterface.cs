using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    [SerializeField] private TextMeshProUGUI revText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;

    [SerializeField] private int speedOf10Minutes = 4;

    private int currentTimeInMinutes = 480;
    private string[] daysOfWeek = new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
    private int currentDayIndex = 0;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        UpdateRevText("0");
        UpdateDayText();
        StartCoroutine(UpdateTime());
    }

    public void UpdateRevText(string text)
    {
        revText.text = "Revenue: £" + text;
    }

    private IEnumerator UpdateTime()
    {
        while (true) // Infinite loop to keep updating time
        {
            UpdateTimeText();
            yield return new WaitForSeconds(speedOf10Minutes); // Wait for 4 seconds
            currentTimeInMinutes += 10; // Increment time by 10 minutes

            if (currentTimeInMinutes >= 1080) // 6 PM in minutes
            {
                currentTimeInMinutes = 480; // Reset to 8 AM
                UpdateDay(); // Update to the next day
            }
        }
    }

    private void UpdateTimeText()
    {
        int hours = currentTimeInMinutes / 60;
        int minutes = currentTimeInMinutes % 60;
        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes); // Format time as HH:MM
    }

    private void UpdateDay()
    {
        currentDayIndex = (currentDayIndex + 1) % daysOfWeek.Length; // Cycle through the days
        UpdateDayText();
    }

    private void UpdateDayText()
    {
        dayText.text = daysOfWeek[currentDayIndex];
    }
}
