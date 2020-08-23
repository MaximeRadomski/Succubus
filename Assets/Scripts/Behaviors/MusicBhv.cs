using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicBhv : MonoBehaviour
{
    private AudioSource _audioSource;
    private MusicTyoe _currentType;

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
        SceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
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
