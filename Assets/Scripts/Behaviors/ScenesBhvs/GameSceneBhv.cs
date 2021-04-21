using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameSceneBhv : SceneBhv
{
    public Character Character;
    public Opponent CurrentOpponent;
    protected List<Opponent> _opponents;
    protected CharacterInstanceBhv _characterInstanceBhv;

    protected GameplayControler _gameplayControler;
    protected string _poppingText = "";
    protected GameObject _pauseMenu;
    protected GameObject _panelGame;

    public override MusicType MusicType => MusicType.Game;

    protected override void Init()
    {
        base.Init();
        _gameplayControler = GetComponent<GameplayControler>();
        GameObject.Find(Constants.GoButtonPauseName).GetComponent<ButtonBhv>().EndActionDelegate = PauseOrPrevious;
        GameObject.Find(Constants.GoButtonInfoName).GetComponent<ButtonBhv>().EndActionDelegate = Info;
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            Character = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        else
            Character = PlayerPrefsHelper.GetRunCharacter();
        _characterInstanceBhv = GameObject.Find(Constants.GoCharacterInstance).GetComponent<CharacterInstanceBhv>();
        _characterInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + Character.Id);
        _panelGame = GameObject.Find("PanelGame");
    }

    public override void PauseOrPrevious()
    {
        if (Paused)
            return;
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewPauseMenu(ResumeGiveUp, this, PlayerPrefsHelper.GetOrientation() == Direction.Horizontal);
    }

    public void Info()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewInfoMenu(ResumeGiveUp, Character, CurrentOpponent, PlayerPrefsHelper.GetOrientation() == Direction.Horizontal);
    }

    private object ResumeGiveUp(bool resume)
    {
        _musicControler.SetNewVolumeLevel();
        if (resume)
        {
            Paused = true;
            Constants.NameLastScene = SceneManager.GetActiveScene().name;
            Destroy(_pauseMenu);
            //var menuSelector = GameObject.Find(Constants.GoMenuSelector);
            //if (menuSelector != null)
            //    menuSelector.GetComponent<MenuSelectorBhv>().Reset(MenuSelectorBasePosition);
            Instantiator.New321(_panelGame.transform.position, () =>
                {
                    Paused = false;
                    return true;
                });
            return true;
        }
        _pauseMenu.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        CameraBhv.transform.position = new Vector3(0.0f, 0.0f, CameraBhv.transform.position.z);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, true);
        if (GameObject.Find("PlayField") != null)
            Destroy(GameObject.Find("PlayField"));
        object OnBlend(bool result)
        {
            if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
                NavigationService.LoadBackUntil(Constants.CharSelScene);
            else
            {
                PlayerPrefsHelper.SaveIsInFight(false);
                var run = PlayerPrefsHelper.GetRun();
                Constants.GameOverParams = $"Abandonment|{ run.CurrentRealm}|{run.RealmLevel}";
                PlayerPrefsHelper.ResetRun();
                NavigationService.LoadNextScene(Constants.GameOverScene);
            }                
            return false;
        }
        return false;
    }

    public virtual void OnNewPiece(GameObject lastPiece)
    {

    }

    public virtual void OnPieceLocked(string pieceLetter)
    {
        if (!string.IsNullOrEmpty(pieceLetter))
            _poppingText += pieceLetter + " twist";
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

    public virtual void DamageOpponent(int amount, GameObject source)
    {

    }

    public virtual void OnLinesCleared(int nbLines, bool isB2B)
    {
        if (_poppingText.Contains("twist"))
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
            _poppingText += " cc";
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
