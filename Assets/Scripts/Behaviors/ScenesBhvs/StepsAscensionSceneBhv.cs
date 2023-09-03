using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepsAscensionSceneBhv : SceneBhv
{
    private Run _run;
    private Character _character;
    private TMPro.TextMeshPro _infoRealm;
    private SpriteRenderer _characterPicture;
    private GameObject _levelsContainer;

    private float _levelHeight;
    private Vector3 _levelsContainerTarget;
    private Vector3 _characterPictureTarget;
    private Vector3 _infoRealmTarget;
    private bool _isAnimated;

    public override MusicType MusicType => MusicType.Ascension;

    private void Start()
    {
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _run = PlayerPrefsHelper.GetRun();
        _character = PlayerPrefsHelper.GetRunCharacter();

        _infoRealm = GameObject.Find("InfoRealm").GetComponent<TMPro.TextMeshPro>();
        _infoRealm.text = $"{Constants.GetMaterial(_run.CurrentRealm, TextType.succubus3x5, TextCode.c43)}{_run.CurrentRealm.ToString().ToLower()}\nlvl {_run.RealmLevel}";
        _characterPicture = GameObject.Find("CharacterPicture").GetComponent<SpriteRenderer>();
        _characterPicture.sprite = Helper.GetCharacterSkin(_character.Id, _character.SkinId);

        _levelHeight = Constants.Pixel * 11;
        _levelsContainer = GameObject.Find("LevelsContainer");
        int idLayer = 0;
        for (int idRealm = 0; idRealm <= (int)_run.CurrentRealm; ++idRealm)
        {
            var maxLevel = 2;
            if (idRealm == (int)_run.CurrentRealm)
                maxLevel = _run.RealmLevel;
            for (int idLevel = 0; idLevel < maxLevel; ++idLevel)
                Instantiator.NewLevel((_levelHeight * idLevel) + (_levelHeight * 2 * idRealm), idRealm, idLayer++, _levelsContainer);
        }
        var yTarget = _levelHeight * _levelsContainer.transform.childCount;
        _levelsContainerTarget = new Vector3(0.0f, _levelsContainer.transform.position.y - yTarget, 0.0f);
        _characterPictureTarget = _characterPicture.transform.position + new Vector3(-5.0f, 5.0f, 0.0f);
        _infoRealmTarget = _infoRealm.transform.position + new Vector3(5.0f, 5.0f, 0.0f);

        PlayerPrefsHelper.AlterTotalResource((int)Realm.Hell, 0); // (OBSOLETE but usefull) In order not to get the lore intro cinematic again (obsolete, but I let it here this way in order to get something else than null)

        _isAnimated = true;
    }

    override protected void FrameUpdate()
    {
        if (_isAnimated)
            DecreaseLevelsContainer();
    }

    private void DecreaseLevelsContainer()
    {
        _levelsContainer.transform.position = Vector3.Lerp(_levelsContainer.transform.position, _levelsContainerTarget, 0.1f);
        _characterPicture.transform.position = Vector3.Lerp(_characterPicture.transform.position, _characterPictureTarget, 0.15f);
        _characterPicture.color = Color.Lerp(_characterPicture.color, Constants.ColorPlain, 0.15f);
        _infoRealm.transform.position = Vector3.Lerp(_infoRealm.transform.position, _infoRealmTarget, 0.15f);
        _infoRealm.color = Color.Lerp(_infoRealm.color, Constants.ColorPlain, 0.15f);
        if (Helper.FloatEqualsPrecision(_levelsContainer.transform.position.y, _levelsContainerTarget.y, 0.1f))
        {
            var lastLevel = _levelsContainer.transform.GetChild(_levelsContainer.transform.childCount - 1);
            lastLevel.GetComponent<IconInstanceBhv>().Pop();
            Invoke(nameof(LoadNextScene), 2.75f);
            _isAnimated = false;
        }
    }

    private void LoadNextScene()
    {
        if (PlayerPrefsHelper.GetIsInFight())
            NavigationService.LoadNextScene(Constants.ClassicGameScene);
        else
            NavigationService.LoadNextScene(Constants.StepsScene);
    }
}
