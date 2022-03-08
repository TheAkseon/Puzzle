using UnityEngine;

public class ContinueScript : MonoBehaviour
{
    public static int idPuzzle;
    public static string idCategory;

    public void setPieces(string json, int state)
    {
        CoordinatesArray coords = JsonUtility.FromJson<CoordinatesArray>(json);
        if (coords.coordinates != null)
        {
            PuzzleAreaScript.puzzles = new PuzzleAreaScript.PuzzlePiece[(PuzzleAreaScript.m + 1) * (PuzzleAreaScript.n + 1)];
            for (int i = 0; i < PuzzleAreaScript.puzzles.Length; i++)
            {
                PuzzleAreaScript.puzzles[i] = new PuzzleAreaScript.PuzzlePiece();
            }
            for (int i = 0; i < coords.coordinates.Length; i++)
            {
                PuzzleAreaScript.puzzles[coords.coordinates[i].piece_id].name = new char[coords.coordinates[i].form.ToCharArray().Length];
                for (int j = 0; j < coords.coordinates[i].form.ToCharArray().Length; j++)
                {
                    PuzzleAreaScript.puzzles[coords.coordinates[i].piece_id].name[j] = coords.coordinates[i].form.ToCharArray()[j];
                }

                PuzzleAreaScript.puzzles[coords.coordinates[i].piece_id].x = coords.coordinates[i].x;
                PuzzleAreaScript.puzzles[coords.coordinates[i].piece_id].y = coords.coordinates[i].y;
            }
            UIManagerScript.LoadScene("playing");
            PuzzleAreaScript.newGame = false;
        }
    }

    public void setPuzzle(string json, int state)
    {
        HistoryList h = JsonUtility.FromJson<HistoryList>(json);
        DifficultyScript.SetPieceCount(h.history[h.history.Length - 1].piece_count.ToString());
        DifficultyScript.complexity = h.history[h.history.Length - 1].puzzle_form;
        PlayingScripts.SetPuzzleInfo(null);
        APIMethodsScript.sendRequest("get", "/api/user/puzzle/" + idPuzzle + "/coordinates", setPieces);
    }

    public void ContinuePlaying()
    {
        AudioScripts.Click();
        APIMethodsScript.sendRequest("get", "/api/user/puzzle/" + idPuzzle + "/history", setPuzzle);
    }

    [System.Serializable]
    public class CoordinatesArray
    {
        public PauseScript.Coordinates[] coordinates;
    }

    [System.Serializable]
    public class History
    {
        public int id;
        public string created_at;
        public string updated_at;
        public int percentage_completed;
        public int piece_count;
        public string puzzle_form;
        public int time_spend;
        public int user_puzzle_id;

        public History()
        {
            id = 0;
            created_at = "";
            updated_at = "";
            percentage_completed = 0;
            piece_count = 0;
            puzzle_form = "Easy";
            time_spend = 0;
            user_puzzle_id = 0;
        }

        public History(int id_, string created_at_, string updated_at_, int percentage_completed_, int piece_count_, string puzzle_form_, int time, int user_puzzle_id_)
        {
            id = id_;
            created_at = created_at_;
            updated_at = updated_at_;
            percentage_completed = percentage_completed_;
            piece_count = piece_count_;
            puzzle_form = puzzle_form_;
            time_spend = time;
            user_puzzle_id = user_puzzle_id_;
        }
    }

    [System.Serializable]
    public class HistoryList
    {
        public History[] history;

        public HistoryList()
        {
            history = null;
        }

        public HistoryList(History[] histories)
        {
            history = new History[histories.Length];
            for (int i = 0; i < histories.Length; i++)
                history[i] = new History(histories[i].id, histories[i].created_at, histories[i].updated_at, histories[i].percentage_completed, histories[i].piece_count, histories[i].puzzle_form, histories[i].time_spend, histories[i].user_puzzle_id);

        }
    }
}
