﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingGameSceneBhv : SceneBhv
{
    private GameplayControler _gameplayControler;

    private int _score;
    private int _level;
    private int _next;
    private int _lines;
    private int _pieces;

    private TMPro.TextMeshPro _scoreTmp;
    private TMPro.TextMeshPro _levelTmp;
    private TMPro.TextMeshPro _nextTmp;
    private TMPro.TextMeshPro _linesTmp;
    private TMPro.TextMeshPro _piecesTmp;

    private string _poppingText = "";

    void Start()
    {
        Init();
        _gameplayControler = GetComponent<GameplayControler>();

        _score = 0;
        _level = 1;
        _next = 10;
        _lines = 0;
        _pieces = 0;

        _scoreTmp = GameObject.Find("Score").GetComponent<TMPro.TextMeshPro>();
        _levelTmp = GameObject.Find("Level").GetComponent<TMPro.TextMeshPro>();
        _nextTmp = GameObject.Find("Next").GetComponent<TMPro.TextMeshPro>();
        _linesTmp = GameObject.Find("Lines").GetComponent<TMPro.TextMeshPro>();
        _piecesTmp = GameObject.Find("Pieces").GetComponent<TMPro.TextMeshPro>();

        DisplayScore();
        _levelTmp.text = _level.ToString();
        _nextTmp.text = _next.ToString();
        _linesTmp.text = _lines.ToString();
        _piecesTmp.text = _pieces.ToString();
    }

    public override void OnNewPiece()
    {
        if (_gameplayControler == null)
            return;
        if (_lines >= _level * Constants.LinesForLevel)
        {
            ++_level;
            _gameplayControler.SetGravity(_level);
        }
        _levelTmp.text = _level.ToString();
        _next = _level * Constants.LinesForLevel - _lines;
        _nextTmp.text = _next.ToString();
    }

    public override void OnPieceLocked(string pieceLetter)
    {
        ++_pieces;
        _piecesTmp.text = _pieces.ToString();
        if (!string.IsNullOrEmpty(pieceLetter))
            _poppingText += pieceLetter + " twist";
    }

    public override void OnSoftDrop()
    {
        _score += 1;
        DisplayScore();
    }

    public override void OnHardDrop(int nbLines)
    {
        _score += (2 * nbLines);
        DisplayScore();
    }

    public override void OnLinesCleared(int nbLines, bool isB2B)
    {
        var tmpAdded = 0;
        if (_poppingText.Contains("twist"))
        {
            if (nbLines == 0)
                tmpAdded = 400 * _level;
            else if (nbLines == 1)
                tmpAdded = 800 * _level;
            else if (nbLines == 2)
                tmpAdded = 1200 * _level;
            else if (nbLines == 3)
                tmpAdded = 1600 * _level;
            if (nbLines == 1)
                _poppingText += " single";
            else
                _poppingText += "\n";
        }
        else
        {
            if (nbLines == 1)
                tmpAdded = 100 * _level;
            else if (nbLines == 2)
                tmpAdded = 300 * _level;
            else if (nbLines == 3)
                tmpAdded = 500 * _level;
            else if (nbLines == 4)
                tmpAdded = 800 * _level;
        }
        if (nbLines > 1)
            _poppingText += nbLines + " lines";
        if (isB2B)
        {
            tmpAdded = (int)((float)tmpAdded * 1.5f);
            _poppingText += " cc";
        }

        _score += tmpAdded;
        DisplayScore();

        _lines += nbLines;
        _linesTmp.text = _lines.ToString();
    }

    public override void OnCombo(int nbCombo)
    {
        var tmpAdded = 50 * nbCombo * _level;
        _score += tmpAdded;
        DisplayScore();

        _poppingText += "\n*" + nbCombo + " combo";
    }

    private void DisplayScore()
    {
        if (_score <= 99999999)
            _scoreTmp.text = _score.ToString();
        else if (_score > 99999999 && _score <= 999999999)
        {
            var tmpScore = (_score / 1000).ToString();
            _scoreTmp.text = tmpScore.Insert(3, ".") + "m";
        }
        else
        {
            var tmpScore = (_score / 1000000).ToString();
            _scoreTmp.text = tmpScore.Insert(3, ".") + "b";
        }
    }

    public override void PopText()
    {
        if (!string.IsNullOrEmpty(_poppingText))
        {
            var y = 17.5f;
            if (_gameplayControler.CurrentPiece.transform.position.y > 17.0f)
                y = 15.0f;
            Instantiator.PopText(_poppingText, new Vector2(4.5f, y));
            _poppingText = "";
        }
    }
}