using UnityEngine;
using UnityEngine.UI;

public class SoundChange : MonoBehaviour
{
    public Sprite img_sound_on;
    public Sprite img_sound_off;
    public string par = "sound";

    private void Start()
    {
        SetSprite(!PlayerPrefs.HasKey(par) || PlayerPrefs.GetInt(par) != 0);
    }

    public void OnClick()
    {
        AudioScripts a = GameObject.Find("MainAudio").GetComponent<AudioScripts>();
        a.ClickSound();
        bool stat = false;
        if (par == "sound")
        {
            stat = !a.soundStat;
            a.soundStat = stat;
        }
        else if (par == "music")
        {
            stat = !a.musicStat;
            a.musicStat = stat;
            a.source.mute = !stat;
        }
        int s = stat ? 1 : 0;
        PlayerPrefs.SetInt(par, s);
        SetSprite(stat);
    }

    public void SetSprite(bool v)
    {
        if (v)
        {
            transform.GetComponent<Image>().sprite = img_sound_on;
        }
        else
        {
            transform.GetComponent<Image>().sprite = img_sound_off;
        }
    }
}
