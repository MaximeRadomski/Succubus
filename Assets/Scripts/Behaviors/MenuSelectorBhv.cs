using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectorBhv : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Reset();
    }

    public void Reset()
    {
        this.transform.position = new Vector3(-30, 30.0f, 0.0f);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
