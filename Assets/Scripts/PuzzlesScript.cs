using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlesScript : MonoBehaviour
{
    [System.Serializable]
    public class Categories
    {
        public CategoryScript.Category[] categories;

        public Categories()
        {
            categories = null;
        }

        public Categories(CategoryScript.Category[] categories)
        {
            categories = new CategoryScript.Category[categories.Length];
            for (int i = 0; i < categories.Length; i++)
            {
                categories[i] = new CategoryScript.Category(categories[i].image, categories[i].puzzleCount, categories[i].priceFull);
            }
        }
    }

    public static CategoryScript.Category FindCategory(int id)
    {
        for (int i = 0; i < clist.categories.Length; i++)
        {
            if (clist.categories[i].id == id)
                return clist.categories[i];
        }
        return null;
    }

    public class Puzzles
    {
        //public PuzzleScript.Puzzle[] puzzles;
        public List<PuzzleScript.Puzzle> puzzles = new List<PuzzleScript.Puzzle>();

        public Puzzles()
        {
            puzzles = null;
        }

        public Puzzles(PuzzleScript.Puzzle[] list)
        {
            this.puzzles = new List<PuzzleScript.Puzzle>();
            for (int i = 0; i < puzzles.Count; i++)
            {
                this.puzzles.Add(new PuzzleScript.Puzzle(list[i].created_at, list[i].updated_at, list[i].image, list[i].description, list[i].is_unlocked, list[i].price, list[i].category_price, list[i].percentage_completed, list[i].is_active, list[i].is_started));
            }
        }
    }

    public static Categories clist;

    public void StartShowing(string json, int state)
    {
        clist = JsonUtility.FromJson<Categories>(json);
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2048, 160 + clist.categories.Length / 3 * 734 + 734);
        for (int i = 0; i < clist.categories.Length; i++)
        {
            CreateCategory(i, clist.categories[i]);
        }
        Destroy(GameObject.Find("Loader"));
    }

    public void TryAgain()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Start();
    }

    private void Start()
    {
        InternetConnectionProblemScripts.setMethod(TryAgain);
        APIMethodsScript.sendRequest("get", "/api/category", StartShowing);
    }

    private void CreateCategory(int i, CategoryScript.Category v)
    {
        GameObject current = UIManagerScript.CreateCategory("CategoryPrefab", UIManagerScript.NewPos(i), transform);
        current.transform.Find("Number").GetComponent<Text>().text = v.puzzleCount.ToString();
        current.GetComponent<CategoryScript>().priceFull = v.priceFull;
        current.transform.name = v.id.ToString();
        ImagesScript.setTextureFromURL(v.image, current.transform.Find("Holder").gameObject, current.transform.Find("PuzzlePreview").GetComponent<RawImage>(), v.id.ToString(), v.updated_at, v.id.ToString(), "mini");
        if (i == 0)
        {
            Tutorial t = GetComponent<Tutorial>();
            t.tList[3].selectBtn[0] = current;
            StartCoroutine(StartTutorial(current.transform.Find("Holder").gameObject, t));
        }
    }

    public static IEnumerator StartTutorial(GameObject holder, Tutorial t)
    {
        while (holder != null)
        {
            yield return new WaitForEndOfFrame();
        }
        if (UIManagerScript.GetActiveScene() == "puzzles")
        {
            while (GameObject.Find("Coins") == null)
            {
                yield return new WaitForEndOfFrame();
            }
            t.tList[2].selectBtn[0] = GameObject.Find("Coins");
            t.tList[2].selectBtn[1] = GameObject.Find("PhotoPuzzles");
        }
        t.enabled = true;
    }
}
