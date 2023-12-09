using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject content;

    [SerializeField] private Button firstButton, secondButton, thirdButton;
    private TextMeshProUGUI firstText, secondText, thirdText;

    private void Awake()
    {
        firstText = firstButton.GetComponentInChildren<TextMeshProUGUI>();
        secondText = secondButton.GetComponentInChildren<TextMeshProUGUI>();
        thirdText = thirdButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        ShowMenu();
    }

    public void ShowMenu(bool playing = false)
    {
        firstButton.onClick.RemoveAllListeners();
        secondButton.onClick.RemoveAllListeners();
        thirdButton.onClick.RemoveAllListeners();

        if (playing)
        {
            firstText.text = "Resume";
            secondText.text = "Settings";
            thirdText.text = "Main Menu";
            firstButton.onClick.AddListener(() => ResumeClicked());
            secondButton.onClick.AddListener(() => SettingsClicked());
            thirdButton.onClick.AddListener(() => ReturnToMainMenu());
        }
        else
        {
            firstText.text = "Start";
            secondText.text = "Settings";
            thirdText.text = "Exit";
            firstButton.onClick.AddListener(() => StartGame());
            secondButton.onClick.AddListener(() => SettingsClicked());
            thirdButton.onClick.AddListener(() => Quit());
        }

        content.SetActive(true);
    }

    private void StartGame()
    {
        Debug.Log("Start");
    }

    private void Quit()
    {
        Debug.Log("Quit");
    }

    private void ReturnToMainMenu()
    {
        Debug.Log("Return To MainMenu");
    }

    private void SettingsClicked()
    {
        Debug.Log("Settings");
    }

    private void ResumeClicked()
    {
        Debug.Log("Resumed");
    }

    public  void HideMenu() 
    {
        content.SetActive(false);
    }
}
