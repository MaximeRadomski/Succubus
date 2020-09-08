using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VisionBlockBhv : MonoBehaviour
{
    private TMPro.TextMeshPro _secondsText;
    private IconInstanceBhv _secondsTextIconInstance;
    private bool _isInGameScene;
    private int _cooldown;
    private float _nextTick;
    private GameSceneBhv _gameScene;
    private bool _hasPausedOnce;
    private Realm _realm;
    private int _nbRows;

    public void Init(int nbRows, int nbSeconds, Realm opponentRealm)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GetScene();
        _realm = opponentRealm;
        _nbRows = nbRows;
        _hasPausedOnce = false;
        var background = transform.Find("Background");
        background.localScale = new Vector3(background.localScale.x, background.localScale.y * _nbRows, 1.0f);
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Corner") || child.name.Contains("Mid"))
            {
                float floatHeight = (float)(_nbRows - 1);
                if (child.name.Contains("Top"))
                    child.transform.position += new Vector3(0.0f, floatHeight / 2 , 0.0f);
                else if (child.name.Contains("Bot"))
                    child.transform.position += new Vector3(0.0f, -floatHeight / 2, 0.0f);
                child.GetComponent<SpriteRenderer>().color = (Color)Constants.GetColorFromNature(_realm, 1);
            }
        }
        _secondsText = transform.Find("Seconds").GetComponent<TMPro.TextMeshPro>();
        _secondsTextIconInstance = _secondsText.GetComponent<IconInstanceBhv>();
        _secondsText.color = (Color)Constants.GetColorFromNature(_realm, 2);
        transform.Find("Scores").GetComponent<TMPro.TextMeshPro>().color = _secondsText.color;
        _cooldown = nbSeconds;
        UpdateTextAndSetNextTick();
        _isInGameScene = true;
    }

    private void GetScene()
    {
        _gameScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameSceneBhv>();
    }

    void Update()
    {
        if (_gameScene == null)
            GetScene();
        if (_isInGameScene && _gameScene != null && !_gameScene.Paused)
        {
            CheckNextTick();
        }
        else
        {
            _hasPausedOnce = true;
        }
    }

    private void CheckNextTick()
    {
        if (_hasPausedOnce)
        {
            _nextTick = Time.time + 1.0f;
            _hasPausedOnce = false;
            return;
        }
        if (Time.time >= _nextTick)
        {
            DecreaseCooldown(1);
        }
    }


    private void End()
    {
        //int xRounded = Mathf.RoundToInt(transform.Find("BotLeftCorner").position.x);
        if (_gameScene == null)
            GetScene();
        int yRounded = Mathf.RoundToInt(transform.Find("BotLeftCorner").position.y);
        int yMax = yRounded + _nbRows;
        for (int y = yRounded; y < yMax; ++y)
        {
            for (int x = 0; x < 10; ++x)
            {
                _gameScene.Instantiator.NewFadeBlock(_realm, new Vector3(x, y, 0.0f), 5, 0);
            }
        }
        Destroy(gameObject);
    }

    private void UpdateTextAndSetNextTick()
    {
        _secondsText.text = _cooldown.ToString();
        _nextTick = Time.time + 1.0f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.Contains("Game"))
            _isInGameScene = true;
        else
            _isInGameScene = false;
    }

    public void DecreaseCooldown(int amount, bool pop = false)
    {
        if (pop)
            _secondsTextIconInstance.Pop();
        _cooldown -= amount;
        if (_cooldown <= 0)
            End();
        else
            UpdateTextAndSetNextTick();
    }
}
