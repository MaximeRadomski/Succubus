using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicBhv : MonoBehaviour
{
    private AudioSource _audioSource;
    private MusicTyoe _currentType;
    private float _level;

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
        SceneManager.sceneLoaded += SceneLoaded;
        _currentType = MusicTyoe.None;
        SetNewVolumeLevel();
        SceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public void SetNewVolumeLevel(float? level = null)
    {
        if (level == null)
            _level = PlayerPrefsHelper.GetMusicLevel();
        else
            _level = level ?? 0.0f;
        _audioSource.volume = Constants.MaximumVolumeMusic * _level;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
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
