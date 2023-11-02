using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class BuildingMenu : MonoBehaviour
    {
        private CanvasGroup group;
        [SerializeField] private float blendTime = 1f;
        [SerializeField] private GameObject content;
        [SerializeField] private BuildingOption optionTemplate;
        [SerializeField] private Transform optionHolder;
        private IEnumerator showingHidingMenu;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
            content.SetActive(true);
            content.SetActive(false);
            GenerateOptions();
        }

        private void GenerateOptions()
        {
            foreach (Transform child in optionHolder)
                Destroy(child.gameObject);

            foreach (BuildingData buildingData in GameManager.Instance.GameConfig.BuildingData)
            {
                BuildingOption option = GameObject.Instantiate(optionTemplate, optionHolder);
                option.SetupButton(buildingData.Name, buildingData.DisplayImage, () => { OptionClicked(buildingData); });
            }
        }

        private void OptionClicked(BuildingData buildingSelected)
        {
            Debug.Log("Option picked! " );
            BuildingManager.instance.selectedBuilding = buildingSelected;
            GameManager.Instance.SwitchState(StateDefinitions.GameStates.BuildingModePlacing.ToString());
        }

        public void ShowMenu()
        {
            if (showingHidingMenu != null)
                StopCoroutine(showingHidingMenu);

            showingHidingMenu = ShowingOrHiding(true, blendTime);
            StartCoroutine(showingHidingMenu);
        }

        public void HideMenu()
        {
            if (showingHidingMenu != null)
                StopCoroutine(showingHidingMenu);

            showingHidingMenu = ShowingOrHiding(false, blendTime);
            StartCoroutine(showingHidingMenu);
        }

        private IEnumerator ShowingOrHiding(bool show, float time)
        {
            float timeStarted = Time.time;

            if (show)
            {
                group.alpha = 0;
                content.SetActive(true);
            }

            while (Time.time - timeStarted <= time)
            {
                float fractionCompleted = ((Time.time - timeStarted) / time);
                if (show)
                    group.alpha = Mathf.Lerp(0, 1, fractionCompleted);
                else
                    group.alpha = Mathf.Lerp(1, 0, fractionCompleted);
                yield return new WaitForEndOfFrame();
            }

            if (show)
                group.alpha = 1;
            else
                group.alpha = 0;


            if (!show)
                content.SetActive(false);

            showingHidingMenu = null;
        }
    }

}
