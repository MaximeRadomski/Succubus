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
    private GameObject _buttonInfo;
    private GameObject _buttonBeholder;
    private InputControlerBhv _inputControler;

    private Step _selectedStep;
    private float _lootCenterLocalY;

    private float _lastSelectedInput = -1;

    public override MusicType MusicType => MusicType.Steps;

    void Start()
    {
        Init();
        if (Constants.NameLastScene == Constants.SettingsScene)
            PauseOrPrevious();
    }

    protected override void Init()
    {
        base.Init();
        CameraBhv.Paused = Constants.NameLastScene == Constants.SettingsScene;
        _run = PlayerPrefsHelper.GetRun();
        _character = PlayerPrefsHelper.GetRunCharacter();

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
        _characterPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _character.Id);
        _characterPicture.GetComponent<ButtonBhv>().EndActionDelegate = Info;
        _buttonInfo = GameObject.Find(Constants.GoButtonInfoName);
        _buttonInfo.GetComponent<ButtonBhv>().EndActionDelegate = Info;
        _beholderPicture = GameObject.Find("BeholderPicture").GetComponent<SpriteRenderer>();
        _beholderPicture.GetComponent<ButtonBhv>().EndActionDelegate = BeholderWarp;
        _buttonBeholder = GameObject.Find("ButtonBeholder");
        _buttonBeholder.GetComponent<ButtonBhv>().EndActionDelegate = BeholderWarp;
        (_playButton = GameObject.Find(Constants.GoButtonPlayName)).GetComponent<ButtonBhv>().EndActionDelegate = GoToStep;
        _selector = GameObject.Find("Selector");
        _position = GameObject.Find("Position");
        _position.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (2 + (_run.CurrentRealm.GetHashCode() * 10)));
        _inputControler = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        _backgroundMask = GameObject.Find("BackgroundMask");

        var resources = _run.GetRunResources();
        for (int i = 0; i < resources.Count; ++i)
        {
            if (resources[i] <= 0)
                GameObject.Find($"Resource{i}").SetActive(false);
            else
                GameObject.Find($"Resource{i}").transform.GetChild(0).GetComponent<TMPro.TextMeshPro>().text = resources[i].ToString();
        }
        _stepsService = new StepsService();
        _selector.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (0 + (10 * _run.CurrentRealm.GetHashCode())));
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
            Constants.InputLocked = true;
            Invoke(nameof(OnBossTriggered), 1.0f);
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
        var lastClicked = Constants.LastEndActionClickedName;
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
            return true;
        }));
        _playButton.SetActive(_selectedStep.LootType != LootType.None);
        var rarity = Rarity.Common;
        if (_selectedStep.LootType != LootType.None)
        {
            _characterPicture.gameObject.SetActive(false);
            _buttonInfo.SetActive(false);
            _beholderPicture.gameObject.SetActive(false);
            _buttonBeholder.SetActive(false);
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
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (2 + (_run.CurrentRealm.GetHashCode() * 10)));
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

            if (x == 50 && y == 50 && PlayerPrefsHelper.GetRealmBossProgression() >= _run.CurrentRealm.GetHashCode() && _run.RealmLevel == 1 && _run.CurrentRealm == Realm.Hell /*DEBUG*/)
            {
                _beholderPicture.gameObject.SetActive(true);
                _buttonBeholder.SetActive(true);
            }
            else
            {
                _beholderPicture.gameObject.SetActive(false);
                _buttonBeholder.SetActive(false);
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

    private object ResumeGiveUp(bool resume)
    {
        _musicControler.SetNewVolumeLevel();
        if (resume)
        {
            Paused = false;
            CameraBhv.Paused = false;
            Constants.NameLastScene = SceneManager.GetActiveScene().name;
            Destroy(_pauseMenu);
            return true;
        }
        _pauseMenu.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        CameraBhv.transform.position = new Vector3(0.0f, 0.0f, CameraBhv.transform.position.z);
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, true);
        if (GameObject.Find("PlayField") != null)
            Destroy(GameObject.Find("PlayField"));
        object OnBlend(bool result)
        {
            PlayerPrefsHelper.SaveIsInFight(false);
            NavigationService.LoadBackUntil(Constants.MainMenuScene);
            return false;
        }
        return false;
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

        bool OffersWarp()
        {
            StartCoroutine(Helper.ExecuteAfterDelay(0.0f, () => { GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>().InitMenuKeyboardInputs(); return true; }));
            Instantiator.NewPopupYesNo("Realm Warp", "the beholder offers you to warp to the next realm. do you accept?", "No", "Yes", OnWarp);
            return true;
        }

        object OnWarp(bool result)
        {
            if (!result)
                return false;

            _run.RealmLevel = 3;
            _run.IncreaseLevel();
            var currentItem = PlayerPrefsHelper.GetCurrentItem();
            if (currentItem != null)
                _run.CurrentItemUses = currentItem.Uses;
            if (currentItem != null && currentItem.Name == ItemsData.Items[25])
                ++_run.DeathScytheAscension;
            PlayerPrefsHelper.SaveRun(_run);
            NavigationService.LoadBackUntil(Constants.StepsAscensionScene);
            return true;
        }
    }

    private void GoToStep()
    {
        if (_selectedStep.LandLordVision)
        {
            Instantiator.NewPopupYesNo("Landlord Vision", "this area is under landlord vision! are you willing to continue?", "No", "Yes", OnLandLordVisionStep);
            return;
        }
        object OnLandLordVisionStep(bool result)
        {
            if (result)
            {
                _selectedStep.LandLordVision = false;
                var alreadyDialog = PlayerPrefsHelper.GetAlreadyDialog();
                var dialogLibelle = $"{_stepsService.GetBoss(_run)[0].Name}|{_character.Name}";
                if (!alreadyDialog.Contains(dialogLibelle))
                {
                    PlayerPrefsHelper.AddToAlreadyDialog(dialogLibelle);
                    Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, _stepsService.GetBoss(_run)[0].Name, _character.Name, _character.StartingRealm, () => { ResetGotoStep(); return true; });
                }
                else
                    ResetGotoStep();
                StartCoroutine(Helper.ExecuteAfterDelay(0.0f, () => { _inputControler.InitMenuKeyboardInputs(); return true; }));
            }

            void ResetGotoStep()
            {
#if !UNITY_ANDROID
                _inputControler.ResetMenuSelector();
#endif
                GoToStep();
            }

            return result;
        }

        _run.X = _selectedStep.X;
        _run.Y = _selectedStep.Y;
        ++_run.CurrentStep;
        _stepsService.DiscoverStepOnPos(_selectedStep.X, _selectedStep.Y, _run);
        PlayerPrefsHelper.SaveRun(_run);
        //PlayerPrefsHelper.SaveCurrentOpponents(_selectedStep.Opponents);
        Constants.ResetClassicGameCache(_character);
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
        else if (_selectedStep.LootType == LootType.Tattoo)
        {
            var tattoo = TattoosData.GetTattooFromName(TattoosData.Tattoos[_selectedStep.LootId]);
            name = tattoo.Name;
            cooldown = null;
            description = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + tattoo.GetDescription();
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
