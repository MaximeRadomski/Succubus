using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsDisplaySceneBhv : SceneBhv
{
    private GameObject _fullscreenSelector;
    private GameObject _resolutionSelector;
    private GameObject _scanlinesSelector;

    private GameObject _fullscreenContainer;
    private GameObject _resolutionContainer;
    private GameObject _scanlinesContainer;
    private GameObject _loremIpsumContainer;

    private ResolutionService _resolutionService;
    private ScanlinesEffect _scanlinesEffect;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _resolutionService = GetComponent<ResolutionService>();
        _scanlinesEffect = CameraBhv.GetComponent<ScanlinesEffect>();

        _fullscreenSelector = GameObject.Find("FullscreenSelector");
        _resolutionSelector = GameObject.Find("ResolutionSelector");
        _scanlinesSelector = GameObject.Find("ScanlinesSelector");

        _fullscreenContainer = GameObject.Find("FullscreenContainer");
        _resolutionContainer = GameObject.Find("ResolutionContainer");
        _scanlinesContainer = GameObject.Find("ScanlinesContainer");
        _loremIpsumContainer = GameObject.Find("LoremIpsumContainer");

        SetButtons();

#if !UNITY_ANDROID
        Constants.SetLastEndActionClickedName($"Res{PlayerPrefsHelper.GetResolution()}");
        SetRes(PlayerPrefsHelper.GetResolution());
        Constants.SetLastEndActionClickedName($"Fullscreen{(PlayerPrefsHelper.GetFullscreen() == true ? "On" : "Off")}");
        SetFullscreen(PlayerPrefsHelper.GetFullscreen());
#else
        _fullscreenContainer.transform.position = new Vector3(-25.0f, -25.0f, 0.0f);
        _resolutionContainer.transform.position = new Vector3(-25.0f, -25.0f, 0.0f);
        _loremIpsumContainer.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
#endif
        SetScanlines(PlayerPrefsHelper.GetScanlinesHardness());
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

        GameObject.Find("Scanline7").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(7); };
        GameObject.Find("Scanline6").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(6); };
        GameObject.Find("Scanline5").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(5); };
        GameObject.Find("Scanline4").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(4); };
        GameObject.Find("Scanline3").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(3); };
        GameObject.Find("Scanline2").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(2); };
        GameObject.Find("Scanline1").GetComponent<ButtonBhv>().DoActionDelegate = () => { SetScanlines(1); };

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
    }

    private void SetFullscreen(bool result)
    {
        var choiceGameObject = GameObject.Find(Constants.LastEndActionClickedName);
        _fullscreenSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _fullscreenSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveFullscreen(result);
        Screen.SetResolution(Screen.width, Screen.height, result);
    }

    private void SetRes(int resId)
    {
        PlayerPrefsHelper.SaveResolution(_resolutionService.SetResolution(resId));
        var choiceGameObject = GameObject.Find($"Res{PlayerPrefsHelper.GetResolution()}");
        _resolutionSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, choiceGameObject.transform.position.y, 0.0f);
    }

    private void SetScanlines(int divider)
    {
        var buttonName = $"Scanline{divider.ToString("0")}";
        var buttonTapped = GameObject.Find(buttonName);
        _scanlinesSelector.transform.position = buttonTapped.transform.position;
        PlayerPrefsHelper.SaveScanlinesHardness(divider);
        _scanlinesEffect.UpdateHardness();
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
