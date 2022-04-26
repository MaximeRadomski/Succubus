using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StepsSceneBhv : SceneBhv
{
    public List<Sprite> _backgroundSprites;

    private Run _run;
    private Character _character;
    private StepsService _stepsService;
    private GameObject _stepsContainer;
    private GameObject _selector;
    private GameObject _position;
    private GameObject _stepsBackground;
    private GameObject _pauseMenu;
    private GameObject _playButton;
    private GameObject _backgroundMask;

    private TMPro.TextMeshPro _lootTypeRarity;
    private TMPro.TextMeshPro _opponents;
    private SpriteRenderer _opponentType;
    private SpriteRenderer _lootPicture;
    private TMPro.TextMeshPro _lootName;
    private SpriteRenderer _characterPicture;
    private SpriteRenderer _beholderPicture;
    private SpriteRenderer _lurkerPicture;
    private GameObject _buttonInfo;
    private GameObject _buttonBeholder;
    private GameObject _buttonLurker;
    private InputControlerBhv _inputControler;

    private Step _selectedStep;
    private float _lootCenterLocalY;

    private float _lastSelectedInput = -1;

    public override MusicType MusicType => MusicType.Steps;

    void Start()
    {
        Init();
        if (Cache.NameLastScene == Constants.SettingsScene)
            PauseOrPrevious();
    }

    protected override void Init()
    {
        try
        { 
            base.Init();
            CameraBhv.Paused = Cache.NameLastScene == Constants.SettingsScene;
            _run = PlayerPrefsHelper.GetRun();
            _character = PlayerPrefsHelper.GetRunCharacter();
            if (_run.RealmLevel == 1)
            {
                PlayerPrefsHelper.SaveHasDoneTrading(Constants.PpHasDoneTradingDefault);
                PlayerPrefsHelper.SaveTradingMarket(Constants.PpTradingMarketDefault);
                PlayerPrefsHelper.SaveBoostButtonPrice(Constants.PpBoostButtonPriceDefault);
            }
            else if (_run.RealmLevel == 2 && PlayerPrefsHelper.GetTradingMarket() == Constants.PpTradingMarketDefault)
                PlayerPrefsHelper.SaveTradingMarket(UnityEngine.Random.Range(0, 6));

            _lootCenterLocalY = GameObject.Find("LootCenter").transform.localPosition.y;
            GameObject.Find("Title").GetComponent<TMPro.TextMeshPro>().text = _run.CurrentRealm + " - lvl " + _run.RealmLevel;
            GameObject.Find("RemainingSteps").GetComponent<TMPro.TextMeshPro>().text = "Remaining Steps : " + (_run.MaxSteps - _run.CurrentStep);
            _lootTypeRarity = GameObject.Find("LootTypeRarity").GetComponent<TMPro.TextMeshPro>();
            _opponents = GameObject.Find("Opponents").GetComponent<TMPro.TextMeshPro>();
            _opponentType = GameObject.Find("OpponentType").GetComponent<SpriteRenderer>();
            _opponentType.sprite = null;
            _lootPicture = GameObject.Find("LootPicture").GetComponent<SpriteRenderer>();
            _lootPicture.GetComponent<ButtonBhv>().EndActionDelegate = LootInfo;
            _lootName = GameObject.Find("LootName").GetComponent<TMPro.TextMeshPro>();
            GameObject.Find(Constants.GoButtonPauseName).GetComponent<ButtonBhv>().EndActionDelegate = Pause;
            _characterPicture = GameObject.Find("CharacterPicture").GetComponent<SpriteRenderer>();
            _characterPicture.sprite = Helper.GetCharacterSkin(_character.Id, _character.SkinId);
            _characterPicture.GetComponent<ButtonBhv>().EndActionDelegate = Info;
            _buttonInfo = GameObject.Find(Constants.GoButtonInfoName);
            _buttonInfo.GetComponent<ButtonBhv>().EndActionDelegate = Info;
            _beholderPicture = GameObject.Find("BeholderPicture").GetComponent<SpriteRenderer>();
            _beholderPicture.GetComponent<ButtonBhv>().EndActionDelegate = BeholderWarp;
            _buttonBeholder = GameObject.Find("ButtonBeholder");
            _buttonBeholder.GetComponent<ButtonBhv>().EndActionDelegate = BeholderWarp;
            _lurkerPicture = GameObject.Find("LurkerPicture").GetComponent<SpriteRenderer>();
            _lurkerPicture.GetComponent<ButtonBhv>().EndActionDelegate = LurkerShop;
            _buttonLurker = GameObject.Find("ButtonLurker");
            _buttonLurker.GetComponent<ButtonBhv>().EndActionDelegate = LurkerShop;
            (_playButton = GameObject.Find(Constants.GoButtonPlayName)).GetComponent<ButtonBhv>().EndActionDelegate = GoToStep;
            _selector = GameObject.Find("Selector");
            _position = GameObject.Find("Position");
            _position.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (Constants.StepsAssetsPositionId + (_run.CurrentRealm.GetHashCode() * Constants.StepsAssetsCount)));
            _inputControler = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
            _backgroundMask = GameObject.Find("BackgroundMask");

            UpdateResourcesInfo();
            _stepsService = new StepsService();
            _selector.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (Constants.StepsAssetsSelectorId + (Constants.StepsAssetsCount * _run.CurrentRealm.GetHashCode())));
            _stepsBackground = GameObject.Find("StepsBackground");
            _stepsBackground.GetComponent<SpriteRenderer>().sprite = _backgroundSprites[_run.CurrentRealm.GetHashCode()];
            if (string.IsNullOrEmpty(_run.Steps))
            {
                _stepsService.GenerateOriginSteps(_run, _character);
                PlayerPrefsHelper.SaveRun(_run);
            }
            else if (_run.CurrentStep >= _run.MaxSteps)
                _stepsService.SetVisionOnAllSteps(_run);
            UpdateAllStepsVisuals();
            FocusOnSelected(_run.X, _run.Y);
            PositionOnCurrent();
            if (_stepsService.GetStepOnPos(_run.X, _run.Y, _run.Steps).LandLordVision)
            {
                Cache.InputLocked = true;
                Invoke(nameof(OnBossTriggered), 1.0f);
            }
        }
        catch (Exception e)
        {
            PlayerPrefsHelper.ResetRun();
            LogService.LogCallback($"Custom Caught Exception:\nMessage: {e.Message}\nSource:{e.Source}", e.StackTrace, LogType.Exception);
        }
    }

    private void UpdateResourcesInfo()
    {
        var resources = _run.GetRunResources();
        for (int i = 0; i < resources.Count; ++i)
        {
            if (resources[i] <= 0)
            {
                GameObject.Find($"StepResource{i}").GetComponent<SpriteRenderer>().enabled = false;
                GameObject.Find($"StepResource{i}").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().enabled = false; 
            }
            else
            {
                GameObject.Find($"StepResource{i}").GetComponent<SpriteRenderer>().enabled = true;
                GameObject.Find($"StepResource{i}").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().enabled = true;
                GameObject.Find($"StepResource{i}").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = resources[i].ToString();
            }
        }
    }

    public override void PauseOrPrevious()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewPauseMenu(ResumeGiveUp);
    }



    private void OnStepClicked()
    {
        var lastClicked = Cache.LastEndActionClickedName;
        FocusOnSelected(int.Parse(lastClicked.Substring(0, 2)), int.Parse(lastClicked.Substring(3, 2)));
    }

    private void PositionOnCurrent()
    {
        _position.transform.position = Helper.TransformFromStepCoordinates(_run.X, _run.Y);
        _position.GetComponent<IconInstanceBhv>().Pop();
    }

    private void FocusOnSelected(int x, int y)
    {
        if (_selectedStep != null && _selectedStep.X == x && _selectedStep.Y == y && _lastSelectedInput >= Time.time - 0.2f && _selectedStep.LootType != LootType.None)
        {
            GoToStep();
            return;
        }
        _lastSelectedInput = Time.time;
        _selectedStep = _stepsService.GetStepOnPos(x, y, _run.Steps);
        _selector.transform.position = Helper.TransformFromStepCoordinates(_selectedStep.X, _selectedStep.Y);
        if (x == _run.X && y == _run.Y)
            PositionOnCurrent();
        _selector.GetComponent<IconInstanceBhv>().Pop();
        var distance = Vector2.Distance(CameraBhv.gameObject.transform.position, _stepsBackground.transform.position);
        StartCoroutine(Helper.ExecuteAfterDelay(0.05f, () => {
            CameraBhv.SlideToPosition(_selector.transform.position + new Vector3(0.0f, -distance, 0.0f));
#if !UNITY_ANDROID
            _inputControler.InitMenuKeyboardInputs(_selector.transform.position + new Vector3(0.0f, 1.5f, 0.0f));
#endif
        }));
        _playButton.SetActive(_selectedStep.LootType != LootType.None);
        var rarity = Rarity.Common;
        if (_selectedStep.LootType != LootType.None)
        {
            _characterPicture.gameObject.SetActive(false);
            _buttonInfo.SetActive(false);
            _beholderPicture.gameObject.SetActive(false);
            _buttonBeholder.SetActive(false);
            _lurkerPicture.gameObject.SetActive(false);
            _buttonLurker.SetActive(false);
            if (_selectedStep.LootType == LootType.Character)
            {
                rarity = Rarity.Legendary;
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _selectedStep.LootId);
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _characterPicture.transform.localPosition.y, 0.0f);
                _lootName.text = CharactersData.Characters[_selectedStep.LootId].Name.ToLower();
            }
            else if (_selectedStep.LootType == LootType.Item)
            {
                rarity = ItemsData.GetItemFromName(ItemsData.Items[_selectedStep.LootId]).Rarity;
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + _selectedStep.LootId);
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _lootCenterLocalY, 0.0f);
                _lootName.text = ItemsData.Items[_selectedStep.LootId].ToLower();
            }
            else if (_selectedStep.LootType == LootType.Resource)
            {
                rarity = Rarity.Common;
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Resources_" + _selectedStep.LootId);
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _lootCenterLocalY, 0.0f);
                _lootName.text = ResourcesData.Resources[_selectedStep.LootId].ToLower();
            }
            else if (_selectedStep.LootType == LootType.Pact)
            {
                rarity = PactsData.GetPactFromName(PactsData.Pacts[_selectedStep.LootId]).Rarity;
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Pacts_" + _selectedStep.LootId);
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _characterPicture.transform.localPosition.y, 0.0f);
                _lootName.text = PactsData.Pacts[_selectedStep.LootId].ToLower();
            }
            else if (_selectedStep.LootType == LootType.Tattoo)
            {
                rarity = TattoosData.GetTattooFromName(TattoosData.Tattoos[_selectedStep.LootId]).Rarity;
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + _selectedStep.LootId);
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _lootCenterLocalY, 0.0f);
                _lootName.text = TattoosData.Tattoos[_selectedStep.LootId].ToLower();
            }
            _lootTypeRarity.text = _selectedStep.LootType.ToString().ToLower() + "\n" + Constants.GetMaterial(_run.CurrentRealm, TextType.succubus3x5, TextCode.c43) + rarity.ToString().ToLower();
            _opponents.text = "opponents\n" + Constants.GetMaterial(_run.CurrentRealm, TextType.succubus3x5, TextCode.c43) + ((OpponentType)rarity.GetHashCode()).ToString().ToLower();
            _opponentType.sprite = rarity == Rarity.Common ? null : Helper.GetSpriteFromSpriteSheet("Sprites/OpponentTypes_" + ((_run.CurrentRealm.GetHashCode() * 3) + (rarity.GetHashCode() - 1)));
            _lootPicture.GetComponent<IconInstanceBhv>().Pop();
        }
        else
        {
            if (x == _run.X && y == _run.Y)
            {
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (Constants.StepsAssetsPositionId + (_run.CurrentRealm.GetHashCode() * Constants.StepsAssetsCount)));
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _lootCenterLocalY, 0.0f);
                _lootName.text = "current location";
                _lootTypeRarity.text = "";
                _opponents.text = "";
                _opponentType.sprite = null;
                _characterPicture.gameObject.SetActive(true);
                _buttonInfo.SetActive(true);
            }
            else
            {
                _lootPicture.sprite = null;
                _lootName.text = " - ";
                _lootTypeRarity.text = " - \n" + Constants.GetMaterial(_run.CurrentRealm, TextType.succubus3x5, TextCode.c43) + " - ";
                _opponents.text = _lootTypeRarity.text;
                _opponentType.sprite = null;
                _characterPicture.gameObject.SetActive(false);
                _buttonInfo.SetActive(false);
            }

            if (x == 50 && y == 50 && PlayerPrefsHelper.GetRealmBossProgression() >= _run.CurrentRealm.GetHashCode() && _run.RealmLevel == 1 && _run.CurrentRealm < Realm.Heaven)
            {
                _beholderPicture.gameObject.SetActive(true);
                _buttonBeholder.SetActive(true);
                _lurkerPicture.gameObject.SetActive(false);
                _buttonLurker.SetActive(false);
            }
            else if (x == 50 && y == 50 && _run.RealmLevel == 2)
            {
                _lurkerPicture.gameObject.SetActive(true);
                _buttonLurker.SetActive(true);
                _beholderPicture.gameObject.SetActive(false);
                _buttonBeholder.SetActive(false);
            }
            else
            {
                _beholderPicture.gameObject.SetActive(false);
                _buttonBeholder.SetActive(false);
                _lurkerPicture.gameObject.SetActive(false);
                _buttonLurker.SetActive(false);
            }
        }
    }

    private void UpdateAllStepsVisuals()
    {
        if (_stepsContainer != null)
            Destroy(_stepsContainer);
        _stepsContainer = Instantiator.NewStepsContainer();
        var steps = _stepsService.GetAllSteps(_run);
        foreach (Step step in steps)
        {
            var stepInstance = Instantiator.NewStepInstance(step, _backgroundMask, _run, _character.MapAquired);
            stepInstance.transform.parent = _stepsContainer.transform;
            stepInstance.GetComponent<ButtonBhv>().EndActionDelegate = OnStepClicked;
        }
    }

    private void Pause()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewPauseMenu(ResumeGiveUp);
    }

    private void ResumeGiveUp(bool resume)
    {
        _musicControler.SetNewVolumeLevel();
        if (resume)
        {
            Paused = false;
            CameraBhv.Paused = false;
            Cache.NameLastScene = SceneManager.GetActiveScene().name;
            Destroy(_pauseMenu);
            return;
        }
        _pauseMenu.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        CameraBhv.transform.position = new Vector3(0.0f, 0.0f, CameraBhv.transform.position.z);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, true);
        if (GameObject.Find("PlayField") != null)
            Destroy(GameObject.Find("PlayField"));
        bool OnBlend(bool result)
        {
            PlayerPrefsHelper.SaveIsInFight(false);
            NavigationService.LoadBackUntil(Constants.MainMenuScene);
            return false;
        }
    }

    private void Info()
    {
        Paused = true;
        _musicControler.HalveVolume();
        _pauseMenu = Instantiator.NewInfoMenu(ResumeGiveUp, _character, null);
    }

    private void BeholderWarp()
    {
        var beholder = CharactersData.CustomCharacters.First(c => c.Name.Contains("Beholder"));
        if (!PlayerPrefsHelper.GetHasMetBeholder())
        {
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, beholder.Name, _character.Name, _character.StartingRealm, OffersWarp, customDialogLibelle: "The Beholder|FirstEncounter");
            PlayerPrefsHelper.SaveHasMetBeholder(true);
        }
        else
        {
            var alreadyDialog = PlayerPrefsHelper.GetAlreadyDialog();
            var dialogLibelle = $"{beholder.Name}|{_character.Name}";
            if (!alreadyDialog.Contains(dialogLibelle))
            {
                PlayerPrefsHelper.AddToAlreadyDialog(dialogLibelle);
                Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, beholder.Name, _character.Name, _character.StartingRealm, OffersWarp);
            }
            else
                OffersWarp();
        }

        void OffersWarp()
        {
            Helper.ReinitKeyboardInputs(this);
            Instantiator.NewPopupYesNo("Realm Warp", "the beholder offers you to warp to the next realm. do you accept?", "No", "Yes", OnWarp);
            return;
        }

        void OnWarp(bool result)
        {
            if (!result)
                return;

            _run.RealmLevel = 2;
            _run.IncreaseLevel(this._character);
            var currentItem = PlayerPrefsHelper.GetCurrentItem();
            if (currentItem != null)
                _run.CurrentItemUses = currentItem.Uses;
            if (currentItem != null && currentItem.Name == ItemsData.Items[25])
                ++_run.DeathScytheAscension;
            PlayerPrefsHelper.SaveRun(_run);
            NavigationService.LoadBackUntil(Constants.StepsAscensionScene);
        }
    }

    private void LurkerShop()
    {
        var lurker = CharactersData.CustomCharacters.First(c => c.Name.Contains("Lurker"));
        if (!PlayerPrefsHelper.GetHasMetLurker())
        {
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, lurker.Name, _character.Name, _character.StartingRealm, OfferShop, customDialogLibelle: "The Lurker|FirstEncounter");
            PlayerPrefsHelper.SaveHasMetLurker(true);
        }
        else
        {
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, lurker.Name, _character.Name, _character.StartingRealm, OfferShop, customDialogLibelle: $"The Lurker|Random{UnityEngine.Random.Range(1, 5)}");
        }

        void OfferShop()
        {
            Helper.ReinitKeyboardInputs(this);
            Instantiator.NewLurkerShop((result) =>
            {
                _run = PlayerPrefsHelper.GetRun();
                UpdateResourcesInfo();
            }, _character);
        }
    }

    private void GoToStep()
    {
        if (_selectedStep.LandLordVision)
        {
            Instantiator.NewPopupYesNo("Landlord Vision", "this area is under landlord vision! are you willing to continue?", "No", "Yes", OnLandLordVisionStep);
            return;
        }
        void OnLandLordVisionStep(bool result)
        {
            if (result)
            {
                _selectedStep.LandLordVision = false;
                var alreadyDialog = PlayerPrefsHelper.GetAlreadyDialog();
                var dialogLibelle = $"{_stepsService.GetBoss(_run)[0].Name}|{_character.Name}";
                if (!alreadyDialog.Contains(dialogLibelle))
                {
                    PlayerPrefsHelper.AddToAlreadyDialog(dialogLibelle);
                    Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, _stepsService.GetBoss(_run)[0].Name, _character.Name, _character.StartingRealm, () => { ResetGotoStep(); });
                }
                else
                    ResetGotoStep();
                StartCoroutine(Helper.ExecuteAfterDelay(0.0f, () => { _inputControler.InitMenuKeyboardInputs(); }));
            }

            void ResetGotoStep()
            {
#if !UNITY_ANDROID
                _inputControler.ResetMenuSelector();
#endif
                GoToStep();
            }
        }

        _run.X = _selectedStep.X;
        _run.Y = _selectedStep.Y;
        ++_run.CurrentStep;
        _stepsService.DiscoverStepOnPos(_selectedStep.X, _selectedStep.Y, _run);
        PlayerPrefsHelper.SaveRun(_run);
        //PlayerPrefsHelper.SaveCurrentOpponents(_selectedStep.Opponents);
        Cache.ResetClassicGameCache(_character);
        NavigationService.LoadNextScene(Constants.ClassicGameScene);
    }

    private void LootInfo()
    {
        string name;
        string cooldown;
        string description;
        if (_selectedStep.LootType == LootType.Character)
        {
            name = CharactersData.Characters[_selectedStep.LootId].Name;
            cooldown = $"{CharactersData.Characters[_selectedStep.LootId].Cooldown}\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}attack: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{CharactersData.Characters[_selectedStep.LootId].Attack}";
            description = CharactersData.Characters[_selectedStep.LootId].SpecialDescription;
        }
        else if (_selectedStep.LootType == LootType.Item)
        {
            var item = ItemsData.GetItemFromName(ItemsData.Items[_selectedStep.LootId]);
            name = item.Name;
            cooldown = item.Cooldown >= 0 ? item.Cooldown.ToString() : null;
            description = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + item.GetDescription();
        }
        else if (_selectedStep.LootType == LootType.Resource)
        {
            var resource = ResourcesData.GetResourceFromName(ResourcesData.Resources[_selectedStep.LootId]);
            name = resource.Name;
            cooldown = null;
            description = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + resource.Description;
        }
        else if (_selectedStep.LootType == LootType.Pact)
        {
            var pact = PactsData.GetPactFromName(PactsData.Pacts[_selectedStep.LootId]);
            name = pact.Name;
            cooldown = null;
            description = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + pact.FullDescription();
        }
        else if (_selectedStep.LootType == LootType.Tattoo)
        {
            var tattoo = TattoosData.GetTattooFromName(TattoosData.Tattoos[_selectedStep.LootId]);
            name = tattoo.Name;
            cooldown = null;
            var upgradable = tattoo.MaxLevel > 1 ? $"\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}[upgradable]" : "";
            description = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + tattoo.GetDescription() + upgradable;
        }
        else
            return;
        Instantiator.NewPopupYesNo(name, description + (cooldown != null ?
            ($"\n---\n{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}cooldown: {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{cooldown}{Constants.MaterialEnd}")
            : ""), null, "Ok", null);
    }

    private void OnBossTriggered()
    {

    }
}
