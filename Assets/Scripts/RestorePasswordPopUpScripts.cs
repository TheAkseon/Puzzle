using UnityEngine;
using UnityEngine.UI;

public class RestorePasswordPopUpScripts : MonoBehaviour
{
    private string email;

    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }

    public void Exit()
    {
        UIManagerScript.StartPopUp("LogInPopUp");
        ClosePopUp();
    }

    public void getResponse(string json, int status)
    {
        if (status == 200)
        {
            GameObject PopUp = UIManagerScript.StartPopUp("ResetPasswordPopUp");
            PopUp.transform.Find("Email").transform.Find("Placeholder").GetComponent<Text>().text = email;
            ClosePopUp();
        }
        else
        {
            GameObject.Find("Email").GetComponent<Image>().color = Color.red;
        }
        Destroy(GameObject.Find("Loader"));
    }

    public void SendNewPassword()
    {
        if (transform.Find("Email").transform.Find("Text").GetComponent<Text>().text != "")
        {
            UIManagerScript.StartLoader();
            email = transform.Find("Email").GetComponent<InputField>().text;
            WWWForm body = new WWWForm();
            body.AddField("email", email);
            APIMethodsScript.sendRequest("post", "/api/forgot_password", getResponse, body);
        }
        else
        {
            transform.Find("Email").GetComponent<Image>().color = Color.red;
        }
    }

    public void ResetPassword()
    {
        transform.Find("Email").GetComponent<Image>().color = Color.white;
        transform.Find("Password").GetComponent<Image>().color = Color.white;
        transform.Find("Code").GetComponent<Image>().color = Color.white;
        if (transform.Find("Email").transform.Find("Placeholder").GetComponent<Text>().text == "" || transform.Find("Password").transform.Find("Text").GetComponent<Text>().text == "" || transform.Find("Code").transform.Find("Text").GetComponent<Text>().text == "")
        {
            if (transform.Find("Email").transform.Find("Placeholder").GetComponent<Text>().text == "")
                transform.Find("Email").GetComponent<Image>().color = Color.red;
            if (transform.Find("Password").transform.Find("Text").GetComponent<Text>().text == "")
                transform.Find("Password").GetComponent<Image>().color = Color.red;
            if (transform.Find("Code").transform.Find("Text").GetComponent<Text>().text == "")
                transform.Find("Code").GetComponent<Image>().color = Color.red;
        }
        else
        {
            WWWForm body = new WWWForm();
            string email = transform.Find("Email").transform.Find("Text").GetComponent<Text>().text;
            if (email == "")
                email = transform.Find("Email").transform.Find("Placeholder").GetComponent<Text>().text;
            string password = transform.Find("Password").transform.Find("Text").GetComponent<Text>().text;
            string code = transform.Find("Code").transform.Find("Text").GetComponent<Text>().text;
            body.AddField("email", email);
            body.AddField("password", password);
            body.AddField("token", code);
            body.AddField("password_confirmation", password);
            APIMethodsScript.sendRequest("post", "/api/reset_password", CheckReset, body);
        }
    }

    public void CheckReset(string json, int status)
    {
        if (status == 200)
        {
            ClosePopUp();
        }
        else
        {
            transform.Find("Email").GetComponent<Image>().color = Color.red;
            transform.Find("Password").GetComponent<Image>().color = Color.red;
            transform.Find("Code").GetComponent<Image>().color = Color.red;
        }
    }
}
