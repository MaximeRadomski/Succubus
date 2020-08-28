using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class GameSceneBhv : SceneBhv
{
    public Character Character;
    protected CharacterInstanceBhv _characterInstanceBhv;
    protected Opponent _currentOpponent;
    protected List<Opponent> _opponents;
    protected CharacterInstanceBhv _opponentInstanceBhv;
    protected TMPro.TextMeshPro _opponentHp;

    protected GameplayControler _gameplayControler;
    protected string _poppingText = "";
    protected GameObject _menu;

    protected override void Init()
    {
        base.Init();
        _gameplayControler = GetComponent<GameplayControler>();
        GameObject.Find(Constants.GoButtonPauseName).GetComponent<ButtonBhv>().EndActionDelegate = PauseOrPrevious;
        GameObject.Find(Constants.GoButtonInfoName).GetComponent<ButtonBhv>().EndActionDelegate = Info;
        Character = CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()];
        _characterInstanceBhv = GameObject.Find(Constants.GoCharacterInstance).GetComponent<CharacterInstanceBhv>();
        _characterInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + Character.Id);
        _opponents = PlayerPrefsHelper.GetCurrentOpponents();
        if (_opponents != null && _opponents.Count > 0)
        {
            if (_opponents.Count == 1)
                GameObject.Find("Enemies").GetComponent<TMPro.TextMeshPro>().text = "enemy";
            for (int i = _opponents.Count; i < 12; ++i)
            {
                GameObject.Find("Opponent" + i).SetActive(false);
            }
            _opponentInstanceBhv = GameObject.Find(Constants.GoOpponentInstance).GetComponent<CharacterInstanceBhv>();
            _opponentHp = GameObject.Find("OpponentHP").GetComponent<TMPro.TextMeshPro>();
            NextOpponent();
        }
    }

    protected void NextOpponent()
    {
        _currentOpponent = _opponents[0];
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Opponents_" + _currentOpponent.Id);
        _opponentHp.text = _currentOpponent.HP.ToString();
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
        _musicControler.HalveVolume();
        _menu = Instantiator.NewInfoMenu(ResumeGiveUp, PlayerPrefsHelper.GetOrientation() == "Horizontal", Character, _currentOpponent);
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

    public virtual void OnCombo(int nbCombo)
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
}
