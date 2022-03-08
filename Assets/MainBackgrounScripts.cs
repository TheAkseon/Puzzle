using UnityEngine;
using UnityEngine.UI;

public class MainBackgrounScripts : MonoBehaviour
{
    // Use this for initialization
    private void Start()
    {
        if (GameObject.Find("User") != null)
        {
            Texture background = (Texture)Resources.Load("Images/Backgrounds/" + GameObject.Find("User").GetComponent<UserScripts>().user.background_id);
            transform.GetComponent<RawImage>().texture = background;
        }
        else
        {
            Texture background = (Texture)Resources.Load("Images/Backgrounds/0");
            transform.GetComponent<RawImage>().texture = background;
        }
    }
}
