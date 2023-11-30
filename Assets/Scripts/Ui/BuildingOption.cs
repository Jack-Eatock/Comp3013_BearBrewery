using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class BuildingOption : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;
    public Toggle button;
    Action<bool> cb;


    public void SetupButton(string _text, Sprite _image, ToggleGroup group, Action<bool> _cb)
    {
        text.text = _text;
        image.sprite = _image;
        button = GetComponent<Toggle>();
        button.group = group;
        cb = _cb;
        button.onValueChanged.AddListener((bool val) => cb(val));
    }

    public void TurnOffWithoutNotif()
    {
        button.onValueChanged.RemoveAllListeners();
        button.isOn = false;
        button.onValueChanged.AddListener((bool val) => cb(val));
    }

}
