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

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _pelliculeTop = GameObject.Find("PelliculeTop").transform;
        _pelliculeBot = GameObject.Find("PelliculeBot").transform;

        _pellicules = new List<SpriteRenderer>();
        _pellicules.Add(GameObject.Find("PelliculeLeft").GetComponent<SpriteRenderer>());
        _pellicules.Add(GameObject.Find("PelliculeMid").GetComponent<SpriteRenderer>());
        _pellicules.Add(GameObject.Find("PelliculeRight").GetComponent<SpriteRenderer>());

        _loreText = GameObject.Find("LoreText").GetComponent<TMPro.TextMeshPro>();
        CinematicsData.Cinematics.TryGetValue("Intro", out _lore);

        _progression = 0;
        _pelliculeMove = 0;
        _hasInit = true;

        StartCoroutine(NextCinematic());
    }

    private IEnumerator NextCinematic()
    {
        var end = false;
        if (_progression % 2 == 0)
        {
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Cinematics_" + _progression / 2);
            if (sprite == null)
            {
                end = true;
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
            _loreText.text = _lore[_progression];
        }

        yield return new WaitForSeconds(_progression == 0 ? 7.6f : 6.5f);
        ++_progression;
        if (!end)
            StartCoroutine(NextCinematic());
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
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.NewRootScene(Constants.CharSelScene);
            return true;
        }
    }
}
