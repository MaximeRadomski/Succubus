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

    private TMPro.TextMeshPro _difficultyLibelle;
    private TMPro.TextMeshPro _difficulty;
    private TMPro.TextMeshPro _resources;
    private TMPro.TextMeshPro _realmSteps;
    private TMPro.TextMeshPro _cooldowns;
    private TMPro.TextMeshPro _hpMax;
    private TMPro.TextMeshPro _gravity;

    private float _yOffset = 4.0f;

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
        (_easy = GameObject.Find(Difficulty.Easy.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(0); };
        (_normal = GameObject.Find(Difficulty.Normal.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(1); };
        (_hard = GameObject.Find(Difficulty.Hard.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(2); };
        (_infernal = GameObject.Find(Difficulty.Infernal.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(3); };
        (_divine = GameObject.Find(Difficulty.Divine.ToString())).GetComponent<ButtonBhv>().EndActionDelegate = () => { SelectDifficulty(4); };

        _difficultyLibelle = GameObject.Find("DifficultyLibelle").GetComponent<TMPro.TextMeshPro>();
        _difficulty = GameObject.Find("Difficulty").GetComponent<TMPro.TextMeshPro>();
        _resources = GameObject.Find("Resources").GetComponent<TMPro.TextMeshPro>();
        _realmSteps = GameObject.Find("RealmSteps").GetComponent<TMPro.TextMeshPro>();
        _cooldowns = GameObject.Find("Cooldowns").GetComponent<TMPro.TextMeshPro>();
        _hpMax = GameObject.Find("HpMax").GetComponent<TMPro.TextMeshPro>();
        _gravity = GameObject.Find("Gravity").GetComponent<TMPro.TextMeshPro>();

        if (PlayerPrefsHelper.GetInfernalUnlocked())
        {
            GameObject.Find("SelectADifficulty").transform.position = new Vector3(50.0f, 50.0f, 0.0f);
            _easy.transform.position += new Vector3(0.0f, _yOffset, 0.0f);
            _normal.transform.position = _easy.transform.position + new Vector3(0.0f, -(_easy.GetComponent<BoxCollider2D>().size.y / 2.0f) - (Constants.Pixel * 3) - (_normal.GetComponent<BoxCollider2D>().size.y / 2.0f), 0.0f);
            _hard.transform.position = _normal.transform.position + new Vector3(0.0f, -(_normal.GetComponent<BoxCollider2D>().size.y / 2.0f) - (Constants.Pixel * 3) - (_hard.GetComponent<BoxCollider2D>().size.y / 2.0f), 0.0f);
            _infernal.transform.position = _hard.transform.position + new Vector3(0.0f, -(_hard.GetComponent<BoxCollider2D>().size.y / 2.0f) - (Constants.Pixel * 3) - (_infernal.GetComponent<BoxCollider2D>().size.y / 2.0f), 0.0f);
        }

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
            _difficultyLibelle.text = $"Wait what?";
            _difficulty.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Difficulty: {Constants.MaterialEnd}{difficulty}";
            _resources.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Resources: {Constants.MaterialEnd}x5";
            _realmSteps.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Realm steps: {Constants.MaterialEnd}1";
            _cooldowns.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Cooldowns: {Constants.MaterialEnd} ? ";
            _hpMax.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Max HP: {Constants.MaterialEnd} ? ";
            _gravity.text = $"{Constants.GetMaterial(Realm.Hell, TextType.AbjectLong, TextCode.c32)}Gravity: {Constants.MaterialEnd} ? ";
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
