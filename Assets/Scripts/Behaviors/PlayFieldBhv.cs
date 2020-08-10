using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFieldBhv : MonoBehaviour
{
    public Transform[,] Grid;

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        var tmp = GameObject.FindGameObjectsWithTag(Constants.TagPlayField);
        if (tmp.Length > 1)
            Destroy(gameObject);
        else
            DontDestroyOnLoad(gameObject);
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>() == null)
            HideShow(0);
        else
            HideShow(1);
    }

    public void HideShow(int show)
    {
        if (Grid == null)
            return;
        foreach (Transform child in Grid)
        {
            if (child == null)
                continue;
            if (show == 0)
                child.gameObject.SetActive(false);
            else
                child.gameObject.SetActive(true);
        }
    }
}
