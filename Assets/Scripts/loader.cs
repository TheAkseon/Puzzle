using UnityEngine;
using UnityEngine.UI;

public class loader : MonoBehaviour
{
    public Sprite[] images;
    public bool internetProblemPopUp = true;
    private int current;
    private float time;
    private float maxDuration = 10;

    // Update is called once per frame
    private void Start()
    {
        if (transform.parent.name != "Holder")
        {
            transform.Find("Image").transform.GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
        }
        else
        {
            transform.Find("Image").transform.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            transform.Find("Image").transform.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        }
        current = 0;
        time = 0;
        InvokeRepeating("NextImage", 0.01f, 0.1f);
    }

    private void NextImage()
    {
        time += 0.1f;
        transform.GetComponent<Image>().sprite = images[current];
        if (current + 1 < images.Length)
            current++;
        else
            current = 0;
        if (internetProblemPopUp && time > maxDuration && GameObject.Find("InternetConnectionProblemsPopUp") == null)
        {
            UIManagerScript.StartPopUp("InternetConnectionProblemsPopUp");
            Destroy(gameObject);
        }
    }
}
