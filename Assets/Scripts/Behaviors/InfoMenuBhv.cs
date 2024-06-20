using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class InfoMenuBhv : PopupBhv
{
    private Camera _mainCamera;
    private Action<bool> _resumeAction;
    private Instantiator _instantiator;
    private bool _isHorizontal;
    private GameObject _characterFrame;
    private GameObject _fightFrame;
    private GameObject _characterTab;
    private GameObject _fightTab;
    private Item _characterItem;
    private Run _run;
    private Realm _currentRealm;
    private SceneBhv _currentScene;
    private string _attackDetails;

    private Character _character;
    private Opponent _opponent;

    private float _buttonOnY;
    private float _buttonOffY;

    public void Init(Instantiator instantiator, Action<bool> resumeAction, Character character, Opponent opponent, bool isHorizontal)
    {
        _run = PlayerPrefsHelper.GetRun();
        _currentRealm = Realm.Hell;
        if (_run != null)
            _currentRealm = _run.CurrentRealm;
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _character = character;
        _opponent = opponent;
        _isHorizontal = isHorizontal;
        _buttonOnY = -13.1582f;
        _buttonOffY = -12.3f;
#if UNITY_ANDROID
        if (_isHorizontal)
        {
            if (Cache.HorizontalCameraInitialPosition == null)
            {
                Cache.HorizontalCameraInitialPosition = Camera.main.transform.position;
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Cache.HorizontalCameraInitialPosition.Value.z);
            }
            if (Cache.HorizontalCameraInitialRotation == null)
            {
                Cache.HorizontalCameraInitialRotation = Camera.main.transform.rotation;
                Camera.main.transform.rotation = transform.rotation;
            }
        }
#endif
        _mainCamera = Helper.GetMainCamera();
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        (_characterTab = GameObject.Find("CharacterTab")).GetComponent<ButtonBhv>().EndActionDelegate = ShowCharacter;
        (_fightTab = GameObject.Find("FightTab")).GetComponent<ButtonBhv>().EndActionDelegate = ShowOpponent;
        _characterFrame = GameObject.Find("CharacterFrame");
        _fightFrame = GameObject.Find("FightFrame");
        InitCharacterFrame(character);
        InitOpponentFrame(opponent);
        if (opponent == null)
            ShowCharacter();
        else
            ShowOpponent();
        GetScene();
        _currentScene.Paused = true;
    }

    private void GetScene()
    {
        _currentScene = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
    }

    private void InitCharacterFrame(Character character)
    {
        _characterFrame.transform.Find("CharacterName").GetComponent<TMPro.TextMeshPro>().text = character.Name + " - " + character.Kind;
        GetAttackDetailsString();
        var characterAttack = _characterFrame.transform.Find("CharacterAttack");
        characterAttack.GetComponent<TMPro.TextMeshPro>().text = "attack: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + character.GetAttackNoBoost();        
        characterAttack.GetComponent<ButtonBhv>().EndActionDelegate = CharacterAttack;
        if (_attackDetails.CountChar('\n') > 3 && !PlayerPrefsHelper.GetHasClickedOnAttackDetails())
            _instantiator.NewClickMe(characterAttack.position, _characterFrame.transform);
        _characterFrame.transform.Find("CharacterCooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + character.Cooldown;
        _characterFrame.transform.Find("CharacterSpecial").GetComponent<TMPro.TextMeshPro>().text = "special: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + character.SpecialName.ToLower() + ":\n" + character.SpecialDescription;
        _characterFrame.transform.Find("CharacterRealm").GetComponent<TMPro.TextMeshPro>().text = "realm: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + character.Realm.ToString().ToLower() + ":\n" + character.Realm.GetDescription().ToLower();
        _characterFrame.transform.Find("ButtonCharacter").GetComponent<ButtonBhv>().EndActionDelegate = CharacterLore;
        _characterFrame.transform.Find("ButtonCharacter").GetComponent<SpriteRenderer>().sprite = Helper.GetCharacterSkin(character.Id, character.SkinId);
        if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
        {
            _characterItem = ItemsData.GetItemFromName(ItemsData.Items[2]);
            _characterFrame.transform.Find("ButtonItem").GetComponent<ButtonBhv>().EndActionDelegate = ItemInfo;
            _characterFrame.transform.Find("ButtonItem").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + _characterItem.Id);
        }
        else
        {
            if ((_characterItem = PlayerPrefsHelper.GetCurrentItem()) != null)
            {
                _characterFrame.transform.Find("ButtonItem").GetComponent<ButtonBhv>().EndActionDelegate = ItemInfo;
                _characterFrame.transform.Find("ButtonItem").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + _characterItem.Id);
            }
            else
                _characterFrame.transform.Find("ButtonItem").gameObject.SetActive(false);
            var tattoos = PlayerPrefsHelper.GetCurrentTattoos();
            foreach (Tattoo tattoo in tattoos)
            {
                var tmpTattooGameObject = GameObject.Find("TattooPlaceHolder" + ((int)tattoo.BodyPart).ToString("00"));
                tmpTattooGameObject.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + tattoo.Id.ToString("00"));
                tmpTattooGameObject.GetComponent<ButtonBhv>().EndActionDelegate = TattooInfo;
                tmpTattooGameObject.name = "Tattoo" + tattoo.Id.ToString("00");
                tmpTattooGameObject.transform.parent.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
            }
        }
    }

    private void ItemInfo()
    {
        var cooldown = _characterItem.Cooldown >= 0 ? _characterItem.Cooldown.ToString() : null;
        _instantiator.NewPopupYesNo(_characterItem.Name,
            $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{_characterItem.GetDescription()}" + (cooldown != null ?
            ($"\n---\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}cooldown: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{cooldown}{Constants.MaterialEnd}")
            : ""),
            null, "Ok", null);
    }

    private void TattooInfo()
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
        var upgradable = clickedTattoo.MaxLevel > 1 && !tattooSuffixe.Contains("Max") ? $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}[upgradable]" : string.Empty;
        _instantiator.NewPopupYesNo($"{clickedTattoo.Name} {tattooSuffixe}", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "inked: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + bodyPart.GetDescription().ToLower() + "\n" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + clickedTattoo.GetDescription() + upgradable, null, "Ok", null);
    }

    private void CharacterLore()
    {
        if (string.IsNullOrEmpty(_character.Lore))
            return;
        _instantiator.NewPopupYesNo("Lore", _character.Lore.ToLower(), null, "Ok", null);
    }

    private void GetAttackDetailsString()
    {
        _attackDetails = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}attack: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{_character.GetAttackDetails()}.";
        _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}crit chance: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{_character.GetCriticalChancePercent()}%.";
        _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}crit damage: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.CritMultiplier}%.";
        _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}against {Helper.GetInferiorFrom(_character.Realm).ToString().ToLower()}: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.DamagePercentToInferiorRealm}%.";
        if (_character.QuadrupleLinesDamageOverride != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}fixed quadruple: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{_character.QuadrupleLinesDamageOverride}.";
        if (_character.SingleLinesDamageOverride != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}fixed single: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{_character.SingleLinesDamageOverride}.";
        if (_character.SingleLineDamageBonus != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}single bonus: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.SingleLineDamageBonus}.";
        if (_character.DamageBigLinesBonus != 0 || _character.DamageBigLinesMalus != 0 || _character.DamageSmallLinesBonus != 0 || _character.DamageSmallLinesMalus != 0)
        {
            var bigLines = _character.DamageBigLinesBonus - _character.DamageBigLinesMalus;
            var smallLines = _character.DamageSmallLinesBonus - _character.DamageSmallLinesMalus;
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}big lines: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{(bigLines > 0 ? "+" : "")}{bigLines}%.";
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}smal lines: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{(smallLines > 0 ? "+" : "")}{smallLines}%.";
        }
        if (_character.DamoclesDamage != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}damocles: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.DamoclesDamage}.";
        if (_character.FireDamagePercent != 0)
        {
            var burnDamage = Mathf.RoundToInt(_character.GetAttack() * Helper.MultiplierFromPercent(0.0f, _character.FireDamagePercent));
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}fire ticks: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{burnDamage}.";
        }
        if (_character.WaterDamagePercent != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}water boost: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.WaterDamagePercent}%.";
        if (_character.GatlingPercentDamage != 0)
        {
            var gatlingDamage = Mathf.RoundToInt(_character.GetAttack() * Helper.MultiplierFromPercent(0.0f, _character.GatlingPercentDamage));
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}gatling: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{gatlingDamage}.";
        }
        if (_character.TwistDamage != 0)
        {
            var twistDamage = Mathf.RoundToInt(_character.GetAttack() * Helper.MultiplierFromPercent(0.0f, _character.TwistDamage));
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}twists: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{twistDamage}.";
        }
        if (_character.SlavWheelDamagePercentBonus != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}slav wheel: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.SlavWheelDamagePercentBonus}%.";
        if (_character.TwistBoostedDamage != 0)
            _attackDetails += $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}after twist: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}+{_character.TwistBoostedDamage}.";
    }

    private void CharacterAttack()
    {
        var clickMe = GameObject.Find(Constants.GoClickMe);
        if (clickMe != null)
        {
            PlayerPrefsHelper.SaveHasClickedOnAttackDetails(true);
            Destroy(clickMe.gameObject);
        }
        _instantiator.NewPopupYesNo("Attack Details", _attackDetails, null, "Ok", null, big: _attackDetails.CountChar('\n') > 5 ? true : false);
    }

    private void InitOpponentFrame(Opponent opponent)
    {
        if (opponent != null)
        {
            _fightFrame.transform.Find("OpponentName").GetComponent<TMPro.TextMeshPro>().text = opponent.Name;
            _fightFrame.transform.Find("OpponentHealth").GetComponent<TMPro.TextMeshPro>().text = "health: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + opponent.HpMax;
            _fightFrame.transform.Find("OpponentCooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + Math.Round(opponent.Cooldown, 1) + " seconds";
            if (opponent.Attacks.Count > 1)
                _fightFrame.transform.Find("OpponentAttackLibelle").GetComponent<TMPro.TextMeshPro>().text = "attacks:";
            for (int i = 0; i < 4; ++i)
            {
                if (i < opponent.Attacks.Count)
                {
                    var prefixe = opponent.Attacks[i].AttackType.Prefixe.ToLower();
                    var suffixe = opponent.Attacks[i].AttackType.Suffixe.ToLower();
                    if (!string.IsNullOrEmpty(suffixe) && suffixe[suffixe.Length - 1] == 's' && opponent.Attacks[i].Param1 == 1)
                        suffixe = suffixe.Substring(0, suffixe.Length - 1);
                    var param1 = !(string.IsNullOrEmpty(prefixe) && string.IsNullOrEmpty(suffixe)) ? $"({prefixe}{opponent.Attacks[i].Param1}{suffixe})" : string.Empty;
                    _fightFrame.transform.Find("OpponentAttack" + (i + 1)).GetComponent<TMPro.TextMeshPro>().text = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}- {opponent.Attacks[i].AttackType.Name.ToLower()} {param1}";
                }
                else
                    _fightFrame.transform.Find("OpponentAttack" + (i + 1)).gameObject.SetActive(false);
            }
            _fightFrame.transform.Find("OpponentRealm").GetComponent<TMPro.TextMeshPro>().text = "realm: " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + opponent.Realm.ToString().ToLower() + "\n" +
                "- takes more damage from " + Helper.GetSuperiorFrom(opponent.Realm).ToString().ToLower() + " characters.\n";
            //+
            //   "- hits " + Helper.GetInferiorFrom(opponent.Realm).ToString().ToLower() + " characters stronger."
            _fightFrame.transform.Find("ButtonOpponent").GetComponent<ButtonBhv>().EndActionDelegate = OpponentLore;
            _fightFrame.transform.Find("ButtonOpponent").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{opponent.Region}Opponents_" + opponent.Id);
            _fightFrame.transform.Find("OpponentType").GetComponent<SpriteRenderer>().sprite = opponent.Type == OpponentType.Common ? null : Helper.GetSpriteFromSpriteSheet("Sprites/OpponentTypes_" + (((int)opponent.Realm * 3) + ((int)opponent.Type - 1)));
            _fightFrame.transform.Find("OpponentWeakness").GetComponent<TMPro.TextMeshPro>().text = "weakness\n" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) +
                (opponent.Weakness == Weakness.xLines ? opponent.XLineWeakness + " " + opponent.Weakness.GetDescription().ToLower() : opponent.Weakness.GetDescription().ToLower());
            _fightFrame.transform.Find("OpponentImmunity").GetComponent<TMPro.TextMeshPro>().text = "immunity\n" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) +
                (opponent.Immunity == Immunity.xLines ? opponent.XLineImmunity + " " + opponent.Immunity.GetDescription().ToLower() : opponent.Immunity.GetDescription().ToLower());
        }
        var pacts = PlayerPrefsHelper.GetCurrentPacts();
        var pactsStr = string.Empty;
        if (pacts.Count == 0
            || Cache.CurrentGameMode == GameMode.TrainingDummy
            || Cache.CurrentGameMode == GameMode.TrainingFree)
            pactsStr = "none.";
        else
        {
            foreach (var pact in pacts)
            {
                if (pactsStr != string.Empty)
                    pactsStr += "\n";
                var remainingFights = pact.MaxFight - pact.NbFight;
                pactsStr += $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{remainingFights} fight{(remainingFights > 1 ? "s" : "")}: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{pact.ShortDescription}.{Constants.MaterialEnd}";
            }
        }
        _fightFrame.transform.Find("PactsList").GetComponent<TMPro.TextMeshPro>().text = pactsStr.ToLower();
    }

    private void OpponentLore()
    {
        if (_opponent.Lore != null)
            _instantiator.NewPopupYesNo("Lore", _opponent.Lore.ToLower(), null, "Ok", null);
    }

    private void ShowCharacter()
    {
        _characterFrame.transform.position = transform.position;
        _fightFrame.transform.position = new Vector3(50.0f, 50.0f, 0.0f);
        _characterTab.transform.position = new Vector3(_characterTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOnY, 0.0f);
        _fightTab.transform.position = new Vector3(_fightTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOffY, 0.0f);
    }

    private void ShowOpponent()
    {
        _fightFrame.transform.position = transform.position;
        _characterFrame.transform.position = new Vector3(50.0f, 50.0f, 0.0f);
        _fightTab.transform.position = new Vector3(_fightTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOnY, 0.0f);
        _characterTab.transform.position = new Vector3(_characterTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOffY, 0.0f);
    }

    private void Resume()
    {
#if UNITY_ANDROID
        if (_isHorizontal)
        {
            if (Cache.HorizontalCameraInitialPosition != null)
            {
                Camera.main.transform.position = Cache.HorizontalCameraInitialPosition.Value;
                Cache.HorizontalCameraInitialPosition = null;
            }
            if (Cache.HorizontalCameraInitialRotation != null)
            {
                Camera.main.transform.rotation = Cache.HorizontalCameraInitialRotation.Value;
                Cache.HorizontalCameraInitialRotation = null;
            }
        }
#endif
        Cache.DecreaseInputLayer();
        _resumeAction.Invoke(true);
    }

    public override void ExitPopup()
    {
        Resume();
    }
}
