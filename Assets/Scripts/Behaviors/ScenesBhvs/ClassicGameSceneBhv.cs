using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicGameSceneBhv : GameSceneBhv
{
    public CharacterInstanceBhv OpponentInstanceBhv;

    private ResourceBarBhv _opponentHpBar;
    private ResourceBarBhv _opponentCooldownBar;
    private SpriteRenderer _opponentType;
    private bool _opponentOnCooldown;
    private float _nextCooldownTick;
    private IconInstanceBhv _weaknessInstance;
    private IconInstanceBhv _immunityInstance;
    private WiggleBhv _stunIcon;

    private Run _run;
    private RealmTree _realmTree;
    private StepsService _stepsService;
    private Step _currentStep;

    private int _characterAttack;
    private int _wetMalus;
    private float _wetTimer;
    private float _timeStopTimer;
    private bool _isCrit;
    private bool _isVictorious;

    private SoundControlerBhv _soundControler;
    private int _idOpponentDeath;
    private int _idOpponentAppearance;
    private int _idHit;
    private int _idCrit;
    private int _idWeakness;
    private int _idImmunity;
    private int _idDodge;
    private int _idTattooSound;

    public override MusicType MusicType => GetMusicType();

    private MusicType GetMusicType()
    {
        if (_stepsService == null)
            _stepsService = new StepsService();
        if (_run == null)
            _run = PlayerPrefsHelper.GetRun();
        if (_run == null)
            return MusicType.Game;
        var currentStep = _stepsService.GetStepOnPos(_run.X, _run.Y, _run.Steps);
        if (currentStep.LandLordVision)
            return MusicType.Boss;
        return MusicType.Game;
    }

    void Start()
    {
        Init();
        if (_run == null)
            _run = PlayerPrefsHelper.GetRun();
        _realmTree = PlayerPrefsHelper.GetRealmTree();
        if (_stepsService == null)
            _stepsService = new StepsService();
        if (Constants.CurrentGameMode == GameMode.TrainingDummy
            || Constants.CurrentGameMode == GameMode.TrainingFree)
        {
            _opponents = PlayerPrefsHelper.GetCurrentOpponents(new Run(Difficulty.Normal));
            Constants.RestartCurrentItemCooldown(Character, ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]));
        }
        else
        {
            PlayerPrefsHelper.SaveIsInFight(true);
            _currentStep = _stepsService.GetStepOnPos(_run.X, _run.Y, _run.Steps);
            if (_currentStep.LandLordVision)
                _opponents = _stepsService.GetBoss(_run);
            else
                _opponents = _currentStep.Opponents;
            Constants.CurrentItemCooldown = _run.CurrentItemCooldown;
            Constants.CurrentItemUses = _run.CurrentItemUses;
        }
        //if (_opponents.Count == 1)
        //    GameObject.Find("Enemies").GetComponent<TMPro.TextMeshPro>().text = "enemy";
        for (int i = _opponents.Count; i < 12; ++i)
        {
            GameObject.Find("Opponent" + i).SetActive(false);
        }
        for (int j = 0; j < _opponents.Count; ++j)
        {
            if (j < Constants.CurrentListOpponentsId)
                GameObject.Find("Opponent" + j).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/OpponentsIcons_" + ((_opponents[j].Realm.GetHashCode() * 2) + 1));
            else
                GameObject.Find("Opponent" + j).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/OpponentsIcons_" + (_opponents[j].Realm.GetHashCode() * 2));
        }
        _wetTimer = -1;
        _timeStopTimer = -1;
        _weaknessInstance = GameObject.Find("Weakness").GetComponent<IconInstanceBhv>();
        _immunityInstance = GameObject.Find("Immunity").GetComponent<IconInstanceBhv>();
        _stunIcon = GameObject.Find("StunIcon").GetComponent<WiggleBhv>();
        _opponentHpBar = GameObject.Find("OpponentHpBar").GetComponent<ResourceBarBhv>();
        _opponentCooldownBar = GameObject.Find("OpponentCooldownBar").GetComponent<ResourceBarBhv>();
        OpponentInstanceBhv = GameObject.Find(Constants.GoOpponentInstance).GetComponent<CharacterInstanceBhv>();
        OpponentInstanceBhv.AfterDeath = AfterOpponentDeath;
        _opponentType = GameObject.Find("OpponentType").GetComponent<SpriteRenderer>();
        _nextCooldownTick = Time.time + 3600;
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _idOpponentDeath = _soundControler.SetSound("OpponentDeath");
        _idOpponentAppearance = _soundControler.SetSound("OpponentAppearance");
        _idCrit = _soundControler.SetSound("Crit");
        _idHit = _soundControler.SetSound("Hit");
        _idWeakness = _soundControler.SetSound("Weakness");
        _idImmunity = _soundControler.SetSound("Immunity");
        _idDodge = _soundControler.SetSound("LevelUp");
        _idTattooSound = _soundControler.SetSound("TattooSound");
        GameObject.Find("InfoRealm").GetComponent<TMPro.TextMeshPro>().text = $"{Constants.GetMaterial(_run?.CurrentRealm ?? Realm.Hell, TextType.succubus3x5, TextCode.c32B)}realm:\n{ Constants.GetMaterial(_run?.CurrentRealm ?? Realm.Hell, TextType.succubus3x5, TextCode.c43B)}{ (_run?.CurrentRealm.ToString().ToLower() ?? Realm.Hell.ToString().ToLower())}\nlvl {_run?.RealmLevel.ToString() ?? "?"}";
        NextOpponent(sceneInit: true);
        _gameplayControler.GetComponent<GameplayControler>().StartGameplay(CurrentOpponent.GravityLevel, Character.Realm, _run?.CurrentRealm ?? Realm.Hell);

        Paused = true;
        _musicControler.Stop();
        Constants.InputLocked = true;
        if (Constants.NameLastScene != Constants.SettingsScene)
        {
            Constants.CurrentRemainingSimpShields = Character.SimpShield;
            Instantiator.NewFightIntro(new Vector3(CameraBhv.transform.position.x, CameraBhv.transform.position.y, 0.0f), Character, _opponents, AfterFightIntro);
        }
        else
            _musicControler.Play();

        if (Constants.CurrentRemainingSimpShields > 0)
        {
            for (int i = 0; i < Constants.CurrentRemainingSimpShields; ++i)
            {
                Instantiator.NewSimpShield(_characterInstanceBhv.OriginalPosition, new Vector3(-1.5f + (1.5f * i), -2.6f, 0.0f), Character.Realm, 3 - i);
            }
        }
    }

    private bool AfterFightIntro()
    {
        if (CurrentOpponent.Haste)
            Instantiator.PopText("haste", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
        Constants.InputLocked = false;        
        Paused = false;
        OpponentAppearance();
        return true;
    }

    private void OpponentAppearance(float customY = 9.0f)
    {
        _musicControler.Play();
        var alreadyDialog = PlayerPrefsHelper.GetAlreadyDialog();
        if ((_currentStep == null || !_currentStep.LandLordVision)
            && (DialogsData.DialogTree.ContainsKey($"{CurrentOpponent.Name}|{Character.Name}") ||  DialogsData.DialogTree.ContainsKey($"{CurrentOpponent.Name}|Any"))
            && !alreadyDialog.Contains($"{CurrentOpponent.Name}|{Character.Name}"))
        {
            Paused = true;
            PlayerPrefsHelper.AddToAlreadyDialog($"{CurrentOpponent.Name}|{Character.Name}");
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, CurrentOpponent.Name, Character.Name, Appearance);
        }
        else
            Appearance();

        bool Appearance()
        {
            Paused = false;
            Instantiator.PopText(CurrentOpponent.Name.ToLower() + " appears!", new Vector2(4.5f, customY));
            return true;
        }
    }

    protected void NextOpponent(bool sceneInit = false)
    {
        if (Constants.CurrentListOpponentsId >= _opponents.Count)
        {
            var effectsCamera = GameObject.Find("EffectsCamera");
            if (effectsCamera != null)
                effectsCamera.GetComponent<EffectsCameraBhv>()?.Reset();
            if (CurrentOpponent.Type == OpponentType.Boss)
            {
                Paused = true;
                Instantiator.NewDialogBoxDeath(CameraBhv.transform.position, CurrentOpponent.Name, () =>
                {
                    StartCoroutine(Helper.ExecuteAfterDelay(0.0f, () => { GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>().InitMenuKeyboardInputs(); return true; }));
                    Victory();
                    return true;
                });
            }
            else
                Victory();
            return;
        }
        CurrentOpponent = _opponents[Constants.CurrentListOpponentsId].Clone();
        CurrentOpponent.Cooldown += (_realmTree.CooldownBrake * 0.666f);
        if (Constants.RandomizedAttackType != AttackType.None)
            RandomizeOpponentAttack();
        if (Constants.CurrentOpponentChangedRealm != Realm.None)
            AlterOpponentRealm(Constants.CurrentOpponentChangedRealm, fromNextOpponent: true);
        if (Constants.HalvedCooldown)
            HalveOpponentMaxCooldown();
        if (Constants.NameLastScene == Constants.SettingsScene)
        {
            if (Constants.IsffectAttackInProgress)
            {
                Constants.CurrentOpponentCooldown = CurrentOpponent.Cooldown + 1;
                Constants.CurrentOpponentAttackId = Constants.CurrentOpponentAttackId - 1 < 0 ? CurrentOpponent.Attacks.Count - 1 : Constants.CurrentOpponentAttackId - 1;
            }
        }
        else
        {
            _soundControler.PlaySound(_idOpponentAppearance);
            Constants.CurrentOpponentAttackId = 0;
            OpponentInstanceBhv.Spawn();
            CameraBhv.Bump(4);
            var minHeight = 9.0f;
            var highestBlockY = _gameplayControler.GetHighestBlock();
            if (minHeight < highestBlockY)
                minHeight = highestBlockY + 1;
            if (!sceneInit)
                OpponentAppearance(minHeight);
        }
        OpponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{CurrentOpponent.Region}Opponents_{CurrentOpponent.Id}");
        _opponentType.sprite = CurrentOpponent.Type == OpponentType.Common ? null : Helper.GetSpriteFromSpriteSheet("Sprites/OpponentTypes_" + ((CurrentOpponent.Realm.GetHashCode() * 3) + (CurrentOpponent.Type.GetHashCode() - 1)));
        Constants.CurrentOpponentHp = Constants.CurrentOpponentHp <= 0 ? CurrentOpponent.HpMax : Constants.CurrentOpponentHp;
        _weaknessInstance.SetVisible(CurrentOpponent.Weakness != Weakness.None);
        _weaknessInstance.SetSkin(Helper.GetSpriteFromSpriteSheet("Sprites/WeaknessImmunity_" + (CurrentOpponent.Realm.GetHashCode() * 2)));
        _immunityInstance.SetVisible(CurrentOpponent.Immunity != Immunity.None);
        _immunityInstance.SetSkin(Helper.GetSpriteFromSpriteSheet("Sprites/WeaknessImmunity_" + (CurrentOpponent.Realm.GetHashCode() * 2 + 1)));
        _opponentHpBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 0),
                               "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 1),
                               $"<material=\"{CurrentOpponent.Realm.ToString().ToLower()}.4.3\">");
        _opponentHpBar.UpdateContent(0, CurrentOpponent.HpMax);
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 2),
                                     "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 3));
        _opponentCooldownBar.UpdateContent(0, 1);
        _gameplayControler.SetGravity(CurrentOpponent.GravityLevel + ((_run?.RealmLevel ?? 1) - 1));
        StartOpponentCooldown(sceneInit, true);
    }

    public void RandomizeOpponentAttack()
    {
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece)
            Destroy(_gameplayControler.ForcedPiece);
        else if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.Drill)
        {
            var tmpDrillTarget = GameObject.Find(Constants.GoDrillTarget);
            if (tmpDrillTarget != null)
                Destroy(tmpDrillTarget);
        }

        CurrentOpponent.Attacks = new List<OpponentAttack> { new OpponentAttack(Constants.RandomizedAttackType, 1, 2) };
        Constants.CurrentOpponentAttackId = 0;
        if (Constants.RandomizedAttackType == AttackType.Drill)
            _gameplayControler.AttackDrill(OpponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1, true);
    }

    public void HalveOpponentMaxCooldown() { CurrentOpponent.Cooldown /= 2; }

    public void AlterOpponentRealm(Realm newRealm, bool fromNextOpponent = false)
    {
        CurrentOpponent.Realm = newRealm;
        if (fromNextOpponent)
            return;
        _opponentHpBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 0),
                               "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 1),
                               $"<material=\"{CurrentOpponent.Realm.ToString().ToLower()}.4.3\">");
        _opponentHpBar.UpdateContent(0, CurrentOpponent.HpMax);
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 2),
                                     "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 3));
        _opponentCooldownBar.UpdateContent(0, 1);
    }

    private bool Victory()
    {
        PlayerPrefsHelper.SaveIsInFight(false);
        Paused = true;
        _isVictorious = true;
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
        _gameplayControler.CleanPlayerPrefs();
        Constants.ResetClassicGameCache();

        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
        {
            LoadBackAfterVictory(false);
            return false;
        }

        _currentStep = _stepsService.GetStepOnPos(_run.X, _run.Y, _run.Steps);
        var loot = Helper.GetLootFromTypeAndId(_currentStep.LootType, _currentStep.LootId);
        if (loot.LootType == LootType.Character)
            PlayerPrefsHelper.AddUnlockedCharacters((Character)loot); //Done here in order to prevent generating a step with the just unlocked character

        _stepsService.ClearLootOnPos(_run.X, _run.Y, _run);
        if (_run.CurrentStep > Character.LandLordLateAmount)
            _stepsService.SetVisionOnRandomStep(_run);
        _stepsService.GenerateAdjacentSteps(_run, Character, _currentStep);
        _run.CurrentItemCooldown = Constants.CurrentItemCooldown - _realmTree.PosthumousItem;
        _run.CurrentItemUses = Constants.CurrentItemUses;
        PlayerPrefsHelper.SaveRun(_run);
        if (loot.LootType == LootType.Character)
        {
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, ((Character)loot).Name, Character.Name, AfterCharacterDialog);
            bool AfterCharacterDialog() {
                StartCoroutine(Helper.ExecuteAfterDelay(0.0f, () => { GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>().InitMenuKeyboardInputs(); return true; }));
                _musicControler.Play(Constants.VictoryAudioClip, once: true);
                Instantiator.NewPopupYesNo("New Character", $"you unlocked {((Character)loot).Name.ToLower()}, a new playable character!", null, "Noice!", LoadBackAfterVictory);
                return true;}
        }
        else if (loot.LootType == LootType.Item)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            var currentItem = PlayerPrefsHelper.GetCurrentItem();
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + ((Item)loot).Id);
            if (currentItem == null)
            {
                Instantiator.NewPopupYesNo("New Item", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Item)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " added to your gear.", null, "Ok", LoadBackAfterVictory, sprite);
                PlayerPrefsHelper.SaveCurrentItem(((Item)loot).Name);
                _run.CurrentItemCooldown = 0;
                _run.CurrentItemUses = ((Item)loot).Uses;
                PlayerPrefsHelper.SaveRun(_run);
            }
            else if (currentItem.Id == ((Item)loot).Id)
            {
                Instantiator.NewPopupYesNo("Same Item", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "well... this is awkward... you already use " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + currentItem.Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "...", null, "Oh...", LoadBackAfterVictory, sprite);
                _run.CurrentItemCooldown = 0;
                _run.CurrentItemUses = ((Item)loot).Uses;
                PlayerPrefsHelper.SaveRun(_run);
            }
            else
            {
                var content = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "switch your " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + currentItem.Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " for " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Item)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "?";
                Instantiator.NewPopupYesNo("New Item", content, "No", "Yes", OnItemSwitch, sprite);
                object OnItemSwitch(bool result)
                {
                    if (result)
                    {
                        PlayerPrefsHelper.SaveCurrentItem(((Item)loot).Name);
                        _run.CurrentItemCooldown = 0;
                        _run.CurrentItemUses = ((Item)loot).Uses;
                        PlayerPrefsHelper.SaveRun(_run);
                    }
                    LoadBackAfterVictory(true);
                    return result;
                }
            }
        }
        else if (loot.LootType == LootType.Resource)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            var amount = 2;
            if (_run.Difficulty == Difficulty.Easy)
                amount = 1;
            else if (_run.Difficulty == Difficulty.Hard)
                amount = 3;
            _run.AlterResource(((Resource)loot).Id, amount);
            PlayerPrefsHelper.AlterResource(((Resource)loot).Id, amount);
            PlayerPrefsHelper.SaveRun(_run);
            Instantiator.NewPopupYesNo("Resources", $"+{amount} {((Resource)loot).Name.ToLower()}{(amount > 1 ? "s" : "")}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} added to your resources.", null, "Ka-Ching!", LoadBackAfterVictory);
        }
        else if (loot.LootType == LootType.Tattoo)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            var nameToCheck = ((Tattoo)loot).Name.Replace(" ", "").Replace("'", "").Replace("-", "");
            var tattoos = PlayerPrefsHelper.GetCurrentTattoosString();
            var bodyPart = PlayerPrefsHelper.AddTattoo(((Tattoo)loot).Name);
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + ((Tattoo)loot).Id);
            if (bodyPart == BodyPart.MaxLevelReached)
            {
                Instantiator.NewPopupYesNo("Max Level", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Tattoo)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " reached its maximum level.", null, "Damn...", LoadBackAfterVictory, sprite);
            }
            else if (bodyPart == BodyPart.None)
            {
                Instantiator.NewPopupYesNo("Filled!", $"sadly, you don't have any place left to ink anything...", null, "But...", LoadBackAfterVictory, sprite);
            }
            else 
            {
                _soundControler.PlaySound(_idTattooSound);
                if (!tattoos.Contains(nameToCheck))
                    Instantiator.NewPopupYesNo("New Tattoo", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Tattoo)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " has been inked \non your " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + bodyPart.GetDescription().ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + ".", null, "Ouch!", LoadBackAfterVictory, sprite);
                else
                    Instantiator.NewPopupYesNo("Tattoo Upgrade", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "your " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Tattoo)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " power has been increased.", null, "Noice", LoadBackAfterVictory, sprite);
                ((Tattoo)loot).ApplyToCharacter(Character);
                PlayerPrefsHelper.SaveRunCharacter(Character);
            }
        }
        else
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            LoadBackAfterVictory(false);
        }
        return true;
    }

    private object LoadBackAfterVictory(bool result)
    {
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            NavigationService.LoadBackUntil(Constants.CharSelScene);
        else
        {
            if (CurrentOpponent.Type == OpponentType.Boss)
            {
                PlayerPrefsHelper.IncrementRunBossVanquished();
                var realmIdBeforeIncrease = _run.CurrentRealm.GetHashCode();
                _run.IncreaseLevel();
                var currentItem = PlayerPrefsHelper.GetCurrentItem();
                if (currentItem != null)
                    _run.CurrentItemUses = currentItem.Uses;
                if (currentItem != null && currentItem.Name == ItemsData.Items[25])
                    ++_run.DeathScytheAscension;
                PlayerPrefsHelper.SaveRun(_run);
                if (_run.CurrentRealm.GetHashCode() > realmIdBeforeIncrease && realmIdBeforeIncrease > PlayerPrefsHelper.GetRealmBossProgression())
                {
                    PlayerPrefsHelper.SaveRealmBossProgression(realmIdBeforeIncrease);
                    StartCoroutine(Helper.ExecuteAfterDelay(0.0f, () => { GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>().InitMenuKeyboardInputs(); return true; }));
                    var content = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}you now start your ascensions with a random item.\n(up to a {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{((Rarity)realmIdBeforeIncrease).ToString().ToLower()}{Constants.MaterialEnd} one).";
                    Instantiator.NewPopupYesNo($"{CurrentOpponent.Name} beaten!", content, null, "Neat!", LoadNext);
                }
                else
                {
                    return (bool)LoadNext(true);
                }
                
                object LoadNext(bool result)
                {
                    //DEBUG
                    if (Constants.BetaMode && _run.CurrentRealm == Realm.Earth)
                    {
                        Constants.GameOverParams = $"Abject|Hell|3";
                        PlayerPrefsHelper.ResetRun();
                        NavigationService.LoadNextScene(Constants.DemoEndScene);
                        return false;
                    }
                    //DEBUG
                    NavigationService.LoadBackUntil(Constants.StepsAscensionScene);
                    return true;
                }
            }
            else
            {
                NavigationService.LoadBackUntil(Constants.StepsScene);
            }
            
        }
        return result;
    }

    private void StartOpponentCooldown(bool sceneInit = false, bool first = false)
    {
        _opponentOnCooldown = true;
        if (!sceneInit)
            Constants.CurrentOpponentCooldown = 0;
        if (first && CurrentOpponent.Haste)
        {
            if (!sceneInit)
                Instantiator.PopText("haste", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
            Constants.CurrentOpponentCooldown = CurrentOpponent.Cooldown;
        }
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.Drill)
            _gameplayControler.AttackDrill(OpponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1, true);
        SetNextCooldownTick();
    }

    private void SetNextCooldownTick()
    {
        if (_timeStopTimer > 0)
            return;
        if (Constants.CurrentOpponentCooldown > CurrentOpponent.Cooldown)
        {
            OpponentAttackIncoming();
        }
        else
        {
            //if (Constants.CurrentOpponentCooldown >= CurrentOpponent.Cooldown - 1.0f)
            //{
            //    var maxHeight = 15.0f;
            //    var highestBlockY = _gameplayControler.GetHighestBlock();
            //    if (maxHeight > highestBlockY + 3)
            //        maxHeight = highestBlockY + 4;
            //    Instantiator.PopText($"coming:\n{CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType}", new Vector2(4.5f, maxHeight));
            //}

            if ((Character.HighPlayPause && _gameplayControler.GetHighestBlock() >= 15)
                || _stunIcon.IsOn)
                _nextCooldownTick = Time.time + 3600;
            else
                _nextCooldownTick = Time.time + 1.0f;
        }
    }

    public void OpponentAttackIncoming()
    {
        _opponentOnCooldown = false;
        _nextCooldownTick = Time.time + 3600;
        _gameplayControler.AttackIncoming = true;
    }

    public void StopTime(int seconds)
    {
        _gameplayControler.SetGravity(0);
        _nextCooldownTick = Time.time + 3600;
        _timeStopTimer = Time.time + seconds;
    }

    public override bool OpponentAttack()
    {
        bool spawnAfterAttack = true;
        CameraBhv.Bump(2);
        OpponentInstanceBhv.Attack();
        
        if (Constants.ChanceAttacksHappeningPercent < 100 && !Helper.RandomDice100(Constants.ChanceAttacksHappeningPercent))
        {
            Instantiator.PopText("missed", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idDodge);
            _characterInstanceBhv.Dodge();
        }
        else if (Constants.BlockPerAttack >= 0 && ++Constants.BlockPerAttack == 3)
        {
            Constants.BlockPerAttack = 0;
            Instantiator.PopText("blocked", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idHit);
        }
        else if (Constants.CurrentRemainingSimpShields > 0)
        {
            --Constants.CurrentRemainingSimpShields;
            var shieldObject = GameObject.Find(Constants.GoSimpShield);
            if (shieldObject != null)
            {
                Instantiator.PopText("blocked", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
                _soundControler.PlaySound(_idHit);
                shieldObject.GetComponent<CharacterInstanceBhv>().GetOS();
            }
        }
        else if (Helper.RandomDice100(Character.DodgeChance + Constants.AddedDodgeChancePercent))
        {
            Instantiator.PopText("dodged", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idDodge);
            _characterInstanceBhv.Dodge();
        }
        else
        {
            _characterInstanceBhv.TakeDamage();
            _gameplayControler.OpponentAttack(
                CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType,
                CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1,
                CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param2,
                CurrentOpponent.Realm,
                OpponentInstanceBhv.gameObject);
            if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece
                || CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.Shift)
                spawnAfterAttack = false;
        }
        if (++Constants.CurrentOpponentAttackId >= CurrentOpponent.Attacks.Count)
            Constants.CurrentOpponentAttackId = 0;
        _opponentCooldownBar.UpdateContent(0, 1, Direction.Down);
        _opponentCooldownBar.ResetTilt();
        StartOpponentCooldown();
        if (Character.ThornsPercent > 0)
        {
            var thornDamages = (int)(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.ThornsPercent));
            DamageOpponent(thornDamages == 0 ? 1 : thornDamages, _gameplayControler.CharacterInstanceBhv.gameObject);
        }
        return spawnAfterAttack;
    }

    protected override void FrameUpdate()
    {
        if (Paused)
        {
            SetNextCooldownTick();
            return;
        }
        HandleForcedPiece();
        if (_gameplayControler.AttackIncoming)
        {
            _opponentCooldownBar.Tilt();
        }
        if (_wetTimer > 0 && Time.time > _wetTimer)
        {
            Character.BoostAttack -= _wetMalus;
            _wetTimer = -1;
        }
        if (_timeStopTimer > 0 && Time.time > _timeStopTimer)
        {
            _gameplayControler.SetGravity(CurrentOpponent.GravityLevel);
            _nextCooldownTick = Time.time + 1;
            _timeStopTimer = -1;
            var maxHeight = 15.0f;
            var highestBlockY = _gameplayControler.GetHighestBlock();
            if (maxHeight > highestBlockY + 3)
                maxHeight = highestBlockY + 4;
            Instantiator.PopText("and time keeps\nmoving on", new Vector2(4.5f, maxHeight));
        }
    }

    private void HandleForcedPiece()
    {
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece && _gameplayControler.ForcedPiece == null)
        {
            _gameplayControler.AttackForcedPiece(OpponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param2);
            _gameplayControler.SetForcedPieceOpacity((float)Constants.CurrentOpponentCooldown, (float)CurrentOpponent.Cooldown);
        }
        if (_opponentOnCooldown && Time.time >= _nextCooldownTick)
        {
            ++Constants.CurrentOpponentCooldown;
            _gameplayControler.SetForcedPieceOpacity((float)Constants.CurrentOpponentCooldown, (float)CurrentOpponent.Cooldown);
            UpdateCooldownBar(Direction.Up);
            SetNextCooldownTick();
        }
    }

    public void RestartOpponentCooldown()
    {
        Constants.CurrentOpponentCooldown = 0;
        UpdateCooldownBar(Direction.Down);
        _opponentOnCooldown = true;
        _nextCooldownTick = Time.time + 1.0f;
        _gameplayControler.AttackIncoming = false;
    }

    private void UpdateCooldownBar(Direction direction)
    {
        _opponentCooldownBar.UpdateContent(Constants.CurrentOpponentCooldown, CurrentOpponent.Cooldown, direction);
    }

    public override void OnGameOver()
    {
        base.OnGameOver();
        _characterInstanceBhv.Die();
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            NavigationService.LoadBackUntil(Constants.CharSelScene);
        else
        {
            Constants.GameOverParams = $"{CurrentOpponent.Name}|{_run.CurrentRealm}|{_run.RealmLevel}";
            PlayerPrefsHelper.ResetRun();
            NavigationService.LoadNextScene(Constants.GameOverScene);
        }
    }

    public override void StunOpponent(int seconds)
    {
        _stunIcon.AppearAndWiggle(seconds, AfterStun);

        bool AfterStun()
        {
            SetNextCooldownTick();
            return true;
        }
    }
    
    private void WetMalusOpponent(int malusPercent, float seconds)
    {
        OpponentInstanceBhv.Malus(Realm.Heaven, seconds);
        _wetMalus = Mathf.RoundToInt(Character.GetAttackNoBoost() * Helper.MultiplierFromPercent(0, malusPercent));
        Character.BoostAttack += _wetMalus;
        _wetTimer = Time.time + seconds;
    }

    public override void DamageOpponent(int amount, GameObject source, Realm? textRealm = null, bool attackLine = true)
    {
        if (Constants.CurrentOpponentHp <= 0)
            return;
        if (Character.QuadDamage > 0 && Constants.CurrentListOpponentsId == 0 && _opponents.Count >= 4)
        {
            amount *= Character.QuadDamage;
            _characterInstanceBhv.Boost(Realm.None, 1.0f);
        }
        var realm = Character.Realm;
        var sourcePosition = _characterInstanceBhv.transform.position;
        Piece piece = null;
        if (source != null)
        {
            sourcePosition = source.transform.position;
            piece = source.GetComponent<Piece>();
        }
        if (piece != null && piece.IsMimic)
        {
            realm = Helper.GetInferiorFrom(Character.Realm);
            if (source.transform.position.x < 0)
                sourcePosition = new Vector3(0.0f, sourcePosition.y, 0.0f);
            else if (source.transform.position.x > 9)
                sourcePosition = new Vector3(9.0f, sourcePosition.y, 0.0f);
        }
        if (attackLine)
            Instantiator.NewAttackLine(sourcePosition, OpponentInstanceBhv.gameObject.transform.position, realm);
        OpponentInstanceBhv.TakeDamage();
        Constants.CurrentOpponentHp -= amount;
        CameraBhv.Bump(2);
        var attackText = "-" + amount;
        if (amount == 69)
            attackText = "nice";
        if (!_isCrit)
            PlayHit();
        else
        {
            attackText = $"</material>{attackText}!";
            _soundControler.PlaySound(_idCrit);
        }
        VibrationService.Vibrate();
        var damagesTextPosition = _opponentHpBar.transform.position + new Vector3(1.0f, 1.6f, 0.0f);
        var poppingTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        //Check for overlapping damages texts
        for (int i = 0; i < poppingTexts.Length; ++i)
        {
            if (Helper.VectorEqualsPrecision(poppingTexts[i].transform.position, damagesTextPosition, 0.5f) && poppingTexts[i].transform.position.y >= damagesTextPosition.y)
                damagesTextPosition = poppingTexts[i].transform.position + new Vector3(0.0f, -1.1f);
        }
        var realmMaterial = "";
        if (textRealm == null || textRealm != Realm.None)
        {
            var tmpRealm = textRealm == null ? Character.Realm : textRealm;
            realmMaterial = $"<material=\"{tmpRealm.ToString().ToLower()}.4.3\">";
        }
        Instantiator.PopText($"{realmMaterial}{attackText}", damagesTextPosition);
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Left);
        if (Constants.CurrentOpponentHp <= 0)
        {
            KillOpponent();
        }
        else
        {
            if (_opponentOnCooldown && Constants.CurrentOpponentCooldown < CurrentOpponent.Cooldown)
            {
                if (CurrentOpponent.Immunity != Immunity.Cooldown)
                    Constants.CurrentOpponentCooldown -= Character.EnemyCooldownProgressionReducer;
                if (Constants.CurrentOpponentCooldown <= 0)
                    Constants.CurrentOpponentCooldown = 0;
                UpdateCooldownBar(Direction.Down);
            }
            SetNextCooldownTick();
        }
    }

    public void KillOpponent()
    {
        _stunIcon.Hide();
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
        _gameplayControler.PlayFieldBhv.ShowSemiOpcaity(1);
        _soundControler.PlaySound(_idOpponentDeath);
        var minHeight = 9.0f;
        var highestBlockY = _gameplayControler.GetHighestBlock();
        if (minHeight < highestBlockY)
            minHeight = highestBlockY + 2;
        Instantiator.PopText(CurrentOpponent.Name.ToLower() + " defeated!", new Vector2(4.5f, minHeight));
        OpponentInstanceBhv.Die();
        _opponentOnCooldown = false;
        Constants.CurrentOpponentCooldown = 0;
        UpdateCooldownBar(Direction.Down);
    }

    public void PlayHit()
    {
        _soundControler.PlaySound(_idHit);
    }

    public override void OnNewPiece(GameObject lastPiece)
    {
        if (_characterAttack > 0)
        {
            var bossHateBonusPercent = 0;
            if (CurrentOpponent.Type == OpponentType.Boss)
                bossHateBonusPercent = _realmTree.BossHate;
            DamageOpponent((int)(_characterAttack * Helper.MultiplierFromPercent(1.0f, bossHateBonusPercent)), lastPiece);
        }
        else if (_characterAttack < 0)
            Instantiator.PopText("missed", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
        _characterAttack = 0;
        _isCrit = false;
    }

    private object AfterOpponentDeath()
    {
        var opponentIcon = GameObject.Find("Opponent" + Constants.CurrentListOpponentsId);
        opponentIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/OpponentsIcons_" + ((_opponents[Constants.CurrentListOpponentsId].Realm.GetHashCode() * 2) + 1));
        opponentIcon.GetComponent<IconInstanceBhv>().Pop();
        ++Constants.CurrentListOpponentsId;
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece)
            Destroy(_gameplayControler.ForcedPiece);
        else if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.Drill)
        {
            var tmpDrillTarget = GameObject.Find(Constants.GoDrillTarget);
            if (tmpDrillTarget != null)
                Destroy(tmpDrillTarget);
        }
        if (Character.ItemCooldownReducerOnKill > 0 && PlayerPrefsHelper.GetCurrentItemName() != null)
        {
            Constants.CurrentItemCooldown -= Character.ItemCooldownReducerOnKill;
            _gameplayControler.UpdateItemAndSpecialVisuals();
        }
        if (Character.DeleteAfterKill > 0)
            _gameplayControler.DeleteFromBottom(Character.DeleteAfterKill);
        Constants.RandomizedAttackType = AttackType.None;
        Constants.HalvedCooldown = false;
        NextOpponent();
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = false;
        _gameplayControler.PlayFieldBhv.ShowSemiOpcaity(0);
        return true;
    }

    public override void OnPieceLocked(string pieceLetter)
    {
        base.OnPieceLocked(pieceLetter);
        if (string.IsNullOrEmpty(pieceLetter))
            return;
        var incomingDamages = 0;
        if (CurrentOpponent.Weakness == Weakness.Twists)
        {
            _weaknessInstance.Pop();
            _soundControler.PlaySound(_idWeakness);
            incomingDamages += CurrentOpponent.DamagesOnWeakness;
        }
        if (CurrentOpponent.Immunity == Immunity.Twists)
        {
            _immunityInstance.Pop();
            _soundControler.PlaySound(_idImmunity);
            incomingDamages = 0;
        }
        _characterAttack += incomingDamages;
    }

    public override void OnLinesCleared(int nbLines, bool isB2B)
    {
        base.OnLinesCleared(nbLines, isB2B);
        var incomingDamages = 0;
        if (nbLines > 0 && Constants.ChanceAttacksHappeningPercent < 100 && !Helper.RandomDice100(Constants.ChanceAttacksHappeningPercent))
        {
            incomingDamages = -1;
            nbLines = 0;
            isB2B = false;
        }
        if (nbLines > 0)
        {
            if (Character.DamoclesDamages > 0 && nbLines == 4)
            {
                Constants.DamoclesDamages += Character.DamoclesDamages;
                _characterInstanceBhv.Boost(Character.Realm, 0.25f);
            }

            incomingDamages = Character.GetAttack();
            if (nbLines == 1)
                incomingDamages += Character.SingleLineDamageBonus;
            if (Helper.RandomDice100(Character.CritChancePercent + Constants.CumulativeCrit + _realmTree.CriticalPrecision))
            {
                Constants.CumulativeCrit += Character.CumulativeCrit;
                incomingDamages += (int)(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.CritMultiplier));
                _isCrit = true;
            }
            if (Helper.IsSuperiorByRealm(Character.Realm, CurrentOpponent.Realm))
                incomingDamages = (int)(incomingDamages * Helper.MultiplierFromPercent(1.0f, Character.DamagePercentToInferiorRealm));
            incomingDamages *= nbLines;
            if (CurrentOpponent.Weakness == Weakness.xLines && CurrentOpponent.XLineWeakness == nbLines)
            {
                _weaknessInstance.Pop();
                _soundControler.PlaySound(_idWeakness);
                incomingDamages += CurrentOpponent.DamagesOnWeakness;
            }
            if (CurrentOpponent.Immunity == Immunity.xLines && CurrentOpponent.XLineImmunity == nbLines)
            {
                _immunityInstance.Pop();
                _soundControler.PlaySound(_idImmunity);
                incomingDamages = 0;
            }
            if (Character.Realm == Realm.Earth && nbLines == 4)
            {
                int targetDestroyed = Character.RealmPassiveEffect;
                targetDestroyed -= _gameplayControler.CheckForDarkRows(targetDestroyed);
                if (targetDestroyed > 0)
                    targetDestroyed -= _gameplayControler.CheckForWasteRows(targetDestroyed);
                if (targetDestroyed > 0)
                    _gameplayControler.CheckForLightRows(brutForceDelete : true);

            }

            //ELEMENTS STONES
            if (nbLines == 3)
            {
                if (Character.EarthStun > 0)
                    StunOpponent(Character.EarthStun);
                if (Character.WaterDamagePercent > 0)
                    WetMalusOpponent(Character.WaterDamagePercent, 4.0f);
                if (Character.FireDamagesPercent > 0)
                    StartCoroutine(Burn(3));
                if (Character.WindTripleBonus > 0)
                {
                    Constants.TripleLineDamageBonus += Character.WindTripleBonus;
                    StartCoroutine(WindBoost());
                }
            }
        }
        if (isB2B)
        {
            if (CurrentOpponent.Weakness == Weakness.Consecutive)
            {
                _weaknessInstance.Pop();
                _soundControler.PlaySound(_idWeakness);
                incomingDamages += CurrentOpponent.DamagesOnWeakness;
            }
            if (CurrentOpponent.Immunity == Immunity.Consecutive)
            {
                _immunityInstance.Pop();
                _soundControler.PlaySound(_idImmunity);
                incomingDamages = 0;
            }
            if (Character.Realm == Realm.Heaven)
            {
                Constants.SelectedCharacterSpecialCooldown -= Character.RealmPassiveEffect;
                Constants.CurrentItemCooldown -= Character.RealmPassiveEffect;
                _gameplayControler.UpdateItemAndSpecialVisuals();
            }
        }
        _characterAttack += incomingDamages;
    }

    public IEnumerator Burn(int time)
    {
        yield return new WaitForSeconds(0.33f);
        if (time > 0)
        {
            var burnDamages = (int)(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.FireDamagesPercent));
            DamageOpponent(burnDamages == 0 ? 1 : burnDamages, null, Realm.Hell, attackLine: false);
            StartCoroutine(Burn(time - 1));
        }
    }

    public IEnumerator WindBoost()
    {
        yield return new WaitForSeconds(0.25f);
        DamageOpponent(Constants.TripleLineDamageBonus, null, Realm.None, attackLine: false);
        _characterInstanceBhv.Boost(Realm.None, 1.0f);
    }

    public override void OnCombo(int nbCombo, int nbLines)
    {
        base.OnCombo(nbCombo, nbLines);
        if (_characterAttack < 0)
            return;
        var incomingDamages = 0;
        if (Character.Realm == Realm.Hell)
            incomingDamages += (int)((Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, 10 * Character.RealmPassiveEffect) + (nbCombo - 2)) * nbLines);
        if (CurrentOpponent.Weakness == Weakness.Combos)
        {
            _weaknessInstance.Pop();
            _soundControler.PlaySound(_idWeakness);
            incomingDamages += CurrentOpponent.DamagesOnWeakness * (nbCombo - 1);
        }
        if (CurrentOpponent.Immunity == Immunity.Combos)
        {
            _immunityInstance.Pop();
            _soundControler.PlaySound(_idImmunity);
            incomingDamages = 0;
        }
        _characterAttack += incomingDamages;
    }

    public override void OnPerfectClear()
    {
        base.OnPerfectClear();
        if (_characterAttack < 0)
            return;
        if (Character.PerfectKills)
            DamageOpponent(9999, _gameplayControler.CharacterInstanceBhv.gameObject);
    }

    private void OnApplicationQuit()
    {
        if (_isVictorious || _run == null)
            return;
        if (Constants.CurrentGameMode == GameMode.Ascension
            || Constants.CurrentGameMode == GameMode.TrueAscension)
        {
            _stepsService.ClearLootOnPos(_run.X, _run.Y, _run);
            _stepsService.SetVisionOnRandomStep(_run);
            _stepsService.GenerateAdjacentSteps(_run, Character, _currentStep);
        }
    }
}
