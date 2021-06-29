using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighScoreSceneBhv : SceneBhv
{
    public Sprite HighLightedBar;

    private List<int> _scoreHistory;
    private TMPro.TextMeshPro _title;
    private GameObject _scoreHistoryContainer;
    private ResourceBarBhv _tiltingBar;

    public bool AlreadyTrySendHighScoreOnThisInstance = false;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_tiltingBar != null)
            _tiltingBar.Tilt();
    }

    protected override void Init()
    {
        base.Init();    
        SetButtons();
        _scoreHistory = PlayerPrefsHelper.GetTrainingHighScoreHistory();
        _title = GameObject.Find("Title").GetComponent<TMPro.TextMeshPro>();
        _scoreHistoryContainer = GameObject.Find("ScoreHistoryContainer");
        var scoreValueStr = "";
        var encryptedScore = "";
        if (Constants.CurrentHighScoreContext != null)
        {
            if (!Helper.FloatEqualsPrecision(Constants.CurrentHighScoreContext[0], Constants.CurrentHighScoreContext[5], Constants.CurrentHighScoreContext[0] * 0.005f)
                || Constants.CurrentHighScoreContext[1] != Constants.CurrentHighScoreContext[6])
            {
                Constants.CurrentHighScoreContext[0] = 0;
                scoreValueStr = "altered score";
                _title.text = "Cheat Detected";
            }
            else
            {
                scoreValueStr = Constants.CurrentHighScoreContext[0].ToString();
                encryptedScore = EncryptedPlayerPrefs.Md5WithKey(Constants.CurrentHighScoreContext[0].ToString());
            }
            GameObject.Find("ScoreContext").GetComponent<TMPro.TextMeshPro>().text = $"{scoreValueStr}\n{Constants.CurrentHighScoreContext[1]}\n{Constants.CurrentHighScoreContext[2]}\n{Constants.CurrentHighScoreContext[3]}";
            if (Constants.CurrentHighScoreContext.Count > 4)
                GameObject.Find("CharacterContext").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + Constants.CurrentHighScoreContext[4]);
            if (_scoreHistory == null || _scoreHistory.Count == 0 || Constants.CurrentHighScoreContext[0] > _scoreHistory[0])
                _title.text = "New High Score";
            if (Constants.CurrentHighScoreContext[0] > 0)
                _scoreHistory.Add(Constants.CurrentHighScoreContext[0]);

        }
        _scoreHistory.Sort();
        _scoreHistory.Reverse();
        if (Constants.CurrentHighScoreContext != null && Constants.CurrentHighScoreContext[0] >= _scoreHistory[0])
        {
            PlayerPrefsHelper.SaveTrainingHighestScoreContext(Constants.CurrentHighScoreContext, encryptedScore);
            TrySendLocalHighest(null);
        }
        else if (Constants.CurrentHighScoreContext == null)
        {
            var _highestScore = PlayerPrefsHelper.GetTrainingHighestScoreContext();
            GameObject.Find("ScoreContext").GetComponent<TMPro.TextMeshPro>().text = $"{_highestScore[0]}\n{_highestScore[1]}\n{_highestScore[2]}\n{_highestScore[3]}";
            if (_highestScore.Count > 4)
                GameObject.Find("CharacterContext").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _highestScore[4]);
            _title.text = "High Scores";
        }
        UpdateScoreList();
        if (Constants.CurrentHighScoreContext != null)
            PlayerPrefsHelper.SaveTrainingHighScoreHistory(_scoreHistory);
        GameObject.Find("ButtonOnlineScores").GetComponent<ButtonBhv>().EndActionDelegate = GoToHighScores;
    }

    private void UpdateScoreList()
    {
        _scoreHistory.Sort();
        _scoreHistory.Reverse();
        for (int i = 0; i < _scoreHistory.Count; ++i)
        {
            var isCurrent = Constants.CurrentHighScoreContext != null && _scoreHistory[i] == Constants.CurrentHighScoreContext[0];
            if (i < 19)
            {
                var tmpInstance = Instantiator.NewScoreHistory(_scoreHistory[i], _scoreHistory[0], i, _scoreHistoryContainer);
                if (isCurrent)
                {
                    _tiltingBar = tmpInstance.transform.Find("Bar").GetComponent<ResourceBarBhv>();
                    tmpInstance.transform.Find("Bar").transform.Find("Content").GetComponent<SpriteRenderer>().sprite = HighLightedBar;
                }
            }
            if (isCurrent && i != 0)
                _title.text = $"{Helper.GetOrdinal(i + 1)} Best Score";
        }
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    public override void PauseOrPrevious()
    {
        GoToPrevious();
    }

    private void TrySendLocalHighest(Action afterTrySend)
    {
        if (AlreadyTrySendHighScoreOnThisInstance == true)
        {
            afterTrySend?.Invoke();
            return;
        }
        AlreadyTrySendHighScoreOnThisInstance = true;
        AccountService.CheckAccount(this.Instantiator, (account) =>
        {
            if (account == null)
            {
                afterTrySend?.Invoke();
                return;
            }
            //IF Better account score ouside of local scores
            HighScoresService.GetHighScore(account.PlayerName, (result) =>
            {
                var highestScore = PlayerPrefsHelper.GetTrainingHighestScoreContext();
                if (result != null && int.Parse(result.Score) > int.Parse(highestScore[0]))
                {
                    Debug.Log("Higher Score Received");
                    _scoreHistory = PlayerPrefsHelper.GetTrainingHighScoreHistory();
                    _scoreHistory.Add(int.Parse(result.Score));
                    PlayerPrefsHelper.SaveTrainingHighScoreHistory(_scoreHistory);
                    PlayerPrefsHelper.SaveTrainingHighestScoreContext(new List<int>() { int.Parse(result.Score), int.Parse(result.Level), int.Parse(result.Lines), int.Parse(result.Pieces), int.Parse(result.CharacterId) }, result.Key_Score);
                    UpdateScoreList();
                    afterTrySend?.Invoke();
                    return;
                }

                HighScoresService.PutHighScore(new HighScoreDto(account.PlayerName, highestScore[0], highestScore[1], highestScore[2], highestScore[3], highestScore[4], highestScore[5]), () =>
                {
                    Debug.Log("HighScore Sent");
                    afterTrySend?.Invoke();
                });
            });
        });
    }

    private void GoToHighScores()
    {
        TrySendLocalHighest(() =>
        {
            Constants.CurrentHighScoreContext = null;
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
            object OnBlend(bool result)
            {
                NavigationService.LoadNextScene(Constants.OnlineScoreScene);
                return true;
            }
        });
    }

    private void GoToPrevious()
    {
        Constants.CurrentHighScoreContext = null;
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadBackUntil(Constants.TrainingChoiceScene);
            return true;
        }
    }
}
