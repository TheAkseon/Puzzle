using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CategoryPopUp1 : MonoBehaviour
{
    public static int? prevImageIndex;
    public static Pop_up.PopUp PopUpClassObject = new Pop_up.PopUp();
    public static PuzzlesScript.Puzzles PuzzlesList;

    public void OnCategoryClick()
    {
        if (!GameObject.Find("PuzzleInfo"))
        {
            string id = "";
            BeginPlaying.idPuzzle = 0;
            PushPopUp p = null;
            if (EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<CategoryScript>() != null)
            {
                BeginPlaying.categoryPrice = EventSystem.current.currentSelectedGameObject.transform.parent.GetComponent<CategoryScript>().priceFull;
                id = EventSystem.current.currentSelectedGameObject.transform.parent.name;
                PopUpClassObject.ImageIndex = 0;
            }
            else
            {
                p = GameObject.FindObjectOfType<PushPopUp>();
                id = p.category.ToString();
            }
            AudioScripts.Click();
            UIManagerScript.LoadScene("puzzleInfo");
            if (p != null && p.puzzle != 0)
            {
                Start();
            }
            else
            {
                PopUpClassObject.ShowCategoryPopUp(id);
            }
        }
    }

    private void Start()
    {
        if (GameObject.Find("puzzleInformation") != null)
        {
            BeginPlaying.idCategory = GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.idCategory.ToString();
            PopUpClassObject.showPuzzleById(GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.indImage, GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.PuzzleList);
        }
    }

    public void OnExitButtonClick()
    {
        Destroy(gameObject);
        GameObject.Find("PuzzlesPanel").GetComponent<ScrollRect>().enabled = true;
        if (GameObject.Find("puzzleInformation") != null)
        {
            Destroy(GameObject.Find("puzzleInformation").gameObject);
        }
    }

    public void RightArrowClicked()
    {
        AudioScripts.Click();
        PopUpClassObject.NextPuzzle(1);
    }

    public void LeftArrowClicked()
    {
        AudioScripts.Click();
        PopUpClassObject.NextPuzzle(-1);
    }
}
