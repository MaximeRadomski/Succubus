using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShiftBlockBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _directionText;
    private GameSceneBhv _gameScene;
    private Realm _realm;
    private int _nbRows;

    public void Init(int nbRows, Realm opponentRealm)
    {
        GetScene();
        _realm = opponentRealm;
        _nbRows = nbRows;
        var background = transform.Find("Background");
        background.localScale = new Vector3(background.localScale.x, background.localScale.y * _nbRows, 1.0f);
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Corner") || child.name.Contains("Mid"))
            {
                float floatHeight = (float)(_nbRows - 1);
                if (child.name.Contains("Top"))
                    child.transform.position += new Vector3(0.0f, floatHeight / 2, 0.0f);
                else if (child.name.Contains("Bot"))
                    child.transform.position += new Vector3(0.0f, -floatHeight / 2, 0.0f);
                child.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromRealm(_realm, 1);
            }
        }
        _directionText = transform.Find("Direction").GetComponent<TMPro.TextMeshPro>();
        _directionText.color = (Color)Constants.GetColorFromRealm(_realm, 2);
    }

    private void GetScene()
    {
        _gameScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameSceneBhv>();
    }
}
