using UnityEngine;

public class puzzleInformationScripts : MonoBehaviour
{
    public PuzzleInformation puzzleInfo = new PuzzleInformation();

    [System.Serializable]
    public class PuzzleInformation
    {
        public int indImage;
        public int idCategory;
        public int idPuzzle;
        public PuzzlesScript.Puzzles PuzzleList;

        public PuzzleInformation(int ind, int cat, int puzzle, PuzzlesScript.Puzzles list)
        {
            indImage = ind;
            idCategory = cat;
            idPuzzle = puzzle;
            PuzzleList = list;
        }

        public PuzzleInformation()
        {

        }
    }
}
