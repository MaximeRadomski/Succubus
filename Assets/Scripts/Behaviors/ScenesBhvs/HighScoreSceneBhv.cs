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
    private bool _isOldSchool = false;

    public bool AlreadyTrySendHighScoreOnThisInstance = false;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void FrameUpdate()
    {
        if (_tiltingBar != null)
            _tiltingBar.Tilt();
    }

    protected override void Init()
    {
        base.Init();
        _isOldSchool = NavigationService.NextSceneParameter?.BoolParam1 == true;
        SetButtons();
        _scoreHistory = PlayerPrefsHelper.GetTrainingHighScoreHistory(_isOldSchool);
        _title = GameObject.Find("Title").GetComponent<TMPro.TextMeshPro>();
        _scoreHistoryContainer = GameObject.Find("ScoreHistoryContainer");
        var scoreValueStr = "";
        var encryptedScore = "";
        var type = UnityEngine.Random.Range(0, Mock.test.Count);
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
                encryptedScore = Mock.Md5WithKey(Constants.CurrentHighScoreContext[0].ToString(), type);
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
        var lastCredentials = PlayerPrefsHelper.GetLastSavedCredentials();
        if (Constants.CurrentHighScoreContext != null && Constants.CurrentHighScoreContext[0] >= _scoreHistory[0])
        {
            PlayerPrefsHelper.SaveTrainingHighestScore(Constants.CurrentHighScoreContext, encryptedScore, type, _isOldSchool);
            TrySendLocalHighest(null);
        }
        else if (Constants.CurrentHighScoreContext != null && lastCredentials != null && lastCredentials.PlayerName != null)
        {
            var notHighestScoreButStill = new HighScoreDto(lastCredentials.PlayerName, Constants.CurrentHighScoreContext[0], Constants.CurrentHighScoreContext[1], Constants.CurrentHighScoreContext[2], Constants.CurrentHighScoreContext[3], Constants.CurrentHighScoreContext[4], type, encryptedScore);
            TrySendLocalHighest(null, notHighestScoreButStill);
        }
        else if (Constants.CurrentHighScoreContext == null)
        {
            var _highestScore = PlayerPrefsHelper.GetTrainingHighestScore(_isOldSchool);
            GameObject.Find("ScoreContext").GetComponent<TMPro.TextMeshPro>().text = $"{_highestScore.Score}\n{_highestScore.Level}\n{_highestScore.Lines}\n{_highestScore.Pieces}";
            GameObject.Find("CharacterContext").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _highestScore.CharacterId);
            _title.text = "High Scores";
        }
        UpdateScoreList();
        if (Constants.CurrentHighScoreContext != null)
            PlayerPrefsHelper.SaveTrainingHighScoreHistory(_scoreHistory, _isOldSchool);
        GameObject.Find("ButtonOnlineScores").GetComponent<ButtonBhv>().EndActionDelegate = GoToHighScores;
        TrySendLocalHighest(null);
    }

    private void UpdateScoreList()
    {
        _scoreHistory.Sort();
        _scoreHistory.Reverse();
        foreach (Transform child in _scoreHistoryContainer.transform)
            Destroy(child.gameObject);
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

    private void TrySendLocalHighest(Action afterTrySend, HighScoreDto thisOneInstead = null)
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
            HighScoresService.GetHighScore(account.PlayerName, _isOldSchool, (onlineScore) =>
            {
                var highestScore = PlayerPrefsHelper.GetTrainingHighestScore(_isOldSchool);
                if (thisOneInstead != null)
                    highestScore = thisOneInstead;
                //IF Better account score outside of local scores
                if (onlineScore != null && onlineScore.Score > highestScore.Score)
                {
                    Debug.Log("Higher Score Received");
                    _scoreHistory = PlayerPrefsHelper.GetTrainingHighScoreHistory(_isOldSchool);
                    if (_scoreHistory.Contains(onlineScore.Score))
                        return;
                    _scoreHistory.Add(onlineScore.Score);
                    PlayerPrefsHelper.SaveTrainingHighScoreHistory(_scoreHistory, _isOldSchool);
                    PlayerPrefsHelper.SaveTrainingHighestScore(new List<int>() { onlineScore.Score, onlineScore.Level, onlineScore.Lines, onlineScore.Pieces, onlineScore.CharacterId }, onlineScore.Checksum, onlineScore.Type, _isOldSchool);
                    UpdateScoreList();
                    afterTrySend?.Invoke();
                    return;
                }
                //If same account score outside of local scores (in order to prevent rewriting everytime the date of the best score)
                else if (onlineScore != null && onlineScore.Score == highestScore.Score)
                {
                    Debug.Log("Same Score Received");
                    afterTrySend?.Invoke();
                    return;
                }
                //If exact same score and 
                HighScoresService.CheckCloneScore(highestScore, _isOldSchool, (clones) =>
                {
                    if (clones != null)
                    {
                        foreach (var clone in clones)
                        {
                            if (clone.PlayerName != account.PlayerName
                            && clone.Lines == highestScore.Lines
                            && clone.Level == highestScore.Level
                            && clone.Pieces == highestScore.Pieces)
                            {
                                Debug.Log("CLONE from another player Received");
                                afterTrySend?.Invoke();
                                return;
                            }
                        }                        
                    }
                    HighScoresService.PutHighScore(new HighScoreDto(account.PlayerName, highestScore.Score, highestScore.Level, highestScore.Lines, highestScore.Pieces, highestScore.CharacterId, highestScore.Type, highestScore.Checksum), _isOldSchool, () =>
                    {
                        Debug.Log("HighScore Sent");
                        afterTrySend?.Invoke();
                    });
                    return;
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
                NavigationService.LoadNextScene(Constants.OnlineScoreScene, new NavigationParameter() { BoolParam1 = _isOldSchool });
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
