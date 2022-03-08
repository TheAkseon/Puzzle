using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PiecesComparer : IComparer<GameObject>
{
    public int Compare(GameObject x, GameObject y)
    {
        return x != null && y != null && x.transform.localPosition.y > y.transform.localPosition.y ? -1 : 1;
    }
}

public class PiecesVectorComparer : IComparer<Vector3>
{
    public int Compare(Vector3 x, Vector3 y)
    {
        return x.y > y.y ? -1 : 1;
    }
}

public class PuzzleAreaScript : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject helpButton;
    public static int n;
    public static int m;
    public static int offsetHeight;
    public static float dHeight;
    public static int offsetWidth;
    public delegate Vector2 function1param(int parameter);
    public static bool newGame;
    public Tutorial[] tutorials;
    public static PuzzlePiece[] puzzles;
    public static float[] koef;
    public static List<LerpPiece> lp = new List<LerpPiece>();
    public static List<GameObject> pieces = new List<GameObject>();
    public static List<Vector3> piecesPos = new List<Vector3>();
    public static Transform pc;
    public static float sidePuzzle;

    public void click()
    {
        GameObject.Find("PuzzleArea").GetComponent<PlayingScripts>().PuzzlePieceClicked();
    }

    private void Start()
    {
        pSizeY = 0;
        pc = GameObject.Find("PiecesCollection").transform;
        if (newGame)
        {
            puzzles = CreatePuzzle(DifficultyScript.complexity);
        }
        PlayingScripts.TotalPieces = n * m;
        PlayingScripts.PiecesLeft = PlayingScripts.TotalPieces;

        Texture plainPuzzle = (Texture)Resources.Load("Images/Puzzles/0022");
        float new_piece_height = (transform.parent.GetComponent<RectTransform>().sizeDelta.y - transform.GetComponent<RectTransform>().offsetMin.y - transform.GetComponent<RectTransform>().offsetMax.y) / n;
        float new_piece_width = (transform.parent.GetComponent<RectTransform>().sizeDelta.x - transform.GetComponent<RectTransform>().offsetMin.x - transform.GetComponent<RectTransform>().offsetMax.x) / m;


        koef = new float[2];
        koef[0] = new_piece_width / (plainPuzzle.width);
        koef[1] = new_piece_height / (plainPuzzle.height);

        float kLeft = koef[0] * 46;
        float kBottom = koef[1] * 44;
        Texture texture;
        string pathFile = string.Empty;
        if (BeginPlaying.Puzzles != null && BeginPlaying.Puzzles.puzzles.Count > BeginPlaying.PuzzleIndex)
        {
            pathFile = Application.persistentDataPath + "/cacheImage/" + BeginPlaying.Puzzles.puzzles[BeginPlaying.PuzzleIndex].category_id.ToString() + "/big" + BeginPlaying.Puzzles.puzzles[BeginPlaying.PuzzleIndex].id + ".jpg";
        }
        else
        {
            //if (!Application.isEditor)
            //{
            //   pathFile = Application.persistentDataPath + "/photoPuzzles/";
            //}
            pathFile += BeginPlaying.photoPuzzleName;
        }
        texture = ImagesScript.LoadPNG(pathFile);
        if (texture != null)
        {
            float hor_koef = (transform.parent.GetComponent<RectTransform>().sizeDelta.x - transform.GetComponent<RectTransform>().offsetMin.x - transform.GetComponent<RectTransform>().offsetMax.x) / texture.width;
            float ver_koef = (transform.parent.GetComponent<RectTransform>().sizeDelta.y - transform.GetComponent<RectTransform>().offsetMin.y - transform.GetComponent<RectTransform>().offsetMax.y) / texture.height;
            float alt_koef = hor_koef < ver_koef ? hor_koef : ver_koef;
            int t = 1;
            pieces = new List<GameObject>();
            while (t <= n * m)
            {
                //game object pool
                int i = t;
                int ipz = i / m + 1;
                int jpz = i % m;
                if (jpz == 0)
                {
                    jpz = PuzzleAreaScript.m;
                    ipz--;
                }
                if (GameObject.Find("PuzzleArea").transform.Find(i.ToString()) == null)
                {
                    t++;
                    GameObject Piece = PoolManager.GetObject("PuzzlePiecePrefab", new Vector3(), new Quaternion());

                    Piece.name = i.ToString();

                    Piece.transform.parent = transform;

                    Piece.transform.localScale = new Vector3(1, 1, 1);
                    GameObject.Find("PuzzleArea").transform.Find(i.ToString()).GetComponent<PuzzlePieceScripts>().stickedPuzzles = new List<int>();


                    string s = "";

                    for (int r = 0; r < puzzles[(ipz - 1) * m + jpz].name.Length; r++)
                    {
                        s = s + puzzles[(ipz - 1) * m + jpz].name[r];
                    }
                    Piece.GetComponent<RawImage>().texture = (Texture)Resources.Load("Images/Puzzles/" + s);

                    GameObject img = Piece.transform.Find("Image").gameObject;

                    img.GetComponent<RawImage>().texture = texture;


                    Piece.GetComponent<RectTransform>().sizeDelta = new Vector2(new_piece_width, new_piece_height);
                    img.GetComponent<RawImage>().SetNativeSize();
                    img.GetComponent<RectTransform>().sizeDelta = new Vector2(alt_koef * texture.width, alt_koef * texture.height);
                    float curX, curY;
                    if (jpz != 1)
                    {
                        GameObject prevObj = transform.Find((i - 1).ToString()).gameObject;

                        curX = prevObj.transform.localPosition.x + prevObj.GetComponent<RectTransform>().sizeDelta.x / 2 + Piece.GetComponent<RectTransform>().sizeDelta.x / 2 - kLeft;
                    }
                    else
                    {
                        curX = -(new_piece_width - 2 * 22 * koef[0]) * m / 2 + Piece.GetComponent<RectTransform>().sizeDelta.x / 2 - koef[0] * 22;
                    }
                    if (i > m)
                    {
                        GameObject prevObj = transform.Find((i - m).ToString()).gameObject;
                        curY = prevObj.transform.localPosition.y - prevObj.GetComponent<RectTransform>().sizeDelta.y / 2 - Piece.GetComponent<RectTransform>().sizeDelta.y / 2 + kBottom;
                    }
                    else
                    {
                        curY = (new_piece_height - 2 * 22 * koef[1]) * n / 2 - Piece.GetComponent<RectTransform>().sizeDelta.y / 2 + koef[1] * 22;
                    }
                    Piece.transform.localPosition = new Vector3(curX, curY);
                    pieces.Add(Piece);
                    RectTransform rt = transform.parent.GetComponent<RectTransform>();
                    float d_x = (alt_koef * texture.width / 2 - (rt.sizeDelta.x - rt.offsetMin.x + rt.offsetMax.x) / 2);
                    img.transform.position = new Vector3(transform.GetComponent<RectTransform>().offsetMin.x - d_x, 0, 0);
                }
            }
        }
        GameObject mask = GameObject.Find("Mask");
        mask.GetComponent<RectTransform>().sizeDelta = new Vector2(m * (new_piece_width - 2 * 23 * koef[0]), n * (new_piece_height - 2 * 22 * koef[1]));
        mask.transform.localPosition = new Vector3(transform.Find("1").localPosition.x - new_piece_width / 2 + 23 * koef[0], transform.Find("1").localPosition.y + new_piece_height / 2 - 22 * koef[1]);

        GameObject background = Instantiate(transform.Find("1").transform.Find("Image").gameObject);
        background.name = "Hint";
        background.transform.position = transform.Find("1").transform.Find("Image").position;
        background.transform.parent = transform.Find("Mask").transform;
        background.transform.localScale = new Vector3(1, 1, 1);
        background.GetComponent<RawImage>().color = new Color(1, 1, 1, 0.5f);
        background.GetComponent<RawImage>().SetNativeSize();
        sidePuzzle = background.GetComponent<RectTransform>().anchoredPosition.x;
        if (sidePuzzle < 0)
        {
            sidePuzzle = 0;
        }
        ResizeObj(background.GetComponent<RectTransform>());
        background.SetActive(DifficultyScript.complexity == "Easy" || DifficultyScript.complexity == "Medium");

        float rect_width = GameObject.Find("Pieces").transform.Find("PiecesCollection").GetComponent<RectTransform>().sizeDelta.x;

        float big_width = GameObject.Find("PuzzleArea").transform.Find("1").GetComponent<RectTransform>().sizeDelta.x;
        float big_height = GameObject.Find("PuzzleArea").transform.Find("1").GetComponent<RectTransform>().sizeDelta.y;
        float small_height = big_height * GameObject.Find("Pieces").transform.Find("PiecesCollection").GetComponent<RectTransform>().sizeDelta.x * 0.8f / big_width;
        float small_width = GameObject.Find("Pieces").transform.Find("PiecesCollection").GetComponent<RectTransform>().sizeDelta.x * 0.8f;
        float height = small_width < big_width ? small_height : big_height;
        GameObject.Find("Pieces").transform.Find("PiecesCollection").GetComponent<RectTransform>().sizeDelta = new Vector2(rect_width, height * (n * m));
        StartCoroutine(Wait());
    }

    public static void ResizeObj(RectTransform t, float h = 1320)
    {
        float width = h / t.sizeDelta.y * t.sizeDelta.x;
        t.sizeDelta = new Vector2(width, h);
        if (sidePuzzle > 0)
        {
            t.anchoredPosition = new Vector2(t.anchoredPosition.x - sidePuzzle, t.anchoredPosition.y);
        }
    }

    private void Update()
    {
        Vector3 newPos = Vector3.zero;
        for (int i = 0; i < lp.Count; i++)
        {
            if (lp[i].obj.transform.parent == pc)
            {
                newPos = UIManagerScript.CalculatePos(lp[i]);
                if (!float.IsNaN(newPos.x))
                {
                    lp[i].obj.localPosition = newPos;
                }
            }
            if (lp[i].obj.localPosition == lp[i].targetPos || float.IsNaN(newPos.x) || lp[i].obj.transform.parent != pc)
            {
                lp.RemoveAt(i);
            }
        }
    }

    private IEnumerator Wait()
    {
        if (GameObject.Find("Pieces").transform.Find("PiecesCollection").childCount +
            GameObject.Find("PuzzleArea").transform.childCount +
            GameObject.Find("Installed").transform.childCount == m * n + 2)
        {
            yield return new WaitForEndOfFrame();
        }
        ShufflePieces(true);
    }

    static float pSizeY = 0;

    public static void ShiftPuzzle()
    {
        pieces.Sort(new PiecesComparer());
        int c = 0;
        float minY = 0;
        lp = new List<LerpPiece>();
        foreach (GameObject p in pieces)
        {
            if (p.transform.parent == pc)
            {
                if (p.transform.localPosition != piecesPos[c])
                {
                    lp.Add(new LerpPiece(p, piecesPos[c]));
                }
                if (minY > piecesPos[c].y)
                {
                    minY = piecesPos[c].y;
                }
                c++;
            }
        }
        RectTransform rt = pc.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, -minY + pSizeY);
    }

    private void ShufflePieces(bool start = false)
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            int r = Random.Range(0, pieces.Count);
            if (pieces[i].transform.parent == pc && pieces[r].transform.parent == pc)
            {
                Vector3 tmp = pieces[i].transform.position;
                pieces[i].transform.position = pieces[r].transform.position;
                pieces[r].transform.position = tmp;
            }
        }
        if (start)
        {
            int maxC = -1;
            for (int c = 0; c < pieces.Count; c++)
            {
                if (maxC == -1 || maxC != -1 && pieces[c].transform.position.y > pieces[maxC].transform.position.y)
                {
                    maxC = c;
                }
            }
            int cT = DifficultyScript.complexity == "Hard" ? 1 : 0;
            tutorials[cT].tList[cT == 1 ? 2 : 1].selectBtn[0] = pieces[maxC];
            tutorials[cT].tList[cT == 1 ? 2 : 1].objectFunction = TutorialPuzzlePiece;
            tutorials[cT].enabled = true;
        }
        piecesPos = new List<Vector3>();
        foreach (GameObject p in pieces)
        {
            if (p.transform.parent == pc)
            {
                if (pSizeY == 0)
                {
                    pSizeY = p.GetComponent<RectTransform>().sizeDelta.y;
                }
                piecesPos.Add(p.transform.localPosition - new Vector3(0, pSizeY * 0.5f, 0));
            }
        }
        piecesPos.Sort(new PiecesVectorComparer());
        Destroy(GameObject.Find("Loader"));
        helpButton.SetActive(DifficultyScript.complexity == "Hard");
    }

    public void TutorialPuzzlePiece(GameObject v)
    {
        v.GetComponent<PuzzlePieceScripts>().enabled = false;
    }

    public class PuzzlePiece
    {
        public char[] name;
        public float? x;
        public float? y;

        public PuzzlePiece()
        {
            name = new char[4];
            for (int i = 0; i < name.Length; i++)
                name[i] = '0';
        }

        public PuzzlePiece(char[] name_)
        {
            name = new char[4];
            for (int i = 0; i < name_.Length; i++)
                name[i] = name_[i];
        }
    }

    private PuzzlePiece[] CreatePuzzle(string complexity)
    {
        return complexity == "Easy" ? CreateEasyPuzzle() : CreateHardPuzzle();
    }

    private PuzzlePiece[] CreateHardPuzzle()
    {
        PuzzlePiece[] puzzles = new PuzzlePiece[(n + 1) * (m + 1)];
        for (int k = 0; k < puzzles.Length; k++)
        {
            puzzles[k] = new PuzzlePiece();
        }
        return puzzles;
    }

    private PuzzlePiece[] CreateEasyPuzzle()
    {
        PuzzlePiece[] puzzles = new PuzzlePiece[(n + 1) * (m + 1)];
        for (int k = 0; k < puzzles.Length; k++)
        {
            puzzles[k] = new PuzzlePiece();
        }
        int i = 1;

        while (i <= n)
        {
            int j = 1;
            while (j <= m)
            {
                int ind = (i - 1) * m + j;
                System.Random rnd = new System.Random();
                string k = rnd.Next(1, 3).ToString();
                if (ind > m)
                {
                    if (puzzles[ind - m].name[2] == '1')
                        puzzles[ind].name[0] = '2';
                    else
                        puzzles[ind].name[0] = '1';
                }
                else
                    puzzles[ind].name[0] = '0';
                if (j < m)
                    puzzles[ind].name[1] = k.ToCharArray()[0];
                else
                    puzzles[ind].name[1] = '0';
                //rnd = new System.Random();
                k = rnd.Next(1, 3).ToString();
                if (i < n)
                    puzzles[ind].name[2] = k.ToCharArray()[0];
                else
                    puzzles[ind].name[2] = '0';
                if (j > 1)
                {
                    if (puzzles[ind - 1].name[1] == '1')
                        puzzles[ind].name[3] = '2';
                    else
                        puzzles[ind].name[3] = '1';
                }
                else
                    puzzles[ind].name[3] = '0';

                j++;
            }

            i++;
        }
        return puzzles;
    }
}
