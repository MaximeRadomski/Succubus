using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControlerBhv : MonoBehaviour
{
    public int ClickIn;
    public int ClickOut;

    public List<Sound> Sounds;

    private AudioSource _pcAudio;
    private int _currentId;
    private bool _hasInit;
    private float _level;

    void Start()
    {
        if (!_hasInit)
            Init();
    }

    private void Init()
    {
        Sounds = new List<Sound>();
        _pcAudio = gameObject.GetComponent<AudioSource>();
        _currentId = 0;
        SetBasicAudios();
        _hasInit = true;
        SetNewVolumeLevel();
    }

    private void SetBasicAudios()
    {
        Sounds.Add(new Sound(_currentId++, "ClickIn"));
        Sounds.Add(new Sound(_currentId++, "ClickOut"));
#if UNITY_EDITOR
#else
        AndroidNativeAudio.makePool();
        ClickIn = AndroidNativeAudio.load(Sounds[0].Name + ".mp3");
        ClickOut = AndroidNativeAudio.load(Sounds[1].Name + ".mp3");
#endif
    }

    public int SetSound(string name)
    {
        if (!_hasInit)
            Init();
#if UNITY_EDITOR
        Sounds.Add(new Sound(_currentId, name));
        return _currentId++;
#else
        return AndroidNativeAudio.load(name + ".mp3");
#endif
    }

    public void PlaySound(int soundId, float customRate = 1.0f)
    {
        if (!_hasInit)
            Init();
#if UNITY_EDITOR
        _pcAudio.pitch = customRate;
        _pcAudio.PlayOneShot((AudioClip)Resources.Load("Sounds/" + Sounds[soundId].Name));
#else
        AndroidNativeAudio.play(soundId, _level, rate: customRate);
#endif
    }

    public void SetNewVolumeLevel(float? level = null)
    {
        if (level == null)
            _level = PlayerPrefsHelper.GetEffectsLevel();
        else
            _level = level ?? 0.0f;
        _pcAudio.volume = Constants.MaximumEffectsMusic * _level;
    }

    void OnDestroy()
    {
        AndroidNativeAudio.unload(ClickIn);
        AndroidNativeAudio.unload(ClickOut);

        AndroidNativeAudio.releasePool();
    }
}
