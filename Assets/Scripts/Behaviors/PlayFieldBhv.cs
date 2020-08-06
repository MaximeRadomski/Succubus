using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFieldBhv : MonoBehaviour
{
    public Transform[,] Grid;

    private void Start()
    {
        //var tmp = GameObject.FindGameObjectsWithTag(Constants.TagPlayField);
        //if (tmp.Length > 1)
        //    Destroy(gameObject);
        //else
        //    DontDestroyOnLoad(gameObject);
    }

    public void HideShow(int param)
    {
        foreach (Transform child in Grid)
        {
            if (child == null)
                continue;
            var spriteRenderer = child.gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null && param == 0)
                spriteRenderer.enabled = false;
            else if (spriteRenderer != null)
                spriteRenderer.enabled = true;
        }
    }
}
