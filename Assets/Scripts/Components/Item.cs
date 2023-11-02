using UnityEngine;

namespace DistilledGames
{
    public class Item : MonoBehaviour
    {
        [Header("Item Details")]
        [Space(10)]
        [SerializeField] private int ID;
        [Space(10)]
        [SerializeField] private string itemName;
        [Space(10)]
        [SerializeField] private bool isInteractable;

        public int ItemID => ID;
        public string ItemName => itemName;
        public bool IsInteractable => isInteractable;

        private void Awake()
        {
            GameManager.Instance.RegisterItem(this);
        }
    }
}

