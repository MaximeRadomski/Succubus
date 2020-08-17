using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicBhv : MonoBehaviour
{
    private AudioSource _audioSource;

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
        SceneManager.sceneLoaded += SceneLoaded;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = (AudioClip)Resources.Load("Musics/MainMenu");
        _audioSource.Play();
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Constants.TrainingGameScene)
            _audioSource.Stop();
    }
}
