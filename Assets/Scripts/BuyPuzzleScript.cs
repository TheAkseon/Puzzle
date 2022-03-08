using UnityEngine;
using UnityEngine.UI;

public class BuyPuzzleScript : MonoBehaviour
{
    public static int idPuzzle;
    public static string idCategory;
    public static PuzzlesScript.Puzzles Puzzles;
    public static int puzzleIndex;

    public void BuyPuzzle()
    {
        AudioScripts.Click();
        puzzleIndex = BeginPlaying.PuzzleIndex;
        idPuzzle = BeginPlaying.idPuzzle;
        idCategory = BeginPlaying.idCategory;
        APIMethodsScript.sendRequest("post", "/api/puzzle/" + Puzzles.puzzles[puzzleIndex].id + "/buy", checkPurchase);
        UIManagerScript.StartLoader();
    }

    public void BuyCategory()
    {
        AudioScripts.Click();
        APIMethodsScript.sendRequest("post", "/api/category/" + BeginPlaying.idCategory + "/buy", checkPurchase);
        UIManagerScript.StartLoader();
    }

    public void checkPurchase(string json, int state)
    {
        BuyPuzzles.BuyPuzzleResponse resp = JsonUtility.FromJson<BuyPuzzles.BuyPuzzleResponse>(json);
        if (state == 200)
        {
            UserScripts u = GameObject.Find("User").GetComponent<UserScripts>();
            GameObject.Find("MainAudio").GetComponent<AudioScripts>().ClickSound(5);
            if (resp.puzzles == null)
            {
                Puzzles.puzzles[puzzleIndex].is_unlocked = true;
            }
            else
            {
                foreach (BuyPuzzles.BuyPuzzleClass p in resp.puzzles)
                {
                    for (int i = 0; i < Puzzles.puzzles.Count; i++)
                    {
                        if (p.puzzle_id == Puzzles.puzzles[i].id)
                        {
                            Puzzles.puzzles[i].is_unlocked = true;
                        }
                    }
                }
            }
            UIManagerScript.StartAchievment(resp.achievments);
            if (resp.currencies != null)
            {
                foreach (BuyPuzzles.Currency c in resp.currencies)
                {
                    u.user.setCurrency(c.currency_id, c.count);
                }
                UIManagerScript.LerpCoins(u.user.getCurrency(resp.currencies[0].currency_id), GameObject.Find("Coins").transform.Find("Text").GetComponent<Text>());
            }
            CategoryPopUp1.PopUpClassObject.DrawButtons(Puzzles.puzzles[BeginPlaying.PuzzleIndex]);
        }
        else
        {
            CoinsScript.getCoinsPriceListPopUp();
        }
        Destroy(GameObject.Find("Loader"));
    }

    public class BuyResponse
    {
        public string message;
        public PuzzleScript.Puzzle puzzle;

        public BuyResponse(string message)
        {
            this.message = message;
            this.puzzle = null;
        }

        public BuyResponse(string message, PuzzleScript.Puzzle puzzle)
        {
            this.message = message;
            this.puzzle = puzzle;
        }
    }
}
