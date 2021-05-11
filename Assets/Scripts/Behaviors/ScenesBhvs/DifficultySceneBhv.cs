using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultySceneBhv : SceneBhv
{
    private TMPro.TextMeshPro _difficultyLibelle;
    private TMPro.TextMeshPro _difficulty;
    private TMPro.TextMeshPro _resources;
    private TMPro.TextMeshPro _realmSteps;
    private TMPro.TextMeshPro _cooldowns;
    private TMPro.TextMeshPro _hpMax;
    private TMPro.TextMeshPro _gravity;

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
        SelectDifficulty(lastSelectedDifficulty.GetHashCode());
    }

    private void SetButtons()
    {
        GameObject.Find(Difficulty.Easy.ToString()).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(0); };
        GameObject.Find(Difficulty.Normal.ToString()).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(1); };
        GameObject.Find(Difficulty.Hard.ToString()).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(2); };

        _difficultyLibelle = GameObject.Find("DifficultyLibelle").GetComponent<TMPro.TextMeshPro>();
        _difficulty = GameObject.Find("Difficulty").GetComponent<TMPro.TextMeshPro>();
        _resources = GameObject.Find("Resources").GetComponent<TMPro.TextMeshPro>();
        _realmSteps = GameObject.Find("RealmSteps").GetComponent<TMPro.TextMeshPro>();
        _cooldowns = GameObject.Find("Cooldowns").GetComponent<TMPro.TextMeshPro>();
        _hpMax = GameObject.Find("HpMax").GetComponent<TMPro.TextMeshPro>();
        _gravity = GameObject.Find("Gravity").GetComponent<TMPro.TextMeshPro>();

        GameObject.Find(Constants.GoButtonBackName).GetComponent<ButtonBhv>().EndActionDelegate = GoToPrevious;
        GameObject.Find(Constants.GoButtonPlayName).GetComponent<ButtonBhv>().EndActionDelegate = Play;
    }

    private void SelectDifficulty(int id)
    {
        var difficulty = (Difficulty)id;
        for (int i = 0; i < Helper.EnumCount<Difficulty>(); ++i)
        {
            var button = GameObject.Find(((Difficulty)i).ToString());
            if (i == id)
                button.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Constants.ColorPlain;
            else
                button.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Constants.ColorPlainSemiTransparent;
        }
        if (difficulty == Difficulty.Easy)
        {
            _difficultyLibelle.text = $"I'm learning the mechanics.";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x1";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}6";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}Longer";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Lower";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Weaker";
        }
        else if (difficulty == Difficulty.Normal)
        {
            _difficultyLibelle.text = $"I know this game. Bring it on!";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x2";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}5";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}Normal";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Normal";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Normal";
        }
        else if (difficulty == Difficulty.Hard)
        {
            _difficultyLibelle.text = $"I would like some pain please.";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x3";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}4";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd}Shorter";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd}Higher";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd}Stronger";
        }
        PlayerPrefsHelper.SaveDifficulty(difficulty);
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
        PlayerPrefsHelper.SaveRun(new Run(PlayerPrefsHelper.GetDifficulty()));
        var randomItemMaxRarity = PlayerPrefsHelper.GetRealmBossProgression();
        if (randomItemMaxRarity >= 0)
            PlayerPrefsHelper.SaveCurrentItem(ItemsData.GetRandomItem((Rarity)randomItemMaxRarity).Name);
        Instantiator.NewOverBlend(OverBlendType.StartLoadingActionEnd, "Ascending", 2, OnBlend);
        object OnBlend(bool result)
        {
            NavigationService.LoadNextScene(Constants.StepsAscensionScene);
            return true;
        }
    }
}
