using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicControlerBhv : MonoBehaviour
{
    private AudioSource _audioSource;
    private MusicTyoe _currentType;
    private float _level;
    private bool _isHalved;

    void Start()
    {
        var musicsGameObjects = GameObject.FindGameObjectsWithTag(Constants.TagMusic);
        if (musicsGameObjects.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        Init();
    }

    private void Init()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        _currentType = MusicTyoe.None;
        SetNewVolumeLevel();
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public void SetNewVolumeLevel(float? level = null)
    {
        if (_isHalved && !SceneManager.GetActiveScene().name.Contains("SettingsAudio"))
            _isHalved = false;
        if (level == null)
            _level = PlayerPrefsHelper.GetMusicLevel();
        else
            _level = level ?? 0.0f;
        _audioSource.volume = Constants.MaximumVolumeMusic * _level;
    }

    public void HalveVolume()
    {
        _isHalved = true;
        _audioSource.volume = Constants.MaximumVolumeMusic * (_level / 2.5f);
    }

    public void Mute()
    {
        _audioSource.volume = 0;
    }

    public void Stop()
    {
        _audioSource.Stop();
    }

    public void PlayFromStart()
    {
        _audioSource.Stop();
        _audioSource.Play();
    }

    public void Play()
    {
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_isHalved)
            HalveVolume();
        if (_currentType == Constants.CurrentMusicType)
            return;
        _currentType = Constants.CurrentMusicType;
        if (_currentType == MusicTyoe.SplashScreen)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/SplashScreen");
        else if (_currentType == MusicTyoe.Menu)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/MainMenu");
        else if (_currentType == MusicTyoe.GameHell)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/GameHell");
        _audioSource.Play();
    }
}
