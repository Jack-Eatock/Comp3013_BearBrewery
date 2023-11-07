using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInterface : MonoBehaviour
{
    public static UserInterface Instance;

    [SerializeField] private TextMeshProUGUI revText;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        UpdateRevText("0");
    }

    public void UpdateRevText(string text)
    {
        revText.text = "Revenue: £" + text;
    }
}
