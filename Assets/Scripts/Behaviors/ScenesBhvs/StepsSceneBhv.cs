using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StepsSceneBhv : SceneBhv
{
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
    private GameObject _buttonInfo;
    private InputControlerBhv _inputControler;

    private Step _selectedStep;
    private float _lootCenterLocalY;

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
        _buttonInfo = GameObject.Find("ButtonInfo");
        _buttonInfo.GetComponent<ButtonBhv>().EndActionDelegate = Info;
        (_playButton = GameObject.Find(Constants.GoButtonPlayName)).GetComponent<ButtonBhv>().EndActionDelegate = GoToStep;
        _selector = GameObject.Find("Selector");
        _position = GameObject.Find("Position");
        _inputControler = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        _backgroundMask = GameObject.Find("BackgroundMask");

        _stepsService = new StepsService();
        _selector.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_" + (0 + (9 * _character.Realm.GetHashCode())));
        _stepsBackground = GameObject.Find("StepsBackground");
        if (string.IsNullOrEmpty(_run.Steps))
        {
            _stepsService.GenerateOriginSteps(_run, _character);
            PlayerPrefsHelper.SaveRun(_run);
        }
        else if (_run.CurrentStep == _run.MaxSteps)
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
            else if (_selectedStep.LootType == LootType.Tattoo)
            {
                rarity = TattoosData.GetTattooFromName(TattoosData.Tattoos[_selectedStep.LootId]).Rarity;
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + _selectedStep.LootId);
                _lootPicture.transform.localPosition = new Vector3(_lootPicture.transform.localPosition.x, _lootCenterLocalY, 0.0f);
                _lootName.text = TattoosData.Tattoos[_selectedStep.LootId].ToLower();
            }
            _lootTypeRarity.text = _selectedStep.LootType.ToString().ToLower() + "\n" + Constants.MaterialHell_4_3 + rarity.ToString().ToLower();
            _opponents.text = "opponents\n" + Constants.MaterialHell_4_3 + ((OpponentType)rarity.GetHashCode()).ToString().ToLower();
            _opponentType.sprite = rarity == Rarity.Common ? null : Helper.GetSpriteFromSpriteSheet("Sprites/OpponentTypes_" + ((_run.CurrentRealm.GetHashCode() * 3) + (rarity.GetHashCode() - 1)));
            _lootPicture.GetComponent<IconInstanceBhv>().Pop();
        }
        else
        {
            if (x == _run.X && y == _run.Y)
            {
                _lootPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/StepsAssets_2");
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
                _lootTypeRarity.text = " - \n" + Constants.MaterialHell_4_3 + " - ";
                _opponents.text = _lootTypeRarity.text;
                _opponentType.sprite = null;
                _characterPicture.gameObject.SetActive(false);
                _buttonInfo.SetActive(false);
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
            var stepInstance = Instantiator.NewStepInstance(step, _backgroundMask);
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

    private void GoToStep()
    {
        if (_selectedStep.LandLordVision)
        {
            Instantiator.NewPopupYesNo("Landlord Vision", "this area is under the landlord vision! are you willing to continue?", "No", "Yes", OnLandLordVisionStep);
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
                    Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, _stepsService.GetBoss(_run)[0].Name, _character.Name, () => { ResetGotoStep(); return true; });
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
            cooldown = $"{CharactersData.Characters[_selectedStep.LootId].Cooldown}\nattack: {CharactersData.Characters[_selectedStep.LootId].Attack}";
            description = CharactersData.Characters[_selectedStep.LootId].SpecialDescription;
        }
        else if (_selectedStep.LootType == LootType.Item)
        {
            var item = ItemsData.GetItemFromName(ItemsData.Items[_selectedStep.LootId]);
            name = item.Name;
            cooldown = item.Cooldown.ToString();
            description = Constants.MaterialHell_3_2 + item.Description;
        }
        else if (_selectedStep.LootType == LootType.Tattoo)
        {
            var tattoo = TattoosData.GetTattooFromName(TattoosData.Tattoos[_selectedStep.LootId]);
            name = tattoo.Name;
            cooldown = null;
            description = Constants.MaterialHell_3_2 + tattoo.GetDescription();
        }
        else
            return;
        Instantiator.NewPopupYesNo(name, description.ToLower() + (cooldown != null ?
            ($"\n---\n{Constants.MaterialHell_3_2}cooldown: {Constants.MaterialHell_4_3}{cooldown}{Constants.MaterialEnd}")
            : ""), null, "Ok", null);
    }

    private void OnBossTriggered()
    {

    }
}
