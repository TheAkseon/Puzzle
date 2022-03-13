using System;
using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using Entities;



    public class GoogleMobileAdsScript : MonoBehaviour
    {
        private RewardedAd rewardBasedVideo;
        private InterstitialAd interstitial;

        public static GoogleMobileAdsScript Instance { get; private set; }

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
            InitializeRewardBasedVideo();
            InitializeInterstitial();
        }

        private void OnDestroy()
        {
            onFinishCallbacks.Clear();
            StopAllCoroutines();
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

        private void InitializeRewardBasedVideo()
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
            rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;
            // Called when an ad request failed to load.
            rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            // Called when an ad is shown.
            rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;
            // Called when the ad starts to play.
            //rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;
            // Called when the user should be rewarded for watching a video.
           // rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;
            // Called when the ad is closed.
            rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;
            // Called when the ad click caused the user to leave the application.
           // rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;
            this.RequestRewardBasedVideo();
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
            this.interstitial = new GoogleMobileAds.Api.InterstitialAd(adUnitId);
            // Called when an ad request has successfully loaded.
            this.interstitial.OnAdLoaded += HandleOnAdLoaded;
            // Called when an ad request failed to load.
            this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            // Called when an ad is shown.
            this.interstitial.OnAdOpening += HandleOnAdOpened;
            // Called when the ad is closed.
            this.interstitial.OnAdClosed += HandleOnAdClosed;
            // Called when the ad click caused the user to leave the application.
            //this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;
            RequestInterstitial();
        }


        private void RequestRewardBasedVideo()
        {
#if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
 string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
 string adUnitId = "unexpected_platform";
#endif
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the rewarded video ad with the request.
           // this.rewardBasedVideo.LoadAd();//request, adUnitId);
        }

        private void RequestInterstitial()
        {
            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            this.interstitial.LoadAd(request);
        }

        /*
        * Reward based video.
        */
        public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoLoaded event received");
        }

        public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
          //  MonoBehaviour.print("HandleRewardBasedVideoFailedToLoad event received with message: " + args.Message);
            StartCoroutine(RewardBasedVideoReloader());
        }

        public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoOpened event received");
        }

        public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoStarted event received");
        }

        public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
        {
            this.RequestRewardBasedVideo();
            InvokeOnFinishCallbacks();
            MonoBehaviour.print("HandleRewardBasedVideoClosed event received");
        }

        public void HandleRewardBasedVideoRewarded(object sender, Reward args)
        {
            string type = args.Type;
            double amount = args.Amount;
            if (needReward)
            {
                RequestReward();
                needReward = false;
            }
            MonoBehaviour.print("HandleRewardBasedVideoRewarded event received for " + amount + " " + type);
        }

        public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleRewardBasedVideoLeftApplication event received");
        }

        /*
        * Interstitial.
        */
        public void HandleOnAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdLoaded event received");
        }

        public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
           // MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
            StartCoroutine(InterstitialReloader());
        }

        public void HandleOnAdOpened(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdOpened event received");
        }

        public void HandleOnAdClosed(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdClosed event received");
            InvokeOnFinishCallbacks();
        }

        public void HandleOnAdLeavingApplication(object sender, EventArgs args)
        {
            MonoBehaviour.print("HandleAdLeavingApplication event received");
        }

        /*
        * Stuff.
        */
        private IEnumerator RewardBasedVideoReloader()
        {
            yield return new WaitForSeconds(3);
            RequestRewardBasedVideo();
        }

        private IEnumerator InterstitialReloader()
        {
            yield return new WaitForSeconds(3);
            RequestInterstitial();
        }

        public void ShowRewardBasedVideo(Action onFinishCallback, bool _needReward = false)
        {
            onFinishCallbacks.Add(onFinishCallback);
            if (rewardBasedVideo.IsLoaded())
            {
                rewardBasedVideo.Show();
                this.needReward = _needReward;
#if UNITY_EDITOR
                InvokeOnFinishCallbacks();
#endif
            }
            else
            {
                InvokeOnFinishCallbacks();
            }
        }
        public void ShowInterstitialAd(Action onFinishCallback)
        {
            onFinishCallbacks.Add(onFinishCallback);
            if (interstitial.IsLoaded())
            {
                interstitial.Show();
#if UNITY_EDITOR
                InvokeOnFinishCallbacks();
#endif
            }
            else
            {
                InvokeOnFinishCallbacks();
            }
        }

        private void InvokeOnFinishCallbacks()
        {
            // Invoke as coroutine to avoid problem related with threads if the GoogleAdsSdk use multiple threads.
            StartCoroutine(InvokeOnFinishCallbacksAsCoroutine());
        }
        /// <summary>
        /// This method will invoke callbacks in next frame in main thread.
        /// </summary>
        private IEnumerator InvokeOnFinishCallbacksAsCoroutine()
        {
            yield return null;
            foreach (var c in onFinishCallbacks)
            {
                c.Invoke();
            }
            onFinishCallbacks.Clear();
        }

        /// <summary>
        /// This method will invoke coroutine in next frame.
        /// </summary>
        public void RequestReward()
        {
            // Invoke as coroutine to avoid problem related with threads if the GoogleAdsSdk use multiple threads.
            StartCoroutine(RequestRewardAsCoroutine());
        }

        /// <summary>
        /// This method will invoke request in next frame in main thread.
        /// </summary>
        private IEnumerator RequestRewardAsCoroutine()
        {
            yield return null;
            APIMethodsScript.sendRequest("patch", "/api/advertisement/watched", GetReward);
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
