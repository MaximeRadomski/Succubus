using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsAudioSceneBhv : SceneBhv
{
    private GameObject _effectsLevelSelector;
    private GameObject _musicLevelSelector;
    private GameObject _vibrationSelector;

    private MusicControlerBhv _musicControlerBhv;
    private SoundControlerBhv _soundControler;

    private int _idSpecial;
    private float _containerSpace = 5.5f;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _effectsLevelSelector = GameObject.Find("EffectsLevelSelector");
        _musicLevelSelector = GameObject.Find("MusicLevelSelector");
        _vibrationSelector = GameObject.Find("VibrationSelector");
        _musicControlerBhv = GameObject.Find(Constants.GoMusicControler).GetComponent<MusicControlerBhv>();
        _soundControler = GameObject.Find("SoundControler").GetComponent<SoundControlerBhv>();
        _idSpecial = _soundControler.SetSound("Special");

#if UNITY_ANDROID
        GameObject.Find("VibrationContainer").transform.position = new Vector3(0.0f, -_containerSpace, 0.0f);
        GameObject.Find("SettingsContainer").transform.position += new Vector3(0.0f, _containerSpace / 2);
        GameObject.Find("SettingsTitle").GetComponent<TMPro.TextMeshPro>().text = "Audio & Vibration";
#endif
        SetButtons();
        Constants.SetLastEndActionClickedName("EffectsLevel" + String.Format("{0:0.00}", PlayerPrefsHelper.GetEffectsLevel()));
        LevelChoice();
        Constants.SetLastEndActionClickedName("MusicLevel" + String.Format("{0:0.00}", PlayerPrefsHelper.GetMusicLevel()));
        LevelChoice();
#if UNITY_ANDROID
        Constants.SetLastEndActionClickedName($"Vibration{(PlayerPrefsHelper.GetVibrationEnabled() == true ? "On" : "Off")}");
        SetVibration(PlayerPrefsHelper.GetVibrationEnabled());
#endif
    }

    private void SetButtons()
    {
        SetLevelsButtons(GameObject.Find("EffectsLevels"), "Effects");
        SetLevelsButtons(GameObject.Find("MusicLevels"), "Music");

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find("ButtonReset").GetComponent<ButtonBhv>().EndActionDelegate = ResetDefault;
        GameObject.Find("VibrationOn").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetVibration(true); };
        GameObject.Find("VibrationOff").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetVibration(false); };
    }

    private void SetLevelsButtons(GameObject levelsContainer, string prefix)
    {
        levelsContainer.transform.Find(prefix + "Level0.00").GetComponent<ButtonBhv>().EndActionDelegate = LevelChoice;
        levelsContainer.transform.Find(prefix + "Level0.25").GetComponent<ButtonBhv>().EndActionDelegate = LevelChoice;
        levelsContainer.transform.Find(prefix + "Level0.50").GetComponent<ButtonBhv>().EndActionDelegate = LevelChoice;
        levelsContainer.transform.Find(prefix + "Level0.75").GetComponent<ButtonBhv>().EndActionDelegate = LevelChoice;
        levelsContainer.transform.Find(prefix + "Level1.00").GetComponent<ButtonBhv>().EndActionDelegate = LevelChoice;
    }

    private void LevelChoice()
    {
        var choiceLevelSring = Constants.LastEndActionClickedName.Substring(Constants.LastEndActionClickedName.Length - 4);
        var choiceLevel = -1.0f;
        if (!float.TryParse(choiceLevelSring.Replace(",", "."), out choiceLevel))
            float.TryParse(choiceLevelSring.Replace(".", ","), out choiceLevel);
        var choiceGameObject = GameObject.Find(Constants.LastEndActionClickedName.Replace(",", "."));
        if (Constants.LastEndActionClickedName.Contains("Effects"))
        {
            _effectsLevelSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _effectsLevelSelector.transform.position.y, 0.0f);
            PlayerPrefsHelper.SaveEffectsLevel(choiceLevel);
            _soundControler.SetNewVolumeLevel(choiceLevel);
            _soundControler.PlaySound(_idSpecial);
        }
        else
        {
            _musicLevelSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _musicLevelSelector.transform.position.y, 0.0f);
            PlayerPrefsHelper.SaveMusicLevel(choiceLevel);
            _musicControlerBhv.SetNewVolumeLevel(choiceLevel);
        }
    }

    private void SetVibration(bool result)
    {
        var choiceGameObject = GameObject.Find(Constants.LastEndActionClickedName);
        _vibrationSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _vibrationSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveVibrationEnabled(result);
        if (result)
            VibrationService.Vibrate();
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

    private void ResetDefault()
    {
        Instantiator.NewPopupYesNo("Default", "are you willing to restore the default settings ?", "Nope", "Yup", OnDefault);
        object OnDefault(bool result)
        {
            if (!result)
                return result;
            PlayerPrefsHelper.SaveEffectsLevel(Constants.PpAudioLevelDefault);
            PlayerPrefsHelper.SaveMusicLevel(Constants.PpAudioLevelDefault);
            PlayerPrefsHelper.SaveVibrationEnabled(Constants.PpVibrationEnabledDefault);
            Init();
            return result;
        }
    }
}
