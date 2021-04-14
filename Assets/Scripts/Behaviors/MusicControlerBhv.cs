using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicControlerBhv : MonoBehaviour
{
    private AudioSource _audioSource;
    private MusicType _currentType;
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
        _currentType = MusicType.None;
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

    public void Pause()
    {
        _audioSource.Pause();
    }

    public void PlayFromStart()
    {
        _audioSource.Stop();
        _audioSource.Play();
    }

    public void Play(string musicName = null, bool once = false)
    {
        if (musicName != null)
            _audioSource.clip = (AudioClip)Resources.Load($"Musics/{musicName}");
        if (!_audioSource.isPlaying && once == false)
            _audioSource.Play();
        else if (once == true && musicName != null)
            _audioSource.PlayOneShot(_audioSource.clip);
    }

    public void Play(AudioClip clip)
    {
        _audioSource.clip = clip;
        if (!_audioSource.isPlaying)
            _audioSource.Play();
    }

    public void ResetSceneLoadedMusic(bool manualReset = false)
    {
        if (_isHalved)
            HalveVolume();
        var sceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
        if ((_currentType == sceneBhv.MusicType && !manualReset)
            || sceneBhv.MusicType == MusicType.Continue)
            return;
        _currentType = sceneBhv.MusicType;
        if (_currentType == MusicType.SplashScreen)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/SplashScreen");
        else if (_currentType == MusicType.Menu)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/MainMenu");
        else if (_currentType == MusicType.Game)
        {
            var randomId = Random.Range(0, 0);
            _audioSource.clip = (AudioClip)Resources.Load($"Musics/Game{randomId.ToString("00")}");
        }
        else if (_currentType == MusicType.Ascension)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/Ascension");
        else if (_currentType == MusicType.GameOver)
            _audioSource.clip = (AudioClip)Resources.Load("Musics/Devlog");
        else
            _audioSource.clip = null;
        _audioSource.Play();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetSceneLoadedMusic();
    }
}
