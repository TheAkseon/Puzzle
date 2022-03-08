using UnityEngine;

public class LogOutScripts : MonoBehaviour
{
    public void LogOutClicked()
    {
        UIManagerScript.StartLoader();
        APIMethodsScript.sendRequest("delete", "/api/deauth", logOut);
    }

    public static void logOut(string json, int state)
    {
        if (state == 200)
        {
            UIManagerScript.ClosePopUp("LogOutPopUp");
            Destroy(GameObject.Find("User"));
            GameObject.Find("User").GetComponent<UserScripts>().user.token.deleteToken();
            if (System.IO.Directory.Exists(Application.persistentDataPath + "/photoPuzzles"))
            {
                System.IO.Directory.Delete(Application.persistentDataPath + "/photoPuzzles", true);
            }
        }
        Destroy(GameObject.Find("Loader"));
    }

    public void StayClicked()
    {
        UIManagerScript.ClosePopUp("LogOutPopUp");
    }
}
