using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoogleMobileAdsScript : MonoBehaviour
{
 /*   public static bool needReward = false;
    void Awake() 
    {
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Yodo1U3dAds.InitializeSdk();
       // load_Interstitial();
        load_Rewarded_Video();
        
    }

    void Update()
    {

        
    }
    private static void load_Rewarded_Video() 
    {
       // Yodo1U3dSDK.setRewardVideoDelegate((Yodo1U3dConstants.AdEvent adEvent, string error) =>
    //    {
          //  Debug.Log("RewardVideoDelegate:" + adEvent + "\n" + error);
         //   switch (adEvent)
            {
              //  case Yodo1U3dConstants.AdEvent.AdEventClick:
                    Debug.Log("Rewarded video ad has been clicked.");
          //          break;
            //    case Yodo1U3dConstants.AdEvent.AdEventClose:
                    load_Rewarded_Video();
                    Debug.Log("Rewarded video ad has been closed.");
            //        break;
             //   case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
            //        Yodo1U3dAds.ShowVideo();
                    Debug.Log("Rewarded video ad has shown successful.");
             //       break;
             //   case Yodo1U3dConstants.AdEvent.AdEventShowFail:
                    load_Rewarded_Video();
                    Debug.Log("Rewarded video ad show failed, the error message:" + error);
              //      break;
             //   case Yodo1U3dConstants.AdEvent.AdEventFinish:
                if (needReward)
                {
                  load_Rewarded_Video();

                  needReward = false;
                }
                    Debug.Log("Rewarded video ad has been played finish, give rewards to the player.");
              //      break;
            }
        });
    }

   /*  private  static void load_Interstitial() 
    {
        Yodo1U3dSDK.setInterstitialAdDelegate((Yodo1U3dConstants.AdEvent adEvent, string error) =>
        {
            Debug.Log("InterstitialAdDelegate:" + adEvent + "\n" + error);
            switch (adEvent)
            {
                case Yodo1U3dConstants.AdEvent.AdEventClick:
                    load_Interstitial();
                    Debug.Log("Interstital ad has been clicked.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventClose:
                    load_Interstitial();
                    Debug.Log("Interstital ad has been closed.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowSuccess:
                    Debug.Log("Interstital ad has been shown successful.");
                    break;
                case Yodo1U3dConstants.AdEvent.AdEventShowFail:
                    Debug.Log("Interstital ad has been show failed, the error message:" + error);
                    break;
            }
        });
    }
     public static void Show_interstitial()
     {
         if (Yodo1U3dAds.InterstitialIsReady())
         {
             Yodo1U3dAds.ShowInterstitial();
             Debug.Log("Interstitial_Showing");
         }
         else 
         {
             load_Interstitial();
             Debug.Log("Interstitial Not Loaded"); 
         }
     }
     public static void Show_Rewarded_Video(bool _NeedReward)
     {
         if (Yodo1U3dAds.VideoIsReady())
         {
             Yodo1U3dAds.ShowVideo();
             needReward = _NeedReward;
             Debug.Log("Rewarded Video Shown");
         }
         else 
         {
             load_Rewarded_Video();
             Debug.Log("Rewarded Video Failed To load");
         }
     }
     
    */
}
