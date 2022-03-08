using UnityEngine;

public class DifficultyExplainedPopUpScripts : MonoBehaviour
{
    public void Close(string name)
    {
        UIManagerScript.StartPopUp("Difficulty");
        UIManagerScript.ClosePopUp(name);
    }
}
