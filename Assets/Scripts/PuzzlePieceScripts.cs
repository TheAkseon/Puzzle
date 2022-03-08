using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzlePieceScripts : MonoBehaviour
{
    private float? x;
    private float? y;
    private int ipz;
    private int jpz;

    public List<int> stickedPuzzles;
    public static float big_width, big_height, small_width, small_height;

    private void Start()
    {
        GameObject image = transform.Find("Image").gameObject;
        image.GetComponent<RawImage>().SetNativeSize();
        PuzzleAreaScript.ResizeObj(image.GetComponent<RectTransform>());
        ipz = int.Parse(transform.name) / PuzzleAreaScript.m + 1;
        jpz = int.Parse(transform.name) % PuzzleAreaScript.m;
        if (jpz == 0)
        {
            jpz = PuzzleAreaScript.m;
            ipz--;
        }
        x = transform.localPosition.x;
        y = transform.localPosition.y;
        if (PuzzleAreaScript.newGame || (!PuzzleAreaScript.newGame && PuzzleAreaScript.puzzles[int.Parse(name)].x == 0))
        {
            big_width = gameObject.GetComponent<RectTransform>().sizeDelta.x;
            big_height = gameObject.GetComponent<RectTransform>().sizeDelta.y;
            small_height = big_height * GameObject.Find("Pieces").transform.Find("PiecesCollection").GetComponent<RectTransform>().sizeDelta.x * 0.8f / big_width;
            small_width = GameObject.Find("Pieces").transform.Find("PiecesCollection").GetComponent<RectTransform>().sizeDelta.x * 0.8f;
            float width = small_width < big_width ? small_width : big_width;
            float height = small_width < big_width ? small_height : big_height;
            transform.parent = GameObject.Find("Pieces").transform.Find("PiecesCollection");
            gameObject.GetComponent<RectTransform>().localScale = new Vector2(width / gameObject.GetComponent<RectTransform>().sizeDelta.x, height / gameObject.GetComponent<RectTransform>().sizeDelta.y);
            transform.localPosition = new Vector3(0, -height * (transform.GetSiblingIndex() + 0.5f), 0);
        }
        else
        {
            transform.localPosition = new Vector3((float)PuzzleAreaScript.puzzles[int.Parse(transform.name)].x, (float)PuzzleAreaScript.puzzles[int.Parse(transform.name)].y);
            if (transform.GetComponent<Button>().enabled && transform.parent.name == "PuzzleArea" && module(transform.localPosition.x - x) < 0.01 && module(transform.localPosition.y - y) < 0.01)
            {
                StickPiece(GetComponent<Button>(), this);
            }
            if (transform.parent.name == "Installed")
                Stick();
        }
    }

    private bool checkStick(int i, int j)
    {
        float? dx = GameObject.Find(i.ToString()).transform.Find("Image").transform.position.x - GameObject.Find(j.ToString()).transform.Find("Image").transform.position.x;
        float? dy = GameObject.Find(i.ToString()).transform.Find("Image").transform.position.y - GameObject.Find(j.ToString()).transform.Find("Image").transform.position.y;
        dx = module(dx);
        dy = module(dy);

        if (dx < 0.01f && dy < 0.01)
        {
            return true;
        }
        return false;
    }

    public void Stick()
    {
        int i = int.Parse(transform.name);
        int j = PuzzleAreaScript.m;
        int[] checkInd = { -1, -j };
        float plainWidth = GameObject.Find("PuzzleArea").GetComponent<RectTransform>().sizeDelta.x / PuzzleAreaScript.m;
        float plainHeight = GameObject.Find("PuzzleArea").GetComponent<RectTransform>().sizeDelta.y / PuzzleAreaScript.n;
        Texture plainPuzzle = (Texture)Resources.Load("Images/Puzzles/0022");

        float koef = plainHeight / plainPuzzle.height;
        if (koef > plainWidth / plainPuzzle.width)
            koef = plainWidth / plainPuzzle.width;
        for (int ind = 0; ind < checkInd.Length; ind++)
        {
            if (!stickedPuzzles.Contains(i + checkInd[ind]) && GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()) != null && checkStick(i, i + checkInd[ind]))
            {
                if ((checkInd[ind] == 1 && i % PuzzleAreaScript.m != 0) || (checkInd[ind] == -1 && (i % PuzzleAreaScript.m != 1)))
                {
                    float curX = GameObject.Find("PuzzleArea").transform.Find((i).ToString()).transform.position.x + (GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()).Find("Image").transform.position.x - GameObject.Find("PuzzleArea").transform.Find((i).ToString()).transform.Find("Image").transform.position.x);
                    float curY = GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()).transform.position.y;
                    GameObject.Find("PuzzleArea").transform.Find((i).ToString()).transform.position = new Vector3(curX, curY, 0);
                    stickedPuzzles.Add(i + checkInd[ind]);
                    GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()).GetComponent<PuzzlePieceScripts>().stickedPuzzles.Add(i);
                }
                else if (checkInd[ind] == j || checkInd[ind] == -j)
                {
                    float curX = GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()).transform.position.x;

                    float curY = GameObject.Find("PuzzleArea").transform.Find((i).ToString()).transform.position.y + (GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()).Find("Image").transform.position.y - GameObject.Find("PuzzleArea").transform.Find((i).ToString()).transform.Find("Image").transform.position.y);
                    GameObject.Find("PuzzleArea").transform.Find((i).ToString()).transform.position = new Vector3(curX, curY, 0);
                    stickedPuzzles.Add(i + checkInd[ind]);
                    GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()).GetComponent<PuzzlePieceScripts>().stickedPuzzles.Add(i);
                }
            }
        }
    }

    public static float? module(float? x)
    {
        if (x < 0)
            return -x;
        return x;
    }

    public void CheckPosition(List<int> checkedPieces, bool enable = true)
    {
        float? dx, dy;
        int i = int.Parse(transform.name);
        int j = PuzzleAreaScript.m;
        int[] checkInd = { -1, -j, 1, j };
        float plainWidth = GameObject.Find("PuzzleArea").GetComponent<RectTransform>().sizeDelta.x / PuzzleAreaScript.m;
        float plainHeight = GameObject.Find("PuzzleArea").GetComponent<RectTransform>().sizeDelta.y / PuzzleAreaScript.n;
        Texture plainPuzzle = (Texture)Resources.Load("Images/Puzzles/0022");
        float koef = plainHeight / plainPuzzle.height;
        if (koef > plainWidth / plainPuzzle.width)
            koef = plainWidth / plainPuzzle.width;
        for (int ind = 0; ind < checkInd.Length; ind++)
        {
            if (!stickedPuzzles.Contains(i + checkInd[ind]) && GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString()) != null && checkStickPuzzles(i, i + checkInd[ind]))
            {
                Transform cPiece = GameObject.Find("PuzzleArea").transform.Find((i).ToString());
                Transform cPiecePlus = GameObject.Find("PuzzleArea").transform.Find((i + checkInd[ind]).ToString());
                List<int> moved = new List<int>();
                if ((checkInd[ind] == 1 && i % PuzzleAreaScript.m != 0) || (checkInd[ind] == -1 && (i % PuzzleAreaScript.m != 1)))
                {
                    float curX = cPiece.position.x + (cPiecePlus.Find("Image").position.x - cPiece.Find("Image").position.x);
                    float curY = cPiecePlus.position.y;
                    dx = cPiece.position.x - curX;
                    dy = cPiece.position.y - curY;
                    transform.parent.GetComponent<PlayingScripts>().moveSticked(transform, -(float)dx, -(float)dy, moved);
                    cPiece.position = new Vector3(curX, curY, 0);
                    stickedPuzzles.Add(i + checkInd[ind]);
                    cPiecePlus.GetComponent<PuzzlePieceScripts>().stickedPuzzles.Add(i);
                }
                else if (checkInd[ind] == j || checkInd[ind] == -j)
                {
                    float curX = cPiecePlus.position.x;
                    float curY = cPiece.position.y + (cPiecePlus.Find("Image").position.y - cPiece.Find("Image").position.y);
                    dx = curX - cPiece.position.x;
                    dy = curY - cPiece.position.y;
                    transform.parent.GetComponent<PlayingScripts>().moveSticked(transform, (float)dx, (float)dy, moved);
                    cPiece.position = new Vector3(curX, curY, 0);
                    stickedPuzzles.Add(i + checkInd[ind]);
                    cPiecePlus.GetComponent<PuzzlePieceScripts>().stickedPuzzles.Add(i);
                }
            }
        }
        if (stickedPuzzles.Count != 0)
        {
            CheckPositionSticked(transform, checkedPieces);
        }
        dx = transform.localPosition.x - x;
        dy = transform.localPosition.y - y;
        dx = module(dx);
        dy = module(dy);
        float kx, ky;
        if (DifficultyScript.complexity == "Hard")
        {
            kx = transform.GetComponent<RectTransform>().sizeDelta.x * 0.2f;
            ky = transform.GetComponent<RectTransform>().sizeDelta.y * 0.2f;
        }
        if (DifficultyScript.complexity == "Easy")
        {
            kx = transform.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
            ky = transform.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        }
        else
        {
            kx = transform.GetComponent<RectTransform>().sizeDelta.x * 0.3f;
            ky = transform.GetComponent<RectTransform>().sizeDelta.y * 0.3f;
        }

        if (transform.GetComponent<Button>().enabled && dx < kx && dy < ky && transform.parent.name == "PuzzleArea")
        {
            StickPiece(GetComponent<Button>(), this);
            GameObject.Find("MainAudio").GetComponent<AudioScripts>().ClickSound(2);
            transform.SetSiblingIndex(1);
            if (enable)
                enableSticked(transform, new List<int>());
        }
    }

    private void StickPiece(Button b, PuzzlePieceScripts p)
    {
        p.transform.localPosition = new Vector3((float)p.x, (float)p.y, 0);
        p.transform.parent = GameObject.Find("Installed").transform;
        b.enabled = false;
        PlayingScripts.PiecesLeft--;
        p.GetComponent<RawImage>().raycastTarget = false;
        p.transform.Find("Image").GetComponent<RawImage>().raycastTarget = false;
    }

    private void CheckPositionSticked(Transform current, List<int> checkedPieces)
    {
        if (!checkedPieces.Contains(int.Parse(current.name)))
        {
            checkedPieces.Add(int.Parse(current.name));
            foreach (int piece in current.GetComponent<PuzzlePieceScripts>().stickedPuzzles)
            {
                if (!checkedPieces.Contains(piece))
                {
                    GameObject.Find("PuzzleArea").transform.Find(piece.ToString()).GetComponent<PuzzlePieceScripts>().CheckPosition(checkedPieces, false);
                }
            }
        }
    }

    private void enableSticked(Transform current, List<int> enabled)
    {
        if (!enabled.Contains(int.Parse(current.name)))
        {
            enabled.Add(int.Parse(current.name));
            foreach (int piece in current.GetComponent<PuzzlePieceScripts>().stickedPuzzles)
            {
                Transform cPiece = GameObject.Find("PuzzleArea").transform.Find(piece.ToString());
                if (cPiece != null)
                {
                    if (!enabled.Contains(piece) && cPiece.GetComponent<Button>().enabled)
                    {
                        StickPiece(cPiece.GetComponent<Button>(), cPiece.GetComponent<PuzzlePieceScripts>());
                    }
                    enableSticked(cPiece, enabled);
                }
            }
        }
    }

    private bool checkStickPuzzles(int i, int j)
    {
        float? dx = GameObject.Find(i.ToString()).transform.Find("Image").transform.position.x - GameObject.Find(j.ToString()).transform.Find("Image").transform.position.x;
        float? dy = GameObject.Find(i.ToString()).transform.Find("Image").transform.position.y - GameObject.Find(j.ToString()).transform.Find("Image").transform.position.y;
        dx = module(dx);
        dy = module(dy);
        float kx, ky;
        if (DifficultyScript.complexity == "Hard")
        {
            kx = transform.GetComponent<RectTransform>().sizeDelta.x * 0.2f;
            ky = transform.GetComponent<RectTransform>().sizeDelta.y * 0.2f;
        }
        if (DifficultyScript.complexity == "Easy")
        {
            kx = transform.GetComponent<RectTransform>().sizeDelta.x * 0.5f;
            ky = transform.GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        }
        else
        {
            kx = transform.GetComponent<RectTransform>().sizeDelta.x * 0.3f;
            ky = transform.GetComponent<RectTransform>().sizeDelta.y * 0.3f;
        }
        if (dx < kx && dy < ky)
        {
            return true;
        }
        return false;
    }
}
