using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdMob : MonoBehaviour
{
    public static AdMob Instance { get; private set; }

    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;
    private BannerView _bannerView;

    private readonly List<Action> onFinishCallbacks = new List<Action>();

    public bool needReward = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        InitializeAdsCore();
        InitializeRewardedAd();
        InitializeInterstitial();
        InitializeBanner();
    }

    private void InitializeAdsCore()
    {
#if UNITY_ANDROID
        string appId = "ca-app-pub-3940256099942544~3347511713";
#elif UNITY_IPHONE
 string appId = "ca-app-pub-3940256099942544~1458002511";
#else
 string appId = "unexpected_platform";
#endif
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });
    }

    private void InitializeRewardedAd()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
 string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
 string adUnitId = "unexpected_platform";
#endif
        // Get singleton reward based video ad reference.
        // this.rewardBasedVideo = RewardBasedVideoAd.Instance;
        // Called when an ad request has successfully loaded.
        _rewardedAd.OnAdLoaded += HandleRewardBasedVideoLoaded;
        // Called when an ad request failed to load.
        _rewardedAd.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
        // Called when an ad is shown.
        _rewardedAd.OnAdOpening += HandleRewardBasedVideoOpened;
        // Called when the ad starts to play.
        //rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
        // Called when the user should be rewarded for watching a video.
        // rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
        // Called when the ad is closed.
        _rewardedAd.OnAdClosed += HandleRewardBasedVideoClosed;
        // Called when the ad click caused the user to leave the application.
        // rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
        this.RequestReward();
    }

    private void RequestReward()
    {

    }

    private void InitializeInterstitial()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
 string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
 string adUnitId = "unexpected_platform";
#endif
        // Initialize an InterstitialAd.
        _interstitialAd = new InterstitialAd(adUnitId);
        // Called when an ad request has successfully loaded.
        _interstitialAd.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        _interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        _interstitialAd.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        _interstitialAd.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
        RequestInterstitial();
    }

    private void RequestInterstitial()
    {

    }

    private void InitializeBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
 string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
 string adUnitId = "unexpected_platform";
#endif
        // Initialize an InterstitialAd.
        //this.interstitial = new GoogleMobileAds.Api.InterstitialAd(adUnitId);
        // Called when an ad request has successfully loaded.
      //  this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
      //  this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
      //  this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
     //   this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        //this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
      //  RequestInterstitial();
    }

    private void RequestBanner()
    {

    }

    private void HandleRewardAdLoaded(object sender, EventArgs args)
    {

    }

    private void HandleRewardAdFailedLoad(object sender, AdFailedToLoadEventArgs args)
    {

    }

    private void HandleRewardAdOpened(object sender, EventArgs args)
    {

    }

    private void HandleRewardAdStarted(object sender, EventArgs args)
    {

    }

    private void HandleRewardAdClosed(object sender, EventArgs args)
    {

    }

    private void HandleRewardAdRewarded(object sender, Reward args)
    {

    }

    private void HandleRewardAdLeftApplication(object sender, EventArgs args)
    {

    }

    private void HandleOnLoaded(object sender, EventArgs args)
    {

    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {

    }

    private void HandleOnAdOpened(object sender, EventArgs args)
    {

    }

    private void HandleOnAdClosed(object sender, EventArgs args)
    {

    }

    private void HandleOnAdLeavingApplication(object sender, EventArgs args)
    {

    }

    private IEnumerator RewardAdReloader()
    {
        yield return new WaitForSeconds(3);
        RequestReward();
    }

    private IEnumerator InterstitialReloader()
    {
        yield return new WaitForSeconds(3);
        RequestInterstitial();
    }

    private IEnumerator BannerReloader()
    {
        yield return new WaitForSeconds(3);
        RequestBanner();
    }

    public void ShowRewardAd(Action onFinishCallback, bool needReward = false)
    {

    }

    public void ShowInterstitialAd(Action onFinishCallback)
    {

    }

    public void GetReward(string json, int status)
    {
        if (status == 200)
        {
            Debug.Log(json);
            var reward = JsonUtility.FromJson<RewardResponse>(json);
            if (reward == null)
            {
                Debug.Log("Serializing error. For the RewardResponse got NULL!");
                return;
            }
            if (reward.currency == null)
            {
                Debug.Log("Seriaalizing error. For the currency got NULL!");
                return;
            }
            var user = GameObject.Find("User");
            if (user != null)
            {
                var userScript = user.GetComponent<UserScripts>();
                if (userScript != null)
                {
                    if (userScript.user != null)
                    {
                        var currentValue = userScript.user.getCurrency(reward.currency.id);
                        userScript.user.setCurrency(reward.currency.id, currentValue + reward.currency.amount);
                        Debug.Log("Money successfully added!");
                        UIManagerScript.CoinsAndPhotoPuzzles();
                    }
                    else
                    {
                        Debug.Log("The User data wasn't found!");
                    }
                }
                else
                {
                    Debug.Log("The UserScripts component of the User game object wasn't found!");
                }
            }
            else
            {
                Debug.Log("The User game object wasn't found!");
            }
        }
    }
}
