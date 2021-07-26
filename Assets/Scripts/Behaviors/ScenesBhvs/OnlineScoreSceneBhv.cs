using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineScoreSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Boss;

    private int _currentPage;
    private int _lastHighest;
    private List<HighScoreDto> _highScores;
    private int _range = 50;
    private int _bigRange = 200;
    private int _listItemDisplay = 5;
    private AccountDto _account;
    private bool _reachedEndOfOnlineScores;
    private bool _isOldSchool = false;

    private TMPro.TextMeshPro _page;
    private List<GameObject> _scoreGo;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _isOldSchool = NavigationService.NextSceneParameter?.BoolParam1 == true;
        _currentPage = 0;
        GameObject.Find("ButtonNext").GetComponent<ButtonBhv>().EndActionDelegate = () => GoToPage(_currentPage + 1);
        GameObject.Find("ButtonPrevious").GetComponent<ButtonBhv>().EndActionDelegate = () => GoToPage(_currentPage - 1);
        GameObject.Find("ButtonFindMe").GetComponent<ButtonBhv>().EndActionDelegate = FindMe;
        _page = GameObject.Find("Page").GetComponent<TMPro.TextMeshPro>();
        _highScores = new List<HighScoreDto>();
        _scoreGo = new List<GameObject>();
        _reachedEndOfOnlineScores = false;
        for (int i = 0; i < _listItemDisplay; ++i)
            _scoreGo.Add(GameObject.Find($"OnlineScore{i}"));
        _scoreGo[0].GetComponent<ButtonBhv>().EndActionDelegate = () => ViewScoreDetails(_currentPage * _listItemDisplay + 0);
        _scoreGo[1].GetComponent<ButtonBhv>().EndActionDelegate = () => ViewScoreDetails(_currentPage * _listItemDisplay + 1);
        _scoreGo[2].GetComponent<ButtonBhv>().EndActionDelegate = () => ViewScoreDetails(_currentPage * _listItemDisplay + 2);
        _scoreGo[3].GetComponent<ButtonBhv>().EndActionDelegate = () => ViewScoreDetails(_currentPage * _listItemDisplay + 3);
        _scoreGo[4].GetComponent<ButtonBhv>().EndActionDelegate = () => ViewScoreDetails(_currentPage * _listItemDisplay + 4);
        _lastHighest = 999999999;
        GoToPage(_currentPage);
        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = () => GoToPrevious(true);
    }

    private void GoToPage(int i)
    {
        if (i < 0)
            i = 0;
        _page.text = $"Page {i + 1}";
        var comingFromNext = i > _currentPage;
        _currentPage = i;
        UpdateList();
    }

    private void FindMe()
    {
        if (_account == null)
        {
            AccountService.CheckAccount(this.Instantiator, (account) =>
            {
                if (account == null)
                {
                    Instantiator.NewPopupYesNo("Error", "you are not logged in.", null, "Ok", null);
                    return;
                }
                else _account = account;
                IfAccountValidated();
            });
        }
        else
            IfAccountValidated();

        void IfAccountValidated()
        {
            Helper.ResumeLoading();
            var myScore = _highScores.Find(s => s.PlayerName == _account.PlayerName);
            if (myScore != null)
                GoToPage(_highScores.IndexOf(myScore) / _listItemDisplay);
            else if (_reachedEndOfOnlineScores == false)
            {
                Instantiator.NewLoading();
                HighScoresService.GetHighScores(_lastHighest, _bigRange, _isOldSchool, (list) =>
                {
                    if (list == null || list.Count == 0)
                        _reachedEndOfOnlineScores = true;
                    else
                        FillScores(list);
                    FindMe();
                });
            }
            else
                Instantiator.NewPopupYesNo("Error", "couldn't find your score", null, "Ok", null);
        }
    }

    private void UpdateList()
    {
        if (_highScores.Count <= (_currentPage * _listItemDisplay))
        {
            Instantiator.NewLoading();
            HighScoresService.GetHighScores(_lastHighest, _range < _listItemDisplay ? _listItemDisplay : _range, _isOldSchool, (list) =>
            {
                Helper.ResumeLoading();
                if (list == null || list.Count == 0)
                {
                    _reachedEndOfOnlineScores = true;
                    GoToPage(_currentPage - 1);
                    return;
                }
                FillScores(list);
                DisplayList();
            });
        }
        else
        {
            DisplayList();
        }
        
    }

    private void DisplayList()
    {
        var maxPage = (_currentPage * _listItemDisplay) + _listItemDisplay;
        int i = 0;
        for (int h = _currentPage * _listItemDisplay; h < maxPage; ++h)
        {
            if (h < _highScores.Count)
            {
                _scoreGo[i].SetActive(true);
                _scoreGo[i].transform.Find("Position").GetComponent<TMPro.TextMeshPro>().text = $"{h + 1}";
                _scoreGo[i].transform.Find("Character00").GetChild(1).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _highScores[h].CharacterId);
                _scoreGo[i].transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = $"{_highScores[h].PlayerName}";
                _scoreGo[i].transform.Find("Score").GetComponent<TMPro.TextMeshPro>().text = $"{_highScores[h].Score.ToSpacedIntString()}";
                _scoreGo[i].transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = $"{_highScores[h].Level}";
                _scoreGo[i].transform.Find("Lines").GetComponent<TMPro.TextMeshPro>().text = $"{_highScores[h].Lines}";
                _scoreGo[i].transform.Find("Pieces").GetComponent<TMPro.TextMeshPro>().text = $"{_highScores[h].Pieces}";

            }
            else
                _scoreGo[i].SetActive(false);
            ++i;
        }
    }

    private void FillScores(List<HighScoreDto> list)
    {
        foreach (var onlineScore in list)
        {
            _highScores.Add(onlineScore);
        }
        _lastHighest = list[list.Count - 1].Score - 1;
    }

    private void ViewScoreDetails(int id)
    {
        Instantiator.NewPopupYesNo("Score", $"score: {_highScores[id].Score}\nlevel: {_highScores[id].Level}\n lines: {_highScores[id].Lines}\n pieces: {_highScores[id].Pieces}", null, "Ok", null);
    }

    private object GoToPrevious(bool result)
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
        return true;
    }
}
