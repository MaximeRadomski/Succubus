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
    private int _basePrice = 2;
    private int _totalResources;

    private List<string> _buttonNames = new List<string>() {
    "ButtonLevelUpTattoo",
    "ButtonRemoveATattoo",
    "ButtonTradeResources",
    "ButtonRandomBoost",
    "ButtonGetAHaircut",
    "ButtonCleanPlayfield" };
    private string _tmpTattoosContainerName = "TmpTattoosContainer";

    public void Init(Instantiator instantiator, Action<bool> resumeAction, Character character)
    {
        _run = PlayerPrefsHelper.GetRun();
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _character = character;
        _mainCamera = Helper.GetMainCamera();
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        GameObject.Find(_buttonNames[0]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowLevelUpTattoo(); };
        GameObject.Find(_buttonNames[1]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowRemoveTattoo(); };
        GameObject.Find(_buttonNames[2]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowTradeResources(); };
        GameObject.Find(_buttonNames[3]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowRandomBoost(); };
        GameObject.Find(_buttonNames[4]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowHaircut(); };
        GameObject.Find(_buttonNames[5]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowCleanPlayfield(); };

        UpdateTotalResources();
    }

    private void UnselectAllButtons()
    {
        GameObject.Find(_buttonNames[0]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[1]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[2]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[3]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[4]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        GameObject.Find(_buttonNames[5]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("4.3", "3.2");
        var tmp = GameObject.Find(_tmpTattoosContainerName);
        if (tmp != null)
            Destroy(tmp);
    }

    private void ShowLevelUpTattoo()
    {
        GameObject.Find(_buttonNames[0]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        var tmpTattoosContainer = CloneFillTattoosContainerTemplate();
        tmpTattoosContainer.transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = "common: 2$\nrare: 4$\nlegendary: 6$";
        var tattoos = PlayerPrefsHelper.GetCurrentTattoos();
        foreach (Tattoo tattoo in tattoos)
        {
            var tmpTattooGameObject = tmpTattoosContainer.transform.FindDeepChild("TattooPlaceHolder" + tattoo.BodyPart.GetHashCode().ToString("00"));
            tmpTattooGameObject.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + tattoo.Id.ToString("00"));
            tmpTattooGameObject.GetComponent<ButtonBhv>().EndActionDelegate = LevelUpTattoo;
            tmpTattooGameObject.name = "Tattoo" + tattoo.Id.ToString("00");
            tmpTattooGameObject.transform.parent.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
        }
        Helper.ReinitKeyboardInputs(this);
    }

    private void LevelUpTattoo()
    {
        var bodyPart = (BodyPart)int.Parse(GameObject.Find(Cache.LastEndActionClickedName).transform.parent.name.Substring("BodyPart".Length));
        var clickedTattoo = PlayerPrefsHelper.GetCurrentInkedTattoo(TattoosData.Tattoos[int.Parse(Cache.LastEndActionClickedName.Substring("Tattoo".Length))]);
        var tattooSuffixe = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}";
        if (clickedTattoo.MaxLevel > 1)
        {
            if (clickedTattoo.Level > 1 && clickedTattoo.Level < clickedTattoo.MaxLevel)
                tattooSuffixe += $" +{clickedTattoo.Level - 1}";
            else if (clickedTattoo.Level > 1 && clickedTattoo.Level == clickedTattoo.MaxLevel)
                tattooSuffixe += "Max";
        }
        var upgradable = $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}[{(clickedTattoo.MaxLevel > 1 && !tattooSuffixe.Contains("Max") ? string.Empty : "not ")}upgradable]";
        _instantiator.NewPopupYesNo($"{clickedTattoo.Name} {tattooSuffixe}", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "inked: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + bodyPart.GetDescription().ToLower() + "\n" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + clickedTattoo.GetDescription() + upgradable, upgradable.Contains("not ") ? null : "Cancel", upgradable.Contains("not ") ? "Nope" : "Upgrade", (result) =>
        {
            if (!result
            || (result && upgradable.Contains("not ")))
                return;
            Helper.ReinitKeyboardInputs(this);
            var upgradePrice = (clickedTattoo.Rarity.GetHashCode() + 1) * _basePrice;
            if (_totalResources < upgradePrice)
                _instantiator.NewPopupYesNo("Not Enough", "you don't have enough resources to upgrade this tattoo.", null, "Damn...", null);
            else
            {
                _instantiator.NewPopupYesNo("Upgrade", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}upgrading this tattoo costs:{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}\n{upgradePrice}$", "Cancel", "Pay", null, defaultPositive: true);
            }
        }, defaultPositive: true);
    }

    private void ShowRemoveTattoo()
    {
        GameObject.Find(_buttonNames[1]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private GameObject CloneFillTattoosContainerTemplate()
    {
        var tmp = Instantiate(GameObject.Find("TattoosContainerTemplate"));
        tmp.transform.SetParent(this.gameObject.transform);
        tmp.name = _tmpTattoosContainerName;
        tmp.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        return tmp;
    }

    private void ShowTradeResources()
    {
        GameObject.Find(_buttonNames[2]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void ShowRandomBoost()
    {
        GameObject.Find(_buttonNames[3]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void ShowHaircut()
    {
        GameObject.Find(_buttonNames[4]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }
    private void ShowCleanPlayfield()
    {
        GameObject.Find(_buttonNames[5]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
    }

    private void UpdateTotalResources()
    {
        var resources = PlayerPrefsHelper.GetTotalResources();
        var amount = 0;
        foreach (var tmpAmount in resources)
            amount += tmpAmount;
        _totalResources = amount;
        GameObject.Find("TotalResources").GetComponent<TMPro.TextMeshPro>().text = $"total resources: {amount}$";
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
