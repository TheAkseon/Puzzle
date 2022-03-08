using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CoinsScript : MonoBehaviour
{
    public void ClosePopUp()
    {
        UIManagerScript.ClosePopUp("CoinsPriceListPopUp");
    }

    public void RestorePurchases()
    {
        StartCoroutine(RestorePurchasesCoroutine());
    }

    public IEnumerator RestorePurchasesCoroutine()
    {
        AudioScripts.Click();
        int c = 0;
        UIManagerScript.StartLoader();
        while (!Purchaser.main.IsInitialized() && c < 10)
        {
            Purchaser.main.LoadPurchaser();
            c++;
            yield return new WaitForSeconds(1);
        }
        if (Purchaser.main.IsInitialized())
        {
            var inRestoreProcess = Purchaser.main.RestorePurchases(result =>
            {
                // TODO: Some message?
                Destroy(GameObject.Find("Loader"));
            });
            if (!inRestoreProcess)
            {
                Destroy(GameObject.Find("Loader"));
            }
        }
        else
        {
            Destroy(GameObject.Find("Loader"));
        }
    }

    public static void GetProducts(string json, int status)
    {

    }

    public void getCoinsPriceListPopUpButton()
    {
        getCoinsPriceListPopUp();
    }

    public static void getCoinsPriceListPopUp()
    {
        Purchaser.main.StartCoroutine(OpenShop());
    }

    public static IEnumerator OpenShop()
    {
        AudioScripts.Click();
        UIManagerScript.StartLoader();
        int c = 0;
        while (!Purchaser.main.IsInitialized() && c < 10)
        {
            Purchaser.main.LoadPurchaser();
            c++;
            yield return new WaitForSeconds(1);
        }
        Destroy(GameObject.Find("Loader"));
        if (Purchaser.main.IsInitialized())
        {
            ProductList prList = Purchaser.main.prList;
            GameObject PopUp = UIManagerScript.StartPopUp("CoinsPriceListPopUp", true, Vector2.zero, -1);
            Transform cParent = PopUp.transform.Find("PricesPanel").transform.Find("Panel");
            cParent.GetComponent<RectTransform>().sizeDelta = new Vector2(cParent.GetComponent<RectTransform>().sizeDelta.x, 600 * Mathf.Ceil((prList.products.Length + 1) / 2) + 250);
            for (int i = 0; i < prList.products.Length; i++)
            {
                GameObject Product = UIManagerScript.CreateCategory("PriceItemPrefab", UIManagerScript.NewPos(i, 2, 600, 750, 850, -125), cParent);
                Product.name = prList.products[i].id.ToString();
                Purchaser.main.ProductImage(i, Product.transform.Find("ItemImage").GetComponent<RawImage>());
                Product.transform.Find("Price").GetComponent<Text>().text = prList.products[i].textPrice;
                Text nameTest = Product.transform.Find("Name").GetComponent<Text>();
                nameTest.text = prList.products[i].name.ToString();
                string coinName = string.Empty;
                if (prList.products[i].product_currencies[0].count > 0)
                {
                    nameTest.text += "\n" + prList.products[i].product_currencies[0].count;
                    coinName = "puzzle";
                }
                else if (prList.products[i].product_currencies[1].count > 0)
                {
                    nameTest.text += "\n" + prList.products[i].product_currencies[1].count;
                    coinName = "photo-puzzle";
                }
                if (!string.IsNullOrEmpty(coinName))
                {
                    Image im = Product.transform.Find("CoinImage").GetComponent<Image>();
                    im.enabled = true;
                    im.sprite = (Sprite)Resources.Load("Images/Interface/header/" + coinName, typeof(Sprite));
                }
                Product.GetComponent<ProductItemScript>().id = i;
            }
        }
        else
        {
            var errorLocalized = LocalizationManager.Instance.GetLocalizedValue("error");
            var storeUnavailableLocalized = LocalizationManager.Instance.GetLocalizedValue("store_unavailable");
            Purchaser.main.PurchaserFinish(errorLocalized, storeUnavailableLocalized);
        }
    }

    [System.Serializable]
    internal class Nullable
    {
        public float? f;

        public Nullable(float? f)
        {
            if (f != null)
            {
                this.f = f.Value;
            }
        }
    }
}
