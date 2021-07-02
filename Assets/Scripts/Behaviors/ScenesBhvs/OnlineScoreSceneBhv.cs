using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineScoreSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Boss;

    private int _currentPage;
    private List<int> _currentHighests;

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
        _scoreGo = new List<GameObject>();
        for (int i = 0; i < 5; ++i)
            _scoreGo.Add(GameObject.Find($"OnlineScore{i}"));
        _currentHighests = new List<int>() { 999999999 };
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
        UpdateList(comingFromNext);
    }

    private void FindMe()
    {

    }

    private void UpdateList(bool comingFromNext = false)
    {
        Instantiator.NewLoading();
        HighScoresService.GetHighScores(_currentHighests[_currentPage], (list) =>
        {
            Helper.ResumeLoading();
            if (list == null || list.Count == 0)
            {
                GoToPage(_currentPage - 1);
                return;
            }
            if (_currentPage >= _currentHighests.Count)
                _currentHighests.Add(list[list.Count].Score - 1);
            for (int i = 0; i < 5; ++i)
            {
                if (i <= list.Count)
                {
                    _scoreGo[i].SetActive(true);
                    _scoreGo[i].transform.Find("Position").GetComponent<TMPro.TextMeshPro>().text = $"{(_currentPage * 5) + i + 1}";
                    _scoreGo[i].transform.Find("Character00").GetChild(1).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + list[i].CharacterId);
                    _scoreGo[i].transform.Find("Name").GetComponent<TMPro.TextMeshPro>().text = $"{list[i].PlayerName}";
                    _scoreGo[i].transform.Find("Score").GetComponent<TMPro.TextMeshPro>().text = $"{list[i].Score.ToSpacedIntString()}";
                    _scoreGo[i].transform.Find("Level").GetComponent<TMPro.TextMeshPro>().text = $"{list[i].Level}";
                    _scoreGo[i].transform.Find("Lines").GetComponent<TMPro.TextMeshPro>().text = $"{list[i].Lines}";
                    _scoreGo[i].transform.Find("Pieces").GetComponent<TMPro.TextMeshPro>().text = $"{list[i].Pieces}";

                }
                else
                    _scoreGo[i].SetActive(false);
            }
        });
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
