using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LerpPiece
{
    public Transform obj;
    public Vector3 startPos;
    public Vector3 targetPos;
    public float startTime;
    public float dist;
    public float shift = 1f;

    public LerpPiece(GameObject p, Vector3 t)
    {
        obj = p.transform;
        startPos = p.transform.localPosition;
        targetPos = t;
    }
}

public class TutorialClass
{
    public bool[] tStats = new bool[12];
}

public class UIManagerScript : MonoBehaviour
{
    [SerializeField]
    private bool devCleaningPlayerPrefs = false;

    private GameObject regButton;
    private static LerpPiece cLerpPopUp;
    private static bool coinDisplay;
    public static TutorialClass tutorial = null;

    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        if (devCleaningPlayerPrefs)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
#else
            Application.Quit();
#endif
            return;
        }

        configScripts.LoadServer();
        if (PlayerPrefs.HasKey("tutorial"))
        {
            tutorial = JsonUtility.FromJson<TutorialClass>(PlayerPrefs.GetString("tutorial"));
        }
        if (tutorial == null)
        {
            tutorial = new TutorialClass();
        }
    }

    public void Start()
    {
       // Yodo1U3dAds.InitializeSdk();
        Application.targetFrameRate = 60;
        DefUser();
        if (GameObject.Find("MainAudio") == null)
        {
            GameObject AudioPrefab = (GameObject)Resources.Load("Prefab/MainAudio", typeof(GameObject));
            GameObject Audio = (GameObject)Instantiate(AudioPrefab);
            Audio.name = "MainAudio";
            DontDestroyOnLoad(Audio.gameObject);
        }
        if (PlayerPrefs.GetInt("deleteCache") == 1)
        {
            if (Directory.Exists(Application.persistentDataPath + "/cacheImage"))
            {
                Directory.Delete(Application.persistentDataPath + "/cacheImage", true);
            }
            PlayerPrefs.DeleteKey("deleteCache");
        }
    }

    public void DefUser()
    {
        if (!GameObject.Find("User") && (System.IO.File.Exists(Application.persistentDataPath + "/token.txt") || (System.IO.File.Exists(Application.persistentDataPath + "/guest_token.txt"))))
        {
            string prefix = "";
            if (System.IO.File.Exists(Application.persistentDataPath + "/guest_token.txt") &&
                !System.IO.File.Exists(Application.persistentDataPath + "/token.txt"))
            {
                prefix = "guest_";
            }
            string[] fileLines = System.IO.File.ReadAllLines(Application.persistentDataPath + "/" + prefix + "token.txt");
            if (TokenScript.Md5Sum(fileLines[0]) != fileLines[1])
            {
                System.IO.File.Delete(Application.persistentDataPath + "/" + prefix + "token.txt");
            }
            else
            {
                TokenScript.Token token = JsonUtility.FromJson<TokenScript.Token>(fileLines[0]);
                AuthPopUpScript.CreateUser(token, prefix);
            }
        }
        else
        {
            RenderGreetings();
        }
    }

    public void RenderGreetings()
    {
        GameObject userObj = GameObject.Find("User");
        coinDisplay = userObj && (GetActiveScene() != "main" && GetActiveScene() != "playing" && GetActiveScene() != "createMyPuzzle");
        if (coinDisplay)
        {
            CoinsAndPhotoPuzzles();
        }
        if (SceneManager.GetActiveScene().name == "main")
        {
            var userName = string.Empty;
            if (userObj)
            {
                userName = userObj.GetComponent<UserScripts>().user.name;
            }
            var guestLocalized = LocalizationManager.Instance.GetLocalizedValue("guest").ToLower();
            var youCanLooseDataLocalized = LocalizationManager.Instance.GetLocalizedValue("greetings");
            var youAuthorizedAsLocalized = LocalizationManager.Instance.GetLocalizedValue("authorized_as");
            bool noGuestStat = !string.IsNullOrEmpty(userName);
            GameObject.Find("Greetings").GetComponent<Text>().text = noGuestStat ? youAuthorizedAsLocalized + userName.ToUpper() : youCanLooseDataLocalized;
            if (regButton == null)
            {
                regButton = GameObject.Find("Register");
            }
            regButton.SetActive(userObj != null && !noGuestStat);
        }
        else if (!userObj)
        {
            StartPopUp("NotLoggedInWarningPopUp", true, new Vector2(-15, -53));
        }
    }

    private void Update()
    {
        if (cLerpPopUp != null)
        {
            if (cLerpPopUp.obj != null)
            {
                cLerpPopUp.obj.localPosition = CalculatePos(cLerpPopUp, Screen.height * 3, true);
            }

            if (cLerpPopUp.obj == null || cLerpPopUp.obj != null && cLerpPopUp.obj.localPosition == cLerpPopUp.targetPos)
            {
                cLerpPopUp = null;
            }
        }
    }

    public static string GetActiveScene()
    {
        return SceneManager.GetActiveScene() != null ? SceneManager.GetActiveScene().name : string.Empty;
    }

    public static void CoinsAndPhotoPuzzles()
    {
        Transform parent = GetParent();
        Transform Coins = parent.Find("Coins");
        Transform PhotoPuzzles = parent.Find("PhotoPuzzles");
        if (Coins == null)
        {
            Coins = StartPopUp("Coins", false, new Vector2(843, 715), 0).transform;
        }
        if (PhotoPuzzles == null)
        {
            PhotoPuzzles = StartPopUp("PhotoPuzzles", false, new Vector2(545, 715), 0).transform;
        }
        Purchaser.main.LoadPurchaser();
        UserScripts u = GameObject.Find("User").GetComponent<UserScripts>();
        Coins.Find("Text").GetComponent<Text>().text = u.user.user_currencies[0].count.ToString();
        PhotoPuzzles.Find("Text").GetComponent<Text>().text = u.user.user_currencies[1].count.ToString();
    }

    public static void LerpCoins(int coins, Text t)
    {
        GameObject.Find("UIManager").GetComponent<UIManagerScript>().StartCoroutine(LerpCoinsIEnumerator(coins, t));
    }

    public static IEnumerator LerpCoinsIEnumerator(int coins, Text t)
    {
        if (t != null)
        {
            int i = Convert.ToInt32(t.text);
            int d = i < coins ? 1 : -1;
            while (i != coins && t != null)
            {
                int diff = Mathf.Abs(i - coins);
                if (diff > 1000)
                {
                    i += 500 * d;
                }
                else if (diff > 500)
                {
                    i += 250 * d;
                }
                else if (diff > 100)
                {
                    i += 50 * d;
                }
                else if (diff > 10)
                {
                    i += 5 * d;
                }
                else
                {
                    i += d;
                }
                t.text = i.ToString();
                yield return new WaitForSeconds(0.03f);
            }
        }
        yield return new WaitForSeconds(0.03f);
    }

    public void ShowScene(string SceneName)
    {
        AudioScripts.Click();
        LoadScene(SceneName);
    }

    public static void LoadScene(string SceneName)
    {
        PopUpDarkScripts.layers = 0;
        SceneManager.LoadScene(SceneName);
        System.GC.Collect();
    }

    public static int[] NewPos(int i, int c = 3, int yPlus = 734, int spaceWidth = 1400, int sx = 0, int sy = -160)
    {
        int curX, curY;
        curX = sx - spaceWidth / 2 + (i % c) * (spaceWidth / (c - 1));
        curY = sy - i / c * yPlus;
        return new int[] { curX, curY };
    }

    public void UserClicked()
    {
        AudioScripts.Click();
        var user = GameObject.Find("User");
        if (user == null)
        {
            StartPopUp("NotLoggedInWarningPopUp", true, new Vector2(-15, -53));
        }
        else if (user && user.GetComponent<UserScripts>() && !string.IsNullOrEmpty(user.GetComponent<UserScripts>().user.name))
        {
            StartPopUp("AuthProfilePopUp");
        }
        else
        {
            StartPopUp("GuestProfilePopUp");
        }
    }

    public static GameObject StartPopUp(string name, bool dark = true, Vector2 pos = default(Vector2), int lerp = 1)
    {
        Transform parent = GetParent();
        if (dark)
        {
            SetDark(parent);
        }
        GameObject PopUp = Instantiate((GameObject)Resources.Load("Prefab/" + name));
        PopUp.transform.parent = parent;
        PopUp.name = name;
        PopUp.transform.localScale = new Vector3(1, 1, 1);
        if (lerp != 0)
        {
            PopUp.transform.localPosition = new Vector2(pos.x, -Screen.height * lerp);
            if (cLerpPopUp != null)
            {
                cLerpPopUp.obj.localPosition = cLerpPopUp.targetPos;
            }
            cLerpPopUp = new LerpPiece(PopUp, pos);
        }
        else
        {
            PopUp.transform.localPosition = pos;
        }
        return PopUp;
    }

    public static void SetDark(Transform parent = null, bool force = false)
    {
        if (parent == null)
        {
            parent = GetParent();
        }
        if (!GameObject.Find("PopUpDark") || force)
        {
            GameObject PopUpDark = Instantiate((GameObject)Resources.Load("Prefab/PopUpDark"));
            PopUpDark.name = "PopUpDark";
            PopUpDark.transform.parent = parent;
            Vector2 cls = GameObject.Find("Canvas").transform.localScale;
            RectTransform drt = PopUpDark.GetComponent<RectTransform>();

            drt.offsetMin = new Vector2(0, 0);
            drt.offsetMax = new Vector2(0, 0);
            drt.sizeDelta = new Vector2(Screen.width / (cls.x - 0.1f), Screen.height / (cls.y - 0.1f));

            PopUpDark.transform.SetAsLastSibling();
        }
        PopUpDarkScripts.layers++;
    }

    public static Transform GetParent()
    {
        Transform parent = GameObject.Find("Canvas").transform;
        if (parent.Find("MainObject"))
        {
            parent = parent.Find("MainObject");
        }
        return parent;
    }

    public static void ClosePopUp(string v)
    {
        AudioScripts.Click();
        int i = 0;
        if (v != string.Empty)
        {
            PopUpDarkScripts.layers--;
            while (GameObject.Find(v) && i < 3)
            {
                Destroy(GameObject.Find(v));
                i++;
            }
        }
        GameObject.Find("UIManager").GetComponent<UIManagerScript>().StartCoroutine(CloseDark());
    }

    public static IEnumerator CloseDark()
    {
        int i = 0;
        while (PopUpDarkScripts.layers <= 0 && GameObject.Find("PopUpDark") && i < 3)
        {
            Destroy(GameObject.Find("PopUpDark"));
            i++;
            PopUpDarkScripts.layers = 0;
            yield return new WaitForEndOfFrame();
        }
    }

    public static GameObject CreateCategory(string name, int[] pos, Transform parent)
    {
        GameObject current = Instantiate((GameObject)Resources.Load("Prefab/" + name));
        current.transform.parent = parent;
        current.transform.localPosition = new Vector3(pos[0], pos[1], 0);
        current.transform.localScale = new Vector3(1, 1, 1);
        if (current.transform.Find("Holder") != null)
        {
            current.transform.Find("Holder").transform.Find("Loader").localScale = new Vector2(1, 1);
        }
        return current;
    }

    public static Vector3 CalculatePos(LerpPiece l, float sp = 100, bool shift = false)
    {
        Vector3 cTarget = shift ? l.targetPos + new Vector3(0, l.shift, 0) : l.targetPos;
        if (l.startTime == 0)
        {
            l.startTime = Time.time;
            l.dist = Vector3.Distance(l.startPos, cTarget);
            l.shift = sp * 0.1f;
        }
        if (shift)
        {
            l.shift = Mathf.MoveTowards(l.shift, 0, Time.deltaTime * sp * 0.6f);
        }
        float speed = ((Time.time - l.startTime) * sp) / l.dist;
        return Vector3.Lerp(l.obj.localPosition, cTarget, speed);
    }

    public static GameObject StartLoader()
    {
        if (GameObject.Find("Loader") == null)
        {
            GameObject l = Instantiate(Resources.Load("Prefab/LoaderPuzzleInfo")) as GameObject;
            l.name = "Loader";
            l.transform.parent = GameObject.Find("Canvas").transform;
            l.transform.localPosition = new Vector3(0, 0);
            l.transform.localScale = new Vector3(1, 1, 1);
            return l;
        }
        return null;
    }

    public static void StartAchievment(AchievmentsScripts.GameAchievment[] achs)
    {
        if (achs != null)
        {
            UIManagerScript u = GameObject.Find("UIManager").GetComponent<UIManagerScript>();
            foreach (AchievmentsScripts.GameAchievment a in achs)
            {
                u.StartCoroutine(StartAchievmentIEnumerator(a));
            }
        }
    }

    public static IEnumerator StartAchievmentIEnumerator(AchievmentsScripts.GameAchievment a)
    {
        WWW www = new WWW(configScripts.server + a.image);
        yield return www;
        if (www.error == null)
        {
            Transform cAch = UIManagerScript.StartPopUp("AchievmentPopUp", true).transform;
            cAch.Find("Glow").Find("Tex").GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
            cAch.Find("Text").GetComponent<Text>().text = a.acquired_text;
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("tutorial", JsonUtility.ToJson(tutorial));
    }
}