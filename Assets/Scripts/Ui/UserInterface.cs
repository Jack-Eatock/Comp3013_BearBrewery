using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    [SerializeField] private TextMeshProUGUI revText;
    [SerializeField] private TextMeshProUGUI timeText;

    private float timeCounter = 0f;
    private int currentTimeInMinutes = 480;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        UpdateRevText("0");
        StartCoroutine(UpdateTime());
    }

    public void UpdateRevText(string text)
    {
        revText.text = "Revenue: £" + text;
    }

    private IEnumerator UpdateTime()
    {
        while (currentTimeInMinutes < 1080) // 6 PM in minutes (18 * 60)
        {
            UpdateTimeText();
            yield return new WaitForSeconds(4); // Wait for 4 seconds
            currentTimeInMinutes += 10; // Increment time by 10 minutes
        }
    }

    private void UpdateTimeText()
    {
        int hours = currentTimeInMinutes / 60;
        int minutes = currentTimeInMinutes % 60;
        timeText.text = string.Format("{0:00}:{1:00}", hours, minutes); // Format time as HH:MM
    }
}
