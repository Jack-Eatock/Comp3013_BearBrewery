using UnityEngine;

namespace DistilledGames
{
    public class Item : MonoBehaviour, IInteractable
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
        [SerializeField] private Collider interactionCollider;

        public int ItemID => ID;
        public string ItemName => itemName;
        public bool IsInteractable => isInteractable;
        public int Value => value;
        public SpriteRenderer Rend => rend;

        private SpriteRenderer rend;

        private void Awake()
        {
            GameManager.Instance.RegisterItem(this);
            rend = GetComponent<SpriteRenderer>();
        }

        public bool TryToInsertItem(Item item)
        {
            return false;
        }

        public bool TryToRetreiveItem(out Item item)
        {
            item = this;
            return true;
        }
    }
}

