using DistilledGames;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Column
{
	public TMP_Dropdown ItemDropdown;
	public TMP_InputField QuantityInput;
	public TMP_Text CostText;
	public Button ConfirmButton;
}

public class EndOfDayMenu : MonoBehaviour
{
	[Header("UI Elements")]
	public Column[] Columns; // Array of columns that each contain dropdown, inputfield, text, and button

	[Header("Items Data")]
	public List<Item> AvailableItems;

	private void Start()
	{
		InitializeColumns();
		SetupUIEventListeners();
	}

	private void InitializeColumns()
	{
		foreach (Column column in Columns)
		{
			column.ItemDropdown.ClearOptions();
			List<TMP_Dropdown.OptionData> options = new();
			foreach (Item item in AvailableItems)
			{
				options.Add(new TMP_Dropdown.OptionData(item.ItemName));
			}
			column.ItemDropdown.AddOptions(options);
		}
	}

	private void SetupUIEventListeners()
	{
		for (int i = 0; i < Columns.Length; i++)
		{
			int index = i;
			Columns[i].ItemDropdown.onValueChanged.AddListener(delegate { CalculateAndShowCost(index); });
			Columns[i].QuantityInput.onValueChanged.AddListener(delegate { CalculateAndShowCost(index); });
		}
	}

	public void CalculateAndShowCost(int columnIndex)
	{
		if (columnIndex < 0 || columnIndex >= Columns.Length)
		{
			return;
		}

		Column column = Columns[columnIndex];
		Item selectedItem = AvailableItems[column.ItemDropdown.value];

		if (!int.TryParse(column.QuantityInput.text, out int quantity))
		{
			Debug.LogError("Invalid quantity input!");
			return;
		}

		float totalCost = selectedItem.BuyValue * quantity;
		column.CostText.SetText($"Cost: ${totalCost:F2}");
	}

}
