using DistilledGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    [SerializeField] private Button firstButton, secondButton, thirdButton;
    private TextMeshProUGUI firstText, secondText, thirdText;

    [SerializeField] private GameObject mainMenuBG;

    private void Awake()
    {
        firstText = firstButton.GetComponentInChildren<TextMeshProUGUI>();
        secondText = secondButton.GetComponentInChildren<TextMeshProUGUI>();
        thirdText = thirdButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        ShowMenu();
        SetupMenu(false);
    }

    public override void ShowMenu()
    {
        base.ShowMenu();
        MenuManager.Instance.SetGUIState(false);
    }

    public void SetupMenu(bool playing = false)
    {
        firstButton.onClick.RemoveAllListeners();
        secondButton.onClick.RemoveAllListeners();
        thirdButton.onClick.RemoveAllListeners();

        if (playing)
        {
            mainMenuBG.SetActive(false);
            firstText.text = "Resume";
            secondText.text = "Settings";
            thirdText.text = "Main Menu";
            firstButton.onClick.AddListener(() => ResumeClicked());
            secondButton.onClick.AddListener(() => SettingsClicked());
            thirdButton.onClick.AddListener(() => ReturnToMainMenu());
        }
        else
        {
            mainMenuBG.SetActive(true);
            firstText.text = "Start";
            secondText.text = "Settings";
            thirdText.text = "Exit";
            firstButton.onClick.AddListener(() => StartGame());
            secondButton.onClick.AddListener(() => SettingsClicked());
            thirdButton.onClick.AddListener(() => Quit());
        }
    }

    private void StartGame()
    {
        Debug.Log("Start");
        GameManager.Instance.StartGame();
        HideMenu();
    }

    private void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
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
}
