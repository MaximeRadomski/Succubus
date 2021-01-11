using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharSelSceneBhv : SceneBhv
{
    private GameObject _charSelector;
    private GameObject _charButtonsContainer;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        var gameModeTitle = "New Ascension";
        if (Constants.CurrentGameMode == GameMode.TrainingFree)
            gameModeTitle = "Free Play";
        else if (Constants.CurrentGameMode == GameMode.TrainingDummy)
            gameModeTitle = "Training Dummy";
        _charSelector = GameObject.Find("CharSelector");
        GameObject.Find("GameModeTitle").GetComponent<TMPro.TextMeshPro>().text = gameModeTitle;
        _charButtonsContainer = GameObject.Find("CharacterButtons");        
        SetButtons();

        var lastSelectedCharacter = PlayerPrefsHelper.GetSelectedCharacterId();
        Constants.SetLastEndActionClickedName(_charButtonsContainer.transform.GetChild(lastSelectedCharacter).name);
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
        var lastClickedButton = GameObject.Find(Constants.LastEndActionClickedName);
        var buttonId = int.Parse(lastClickedButton.gameObject.name.Substring(lastClickedButton.gameObject.name.Length - 2, 2));
        var realm = 0;
        if (lastClickedButton.name.Contains(Realm.Earth.ToString()))
            realm = 4;
        else if (lastClickedButton.name.Contains(Realm.Heaven.ToString()))
            realm = 8;
        var unlockedCharacters = PlayerPrefsHelper.GetUnlockedCharactersString();
        if (unlockedCharacters[realm + buttonId] == '0')
        {
            Instantiator.NewPopupYesNo("Locked", "you haven't unlocked this character yet", null, "Hmm...", null);
            return;
        }
        _charSelector.transform.position = new Vector3(lastClickedButton.transform.position.x, lastClickedButton.transform.position.y - 0.071f, 0.0f);
        PlayerPrefsHelper.SaveSelectedCharacter(buttonId);
        var tmpChar = CharactersData.Characters[buttonId];
        GameObject.Find("CharacterName").GetComponent<TMPro.TextMeshPro>().text = tmpChar.Name + " - " + tmpChar.Kind;
        GameObject.Find("Attack").GetComponent<TMPro.TextMeshPro>().text = "attack:" + Constants.MaterialHell_4_3 + tmpChar.GetAttack();
        GameObject.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown:" + Constants.MaterialHell_4_3 + tmpChar.Cooldown;
        GameObject.Find("Special").GetComponent<TMPro.TextMeshPro>().text = "special:" + Constants.MaterialHell_4_3 + tmpChar.SpecialName.ToLower() + ":\n" + tmpChar.SpecialDescription;
        GameObject.Find("Realm").GetComponent<TMPro.TextMeshPro>().text = "realm:" + Constants.MaterialHell_4_3 + tmpChar.Realm.ToString().ToLower() + ":\n" + tmpChar.Realm.GetDescription();
        GameObject.Find("CharacterPicture").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + tmpChar.Id);
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
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            Instantiator.NewOverBlend(OverBlendType.StartLoadingActionEnd, "Get Ready", 2, OnBlend);
        else
            Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend);
        object OnBlend(bool result)
        {
            var scene = "";
            if (Constants.CurrentGameMode == GameMode.TrainingFree)
            {
                Constants.CurrentMusicType = MusicType.GameHell;
                scene = Constants.TrainingFreeGameScene;
                PlayerPrefsHelper.ResetTraining();
                PlayerPrefsHelper.SaveCurrentOpponents(null);
                Constants.ResetClassicGameCache();
                Constants.CurrentItemCooldown = 0;
            }
            else if (Constants.CurrentGameMode == GameMode.TrainingDummy)
            {
                Constants.CurrentMusicType = MusicType.GameHell;
                scene = Constants.ClassicGameScene;
                var opponents = new List<Opponent>() { OpponentsData.HellOpponents[0], OpponentsData.HellOpponents[1], OpponentsData.HellOpponents[2] };
                PlayerPrefsHelper.SaveCurrentOpponents(opponents);
                //PlayerPrefsHelper.ResetTattoos();
                Constants.ResetClassicGameCache();
                Constants.ResetCurrentItemCooldown(CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()], ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]));
            }
            else
            {
                scene = Constants.DifficultyScene;
                PlayerPrefsHelper.ResetCurrentItem();
                PlayerPrefsHelper.ResetTattoos();
                PlayerPrefsHelper.SaveRunCharacter(CharactersData.Characters[PlayerPrefsHelper.GetSelectedCharacterId()]);
                PlayerPrefsHelper.ResetRun();
            }
            NavigationService.LoadNextScene(scene);
            return true;
        }
    }
}
