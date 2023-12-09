using DistilledGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    public static MainMenu instance;

    [SerializeField] private Button firstButton, secondButton, thirdButton;
    private TextMeshProUGUI firstText, secondText, thirdText;

    [SerializeField] private GameObject mainMenuBG;
    [SerializeField] private SettingsMenu menuSettings;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

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
        Time.timeScale = 0f;
        GameManager.Instance.SwitchState(StateDefinitions.GameStates.InMenu.ToString());
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

    public override void HideMenu()
    {
        base.HideMenu();
        Time.timeScale = 1f;
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
        SetupMenu(false);
    }

    private void SettingsClicked()
    {
        Debug.Log("Settings");
        menuSettings.gameObject.SetActive(true);
    }

    private void ResumeClicked()
    {
        Debug.Log("Resumed");
        MenuManager.Instance.SetGUIState(false);
        HideMenu();
        GameManager.Instance.SwitchState(StateDefinitions.GameStates.Normal.ToString());
    }
}
