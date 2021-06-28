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
        if (Constants.CurrentHighScoreContext != null)
        {
            if (!Helper.FloatEqualsPrecision(Constants.CurrentHighScoreContext[0], Constants.CurrentHighScoreContext[5], Constants.CurrentHighScoreContext[0] * 0.005f))
            {
                return;
            }
            GameObject.Find("ScoreContext").GetComponent<TMPro.TextMeshPro>().text = $"{Constants.CurrentHighScoreContext[0]}\n{Constants.CurrentHighScoreContext[1]}\n{Constants.CurrentHighScoreContext[2]}\n{Constants.CurrentHighScoreContext[3]}";
            if (Constants.CurrentHighScoreContext.Count > 4)
                GameObject.Find("CharacterContext").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + Constants.CurrentHighScoreContext[4]);
            if (_scoreHistory == null || _scoreHistory.Count == 0 || Constants.CurrentHighScoreContext[0] > _scoreHistory[0])
                _title.text = "New High Score";
            _scoreHistory.Add(Constants.CurrentHighScoreContext[0]);

        }
        _scoreHistory.Sort();
        _scoreHistory.Reverse();
        if (Constants.CurrentHighScoreContext != null && Constants.CurrentHighScoreContext[0] >= _scoreHistory[0])
            PlayerPrefsHelper.SaveTrainingHighestScoreContext(Constants.CurrentHighScoreContext);
        else if (Constants.CurrentHighScoreContext == null)
        {
            var highestScore = PlayerPrefsHelper.GetTrainingHighestScoreContext();
            GameObject.Find("ScoreContext").GetComponent<TMPro.TextMeshPro>().text = $"{highestScore[0]}\n{highestScore[1]}\n{highestScore[2]}\n{highestScore[3]}";
            if (highestScore.Count > 4)
                GameObject.Find("CharacterContext").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + highestScore[4]);
            _title.text = "High Scores";
        }
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
        PlayerPrefsHelper.SaveTrainingHighScoreHistory(_scoreHistory);
    }

    private void SetButtons()
    {
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    public override void PauseOrPrevious()
    {
        GoToPrevious();
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
