using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDisplaySceneBhv : SceneBhv
{
    private GameObject _fullScreenSelector;
    private GameObject _resolutionSelector;

    private ResolutionService _resolutionService;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _resolutionService = GetComponent<ResolutionService>();
        _fullScreenSelector = GameObject.Find("FullscreenSelector");
        _resolutionSelector = GameObject.Find("ResolutionSelector");

        SetButtons();

        Constants.SetLastEndActionClickedName($"Res{PlayerPrefsHelper.GetResolution()}");
        SetRes(PlayerPrefsHelper.GetResolution());
        Constants.SetLastEndActionClickedName($"Fullscreen{(PlayerPrefsHelper.GetFullscreen() == true ? "On" : "Off")}");
        SetFullscreen(PlayerPrefsHelper.GetFullscreen());
    }

    private void SetButtons()
    {
        var resolutions = _resolutionService.GetResolutions();
        for (int i = 0; i < resolutions.Count; ++i)
            GameObject.Find($"Res{i}").GetComponent<TMPro.TextMeshPro>().text = $"{resolutions[i].width}\nx\n{resolutions[i].height}";
        GameObject.Find($"Res0").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(0); };
        GameObject.Find($"Res1").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(1); };
        GameObject.Find($"Res2").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(2); };
        GameObject.Find($"Res3").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(3); };
        GameObject.Find($"Res4").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(4); };
        GameObject.Find($"Res5").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(5); };
        GameObject.Find($"Res6").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(6); };
        GameObject.Find($"Res7").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(7); };
        GameObject.Find($"Res8").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetRes(8); };

        GameObject.Find("FullscreenOn").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFullscreen(true); };
        GameObject.Find("FullscreenOff").GetComponent<ButtonBhv>().EndActionDelegate = () => { SetFullscreen(false); };

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void SetFullscreen(bool result)
    {
        var choiceGameObject = GameObject.Find(Constants.LastEndActionClickedName);
        _fullScreenSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _fullScreenSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveFullscreen(result);
        Screen.SetResolution(Screen.width, Screen.height, result);
    }

    private void SetRes(int resId)
    {
        PlayerPrefsHelper.SaveResolution(_resolutionService.SetResolution(resId));
        var choiceGameObject = GameObject.Find($"Res{PlayerPrefsHelper.GetResolution()}");
        _resolutionSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, choiceGameObject.transform.position.y, 0.0f);
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
