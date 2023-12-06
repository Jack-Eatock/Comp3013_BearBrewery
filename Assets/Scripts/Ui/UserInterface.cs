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

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        UpdateRevText("0");

        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged += UpdateTimeText;
            TimeManager.Instance.OnDayChanged += UpdateDayText;
        }
    }

    public void UpdateRevText(string text)
    {
        revText.text = "Revenue: �" + text;
    }

    private void UpdateTimeText(string time)
    {
        timeText.text = time;
    }

    private void UpdateDayText(string day)
    {
        dayText.text = day;
    }
}
