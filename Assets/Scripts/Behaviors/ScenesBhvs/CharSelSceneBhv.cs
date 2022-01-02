using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelSceneBhv : SceneBhv
{
    private GameObject _charSelector;
    private GameObject _charButtonsContainer;
    private int _selectedSkin = -1;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        var gameModeTitle = "New Ascension";
        if (Cache.CurrentGameMode == GameMode.TrainingFree)
            gameModeTitle = "Free Play";
        else if (Cache.CurrentGameMode == GameMode.TrainingDummy)
            gameModeTitle = "Training Dummy";
        _charSelector = GameObject.Find("CharSelector");
        GameObject.Find("GameModeTitle").GetComponent<TMPro.TextMeshPro>().text = gameModeTitle;
        _charButtonsContainer = GameObject.Find("CharacterButtons");        
        SetButtons();
        var unlockedChars = PlayerPrefsHelper.GetUnlockedCharactersString();
        for (int i = 2; i < unlockedChars.Length; ++i)
        {
            if (unlockedChars[i] == '1')
                GameObject.Find($"Character{i.ToString("00")}").transform.GetChild(1).GetComponent<SpriteRenderer>().color = Constants.ColorPlain;
        }
        var lastSelectedCharacter = PlayerPrefsHelper.GetSelectedCharacterId();
        Cache.SetLastEndActionClickedName(_charButtonsContainer.transform.GetChild(lastSelectedCharacter).name);
        SelectCharacter();
    }

    private void SetButtons()
    {
        foreach (Transform child in _charButtonsContainer.transform)
        {
            child.GetComponent<ButtonBhv>().EndActionDelegate = SelectCharacter;
        }
        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find(Constants.GoButtonPlayName).GetComponent<ButtonBhv>().EndActionDelegate = Play;
        GameObject.Find("CharacterPicture").GetComponent<ButtonBhv>().EndActionDelegate = CharacterLore;
    }

    private void SelectCharacter()
    {
        var lastClickedButton = GameObject.Find(Cache.LastEndActionClickedName);
        var buttonId = int.Parse(lastClickedButton.gameObject.name.Substring(lastClickedButton.gameObject.name.Length - 2, 2));
        var realm = 0;
        if (lastClickedButton.name.Contains(Realm.Earth.ToString()))
            realm = 4;
        else if (lastClickedButton.name.Contains(Realm.Heaven.ToString()))
            realm = 8;
        var unlockedCharacters = PlayerPrefsHelper.GetUnlockedCharactersString();
        if (unlockedCharacters[realm + buttonId] == '0'  && false)
        {
            Instantiator.NewPopupYesNo("Locked", "you haven't unlocked this character yet", null, "Hmm...", null);
            return;
        }
        _charSelector.transform.position = new Vector3(lastClickedButton.transform.position.x, lastClickedButton.transform.position.y - 0.071f, 0.0f);
        PlayerPrefsHelper.SaveSelectedCharacter(buttonId);
        var tmpChar = CharactersData.Characters[buttonId];
        GameObject.Find("CharacterName").GetComponent<TMPro.TextMeshPro>().text = tmpChar.Name + " - " + tmpChar.Kind;
        GameObject.Find("Attack").GetComponent<TMPro.TextMeshPro>().text = "attack:" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + tmpChar.GetAttackNoBoost();
        GameObject.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown:" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + tmpChar.Cooldown;
        GameObject.Find("Special").GetComponent<TMPro.TextMeshPro>().text = "special:" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + tmpChar.SpecialName.ToLower() + ":\n" + tmpChar.SpecialDescription;
        GameObject.Find("Realm").GetComponent<TMPro.TextMeshPro>().text = "realm:" + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + tmpChar.Realm.ToString().ToLower() + ":\n" + tmpChar.Realm.GetDescription();
        var characterSkins = Helper.GetUnlockedCharacterSkins(tmpChar.Id);
        if (_selectedSkin == -1) //First time SelectCharacter() is called in Init()
        {
            if (characterSkins != null)
            {
                _selectedSkin = PlayerPrefsHelper.GetSelectedSkinId();
                PlayerPrefsHelper.SaveSelectedSkinId(_selectedSkin);
            }
            else
            {
                _selectedSkin = 0;
                PlayerPrefsHelper.SaveSelectedSkinId(0);
            }
        }
        else
        {
            if (characterSkins != null)
            {
                this.Instantiator.NewPopupCharacterSkins(tmpChar.Id, characterSkins, _selectedSkin, (int selectedSkin) =>
                {
                    if (_selectedSkin != selectedSkin)
                    {
                        _selectedSkin = selectedSkin;
                        PlayerPrefsHelper.SaveSelectedSkinId(_selectedSkin);
                        GameObject.Find("CharacterPicture").GetComponent<SpriteRenderer>().sprite = Helper.GetCharacterSkin(tmpChar.Id, _selectedSkin);
                    }
                });
            }
            else
            {
                _selectedSkin = 0;
                PlayerPrefsHelper.SaveSelectedSkinId(0);
            }
        }
        GameObject.Find("CharacterPicture").GetComponent<SpriteRenderer>().sprite = Helper.GetCharacterSkin(tmpChar.Id, _selectedSkin);
    }

    private void CharacterLore()
    {
        Instantiator.NewPopupYesNo("Lore", CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()].Lore.ToLower(), null, "Ok", null);
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

    private void Play()
    {
        if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
            Instantiator.NewOverBlend(OverBlendType.StartLoadingActionEnd, "Get Ready", 2, OnBlend);
        else
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            var scene = "";
            if (Cache.CurrentGameMode == GameMode.TrainingFree)
            {
                scene = Constants.TrainingFreeGameScene;
                PlayerPrefsHelper.ResetTraining();
                PlayerPrefsHelper.SaveCurrentOpponents(null);
                Cache.ResetClassicGameCache();
                Cache.CurrentItemCooldown = 0;
            }
            else if (Cache.CurrentGameMode == GameMode.TrainingDummy)
            {
                scene = Constants.ClassicGameScene;
                var opponents = new List<Opponent>() { OpponentsData.HellOpponents[0], OpponentsData.HellOpponents[1], OpponentsData.HellOpponents[2] };
                //DEBUG
                if (OpponentsData.DebugEnabled)
                    opponents.Insert(0, OpponentsData.DebugOpponent());
                //DEBUG
                PlayerPrefsHelper.SaveCurrentOpponents(opponents);
                //PlayerPrefsHelper.ResetTattoos();
                Cache.ResetClassicGameCache();
                Cache.RestartCurrentItemCooldown(CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()], ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]));
            }
            else
            {
                scene = Constants.DifficultyScene;
                PlayerPrefsHelper.ResetCurrentItem();
                PlayerPrefsHelper.ResetTattoos();
                PlayerPrefsHelper.ResetPacts();
                PlayerPrefsHelper.ResetAlreadyDialog();
                PlayerPrefsHelper.SaveRunCharacter(CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()], _selectedSkin);
                PlayerPrefsHelper.ResetRunBossVanquished();
                PlayerPrefsHelper.ResetRun();
            }
            NavigationService.LoadNextScene(scene);
            return true;
        }
    }
}
