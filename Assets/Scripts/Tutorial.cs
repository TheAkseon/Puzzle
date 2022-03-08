using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public delegate void function1param(GameObject parameter);

    public enum TutorDirection { Center = 0, Left = 1, Right = 2 };

    [System.Serializable]
    public class ArrowList
    {
        public int num;
        public Vector2 pos;
        public Vector2 scale = new Vector2(1, 1);
        public string text;
        public TutorDirection dir;
    }

    [System.Serializable]
    public class TutorialList
    {
        public bool button = true;
        public GameObject[] selectBtn;
        public function1param objectFunction;
        public string[] findButton;
        public ArrowList[] arrowList;
    }

    public int num = 0;
    public float wait = 0;
    public TutorialList[] tList;
    public Vector2 customNextPos;

    private GameObject[] cloneBtn;
    private int tNum = 0;
    private bool dark = false;
    private GameObject[] arrow;
    private GameObject next;

    // Use this for initialization
    private IEnumerator Start()
    {
        if (wait != 0)
        {
            yield return new WaitForSeconds(wait);
        }
        else
        {
            yield return new WaitForEndOfFrame();
        }
        if (!UIManagerScript.tutorial.tStats[num])
        {
            StartTutorial();
        }
        else
        {
            enabled = false;
        }
    }

    private void StartTutorial()
    {
        cloneBtn = new GameObject[tList[tNum].selectBtn.Length];
        for (int i = 0; i < tList[tNum].selectBtn.Length; i++)
        {
            if (tList[tNum].selectBtn[i].activeInHierarchy)
            {
                if (!dark)
                {
                    UIManagerScript.SetDark(null, true);
                    dark = true;
                }
                cloneBtn[i] = Instantiate(tList[tNum].selectBtn[i]);
                if (tList[tNum].objectFunction != null)
                {
                    tList[tNum].objectFunction(cloneBtn[i]);
                }
                cloneBtn[i].name = tList[tNum].selectBtn[i].name;
                cloneBtn[i].transform.parent = tList[tNum].selectBtn[i].transform.parent;
                cloneBtn[i].transform.position = tList[tNum].selectBtn[i].transform.position;
                cloneBtn[i].transform.rotation = tList[tNum].selectBtn[i].transform.rotation;
                cloneBtn[i].transform.localScale = tList[tNum].selectBtn[i].transform.localScale;
                cloneBtn[i].transform.parent = UIManagerScript.GetParent();
                var crt = cloneBtn[i].transform as RectTransform;
                var prt = tList[tNum].selectBtn[i].transform as RectTransform;
                if (crt != null && prt != null)
                {
                    crt.sizeDelta = prt.sizeDelta;
                }
                Button b;
                if (tList[tNum].findButton != null && i < tList[tNum].findButton.Length && !string.IsNullOrEmpty(tList[tNum].findButton[i]))
                {
                    b = cloneBtn[i].transform.Find(tList[tNum].findButton[i]).GetComponent<Button>();
                }
                else
                {
                    b = cloneBtn[i].GetComponent<Button>();
                }
                if (b != null)
                {
                    if (tList[tNum].button)
                    {
                        b.onClick.AddListener(Click);
                    }
                    else
                    {
                        b.enabled = false;
                    }
                }
            }
        }

        arrow = new GameObject[tList[tNum].arrowList.Length];

        for (int a = 0; a < arrow.Length; a++)
        {
            if (tList[tNum].arrowList[a].num != 0)
            {
                GameObject arrowPrefab = (GameObject)Resources.Load("Prefab/Tutorial/" + tList[tNum].arrowList[a].num);
                arrow[a] = Instantiate(arrowPrefab);
                RectTransform rt = arrow[a].GetComponent<RectTransform>();
                arrow[a].transform.parent = UIManagerScript.GetParent();
                rt.anchoredPosition = tList[tNum].arrowList[a].pos == Vector2.zero ? arrowPrefab.GetComponent<RectTransform>().anchoredPosition : tList[tNum].arrowList[a].pos;
                Transform textObj = arrow[a].transform.Find("Text");
                rt.localScale = tList[tNum].arrowList[a].scale;
                textObj.localScale = tList[tNum].arrowList[a].scale;
                Text t = textObj.GetComponent<Text>();
                if (tList[tNum].arrowList[a].dir != TutorDirection.Center)
                {
                    Vector2 cls = GameObject.Find("Canvas").transform.localScale;
                    float dirX = (((Screen.width / cls.x) - ((Screen.height / cls.y) / 3 * 4)) / 2) * ((tList[tNum].arrowList[a].dir == TutorDirection.Left) ? -1 : 1);
                    arrow[a].GetComponent<RectTransform>().anchoredPosition += new Vector2(dirX, 0);
                }
                if (tList[tNum].arrowList[a].scale.y < 0)
                {
                    textObj.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 50);
                    t.alignment = TextAnchor.UpperCenter;
                }
                if (!string.IsNullOrEmpty(tList[tNum].arrowList[a].text))
                {
                    var localizedText = LocalizationManager.Instance.GetLocalizedValue(tList[tNum].arrowList[a].text);
                    t.text = localizedText;
                }
            }
        }

        if (tNum <= tList.Length - 1 && !tList[tNum].button)
        {
            GameObject nextPrefab = (GameObject)Resources.Load("Prefab/Tutorial/Next");
            next = Instantiate(nextPrefab);
            RectTransform rt = next.GetComponent<RectTransform>();
            next.transform.parent = UIManagerScript.GetParent();
            next.transform.localScale = nextPrefab.transform.localScale;
            rt.anchoredPosition = customNextPos == Vector2.zero ? nextPrefab.GetComponent<RectTransform>().anchoredPosition : customNextPos;
            next.GetComponent<Button>().onClick.AddListener(Next);
        }
    }

    public void Next()
    {
        ResetTutorial();
        if (tNum < tList.Length - 1)
        {
            tNum++;
            StartTutorial();
        }
        else
        {
            Exit();
        }
    }


    private void ResetTutorial()
    {
        foreach (GameObject c in cloneBtn)
        {
            Destroy(c);
        }
        foreach (GameObject a in arrow)
        {
            Destroy(a);
        }
        Destroy(next);
    }

    public void Click()
    {
        ResetTutorial();
        Exit();
    }

    public void Exit()
    {
        if (dark)
        {
            PopUpDarkScripts.layers--;
            UIManagerScript.ClosePopUp(string.Empty);
        }
        UIManagerScript.tutorial.tStats[num] = true;
        enabled = false;
    }
}
