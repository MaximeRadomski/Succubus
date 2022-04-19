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
    private GameObject _menuSelector;
    private GameObject _tradeContainer;
    private GameObject _randomBoostContainer;
    private GameObject _selectorResourcesLeft;
    private GameObject _selectorResourcesRight;
    private GameObject _boostButton;
    private TMPro.TextMeshPro _amountLeftText;
    private TMPro.TextMeshPro _amountRightText;
    private int _amountLeft;
    private int _amountRight;
    private Vector3 _menuResetPos;
    private int _basePrice = 2;
    private int _totalResources;
    private int _tradingMarkerId;
    private int _resourceSelectedLeft;
    private int _resourceSelectedRight;
    private string _currentResourcesStr => $"\n\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}you currently have: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{_totalResources}$";
    private List<List<float>> _tradingMarketRate = new List<List<float>>
    {
        new List<float> { 0.35f, 0.40f, 0.70f},
        new List<float> { 0.65f, 0.30f, 0.20f},
        new List<float> { 0.35f, 0.65f, 0.50f},
        new List<float> { 0.20f, 0.75f, 0.25f},
        new List<float> { 0.50f, 0.40f, 0.60f},
        new List<float> { 0.50f, 0.25f, 0.20f},
    };

    private List<string> _buttonNames = new List<string>() {
    "ButtonLevelUpTattoo",
    "ButtonRemoveATattoo",
    "ButtonTradeResources",
    "ButtonRandomBoost",
    "ButtonGetAHaircut",
    "ButtonCleanPlayfield" };
    private string _tmpTattoosContainerName = "TmpTattoosContainer";

    private SoundControlerBhv _soundControler;
    private int _idTattooSound;
    private int _idRemoveSound;

    public void Init(Instantiator instantiator, Action<bool> resumeAction, Character character)
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _run = PlayerPrefsHelper.GetRun();
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _character = character;
        _mainCamera = Helper.GetMainCamera();
        _menuSelector = GameObject.Find("MenuSelector");
        _menuResetPos = new Vector3(0.0f, 2.0f, 0.0f);
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);
        _tradingMarkerId = PlayerPrefsHelper.GetTradingMarket();

        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        GameObject.Find("ButtonResources").GetComponent<ButtonBhv>().EndActionDelegate = ResourcesInfo;
        GameObject.Find(_buttonNames[0]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowLevelUpTattoo(); };
        GameObject.Find(_buttonNames[1]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowRemoveTattoo(); };
        GameObject.Find(_buttonNames[2]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowTradeResources(); };
        GameObject.Find(_buttonNames[3]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowRandomBoost(); };
        GameObject.Find(_buttonNames[4]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowHaircut(); };
        GameObject.Find(_buttonNames[5]).GetComponent<ButtonBhv>().EndActionDelegate = () => { UnselectAllButtons(); ShowCleanPlayfield(); };

        GameObject.Find("ButtonUpLeft").GetComponent<ButtonBhv>().EndActionDelegate = () => { AlterLeftAmount(1); };
        GameObject.Find("ButtonDownLeft").GetComponent<ButtonBhv>().EndActionDelegate = () => { AlterLeftAmount(-1); };

        GameObject.Find("Resource0").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceLeft(0); };
        GameObject.Find("Resource1").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceLeft(1); };
        GameObject.Find("Resource2").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceLeft(2); };
        GameObject.Find("Resource3").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceRight(3); };
        GameObject.Find("Resource4").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceRight(4); };
        GameObject.Find("Resource5").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceRight(5); };

        (_boostButton = GameObject.Find("BoostButton")).GetComponent<ButtonBhv>().BeginActionDelegate = PushRandomBoost;
        (_boostButton = GameObject.Find("BoostButton")).GetComponent<ButtonBhv>().EndActionDelegate = RandomBoost;

        _amountLeftText = GameObject.Find("AmountLeft").GetComponent<TMPro.TextMeshPro>();
        _amountRightText = GameObject.Find("AmountRight").GetComponent<TMPro.TextMeshPro>();

        _selectorResourcesLeft = GameObject.Find("SelectorResourcesLeft");
        _selectorResourcesRight = GameObject.Find("SelectorResourcesRight");

        SelectResourceRight(4);
        SelectResourceLeft(0);

        GameObject.Find("ButtonValidateTrade").GetComponent<ButtonBhv>().EndActionDelegate = ValidateTrade;
        if (PlayerPrefsHelper.GetHasDoneTrading())
            GameObject.Find("ButtonValidateTrade").SetActive(false);

        _idTattooSound = _soundControler.SetSound("TattooSound");
        _idRemoveSound = _soundControler.SetSound("Jackhammer");

        _tradeContainer = GameObject.Find("TradeContainer");
        _randomBoostContainer = GameObject.Find("RandomBoostContainer");

        UpdateTotalResources();
        _menuSelector.transform.position = new Vector3(-10.0f, 20.0f, _menuSelector.transform.position.z);
        ShowLevelUpTattoo();
    }

    /// <summary>
    ///      run: 2$\t+ 3$\t+ 0$\n
    /// previous: 2$\t+ 3$\t+ 0$\n
    ///    total: 4$\t+ 6$\t+ 0$
    /// </summary>

    private void ResourcesInfo()
    {
        var runAmount = _run.GetRunResources();
        var allAmount = PlayerPrefsHelper.GetTotalResources();
        var previous0 = allAmount[0] - runAmount[0]; previous0 = previous0 < 0 ? 0 : previous0;
        var previous1 = allAmount[1] - runAmount[1]; previous1 = previous1 < 0 ? 0 : previous1;
        var previous2 = allAmount[2] - runAmount[2]; previous2 = previous2 < 0 ? 0 : previous2;
        var totalAmount = 0;
        foreach (var tmpAmount in allAmount)
            totalAmount += tmpAmount;
        var line1 = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}     run: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{runAmount[0]}${Constants.MaterialEnd} + {Constants.GetMaterial(Realm.Earth, TextType.succubus3x5, TextCode.c43)}{runAmount[1]}${Constants.MaterialEnd} + {Constants.GetMaterial(Realm.Heaven, TextType.succubus3x5, TextCode.c43)}{runAmount[2]}${Constants.MaterialEnd}\n";
        var line2 = $"previous: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{previous0}${Constants.MaterialEnd} + {Constants.GetMaterial(Realm.Earth, TextType.succubus3x5, TextCode.c43)}{previous1}${Constants.MaterialEnd} + {Constants.GetMaterial(Realm.Heaven, TextType.succubus3x5, TextCode.c43)}{previous2}${Constants.MaterialEnd}\n";
        var line3 = $"   {Constants.MaterialEnd}total: {totalAmount} resources";
        this._instantiator.NewPopupYesNo("Resources Info", $"{line1}{line2}{line3}", null, "Ok", null);
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
        _tradeContainer.transform.position = new Vector3(-51.0f, 50.0f, 0.0f);
        _randomBoostContainer.transform.position = new Vector3(-51.0f, -50.0f, 0.0f);

    }

    private void ShowLevelUpTattoo()
    {
        GameObject.Find(_buttonNames[0]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        var tmpTattoosContainer = CloneFillTattoosContainerTemplate();
        tmpTattoosContainer.transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = $"common: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}2${Constants.MaterialEnd}\nrare: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}4${Constants.MaterialEnd}\nlegendary: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}6${Constants.MaterialEnd}";
        var tattoos = PlayerPrefsHelper.GetCurrentTattoos();
        foreach (Tattoo tattoo in tattoos)
        {
            var tmpTattooGameObject = tmpTattoosContainer.transform.FindDeepChild("TattooPlaceHolder" + tattoo.BodyPart.GetHashCode().ToString("00"));
            tmpTattooGameObject.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + tattoo.Id.ToString("00"));
            tmpTattooGameObject.GetComponent<ButtonBhv>().EndActionDelegate = LevelUpTattoo;
            tmpTattooGameObject.name = "Tattoo" + tattoo.Id.ToString("00");
            tmpTattooGameObject.transform.parent.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
        }
        Helper.ReinitKeyboardInputs(this, _menuSelector?.transform.position + _menuResetPos);
    }

    private void LevelUpTattoo()
    {
        var bodyPart = (BodyPart)int.Parse(GameObject.Find(Cache.LastEndActionClickedName).transform.parent.name.Substring("BodyPart".Length));
        var clickedTattoo = PlayerPrefsHelper.GetCurrentInkedTattoo(TattoosData.Tattoos[int.Parse(Cache.LastEndActionClickedName.Substring("Tattoo".Length))]);
        var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + clickedTattoo.Id);
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
                _instantiator.NewPopupYesNo("Upgrade", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}upgrading this tattoo costs:{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}\n{upgradePrice}${_currentResourcesStr}", "Cancel", "Pay", (result) =>
                {
                    if (!result)
                        return;

                    Helper.ReinitKeyboardInputs(this);
                    _soundControler.PlaySound(_idTattooSound);
                    SpendResources(upgradePrice);
                    PlayerPrefsHelper.AddTattoo(clickedTattoo.Name);
                    _instantiator.NewPopupYesNo("Tattoo Upgrade", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}your {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{clickedTattoo.Name.ToLower()}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} power has been increased.", null, "Noice", null, sprite);
                }, sprite, true);
            }
        }, sprite, true);
    }

    private void ShowRemoveTattoo()
    {
        GameObject.Find(_buttonNames[1]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        var tmpTattoosContainer = CloneFillTattoosContainerTemplate();
        tmpTattoosContainer.transform.Find("Title").GetComponent<TMPro.TextMeshPro>().text = $"\nremoval: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}2${Constants.MaterialEnd}";
        var tattoos = PlayerPrefsHelper.GetCurrentTattoos();
        foreach (Tattoo tattoo in tattoos)
        {
            var tmpTattooGameObject = tmpTattoosContainer.transform.FindDeepChild("TattooPlaceHolder" + tattoo.BodyPart.GetHashCode().ToString("00"));
            tmpTattooGameObject.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + tattoo.Id.ToString("00"));
            tmpTattooGameObject.GetComponent<ButtonBhv>().EndActionDelegate = RemoveTattoo;
            tmpTattooGameObject.name = "Tattoo" + tattoo.Id.ToString("00");
            tmpTattooGameObject.transform.parent.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
        }
        Helper.ReinitKeyboardInputs(this, _menuSelector?.transform.position + _menuResetPos);
    }

    private void RemoveTattoo()
    {
        var bodyPart = (BodyPart)int.Parse(GameObject.Find(Cache.LastEndActionClickedName).transform.parent.name.Substring("BodyPart".Length));
        var clickedTattoo = PlayerPrefsHelper.GetCurrentInkedTattoo(TattoosData.Tattoos[int.Parse(Cache.LastEndActionClickedName.Substring("Tattoo".Length))]);
        var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + clickedTattoo.Id);
        var tattooSuffixe = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}";
        if (clickedTattoo.MaxLevel > 1)
        {
            if (clickedTattoo.Level > 1 && clickedTattoo.Level < clickedTattoo.MaxLevel)
                tattooSuffixe += $" +{clickedTattoo.Level - 1}";
            else if (clickedTattoo.Level > 1 && clickedTattoo.Level == clickedTattoo.MaxLevel)
                tattooSuffixe += "Max";
        }
        var upgradable = $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}[{(clickedTattoo.MaxLevel > 1 && !tattooSuffixe.Contains("Max") ? string.Empty : "not ")}upgradable]";
        var removePrice = _basePrice;
        if (_totalResources < removePrice)
            _instantiator.NewPopupYesNo("Not Enough", "you don't have enough resources to remove a tattoo.", null, "Damn...", null);
        else
            _instantiator.NewPopupYesNo($"{clickedTattoo.Name} {tattooSuffixe}", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "inked: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + bodyPart.GetDescription().ToLower() + "\n" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + clickedTattoo.GetDescription() + upgradable, "Cancel", "Remove", (result) =>
            {
                if (!result)
                    return;
                Helper.ReinitKeyboardInputs(this);
                _soundControler.PlaySound(_idRemoveSound);
                SpendResources(removePrice);
                PlayerPrefsHelper.RemoveTattoo(clickedTattoo);
                _instantiator.NewPopupYesNo("Tattoo Removal", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}your {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{clickedTattoo.Name.ToLower()}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} tattoo has been removed.", null, "Ouch", (result) =>
                {
                    UnselectAllButtons();
                    ShowRemoveTattoo();
                }, sprite);
            }, sprite, true);
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
        GameObject.Find("TradeFrame").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/TradeFrame_{_tradingMarkerId}");
        GameObject.Find(_buttonNames[2]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        _tradeContainer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void SelectResourceLeft(int id)
    {
        _resourceSelectedLeft = id;
        _selectorResourcesLeft.transform.position = GameObject.Find($"Resource{id}").transform.position;
        var maxResource = PlayerPrefsHelper.GetTotalResources()[id];
        if (maxResource > 10)
            maxResource = 10;
        _amountLeft = maxResource;
        _amountLeftText.text = $"{Constants.GetMaterial((Realm)id, TextType.succubus3x5, TextCode.c43)}{maxResource}";
        if (_resourceSelectedRight == id + 3)
        {
            var rightId = id + 4;
            if (rightId >= 6)
                rightId = 3;
            SelectResourceRight(rightId);
        }
        UpdateRateResources();
    }

    private void SelectResourceRight(int id)
    {
        _resourceSelectedRight = id;
        _selectorResourcesRight.transform.position = GameObject.Find($"Resource{id}").transform.position;
        if (_resourceSelectedLeft == id - 3)
        {
            var rightId = id - 2;
            if (rightId == 3)
                rightId = 0;
            SelectResourceLeft(rightId);
        }
        UpdateRateResources();
    }

    private void AlterLeftAmount(int add)
    {
        var max = PlayerPrefsHelper.GetTotalResources()[_resourceSelectedLeft];
        _amountLeft += add;
        if (_amountLeft > 10)
            _amountLeft = 10;
        else if (_amountLeft > max)
            _amountLeft = max;
        else if (_amountLeft < 1)
            _amountLeft = 1;
        _amountLeftText.text = $"{Constants.GetMaterial((Realm)_resourceSelectedLeft, TextType.succubus3x5, TextCode.c43)}{_amountLeft}";
        UpdateRateResources();
    }

    private void UpdateRateResources()
    {
        var rate1 = _tradingMarketRate[_tradingMarkerId][_resourceSelectedLeft];
        var rate2 = _tradingMarketRate[_tradingMarkerId][_resourceSelectedRight - 3];
        float rateResources = ((1.0f - Mathf.Abs(rate1 - rate2)) * _amountLeft);
        int intRateResources = (int)Math.Round(rateResources);
        _amountRightText.text = $"{Constants.GetMaterial((Realm)(_resourceSelectedRight - 3), TextType.succubus3x5, TextCode.c43)}{intRateResources}";
        _amountRight = intRateResources;
    }

    private void ValidateTrade()
    {
        this._instantiator.NewPopupYesNo("Trading", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}trade {Constants.GetMaterial((Realm)_resourceSelectedLeft, TextType.succubus3x5, TextCode.c43)}{_amountLeft}${Constants.MaterialEnd} for {Constants.GetMaterial((Realm)_resourceSelectedRight - 3, TextType.succubus3x5, TextCode.c43)}{_amountRight}${Constants.MaterialEnd} ?\n\nonly one trade can be done per lurker shop.", "Cancel", "Trade", (result) =>
        {
            if (!result)
                return;
            PlayerPrefsHelper.SaveHasDoneTrading(true);
            GameObject.Find("ButtonValidateTrade").SetActive(false);
            PlayerPrefsHelper.AlterResource(_resourceSelectedRight - 3, _amountRight);
            SpendResources(_amountLeft, _resourceSelectedLeft);
            Helper.ReinitKeyboardInputs(this);
        }, defaultPositive:true);
    }

    private void ShowRandomBoost()
    {
        GameObject.Find(_buttonNames[3]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        UpdateRandomBoostPrice();
        _randomBoostContainer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void UpdateRandomBoostPrice()
    {
        GameObject.Find("RandomBoostPrice").GetComponent<TMPro.TextMeshPro>().text = $"{PlayerPrefsHelper.GetPpBoostButtonPrice()}$";
    }

    private void PushRandomBoost()
    {
        _boostButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/BoostButton_1");
    }

    private void RandomBoost()
    {
        Helper.ExecuteAfterDelay(0.15f, () =>
        {
            _boostButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/BoostButton_0");
        });
        var price = PlayerPrefsHelper.GetPpBoostButtonPrice();
        if (this._totalResources < price)
        {
            this._instantiator.NewPopupYesNo("Cheater!", "don't try to touch the big button if you don't have big money!", null, "Damn!", null);
            return;
        }
        var nothingId = UnityEngine.Random.Range(0, 3);
        if (nothingId == 0)
        {
            this._instantiator.NewPopupYesNo("Oops!", "nothing! nada! try again!", null, "Damn!", null);
            return;
        }
        var randId = UnityEngine.Random.Range(0, 6);
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
        _totalResources = GetTotalResourcesAmount();
        GameObject.Find("TotalResources").GetComponent<TMPro.TextMeshPro>().text = $"total resources: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{_totalResources}$";
    }

    private int GetRunResourcesAmount()
    {
        var resources = _run.GetRunResources();
        var amount = 0;
        foreach (var tmpAmount in resources)
            amount += tmpAmount;
        return amount;
    }

    private int GetTotalResourcesAmount()
    {
        var resources = PlayerPrefsHelper.GetTotalResources();
        var amount = 0;
        foreach (var tmpAmount in resources)
            amount += tmpAmount;
        return amount;
    }

    private bool SpendResources(int amountSpent, int idResource = -1)
    {
        var runResources = _run.GetRunResources();
        var totalResources = PlayerPrefsHelper.GetTotalResources();
        var min = 0;
        var max = 2;
        if (idResource != -1)
        {
            min = idResource;
            max = idResource;
        }
        for (int i = min; i <= max; ++i)
        {
            if (amountSpent <= 0)
                break;
            if (runResources[i] - amountSpent >= 0)
            {
                _run.AlterResource(i, -amountSpent);
                PlayerPrefsHelper.AlterResource(i, -amountSpent);
                runResources[i] -= amountSpent;
                totalResources[i] -= amountSpent;
                amountSpent = 0;
                break;
            }
            else
            {
                amountSpent -= runResources[i];
                _run.AlterResource(i, -runResources[i]);
                PlayerPrefsHelper.AlterResource(i, -runResources[i]);
                runResources[i] -= runResources[i];
                totalResources[i] -= runResources[i];
            }
        }

        for (int i = min; i <= max; ++i)
        {
            if (amountSpent <= 0)
                break;
            if (totalResources[i] - amountSpent >= 0)
            {
                PlayerPrefsHelper.AlterResource(i, -amountSpent);
                totalResources[i] -= amountSpent;
                amountSpent = 0;
                break;
            }
            else
            {
                amountSpent -= totalResources[i];
                PlayerPrefsHelper.AlterResource(i, -totalResources[i]);
                totalResources[i] -= totalResources[i];
            }
        }

        PlayerPrefsHelper.SaveRun(_run);
        UpdateTotalResources();
        if (amountSpent > 0)
            return false;
        return true;
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
