using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameSceneBhv : SceneBhv
{
    protected Character _character;
    protected CharacterInstanceBhv _characterInstanceBhv;
    protected GameplayControler _gameplayControler;

    protected string _poppingText = "";
    protected GameObject _menu;

    protected override void Init()
    {
        base.Init();
        _gameplayControler = GetComponent<GameplayControler>();
        GameObject.Find(Constants.GoButtonPauseName).GetComponent<ButtonBhv>().EndActionDelegate = PauseOrPrevious;
        GameObject.Find(Constants.GoButtonInfoName).GetComponent<ButtonBhv>().EndActionDelegate = Info;
        GameObject.Find(Constants.GoCharacterInstance).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + PlayerPrefsHelper.GetSelectedCharacter());
        GetCharacter();
    }

    private void GetCharacter()
    {
        _character = _gameplayControler.Character;
        _characterInstanceBhv = GameObject.Find(Constants.GoCharacterInstance).GetComponent<CharacterInstanceBhv>();
    }

    public override void PauseOrPrevious()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _menu = Instantiator.NewPauseMenu(ResumeGiveUp, PlayerPrefsHelper.GetOrientation() == "Horizontal");
    }

    private void Info()
    {
        Paused = true;
        if (_character == null)
            GetCharacter();
        _musicControler.HalveVolume();
        _menu = Instantiator.NewInfoMenu(ResumeGiveUp, PlayerPrefsHelper.GetOrientation() == "Horizontal", _character, Constants.CurrentOpponent);
    }

    private object ResumeGiveUp(bool resume)
    {
        _musicControler.SetNewVolumeLevel();
        if (resume)
        {
            Paused = false;
            Constants.NameLastScene = SceneManager.GetActiveScene().name;
            Destroy(_menu);
            return true;
        }
        _menu.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        Camera.main.transform.position = new Vector3(0.0f, 0.0f, Camera.main.transform.position.z);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, true);
        if (GameObject.Find("PlayField") != null)
            Destroy(GameObject.Find("PlayField"));
        object OnBlend(bool result)
        {
            Constants.CurrentMusicType = MusicTyoe.Menu;
            NavigationService.LoadPreviousScene();
            return false;
        }
        return false;
    }

    public virtual void OnNewPiece()
    {

    }

    public virtual void OnPieceLocked(string pieceLetter)
    {

    }

    public virtual void OnSoftDrop()
    {

    }

    public virtual void OnHardDrop(int nbLines)
    {

    }

    public virtual void OnLinesCleared(int nbLines, bool isB2B)
    {

    }

    public virtual void OnPerfectClear()
    {

    }

    public virtual void OnCombo(int nbCombo)
    {

    }

    public virtual void PopText()
    {

    }
}
