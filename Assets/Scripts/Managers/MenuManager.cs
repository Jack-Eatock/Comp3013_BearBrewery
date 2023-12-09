using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DistilledGames
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager Instance;

        public enum Menus { None, BuildingMenu, MainMenu, Settings }

        private Menus activeMenu = Menus.None;


        [SerializeField] private BuildingMenu buildingMenu;
        [SerializeField] private MainMenu mainMenu;

        // GUI

        [SerializeField] private GameObject gui;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        public void SetGUIState(bool active)
        {
            gui.SetActive(active);
        }

        public void SetCurrentMenu(Menus menuToShow)
        {
            if (activeMenu == menuToShow)
                return;

            if (activeMenu != Menus.None)
                HideCurrentMenu(activeMenu);

            activeMenu = menuToShow;
        }

        public void HideCurrentMenu(Menus menuToHide)
        {
            activeMenu = Menus.None;
            switch (menuToHide)
            {
                case Menus.BuildingMenu:
                    buildingMenu.HideMenu();
                    break;

                case Menus.MainMenu: 
                    mainMenu.HideMenu();
                    break;
            }
        }
    }

}
