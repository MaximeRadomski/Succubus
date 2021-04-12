using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHowToPlaySceneBhv : SceneBhv
{
    private GameObject _buttonPrevious;
    private GameObject _buttonNext;
    private List<GameObject> _panels;
    private int _currentPanelId;
    private InputControlerBhv _inputControlerBhv;

    public override MusicType MusicType => MusicType.Continue;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _panels = new List<GameObject>();
        for (int i = 0; i < 55/*Why 55?*/; ++i)
        {
            var tmpPanel = GameObject.Find($"Panel{i.ToString("00")}");
            if (tmpPanel == null)
                break;
            _panels.Add(tmpPanel);
        }
        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        SetButtons();
        _currentPanelId = 0;
        UpdateVisuals(Direction.None);
    }

    private void SetButtons()
    {
        (_buttonPrevious = GameObject.Find("ButtonPrevious")).GetComponent<ButtonBhv>().EndActionDelegate = Previous;
        (_buttonNext = GameObject.Find("ButtonNext")).GetComponent<ButtonBhv>().EndActionDelegate = Next;
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void UpdateVisuals(Direction direction)
    {
        for (int i = 0; i < _panels.Count; ++i)
        {
            if (i == _currentPanelId)
                _panels[i].transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            else
                _panels[i].transform.position = new Vector3((i * 10.0f) + 30.0f, 30.0f, 0.0f);
        }

        if (_currentPanelId == 0)
            _buttonPrevious.SetActive(false);
        else
            _buttonPrevious.SetActive(true);

        if (_currentPanelId == _panels.Count - 1)
                _buttonNext.SetActive(false);
            else
            _buttonNext.SetActive(true);

#if !UNITY_ANDROID
        if (direction == Direction.None)
            _inputControlerBhv.InitMenuKeyboardInputs();
        else
            _inputControlerBhv.InitMenuKeyboardInputs(_inputControlerBhv.MenuSelector.transform.position + new Vector3(0.0f, 3.0f, 0.0f));
#endif
    }

    private void Previous()
    {
        --_currentPanelId;
        if (_currentPanelId < 0)
            _currentPanelId = 0;
        UpdateVisuals(Direction.Left);
    }

    private void Next()
    {
        ++_currentPanelId;
        if (_currentPanelId >= _panels.Count)
            _currentPanelId = _panels.Count - 1;
        UpdateVisuals(Direction.Right);
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        object OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }
}
