using UnityEngine;

public class PushPopUp : MonoBehaviour
{
    public int puzzle;
    public int category;
    public int index = 0;
    public CategoryPopUp1 c;
    private puzzleInformationScripts pi;

    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }

    public void Click()
    {
        AudioScripts.Click();
        UIManagerScript.StartLoader();
        if (UIManagerScript.GetActiveScene() == "playing")
        {
            PauseScript p = GameObject.FindObjectOfType<PauseScript>();
            p.answerSaveComplite = gameObject;
            p.Save();
        }
        else
        {
            SaveComplite();
        }
    }

    public void getCategory(string json, int state)
    {
        PuzzlesScript.clist = JsonUtility.FromJson<PuzzlesScript.Categories>(json);
        for (int i = 0; i < PuzzlesScript.clist.categories.Length; i++)
        {
            if (PuzzlesScript.clist.categories[i].id == category)
            {
                BeginPlaying.categoryPrice = PuzzlesScript.clist.categories[i].priceFull;
            }
        }
        if (puzzle != 0)
        {
            APIMethodsScript.sendRequest("get", "/api/puzzle?category_id=" + category, getPuzzlesInCateCategory);
        }
        else
        {
            Continue();
        }
    }

    public void getPuzzlesInCateCategory(string json, int state)
    {
        PuzzlesScript.Puzzles PuzzlesList = JsonUtility.FromJson<PuzzlesScript.Puzzles>(json);
        PuzzlesList.puzzles.Sort(new PuzzlesComparer());
        pi.puzzleInfo.PuzzleList = PuzzlesList;
        if (puzzle != 0)
        {
            for (int i = 0; i < PuzzlesList.puzzles.Count; i++)
            {
                if (PuzzlesList.puzzles[i].id == puzzle)
                {
                    index = i;
                    BeginPlaying.PuzzleIndex = i;
                    pi.puzzleInfo.indImage = i;
                    Continue();
                    break;
                }
            }
        }
        else
        {
            Continue();
        }
    }

    private void SaveComplite()
    {
        Destroy(GameObject.Find("puzzleInformation"));
        GameObject puzzleInfo = new GameObject("puzzleInformation");
        DontDestroyOnLoad(puzzleInfo);
        pi = puzzleInfo.AddComponent<puzzleInformationScripts>();
        pi.puzzleInfo.idCategory = category;
        pi.puzzleInfo.idPuzzle = puzzle;
        APIMethodsScript.sendRequest("get", "/api/category", getCategory);
    }

    private void Continue()
    {
        Destroy(GameObject.Find("Loader"));
        UIManagerScript.LoadScene("puzzleInfo");
        c.OnCategoryClick();
    }
}
