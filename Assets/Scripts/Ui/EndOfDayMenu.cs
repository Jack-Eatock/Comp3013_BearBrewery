using DistilledGames;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Column
{
    public TMP_Dropdown itemDropdown;
    public TMP_InputField quantityInput;
    public TMP_Text costText;
    public Button confirmButton;
}

public class EndOfDayMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public Column[] columns; // Array of columns that each contain dropdown, inputfield, text, and button

    [Header("Items Data")]
    public List<Item> availableItems;

    private void Start()
    {
        InitializeColumns();
        SetupUIEventListeners();
    }

    private void InitializeColumns()
    {
        foreach (var column in columns)
        {
            column.itemDropdown.ClearOptions();
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var item in availableItems)
            {
                options.Add(new TMP_Dropdown.OptionData(item.ItemName));
            }
            column.itemDropdown.AddOptions(options);
        }
    }

    private void SetupUIEventListeners()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            int index = i;        
            columns[i].itemDropdown.onValueChanged.AddListener(delegate { CalculateAndShowCost(index); });
            columns[i].quantityInput.onValueChanged.AddListener(delegate { CalculateAndShowCost(index); });
        }
    }

    public void CalculateAndShowCost(int columnIndex)
    {
        if (columnIndex < 0 || columnIndex >= columns.Length) return;

        Column column = columns[columnIndex];
        Item selectedItem = availableItems[column.itemDropdown.value];

        if (!int.TryParse(column.quantityInput.text, out int quantity))
        {
            Debug.LogError("Invalid quantity input!");
            return;
        }

        float totalCost = selectedItem.BuyValue * quantity;
        column.costText.SetText($"Cost: ${totalCost:F2}");
    }

}
