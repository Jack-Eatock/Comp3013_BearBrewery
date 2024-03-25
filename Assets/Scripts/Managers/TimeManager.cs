using System;
using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public static TimeManager Instance;

	public event Action<string> OnTimeChanged;
	public event Action<string> OnDayChanged;

	[SerializeField] private float secondsInTenMinutes = 4;

	private int currentTimeInMinutes = 480; // 8 AM in minutes
	private readonly string[] daysOfWeek = { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
	private int currentDayIndex = 0;

	private IEnumerator updatingTime;

	private void Awake()
	{
		if (Instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
		}

		updatingTime = UpdateTime();
		StartCoroutine(updatingTime);
	}

	private IEnumerator UpdateTime()
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

			yield return null;
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
