using System.Collections.Generic;
using UnityEngine;

public class SettingsGameplaySceneBhv : SceneBhv
{
    private GameObject _ghostSelector;
    private GameObject _dasSelector;
    private GameObject _arrSelector;
    private GameObject _rotationPointSelector;
    private GameObject _classicPiecesSelector;

    private Piece _tHell;
    private Piece _iHell;

    private TMPro.TextMeshPro _unpleasedDev;

    public override MusicType MusicType => MusicType.Continue;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _ghostSelector = GameObject.Find("GhostSelector");
        _dasSelector = GameObject.Find("DasSelector");
        _arrSelector = GameObject.Find("ArrSelector");
        _rotationPointSelector = GameObject.Find("RotationPointSelector");
        _classicPiecesSelector = GameObject.Find("ClassicPiecesSelector");

        _tHell = GameObject.Find("T-Hell").GetComponent<Piece>();
        _iHell = GameObject.Find("I-Hell").GetComponent<Piece>();

        _unpleasedDev = GameObject.Find("UnpleasedDev").GetComponent<TMPro.TextMeshPro>();

        SetButtons();

        Cache.SetLastEndActionClickedName(PlayerPrefsHelper.GetGhostColor());
        GhostColorChoice();

        DasChoice(PlayerPrefsHelper.GetDas());
        ArrChoice(PlayerPrefsHelper.GetArr());
        ClassicPiecesChoice(PlayerPrefsHelper.GetClassicPieces());
    }

    private void SetButtons()
    {
        GameObject.Find("Ghost1").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost2").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost3").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost4").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;
        GameObject.Find("Ghost5").GetComponent<ButtonBhv>().EndActionDelegate = GhostColorChoice;

        GameObject.Find("Das04").GetComponent<ButtonBhv>().EndActionDelegate = () => { DasChoice(04); };
        GameObject.Find("Das06").GetComponent<ButtonBhv>().EndActionDelegate = () => { DasChoice(06); };
        GameObject.Find("Das08").GetComponent<ButtonBhv>().EndActionDelegate = () => { DasChoice(08); };
        GameObject.Find("Das10").GetComponent<ButtonBhv>().EndActionDelegate = () => { DasChoice(10); };
        GameObject.Find("Das12").GetComponent<ButtonBhv>().EndActionDelegate = () => { DasChoice(12); };

        GameObject.Find("Arr0").GetComponent<ButtonBhv>().EndActionDelegate = () => { ArrChoice(0); };
        GameObject.Find("Arr1").GetComponent<ButtonBhv>().EndActionDelegate = () => { ArrChoice(1); };
        GameObject.Find("Arr2").GetComponent<ButtonBhv>().EndActionDelegate = () => { ArrChoice(2); };
        GameObject.Find("Arr3").GetComponent<ButtonBhv>().EndActionDelegate = () => { ArrChoice(3); };
        GameObject.Find("Arr4").GetComponent<ButtonBhv>().EndActionDelegate = () => { ArrChoice(4); };

        GameObject.Find("RotationPointOff").GetComponent<ButtonBhv>().EndActionDelegate = () => { RotationPointChoice(false); };
        GameObject.Find("RotationPointOn").GetComponent<ButtonBhv>().EndActionDelegate = () => { RotationPointChoice(true); };

        GameObject.Find("ClassicPiecesOff").GetComponent<ButtonBhv>().EndActionDelegate = () => { ClassicPiecesChoice(false); };
        GameObject.Find("ClassicPiecesOn").GetComponent<ButtonBhv>().EndActionDelegate = () => { ClassicPiecesChoice(true); };

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find("ButtonReset").GetComponent<ButtonBhv>().EndActionDelegate = ResetDefault;
    }

    private void GhostColorChoice()
    {
        var choiceId = Cache.LastEndActionClickedName.Substring(Cache.LastEndActionClickedName.Length - 1, 1);
        var choiceGameObject = GameObject.Find("Ghost" + choiceId);
        _ghostSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _ghostSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveGhostColor(choiceId);
    }

    private void DasChoice(int das)
    {
        var choiceGameObject = GameObject.Find("Das" + das.ToString("00"));
        _dasSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _dasSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveDas(das);
    }

    private void ArrChoice(int arr)
    {
        var choiceGameObject = GameObject.Find("Arr" + arr);
        _arrSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _arrSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveArr(arr);
    }

    private void RotationPointChoice(bool enable)
    {
        var choiceGameObject = GameObject.Find("RotationPoint" + (enable ? "On" : "Off"));
        _rotationPointSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _rotationPointSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveRotationPoint(enable);
        if (enable)
        {
            _tHell.EnableRotationPoint(true, Instantiator);
            _iHell.EnableRotationPoint(true, Instantiator);
        }
        else
        {
            _tHell.EnableRotationPoint(false, Instantiator);
            _iHell.EnableRotationPoint(false, Instantiator);
        }
    }

    private void ClassicPiecesChoice(bool enable)
    {
        var choiceGameObject = GameObject.Find("ClassicPieces" + (enable ? "On" : "Off"));
        _classicPiecesSelector.transform.position = new Vector3(choiceGameObject.transform.position.x, _classicPiecesSelector.transform.position.y, 0.0f);
        PlayerPrefsHelper.SaveClassicPieces(enable);
        Instantiator.Init();
        var tmpTHell = this.Instantiator.NewPiece("T", "Hell", _tHell.transform.position, true);
        tmpTHell.name = "T-Hell";
        var tmpIHell = this.Instantiator.NewPiece("I", "Hell", _iHell.transform.position, true);
        tmpIHell.name = "I-Hell";
        tmpIHell.transform.position = _iHell.transform.position;
        Destroy(_tHell.gameObject);
        Destroy(_iHell.gameObject);
        _tHell = tmpTHell.GetComponent<Piece>();
        _iHell = tmpIHell.GetComponent<Piece>();
        RotationPointChoice(PlayerPrefsHelper.GetRotationPoint());

        if (enable)
            _unpleasedDev.enabled = true;
        else
            _unpleasedDev.enabled = false;
    }

    public override void PauseOrPrevious()
    {
        GoToPrevious();
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse:true);
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
            PlayerPrefsHelper.SaveGhostColor(Constants.PpGhostPieceColorDefault);
            PlayerPrefsHelper.SaveDas(Constants.PpDasDefault);
            PlayerPrefsHelper.SaveArr(Constants.PpArrDefault);
            Init();
            return result;
        }
    }
}
