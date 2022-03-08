using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BackgroundsScripts : MonoBehaviour
{
    private Texture currentBackground;
    private static UserScripts u;

    private void Start()
    {
        transform.GetComponent<RawImage>().texture = GameObject.Find("Background").GetComponent<RawImage>().texture;
        currentBackground = GameObject.Find("Background").GetComponent<RawImage>().texture;
        GetUser();
    }

    public void ChangeBackground()
    {
        AudioScripts.Click();
        currentBackground = EventSystem.current.currentSelectedGameObject.GetComponent<RawImage>().texture;
        transform.GetComponent<RawImage>().texture = currentBackground;
    }

    public static void UpdateBackground()
    {
        GetUser();
        if (GameObject.Find("Background"))
        {
            GameObject.Find("Background").GetComponent<RawImage>().texture = (Texture)Resources.Load("Images/Backgrounds/" + u.user.background_id.ToString());
        }
    }

    public void SaveBackground()
    {
        AudioScripts.Click();
        u.user.background_id = int.Parse(transform.GetComponent<RawImage>().texture.name);
        GameObject.Find("Background").GetComponent<RawImage>().texture = currentBackground;
        WWWForm body = new WWWForm();
        body.AddField("background_id", u.user.background_id.ToString());
        APIMethodsScript.sendRequest("patch", "/api/user", ContinueSave, body);
        UIManagerScript.StartLoader();
        //Exit();
    }

    private void ContinueSave(string json, int state)
    {
        Destroy(GameObject.Find("Loader"));
        Exit();
    }

    public void Cancel()
    {
        AudioScripts.Click();
        Exit();
    }

    private static void GetUser()
    {
        if (u == null)
        {
            u = GameObject.Find("User").GetComponent<UserScripts>();
        }
    }

    public void Exit()
    {
        Destroy(GameObject.Find("BackgroundCustomizer"));
    }
}
