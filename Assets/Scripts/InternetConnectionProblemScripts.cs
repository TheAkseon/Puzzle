using UnityEngine;

public class InternetConnectionProblemScripts : MonoBehaviour
{
    public delegate void function0param();
    public static function0param method;

    public static void setMethod(function0param f)
    {
        method = f;
    }

    public void TryAgain()
    {
        ClosePopUp();
        method();
    }

    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }
}
