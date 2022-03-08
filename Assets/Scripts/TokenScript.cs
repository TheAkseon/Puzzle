using System.Collections;
using UnityEngine;

public class TokenScript : MonoBehaviour
{
    [System.Serializable]
    public class Errors
    {
        public string[] email;
        public string[] username;
    }

    [System.Serializable]
    public class Token
    {
        public string token_type = null;
        public string expires_in = null;
        public string access_token = null;
        public string refresh_token = null;
        public string error = null;
        public string message = null;
        public int success = 1;
        public Errors errors;

        public Token(int success, string token_type, string expires_in, string access_token, string refresh_token)
        {
            this.token_type = token_type;
            this.expires_in = expires_in;
            this.access_token = access_token;
            this.refresh_token = refresh_token;
            this.success = success;
        }

        public Token(Errors e, string m)
        {
            this.errors = e;
            this.message = m;
        }

        public Token(int s, string e, string m)
        {
            this.error = e;
            this.success = s;
            this.message = m;
        }

        public Token(Token other)
        {
            this.token_type = other.token_type;
            this.expires_in = other.expires_in;
            this.access_token = other.access_token;
            this.refresh_token = other.refresh_token;
            this.error = other.error;
            this.message = other.message;
            this.success = other.success;
            errors = other.errors;
        }

        public void saveToken(string prefix = "")
        {
            string text = "{\"token_type\":\"" + this.token_type + "\",\"expires_in\":" + this.expires_in + ",\"access_token\":\"" + this.access_token + "\",\"refresh_token\":\"" + this.refresh_token + "\"}";
            string hashString = Md5Sum(text);
            if (!System.IO.File.Exists(Application.persistentDataPath + "/" + prefix + "token.txt"))
            {
                string cText = text + "\r\n" + hashString;
                System.IO.File.WriteAllText(Application.persistentDataPath + "/" + prefix + "token.txt", cText);
            }
        }

        public void deleteToken()
        {
            if (System.IO.File.Exists(Application.persistentDataPath + "/token.txt"))
            {
                System.IO.File.Delete(Application.persistentDataPath + "/token.txt");
                if (GameObject.Find("User"))
                    Destroy(GameObject.Find("User"));
                if (!GameObject.Find("User"))
                    GameObject.Find("UIManager").GetComponent<UIManagerScript>().DefUser();
                else
                    GameObject.Find("UIManager").GetComponent<UIManagerScript>().StartCoroutine(Wait(0.5f));
            }
        }
    }

    private static IEnumerator Wait(float t)
    {
        yield return new WaitForSeconds(t);
        if (!GameObject.Find("User"))
        {
            GameObject.Find("UIManager").GetComponent<UIManagerScript>().DefUser();
        }
        else
        {
            Destroy(GameObject.Find("User"));
            GameObject.Find("UIManager").GetComponent<UIManagerScript>().StartCoroutine(Wait(t));
        }
    }

    public static string Md5Sum(string strToEncrypt)
    {
        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
        // encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }
}
