using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingFreeGameSceneBhv : GameSceneBhv
{
    private int _score;
    private int _level;
    private int _next;
    private int _lines;
    private int _boostedLines;
    private int _pieces;

    private List<float> _verif;
    /* _verif
     * _verif[0] = soft drops * 8
     * _verif[1] = lines / 3
     * _verif[2] = combos * 4
     * _verif[3] = perfect clears / 20
     * _verif[4] = levels * 4
     */

    private TMPro.TextMeshPro _scoreTmp;
    private TMPro.TextMeshPro _levelTmp;
    private TMPro.TextMeshPro _nextTmp;
    private TMPro.TextMeshPro _linesTmp;
    private TMPro.TextMeshPro _piecesTmp;

    private SoundControlerBhv _soundControler;
    private int _levelUp;
    private bool _isRestarting = false;

    public override MusicType MusicType => MusicType.Game;

    void Start()
    {
        Init();
        GameObject.Find(Constants.GoButtonInfoName).GetComponent<ButtonBhv>().EndActionDelegate = AskRestartTraining;
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _levelUp = _soundControler.SetSound("LevelUp");

        var results = PlayerPrefsHelper.GetTraining();
        _verif = PlayerPrefsHelper.GetVerif();
        _score = results[0];
        _level = results[1];
        _lines = results[2];
        _pieces = results[3];
        _next = Constants.LinesForLevel - _lines % 10;

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

        CurrentOpponent = null;
        if (Cache.TrainingFreeSelectedLevel == 0)
        {
            this.Instantiator.NewPopupTrainingFreeLevel((int selectedLevel) =>
            {
                SetLevelAndStart(selectedLevel);
            });
        }
        else
            SetLevelAndStart(Cache.TrainingFreeSelectedLevel);
    }

    void SetLevelAndStart(int selectedLevel)
    {
        _level = selectedLevel;
        _boostedLines = (_level - 1) * 10;
        Cache.TrainingFreeSelectedLevel = selectedLevel;
        _levelTmp.text = _level.ToString();
        this.InvokeNextFrame(() =>
        {
            _gameplayControler.StartGameplay(_level, Character?.Realm ?? Realm.Hell, Realm.Hell);
        });
    }

    public override void OnGameOver()
    {
        var verif = (_verif[0] / 8.0f) + (_verif[1] * 3.0f) + (_verif[2] / 4.0f) + (_verif[3] * 20.0f);
        Cache.CurrentHighScoreContext = new List<int>() {_score, _level, _lines, _pieces, Character.Id, (int)verif, (int)(_verif[4] / 4) };
        //_score = 0;
        //_level = 1;
        //_lines = 0;
        //_pieces = 0;
        NavigationService.LoadNextScene(Constants.HighScoreScene, new NavigationParameter() { BoolParam0 = Cache.CurrentGameMode == GameMode.TrainingOldSchool });
        //Reload();
    }

    private void OnDestroy()
    {
        if (!_isRestarting)
            PlayerPrefsHelper.SaveTraining(_score, _level, _lines, _pieces, _verif);
    }

    public void AskRestartTraining()
    {
        if (!_gameplayControler.CanBeReload)
            return;
        _gameplayControler.GameplayOnHold = true;
        this.Instantiator.NewPopupYesNo("Restart", "would you like to restart this game?", "No", "Yes", RestartTraining);
    }

    private void RestartTraining(bool result)
    {
        _gameplayControler.GameplayOnHold = false;
        if (!result)
            return;
        _isRestarting = true;
        PlayerPrefsHelper.ResetTraining();
        PlayerPrefsHelper.SaveCurrentOpponents(null);
        Cache.ResetClassicGameCache();
        Cache.CurrentItemCooldown = 0;
        NavigationService.ReloadScene();
        _gameplayControler.CleanPlayerPrefs();
    }

    public override void OnNewPiece(GameObject lastPiece)
    {
        if (_gameplayControler == null)
            return;
        if (_lines + _boostedLines >= _level * Constants.LinesForLevel)
        {
            ++_level;
            _verif[4] += 4;
            _gameplayControler.SetGravity(_level);
            _soundControler.PlaySound(_levelUp);
            var maxHeight = 15.0f;
            var highestBlockY = _gameplayControler.GetHighestBlock();
            if (maxHeight > highestBlockY + 3)
                maxHeight = highestBlockY + 4;
            Instantiator.PopText("L   level  up   J", new Vector2(4.5f, maxHeight));
            CameraBhv.Bump(4);
        }
        _levelTmp.text = _level.ToString();
        _next = _level * Constants.LinesForLevel - _lines - _boostedLines;
        _nextTmp.text = _next.ToString();
    }

    public override void OnPieceLocked(string pieceLetter)
    {
        ++_pieces;
        _piecesTmp.text = _pieces.ToString();
        base.OnPieceLocked(pieceLetter);
    }

    public override void OnSoftDropStomp(int linesStomped)
    {
        var toAdd = 1 * linesStomped;
        _score += toAdd;
        _verif[0] += toAdd * 8;
        DisplayScore();
    }

    public override void OnSoftDrop()
    {
        var toAdd = 1;
        _score += toAdd;
        _verif[0] += toAdd * 8;
        DisplayScore();
    }

    public override void OnHardDrop(int nbLines)
    {
        var toAdd = 2 * nbLines;
        _score += toAdd;
        _verif[0] += toAdd * 8;
        DisplayScore();
    }

    public override void OnLinesCleared(int nbLines, bool isB2B, bool lastLockIsTwist)
    {
        var tmpAdded = 0;
        if (lastLockIsTwist)
        {
            if (nbLines == 0)
                tmpAdded = 400 * _level;
            else if (nbLines == 1)
                tmpAdded = 800 * _level;
            else if (nbLines == 2)
                tmpAdded = 1200 * _level;
            else if (nbLines == 3)
                tmpAdded = 1600 * _level;
            else if (nbLines == 4)
                tmpAdded = 2000 * _level;
            if (nbLines == 1)
                _poppingText += " single";
            else
                _poppingText += "\n";
            _characterInstanceBhv.Attack();
        }
        else if (nbLines > 0)
        {
            CameraBhv.Bump(4);
            if (nbLines == 1)
                tmpAdded = 100 * _level;
            else if (nbLines == 2)
                tmpAdded = 300 * _level;
            else if (nbLines == 3)
                tmpAdded = 500 * _level;
            else if (nbLines == 4)
                tmpAdded = 800 * _level;
            _characterInstanceBhv.Attack();
        }
        if (nbLines > 1)
            _poppingText += nbLines + " lines";
        if (isB2B)
        {
            tmpAdded = Mathf.RoundToInt((float)tmpAdded * 1.5f);
            _poppingText += "\nconsecutive";
        }

        if (lastLockIsTwist)
        {
            if (_gameplayControler.CharacterRealm == Realm.Heaven)
            {
                Cache.SelectedCharacterSpecialCooldown -= Character.RealmPassiveEffect;
                Cache.CurrentItemCooldown -= Character.RealmPassiveEffect;
                _gameplayControler.UpdateItemAndSpecialVisuals();
            }
        }
        if (_gameplayControler.CharacterRealm == Realm.Earth && nbLines == 4)
        {
            int linesDestroyed = Character.RealmPassiveEffect;
            linesDestroyed -= _gameplayControler.CheckForDarkRows(linesDestroyed);
            if (linesDestroyed > 0)
                linesDestroyed -= _gameplayControler.CheckForWasteRows(linesDestroyed);
            if (linesDestroyed > 0)
            {
                for (int i = 0; i < linesDestroyed; ++i)
                    _gameplayControler.CheckForLightRows();
            }

        }

        _score += tmpAdded;
        _verif[1] += tmpAdded / 3;
        DisplayScore();

        _lines += nbLines;
        _linesTmp.text = _lines.ToString();
    }

    public override void OnCombo(int nbCombo, int nbLines)
    {
        var tmpAdded = 50 * nbCombo * _level;
        _score += tmpAdded;
        _verif[2] += tmpAdded * 4;
        DisplayScore();

        _poppingText += "\n*" + nbCombo + " combo";
    }

    private void DisplayScore()
    {
        if (_score <= 999999999)
            _scoreTmp.text = _score.ToString();
        else if (_score > 999999999 && _score <= 999999999)
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

    public override void OnPerfectClear()
    {
        if (_poppingText.Length > 0)
            _poppingText += "\n";
        _poppingText += "<b>perfect clear!</b>";
        var toAdd = 4000 * _level;
        _score += toAdd;
        _verif[3] += toAdd / 20;
        DisplayScore();
    }
}
