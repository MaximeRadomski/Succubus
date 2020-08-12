using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoMenuBhv : PopupBhv
{
    private System.Func<bool, object> _resumeAction;
    private Instantiator _instantiator;
    private bool _isRotated;
    private Vector3 _cameraInitialPosition;
    private GameObject _characterFrame;
    private GameObject _opponentFrame;
    private Item _characterItem;

    public void Init(Instantiator instantiator, System.Func<bool, object> resumeAction, bool isRotated, Character character, Opponent opponent)
    {
        _instantiator = instantiator;
        _resumeAction = resumeAction;
        _isRotated = isRotated;
        if (_isRotated)
        {
            _cameraInitialPosition = Camera.main.transform.position;
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, _cameraInitialPosition.z);
            Camera.main.transform.Rotate(0.0f, 0.0f, -90.0f);
        }
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0.0f);
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = Resume;
        _characterFrame = GameObject.Find("CharacterFrame");
        _opponentFrame = GameObject.Find("OpponentFrame");
        InitCharacterFrame(character);
    }

    private void InitCharacterFrame(Character character)
    {
        _characterFrame.transform.Find("CharacterName").GetComponent<TMPro.TextMeshPro>().text = character.Name + " - " + character.Kind;
        _characterFrame.transform.Find("Attack").GetComponent<TMPro.TextMeshPro>().text = "attack:" + Constants.MaterialHell_4_3 + character.Attack;
        _characterFrame.transform.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown:" + Constants.MaterialHell_4_3 + character.Cooldown;
        _characterFrame.transform.Find("Special").GetComponent<TMPro.TextMeshPro>().text = "special:" + Constants.MaterialHell_4_3 + character.SpecialName.ToLower() + ":\n" + character.SpecialDescription;
        _characterFrame.transform.Find("Realm").GetComponent<TMPro.TextMeshPro>().text = "realm:" + Constants.MaterialHell_4_3 + character.Realm.ToString().ToLower() + ":\n" + character.Realm.GetDescription();
        _characterFrame.transform.Find("CharacterPicture").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + character.Id);
        var itemName = PlayerPrefsHelper.GetCurrentItem();
        if (itemName != null)
        {
            _characterItem = (Item)Activator.CreateInstance(Type.GetType("Item" + itemName.Replace(" ", "").Replace("'", "")));
            _characterItem.Init(character, null);
            _characterFrame.transform.Find("ButtonItem").GetComponent<ButtonBhv>().EndActionDelegate = ItemInfo;
            _characterFrame.transform.Find("ButtonItem").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + _characterItem.Id);
        }
    }

    private void ItemInfo()
    {
        _instantiator.NewPopupYesNo("Item", Constants.MaterialHell_3_2 + _characterItem.Name.ToLower() + Constants.MaterialEnd + ":\n" + _characterItem.Description.ToLower(), null, "Ok", null);
    }

    private void Resume()
    {
        if (_isRotated)
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
