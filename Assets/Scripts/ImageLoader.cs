using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImageLoader : MonoBehaviour
{
    public IEnumerator LoadImage(string path, GameObject current)
    {
        path = System.Uri.EscapeUriString(path);
        WWW www = new WWW(path);
        yield return www;
        current.GetComponent<CategoryScript>().transform.Find("PuzzlePreview").GetComponent<RawImage>().texture = www.texture;
    }
}
