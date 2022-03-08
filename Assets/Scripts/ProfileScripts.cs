using UnityEngine;
using UnityEngine.UI;

public class ProfileScripts : MonoBehaviour
{
    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }

    // Use this for initialization
    private void Start()
    {
        if (GameObject.Find("AuthProfilePopUp") && GameObject.Find("User") && GameObject.Find("User").GetComponent<UserScripts>().user != null && GameObject.Find("User").GetComponent<UserScripts>().user.name != null)
        {
            var authorizedAsLocalizedText = LocalizationManager.Instance.GetLocalizedValue("authorized_as");
            GameObject.Find("AuthProfilePopUp").transform.Find("Text").GetComponent<Text>().text = authorizedAsLocalizedText + GameObject.Find("User").GetComponent<UserScripts>().user.name.ToUpper();
        }
    }

    public void LogIn()
    {
        UIManagerScript.StartPopUp("LogInPopUp");
        ClosePopUp();
    }

    public void SignIn()
    {
        UIManagerScript.StartPopUp("SignInPopUp");
        ClosePopUp();
    }

    public void LogOut()
    {
        UIManagerScript.StartPopUp("LogOutPopUp");
        ClosePopUp();
    }
}
