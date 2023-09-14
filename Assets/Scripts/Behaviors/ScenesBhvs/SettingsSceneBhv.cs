using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsSceneBhv : SceneBhv
{
    private GameObject _buttonContainer;
    private GameObject _buttonAudio;
    private GameObject _buttonGameplay;
    private GameObject _buttonInputs;
    private GameObject _buttonHowToPlay;
    private GameObject _buttonDisplay;

    public override MusicType MusicType => MusicType.Continue;

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
        _buttonContainer = GameObject.Find("ButtonContainer");
        (_buttonAudio = GameObject.Find("ButtonAudio")).GetComponent<ButtonBhv>().EndActionDelegate = GoToAudioSettings;
        (_buttonGameplay = GameObject.Find("ButtonGameplay")).GetComponent<ButtonBhv>().EndActionDelegate = GoToGameplaySettings;
        (_buttonInputs = GameObject.Find("ButtonInputs")).GetComponent<ButtonBhv>().EndActionDelegate = GoToInputsSettings;
        (_buttonHowToPlay = GameObject.Find("ButtonHowToPlay")).GetComponent<ButtonBhv>().EndActionDelegate = GoToHowToPlay;
        (_buttonDisplay = GameObject.Find("ButtonDisplay")).GetComponent<ButtonBhv>().EndActionDelegate = GoToDisplaySettings;
#if !UNITY_ANDROID
        _buttonAudio.transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = "Audio";
        _buttonAudio.transform.GetChild(1).localScale = new Vector3(2.411f, 1, 1);
#endif
        //_buttonDisplay.transform.position = _buttonHowToPlay.transform.position;
        //_buttonHowToPlay.transform.position += new Vector3(0.0f, -_buttonSpacing, 0.0f);
        //_buttonContainer.transform.position += new Vector3(0.0f, _buttonSpacing / 2);

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void GoToAudioSettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsAudioScene);
            return true;
        }
    }

    private void GoToGameplaySettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsGameplayScene);
            return true;
        }
    }

    private void GoToInputsSettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsInputsScene);
            return true;
        }
    }

    private void GoToDisplaySettings()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsDisplayScene);
            return true;
        }
    }

    private void GoToHowToPlay()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.SettingsHowToPlay);
            return true;
        }
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        bool OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }
}
