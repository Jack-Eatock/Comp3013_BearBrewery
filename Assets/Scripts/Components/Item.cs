using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Details")]
    [Space(10)]
    [SerializeField] private int ID;
    [Space(10)]
    [SerializeField] private string itemName;
    [Space(10)]
    [SerializeField] private bool isInteractable;
    [Space(10)]
    [SerializeField] private int value;

    public int ItemID => ID;
    public string ItemName => itemName;
    public bool IsInteractable => isInteractable;
    public int Value => value;
}