using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LurkerShopBhv : PopupBhv
{
    private Camera _mainCamera;
    private Action<bool> _resumeAction;
    private Instantiator _instantiator;
    
    private Run _run;
    private Character _character;

    private List<string> _buttonNames = new List<string>() {
    "ButtonLevelUpTattoo",
    "ButtonRemoveATattoo",
    "ButtonTradeResources",
    "ButtonRandomBoost",
    "ButtonGetAHaircut",
    "ButtonCleanPlayfield" };

    public void Init(Instantiator instantiator, Action<bool> resumeAction, Character character)
    {
        _run = PlayerPrefsHelper.GetRun();
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _character = character;
        _mainCamera = Helper.GetMainCamera();
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        GameObject.Find(_buttonNames[0]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); LevelUpTattoo(); };
        GameObject.Find(_buttonNames[1]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); RemoveTattoo(); };
        GameObject.Find(_buttonNames[2]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); TradeResources(); };
        GameObject.Find(_buttonNames[3]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); RandomBoost(); };
        GameObject.Find(_buttonNames[4]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); Haircut(); };
        GameObject.Find(_buttonNames[5]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); CleanPlayfield(); };
    }

    private void UnselectAllButtons()
    {
        GameObject.Find(_buttonNames[0]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[1]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[2]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[3]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[4]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[5]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
    }

    private void LevelUpTattoo()
    {
        GameObject.Find(_buttonNames[0]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void RemoveTattoo()
    {
        GameObject.Find(_buttonNames[1]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void TradeResources()
    {
        GameObject.Find(_buttonNames[2]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void RandomBoost()
    {
        GameObject.Find(_buttonNames[3]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void Haircut()
    {
        GameObject.Find(_buttonNames[4]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }
    private void CleanPlayfield()
    {
        GameObject.Find(_buttonNames[5]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void Resume()
    {
        Cache.DecreaseInputLayer();
        _resumeAction?.Invoke(true);
        Destroy(this.gameObject);
    }

    public override void ExitPopup()
    {
        Resume();
    }
}
