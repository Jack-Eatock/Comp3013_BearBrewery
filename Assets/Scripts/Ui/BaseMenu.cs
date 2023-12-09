using DistilledGames;
using UnityEngine;

public class BaseMenu : MonoBehaviour
{
    [SerializeField] private MenuManager.Menus menuType;
    [SerializeField] protected GameObject content;

    public virtual void ShowMenu()
    {
        MenuManager.Instance.SetCurrentMenu(menuType);
        content.SetActive(true);
    }

    public virtual void HideMenu()
    {
        content.SetActive(false);
        MenuManager.Instance.SetCurrentMenu(MenuManager.Menus.None);
    }
}
