using UnityEngine;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    string save;
    public GameObject answerSaveComplite;

    public void ShowBackgroundCustomization()
    {
        AudioScripts.Click();
        ClosePausePopUp();
        GameObject Prefab = (GameObject)Resources.Load("Prefab/BackgroundCustomizer", typeof(GameObject));
        GameObject cust = (GameObject)Instantiate(Prefab);
        cust.transform.parent = GameObject.Find("Canvas").transform;
        cust.name = "BackgroundCustomizer";
        cust.transform.SetAsLastSibling();
        cust.transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        cust.transform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        cust.transform.localPosition = new Vector3(0, 0, 0);
        cust.transform.localScale = new Vector3(1, 1, 1);
    }

    public void PausePlaying()
    {
        AudioScripts.Click();
        if (!PlayingScripts.finish)
        {
            if (GameObject.Find("Results") == null && GameObject.Find("PausePopUp") == null)
            {
                Save();
                UIManagerScript.StartPopUp("PausePopUp");
            }
        }
        else
        {
            UIManagerScript.LoadScene("puzzles");
        }
    }

    public void Save()
    {
        SavePuzzle();
        WWWForm body = new WWWForm();
        body.AddField("percentage_completed", (int)(100 - 100 * (float)PlayingScripts.PiecesLeft / PlayingScripts.TotalPieces));
        body.AddField("piece_count", PlayingScripts.TotalPieces);
        body.AddField("time_spent", Mathf.Round(PlayingScripts.gameTime).ToString());
        if (BeginPlaying.Puzzles != null)
        {
            APIMethodsScript.sendRequest("patch", "/api/user/puzzle/" + BeginPlaying.Puzzles.puzzles[BeginPlaying.PuzzleIndex].id, getRes, body);
        }
    }

    public void ClosePausePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }

    public void getRes(string json, int state)
    {
        byte[] pData = System.Text.Encoding.ASCII.GetBytes(save);
        APIMethodsScript.sendRequest("post", "/api/user/puzzle/" + BeginPlaying.Puzzles.puzzles[BeginPlaying.PuzzleIndex].id + "/coordinates", getUpdate, pData, "application/json");
    }

    private void SavePuzzle()
    {
        int paCount = GameObject.Find("PuzzleArea").transform.childCount;
        int pcCount = GameObject.Find("Pieces").transform.Find("PiecesCollection").transform.childCount;
        int inCount = GameObject.Find("Installed").transform.childCount;
        Coordinates[] coords = new Coordinates[paCount + pcCount + inCount];
        int c = 0;
        foreach (Transform piece in GameObject.Find("PuzzleArea").transform)
        {
            if (piece.name != "Mask" && piece.name != "Installed")
            {
                coords[c] = new Coordinates(int.Parse(piece.name), piece.localPosition.x, piece.localPosition.y, piece.GetComponent<RawImage>().texture.name, "", "", 0);
                c++;
            }
        }
        foreach (Transform piece in GameObject.Find("PuzzleArea").transform.Find("Installed"))
        {
            coords[c] = new Coordinates(int.Parse(piece.name), piece.localPosition.x, piece.localPosition.y, piece.GetComponent<RawImage>().texture.name, "", "", 0);
            c++;
        }
        foreach (Transform piece in GameObject.Find("Pieces").transform.Find("PiecesCollection").transform)
        {
            coords[c] = new Coordinates(int.Parse(piece.name), piece.GetComponent<RawImage>().texture.name, "", "", 0);
            c++;
        }
        save = "{\"coordinates\":[";
        int j = 0;
        while (j < paCount + inCount + pcCount && j < coords.Length)
        {
            if (coords[j] != null)
            {
                if (j != 0)
                    save += ",";
                save += "{\"piece_id\":" + coords[j].piece_id +
                        (coords[j].x != null ? ",\"x\":" + coords[j].x : string.Empty) +
                        (coords[j].y != null ? ",\"y\":" + coords[j].y : string.Empty) +
                        ",\"form\":\"" + coords[j].form + "\"}";
            }
            j++;
        }
        save = save + "]}";
    }

    public void getUpdate(string json, int state)
    {
        SavePuzzleProgress.UpdateResponse res = JsonUtility.FromJson<SavePuzzleProgress.UpdateResponse>(json);
        if (answerSaveComplite != null)
        {
            answerSaveComplite.SendMessage("SaveComplite");
        }
    }

    public void ToMenu()
    {
        AudioScripts.Click();
        CategoryPopUp1.prevImageIndex = null;
        BeginPlaying.idCategory = "";
        BeginPlaying.idPuzzle = 0;
        BeginPlaying.PuzzleIndex = 0;
        BeginPlaying.photoPuzzleName = "";
        BeginPlaying.totalPuzzles = 0;
        BeginPlaying.Puzzles = null;
        UIManagerScript.LoadScene("puzzles");
    }

    [System.Serializable]
    public class Coordinates
    {
        public int piece_id;
        public float x;
        public float y;
        public string form;
        public string created_at;
        public string updated_at;
        public int history_id;

        public Coordinates()
        {
            piece_id = 0;
            form = "";
            created_at = "";
            updated_at = "";
            history_id = 0;
        }

        public Coordinates(int id, float x_, float y_, string form_, string created_at_, string updated_at_, int history_id_)
        {
            piece_id = id;
            x = x_;
            y = y_;
            form = form_;
            created_at = created_at_;
            updated_at = updated_at_;
            history_id = history_id_;
        }

        public Coordinates(int id, string form_, string created_at_, string updated_at_, int history_id_)
        {
            piece_id = id;
            form = form_;
            created_at = created_at_;
            updated_at = updated_at_;
            history_id = history_id_;
        }

        public Coordinates(Coordinates other)
        {
            piece_id = other.piece_id;
            x = other.x;
            y = other.y;
            x = other.x;
            y = other.y;
            form = other.form;
            created_at = other.created_at;
            updated_at = other.updated_at;
            history_id = other.history_id;
        }
    }
}
