using UnityEngine;
using UnityEngine.UI;

public class HelpScript : MonoBehaviour
{
    public RawImage image;

    private void Start()
    {
        if (image != null)
        {
            image.texture = GameObject.Find("1").transform.Find("Image").gameObject.GetComponent<RawImage>().texture;
            image.SetNativeSize();
            PuzzleAreaScript.ResizeObj(image.GetComponent<RectTransform>(), 600);
        }
    }

    public void PopUp()
    {
        UIManagerScript.StartPopUp(name);
    }

    public void Exit()
    {
        UIManagerScript.ClosePopUp(name);
    }
}
