using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySceneBhv : SceneBhv
{
    private GameObject _easy;
    private GameObject _normal;
    private GameObject _hard;
    private GameObject _infernal;
    private GameObject _divine;
    private GameObject _buttonNext;
    private GameObject _buttonPrevious;
    private GameObject _buttonPlay;
    private GameObject _baseDifficultiesContainer;
    private GameObject _advancedDifficultiesContainer;

    private TMPro.TextMeshPro _difficultyLibelle;
    private TMPro.TextMeshPro _difficulty;
    private TMPro.TextMeshPro _resources;
    private TMPro.TextMeshPro _realmSteps;
    private TMPro.TextMeshPro _cooldowns;
    private TMPro.TextMeshPro _hpMax;
    private TMPro.TextMeshPro _gravity;
    private TMPro.TextMeshPro _infoStr;

    private InputControlerBhv _inputControlerBhv;

    public override MusicType MusicType => MusicType.Menu;

    void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        SetButtons();

        var lastSelectedDifficulty = PlayerPrefsHelper.GetDifficulty();
        if (lastSelectedDifficulty > Difficulty.Hard)
            Next();
        SelectDifficulty((int)lastSelectedDifficulty);
    }

    private void SetButtons()
    {
        (_easy = GameObject.Find(Difficulty.Easy.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(0); };
        (_normal = GameObject.Find(Difficulty.Normal.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(1); };
        (_hard = GameObject.Find(Difficulty.Hard.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(2); };
        (_infernal = GameObject.Find(Difficulty.Infernal.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(3); };
        (_divine = GameObject.Find(Difficulty.Divine.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(4); };
        (_buttonNext = GameObject.Find("ButtonNext")).GetComponent<ButtonBhv>().EndActionDelegate = Next;
        _buttonNext.SetActive(PlayerPrefsHelper.GetInfernalUnlocked());
        (_buttonPrevious = GameObject.Find("ButtonPrevious")).GetComponent<ButtonBhv>().EndActionDelegate = Previous;
        _buttonPrevious.SetActive(false);
        _baseDifficultiesContainer = GameObject.Find("BaseDifficultiesContainer");
        _advancedDifficultiesContainer = GameObject.Find("AdvancedDifficultiesContainer");

        _difficultyLibelle = GameObject.Find("DifficultyLibelle").GetComponent<TMPro.TextMeshPro>();
        _difficulty = GameObject.Find("Difficulty").GetComponent<TMPro.TextMeshPro>();
        _resources = GameObject.Find("Resources").GetComponent<TMPro.TextMeshPro>();
        _realmSteps = GameObject.Find("RealmSteps").GetComponent<TMPro.TextMeshPro>();
        _cooldowns = GameObject.Find("Cooldowns").GetComponent<TMPro.TextMeshPro>();
        _hpMax = GameObject.Find("HpMax").GetComponent<TMPro.TextMeshPro>();
        _gravity = GameObject.Find("Gravity").GetComponent<TMPro.TextMeshPro>();
        _infoStr = GameObject.Find("InfoStr").GetComponent<TMPro.TextMeshPro>();

        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        (_buttonPlay = GameObject.Find(Constants.GoButtonPlayName)).GetComponent<ButtonBhv>().EndActionDelegate = Play;

        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
    }

    private void SelectDifficulty(int id)
    {
        var difficulty = (Difficulty)id;
        for (int i = 0; i < Helper.EnumCount<Difficulty>(); ++i)
        {
            var button = GameObject.Find(((Difficulty)i).ToString());
            if (button == null)
                continue;
            if (i == id)
                button.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Constants.ColorPlain;
            else
                button.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Constants.ColorPlainQuarterTransparent;
        }
        if (difficulty == Difficulty.Easy)
        {
            _difficultyLibelle.text = $"I'm learning the mechanics.";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x1";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}5";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}Longer";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Lower";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Weaker";
        }
        else if (difficulty == Difficulty.Normal)
        {
            _difficultyLibelle.text = $"I know this game. Bring it on!";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x2";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}4";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}Normal";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Normal";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Normal";
        }
        else if (difficulty == Difficulty.Hard)
        {
            _difficultyLibelle.text = $"I would like some pain please.";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x3";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}3";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}Shorter";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Higher";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Stronger";
        }
        else if (difficulty == Difficulty.Infernal)
        {
            _difficultyLibelle.text = $"I'm all about unfair challenge.";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x4";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}2";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}What?";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Yes";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Stomp";
        }
        else if (difficulty == Difficulty.Divine)
        {
            _difficultyLibelle.text = $"Wait...";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x5";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}1";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd} !? ";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd} !? ";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd} !? ";
        }
        var infoStr = string.Empty;
        if (difficulty >= Difficulty.Infernal)
            infoStr = "stronger opponents attacks.";
        if (difficulty >= Difficulty.Divine)
            infoStr = "even stronger opponents attacks.";
        _infoStr.text = infoStr;
        PlayerPrefsHelper.SaveDifficulty(difficulty);
    }

    private void GoToPrevious()
    {
        Instantiator.NewOverBlend(OverBlendType.StartLoadMidActionEnd, "", null, OnBlend, reverse: true);
        bool OnBlend(bool result)
        {
            NavigationService.LoadPreviousScene();
            return true;
        }
    }

    private void Play()
    {
        var run = new Run(PlayerPrefsHelper.GetDifficulty());
        var randomItemMaxRarity = PlayerPrefsHelper.GetRealmBossProgression();
        if (randomItemMaxRarity >= 0)
        {
            var item = ItemsData.GetRandomItem((Rarity)randomItemMaxRarity);
            PlayerPrefsHelper.SaveCurrentItem(item.Name);
            if (item.Type == ItemType.UsesBased)
                run.CurrentItemUses = item.Uses;
        }
        PlayerPrefsHelper.SaveRun(run);
        Cache.AlreadyConfrontedOpponents = null;
        Instantiator.NewOverBlend(OverBlendType.StartLoadingActionEnd, "Ascending", 2, OnBlend);
        bool OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.StepsAscensionScene);
            return true;
        }
    }
    private void Previous()
    {
        _buttonPlay.GetComponent<ButtonBhv>().IsMenuSelectorResetButton = false;
        _buttonPrevious.SetActive(false);
        _buttonNext.SetActive(true);
        var tmpPos = _baseDifficultiesContainer.transform.position;
        _baseDifficultiesContainer.transform.position = _advancedDifficultiesContainer.transform.position;
        _advancedDifficultiesContainer.transform.position = tmpPos;
        _inputControlerBhv.InitMenuKeyboardInputs(_buttonNext.transform.position + new Vector3(0.0f, 1.0f, 0.0f));
    }

    private void Next()
    {
        _buttonPlay.GetComponent<ButtonBhv>().IsMenuSelectorResetButton = false;
        _buttonPrevious.SetActive(true);
        _buttonNext.SetActive(false);
        var tmpPos = _advancedDifficultiesContainer.transform.position;
        _advancedDifficultiesContainer.transform.position = _baseDifficultiesContainer.transform.position;
        _baseDifficultiesContainer.transform.position = tmpPos;
        _inputControlerBhv.InitMenuKeyboardInputs(_buttonPrevious.transform.position + new Vector3(0.0f, 1.1f, 0.0f));
    }
}
