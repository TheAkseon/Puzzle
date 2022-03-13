using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using Ads;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DifficultyScript : MonoBehaviour
{
    public Sprite inactive;
    public Sprite active;
    public static string complexity = "Easy";
    public string pieceCount = "66";
    private int level = 0;
    public static int levels_count;
    public Sprite[] diff_Sprites = new Sprite[levels_count];
    private ResponsePuzzle.PuzzleResponse pz;
    private AchievmentsScripts.timedAchievmentList Types;

    public const string IS_ADS_OFF_KEY = "{2F66F0C6-1680-4363-A75D-A14FDF14A60D}";

    public void Explain()
    {
        UIManagerScript.StartPopUp("DifficultyExplainedPopUp");
        UIManagerScript.ClosePopUp(name);
    }

    public void Delete()
    {
        AudioScripts.Click();
        File.Delete(Application.persistentDataPath + "/photoPuzzles/" + BeginPlaying.idPuzzle + ".jpg");
        UIManagerScript.LoadScene("mypuzzles");
    }

    public void CloseDifficultyPopUp()
    {
        AudioScripts.Click();
        UIManagerScript.ClosePopUp("Difficulty");
    }

    public void ShowPuzzleSolving(string json, int state)
    {
        AudioScripts.Click();
        if (state == 200)
        {
            PlayingScripts.SetPuzzleInfo(pz);
            UIManagerScript.LoadScene("playing");
            CategoryPopUp1.PopUpClassObject.CategoryId = "";
            CategoryPopUp1.PopUpClassObject.PuzzlesList = null;
            CategoryPopUp1.PopUpClassObject.ImageIndex = 0;
            PuzzleAreaScript.newGame = true;
        }
    }

    private IEnumerator WaitForImage(float t, int id, int category)
    {
        yield return new WaitForSeconds(t);
        if (!File.Exists(Application.persistentDataPath + "/cacheImage/" + category + "/big" + id + ".jpg"))
        {
            WaitForImage(t, id, category);
        }
        else
        {
            NewGame();
        }
    }

    private void NewGame()
    {
        UIManagerScript.StartLoader();
        if (pz != null && pz.puzzle.category_id.ToString() != "" && BeginPlaying.photoPuzzleName == "")
        {
            WWWForm body = new WWWForm();
            body.AddField("puzzle_form", complexity);
            body.AddField("tachievment_type_id", getServerComplexity(level));
            APIMethodsScript.sendRequest("post", "/api/user/puzzle/" + BeginPlaying.idPuzzle + "/start", ShowPuzzleSolving, body);
        }
        else
        {
            ShowPuzzleSolving("", 200);
        }
    }

    public void getImage(string json, int status)
    {
        GameObject l = UIManagerScript.StartLoader();
        pz = JsonUtility.FromJson<ResponsePuzzle.PuzzleResponse>(json);
        ImagesScript.getCachedWWW(pz.puzzle.image, l, null, pz.puzzle.id.ToString(), pz.puzzle.updated_at, pz.puzzle.category_id.ToString(), "big");
        StartCoroutine(WaitForImage(0.1f, pz.puzzle.id, pz.puzzle.category_id));
    }

    public void StartPlaying()
    {
        AudioScripts.Click();
        if (IsAdsOn())
        {
           //GoogleMobileAdsScript.Instance.ShowInterstitialAd(StartPlayingAfterAds);
            //GoogleMobileAdsScript.Show_interstitial();
            StartPlayingAfterAds();
        }
        else
        {
            StartPlayingAfterAds();
        }
    }

    private bool IsAdsOn()
    {
        // If ads string empty so we didn't off ads.
        var adsString = PlayerPrefs.GetString(IS_ADS_OFF_KEY, string.Empty);
        return string.IsNullOrEmpty(adsString);
    }

    private void StartPlayingAfterAds()
    {
        if (UIManagerScript.GetActiveScene() == "puzzleInfo")
        {
            defComplexity();
            APIMethodsScript.sendRequest("get", "/api/puzzle/" + BeginPlaying.idPuzzle, getImage);
        }
        else if (UIManagerScript.GetActiveScene() == "mypuzzles")
        {
            BeginPlaying.Puzzles = null;
            defComplexity();
            NewGame();
        }
    }

    public void setComplexity()
    {
        AudioScripts.Click();
        complexity = EventSystem.current.currentSelectedGameObject.name;
    }

    public void defComplexity()
    {
        switch (level)
        {
            case 0:
                complexity = "Easy";
                break;
            case 1:
                complexity = "Medium";
                break;
            case 2:
                complexity = "Hard";
                break;
        }
    }

    public static int getServerComplexity(int v)
    {
        return (v == 0 ? 3 : (v == 1 ? 2 : 1));
    }

    public void chooseLevel(int v)
    {
        AudioScripts.Click();
        level += v;
        DisplayLevel();
        defComplexity();
        SaveDiff();
    }

    public void choosePieceCount()
    {
        AudioScripts.Click();
        DisplayPieceCount(EventSystem.current.currentSelectedGameObject.name.Substring("PieceCount".Length), EventSystem.current.currentSelectedGameObject);
        SaveDiff();
    }

    private void DisplayPieceCount(string name, GameObject obj)
    {
        foreach (Transform option in GameObject.Find("Difficulty").transform.Find("PieceCountOptions").transform)
        {
            option.Find("PieceCount" + option.name).GetComponent<Image>().sprite = inactive;
            option.Find("PieceCount" + option.name).Find("Glow").transform.GetComponent<Image>().enabled = false;
        }
        obj.GetComponent<Image>().sprite = active;
        obj.transform.Find("Glow").transform.GetComponent<Image>().enabled = true;
        pieceCount = name;
        SetPieceCount(pieceCount);
    }

    private void DisplayLevel()
    {
        transform.Find("Less").GetComponent<Button>().enabled = level > 0;
        transform.Find("More").GetComponent<Button>().enabled = level < diff_Sprites.Length - 1;
        transform.Find("Bar").GetComponent<Image>().sprite = diff_Sprites[level];
    }

    private void Start()
    {
        string[] diff = PlayerPrefs.GetString("diffPuzzle").Split(";"[0]);
        if (diff.Length > 1)
        {
            pieceCount = diff[0];
            if (diff[1].Length == 1)
            {
                level = Convert.ToInt32(diff[1]);
            }
        }
        DisplayLevel();
        defComplexity();
        DisplayPieceCount(pieceCount, GameObject.Find(pieceCount).transform.Find("PieceCount" + pieceCount).gameObject);
        if (UIManagerScript.GetActiveScene() != "mypuzzles")
        {
            APIMethodsScript.sendRequest("get", "/api/timedAchievment", getTypes);
        }
    }

    private void SaveDiff()
    {
        PlayerPrefs.SetString("diffPuzzle", pieceCount + ";" + level);
    }

    public Dictionary<int, int> posPieceCount;
    public Dictionary<int, AchievmentsScripts.timeAchievmentType[]> posCups;

    public void getTypes(string json, int status)
    {
        Types = JsonUtility.FromJson<AchievmentsScripts.timedAchievmentList>(json);
        posPieceCount = new Dictionary<int, int>();
        for (int i = 0; i < Types.timedAchievments.Length; i++)
        {
            posPieceCount.Add(Types.timedAchievments[i].pieceCount.id, Types.timedAchievments[i].pieceCount.piece_count);
        }
        posCups = new Dictionary<int, AchievmentsScripts.timeAchievmentType[]>();
        for (int i = 0; i < Types.timedAchievments.Length; i++)
        {
            posCups.Add(Types.timedAchievments[i].pieceCount.id, Types.timedAchievments[i].types);

        }
        APIMethodsScript.sendRequest("get", "/api/user/timedAchievment/", getCups);
    }

    public void getCups(string json, int status)
    {
        if (this != null && gameObject != null && BeginPlaying.Puzzles != null)
        {
            AchievmentsScripts.timedAchievmentsList Cups = JsonUtility.FromJson<AchievmentsScripts.timedAchievmentsList>(json);
            System.Array.Sort(Cups.timedAchievments, delegate (AchievmentsScripts.timedAchievment x, AchievmentsScripts.timedAchievment y)
            {
                if (x.piece_count_id == y.piece_count_id) return x.type_id.CompareTo(y.type_id);
                return x.piece_count_id.CompareTo(y.piece_count_id);
            });
            if (Cups.timedAchievments.Length > 0)
            {
                int[] cCups = new int[Types.timedAchievments.Length];
                for (int i = 0; i < Cups.timedAchievments.Length; i++)
                {
                    if (Cups.timedAchievments[i].puzzle_id == BeginPlaying.Puzzles.puzzles[BeginPlaying.PuzzleIndex].id)
                    {
                        int pieceCountId = Cups.timedAchievments[i].piece_count_id;
                        int cCup = cCups[pieceCountId];
                        if (cCup == 0 || cCup != 0 && Cups.timedAchievments[i].type_id < cCup)
                        {
                            cCups[pieceCountId] = Cups.timedAchievments[i].type_id;
                            Image cCupSprite = GameObject.Find("PieceCountOptions").transform.Find(posPieceCount[pieceCountId].ToString()).transform.Find("Cup").transform.Find("Image").GetComponent<Image>();
                            cCupSprite.color = new Color(255, 255, 255, 1);
                            cCupSprite.sprite = (Sprite)Resources.Load("Images/Interface/Difficulty/small-trophy-" + posCups[pieceCountId][cCups[pieceCountId] - 1].name, typeof(Sprite));
                        }
                    }
                }
            }
        }
    }

    public static void SetPieceCount(string v)
    {
        switch (v)
        {
            case "9":
                {
                    PuzzleAreaScript.m = 3;
                    PuzzleAreaScript.n = 3;
                }
                break;
            case "12":
                {
                    PuzzleAreaScript.m = 4;
                    PuzzleAreaScript.n = 3;
                }
                break;
            case "28":
                {
                    PuzzleAreaScript.m = 7;
                    PuzzleAreaScript.n = 4;
                }
                break;
            case "66":
                {
                    PuzzleAreaScript.m = 11;
                    PuzzleAreaScript.n = 6;
                }
                break;
            case "104":
                {
                    PuzzleAreaScript.m = 13;
                    PuzzleAreaScript.n = 8;
                }
                break;
            case "150":
                {
                    PuzzleAreaScript.m = 15;
                    PuzzleAreaScript.n = 10;
                }
                break;
            case "198":
                {
                    PuzzleAreaScript.m = 18;
                    PuzzleAreaScript.n = 11;
                }
                break;
            case "260":
                {
                    PuzzleAreaScript.m = 20;
                    PuzzleAreaScript.n = 13;
                }
                break;
            case "384":
                {
                    PuzzleAreaScript.m = 24;
                    PuzzleAreaScript.n = 16;
                }
                break;
        }
    }
}
