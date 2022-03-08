using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyPhotoPuzzleActions : MonoBehaviour
{
    public void ShowDifficultyPopUp()
    {
        GameObject diff = UIManagerScript.StartPopUp("Difficulty", true, new Vector2(0, -95));
        diff.transform.Find("Explain").gameObject.SetActive(false);
        diff.transform.Find("Delete").gameObject.SetActive(true);
    }

    private int getId(string name)
    {
        //if (Application.isEditor)
        //{
        name = name.Substring((Application.persistentDataPath + "/photoPuzzles/").Length, name.Length - (Application.persistentDataPath + "/photoPuzzles/").Length);
        //}
        name = name.Substring(0, name.Length - ".jpg".Length);
        int id = int.Parse(name);
        return id;
    }

    public void ChoosePhotoPuzzle()
    {
        BeginPlaying.photoPuzzleName = EventSystem.current.currentSelectedGameObject.transform.parent.name;
        BeginPlaying.idPuzzle = getId(BeginPlaying.photoPuzzleName);
        BeginPlaying.PuzzleIndex = getId(BeginPlaying.photoPuzzleName);
        var info = new DirectoryInfo(Application.persistentDataPath + "/photoPuzzles");
        var fileInfo = info.GetFiles();
        BeginPlaying.totalPuzzles = fileInfo.Length;
        BeginPlaying.idCategory = "";
        ShowDifficultyPopUp();
        AudioScripts.Click();
    }
}
