using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMenuBhv : PopupBhv
{
    private Camera _mainCamera;
    private System.Func<bool, object> _resumeAction;
    private Instantiator _instantiator;
    private bool _isHorizontal;
    private Vector3 _cameraInitialPosition;
    private GameObject _characterFrame;
    private GameObject _opponentFrame;
    private GameObject _characterTab;
    private GameObject _opponentTab;
    private Item _characterItem;
    private Run _run;
    private Realm _currentRealm;

    private Character _character;
    private Opponent _opponent;

    private float _buttonOnY;
    private float _buttonOffY;

    public void Init(Instantiator instantiator, System.Func<bool, object> resumeAction, Character character, Opponent opponent, bool isHorizontal)
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
        if (_isHorizontal)
        {
            _cameraInitialPosition = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, _cameraInitialPosition.z);
            Camera.main.transform.Rotate(0.0f, 0.0f, -90.0f);
        }
        _mainCamera = Helper.GetMainCamera();
        transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, 0.0f);
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        (_characterTab = GameObject.Find("CharacterTab")).GetComponent<ButtonBhv>().EndActionDelegate = ShowCharacter;
        (_opponentTab = GameObject.Find("OpponentTab")).GetComponent<ButtonBhv>().EndActionDelegate = ShowOpponent;
        _characterFrame = GameObject.Find("CharacterFrame");
        _opponentFrame = GameObject.Find("OpponentFrame");
        InitCharacterFrame(character);
        InitOpponentFrame(opponent);
        if (opponent == null)
            ShowCharacter();
        else
            ShowOpponent();
    }

    private void InitCharacterFrame(Character character)
    {
        _characterFrame.transform.Find("CharacterName").GetComponent<TMPro.TextMeshPro>().text = character.Name + " - " + character.Kind;
        _characterFrame.transform.Find("CharacterAttack").GetComponent<TMPro.TextMeshPro>().text = "attack: " + Constants.MaterialHell_4_3 + character.GetAttack();
        _characterFrame.transform.Find("CharacterCooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown: " + Constants.MaterialHell_4_3 + character.Cooldown;
        _characterFrame.transform.Find("CharacterSpecial").GetComponent<TMPro.TextMeshPro>().text = "special: " + Constants.MaterialHell_4_3 + character.SpecialName.ToLower() + ":\n" + character.SpecialDescription;
        _characterFrame.transform.Find("CharacterRealm").GetComponent<TMPro.TextMeshPro>().text = "realm: " + Constants.MaterialHell_4_3 + character.Realm.ToString().ToLower() + ":\n" + character.Realm.GetDescription().ToLower();
        _characterFrame.transform.Find("ButtonCharacter").GetComponent<ButtonBhv>().EndActionDelegate = CharacterLore;
        _characterFrame.transform.Find("ButtonCharacter").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + character.Id);
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
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
                var tmpTattooGameObject = GameObject.Find("TattooPlaceHolder" + tattoo.BodyPart.GetHashCode().ToString("00"));
                tmpTattooGameObject.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + tattoo.Id.ToString("00"));
                tmpTattooGameObject.GetComponent<ButtonBhv>().EndActionDelegate = TattooInfo;
                tmpTattooGameObject.name = "Tattoo" + tattoo.Id.ToString("00");
                tmpTattooGameObject.transform.parent.GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
            }
        }
    }

    private void ItemInfo()
    {
        _instantiator.NewPopupYesNo(_characterItem.Name, Constants.MaterialHell_3_2 + "cooldown: " + _characterItem.Cooldown + Constants.MaterialEnd + "\n" + _characterItem.Description.ToLower(), null, "Ok", null);
    }

    private void TattooInfo()
    {
        var bodyPart = (BodyPart)int.Parse(GameObject.Find(Constants.LastEndActionClickedName).transform.parent.name.Substring("BodyPart".Length));
        var clickedTattoo = PlayerPrefsHelper.GetCurrentInkedTattoo(TattoosData.Tattoos[int.Parse(Constants.LastEndActionClickedName.Substring("Tattoo".Length))]);
        _instantiator.NewPopupYesNo(clickedTattoo.Name + (clickedTattoo.Level > 1 ? (" +" + (clickedTattoo.Level - 1).ToString()) : ""), Constants.MaterialHell_3_2 + "inked: " + Constants.MaterialHell_4_3 + bodyPart.GetDescription().ToLower() + "\n" + Constants.MaterialHell_3_2 + clickedTattoo.GetDescription(), null, "Ok", null);
    }

    private void CharacterLore()
    {
        _instantiator.NewPopupYesNo("Lore", _character.Lore.ToLower(), null, "Ok", null);
    }

    private void InitOpponentFrame(Opponent opponent)
    {
        if (opponent == null)
            return;
        _opponentFrame.transform.Find("OpponentName").GetComponent<TMPro.TextMeshPro>().text = opponent.Name;
        _opponentFrame.transform.Find("OpponentHealth").GetComponent<TMPro.TextMeshPro>().text = "health: " + Constants.MaterialHell_4_3 + opponent.HpMax;
        _opponentFrame.transform.Find("OpponentCooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown: " + Constants.MaterialHell_4_3 + opponent.Cooldown + " seconds";
        if (opponent.Attacks.Count > 1)
        _opponentFrame.transform.Find("OpponentAttackLibelle").GetComponent<TMPro.TextMeshPro>().text = "attacks:";
        for (int i = 0; i < 4; ++i)
        {
            if (i < opponent.Attacks.Count)
            {
                var prefixe = opponent.Attacks[i].AttackType.GetAttribute<PrefixeAttribute>().ToLower();
                var suffixe = opponent.Attacks[i].AttackType.GetAttribute<SuffixeAttribute>().ToLower();
                var param1 = !(string.IsNullOrEmpty(prefixe) && string.IsNullOrEmpty(suffixe)) ? $"({prefixe}{opponent.Attacks[i].Param1}{suffixe})" : string.Empty;
                _opponentFrame.transform.Find("OpponentAttack" + (i + 1)).GetComponent<TMPro.TextMeshPro>().text = $"{Constants.MaterialHell_4_3}- {opponent.Attacks[i].AttackType.GetDescription().ToLower()} {param1}";
            }
            else
                _opponentFrame.transform.Find("OpponentAttack" + (i + 1)).gameObject.SetActive(false);
        }
        _opponentFrame.transform.Find("OpponentRealm").GetComponent<TMPro.TextMeshPro>().text = "realm: " + Constants.MaterialHell_4_3 + opponent.Realm.ToString().ToLower() + "\n" +
            "- takes more damages from " + Helper.GetSuperiorFrom(opponent.Realm).ToString().ToLower() + " characters.\n" +
            "- hits " + Helper.GetInferiorFrom(opponent.Realm).ToString().ToLower() + " characters stronger.";
        _opponentFrame.transform.Find("ButtonOpponent").GetComponent<ButtonBhv>().EndActionDelegate = OpponentLore;
        _opponentFrame.transform.Find("ButtonOpponent").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{_currentRealm}Opponents_" + opponent.Id);
        _opponentFrame.transform.Find("OpponentType").GetComponent<SpriteRenderer>().sprite = opponent.Type == OpponentType.Common ? null : Helper.GetSpriteFromSpriteSheet("Sprites/OpponentTypes_" + ((opponent.Realm.GetHashCode() * 3) + (opponent.Type.GetHashCode() - 1)));
        _opponentFrame.transform.Find("OpponentWeakness").GetComponent<TMPro.TextMeshPro>().text = "weakness\n" + Constants.MaterialHell_4_3 + 
            (opponent.Weakness == Weakness.xLines ? opponent.XLineWeakness + " " + opponent.Weakness.ToString().ToLower() : opponent.Weakness.ToString().ToLower());
        _opponentFrame.transform.Find("OpponentImmunity").GetComponent<TMPro.TextMeshPro>().text = "immunity\n" + Constants.MaterialHell_4_3 +
            (opponent.Immunity == Immunity.xLines ? opponent.XLineImmunity + " " + opponent.Immunity.ToString().ToLower() : opponent.Immunity.ToString().ToLower());
    }

    private void OpponentLore()
    {
        if (_opponent.Lore != null)
            _instantiator.NewPopupYesNo("Lore", _opponent.Lore.ToLower(), null, "Ok", null);
    }

    private void ShowCharacter()
    {
        _characterFrame.transform.position = transform.position;
        _opponentFrame.transform.position = new Vector3(50.0f, 50.0f, 0.0f);
        _characterTab.transform.position = new Vector3(_characterTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOnY, 0.0f);
        _opponentTab.transform.position = new Vector3(_opponentTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOffY, 0.0f);
    }

    private void ShowOpponent()
    {
        if (_opponent == null)
        {
            _opponentTab.transform.position = new Vector3(_opponentTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOnY, 0.0f);
            _characterTab.transform.position = new Vector3(_characterTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOffY, 0.0f);
            _instantiator.NewPopupYesNo("No Opponent", "you currently have no opponent", null, "Ok", OnOk);
            object OnOk(bool result)
            {
                _characterTab.transform.position = new Vector3(_characterTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOnY, 0.0f);
                _opponentTab.transform.position = new Vector3(_opponentTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOffY, 0.0f);
                return result;
            }
            return;
        }
        _opponentFrame.transform.position = transform.position;
        _characterFrame.transform.position = new Vector3(50.0f, 50.0f, 0.0f);
        _opponentTab.transform.position = new Vector3(_opponentTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOnY, 0.0f);
        _characterTab.transform.position = new Vector3(_characterTab.transform.position.x, _characterTab.transform.parent.position.y + _buttonOffY, 0.0f);
    }

    private void Resume()
    {
        if (_isHorizontal)
        {
            Camera.main.transform.position = _cameraInitialPosition;
            Camera.main.transform.Rotate(0.0f, 0.0f, 90.0f);
        }
        Constants.DecreaseInputLayer();
        _resumeAction.Invoke(true);
    }

    public override void ExitPopup()
    {
        Resume();
    }
}
