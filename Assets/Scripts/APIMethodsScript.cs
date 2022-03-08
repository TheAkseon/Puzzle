using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIMethodsScript : MonoBehaviour
{
    public delegate void function1param(string parameter);
    public delegate void function2param(string parameter1, int parameter2);
    public delegate void function3param(string parameter1, int parameter2, string parameter3);
    public delegate void function4param(string parameter1, int parameter2, int parameter3, string parameter4);

    public static void sendRequest(string type, string url, function2param method, WWWForm body = null)
    {
        UnityHTTP.Request someRequest = Request(type, url, body);
        someRequest.Send((request) =>
        {
            Debug.Log("[" + type + "] " + url + " : " + someRequest.response.status);
            string thing = request.response.Text;
            method(thing, someRequest.response.status);
        });
    }

    public static void sendRequest(string type, string url, function3param method, string param, WWWForm body = null)
    {
        UnityHTTP.Request someRequest = Request(type, url, body);
        someRequest.Send((request) =>
        {
            string thing = request.response.Text;
            method(thing, someRequest.response.status, param);
        });

    }

    public static void sendRequest(string type, string url, function4param method, int param1, string param2, WWWForm body = null)
    {
        UnityHTTP.Request someRequest = Request(type, url, body);
        someRequest.Send((request) =>
        {
            string thing = request.response.Text;
            method(thing, someRequest.response.status, param1, param2);
        });

    }

    public static void sendRequest(string type, string url, function2param method, byte[] body)
    {
        UnityHTTP.Request someRequest = Request(type, url, null, body);
        someRequest.Send((request) =>
        {
            string thing = request.response.Text;
            method(thing, someRequest.response.status);
        });
    }

    public static void sendRequest(string type, string url, function2param method, byte[] body, string customHeader)
    {
        UnityHTTP.Request someRequest = Request(type, url, null, body, customHeader);
        someRequest.Send((request) =>
        {
            string thing = request.response.Text;
            method(thing, someRequest.response.status);
        });
    }

    private static UnityHTTP.Request Request(string type, string url, WWWForm body = null, byte[] bodyByte = null, string customHeader = null)
    {
        UnityHTTP.Request someRequest = null;
        if (body != null)
        {
            someRequest = new UnityHTTP.Request(type, configScripts.server + url, body);
        }
        else if (bodyByte != null)
        {
            someRequest = new UnityHTTP.Request(type, configScripts.server + url, bodyByte);
        }
        else
        {
            someRequest = new UnityHTTP.Request(type, configScripts.server + url);
        }
        if (GameObject.Find("User"))
        {
            someRequest.SetHeader("Authorization", "Bearer " + GameObject.Find("User").GetComponent<UserScripts>().user.token.access_token);
        }
        someRequest.SetHeader("Content-Type", string.IsNullOrEmpty(customHeader) ?
                                              (type == "put" || type == "post" || type == "patch" ?
                                              "application/x-www-form-urlencoded" : "application/json") : customHeader);
        return someRequest;
    }

    public static void sendRequest(string type, string url, function1param method)
    {
        UnityHTTP.Request someRequest = Request(type, url);
        someRequest.Send((request) =>
        {
            string thing = request.response.Text;
            method(thing);

        });
    }

    public static IEnumerator getO(string url, function1param method, Dictionary<string, string> headers)
    {
        WWW www = new WWW(url, null, headers);
        yield return www;
        method(www.text);
        www.Dispose();
        www = null;
    }
}
