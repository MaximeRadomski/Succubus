using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class RhythmIndicatorBhv : FrameRateBehavior
{
    private CharacterInstanceBhv _opponentInstance;
    private CharacterInstanceBhv _characterInstance;
    private float _delay;
    private int _currentId;
    private Color _color;
    private float _beatTime;
    private int _remainingMoves;
    private int _gravityBefore;

    private bool _isTilting;

    /// <summary>
    /// Beat in millisecond: 1000 = 1 second
    /// </summary>
    /// <param name="opponentInstance"></param>
    /// <param name="characterInstance"></param>
    /// <param name="nbMoves"></param>
    /// <param name="beat"></param>
    public void StartRhythm(CharacterInstanceBhv opponentInstance, CharacterInstanceBhv characterInstance, int nbMoves, int beat, Color color, int gravity)
    {
        _opponentInstance = opponentInstance;
        _characterInstance = characterInstance;
        _gravityBefore = gravity;
        _delay = beat / 1000.0f;
        if (_remainingMoves < 0)
            _remainingMoves = 0;
        _remainingMoves += nbMoves;
        _color = color;
        ++_currentId;
        StartCoroutine(Beat(_currentId));
    }

    private IEnumerator Beat(int id)
    {
        if (id == _currentId)
        {
            _beatTime = Time.time;
            if (_opponentInstance == null)
                _opponentInstance = GameObject.Find(Constants.GoSceneBhvName).GetComponent<ClassicGameSceneBhv>().OpponentInstanceBhv;
            if (_characterInstance == null)
                _characterInstance = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>().CharacterInstanceBhv;
            _opponentInstance.Dodge();
            _characterInstance.Dodge();
            Tilt();
            yield return new WaitForSeconds(_delay);
            StartCoroutine(Beat(id));
        }
    }

    private void Tilt()
    {
        foreach (Transform child in transform)
            child.GetComponent<SpriteRenderer>().color = _color;
        _isTilting = true;
    }

    protected override void FrameUpdate()
    {
        if (_isTilting)
        {
            var newOpacity = Mathf.Lerp(transform.GetChild(0).GetComponent<SpriteRenderer>().color.a, 0.0f, 0.05f);
            foreach (Transform child in transform)
                child.GetComponent<SpriteRenderer>().color = new Color(_color.r, _color.g, _color.b, newOpacity);
            if (Helper.FloatEqualsPrecision(newOpacity, 0.0f, 0.05f))
                _isTilting = false;
        }
    }

    public bool IsInBeat()
    {
        bool isInBeat = Helper.FloatEqualsPrecision(Time.time, _beatTime, 0.1f)
            || Helper.FloatEqualsPrecision(Time.time, _beatTime + _delay, 0.1f);
        if (isInBeat)
        {
            --_remainingMoves;
            if (_remainingMoves <= 0)
            {
                GameObject.Find(Constants.GoSceneBhvName).GetComponent<ClassicGameSceneBhv>().ResetToOpponentGravity();
                Destroy(gameObject);
            }
        }
        return isInBeat;
    }
}