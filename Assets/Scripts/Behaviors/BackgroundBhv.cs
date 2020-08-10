using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundBhv : MonoBehaviour
{
    private Vector2 _positionToReach;
    private bool _isMoving;
    private Instantiator _instantiator;
    private int _i;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _positionToReach = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-2.0f, 2.0f));
        _instantiator = GameObject.Find(Constants.GoSceneBhvName).GetComponent<Instantiator>();
        _i = 0;
        _isMoving = true;
    }

    void Update()
    {
        if (_isMoving)
            Move();
        ++_i;
        if (_i >= 10)
        {
            _instantiator.NewFadeBlock(Realm.Hell, new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-18.0f, 18.0f), 0.0f), 2, -1);
            _i = 0;
        }
    }

    private void Move()
    {
        transform.position = Vector2.Lerp(transform.position, _positionToReach, 0.005f);
        if (Helper.VectorEqualsPrecision(transform.position, _positionToReach, 0.5f))
        {
            Init();
        }
    }
}
