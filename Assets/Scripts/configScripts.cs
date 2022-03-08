using UnityEngine;

public class configScripts : MonoBehaviour
{
    public static string server;

    public class ConfigFile
    {
        public string server;

        public ConfigFile()
        {
            this.server = "";
        }

        public ConfigFile(string server)
        {
            this.server = server;
        }

        public ConfigFile(ConfigFile other)
        {
            this.server = other.server;
        }
    }

    public static void LoadServer()
    {
        TextAsset txt = (TextAsset)Resources.Load("config", typeof(TextAsset));
        ConfigFile conf = JsonUtility.FromJson<configScripts.ConfigFile>(txt.text);
        server = conf.server;
    }
}
