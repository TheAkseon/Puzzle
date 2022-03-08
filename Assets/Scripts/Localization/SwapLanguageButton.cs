using Localization;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class SwapLanguageButton : MonoBehaviour
{
    [SerializeField]
    private Sprite englishSprite;
    [SerializeField]
    private Sprite russianSprite;

    private Image image;
    private Button button;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();

        button.onClick.AddListener(OnButtonPressedEventHandler);
    }

    private void Start()
    {
        SetSprite(LocalizationManager.Instance.ChosenLanguage);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }

    private void OnButtonPressedEventHandler()
    {
        LocalizationManager.Instance.SwapLanguage();
        RequestToChangeLanguage();
    }

    private void RequestToChangeLanguage()
    {
        var obj = GameObject.Find("User");
        if (obj == null) return;
        var userScripts = obj.GetComponent<UserScripts>();
        if (userScripts == null || userScripts.user == null) return;
        var languageParam = LocalizationManager.Instance.ChosenLanguage == Localizations.RUSSIAN ? 1 : 2;
        WWWForm body = new WWWForm();
        body.AddField("language_id", languageParam);
        button.interactable = false;
        APIMethodsScript.sendRequest("patch", "/api/user", ResponseCodeChange, body);
    }

    private void ResponseCodeChange(string json, int status)
    {
        button.interactable = true;
        if (status != 200)
        {
            LocalizationManager.Instance.SwapLanguage();
        }
        Debug.Log("Language change request returned with code: " + status);
    }

    public void SetSprite(string language)
    {
        image.sprite = language == "English" ? englishSprite : russianSprite;
    }
}
