using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundBhv : MonoBehaviour
{
    private Vector2 _positionToReach;
    private bool _isMoving;
    private Instantiator _instantiator;
    private int _i;
    private bool _fadeBlocksMuted;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        NewPositionToReach();
        SceneManager.sceneLoaded += OnSceneLoaded;
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        _i = 0;
        _isMoving = true;
    }

    private void NewPositionToReach()
    {
        _positionToReach = new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
    }

    void Update()
    {
        if (_isMoving)
            Move();
        if (!!_fadeBlocksMuted)
        {
            ++_i;
            if (_i >= 10)
            {
                _instantiator.NewFadeBlock(Realm.Hell, new Vector3(Random.Range(-10.0f, 10.0f) + Camera.main.transform.position.x, Random.Range(-18.0f, 18.0f) + Camera.main.transform.position.y, 0.0f), 2, -1);
                _i = 0;
            }
        }
    }

    private void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach + (Vector2)Camera.main.transform.position, 0.005f);
        if (Helper.VectorEqualsPrecision(transform.position, _positionToReach + (Vector2)Camera.main.transform.position, 0.5f))
        {
            NewPositionToReach();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == Constants.StepsScene)
            _fadeBlocksMuted = true;
        else
            _fadeBlocksMuted = false;
    }
}
