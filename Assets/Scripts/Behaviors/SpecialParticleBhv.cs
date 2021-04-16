using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialParticleBhv : MonoBehaviour
{
    private GameObject _characterInstance;
    private GameObject _particlesFront;
    private GameObject _particlesBack;

    private float _maxVelocity = 1.8f;
    private Vector3 _leftPos;
    private Vector3 _rightPos;
    private float _velocity;

    private int _maxCycle = 4;
    private int _currentCycle;
    private bool _isCycling;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _characterInstance = transform.parent.gameObject;
        _particlesFront = transform.GetChild(0).gameObject;
        _particlesBack = transform.GetChild(1).gameObject;

        //_characterInstance.GetComponent<SpriteRenderer>().sortingOrder++;
        _particlesFront.GetComponent<ParticleSystemRenderer>().sortingLayerID = _characterInstance.GetComponent<SpriteRenderer>().sortingLayerID;
        _particlesFront.GetComponent<ParticleSystemRenderer>().sortingOrder = _characterInstance.GetComponent<SpriteRenderer>().sortingOrder + 1;
        _particlesBack.GetComponent<ParticleSystemRenderer>().sortingLayerID = _characterInstance.GetComponent<SpriteRenderer>().sortingLayerID;
        _particlesBack.GetComponent<ParticleSystemRenderer>().sortingOrder = _characterInstance.GetComponent<SpriteRenderer>().sortingOrder - 1;

        _leftPos = new Vector3(-_maxVelocity, -3f, 0.0f);
        _rightPos = new Vector3(_maxVelocity, -3f, 0.0f);
    }

    public void Activate(Realm realm)
    {
        var mainFront = _particlesFront.GetComponent<ParticleSystem>().main;
        mainFront.startColor = (Color)Constants.GetColorFromRealm(realm, 4);
        var mainBack = _particlesBack.GetComponent<ParticleSystem>().main;
        mainBack.startColor = (Color)Constants.GetColorFromRealm(realm, 3);
        _particlesFront.GetComponent<ParticleSystem>().Play();
        _particlesBack.GetComponent<ParticleSystem>().Play();
        _currentCycle = 0;
        NewCycle();
    }

    private void NewCycle()
    {
        ++_currentCycle;
        _velocity = 0.0f;
        _particlesFront.transform.localPosition = _leftPos;
        _particlesBack.transform.localPosition = _rightPos;
        _isCycling = true;
    }

    private void Deactivate()
    {
        _particlesFront.GetComponent<ParticleSystem>().Stop();
        _particlesBack.GetComponent<ParticleSystem>().Stop();
        _isCycling = false;
    }
    
    void Update()
    {
        if (_isCycling)
        {
            Cycle();
        }
    }

    private void Cycle()
    {
        _velocity = GetSpeed(_particlesFront.transform.localPosition.x);
        _particlesFront.transform.localPosition = PosFromSpeed(_particlesFront.transform.localPosition, 1.0f);
        _particlesBack.transform.localPosition = PosFromSpeed(_particlesBack.transform.localPosition, -1.0f);

        if (_particlesFront.transform.localPosition.x >= _rightPos.x)
        {
            if (_currentCycle >= _maxCycle)
                Deactivate();
            else
                NewCycle();
        }

    }

    private Vector3 PosFromSpeed(Vector3 pos, float mult)
    {
        var x = pos.x + ((_velocity < 0 ? 0 : _velocity) * mult);
        var y = pos.y; // _leftPos.y - (_speed * mult * 10);
        return new Vector3(x, y, pos.z);
    }

    private float GetSpeed(float x)
    {
        return (-(x * x) * 0.03f) + (Constants.Pixel * 1.1f);
    }
}
