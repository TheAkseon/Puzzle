using System.Collections;
using UnityEngine;

public class AudioScripts : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioClip[] musics;
    public AudioSource source;
    public AudioSource clickSource;
    public bool soundStat;
    public bool musicStat;

    private void Start()
    {
        soundStat = !PlayerPrefs.HasKey("sound") || PlayerPrefs.GetInt("sound") != 0;
        musicStat = !PlayerPrefs.HasKey("music") || PlayerPrefs.GetInt("music") != 0;
        StartCoroutine(playEngineSound());
        source.mute = !musicStat;
    }

    private IEnumerator playEngineSound()
    {
        if (source.clip != null)
        {
            yield return new WaitForSeconds(source.clip.length);
        }
        source.clip = musics[Random.Range(0, musics.Length)];
        source.Play();
        StartCoroutine(playEngineSound());
    }

    public void ClickSound(int v = 1)
    {
        if (soundStat)
        {
            clickSource.clip = clips[v];
            clickSource.Play();
        }
    }

    public static void Click()
    {
        GameObject.Find("MainAudio").GetComponent<AudioScripts>().ClickSound();
    }
}
