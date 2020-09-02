using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayFieldBhv : MonoBehaviour
{
    public Transform[,] Grid;
    public SpriteRenderer _semiOpacitySpriteRenderer;

    private void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;
        var semiOpacityTransform = transform.Find("SemiOpacity");
        if (semiOpacityTransform != null)
            _semiOpacitySpriteRenderer = semiOpacityTransform.GetComponent<SpriteRenderer>();
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

    public void ShowSemiOpcaity(int show)
    {
        if (Grid == null || _semiOpacitySpriteRenderer == null)
            return;
        if (show == 0)
            _semiOpacitySpriteRenderer.enabled = false;
        else
            _semiOpacitySpriteRenderer.enabled = true;
        
    }
}
