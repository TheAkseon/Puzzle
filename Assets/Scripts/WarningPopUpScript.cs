using Localization;
using UnityEngine;

public class WarningPopUpScript : MonoBehaviour
{
    private void checkDarkPopUp()
    {
        UIManagerScript.ClosePopUp(string.Empty);
    }

    public void AuthAsGeust()
    {
        AudioScripts.Click();
        WWWForm body = new WWWForm();
        var languageParam = LocalizationManager.Instance.ChosenLanguage == Localizations.RUSSIAN ? 1 : 2;
        body.AddField("language_id", languageParam);
        APIMethodsScript.sendRequest("post", "/api/register_guest", getToken, body);
    }

    public void Auth()
    {
        AudioScripts.Click();
        UIManagerScript.StartPopUp("LogInPopUp");
        UIManagerScript.ClosePopUp("NotLoggedInWarningPopUp");
    }

    public void getToken(string json, int state)
    {
        TokenScript.Token token = JsonUtility.FromJson<TokenScript.Token>(json);
        if (token.error == null && token.access_token != null)
        {
            AuthPopUpScript.CreateUser(token, "guest_");
            UIManagerScript.ClosePopUp("NotLoggedInWarningPopUp");
            checkDarkPopUp();
        }
    }
}
