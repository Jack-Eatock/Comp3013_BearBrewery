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
    private Button button;

    public void SetupButton(string _text, Sprite _image, Action cb)
    {
        text.text = _text;
        image.sprite = _image;
        button = GetComponent<Button>();
        button.onClick.AddListener(() => cb());
    }
}
