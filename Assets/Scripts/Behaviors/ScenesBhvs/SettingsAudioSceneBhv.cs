using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsAudioSceneBhv : SceneBhv
{
    private GameObject _effectsLevelSelector;
    private GameObject _musicLevelSelector;
    private GameObject _vibrationSelector;
    private GameObject _rhythmAttacksSelector;

    private MusicControlerBhv _musicControlerBhv;
    private SoundControlerBhv _soundControler;

    private int _idSpecial;
#if UNITY_ANDROID
    private bool _hasInit;
    private float _containerSpace = 5.5f;
#endif

    public override MusicType MusicType => MusicType.Continue;

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
        _rhythmAttacksSelector = GameObject.Find("RhythmAttacksSelector");
        _musicControlerBhv = GameObject.Find(Constants.GoMusicControler)?.GetComponent<MusicControlerBhv>();
        _soundControler = GameObject.Find("SoundControler").GetComponent<SoundControlerBhv>();
        _idSpecial = _soundControler.SetSound("Special");

#if UNITY_ANDROID
        if (!_hasInit)
        {
            GameObject.Find("VibrationContainer").transform.position = new Vector3(0.0f, -_containerSpace, 0.0f);
            GameObject.Find("SettingsContainer").transform.position += new Vector3(0.0f, _containerSpace / 1.5f);
            GameObject.Find("SettingsTitle").GetComponent<TMPro.TextMeshPro>().text = "Audio & Vibration";
        }
        _hasInit = true;
#endif
        SetButtons();
        Cache.SetLastEndActionClickedName("EffectsLevel" + String.Format("{0:0.00}", PlayerPrefsHelper.GetEffectsLevel()));
        LevelChoice();
        Cache.SetLastEndActionClickedName("MusicLevel" + String.Format("{0:0.00}", PlayerPrefsHelper.GetMusicLevel()));
        LevelChoice();
#if UNITY_ANDROID
        Cache.SetLastEndActionClickedName($"Vibration{(PlayerPrefsHelper.GetVibrationEnabled() == true ? "On" : "Off")}");
        SetVibration(PlayerPrefsHelper.GetVibrationEnabled());
#endif
        Cache.SetLastEndActionClickedName($"RhythmAttacks{(PlayerPrefsHelper.GetRhythmAttacksEnabled() == true ? "On" : "Off")}");
        SetRhythmAttacks(PlayerPrefsHelper.GetRhythmAttacksEnabled());
    }

    private void SetButtons()
    {
        SetLevelsButtons(GameObject.Find("EffectsLevels"), "Effects");
        SetLevelsButtons(GameObject.Find("MusicLevels"), "Music");

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find("ButtonReset").GetComponent<ButtonBhv>().EndActionDelegate = ResetDefault;
        GameObject.Find("VibrationOn").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetVibration(true); };
        GameObject.Find("VibrationOff").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetVibration(false); };
        GameObject.Find("RhythmAttacksOn").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRhythmAttacks(true); };
        GameObject.Find("RhythmAttacksOff").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRhythmAttacks(false); };
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
        var choiceLevelSring = Cache.LastEndActionClickedName.Substring(Cache.LastEndActionClickedName.Length - 4);
        var choiceLevel = -1.0f;
        if (!float.TryParse(choiceLevelSring.Replace(",", "."), out choiceLevel))
            float.TryParse(choiceLevelSring.Replace(".", ","), out choiceLevel);
        var choiceGameObject = GameObject.Find(Cache.LastEndActionClickedName.Replace(",", "."));
        if (Cache.LastEndActionClickedName.Contains("Effects"))
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
        var choiceGameObject = GameObject.Find(Cache.LastEndActionClickedName);
        _vibrationSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _vibrationSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveVibrationEnabled(result);
        VibrationService.SetVibrationEnabled();
        if (result)
            VibrationService.Vibrate();
    }

    private void SetRhythmAttacks(bool result)
    {
        var choiceGameObject = GameObject.Find(Cache.LastEndActionClickedName);
        _rhythmAttacksSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _rhythmAttacksSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveRhythmAttacksEnabled(result);
        GameObject.Find("RhythmAttacksStatus").GetComponent<TMPro.TextMeshPro>().enabled = !result;
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

    private void ResetDefault()
    {
        Instantiator.NewPopupYesNo("Default", "are you willing to restore the default settings ?", "Nope", "Yup", OnDefault);
        void OnDefault(bool result)
        {
            if (!result)
                return;
            PlayerPrefsHelper.SaveEffectsLevel(Constants.PpAudioLevelDefault);
            PlayerPrefsHelper.SaveMusicLevel(Constants.PpAudioLevelDefault);
            PlayerPrefsHelper.SaveVibrationEnabled(Constants.PpVibrationEnabledDefault);
            Init();
        }
    }
}
