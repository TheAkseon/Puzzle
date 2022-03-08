using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ImagesScript : MonoBehaviour
{
    public static void LoadTexture(Texture2D texture, string filename)
    {
        byte[] bytes = texture.EncodeToJPG();
        File.WriteAllBytes(Application.persistentDataPath + "/" + filename + ".jpg", bytes);
    }

    public static Texture2D LoadPNG(string filePath, GameObject Loader = null)
    {

        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
            if (Loader != null)
                Destroy(Loader);
        }
        return tex;
    }

    public static void load(string filePath, RawImage image, GameObject Loader = null)
    {
        LoadImage(filePath, image, Loader);
    }

    public static void LoadImage(string filePath, RawImage image, GameObject Loader)
    {
        Texture2D tex = null;
        byte[] fileData;
        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(1, 1);
            tex.LoadImage(fileData);
        }
        if (image != null && tex != null)
        {
            image.texture = tex;
            ResizeImage(image);
        }
        if (Loader != null)
            Destroy(Loader);

    }

    public static void setTextureFromURL(string url, GameObject Loader, RawImage image, string id, string updated_at, string categoryId = "", string prefix = "")
    {
        getCachedWWW(url, Loader, image, id, updated_at, categoryId, prefix);
    }

    public static void ResizeImage(RawImage image)
    {
        if (image != null && image.texture != null)
        {
            Rect r = new Rect(0, 0, 1, 1);
            r.width = (float)image.texture.height / image.texture.width;
            r.x = (1 - r.width) / 2;
            image.uvRect = r;
            image.enabled = true;
        }
    }

    public static void getCachedWWW(string url, GameObject Loader, RawImage image, string id, string updated_at, string categoryId = "", string prefix = "")
    {
        url = configScripts.server + url;
        string filePath = Application.persistentDataPath + "/cacheImage";
        string filename;
        if (categoryId == "")
        {
            filename = id;
            filePath += "/" + filename;
        }
        else
        {
            filename = categoryId + "/";
            filePath += "/" + filename;
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);
            filePath += prefix + id;
        }
        filePath += ".jpg";
        string loadFilepath = filePath;
        bool web = false;
        bool useCached = System.IO.File.Exists(filePath);
        if (File.GetLastWriteTimeUtc(filePath) < System.DateTime.Parse(updated_at))
            useCached = false;
        if (useCached)
        {
            LoadImage(loadFilepath, image, null);
            if (Loader != null)
            {
                Destroy(Loader);
                System.GC.Collect();
            }
        }
        else
        {
            web = true;
            WWW www = new WWW(url);
            GameObject uimanager = GameObject.Find("UIManager");
            MonoBehaviour dummy = uimanager.GetComponent<UIManagerScript>();
            bool save = false;
            if (!string.IsNullOrEmpty(prefix))
                save = true;
            if (!useCached || image != null)
                dummy.StartCoroutine(doLoad(www, image, filePath, web, Loader, save));
        }

    }

    public static IEnumerator doLoad(WWW www, RawImage image, string filePath = "", bool web = false, GameObject Loader = null, bool save = false)
    {
        yield return www;
        if (www.error == null)
        {
            if (image != null)
            {
                image.texture = www.texture;
                ResizeImage(image);
            }
            if (Loader != null)
            {
                Destroy(Loader);
                System.GC.Collect();
            }
            if (web)
            {
                if (save)
                    File.WriteAllBytes(filePath, www.bytes);
            }
        }
        else if (!web)
        {
            File.Delete(filePath);
        }
        www.Dispose();
        CulculateSize();
    }

    public static void CulculateSize()
    {
        var d = new DirectoryInfo(Application.persistentDataPath + "/cacheImage");
        float size = 0;
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }
        string sSize = (Mathf.Round(size / 1024 / 1024 * 10) / 10) + " mb";
        PlayerPrefs.SetString("cache_size", sSize);
    }

    public static float DirSize(DirectoryInfo d)
    {
        float size = 0;
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            size += fi.Length;
        }
        DirectoryInfo[] dis = d.GetDirectories();
        foreach (DirectoryInfo di in dis)
        {
            size += DirSize(di);
        }
        return size;
    }
}
