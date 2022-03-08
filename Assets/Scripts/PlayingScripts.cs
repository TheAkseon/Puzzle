using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayingScripts : MonoBehaviour
{
    private float MaxWidth;
    private static float timeTotal;
    public static float timeLeft;
    private bool inTime = true;
    public static int PiecesLeft;
    public static int TotalPieces;
    private static Vector3 ClickPos;
    public static bool finish = false;
    public AchievmentsScripts.AchievmentList achiev;
    private System.DateTime lastTouch;
    public Sprite gold;
    public Sprite silver;
    public Sprite bronze;
    public Sprite gold_glow;
    public Sprite silver_glow;
    public Sprite bronze_glow;
    public Tutorial[] tutorials;
    private bool waitingAchiev = false;
    public static float gameTime = 0;

    public void getCups(string json, int state)
    {
        AchievmentsScripts.UserAchievmentList Cups = JsonUtility.FromJson<AchievmentsScripts.UserAchievmentList>(json);
        GameObject res = UIManagerScript.StartPopUp("Results");
        if (BeginPlaying.PuzzleIndex + 1 >= BeginPlaying.Puzzles.puzzles.Count)
        {
            GameObject.Find("Results").transform.Find("next").gameObject.SetActive(false);
        }
        Sprite Cup = null;
        Sprite Glow = null;
        if (Cups.timedAchievments[Cups.timedAchievments.Length - 1].puzzle_id == BeginPlaying.idPuzzle)
        {
            switch (Cups.timedAchievments[Cups.timedAchievments.Length - 1].type_id)
            {
                case 1:
                    Cup = gold;
                    Glow = gold_glow;
                    break;
                case 2:
                    Cup = silver;
                    Glow = silver_glow;
                    break;
                case 3:
                    Cup = bronze;
                    Glow = bronze_glow;
                    break;
            }
            res.transform.Find("Glow").transform.Find("Cup").GetComponent<Image>().sprite = Cup;
            res.transform.Find("Glow").GetComponent<Image>().sprite = Glow;
        }
        else
        {
            Destroy(res.transform.Find("Glow").gameObject);
        }
    }

    public static void SetPuzzleInfo(ResponsePuzzle.PuzzleResponse pz)
    {
        if (GameObject.Find("puzzleInformation") != null)
            Destroy(GameObject.Find("puzzleInformation").gameObject);
        GameObject puzzleInfo = new GameObject("puzzleInformation");
        DontDestroyOnLoad(puzzleInfo);
        puzzleInfo.AddComponent<puzzleInformationScripts>();
        if (BeginPlaying.idCategory != "" && BeginPlaying.photoPuzzleName == "" && pz != null)
            puzzleInfo.GetComponent<puzzleInformationScripts>().puzzleInfo = new puzzleInformationScripts.PuzzleInformation(BeginPlaying.PuzzleIndex, pz.puzzle.category_id, pz.puzzle.id, BeginPlaying.Puzzles);
        else
            puzzleInfo.GetComponent<puzzleInformationScripts>().puzzleInfo = new puzzleInformationScripts.PuzzleInformation(BeginPlaying.PuzzleIndex, 0, BeginPlaying.idPuzzle, BeginPlaying.Puzzles);

    }

    public void getRes(string json, int state)
    {
        PuzzleCompleted.PuzzleCompletedResult result = JsonUtility.FromJson<PuzzleCompleted.PuzzleCompletedResult>(json);
        finish = true;
        GameObject res = UIManagerScript.StartPopUp("Results");
        if (BeginPlaying.idCategory != "" && BeginPlaying.photoPuzzleName == "" && BeginPlaying.PuzzleIndex + 1 >= BeginPlaying.Puzzles.puzzles.Count)
            res.transform.Find("next").gameObject.SetActive(false);
        if (BeginPlaying.idCategory == "" && BeginPlaying.photoPuzzleName != "")
        {
            res.transform.Find("again").gameObject.SetActive(false);
            res.transform.Find("next").gameObject.SetActive(false);
            res.transform.Find("menu").transform.localPosition = new Vector3(0, res.transform.Find("menu").transform.localPosition.y);
        }
        Sprite Cup = null;
        Sprite Glow = null;
        Transform pause = GameObject.Find("Pause").transform;
        pause.Find("Image").gameObject.SetActive(false);
        pause.Find("Arrow").gameObject.SetActive(true);
        GameObject GlowObj = res.transform.Find("Glow").gameObject;
        if (result != null && result.timedAchievment != null)
        {
            UIManagerScript.StartAchievment(result.achievments);
            switch (result.timedAchievment.type_id)
            {
                case 1:
                    Cup = gold;
                    Glow = gold_glow;
                    break;
                case 2:
                    Cup = silver;
                    Glow = silver_glow;
                    break;
                case 3:
                    Cup = bronze;
                    Glow = bronze_glow;
                    break;
                default:
                    Destroy(GlowObj);
                    break;
            }
            if (GlowObj != null)
            {
                GlowObj.transform.Find("Cup").GetComponent<Image>().sprite = Cup;
                GlowObj.GetComponent<Image>().sprite = Glow;
                if (Glow != null && Cup != null)
                {
                    GameObject.Find("MainAudio").GetComponent<AudioScripts>().ClickSound(4);
                }
                UserScripts u = GameObject.Find("User").GetComponent<UserScripts>();
                int cCoins = u.user.user_currencies[0].count;
                for (int i = 0; i < result.currencies.Length; i++)
                {
                    u.user.setCurrency(result.currencies[i].currency_id, result.currencies[i].count);
                }
                UIManagerScript.LerpCoins((result.currencies[0].count - cCoins), res.transform.Find("Coins").transform.Find("Text").GetComponent<Text>());
            }
        }
        else if (result == null || (result.timedAchievment == null || result.timedAchievment.type_id == 0))
        {
            Destroy(GlowObj);
        }
        res.transform.Find("NotCup").gameObject.SetActive(GlowObj == null);
        GameObject.Find("time").GetComponent<Text>().text = GetTime(gameTime);
    }

    public void ShowResults()
    {
        WWWForm body = new WWWForm();
        body.AddField("percentage_completed", 100);
        body.AddField("piece_count", TotalPieces);
        body.AddField("time_spent", Mathf.Round(gameTime).ToString());
        if (GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.idCategory.ToString() != "" && BeginPlaying.photoPuzzleName == "")
        {
            APIMethodsScript.sendRequest("patch", "/api/user/puzzle/" + BeginPlaying.Puzzles.puzzles[BeginPlaying.PuzzleIndex].id + "/complete", getRes, body);
        }
        else
        {
            getRes("", 200);
        }
    }

    public void getTime(string json, int status)
    {
        achiev = JsonUtility.FromJson<AchievmentsScripts.AchievmentList>(json);
        waitingAchiev = true;
        if (achiev.getTime(TotalPieces) != null)
        {
            timeTotal = achiev.getTime(TotalPieces)[getServerComplexity(DifficultyScript.complexity)];
        }
        else
        {
            timeTotal = 1;
        }
        timeLeft += timeTotal;
        MaxWidth = GameObject.Find("TimeLeftBar").GetComponent<RectTransform>().offsetMax.x;
        Texture background = (Texture)Resources.Load("Images/Backgrounds/" + GameObject.Find("User").GetComponent<UserScripts>().user.background_id);
        GameObject.Find("Background").GetComponent<RawImage>().texture = background;
        PopUpDarkScripts.layers = 0;
    }

    private void getSpentTime(string json, int status)
    {
        ResponsePuzzle.currentPuzzle pz = JsonUtility.FromJson<ResponsePuzzle.currentPuzzle>(json);
        gameTime = pz.userPuzzle.active_puzzle.time_spent;
        timeLeft = -pz.userPuzzle.active_puzzle.time_spent;
        APIMethodsScript.sendRequest("get", "/api/timedAchievment", getTime);
    }

    private void Start()
    {
        finish = false;
        gameTime = 0;
        APIMethodsScript.sendRequest("get", "/api/user/puzzle/" + BeginPlaying.idPuzzle, getSpentTime);
    }

    private bool clicked = false;
    private GameObject current;

    public void PuzzlePieceClicked()
    {
        clicked = true;
        GameObject.Find("Pieces").GetComponent<ScrollRect>().vertical = true;
        if (current != null && current.GetComponent<Button>() != null && current.GetComponent<Button>().enabled)
        {
            lastTouch = System.DateTime.Now;
            if (current.transform.parent && current.transform.parent.name == "PuzzleArea")
            {

                //right
                if (current.transform.GetComponent<PuzzlePieceScripts>().stickedPuzzles.Count == 0 && current.transform.position.x > GameObject.Find("Pieces").transform.position.x - GameObject.Find("Pieces").GetComponent<RectTransform>().sizeDelta.x / 3)
                {
                    float new_width = PuzzlePieceScripts.small_width < PuzzlePieceScripts.big_width ? PuzzlePieceScripts.small_width : PuzzlePieceScripts.big_width;
                    float new_height = PuzzlePieceScripts.small_width < PuzzlePieceScripts.big_width ? PuzzlePieceScripts.small_height : PuzzlePieceScripts.big_height;

                    current.GetComponent<RectTransform>().localScale = new Vector2(new_width / current.GetComponent<RectTransform>().sizeDelta.x, new_height / current.GetComponent<RectTransform>().sizeDelta.y);

                    current.transform.parent = GameObject.Find("Pieces").transform.Find("PiecesCollection").transform;
                }
            }
            List<int> checkedPieces = new List<int>();
            current.transform.GetComponent<PuzzlePieceScripts>().CheckPosition(checkedPieces);
            if (current.transform.parent && current.transform.parent.name == "PiecesCollection")
            {
                current = null;
            }
            PuzzleAreaScript.ShiftPuzzle();
        }
    }

    public void moveSticked(Transform current, float dx, float dy, List<int> moved)
    {
        if (!moved.Contains(int.Parse(current.name)))
        {
            moved.Add(int.Parse(current.name));
            foreach (int piece in current.GetComponent<PuzzlePieceScripts>().stickedPuzzles)
            {
                if (!moved.Contains(piece))
                {
                    GameObject.Find("PuzzleArea").transform.Find((piece).ToString()).transform.position = new Vector3(GameObject.Find("PuzzleArea").transform.Find((piece).ToString()).transform.position.x + dx, GameObject.Find("PuzzleArea").transform.Find((piece).ToString()).transform.position.y + dy, 0);
                    GameObject.Find("PuzzleArea").transform.Find((piece).ToString()).transform.SetAsLastSibling();
                }
                moveSticked(GameObject.Find("PuzzleArea").transform.Find((piece).ToString()), dx, dy, moved);
            }
        }
    }

    private void Update()
    {
        if (PiecesLeft >= 0)
        {
            if (PiecesLeft == 0)
            {
                clicked = false;
                StartCoroutine(Wait(1));
                PiecesLeft = -1;
            }
            else
            {
                if (/*(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || */(Input.GetMouseButtonUp(0)))
                {
                    clicked = true;
                    PuzzlePieceClicked();
                    if (current != null && current.GetComponent<PuzzlePieceScripts>().stickedPuzzles.Count == 0 && current.transform.parent.name == "PuzzleArea")
                    {
                        float area_width = transform.parent.GetComponent<RectTransform>().sizeDelta.x - transform.GetComponent<RectTransform>().offsetMin.x + transform.GetComponent<RectTransform>().offsetMax.x;
                        float area_height = transform.parent.GetComponent<RectTransform>().sizeDelta.y - transform.GetComponent<RectTransform>().offsetMin.y + transform.GetComponent<RectTransform>().offsetMax.y;
                        //forbidding flying over the right side of area
                        float add_left = (current.GetComponent<RawImage>().texture.name.ToCharArray()[3] != '1') ? 22 * PuzzleAreaScript.koef[0] : 0;
                        if (current.transform.localPosition.x - current.GetComponent<RectTransform>().sizeDelta.x / 2 + add_left < -area_width / 2)
                            current.transform.localPosition = new Vector3(0, 0);
                        //forbidding flying over the top side of area
                        float add_top = (current.GetComponent<RawImage>().texture.name.ToCharArray()[0] != '1') ? 22 * PuzzleAreaScript.koef[1] : 0;
                        if (-add_top + current.transform.localPosition.y + current.GetComponent<RectTransform>().sizeDelta.y / 2 > area_height / 2)
                            current.transform.localPosition = new Vector3(0, 0);
                    }
                    current = null;
                }
                if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.parent && (EventSystem.current.currentSelectedGameObject.transform.parent.name == "PuzzleArea" || EventSystem.current.currentSelectedGameObject.transform.parent.name == "PiecesCollection") && (EventSystem.current.currentSelectedGameObject.GetComponent<Button>().colors.pressedColor == EventSystem.current.currentSelectedGameObject.GetComponent<Button>().GetComponent<CanvasRenderer>().GetColor() || !clicked))
                {
                    clicked = false;
                    EventSystem.current.currentSelectedGameObject.transform.SetAsLastSibling();

                    current = EventSystem.current.currentSelectedGameObject;
                    float dx = Input.mousePosition.x - current.transform.position.x;
                    float dy = Input.mousePosition.y - current.transform.position.y;
                    current.transform.position = Input.mousePosition;
                    moveSticked(current.transform, dx, dy, new List<int>());
                    if (current.transform.parent.name == "PiecesCollection")
                    {
                        GameObject.Find("Pieces").GetComponent<ScrollRect>().vertical = false;
                        float new_width = PuzzlePieceScripts.big_width;
                        float new_height = PuzzlePieceScripts.big_height;
                        current.GetComponent<RectTransform>().localScale = new Vector2(new_width / current.GetComponent<RectTransform>().sizeDelta.x, new_height / current.GetComponent<RectTransform>().sizeDelta.y);
                        current.transform.parent = GameObject.Find("PuzzleArea").transform;
                        PuzzleAreaScript.ShiftPuzzle();
                    }
                }
                else if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.transform.parent && EventSystem.current.currentSelectedGameObject.transform.parent)
                {
                    clicked = true;

                }
                GameObject.Find("TimeLeft").GetComponent<Text>().text = GetTime(inTime ? timeLeft : gameTime);
                if (GameObject.Find("Results") == null && GameObject.Find("PausePopUp") == null && GameObject.Find("BackgroundCustomizer") == null && waitingAchiev && !tutorials[0].enabled && !tutorials[1].enabled)
                {
                    gameTime += Time.deltaTime;
                    if (inTime && timeLeft < 0)
                    {
                        inTime = false;
                        GameObject.Find("TimeLeftBar").GetComponent<Image>().color = Color.red;
                    }
                    timeLeft += Time.deltaTime * (inTime ? -1 : 1);
                    RectTransform TimeLeftBar = GameObject.Find("TimeLeftBar").GetComponent<RectTransform>();
                    RectTransform Canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
                    float allSize = (Canvas.sizeDelta.x - TimeLeftBar.offsetMin.x + MaxWidth);
                    float newRightBar = allSize - (allSize / timeTotal * timeLeft);
                    TimeLeftBar.offsetMax = new Vector2(inTime ? (MaxWidth - newRightBar) : MaxWidth, TimeLeftBar.offsetMax.y);
                }
            }
        }
    }

    public int getServerComplexity(string v)
    {
        return (v == "Easy" ? 2 : (v == "Medium" ? 1 : 0));
    }

    private string GetTime(float v)
    {
        string timeResult = string.Empty;
        float[] t = new float[3];
        t[0] = Mathf.Floor(v / 3600);
        t[1] = Mathf.Floor(v / 60 - t[0] * 60);
        t[2] = Mathf.Floor(v - t[0] * 3600 - t[1] * 60);
        for (int i = 0; i < 3; i++)
        {
            timeResult += ZeroTimer(t[i]) + (i < 2 ? ":" : string.Empty);
        }
        return timeResult;
    }

    private string ZeroTimer(float v)
    {
        return (v < 10 ? "0" : string.Empty) + v.ToString();
    }

    private IEnumerator Wait(int t)
    {
        yield return new WaitForSeconds(t);
        ShowResults();
    }
}
