using UnityEngine;

public class AchievmentsListScripts : MonoBehaviour
{
    public class GameAchievments
    {
        public AchievmentsScripts.GameAchievment[] achievments;
    }

    // Use this for initialization
    private void Start()
    {
        Load();
    }

    public void Load(string v = "4")
    {
        shift = 0;
        ResetAch();
        InternetConnectionProblemScripts.setMethod(TryAgain);
        underlines[0].SetActive(v == "4");
        underlines[1].SetActive(v == "1,2,3");
        APIMethodsScript.sendRequest("get", "/api/user/achievment?type_id=" + v, StartShowing);
    }

    public void TryAgain()
    {
        Load();
    }

    public void ResetAch()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public static GameAchievments clist;
    private int shift = 0;
    public GameObject[] arrows;
    public GameObject[] underlines;

    public void StartShowing(string json, int state)
    {
        clist = JsonUtility.FromJson<GameAchievments>(json);
        if (clist.achievments != null)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2048, 160 + clist.achievments.Length / 3 * 734 + 734);
            Showing();
        }
        Destroy(GameObject.Find("Loader"));
    }

    private void Showing()
    {
        arrows[0].SetActive(shift > 0);
        arrows[1].SetActive(shift + 8 < clist.achievments.Length);
        for (int i = shift; i < clist.achievments.Length && i < 8 + shift; i++)
        {
            CreateAchievment(i - shift, clist.achievments[i]);
        }
    }

    public void NextAchievmens(int v)
    {
        shift += 8 * v;
        ResetAch();
        Showing();
    }

    private void CreateAchievment(int i, AchievmentsScripts.GameAchievment v)
    {
        GameObject current = UIManagerScript.CreateCategory("AchievmentPrefab", UIManagerScript.NewPos(i, 4, 500), transform);
        current.GetComponent<AchievmentList>().StartPreview(v);
        if (i == 0)
        {
            GetComponent<Tutorial>().tList[0].selectBtn[0] = current;
            StartCoroutine(PuzzlesScript.StartTutorial(current.transform.Find("Holder").gameObject, GetComponent<Tutorial>()));
        }
    }
}
