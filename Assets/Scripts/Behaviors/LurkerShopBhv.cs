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
    private GameObject _haircutContainer;
    private GameObject _cleanContainer;
    private GameObject _selectorResourcesLeft;
    private GameObject _selectorResourcesRight;
    private GameObject _boostButton;
    private GameObject _playfieldContainer;
    private Transform[,] _grid;
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

        GameObject.Find("ButtonTradeUpLeft").GetComponent<ButtonBhv>().EndActionDelegate = () => { AlterLeftAmount(1); };
        GameObject.Find("ButtonTradeDownLeft").GetComponent<ButtonBhv>().EndActionDelegate = () => { AlterLeftAmount(-1); };

        GameObject.Find("Resource0").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceLeft(0); };
        GameObject.Find("Resource1").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceLeft(1); };
        GameObject.Find("Resource2").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceLeft(2); };
        GameObject.Find("Resource3").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceRight(3); };
        GameObject.Find("Resource4").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceRight(4); };
        GameObject.Find("Resource5").GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectResourceRight(5); };

        GameObject.Find("HaircutHell").GetComponent<ButtonBhv>().EndActionDelegate = () => { Haircut(Realm.Hell); };
        GameObject.Find("HaircutEarth").GetComponent<ButtonBhv>().EndActionDelegate = () => { Haircut(Realm.Earth); };
        GameObject.Find("HaircutHeaven").GetComponent<ButtonBhv>().EndActionDelegate = () => { Haircut(Realm.Heaven); };

        (_boostButton = GameObject.Find("BoostButton")).GetComponent<ButtonBhv>().BeginActionDelegate = PushRandomBoost;
        (_boostButton = GameObject.Find("BoostButton")).GetComponent<ButtonBhv>().EndActionDelegate = RandomBoost;

        _amountLeftText = GameObject.Find("AmountTradeLeft").GetComponent<TMPro.TextMeshPro>();
        _amountRightText = GameObject.Find("AmountTradeRight").GetComponent<TMPro.TextMeshPro>();

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
        _haircutContainer = GameObject.Find("HaircutContainer");
        _cleanContainer = GameObject.Find("CleanContainer");
        _playfieldContainer = GameObject.Find("PlayfieldContainer");
        _grid = new Transform[Constants.PlayFieldWidth, Constants.PlayFieldHeight];
        ApplyLastFightPlayField();

        GameObject.Find("ButtonValidateClean").GetComponent<ButtonBhv>().EndActionDelegate = CleanPlayfield;

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
        _haircutContainer.transform.position = new Vector3(0.0f, -50.0f, 0.0f);
        _cleanContainer.transform.position = new Vector3(50.0f, -50.0f, 0.0f);

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
        {
            if (max >= 1)
                _amountLeft = 1;
            else
                _amountLeft = 0;
        }
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
            PlayerPrefsHelper.AlterTotalResource(_resourceSelectedRight - 3, _amountRight);
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
        GameObject.Find("RandomBoostPrice").GetComponent<TMPro.TextMeshPro>().text = $"{PlayerPrefsHelper.GetBoostButtonPrice()}$";
    }

    private void PushRandomBoost()
    {
        _boostButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/BoostButton_1");
    }

    private void RandomBoost()
    {
        _boostButton.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/BoostButton_0");
        var price = PlayerPrefsHelper.GetBoostButtonPrice();
        if (this._totalResources < price)
        {
            this._instantiator.NewPopupYesNo("Cheater!", "don't try to touch the big button if you don't have big money!", null, "Damn!", null);
            return;
        }
        SpendResources(price);
        PlayerPrefsHelper.SaveBoostButtonPrice(++price);
        UpdateRandomBoostPrice();
        var nothingId = UnityEngine.Random.Range(0, 3);
        if (nothingId == 0)
        {
            this._instantiator.NewPopupYesNo("Oops!", "nothing! nada! try again!", null, "Damn!", null);
            return;
        }
        var randId = UnityEngine.Random.Range(0, 3);
        switch (randId)
        {
            case 0:
                this._character.DamageFlatBonus += 1;
                this._instantiator.NewPopupYesNo("Damage Up", "+1 damage.", null, "Noice!", null);
                break;
            case 1:
                this._character.CritChancePercent += 4;
                this._instantiator.NewPopupYesNo("Crit Up", "+4% crit chance.", null, "Noice!", null);
                break;
            case 2:
                this._character.EnemyMaxCooldownMalus += 1;
                this._instantiator.NewPopupYesNo("Cooldown Up", "+1 second to opponents cooldowns.", null, "Noice!", null);
                break;
        }
        PlayerPrefsHelper.SaveRunCharacter(_character);
    }

    private void ShowHaircut()
    {
        GameObject.Find(_buttonNames[4]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        _haircutContainer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void Haircut(Realm realm)
    {
        var price = 8;
        if (this._totalResources < price)
        {
            this._instantiator.NewPopupYesNo("Nope!", "you don't have enough resources to get a haircut.", null, "Damn...", null);
            return;
        }
        else if (_character.Realm == realm)
        {
            this._instantiator.NewPopupYesNo("Ahem...", "you already have this haircut.", null, "Damn...", null);
            return;
        }
        SpendResources(price);
        _character.Realm = realm;
        PlayerPrefsHelper.SaveRunCharacter(_character);
        this._instantiator.NewPopupYesNo($"{realm}ish", $"you now look like a fervent worshiper of {realm.ToString().ToLower()}.", null, "Neat!", null);
    }

    private void ShowCleanPlayfield()
    {
        GameObject.Find(_buttonNames[5]).transform.Find("Text").GetComponent<TMPro.TextMeshPro>().ReplaceText("3.2", "4.3");
        _cleanContainer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    private void CleanPlayfield()
    {
        var price = 1;
        if (this._totalResources < price)
        {
            this._instantiator.NewPopupYesNo("Nope!", "you don't have enough resources to get a nice clean.", null, "Damn...", null);
            return;
        }
        SpendResources(2);
        CleanRows();
    }

    public void ApplyLastFightPlayField()
    {
        var lastPlayField = PlayerPrefsHelper.GetLastFightPlayField();
        if (lastPlayField == null)
            return;
        foreach (var cell in lastPlayField)
        {
            var remainingPiece = this._instantiator.NewPiece(AttackType.WasteRow.ToString(), _character.Realm.ToString(), _playfieldContainer.transform.position + new Vector3(cell.Item1, cell.Item2, 0.0f));
            remainingPiece.transform.SetParent(_playfieldContainer.gameObject.transform);
            _grid[cell.Item1, cell.Item2] = remainingPiece.transform.GetChild(0);
            foreach (Transform bloc in remainingPiece.transform)
            {
                var spr = bloc.GetComponent<SpriteRenderer>();
                if (spr != null)
                {
                    spr.sortingLayerName = "MenuTop";
                    spr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }
        }
        ClearLineSpace();
    }

    private void SaveLastFightPlayField()
    {
        var remainingBlocks = string.Empty;
        foreach (Transform piece in _playfieldContainer.transform)
        {
            if (piece.childCount > 0)
            {
                var x = Mathf.RoundToInt(piece.transform.localPosition.x + transform.GetChild(0).localPosition.x);
                var y = Mathf.RoundToInt(piece.transform.localPosition.y + transform.GetChild(0).localPosition.y);
                var pos = $"{x}-{y};";
                if (remainingBlocks.Contains(pos))
                    continue;
                remainingBlocks += pos;
            }
        }
        PlayerPrefsHelper.SaveLastFightPlayField(remainingBlocks);
    }

    private void CleanRows()
    {
        int start = 0;
        int end = start + (4 - 1);
        for (int y = start; y <= end; ++y)
            DeleteLine(y);
        ClearLineSpace();
        InvokeNextFrame(SaveLastFightPlayField);
    }

    private void DeleteLine(int y)
    {
        for (int x = 0; x < Constants.PlayFieldWidth; ++x)
        {
            if (_grid[x, y] == null)
                continue;
            this._instantiator.NewFadeBlock(_character.Realm, _grid[x, y].transform.position, 5, 0);
            Destroy(_grid[x, y].gameObject);
            _grid[x, y] = null;
        }
    }

    public void ClearLineSpace()
    {
        int highestBlock = Cache.PlayFieldMinHeight - 1;
        for (int y = Cache.PlayFieldMinHeight; y < Constants.PlayFieldHeight; ++y)
        {
            if (y == Cache.PlayFieldMinHeight)
                highestBlock = GetHighestBlock();
            if (y > highestBlock || highestBlock == Cache.PlayFieldMinHeight - 1)
                break;
            if (HasFullLineSpace(y))
            {
                DropAllAboveLines(y);
                y = Cache.PlayFieldMinHeight - 1;
            }
        }
        foreach (Transform child in _playfieldContainer.transform)
        {
            if (child.childCount == 0 && child.GetComponent<Piece>() != null)
                Destroy(child.gameObject);
        }
    }

    public int GetHighestBlock()
    {
        for (int y = Constants.PlayFieldHeight - 1; y >= Cache.PlayFieldMinHeight; --y)
        {
            if (!HasFullLineSpace(y))
                return y;
        }
        return Cache.PlayFieldMinHeight - 1;
    }

    private bool HasFullLineSpace(int y)
    {
        for (int x = 0; x < Constants.PlayFieldWidth; ++x)
        {
            if (_grid[x, y] != null)
                return false;
        }
        return true;
    }

    private void DropAllAboveLines(int underY)
    {
        for (int y = underY + 1; y < Constants.PlayFieldHeight; ++y)
        {
            for (int x = 0; x < Constants.PlayFieldWidth; ++x)
            {
                if (_grid[x, y] != null)
                {
                    _grid[x, y - 1] = _grid[x, y];
                    _grid[x, y] = null;
                    _grid[x, y - 1].transform.position += new Vector3(0.0f, -1.0f, 0.0f);
                }
            }
        }
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
                PlayerPrefsHelper.AlterTotalResource(i, -amountSpent);
                runResources[i] -= amountSpent;
                totalResources[i] -= amountSpent;
                amountSpent = 0;
                break;
            }
            else
            {
                int amountResource = runResources[i];
                amountSpent -= amountResource;
                _run.AlterResource(i, -amountResource);
                PlayerPrefsHelper.AlterTotalResource(i, -amountResource);
                runResources[i] -= amountResource;
                totalResources[i] -= amountResource;
            }
        }

        for (int i = min; i <= max; ++i)
        {
            if (amountSpent <= 0)
                break;
            if (totalResources[i] - amountSpent >= 0)
            {
                PlayerPrefsHelper.AlterTotalResource(i, -amountSpent);
                totalResources[i] -= amountSpent;
                amountSpent = 0;
                break;
            }
            else
            {
                int amountResource = totalResources[i];
                amountSpent -= amountResource;
                PlayerPrefsHelper.AlterTotalResource(i, -amountResource);
                totalResources[i] -= amountResource;
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
