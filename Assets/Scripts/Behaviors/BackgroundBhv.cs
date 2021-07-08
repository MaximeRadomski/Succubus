using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundBhv : FrameRateBehavior
{
    public bool BackgroundParticles = true;

    private Vector2 _positionToReach;
    private bool _isMoving;
    private Instantiator _instantiator;
    private int _i;
    private Camera _mainCamera;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        NewPositionToReach();
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        _i = 0;
        _isMoving = true;
        _mainCamera = Helper.GetMainCamera();
    }

    private void NewPositionToReach()
    {
        _positionToReach = new Vector2(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f));
    }

    protected override void FrameUpdate()
    {
        if (_isMoving)
            Move();
        if (BackgroundParticles)
        {
            ++_i;
            if (_i == 25)
                _instantiator.NewBackgroundLine(new Vector3(Random.Range(-10.0f, 10.0f) + _mainCamera.transform.position.x, Random.Range(-18.0f, 18.0f) + _mainCamera.transform.position.y, 0.0f), null);
            if (_i >= 50)
            {
                _instantiator.NewBackgroundPiece(new Vector3(Random.Range(-10.0f, 10.0f) + _mainCamera.transform.position.x, Random.Range(-18.0f, 18.0f) + _mainCamera.transform.position.y, 0.0f), null);
                _i = 0;
            }
        }
    }

    private void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach + (Vector2)_mainCamera.transform.position, 0.005f);
        if (Helper.VectorEqualsPrecision(transform.position, _positionToReach + (Vector2)_mainCamera.transform.position, 0.5f))
        {
            NewPositionToReach();
        }
    }
}
