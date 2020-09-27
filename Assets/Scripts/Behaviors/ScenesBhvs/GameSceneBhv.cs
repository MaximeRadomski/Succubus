using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameSceneBhv : SceneBhv
{
    public Character Character;
    protected List<Opponent> _opponents;
    protected Opponent _currentOpponent;
    protected CharacterInstanceBhv _characterInstanceBhv;

    protected GameplayControler _gameplayControler;
    protected string _poppingText = "";
    protected GameObject _pauseMenu;

    protected override void Init()
    {
        base.Init();
        _gameplayControler = GetComponent<GameplayControler>();
        GameObject.Find(Constants.GoButtonPauseName).GetComponent<ButtonBhv>().EndActionDelegate = PauseOrPrevious;
        GameObject.Find(Constants.GoButtonInfoName).GetComponent<ButtonBhv>().EndActionDelegate = Info;
        if (SceneManager.GetActiveScene().name.Contains("Training"))
            Character = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        else
            Character = PlayerPrefsHelper.GetRunCharacter();
        _characterInstanceBhv = GameObject.Find(Constants.GoCharacterInstance).GetComponent<CharacterInstanceBhv>();
        _characterInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + Character.Id);
    }

    public override void PauseOrPrevious()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewPauseMenu(ResumeGiveUp, PlayerPrefsHelper.GetOrientation() == "Horizontal");
    }

    private void Info()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewInfoMenu(ResumeGiveUp, PlayerPrefsHelper.GetOrientation() == "Horizontal", Character, _currentOpponent);
    }

    private object ResumeGiveUp(bool resume)
    {
        _musicControler.SetNewVolumeLevel();
        if (resume)
        {
            Paused = false;
            Constants.NameLastScene = SceneManager.GetActiveScene().name;
            Destroy(_pauseMenu);
            return true;
        }
        _pauseMenu.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, Camera.main.transform.position.z);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, true);
        if (GameObject.Find("PlayField") != null)
            Destroy(GameObject.Find("PlayField"));
        object OnBlend(bool result)
        {
            Constants.CurrentMusicType = MusicType.Menu;
            if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
                NavigationService.LoadBackUntil(Constants.CharSelScene);
            else
            {
                PlayerPrefsHelper.ResetRun();
                NavigationService.LoadBackUntil(Constants.MainMenuScene);
            }                
            return false;
        }
        return false;
    }

    public virtual void OnNewPiece()
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
            Instantiator.PopText(_poppingText, new Vector2(4.5f, 17.4f));
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
        Constants.CurrentMusicType = MusicType.Menu;
    }
}
