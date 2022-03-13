using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MyPhotoPuzzlesScripts : MonoBehaviour
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
        for (int i = 0; i < CategoriesList.categories.Length; i++)
        {
            if (CategoriesList.categories[i].id == id)
                return CategoriesList.categories[i];
        }
        return null;
    }

    public class Puzzles
    {
        public PuzzleScript.Puzzle[] puzzles;

        public Puzzles()
        {
            puzzles = null;
        }

        public Puzzles(PuzzleScript.Puzzle[] list)
        {
            this.puzzles = new PuzzleScript.Puzzle[list.Length];
            for (int i = 0; i < puzzles.Length; i++)
            {
                this.puzzles[i] = new PuzzleScript.Puzzle(list[i].created_at, list[i].updated_at, list[i].image, list[i].description, list[i].is_unlocked, list[i].price, list[i].category_price, list[i].percentage_completed, list[i].is_active, list[i].is_started);
            }
        }
    }

    public static Categories CategoriesList;

    public void StartShowing()
    {
        var info = new DirectoryInfo(Application.persistentDataPath + "/photoPuzzles");
        var fileInfo = info.GetFiles();
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2048, 160 + fileInfo.Length / 3 * 734 + 734);
        if (GameObject.Find("Loader") != null)
            Destroy(GameObject.Find("Loader").gameObject);
        int i = 0;
        foreach (FileInfo file in fileInfo)
        {
            if (file.ToString().EndsWith(".jpg"))
            {
                Debug.Log(file.Name);
                CreateCategory(i, file.ToString());
                i++;
            }
        }
        int[] pos = UIManagerScript.NewPos(i);
        Debug.Log($"PosX: {pos[0]}. PosY: {pos[1]}");
        GameObject.Find("PuzzlesPanel").transform.Find("Puzzles").transform.Find("AddNewPuzzle").transform.parent = transform;
        GameObject.Find("PuzzlesPanel").transform.Find("Puzzles").transform.Find("AddNewPuzzle").SetAsLastSibling();
        GameObject.Find("PuzzlesPanel").transform.Find("Puzzles").transform.Find("AddNewPuzzle").transform.localPosition = new Vector3(pos[0], pos[1] - 210, 0);
    }

    private void Start()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/photoPuzzles"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/photoPuzzles");
        }
        if (Directory.Exists(Application.persistentDataPath + "/photoPuzzles"))
        {
            StartShowing();
        }
    }

    public void CreateButton()
    {
        AudioScripts.Click();
        if (GameObject.Find("User").GetComponent<UserScripts>().user.user_currencies[1].count > 0)
        {
            UIManagerScript.LoadScene("createMyPuzzle");
        }
        else
        {
          //  CoinsScript.getCoinsPriceListPopUp();
        }
    }

    private void CreateCategory(int i, string path)
    {
        GameObject current = UIManagerScript.CreateCategory("PhotoPuzzlePrefab", UIManagerScript.NewPos(i), transform);
        current.transform.name = path;
      //  if (!Application.isEditor)
       // {
       //     path = Application.persistentDataPath + "/photoPuzzles/" + path;
      //  }
        ImagesScript.load(path, current.transform.Find("PuzzlePreview").GetComponent<RawImage>(), current.transform.Find("Holder").gameObject);
    }
}
