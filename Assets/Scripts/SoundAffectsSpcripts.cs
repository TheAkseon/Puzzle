using UnityEngine;

public class SoundAffectsSpcripts : MonoBehaviour
{
    // Use this for initialization
    public void Play()
    {
        transform.Find("Audio").transform.GetComponent<AudioSource>().mute = false;
    }
}
