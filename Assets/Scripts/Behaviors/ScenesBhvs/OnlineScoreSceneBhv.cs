using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineScoreSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Boss;

    private int _currentPage;
    private int _lastHighest;
    private List<HighScoreDto> _highScores;
    private int _range;

    private TMPro.TextMeshPro _page;
    private List<GameObject> _scoreGo;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _currentPage = 0;
        GameObject.Find("ButtonNext").GetComponent<ButtonBhv>().EndActionDelegate = () => GoToPage(_currentPage + 1);
        GameObject.Find("ButtonPrevious").GetComponent<ButtonBhv>().EndActionDelegate = () => GoToPage(_currentPage - 1);
        GameObject.Find("ButtonFindMe").GetComponent<ButtonBhv>().EndActionDelegate = FindMe;
        _page = GameObject.Find("Page").GetComponent<TMPro.TextMeshPro>();
        _highScores = new List<HighScoreDto>();
        _range = 10;
        _scoreGo = new List<GameObject>();
        for (int i = 0; i < 5; ++i)
            _scoreGo.Add(GameObject.Find($"OnlineScore{i}"));
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

    }

    private void UpdateList()
    {
        if (_highScores.Count <= (_currentPage * 5))
        {
            Instantiator.NewLoading();
            HighScoresService.GetHighScores(_lastHighest, _range, (list) =>
            {
                Helper.ResumeLoading();
                if (list == null || list.Count == 0)
                {
                    GoToPage(_currentPage - 1);
                    return;
                }
                foreach (var onlineScore in list)
                {
                    _highScores.Add(onlineScore);
                }
                _lastHighest = list[list.Count - 1].Score - 1;
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
        var maxPage = (_currentPage * 5) + 5;
        int i = 0;
        for (int h = _currentPage * 5; h < maxPage; ++h)
        {
            if (h < _highScores.Count)
            {
                _scoreGo[i].SetActive(true);
                _scoreGo[i].transform.Find("Position").GetComponent<TMPro.TextMeshPro>().text = $"{(_currentPage * 5) + h + 1}";
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
