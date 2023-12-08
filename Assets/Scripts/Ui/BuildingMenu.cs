using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DistilledGames.States;

namespace DistilledGames
{
    public enum BuildMode { Select, Delete, Placing }

    public class BuildingMenu : MonoBehaviour
    {
        public static BuildingMenu instance;

        public enum BuildingMenuPanels { BuildingOptions, PlacingBuilding }
        private BuildingMenuPanels activePanel;

        [SerializeField] private BuildingMenuPanels defaultPanel;

        private CanvasGroup group;
        [SerializeField] private float blendTime = 1f;
        [SerializeField] private GameObject content, placingBuildingPanel, buildingOptionsPanel;
        [SerializeField] private BuildingOption optionTemplate;
        [SerializeField] private Transform optionHolder;
        [SerializeField] private CanvasGroup oldMessage;
        [SerializeField] private Transform newMessage;
        private IEnumerator showingHidingMenu;

        [SerializeField]
        private ToggleGroup toggleGroup;

        [SerializeField]
        private Toggle mouseMode;
        private List<BuildingOption> dynamicToggles = new List<BuildingOption>();

        public BuildMode buildingMode;


        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
            group = GetComponent<CanvasGroup>();
            ShowPanel(BuildingMenuPanels.BuildingOptions, true); // Strange TMP bug
            ShowPanel(BuildingMenuPanels.BuildingOptions, false);

            foreach (BuildingMenuPanels panel in Enum.GetValues(typeof(BuildingMenuPanels)))
                ShowPanel(panel, false);

            HideMenu();

            GenerateOptions();
            activePanel = defaultPanel;
        }

        public void RefreshCosts()
        {
            for (int i = 0; i < dynamicToggles.Count; i++)
            {
                bool canTheyAffordIt = dynamicToggles[i].buildingData.Cost < GameManager.Instance.Cash;
                dynamicToggles[i].UpdateCost(canTheyAffordIt);
            }
        }

        private void GenerateOptions()
        {
            dynamicToggles.Clear();
            foreach (Transform child in optionHolder)
                Destroy(child.gameObject);

            foreach (BuildingData buildingData in GameManager.Instance.GameConfig.BuildingData)
            {
                BuildingOption option = GameObject.Instantiate(optionTemplate, optionHolder);
                option.SetupButton(buildingData,buildingData.Name, buildingData.DisplayImage, toggleGroup, "�" + buildingData.Cost.ToString(), (bool val) => { OptionToggled(buildingData, val); });
                dynamicToggles.Add(option);
            }
        }

        private void OptionToggled(BuildingData buildingSelected, bool toggledOn)
        {
            if (!toggledOn)
                return;


            Debug.Log("Option picked! ");
            BuildingManager.instance.selectedBuilding = buildingSelected;
            BuildingSelected(true);
        }

        public void DeleteOptionSelected(bool val)
        {
            if (!val)
                return;
            buildingMode = BuildMode.Delete;
            GameManager.Instance.NextState = StateDefinitions.GameStates.BuildingModeDeleting.ToString();
            GameManager.Instance.CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
        }

        public void SelectOptionSelected(bool val)
        {
            if (!val)
                return;
            buildingMode = BuildMode.Select;
            GameManager.Instance.NextState = StateDefinitions.GameStates.BuildingMode.ToString();
            GameManager.Instance.CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
        }

        public void BuildingSelected(bool val)
        {
            if (!val)
                return;
            buildingMode = BuildMode.Placing;
            GameManager.Instance.NextState = StateDefinitions.GameStates.BuildingModePlacing.ToString();
            GameManager.Instance.CheckIfStateShouldChange(StateDefinitions.ChangeInState.NextState);
        }

        public void ShowMenu()
        {
            if (showingHidingMenu != null)
                StopCoroutine(showingHidingMenu);

            toggleGroup.SetAllTogglesOff(false);
            mouseMode.SetIsOnWithoutNotify(true);

            ShowPanel(activePanel, true);
            showingHidingMenu = ShowingOrHiding(true, blendTime);
            StartCoroutine(showingHidingMenu);

        }

        public void HideMenu()
        {
            toggleGroup.SetAllTogglesOff(false);
            //foreach (BuildingOption toggle in dynamicToggles)
            //{
            //    toggle.TurnOffWithoutNotif();
            //}
           

            if (showingHidingMenu != null)
                StopCoroutine(showingHidingMenu);

            ShowPanel(activePanel, false);
            showingHidingMenu = ShowingOrHiding(false, blendTime);
            StartCoroutine(showingHidingMenu);
        }

        public void SwitchPanel(BuildingMenuPanels panel)
        { 
            ShowPanel(activePanel, false);
            ShowPanel(panel, true);
            activePanel = panel;
        }

        private void ShowPanel(BuildingMenuPanels panel, bool show)
        {
            switch (panel)
            {
                case BuildingMenuPanels.BuildingOptions:
                    buildingOptionsPanel.SetActive(show);
                    break;

                case BuildingMenuPanels.PlacingBuilding:
                    placingBuildingPanel.SetActive(show);
                    break;
            }
        }

        private IEnumerator ShowingOrHiding(bool show, float time)
        {
            float timeStarted = Time.time;

            if (show)
            {
                oldMessage.alpha = 0;
                group.alpha = 0;
                content.SetActive(true);
                newMessage.gameObject.SetActive(true);
            }
            else
            {
                newMessage.gameObject.SetActive(false);
            }

            while (Time.time - timeStarted <= time)
            {
                float fractionCompleted = ((Time.time - timeStarted) / time);
                if (show)
                    group.alpha = Mathf.Lerp(0, 1, fractionCompleted);
                else
                {
                    group.alpha = Mathf.Lerp(0, 1, fractionCompleted);
                    oldMessage.alpha = Mathf.Lerp(0, 1, fractionCompleted);
                }
                  
                yield return new WaitForEndOfFrame();
            }

            if (show)
                group.alpha = 1;
            else 
            {
                oldMessage.alpha = 1;
                group.alpha = 0;
            }

            if (!show)
                content.SetActive(false);

            showingHidingMenu = null;
        }
    }

}
