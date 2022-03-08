using UnityEngine;

public class DescriptionArrows : MonoBehaviour
{
    public GameObject[] arrows;
    public UnityEngine.UI.ScrollRect sr;
    public RectTransform descText;

    // Update is called once per frame
    private void Update()
    {
        arrows[0].SetActive(sr.verticalNormalizedPosition < 0.8f && descText.rect.height > Screen.height * 0.75);
        arrows[1].SetActive(sr.verticalNormalizedPosition > 0.2f && descText.rect.height > Screen.height * 0.75);
    }
}
