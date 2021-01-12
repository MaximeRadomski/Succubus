using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHowToPlaySceneBhv : SceneBhv
{
    private GameObject _buttonPrevious;
    private GameObject _buttonNext;
    private SpriteRenderer _panel1;
    private SpriteRenderer _panel2;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        SetButtons();
    }

    private void SetButtons()
    {
        (_buttonPrevious = GameObject.Find("ButtonPrevious")).GetComponent<ButtonBhv>().EndActionDelegate = Previous;
        (_buttonNext = GameObject.Find("ButtonNext")).GetComponent<ButtonBhv>().EndActionDelegate = Next;

        _panel1 = GameObject.Find("Panel1").GetComponent<SpriteRenderer>();
        _panel1 = GameObject.Find("Panel2").GetComponent<SpriteRenderer>();
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void Previous()
    {

    }

    private void Next()
    {

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
