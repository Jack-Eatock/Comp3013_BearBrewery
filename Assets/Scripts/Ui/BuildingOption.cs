using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingOption : MonoBehaviour
{
	[SerializeField] private Image image;
	[SerializeField] private TextMeshProUGUI text, costText;
	[SerializeField] private Image costColour;
	[SerializeField] private Color cantBuy;
	[SerializeField] private Color canBuy;
	public BuildingData buildingData;
	public Toggle button;
	private Action<bool> cb;

	public void SetupButton(BuildingData data, string _text, Sprite _image, ToggleGroup group, string _costText, Action<bool> _cb)
	{
		buildingData = data;
		text.text = _text;
		image.sprite = _image;
		button = GetComponent<Toggle>();
		button.group = group;
		cb = _cb;
		costText.text = _costText;
		button.onValueChanged.AddListener((bool val) => cb(val));
	}

	public void UpdateCost(bool canBuy)
	{
		costColour.color = canBuy ? this.canBuy : cantBuy;
	}

	public void TurnOffWithoutNotif()
	{
		button.onValueChanged.RemoveAllListeners();
		button.isOn = false;
		button.onValueChanged.AddListener((bool val) => cb(val));
	}

}
