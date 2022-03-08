//using Ads;
using Entities;
using UnityEngine;

public class ResultsScript : MonoBehaviour
{
    public void Close()
    {
        PopUpDarkScripts.layers = 0;
        UIManagerScript.ClosePopUp(name);
    }

    public void ToMenu()
    {
        AudioScripts.Click();
        if (BeginPlaying.idCategory != "" && BeginPlaying.photoPuzzleName == "")
        {
            UIManagerScript.LoadScene("puzzles");
        }
        else if (BeginPlaying.idCategory == "" && BeginPlaying.photoPuzzleName != "")
        {
            UIManagerScript.LoadScene("mypuzzles");

        }
        Destroy(GameObject.Find("puzzleInformation").gameObject);
    }

    public void PlayAgain()
    {
        AudioScripts.Click();
        UIManagerScript.LoadScene("puzzleInfo");
    }

    public void PlayNext()
    {
        AudioScripts.Click();
        if (IsAdsOn())
        {

          //  GoogleMobileAdsScript.Instance.ShowRewardBasedVideo(PlayNextAfterAds, true);
            
         //   GoogleMobileAdsScript.Show_Rewarded_Video(true);
            PlayNextAfterAds();
        }
        else
        {
            PlayNextAfterAds();
        }
    }

    public void PlayNextAfterAds()
    {
        GameObject.Find("puzzleInformation").GetComponent<puzzleInformationScripts>().puzzleInfo.indImage++;
        UIManagerScript.LoadScene("puzzleInfo");
    }

    private bool IsAdsOn()
    {
        // If ads string empty so we didn't off ads.
        var adsString = PlayerPrefs.GetString(DifficultyScript.IS_ADS_OFF_KEY, string.Empty);
        return string.IsNullOrEmpty(adsString);
    }
}
