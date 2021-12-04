using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class RhythmIndicatorBhv : FrameRateBehavior
{
    private SoundControlerBhv _soundControler;
    private int _idBeat;

    private CharacterInstanceBhv _opponentInstance;
    private CharacterInstanceBhv _characterInstance;
    private GameplayControler _gameplayControler;
    private float _delay;
    private int _currentId;
    private Color _color;
    private Realm _realm;
    private float _beatTime;
    private int _remainingMoves;
    private int _nbEmptyRowsOnMiss;

    private bool _isTilting;
    private int _idTilt;
    private bool _hasDoneActionOnBeat;
    private bool _hasDoneActionOnNextBeat;

    /// <summary>
    /// Beat in millisecond: 1000 = 1 second
    /// </summary>
    /// <param name="opponentInstance"></param>
    /// <param name="characterInstance"></param>
    /// <param name="nbMoves"></param>
    /// <param name="beat"></param>
    public void StartRhythm(CharacterInstanceBhv opponentInstance, CharacterInstanceBhv characterInstance, int nbMoves, int beat, Color color, int nbEmptyRowsOnMiss, Realm realm)
    {
        _opponentInstance = opponentInstance;
        _characterInstance = characterInstance;
        _nbEmptyRowsOnMiss = nbEmptyRowsOnMiss;
        _realm = realm;
        _delay = beat / 1000.0f;
        if (_remainingMoves < 0)
            _remainingMoves = 0;
        _remainingMoves += nbMoves;
        _color = color;
        ++_currentId;
        StartCoroutine(Beat(_currentId));
    }

    public void UnpauseBeat()
    {
        StartCoroutine(Beat(_currentId));
    }

    private IEnumerator Beat(int id)
    {
        if (id == _currentId)
        {
            if (_hasDoneActionOnNextBeat)
            {
                _hasDoneActionOnBeat = true;
                _hasDoneActionOnNextBeat = false;
            }
            else
                _hasDoneActionOnBeat = false;
            _beatTime = Time.time;
            if (_opponentInstance == null)
                _opponentInstance = GameObject.Find(Constants.GoSceneBhvName).GetComponent<ClassicGameSceneBhv>().OpponentInstanceBhv;
            if (_characterInstance == null)
                _characterInstance = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>().CharacterInstanceBhv;
            _opponentInstance.Dodge();
            _characterInstance.Dodge();
            Tilt();
            if (_soundControler == null)
            {
                _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
                _idBeat = _soundControler.SetSound("Beat");
            }
            _soundControler.PlaySound(_idBeat, customRate: _idTilt == 3 ? 1.2f : 1.0f);
            ++_idTilt;
            if (_idTilt >= 4)
                _idTilt = 0;
            yield return new WaitForSeconds(_delay);
            if (_gameplayControler == null)
                _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
            if (!_gameplayControler.SceneBhv.Paused)
                StartCoroutine(Beat(id));
        }
    }

    private void Tilt()
    {
        ApplyColor(_color);
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

    public void ApplyColor(Color color)
    {
        foreach (Transform child in transform)
            child.GetComponent<SpriteRenderer>().color = color;
    }

    public bool IsInBeat(bool exactBeat)
    {
        bool isInBeat;
        bool isInCurrentBeat;
        bool isInNextBeat;
        isInCurrentBeat = !_hasDoneActionOnBeat && Helper.FloatEqualsPrecision(Time.time, _beatTime, exactBeat ? 0.01f : 0.15f);
        isInNextBeat = !_hasDoneActionOnNextBeat && Helper.FloatEqualsPrecision(Time.time, _beatTime + _delay, exactBeat ? 0.01f : 0.15f);
        isInBeat = isInCurrentBeat || isInNextBeat;
        if (isInBeat)
        {
            if (isInCurrentBeat)
                _hasDoneActionOnBeat = true;
            else
                _hasDoneActionOnNextBeat = true;
            if (!exactBeat)
            {
                DecrementRemainingMovesAndCheck();
            }
        }
        else if (!exactBeat)
        {
            if (_gameplayControler == null)
                _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
            if (_opponentInstance == null)
                _opponentInstance = GameObject.Find(Constants.GoSceneBhvName).GetComponent<ClassicGameSceneBhv>().OpponentInstanceBhv;
            _gameplayControler.AttackEmptyRows(_opponentInstance.gameObject, _nbEmptyRowsOnMiss, _realm);
            _gameplayControler.DropGhost();
        }
        return isInBeat;
    }

    public void DecrementRemainingMovesAndCheck()
    {
        --_remainingMoves;
        if (_remainingMoves <= 0)
        {
            GameObject.Find(Constants.GoSceneBhvName).GetComponent<ClassicGameSceneBhv>().ResetToOpponentGravity();
            GameObject.Find(Constants.GoMusicControler)?.GetComponent<MusicControlerBhv>().SetNewVolumeLevel();
            Destroy(gameObject);
        }
    }
}