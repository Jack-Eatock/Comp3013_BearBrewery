using UnityEngine;

namespace DistilledGames
{
	public class Item : MonoBehaviour, IInteractable
	{
		[Header("Item Details")]
		[Space(10)]
		[SerializeField] private int iD;
		[Space(10)]
		[SerializeField] private string itemName;
		[Space(10)]
		[SerializeField] private int sellValue;
		[Space(10)]
		[SerializeField] private int buyValue;

		private Collider2D interactionCollider;

		public int ItemID => iD;
		public string ItemName => itemName;
		public int SellValue => sellValue;
		public int BuyValue => buyValue;
		public SpriteRenderer Rend { get; private set; }

		private void Awake()
		{
			GameManager.Instance.RegisterItem(this);
			Rend = GetComponent<SpriteRenderer>();
			interactionCollider = transform.GetChild(0).GetComponent<Collider2D>();
		}

		public void SetUpItem(int _id)
		{
			iD = _id;
		}

		public bool TryToInsertItem(Item item, bool conveyer = false)
		{
			return false;
		}

		public bool TryToRetreiveItem(out Item item)
		{
			item = this;
			return true;
		}

		public void SetInteractable(bool isInteractable)
		{
			interactionCollider.enabled = isInteractable;
		}
	}
}

