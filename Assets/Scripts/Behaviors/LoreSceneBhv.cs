using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreSceneBhv : SceneBhv
{
    public override MusicType MusicType => MusicType.Lore;

    private Transform _pelliculeTop;
    private Transform _pelliculeBot;
    private List<SpriteRenderer> _pellicules;
    private TMPro.TextMeshPro _loreText;

    private int _progression;
    private int _pelliculeMove;
    private List<string> _lore;
    private bool _hasInit;

    private float _spamNextDelay = 0.1f;
    private float _nextAvailableSpamNext = 0.0f;
    private bool _canGoNext = true;

    private int _cinematicId;

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        CameraBhv.Init();
        _pelliculeTop = GameObject.Find("PelliculeTop").transform;
        _pelliculeBot = GameObject.Find("PelliculeBot").transform;

        _pellicules = new List<SpriteRenderer>();
        _pellicules.Add(GameObject.Find("PelliculeLeft").GetComponent<SpriteRenderer>());
        _pellicules.Add(GameObject.Find("PelliculeMid").GetComponent<SpriteRenderer>());
        _pellicules.Add(GameObject.Find("PelliculeRight").GetComponent<SpriteRenderer>());

        _loreText = GameObject.Find("LoreText").GetComponent<TMPro.TextMeshPro>();
        if (NavigationService.SceneParameter != null)
            _cinematicId = NavigationService.SceneParameter?.IntParam0 ?? 0;
        else
        {
            var run = PlayerPrefsHelper.GetRun();
            _cinematicId = run?.CurrentRealm.GetHashCode() ?? 0;
        }
        _lore = CinematicsData.Cinematics[_cinematicId];

        _progression = 0;
        _pelliculeMove = 0;
        _hasInit = true;

        GameObject.Find("Background").GetComponent<ButtonBhv>().EndActionDelegate = GoToNext;

        StartCoroutine(PlayCinematicPart());
    }

    private IEnumerator PlayCinematicPart()
    {
        if (_progression % 2 == 0)
        {
            var sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Cinematic{_cinematicId}_{_progression / 2}");
            if (sprite == null)
            {
                _canGoNext = false;
                LoadNext();
            }
            else
            {
                _loreText.text = "";
                for (int i = 2; i >= 0; --i)
                {
                    _pellicules[i].sprite = sprite;
                    yield return new WaitForSeconds(0.075f);
                }
                _loreText.text = _lore[_progression];
            }
        }
        else
        {
            _loreText.text = "";
            yield return new WaitForSeconds(0.25f);
            if (_progression > 0 && _progression < _lore.Count)
                _loreText.text = _lore[_progression];
        }

        var progressionBeforeDelay = _progression;
        yield return new WaitForSeconds(_progression == 0 ? 7.6f : 6.5f);
        if (progressionBeforeDelay == _progression) // This checks if player hasn't click next
        {
            ++_progression;
            if (_canGoNext)
                StartCoroutine(PlayCinematicPart());
        }
    }

    protected override void FrameUpdate()
    {
        if (!_hasInit)
            return;
        if (_pelliculeMove >= 10)
        {
            _pelliculeTop.transform.localPosition = new Vector3(0.0f, _pelliculeTop.transform.localPosition.y, 0.0f);
            _pelliculeBot.transform.localPosition = new Vector3(0.0f, _pelliculeBot.transform.localPosition.y, 0.0f);
            _pelliculeMove = 0;
        }
        else
        {
            _pelliculeTop.transform.localPosition = new Vector3(_pelliculeTop.transform.localPosition.x + (2 * Constants.Pixel), _pelliculeTop.transform.localPosition.y, 0.0f);
            _pelliculeBot.transform.localPosition = new Vector3(_pelliculeBot.transform.localPosition.x - (2 * Constants.Pixel), _pelliculeBot.transform.localPosition.y, 0.0f);
            ++_pelliculeMove;
        }
    }

    private void LoadNext()
    {
        PlayerPrefsHelper.SaveWatchedCinematics(_cinematicId);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            if (NavigationService.Path.Contains(Constants.StepsAscensionScene))
                NavigationService.LoadBackUntil(NavigationService.SceneParameter?.StringParam0 ?? Constants.StepsAscensionScene);
            else
                NavigationService.LoadNextScene(NavigationService.SceneParameter?.StringParam0 ?? Constants.CharSelScene);
            return true;
        }
    }

    private void GoToNext()
    {
        if (Time.time > _nextAvailableSpamNext)
        {
            _nextAvailableSpamNext = Time.time + _spamNextDelay;
            ++_progression;
            if (_canGoNext)
                StartCoroutine(PlayCinematicPart());
        }
        
        //this.Instantiator.NewPopupYesNo("Don't like lore?", "would you like to skip the cinematic?", "No", "Yes", OnSkipResult);

        //object OnSkipResult(bool result)
        //{
        //    if (result)
        //        LoadNext();
        //    return result;
        //}
    }
}
