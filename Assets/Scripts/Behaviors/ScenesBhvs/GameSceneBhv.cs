﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Networking.UnityWebRequest;

public abstract class GameSceneBhv : SceneBhv
{
    public Character Character;
    public Opponent CurrentOpponent;

    protected List<Opponent> _opponents;
    protected CharacterInstanceBhv _characterInstanceBhv;

    protected GameplayControler _gameplayControler;
    protected GameObject _pauseMenu;
    protected GameObject _panelGame;
    protected string _poppingText = "";

    public override MusicType MusicType => MusicType.Game;

    protected override void Init()
    {
        base.Init();
        _gameplayControler = GetComponent<GameplayControler>();
        GameObject.Find(Constants.GoButtonPauseName).GetComponent<ButtonBhv>().EndActionDelegate = PauseOrPrevious;
        GameObject.Find(Constants.GoButtonInfoName).GetComponent<ButtonBhv>().EndActionDelegate = Info;
        if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
        {
            Character = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
            Character.SkinId = PlayerPrefsHelper.GetSelectedSkinId();
        }
        else if (Cache.CurrentGameMode == GameMode.TrainingOldSchool)
            Character = CharactersData.CustomCharacters[0];
        else
            Character = PlayerPrefsHelper.GetRunCharacter();
        Character.BoostAttack = 0;
        _characterInstanceBhv = GameObject.Find(Constants.GoCharacterInstance).GetComponent<CharacterInstanceBhv>();
        if (Cache.CurrentGameMode == GameMode.TrainingOldSchool)
            _characterInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{Character.Kind}");
        else
            _characterInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetCharacterSkin(Character.Id, Character.SkinId);
        _panelGame = GameObject.Find("PanelGame");
    }

    public void TemporaryCharacter(Character character)
    {
        Cache.TemporaryCharacter = character;
        _characterInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetCharacterSkin(character.Id, 0);
        _gameplayControler.TemporaryCharacter(character);
    }

    public override void PauseOrPrevious()
    {
        if (Paused)
            return;
        Paused = true;
        CameraBhv.Paused = true;
        _musicControler.HalveVolume();
        Cache.OnResumeLastPiecePosition = _gameplayControler.CurrentPiece.transform.position;
        Cache.OnResumeLastPieceRotation = _gameplayControler.CurrentPiece.transform.rotation;
        if (_gameplayControler.CurrentPiece.transform.childCount > 4)
            Cache.OnResumeLastForcedBlocks = _gameplayControler.CurrentPiece.transform.childCount - 4;
        _pauseMenu = Instantiator.NewPauseMenu(ResumeGiveUp, this, PlayerPrefsHelper.GetOrientation() == Direction.Horizontal && PlayerPrefsHelper.GetGameplayChoice() == GameplayChoice.Buttons);
    }

    public void Info()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewInfoMenu(ResumeGiveUp, Character, CurrentOpponent, PlayerPrefsHelper.GetOrientation() == Direction.Horizontal && PlayerPrefsHelper.GetGameplayChoice() == GameplayChoice.Buttons);
    }

    private void ResumeGiveUp(bool resume)
    {
        _musicControler.SetNewVolumeLevel();
        CameraBhv.Paused = false;
        if (resume)
        {
            _gameplayControler.GameplayOnHold = true;
            Cache.NameLastScene = SceneManager.GetActiveScene().name;
            Destroy(_pauseMenu);
            //var menuSelector = GameObject.Find(Constants.GoMenuSelector);
            //if (menuSelector != null)
            //    menuSelector.GetComponent<MenuSelectorBhv>().Reset(MenuSelectorBasePosition);
            Instantiator.New321(_panelGame.transform.position, () =>
                {
                    _gameplayControler.GameplayOnHold = false;
                    Paused = false;
                    var rhythmIndicator = GameObject.Find(Constants.GoRhythmIndicator)?.GetComponent<RhythmIndicatorBhv>() ?? null;
                    if (rhythmIndicator != null)
                        rhythmIndicator.UnpauseBeat();
                });
            return;
        }
        _pauseMenu.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        CameraBhv.transform.position = new Vector3(0.0f, 0.0f, CameraBhv.transform.position.z);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, true);
        if (GameObject.Find("PlayField") != null)
            Destroy(GameObject.Find("PlayField"));
        bool OnBlend(bool result)
        {
            if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
                NavigationService.LoadBackUntil(Constants.CharSelScene);
            else if (Cache.CurrentGameMode == GameMode.TrainingOldSchool)
                NavigationService.LoadBackUntil(Constants.TrainingChoiceScene);
            else
            {
                Cache.CurrentBossId = 0;
                PlayerPrefsHelper.SaveIsInFight(false);
                var run = PlayerPrefsHelper.GetRun();
                Cache.GameOverParams = $"Abandonment|{ run.CurrentRealm}|{run.RealmLevel}";
                if (run.CharacterEncounterAvailability)
                    PlayerPrefsHelper.IncrementNumberRunWithoutCharacterEncounter();
                PlayerPrefsHelper.ResetRun();
                NavigationService.LoadNextScene(Constants.GameOverScene);
            }                
            return false;
        }
    }

    public virtual void OnNewPiece(GameObject lastPiece)
    {

    }

    public virtual void OnPieceLocked(string pieceLetterTwist)
    {
        if (!string.IsNullOrEmpty(pieceLetterTwist))
            _poppingText += pieceLetterTwist + " twist";
    }

    public virtual void OnSoftDropStomp(int linesStomped)
    {

    }

    public virtual void OnSoftDrop()
    {

    }

    public virtual void OnHardDrop(int nbLines)
    {

    }

    public virtual void StunOpponent(int seconds)
    {

    }

    public virtual bool DamageOpponent(int amount, GameObject source, Realm? textRealm = null, bool attackLine = true)
    {
        return false;
    }

    public virtual void OnLinesCleared(int nbLines, bool isB2B, bool lastLockIsTwist)
    {
        if (lastLockIsTwist)
        {
            if (nbLines == 1)
                _poppingText += " single";
            else
                _poppingText += "\n";
            _characterInstanceBhv.Attack();
        }
        else if (nbLines > 0)
        {
            _characterInstanceBhv.Attack();
        }
        if (nbLines > 1)
            _poppingText += nbLines + " lines";
        if (isB2B)
        {
            _poppingText += "\nconsecutive";
        }
    }

    public virtual void OnPerfectClear()
    {
        if (_poppingText.Length > 0)
            _poppingText += "\n";
        _poppingText += "<b>perfect clear!</b>";
    }

    public virtual void OnCombo(int nbCombo, int nbLines)
    {
        _poppingText += "\n*" + nbCombo + " combo";
    }

    public virtual void PopText()
    {
        if (!string.IsNullOrEmpty(_poppingText))
        {
            var x = _gameplayControler.CurrentPiece.transform.position.x;
            var y = _gameplayControler.CurrentPiece.transform.position.y;
            Instantiator.PopText(_poppingText, new Vector2(((x - 5f) / 1.5f) + 5f, y), distance: 2.0f, startFadingDistancePercent: 0.6f, fadingSpeed: 0.04f);
            _poppingText = "";
            _gameplayControler.FadeBlocksOnText();
        }
    }

    public virtual bool OpponentAttack()
    {
        return true;
    }

    public virtual void OnGameOver()
    {
        PlayerPrefsHelper.SaveIsInFight(false);
    }
}
