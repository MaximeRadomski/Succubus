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
        if (Constants.SelectedGameMode == Constants.TrainingGameScene)
            gameModeTitle = "Training";
        _charSelector = GameObject.Find("CharSelector");
        GameObject.Find("GameModeTitle").GetComponent<TMPro.TextMeshPro>().text = gameModeTitle;
        _charButtonsContainer = GameObject.Find("CharacterButtons");        
        SetButtons();

        var lastSelectedCharacter = PlayerPrefsHelper.GetSelectedCharacter();
        Constants.SetLastEndActionClickedName(_charButtonsContainer.transform.GetChild(lastSelectedCharacter).name);
        SelectCharacter();
    }

    private void SetButtons()
    {
        foreach (Transform child in _charButtonsContainer.transform)
        {
            child.GetComponent<ButtonBhv>().EndActionDelegate = SelectCharacter;
        }
        GameObject.Find("ButtonBack").GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find("ButtonPlay").GetComponent<ButtonBhv>().EndActionDelegate = Play;
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
        var unlockedCharacters = PlayerPrefsHelper.GetUnlockedCharacters();
        if (unlockedCharacters[realm + buttonId] == '0')
        {
            Instantiator.NewPopupYesNo("Locked", "you haven't unlocked this character yet", null, "Hmm...", null);
            return;
        }
        _charSelector.transform.position = new Vector3(lastClickedButton.transform.position.x, lastClickedButton.transform.position.y - 0.071f, 0.0f);
        PlayerPrefsHelper.SaveSelectedCharacter(buttonId);
        var tmpChar = CharactersData.Characters[buttonId];
        GameObject.Find("CharacterName").GetComponent<TMPro.TextMeshPro>().text = tmpChar.Name + " - " + tmpChar.Kind;
        GameObject.Find("Attack").GetComponent<TMPro.TextMeshPro>().text = "attack:" + Constants.MaterialHell_4_3 + tmpChar.Attack;
        GameObject.Find("Cooldown").GetComponent<TMPro.TextMeshPro>().text = "cooldown:" + Constants.MaterialHell_4_3 + tmpChar.Cooldown;
        GameObject.Find("Special").GetComponent<TMPro.TextMeshPro>().text = "special:" + Constants.MaterialHell_4_3 + tmpChar.SpecialName.ToLower() + ":\n" + tmpChar.SpecialDescription;
        GameObject.Find("Realm").GetComponent<TMPro.TextMeshPro>().text = "realm:" + Constants.MaterialHell_4_3 + tmpChar.Realm.ToString().ToLower() + ":\n" + tmpChar.Realm.GetDescription();
        GameObject.Find("CharacterPicture").GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + tmpChar.Id);
        Helper.ResetSelectedCharacterSpecialCooldown();
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
        Instantiator.NewOverBlend(OverBlendType.StartLoadingActionEnd, "Get Ready", 2, OnBlend);
        object OnBlend(bool result)
        {
            var scene = Constants.AscensionScene;
            if (Constants.SelectedGameMode == Constants.TrainingGameScene)
            {
                Constants.CurrentMusicType = MusicTyoe.GameHell;
                scene = Constants.TrainingGameScene;
                PlayerPrefsHelper.ResetTraining();
                PlayerPrefsHelper.SaveCurrentItem(ItemsData.NormalItemsNames[2]);
                PlayerPrefsHelper.ResetTattoos();
            }
            else
            {
                PlayerPrefsHelper.ResetCurrentItem();
                PlayerPrefsHelper.ResetTattoos();
            }
            NavigationService.LoadNextScene(scene);
            return true;
        }
    }
}
