using UnityEngine;

public class HelpResetPopUp : MonoBehaviour
{
    public void StardPopUp()
    {
        UIManagerScript.StartPopUp("HelpResetPopUp");
    }

    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }

    public void HelpReset()
    {
        PlayerPrefs.DeleteKey("tutorial");
        UIManagerScript.tutorial = new TutorialClass();
        UIManagerScript.LoadScene("main");
    }
}
