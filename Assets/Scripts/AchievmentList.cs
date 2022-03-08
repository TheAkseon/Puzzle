using UnityEngine;
using UnityEngine.UI;

public class AchievmentList : MonoBehaviour
{
    public AchievmentsScripts.GameAchievment a;
    public RawImage image;
    public GameObject lockObj;

    // Use this for initialization
    public void StartPreview(AchievmentsScripts.GameAchievment v)
    {
        a = v;
        string dirName = "ach";
        name = a.id.ToString();
        lockObj.SetActive(!a.isUnlocked);
        ImagesScript.setTextureFromURL(a.image, transform.Find("Holder").gameObject, transform.Find("AchievmentPreview").GetComponent<RawImage>(), a.id.ToString(), a.updated_at, dirName, "a");
    }

    public void OnAchievmentClick()
    {
        Transform current = UIManagerScript.StartPopUp("AchievmentGamePopUp").transform;
        current.Find("AchievmentImage").GetComponent<RawImage>().texture = image.texture;
        current.Find("lock").GetComponent<RawImage>().enabled = !a.isUnlocked;
        current.Find("info-box").Find("Title").Find("Title").GetComponent<Text>().text = a.name;
        current.Find("info-box").Find("Info").GetComponent<Text>().text = a.isUnlocked ? a.acquired_text : a.description;
    }

    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp(name);
    }
}
