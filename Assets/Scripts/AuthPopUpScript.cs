using System.Collections.Generic;
using System.Text.RegularExpressions;
using Localization;
using UnityEngine;
using UnityEngine.UI;

public class AuthPopUpScript : MonoBehaviour
{
    public const string MatchEmailPattern =
           @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
           + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
             + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
           + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public void ClosePopUp(string name)
    {
        UIManagerScript.ClosePopUp(name);
    }

    public void Exit()
    {
        if (name == "LogInPopUp" || name == "SignInPopUp")
        {
            GameObject.Find("UIManager").GetComponent<UIManagerScript>().UserClicked();
        }
        ClosePopUp(name);
    }

    public void ForgotPasswordClicked()
    {
        UIManagerScript.StartPopUp("PasswordRestorePopUp");
        ClosePopUp(name);
    }

    public void getToken(string json, int state)
    {
        TokenScript.Token token = JsonUtility.FromJson<TokenScript.Token>(json);
        if ((GameObject.Find("LogInPopUp") && token.error == null && token.access_token != null) || (GameObject.Find("SignInPopUp") && token.access_token != null))
        {
            CreateUser(token);
            ClosePopUp(name);
        }
        else
        {
            ErrorMessage(token);
        }
        Destroy(GameObject.Find("Loader"));
    }

    public static void setUserData(string json, int status, string prefix = "")
    {
        if (status == 200)
        {
            setDataUser(json);
        }
    }

    public static void setDataUser(string json)
    {
        UserScripts u = GameObject.Find("User").GetComponent<UserScripts>();
        TokenScript.Token tmpToken = new TokenScript.Token(u.user.token);
        u.user = JsonUtility.FromJson<User.UserResponse>(json).user;
        var localizationId = LocalizationManager.Instance.ChosenLanguage == Localizations.RUSSIAN ? 1 : 2;
        if (u.user.language_id != localizationId)
        {
            LocalizationManager.Instance.SetLocalization(localizationId == 1 ? Localizations.RUSSIAN : Localizations.ENGLISH);
        }
        u.user.token = new TokenScript.Token(tmpToken);
        if (string.IsNullOrEmpty(u.user.name))
        {
            u.user.name = string.Empty;
        }
        BackgroundsScripts.UpdateBackground();
        GameObject.Find("UIManager").GetComponent<UIManagerScript>().RenderGreetings();
        Push(u.user.name);
    }

    private static void UpdateUserLanguage()
    {
        UserScripts u = GameObject.Find("User").GetComponent<UserScripts>();


    }

    private static void Push(string username)
    {
        OneSignal.StartInit("c67ff5c9-64b3-4a08-bffd-a5d794aa7c45")
            .HandleNotificationOpened(HandleNotificationOpened)
            .EndInit();
        OneSignal.inFocusDisplayType = OneSignal.OSInFocusDisplayOption.Notification;
        if (OneSignal.GetPermissionSubscriptionState().subscriptionStatus.userId != null)
        {
            string push_id = OneSignal.GetPermissionSubscriptionState().subscriptionStatus.userId;
            WWWForm body = new WWWForm();
            body.AddField("username", username);
            body.AddField("push_id", push_id);
            APIMethodsScript.sendRequest("patch", "/api/user", pushStat, body);
        }
    }

    public static void pushStat(string json, int status)
    {
        if (status == 200)
        {

        }
    }

    private static void HandleNotificationOpened(OSNotificationOpenedResult result)
    {
        OSNotificationPayload payload = result.notification.payload;
        Dictionary<string, object> additionalData = payload.additionalData;

        Transform current = UIManagerScript.StartPopUp("PushPopUp").transform;
        current.Find("info-box").Find("Title").Find("Title").GetComponent<Text>().text = payload.body;
        if (additionalData != null)
        {
            int id = 0;
            if (additionalData.ContainsKey("type_id"))
            {
                id = (int)(long)additionalData["type_id"];
            }
            if (additionalData.ContainsKey("description"))
            {
                current.Find("info-box").Find("Info").GetComponent<Text>().text = (string)additionalData["description"];
            }
            if (additionalData.ContainsKey("image"))
            {
                MonoBehaviour dummy = GameObject.Find("UIManager").GetComponent<UIManagerScript>();
                dummy.StartCoroutine(ImagesScript.doLoad(new WWW((string)additionalData["image"]), current.Find("image").GetComponent<RawImage>()));
            }
            current.Find("next").gameObject.SetActive(id == 1 || id == 2);
            PushPopUp p = current.GetComponent<PushPopUp>();
            if (id == 1 || id == 2)
            {
                if (id == 1 && additionalData.ContainsKey("puzzle_id"))
                {
                    p.puzzle = int.Parse((string)additionalData["puzzle_id"]);
                }
                if (additionalData.ContainsKey("category_id"))
                {
                    p.category = (int)(long)additionalData["category_id"];
                }
            }
            else if (id == 3)
            {
                if (additionalData.ContainsKey("currency_id") && additionalData.ContainsKey("currency_count"))
                {
                    current.Find("Coin").gameObject.SetActive(true);
                    UIManagerScript.LerpCoins((int)(long)additionalData["currency_count"], current.Find("Coin").transform.Find("Coins").Find("Text").GetComponent<Text>());
                }
            }
        }
    }

    public static void CreateUser(TokenScript.Token token, string prefix = "")
    {
        token.saveToken(prefix);
        GameObject User = GameObject.Find("User");
        UserScripts u;
        if (!User)
        {
            User = new GameObject("User");
            DontDestroyOnLoad(User);
            u = User.AddComponent<UserScripts>();
        }
        else
        {
            u = User.GetComponent<UserScripts>();
        }
        User.AddComponent<Purchaser>();
        u.user = new UserScripts.User(token);
        APIMethodsScript.sendRequest("get", "/api/user/self", setUserData, prefix);
    }

    public void resetUserStatus(string json, int status)
    {
        if (status == 200)
        {
            string text = System.IO.File.ReadAllText(Application.persistentDataPath + "/guest_token.txt");
            System.IO.File.WriteAllText(Application.persistentDataPath + "/token.txt", text);
            System.IO.File.Delete(Application.persistentDataPath + "/guest_token.txt");
            setDataUser(json);
            GameObject.Find("UIManager").GetComponent<UIManagerScript>().RenderGreetings();
            ClosePopUp(name);
        }
        else
        {
            TokenScript.Token token = JsonUtility.FromJson<TokenScript.Token>(json);
            ErrorMessage(token);
        }
        Destroy(GameObject.Find("Loader"));
    }

    private void ErrorMessage(TokenScript.Token token)
    {
        bool eStat = token.errors != null;
        bool logining = name == "LogInPopUp";
        if (!logining)
        {
            SetValidationColor("Login", !eStat || eStat && token.errors.username == null);
        }
        SetValidationColor("Password", !logining);
        SetValidationColor("Email", !logining && (!eStat || eStat && token.errors.email == null));
        ErrorMessage(token.message);
        Destroy(GameObject.Find("Loader"));
    }

    private void ErrorMessage(string v)
    {
        transform.Find("ErrorMessage").GetComponent<Text>().text = v;
    }

    public void ClickedPopUp()
    {
        bool logining = name == "LogInPopUp";
        string login = logining ? string.Empty : GetText("Login");
        string password = GetText("Password");
        string email = GetText("Email");
        bool emailRegex = email != "" && Regex.IsMatch(email, MatchEmailPattern);
        bool passRegex = password != "" && password.Length >= 6;
        if ((login != "" || logining) && passRegex && emailRegex)
        {
            UIManagerScript.StartLoader();
            WWWForm body = new WWWForm();
            body.AddField("email", email);
            body.AddField("password", password);
            if (logining)
            {
                APIMethodsScript.sendRequest("post", "/api/auth", getToken, body);
            }
            else
            {
                body.AddField("firstname", login);
                body.AddField("password_confirmation", password);
                
                var languageParam = LocalizationManager.Instance.ChosenLanguage == Localizations.RUSSIAN ? 1 : 2;
                body.AddField("language_id", languageParam);
                
                if (GameObject.Find("User") == null || !System.IO.File.Exists(Application.persistentDataPath + "/guest_token.txt"))
                {
                    APIMethodsScript.sendRequest("post", "/api/register", getToken, body);
                }
                else
                {
                    body.AddField("is_guest", "1");
                    APIMethodsScript.sendRequest("patch", "/api/user", resetUserStatus, body);
                }
            }
        }
        else
        {
            if (!logining)
            {
                SetValidationColor("Login", login != "");
            }
            SetValidationColor("Password", passRegex);
            SetValidationColor("Email", emailRegex);
            var incorrectDataMessage = LocalizationManager.Instance.GetLocalizedValue("incorrect_data");
            ErrorMessage(incorrectDataMessage);
        }
    }

    private string GetText(string v)
    {
        return transform.Find(v).transform.Find(v).GetComponent<InputField>().text;
    }

    private void SetValidationColor(string v, bool s)
    {
        transform.Find(v).GetComponent<Image>().color = s ? Color.white : Color.red;
    }

    public void OpenPopUp(string v)
    {
        UIManagerScript.StartPopUp(v);
        ClosePopUp(name);
    }
}