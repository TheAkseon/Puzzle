using System;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.UI;

[Serializable]
public class ProductItem
{
    public int id;
    public string created_at;
    public string updated_at;
    public string name;
    public string description;
    public string image;
    public int price;
    public string itunes_product_id;
    public BuyPuzzles.Currency[] product_currencies;
    public string textPrice;
    public Product p;
}

[Serializable]
public class ProductList
{
    public ProductItem[] products;
}

public class Purchaser : MonoBehaviour, IStoreListener
{
    // TODO: Think about dynamic ID...
    private const string ADS_PRODUCT_ITUNCE_ID = "pazzlarium.product.5";

    public ProductList prList;
    public static Purchaser main;
    private IStoreController s;
    private IExtensionProvider provider;
    private int cProd = 0;

    private void Start()
    {
        main = this;
    }

    public void LoadPurchaser()
    {
        APIMethodsScript.sendRequest("get", "/api/product", GetProducts);
    }

    private void GetProducts(string json, int status)
    {
        prList = JsonUtility.FromJson<ProductList>(json);
        ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        for (int i = 0; i < prList.products.Length; i++)
        {
            if (!string.IsNullOrEmpty(prList.products[i].itunes_product_id))
            {
                if (prList.products[i].itunes_product_id == ADS_PRODUCT_ITUNCE_ID)
                {
                    builder.AddProduct(prList.products[i].itunes_product_id, ProductType.NonConsumable);
                }
                else
                {
                    builder.AddProduct(prList.products[i].itunes_product_id, ProductType.Consumable);
                }
            }
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController store, IExtensionProvider extensions)
    {
        s = store;
        provider = extensions;
        for (int i = 0; i < prList.products.Length; i++)
        {
            if (!string.IsNullOrEmpty(prList.products[i].itunes_product_id))
            {
                prList.products[i].p = store.products.WithID(prList.products[i].itunes_product_id);
                prList.products[i].name = prList.products[i].name;
                prList.products[i].textPrice = prList.products[i].p.metadata.localizedPriceString;
            }
            else
            {
                // Product for puzzles.
                var currencyPuzzlesLocalized = LocalizationManager.Instance.GetLocalizedValue("currency_puzzles");
                prList.products[i].textPrice = Math.Abs(prList.products[i].product_currencies[0].count)  + " " + currencyPuzzlesLocalized;
            }
        }
    }

    public bool IsInitialized()
    {
        return s != null /*&& provider != null*/;
    }

    public void BuyProduct(int v)
    {
        if (IsInitialized())
        {
            UIManagerScript.StartPopUp("PurchaserLoader");
            UIManagerScript.ClosePopUp("CoinsPriceListPopUp");
            if (!string.IsNullOrEmpty(prList.products[v].itunes_product_id))
            {
                Debug.Log("InitiatePurchase: " + prList.products[v].itunes_product_id);
                s.InitiatePurchase(prList.products[v].p);
            }
            else
            {
                ProcessPurchase2(v);
            }
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFAILED: " + error);
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        var errorLocalized = LocalizationManager.Instance.GetLocalizedValue("error");
        var purchaseCanceledLocalized = LocalizationManager.Instance.GetLocalizedValue("purchase_canceled");
        PurchaserFinish(errorLocalized, purchaseCanceledLocalized);
        Debug.Log("OnPurchaseFAILED: " + i.metadata.localizedTitle + "\nReason: " + p);
    }

    public GameObject PurchaserFinish(string title, string text)
    {
        GameObject PopUp = UIManagerScript.StartPopUp("PurchaserFinish");
        UIManagerScript.ClosePopUp("PurchaserLoader");
        if (!string.IsNullOrEmpty(title))
        {
            PopUp.transform.Find("Title").Find("Text").GetComponent<Text>().text = title;
        }
        PopUp.transform.Find("Text").GetComponent<Text>().text = text;
        return PopUp;
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        bool validPurchase = true;
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        if (!Application.isEditor)
        {
            var validator = new CrossPlatformValidator(null, AppleTangle.Data(), Application.identifier);
            try
            {
                var result = validator.Validate(e.purchasedProduct.receipt);
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    TryOffAdsByITunceId(productReceipt);
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                }
            }
            catch (IAPSecurityException)
            {
                var errorLocalized = LocalizationManager.Instance.GetLocalizedValue("error");
                var purchaseNotHappen = LocalizationManager.Instance.GetLocalizedValue("purchase_not_happen");
                PurchaserFinish(errorLocalized, purchaseNotHappen);
                validPurchase = false;
            }
        }
#endif

        if (validPurchase)
        {
            for (int i = 0; i < prList.products.Length; i++)
            {
                if (String.Equals(e.purchasedProduct.definition.id, prList.products[i].itunes_product_id, StringComparison.Ordinal))
                {
                    ProcessPurchase2(i, e.purchasedProduct.receipt);
                }
            }
        }
        return PurchaseProcessingResult.Complete;
    }

    private void TryOffAdsByITunceId(IPurchaseReceipt productReceipt)
    {
        if (productReceipt.productID == ADS_PRODUCT_ITUNCE_ID)
        {
            PlayerPrefs.SetString(DifficultyScript.IS_ADS_OFF_KEY, "ADS_IS_OFF");
            PlayerPrefs.Save();
        }
    }

    public bool RestorePurchases(Action<bool> callback)
    {
        if (!IsInitialized())
        {
            Debug.Log("[RestorePurchases] RestorePurchases FAILED. Not initialized.");
            return false;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("[RestorePurchases] RestorePurchases started ...");
            var apple = provider.GetExtension<IAppleExtensions>();
            apple.RestoreTransactions((result) =>
            {
                callback.Invoke(result);
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
            return true;
        }
        else
        {
            Debug.Log("[RestorePurchases] RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            return false;
        }
    }

    /// <summary>
    /// Purchase for puzzles currency.
    /// </summary>
    public void ProcessPurchase2(int v, string json = default(string))
    {
        cProd = v;
        if (string.IsNullOrEmpty(json))
        {
            APIMethodsScript.sendRequest("post", "/api/product/" + prList.products[v].id + "/buy", ContinuePurchase);
        }
        else
        {
            Debug.Log(json);
            Debug.Log(prList.products[v].id);
            byte[] rData = System.Text.Encoding.ASCII.GetBytes(json);
            APIMethodsScript.sendRequest("post", "/api/product/" + prList.products[v].id + "/buy", ContinuePurchase, rData, "application/json");
        }
    }

    public void ProductImage(int v, RawImage i)
    {
        i.enabled = true;
        ImagesScript.setTextureFromURL(prList.products[v].image, null, i, prList.products[v].id.ToString(), prList.products[v].created_at, "products", "p");
    }

    public void ContinuePurchase(string json, int status)
    {
        Debug.Log(json);
        if (status == 200)
        {
            var youPurchasedSomeLocalized = LocalizationManager.Instance.GetLocalizedValue("you_purchased_some");
            var message = string.Format("{0} {1}.", youPurchasedSomeLocalized, prList.products[cProd].name);
            GameObject PopUp = PurchaserFinish(string.Empty, message);
            ProductImage(cProd, PopUp.transform.Find("ItemImage").GetComponent<RawImage>());
            BuyPuzzles.BuyPuzzleResponse resp = JsonUtility.FromJson<BuyPuzzles.BuyPuzzleResponse>(json);
            if (resp.currencies != null)
            {
                UserScripts u = GameObject.Find("User").GetComponent<UserScripts>();
                foreach (BuyPuzzles.Currency c in resp.currencies)
                {
                    u.user.setCurrency(c.currency_id, c.count);
                }
                UIManagerScript.LerpCoins(u.user.getCurrency(resp.currencies[0].currency_id), GameObject.Find("Coins").transform.Find("Text").GetComponent<Text>());
                UIManagerScript.LerpCoins(u.user.getCurrency(resp.currencies[1].currency_id), GameObject.Find("PhotoPuzzles").transform.Find("Text").GetComponent<Text>());
            }
        }
        else
        {
            var errorLocalized = LocalizationManager.Instance.GetLocalizedValue("error");
            var purchaseNotHappen = LocalizationManager.Instance.GetLocalizedValue("purchase_not_happen");
            PurchaserFinish(errorLocalized, purchaseNotHappen);
        }
    }
}
