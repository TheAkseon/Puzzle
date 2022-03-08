using System;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    public string key;
    private Text text;

    private void OnEnable()
    {
        LocalizationManager.onLanguageChange += RefreshText;
    }

    private void OnDisable()
    {
        LocalizationManager.onLanguageChange -= RefreshText;
    }

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    private void Start()
    {
        RefreshText();
    }

    private void RefreshText()
    {
        text.text = LocalizationManager.Instance.GetLocalizedValue(key);
    }
}
