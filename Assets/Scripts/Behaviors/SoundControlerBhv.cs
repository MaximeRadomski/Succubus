using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControlerBhv : MonoBehaviour
{
    public int ClickIn;
    public int ClickOut;

    private AudioSource _pcAudio;
    private List<string> _pcAudioReference;

    void Start()
    {
        SetPrivates();
        SetAudios();
    }

    private void SetPrivates()
    {
        _pcAudio = gameObject.GetComponent<AudioSource>();
    }

    private void SetAudios()
    {
#if UNITY_EDITOR
        _pcAudioReference = new List<string>();
        _pcAudioReference.Add("ClickIn");
        _pcAudioReference.Add("ClickOut");
#else
        AndroidNativeAudio.makePool();
        ClickIn = AndroidNativeAudio.load("ClickIn.mp3");
        ClickOut = AndroidNativeAudio.load("ClickOut.mp3");
#endif
    }

    public void PlaySound(int id, float customRate = 1.0f)
    {
#if UNITY_EDITOR
        _pcAudio.PlayOneShot((AudioClip)Resources.Load("Sounds/" + _pcAudioReference[id]));
#else
        float level = PlayerPrefs.GetFloat(Constants.PpAudioLevel, Constants.PpAudioLevelDefault);
        AndroidNativeAudio.play(id, level, rate: customRate);
#endif
    }

    void OnDestroy()
    {
        AndroidNativeAudio.unload(ClickIn);
        AndroidNativeAudio.unload(ClickOut);

        AndroidNativeAudio.releasePool();
    }
}
