using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // Gets the list of devices and prints them to the console.
    private bool camAvailable;
    private bool cameraperm = false;
    private WebCamTexture backCam;
    public GameObject photoView;
    public GameObject photoViewGallery;
    public GameObject CameraInterface;
    public GameObject CropView;
    public GameObject CropButtons;
    public RawImage background;
    public RawImage cropPhoto;
    public AspectRatioFitter fit;
    private bool photoTaken = false;
    private bool isCroped;
    private int cid = 0;
    private bool confirmPurchase = false;
    public RectTransform Content;
    public Texture2D main_background;

    [Header("Loader Problem")]
    [SerializeField]
    GameObject Loader;

    private Texture2D newTex;

    private void Start()
    {
        isCroped = false;
        confirmPurchase = false;
        StartCoroutine(askPermission());
    }

    public static void resizeImage(GameObject go, Vector2 size)
    {
        RawImage image = go.GetComponent<RawImage>();
        image.SetNativeSize();
        Vector2 newScale = new Vector2((float)Screen.height / size.y * size.x, Screen.height) / GameObject.Find("Canvas").GetComponent<RectTransform>().localScale.x;
        go.GetComponent<RectTransform>().sizeDelta = newScale;
        image.transform.localPosition = new Vector3(0, 0, 0);
    }

    private void Display()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length == 0)
        {
            camAvailable = false;
            return;
        }
        backCam = new WebCamTexture(1280, 800);
        if (backCam == null)
        {
            return;
        }
        backCam.filterMode = FilterMode.Trilinear;
        backCam.Play();
        background.texture = backCam;
        resizeImage(GameObject.Find("CameraCapture").transform.Find("Canvas").transform.Find("RawImage").gameObject, new Vector2(backCam.requestedWidth, backCam.requestedHeight));
        camAvailable = true;
    }

    private IEnumerator askPermission()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam))
        {
            cameraperm = true;
            Display();
        }
        else
        {
            cameraperm = false;
        }
    }

    private void Update()
    {
        if (!camAvailable || !cameraperm)
            return;
        float ratio = (float)backCam.width / (float)backCam.height;
        fit.aspectRatio = ratio;
        float scaleX = (photoTaken && backCam.videoRotationAngle == 180) ? -1f : 1f;
        float scaleY = (backCam.videoRotationAngle == 0 && !photoTaken || backCam.videoRotationAngle == 180) && !Application.isEditor ? -1f : 1f;
        background.rectTransform.localScale = new Vector3(scaleX, scaleY, 1f);
        int orient = -backCam.videoRotationAngle;
        background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);


        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            float pinchAmount = deltaMagnitudeDiff * 0.02f * Time.deltaTime;
            if (cropPhoto.rectTransform.localScale.x <= 1)
            {
                cropPhoto.rectTransform.localScale = new Vector3(1, 1);
            }
            if (cropPhoto.rectTransform.localScale.x <= 0.1)
            {
                cropPhoto.rectTransform.localScale = new Vector3(0.1f, 0.1f);
            }
            cropPhoto.rectTransform.localScale -= new Vector3(pinchAmount, pinchAmount, pinchAmount);
        }
    }

    private int getId()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/photoPuzzles"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/photoPuzzles");
        }
        var info = new DirectoryInfo(Application.persistentDataPath + "/photoPuzzles");
        var fileInfo = info.GetFiles();
        int id = -1;
        foreach (FileInfo file in fileInfo)
        {
            string tmp = file.ToString();
            //if (Application.isEditor)
            //{
                tmp = tmp.Substring((Application.persistentDataPath + "/photoPuzzles/").Length);
                Debug.Log("tmp(1): " + tmp);
            //}

            Debug.Log("try SaveImage");
            tmp = tmp.Substring(0, tmp.Length - ".jpg".Length);
            Debug.Log("tmp(2): " + tmp);

            int tmpToInt = 0;//new
            int.TryParse(tmp, out tmpToInt);//new
            Debug.Log("tmpInt(3): " + tmpToInt);

            if (id < tmpToInt)//new
                id = tmpToInt;//new

            Debug.Log("ID(4): " + id);
            //if (id < int.Parse(tmp)) //old
                //id = int.Parse(tmp); //old
        }

        return id + 1;
    }

    private void OnDisable()
    {
        backCam.Stop();
        if (!confirmPurchase && photoTaken)
        {
            Delete();
        }
    }

    public void SaveImage()
    {
        AudioScripts.Click();
        cid = getId();
        Debug.Log("Cid(5): " + cid);

        photoTaken = true;
        Texture2D texture = new Texture2D(backCam.width, backCam.height);
        Color[] image = backCam.GetPixels();
        image = Flip(image, backCam.videoRotationAngle == 180, backCam.videoRotationAngle == 180);
        texture.SetPixels(image);

        string path = "photoPuzzles/" + cid.ToString();
        Debug.Log("Path(6): " + path);

        ImagesScript.LoadTexture(texture, path);
        byte[] fileData = File.ReadAllBytes(Application.persistentDataPath + "/" + path + ".jpg");

        texture = new Texture2D(backCam.width, backCam.height);
        texture.LoadImage(fileData);
        background.texture = texture;

        GameObject.Find("CameraInterface").SetActive(false);
        photoView.SetActive(true);
    }

    public void Retry()
    {
        AudioScripts.Click();
        Delete();
        UIManagerScript.LoadScene("createMyPuzzle");
    }

    public void OpenGallery()
    {
        AudioScripts.Click();
        background.texture = main_background;
        Debug.Log("openning gallery");
        AudioScripts.Click();
        CameraInterface.SetActive(false);
        StartCoroutine(WaitGallery());
    }

    private IEnumerator WaitGallery()
    {
        yield return new WaitForSeconds(1);
        background.texture = main_background;

        NativeGalleryUsage.PickImage((textureFromGallery) =>
        {
            cid = getId();
            photoTaken = true;

            if (textureFromGallery == null)
            {
                background.texture = main_background;
                photoViewGallery.SetActive(true);
            }

            Texture2D fake_image = textureFromGallery;/* new Texture2D(1000, 1000);*/ //image selected from camera roll
            Texture2D resImage = new Texture2D(Screen.width, Screen.height);
            Color[] image = resImage.GetPixels();
            string path = "photoPuzzles/" + cid;

            ImagesScript.LoadTexture(textureFromGallery, path);
            byte[] fileData = File.ReadAllBytes(Application.persistentDataPath + "/" + path + ".jpg");
            Texture2D texture = new Texture2D(fake_image.width, fake_image.height);
            texture.SetPixels(image);
            texture.LoadImage(fileData);
            background.texture = texture;
            photoViewGallery.SetActive(true);
        });
    }

    public void Crop()
    {
        AudioScripts.Click();
        cropPhoto.texture = null;
        CropView.SetActive(true);
        photoView.SetActive(false);
        photoViewGallery.SetActive(false);
        cropPhoto.texture = background.texture;
        //cropPhoto.texture = testCrop;
        cropPhoto.SetNativeSize();
        ResizeCrop();
        background.texture = main_background;
    }

    public void Bigger()
    {
        AudioScripts.Click();
        cropPhoto.rectTransform.localScale += new Vector3(0.1f, 0.1f);
    }

    public void Smaller()
    {
        if (cropPhoto.rectTransform.localScale.x > 1)
        {
            AudioScripts.Click();
            cropPhoto.rectTransform.localScale -= new Vector3(0.1f, 0.1f);
        }
        if (cropPhoto.rectTransform.localScale.x <= 1)
        {
            AudioScripts.Click();
            cropPhoto.rectTransform.localScale = new Vector3(1, 1);
        }
    }

    private void ResizeCrop()
    {
        if (cropPhoto.texture.width < cropPhoto.texture.height)
        {
            float koefCh = cropPhoto.texture.height / cropPhoto.texture.width;
            cropPhoto.rectTransform.localScale += new Vector3(koefCh, koefCh);
        }

        if (cropPhoto.texture.width > cropPhoto.texture.height)
        {
            float koefCh = cropPhoto.texture.width / cropPhoto.texture.height;
            cropPhoto.rectTransform.localScale += new Vector3(koefCh, koefCh);
        }
    }

    public void CropArea()
    {
        CropButtons.SetActive(false);
        AudioScripts.Click();
        StartCoroutine(WaitCrop());
    }

    private IEnumerator WaitCrop()
    {
        Delete();
        yield return new WaitForSecondsRealtime(0.2f);
        cid = getId();
        //cid -= 1;
        photoTaken = true;
        isCroped = true;
        Texture2D texture = new Texture2D(Screen.width, Screen.height);
        yield return new WaitForEndOfFrame();
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        string path = "photoPuzzles/" + cid;
        ImagesScript.LoadTexture(texture, path);
        byte[] fileData = File.ReadAllBytes(Application.persistentDataPath + "/" + path + ".jpg");
        texture = new Texture2D(Screen.width, Screen.height);
        texture.LoadImage(fileData);
        ImagesScript.LoadTexture(texture, path);
        yield return new WaitForSecondsRealtime(1.5f);
        Create();
    }

    public void CancelCrop()
    {
        AudioScripts.Click();
        cropPhoto.texture = null;
        CropView.SetActive(false);
    }

    private void Delete()
    {
        File.Delete(Application.persistentDataPath + "/photoPuzzles/" + cid + ".jpg");
    }

    public Color[] Flip(Color[] data, bool vertical, bool horizontal)
    {
        Color[] flipImage = new Color[data.Length];
        int index = 0;
        for (int x = 0; x < backCam.width; x++)
        {
            for (int y = 0; y < backCam.height; y++)
            {
                if (horizontal && vertical)
                    index = backCam.width - 1 - x + (backCam.height - 1 - y) * backCam.width;
                else if (horizontal && !vertical)
                    index = backCam.width - 1 - x + y * backCam.width;
                else if (!horizontal && vertical)
                    index = x + (backCam.height - 1 - y) * backCam.width;
                else if (!horizontal && !vertical)
                    index = x + y * backCam.width;

                flipImage[x + y * backCam.width] = data[index];
            }
        }
        return flipImage;
    }

    public void Create()
    {
        isCroped = false;
        UIManagerScript.StartLoader();
        APIMethodsScript.sendRequest("patch", "/api/user/spendPhotopuzzle", checkPurchase);
    }

    public void checkPurchase(string json, int state)
    {
        int checkMoney;
        Destroy(GameObject.Find("Loader").gameObject);
        checkMoney = GameObject.Find("User").GetComponent<UserScripts>().user.user_currencies[1].count;
        if (checkMoney > 0)
        {
            GameObject.Find("User").GetComponent<UserScripts>().user.user_currencies[1].count--;
            confirmPurchase = true;
        }
        else
        {
            Delete();
        }
        //Destroy(background.texture);
        UIManagerScript.LoadScene("mypuzzles");
    }

    /*public void checkPurchase(string json, int state)
	{
        Destroy(GameObject.Find("Loader").gameObject);
		if (state == 200)
		{
			GameObject.Find("User").GetComponent<UserScripts>().user.user_currencies[1].count--;
			confirmPurchase = true;
            Debug.Log("go");
		}
		else
		{
            Delete();
            Debug.Log("no");
        }

		Destroy(background.texture);

		UIManagerScript.LoadScene("mypuzzles");
	}*/
}