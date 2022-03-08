using UnityEngine;

public class BeginPlaying : MonoBehaviour
{
    public static int idPuzzle;
    public static string idCategory = "";
    public static string photoPuzzleName = "";
    public static int totalPuzzles;
    public static PuzzlesScript.Puzzles Puzzles;
    public static int PuzzleIndex;
    public static float categoryPrice = 0;

    public void TryAgain()
    {
        AudioScripts.Click();
        UIManagerScript.StartLoader();
        APIMethodsScript.sendRequest("get", "/api/puzzle/" + Puzzles.puzzles[PuzzleIndex].id, CategoryPopUp1.PopUpClassObject.setImage);
    }

    public void ShowDifficultyPopUp()
    {
        AudioScripts.Click();
        UIManagerScript.StartPopUp("Difficulty");
    }
}
