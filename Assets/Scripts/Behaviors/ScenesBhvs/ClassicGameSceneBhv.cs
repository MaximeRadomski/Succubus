using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicGameSceneBhv : GameSceneBhv
{
    public CharacterInstanceBhv OpponentInstanceBhv;
    public Run Run;

    private ResourceBarBhv _opponentHpBar;
    private ResourceBarBhv _opponentCooldownBar;
    private SpriteRenderer _opponentType;
    private bool _opponentOnCooldown;
    private float _nextCooldownTick;
    private IconInstanceBhv _weaknessInstance;
    private IconInstanceBhv _immunityInstance;
    private WiggleBhv _stunIcon;
    private RealmTree _realmTree;
    private StepsService _stepsService;
    private Step _currentStep;
    private List<Pact> _pacts;

    private int _characterAttack;
    private int _wetMalus;
    private float _wetTimer;
    private float _timeStopTimer;
    private bool _isCrit;
    //private bool _isVictorious;
    private bool _isTraining;

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

    public float GetCurrentOpponentMaxCooldown()
    {
        var cooldown = CurrentOpponent.Cooldown + Character.EnemyMaxCooldownMalus + Cache.EnemyCooldownInfiniteStairMalus + Character.DevilsContractMalus + Cache.PactEnemyMaxCooldownMalus;
        if (cooldown < 1.0f && Run.Difficulty.GetHashCode() <= Difficulty.Infernal.GetHashCode())
            return 1.0f;
        else if (cooldown < 0.5f && Run.Difficulty.GetHashCode() >= Difficulty.Divine.GetHashCode())
            return 0.5f;
        return cooldown;
    }

    private MusicType GetMusicType()
    {
        if (_stepsService == null)
            _stepsService = new StepsService();
        if (Run == null)
            Run = PlayerPrefsHelper.GetRun();
        if (Run == null)
            return MusicType.Game;
        var currentStep = _stepsService.GetStepOnPos(Run.X, Run.Y, Run.Steps);
        if (currentStep != null && currentStep.LandLordVision)
            return MusicType.Boss;
        return MusicType.Game;
    }

    void Start()
    {
        try
        {
            Init();
            if (Run == null)
                Run = PlayerPrefsHelper.GetRun();
            _realmTree = PlayerPrefsHelper.GetRealmTree();
            if (_stepsService == null)
                _stepsService = new StepsService();
            if (Cache.CurrentGameMode == GameMode.TrainingDummy
                || Cache.CurrentGameMode == GameMode.TrainingFree)
            {
                _isTraining = true;
                _opponents = PlayerPrefsHelper.GetCurrentOpponents(new Run(Difficulty.Normal));
                Cache.RestartCurrentItemCooldown(Character, ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]));
            }
            else
            {
                if (PlayerPrefsHelper.GetIsInFight() && Cache.NameLastScene != Constants.SettingsScene) // If we get back to a fight after having force-quit the game.
                    BoostCurrentOpponentsAfterForceQuit();
                PlayerPrefsHelper.SaveIsInFight(true);
                _currentStep = _stepsService.GetStepOnPos(Run.X, Run.Y, Run.Steps);
                if (_currentStep.LandLordVision)
                {
                    _opponents = _stepsService.GetBoss(Run);
                    Helper.ApplyDifficulty(_opponents, Run.Difficulty);
                }
                else
                    _opponents = _currentStep.Opponents;
                Cache.CurrentItemCooldown = Run.CurrentItemCooldown;
                Cache.CurrentItemUses = Run.CurrentItemUses;
            }

            _pacts = PlayerPrefsHelper.GetCurrentPacts();
            if (Cache.NameLastScene != Constants.SettingsScene && !_isTraining)
            {
                foreach (var pact in _pacts)
                    pact.ApplyPact(this.Character);
                _gameplayControler.UpdateItemAndSpecialVisuals();
            }

            //if (_opponents.Count == 1)
            //    GameObject.Find("Enemies").GetComponent<TMPro.TextMeshPro>().text = "enemy";
            for (int i = _opponents.Count; i < 12; ++i)
            {
                GameObject.Find("Opponent" + i).SetActive(false);
            }
            for (int j = 0; j < _opponents.Count; ++j)
            {
                if (j < Cache.CurrentListOpponentsId)
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
            _nextCooldownTick = Time.time + Constants.OpponentCooldownOneHour;
            _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
            _idOpponentDeath = _soundControler.SetSound("OpponentDeath");
            _idOpponentAppearance = _soundControler.SetSound("OpponentAppearance");
            _idCrit = _soundControler.SetSound("Crit");
            _idHit = _soundControler.SetSound("Hit");
            _idWeakness = _soundControler.SetSound("Weakness");
            _idImmunity = _soundControler.SetSound("Immunity");
            _idDodge = _soundControler.SetSound("LevelUp");
            _idTattooSound = _soundControler.SetSound("TattooSound");
            var realm = _isTraining ? Realm.Hell : Run?.CurrentRealm ?? Realm.Hell;
            GameObject.Find("InfoRealm").GetComponent<TMPro.TextMeshPro>().text = $"{Constants.GetMaterial(realm, TextType.succubus3x5, TextCode.c32B)}realm:\n{ Constants.GetMaterial(realm, TextType.succubus3x5, TextCode.c43B)}{ (realm.ToString().ToLower())}\nlvl {Run?.RealmLevel.ToString() ?? "?"}";
            NextOpponent(sceneInit: true);
            _gameplayControler.GetComponent<GameplayControler>().StartGameplay(CurrentOpponent.GravityLevel, Character.Realm, Run?.CurrentRealm ?? Realm.Hell);

            _gameplayControler.GameplayOnHold = true;
            _musicControler.Stop();
            Cache.InputLocked = true;
            if (Cache.NameLastScene != Constants.SettingsScene)
            {
                if (!_isTraining)
                {
                    Cache.CurrentRemainingSimpShields = Character.SimpShield;
                    if (!Cache.PactNoLastFightPlayField && !Character.AllClear)
                        _gameplayControler.ApplyLastFightPlayField();
                }
                Instantiator.NewFightIntro(new Vector3(CameraBhv.transform.position.x, CameraBhv.transform.position.y, 0.0f), Character, _opponents, AfterFightIntro);
            }
            else
                _musicControler.Play();

            if (Cache.CurrentRemainingSimpShields > 0)
            {
                for (int i = 0; i < Cache.CurrentRemainingSimpShields; ++i)
                {
                    Instantiator.NewSimpShield(_characterInstanceBhv.OriginalPosition, i, _gameplayControler.CharacterRealm);
                }
            }
            if (Character.FillTargetBlocks > 0)
                Instantiator.NewFillTarget(_gameplayControler.CharacterRealm, Character.FillTargetBlocks, _gameplayControler);
        }
        catch (Exception e)
        {
            PlayerPrefsHelper.ResetRun();
            LogService.LogCallback($"Custom Caught Exception:\nMessage: {e.Message}\nSource:{e.Source}", e.StackTrace, LogType.Exception);
        }
    }

    private void BoostCurrentOpponentsAfterForceQuit()
    {
        Cache.PactOnlyHaste = true;
        Cache.PactEnemyMaxCooldownMalus -= 2;
    }

    private void AfterFightIntro()
    {
        if (!Character.SlumberingDragoncrest && !Cache.PactStealth && (CurrentOpponent.Haste || Character.HasteForAll || Cache.PactOnlyHaste))
            Instantiator.PopText("haste", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
        Cache.InputLocked = false;
        _gameplayControler.GameplayOnHold = false;
        OpponentAppearance();
        if (!_isTraining && _pacts != null && _pacts.Count > 0)
            Instantiator.PopText($"{_pacts.Count} active pact{(_pacts.Count > 1 ? "s" : "")}", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
    }

    private void OpponentAppearance(float customY = 9.0f)
    {
        _musicControler.Play();
        var alreadyDialog = PlayerPrefsHelper.GetAlreadyDialog();
        if ((_currentStep == null || !_currentStep.LandLordVision)
            && (DialogsData.DialogTree.ContainsKey($"{CurrentOpponent.Name}|{Character.Name}") ||  DialogsData.DialogTree.ContainsKey($"{CurrentOpponent.Name}|Any"))
            && !alreadyDialog.Contains($"{CurrentOpponent.Name}|{Character.Name}"))
        {
            _gameplayControler.GameplayOnHold = true;
            PlayerPrefsHelper.AddToAlreadyDialog($"{CurrentOpponent.Name}|{Character.Name}");
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, CurrentOpponent.Name, Character.Name, Character.StartingRealm, Appearance);
        }
        else
            Appearance();

        void Appearance()
        {
            _gameplayControler.GameplayOnHold = false;
            Instantiator.PopText(CurrentOpponent.Name.ToLower() + " appears!", new Vector2(4.5f, customY));
        }
    }

    protected void NextOpponent(bool sceneInit = false)
    {
        if (Cache.CurrentListOpponentsId >= _opponents.Count)
        {
            var effectsCamera = GameObject.Find("EffectsCamera");
            if (effectsCamera != null)
                effectsCamera.GetComponent<EffectsCameraBhv>()?.Reset();
            if (CurrentOpponent.Type == OpponentType.Boss)
            {
                _gameplayControler.GameplayOnHold = true;
                Instantiator.NewDialogBoxDeath(CameraBhv.transform.position, CurrentOpponent.Name, () =>
                {
                    Helper.ReinitKeyboardInputs(this);
                    Victory();
                    ResetAndCleanCache();
                });
            }
            else
            {
                Victory();
                ResetAndCleanCache();
            }
            return;
        }
        CurrentOpponent = _opponents[Cache.CurrentListOpponentsId].Clone();
        Cache.EnemyCooldownInfiniteStairMalus = 0;
        if (Cache.RandomizedAttackType != AttackType.None)
            RandomizeOpponentAttack();
        if (Cache.CurrentOpponentChangedRealm != Realm.None)
            AlterOpponentRealm(Cache.CurrentOpponentChangedRealm, fromNextOpponent: true);
        if (Cache.HalvedCooldown)
            HalveOpponentMaxCooldown();
        if (Cache.NameLastScene == Constants.SettingsScene && sceneInit)
        {
            if (Cache.IsEffectAttackInProgress != AttackType.None)
            {
                Cache.CurrentOpponentAttackId = Cache.CurrentOpponentAttackId - 1 < 0 ? CurrentOpponent.Attacks.Count - 1 : Cache.CurrentOpponentAttackId - 1;
                Cache.IsEffectAttackInProgress = AttackType.None;
                Cache.CurrentOpponentCooldown = GetCurrentOpponentMaxCooldown();
                UpdateCooldownBar(Direction.Up);
                OpponentAttackIncoming();
            }
        }
        else
        {
            _soundControler.PlaySound(_idOpponentAppearance);
            Cache.CurrentOpponentAttackId = 0;
            Cache.CurrentOpponentAttackCount = 0;
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
        Cache.CurrentOpponentHp = Cache.CurrentOpponentHp <= 0 ? CurrentOpponent.HpMax : Cache.CurrentOpponentHp;
        _weaknessInstance.SetVisible(CurrentOpponent.Weakness != Weakness.None);
        _weaknessInstance.SetSkin(Helper.GetSpriteFromSpriteSheet("Sprites/WeaknessImmunity_" + (CurrentOpponent.Realm.GetHashCode() * 2)));
        _immunityInstance.SetVisible(CurrentOpponent.Immunity != Immunity.None);
        _immunityInstance.SetSkin(Helper.GetSpriteFromSpriteSheet("Sprites/WeaknessImmunity_" + (CurrentOpponent.Realm.GetHashCode() * 2 + 1)));
        _opponentHpBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 0),
                               "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 1),
                               $"<material=\"{CurrentOpponent.Realm.ToString().ToLower()}.4.3\">");
        _opponentHpBar.UpdateContent(0, CurrentOpponent.HpMax);
        _opponentHpBar.UpdateContent(Cache.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 2),
                                     "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 3));
        _opponentCooldownBar.UpdateContent(0, 1);
        ResetToOpponentGravity(true);
        _gameplayControler.OnNextOpponent();
        StartOpponentCooldown(sceneInit, true);
    }

    private void ResetAndCleanCache()
    {
        Cache.ResetClassicGameCache();
        if (_pacts != null && _pacts.Count > 0)
            for (int i = _pacts.Count - 1; i >= 0; --i)
            {
                if (++_pacts[i].NbFight == _pacts[i].MaxFight)
                    _pacts.RemoveAt(i);
            }
        PlayerPrefsHelper.SetPacts(_pacts);
    }

    public void ResetToOpponentGravity(bool fromOpponentSpawn = false)
    {
        _gameplayControler.SetGravity(CurrentOpponent.GravityLevel + ((Run?.RealmLevel ?? 1) - 1), fromOpponentSpawn);
    }

    public void RandomizeOpponentAttack()
    {
        if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece)
            Destroy(_gameplayControler.ForcedPiece);
        else if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.Drill)
        {
            var tmpDrillTarget = GameObject.Find(Constants.GoDrillTarget);
            if (tmpDrillTarget != null)
                Destroy(tmpDrillTarget);
        }

        CurrentOpponent.Attacks = new List<OpponentAttack> { new OpponentAttack(Cache.RandomizedAttackType, UnityEngine.Random.Range(1, 6), 1) };
        Cache.CurrentOpponentAttackId = 0;
        if (Cache.RandomizedAttackType == AttackType.Drill)
            _gameplayControler.AttackDrill(OpponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].Param1, true);
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
        _opponentHpBar.UpdateContent(Cache.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.SetSkin("Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 2),
                                     "Sprites/Bars_" + (CurrentOpponent.Realm.GetHashCode() * 4 + 3));
        _opponentCooldownBar.UpdateContent(0, 1);
    }

    private bool Victory()
    {
        PlayerPrefsHelper.SaveIsInFight(false);
        _gameplayControler.GameplayOnHold = true;
        //_isVictorious = true;
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
        _gameplayControler.CleanPlayerPrefs();

        if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
        {
            LoadBackAfterVictory(false);
            return false;
        }

        _currentStep = _stepsService.GetStepOnPos(Run.X, Run.Y, Run.Steps);
        var loot = Helper.GetLootFromTypeAndId(_currentStep.LootType, _currentStep.LootId);
        if (Cache.PactNoLoot)
            loot.LootType = LootType.None;
        if (loot?.LootType == LootType.Character)
            PlayerPrefsHelper.AddUnlockedCharacters((Character)loot); //Done here in order to prevent generating a step with the just unlocked character

        _stepsService.ClearLootOnPos(Run.X, Run.Y, Run);
        if (Run.CurrentStep > Character.LandLordLateAmount)
            _stepsService.SetVisionOnRandomStep(Run);
        _stepsService.GenerateAdjacentSteps(Run, Character, _currentStep);
        Run.CurrentItemCooldown = Cache.CurrentItemCooldown - Mathf.RoundToInt(_realmTree.PosthumousItem * Helper.MultiplierFromPercent(1.0f, this.Character.RealmTreeBoost));
        Run.CurrentItemUses = Cache.CurrentItemUses;
        PlayerPrefsHelper.SaveRun(Run);
        if (loot?.LootType == LootType.Character)
        {
            Instantiator.NewDialogBoxEncounter(CameraBhv.transform.position, ((Character)loot).Name, Character.Name, Character.StartingRealm, AfterCharacterDialog);
            void AfterCharacterDialog() {
                Helper.ReinitKeyboardInputs(this);
                _musicControler.Play(Constants.VictoryAudioClip, once: true);
                Instantiator.NewPopupYesNo("New Character", $"you unlocked {((Character)loot).Name.ToLower()}, a new playable character!", null, "Noice!", LoadBackAfterVictory);}
        }
        else if (loot?.LootType == LootType.Item)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            var currentItem = PlayerPrefsHelper.GetCurrentItem();
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + ((Item)loot).Id);
            if (currentItem == null)
            {
                Instantiator.NewPopupYesNo("New Item", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Item)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " added to your gear.", null, "Ok", LoadBackAfterVictory, sprite);
                PlayerPrefsHelper.SaveCurrentItem(((Item)loot).Name);
                Run.CurrentItemCooldown = 0;
                Run.CurrentItemUses = ((Item)loot).Uses;
                PlayerPrefsHelper.SaveRun(Run);
            }
            else if (currentItem.Id == ((Item)loot).Id)
            {
                Instantiator.NewPopupYesNo("Same Item", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "well... this is awkward... you already use " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + currentItem.Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "...", null, "Oh...", LootResources, sprite);
                Run.CurrentItemCooldown = 0;
                Run.CurrentItemUses = ((Item)loot).Uses;
                PlayerPrefsHelper.SaveRun(Run);
            }
            else
            {
                var content = Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "switch your " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + currentItem.Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " for " + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Item)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + "?";
                Instantiator.NewPopupYesNo("New Item", content, "No", "Yes", OnItemSwitch, sprite, defaultPositive: true);
                void OnItemSwitch(bool result)
                {
                    if (result)
                    {
                        PlayerPrefsHelper.SaveCurrentItem(((Item)loot).Name);
                        Run.CurrentItemCooldown = 0;
                        Run.CurrentItemUses = ((Item)loot).Uses;
                        PlayerPrefsHelper.SaveRun(Run);
                        LoadBackAfterVictory(true);
                    }
                    else
                        LootResources(instead: true);
                    
                }
            }
        }
        else if (loot?.LootType == LootType.Resource)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            LootResources();
        }
        else if (loot?.LootType == LootType.Tattoo)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            var nameToCheck = ((Tattoo)loot).Name.Replace(" ", "").Replace("'", "").Replace("-", "");
            var tattoos = PlayerPrefsHelper.GetCurrentTattoosString();
            var bodyPart = PlayerPrefsHelper.AddTattoo(((Tattoo)loot).Name, tryAdd: true);
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + ((Tattoo)loot).Id);
            if (bodyPart == BodyPart.MaxLevelReached)
            {
                Instantiator.NewPopupYesNo("Max Level", Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43) + ((Tattoo)loot).Name.ToLower() + Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32) + " reached its maximum level.", null, "Damn...", LootResources, sprite);
            }
            else if (bodyPart == BodyPart.None)
            {
                Instantiator.NewPopupYesNo("Filled!", $"sadly, you don't have any place left to ink anything...", null, "But...", LootResources, sprite);
            }
            else 
            {
                if (!tattoos.Contains(nameToCheck))
                {
                    Instantiator.NewPopupYesNo("Tattoo", $"are you ready to get inked?", "Nope!", "Sure!", InkTattoo, sprite, defaultPositive: true);

                    void InkTattoo(bool ink)
                    {
                        if (ink)
                        {
                            _soundControler.PlaySound(_idTattooSound);
                            PlayerPrefsHelper.AddTattoo(((Tattoo)loot).Name);
                            Helper.ReinitKeyboardInputs(this);
                            Instantiator.NewPopupYesNo("New Tattoo", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{((Tattoo)loot).Name.ToLower()}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} has been inked \non your {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{bodyPart.GetDescription().ToLower()}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}.", null, "Ouch!", LoadBackAfterVictory, sprite);
                        }
                        else
                            LootResources(instead: true);
                    }
                }
                else
                {
                    _soundControler.PlaySound(_idTattooSound);
                    PlayerPrefsHelper.AddTattoo(((Tattoo)loot).Name);
                    Instantiator.NewPopupYesNo("Tattoo Upgrade", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}your {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{((Tattoo)loot).Name.ToLower()}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} power has been increased.", null, "Noice", LoadBackAfterVictory, sprite);
                }
                ((Tattoo)loot).ApplyToCharacter(Character);
                PlayerPrefsHelper.SaveRunCharacter(Character);
            }
        }
        else if (loot?.LootType == LootType.Pact)
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            var nameToCheck = ((Pact)loot).Name.Replace(" ", "").Replace("'", "").Replace("-", "");
            var pacts = PlayerPrefsHelper.GetCurrentPactsString();
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Pacts_" + ((Pact)loot).Id);

            if (pacts.Contains(nameToCheck))
                Instantiator.NewPopupYesNo("Ongoing Pact", $"this pact is already signed, you cannot commit to a same pact twice.", null, "Damn...", LootResources, sprite);
            else
                Instantiator.NewPopupYesNo("New Pact", $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}{((Pact)loot).FullDescription()}", "Withdraw", "Endorse", OnPactSign, sprite, defaultPositive: true);
            void OnPactSign(bool result)
            {
                if (result)
                {
                    PlayerPrefsHelper.AddPact(((Pact)loot).Name);
                    LoadBackAfterVictory(true);
                }
                else
                    LootResources(instead: true);
            }
        }
        else
        {
            _musicControler.Play(Constants.VictoryAudioClip, once: true);
            LoadBackAfterVictory(false);
        }
        return true;

        void LootResources(bool instead = false)
        {
            var insteadStr = string.Empty;
            var title = "Resources";
            if (instead)
            {
                title = "Booby Prize";
                insteadStr = " instead";
                loot = ResourcesData.GetResourceFromName(ResourcesData.Resources[this.Run.CurrentRealm.GetHashCode()]);
                Helper.ReinitKeyboardInputs(this);
            }
            var amount = 2;
            if (Run.Difficulty == Difficulty.Easy)
                amount = 1;
            else if (Run.Difficulty == Difficulty.Hard)
                amount = 3;
            else if (Run.Difficulty == Difficulty.Infernal)
                amount = 4;
            else if (Run.Difficulty == Difficulty.Divine || Run.Difficulty.GetHashCode() > Difficulty.Divine.GetHashCode())
                amount = 5;
            if (Character.ResourceFarmBonus > 0)
                amount += Character.ResourceFarmBonus;
            Run.AlterResource(((Resource)loot).Id, amount);
            PlayerPrefsHelper.AlterTotalResource(((Resource)loot).Id, amount);
            PlayerPrefsHelper.SaveRun(Run);
            Instantiator.NewPopupYesNo(title, $"+{amount} {((Resource)loot).Name.ToLower()}{(amount > 1 ? "s" : "")}{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)} added to your resources{insteadStr}.", null, "Ka-Ching!", LoadBackAfterVictory);
        }
    }

    private void LoadBackAfterVictory(bool result)
    {
        if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
            NavigationService.LoadBackUntil(Constants.CharSelScene);
        else
        {
            if (CurrentOpponent.Type == OpponentType.Boss)
            {
                Cache.CurrentBossId = 0;
                PlayerPrefsHelper.IncrementRunBossVanquished();
                var realmIdBeforeIncrease = Run.CurrentRealm.GetHashCode();
                var hasUnlockedSkin = false;
                Run.IncreaseLevel(this.Character);
                var currentItem = PlayerPrefsHelper.GetCurrentItem();
                if (currentItem != null)
                    Run.CurrentItemUses = currentItem.Uses;
                if (currentItem != null && currentItem.Name == ItemsData.Items[25])
                    ++Run.DeathScytheAscension;
                if (Character.LastStandMultiplier > 0)
                    Cache.HasLastStanded = false;
                PlayerPrefsHelper.SaveRun(Run);
                if (Run.CurrentRealm.GetHashCode() > realmIdBeforeIncrease && Helper.UnlockCharacterSkinIfNotAlready(Character.Id, realmIdBeforeIncrease))
                    hasUnlockedSkin = true;
                if (Run.CurrentRealm.GetHashCode() > realmIdBeforeIncrease && realmIdBeforeIncrease > PlayerPrefsHelper.GetRealmBossProgression())
                {
                    PlayerPrefsHelper.SaveRealmBossProgression(realmIdBeforeIncrease);
                    Helper.ReinitKeyboardInputs(this);
                    var content = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}you now start your ascensions with a random item.\n(up to a {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{((Rarity)realmIdBeforeIncrease).ToString().ToLower()}{Constants.MaterialEnd} one).";
                    Instantiator.NewPopupYesNo($"{CurrentOpponent.Name} beaten!", content, null, "Neat!", CheckForSkin);
                }
                else
                {
                    CheckForSkin(true);
                    return;
                }

                void CheckForSkin(bool result)
                {
                    if (hasUnlockedSkin)
                    {
                        Helper.ReinitKeyboardInputs(this);
                        var skinContent = $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c32)}you unlocked a {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}new skin{Constants.MaterialEnd} on {Character.Name.ToLower()}.";
                        Instantiator.NewPopupYesNo($"{CurrentOpponent.Name} beaten!", skinContent, null, "Neat!", LoadNextAfterBoss);
                    }
                    else
                        LoadNextAfterBoss(true);
                }
                
                void LoadNextAfterBoss(bool result)
                {
                    if (Run.CurrentRealm == Realm.End)
                    {
                        if (Run.Difficulty == Difficulty.Hard)
                            PlayerPrefsHelper.SaveInfernalUnlocked(true);
                        if (Run.Difficulty == Difficulty.Infernal)
                            PlayerPrefsHelper.SaveDivineUnlocked(true);
                        Cache.GameOverParams = $"{Character.Name}|{Run.CurrentRealm - 1}|3|{Constants.EndScene}";
                        PlayerPrefsHelper.EndlessRun(Run);
                        NavigationService.LoadNextScene(Constants.LoreScene, new NavigationParameter() { IntParam0 = Realm.End.GetHashCode(), StringParam0 = Constants.EndScene });
                        return;
                    }
                    if (!PlayerPrefsHelper.IsCinematicWatched(Run.CurrentRealm.GetHashCode()))
                        NavigationService.LoadNextScene(Constants.LoreScene, new NavigationParameter() { IntParam0 = Run.CurrentRealm.GetHashCode(), StringParam0 = Constants.StepsAscensionScene });
                    else
                        NavigationService.LoadBackUntil(Constants.StepsAscensionScene);
                }
            }
            else
            {
                NavigationService.LoadBackUntil(Constants.StepsScene);
            }
            
        }
    }

    private void StartOpponentCooldown(bool sceneInit = false, bool first = false)
    {
        if ((Character.SlumberingDragoncrest || Cache.PactStealth) && Cache.CurrentListOpponentsId == 0 && first)
        {
            Cache.SlumberingDragoncrestInEffect = true;
            _opponentOnCooldown = false;
            return;
        }
        _opponentOnCooldown = true;
        if (!sceneInit)
            Cache.CurrentOpponentCooldown = 0;
        if (first && (CurrentOpponent.Haste || Character.HasteForAll || Cache.PactOnlyHaste))
        {
            if (!sceneInit)
                Instantiator.PopText("haste", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
            if (Character.HasteCancel || Cache.PactNoHaste)
                Instantiator.PopText("canceled", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
            else
                Cache.CurrentOpponentCooldown = GetCurrentOpponentMaxCooldown();
        }
        if (Character.EnemyCooldownInfiniteStairMalus > 0)
            Cache.EnemyCooldownInfiniteStairMalus += Character.EnemyCooldownInfiniteStairMalus;
        if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.Drill)
            _gameplayControler.AttackDrill(OpponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].Param1, true);
        SetNextCooldownTick();
    }

    private void SetNextCooldownTick()
    {
        if (_timeStopTimer > 0)
            return;
        if (!Paused && !_gameplayControler.GameplayOnHold && _opponentOnCooldown && Cache.CurrentOpponentCooldown > GetCurrentOpponentMaxCooldown())
            OpponentAttackIncoming();
        else
        {
            //if (Cache.CurrentOpponentCooldown >= CurrentOpponent.Cooldown - 1.0f)
            //{
            //    var maxHeight = 15.0f;
            //    var highestBlockY = _gameplayControler.GetHighestBlock();
            //    if (maxHeight > highestBlockY + 3)
            //        maxHeight = highestBlockY + 4;
            //    Instantiator.PopText($"coming:\n{CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType}", new Vector2(4.5f, maxHeight));
            //}

            if ((Character.HighPlayPause && _gameplayControler.GetHighestBlock() >= 17)
                || _stunIcon.IsOn)
                _nextCooldownTick = Time.time + Constants.OpponentCooldownOneHour;
            else if (_opponentOnCooldown)
                _nextCooldownTick = Time.time + Constants.OpponentCooldownIncrement;
        }
    }

    public void OpponentAttackIncoming()
    {
        _opponentOnCooldown = false;
        _nextCooldownTick = Time.time + Constants.OpponentCooldownOneHour;
        StartCoroutine(Helper.ExecuteAfterDelay(0.5f, () => { _gameplayControler.AttackIncoming = true; }, lockInputWhile: false));
    }

    public void StopTime(int seconds)
    {
        _gameplayControler.SetGravity(0);
        _nextCooldownTick = Time.time + Constants.OpponentCooldownOneHour;
        _timeStopTimer = Time.time + seconds;
    }

    public override bool OpponentAttack()
    {
        bool spawnAfterAttack = true;
        CameraBhv.Bump(2);
        OpponentInstanceBhv.Attack();
        
        if (Cache.IsNextOpponentAttackCanceled)
        {
            Cache.IsNextOpponentAttackCanceled = false;
            Instantiator.PopText("canceled", OpponentInstanceBhv.transform.position + new Vector3(3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idDodge);
            _characterInstanceBhv.Dodge();
        }
        else if (Cache.BlockPerAttack >= 0 && ++Cache.BlockPerAttack >= 3)
        {
            Cache.BlockPerAttack = 0;
            Instantiator.PopText("blocked", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idHit);
        }
        else if (Character.HolyMantle > 0 && Cache.CurrentOpponentAttackCount < Character.HolyMantle)
        {
            Instantiator.PopText("blessed", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idDodge);
            _characterInstanceBhv.Dodge();
        }
        else if (Cache.CurrentRemainingSimpShields > 0)
        {
            --Cache.CurrentRemainingSimpShields;
            var shieldObjects = GameObject.FindGameObjectsWithTag(Constants.TagSimpShield);
            if (shieldObjects != null && shieldObjects.Length > 0)
            {
                Instantiator.PopText("blocked", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
                _soundControler.PlaySound(_idHit);
                shieldObjects[shieldObjects.Length - 1].GetComponent<CharacterInstanceBhv>().GetOS();
            }
        }
        else if (Helper.RandomDice100(Character.DodgeChance + Cache.AddedDodgeChancePercent))
        {
            Instantiator.PopText("dodged", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
            _soundControler.PlaySound(_idDodge);
            _characterInstanceBhv.Dodge();
        }
        else
        {
            _characterInstanceBhv.TakeDamage();
            _gameplayControler.OpponentAttack(
                CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType,
                CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].Param1,
                CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].Param2,
                CurrentOpponent.Realm,
                OpponentInstanceBhv.gameObject);
            if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece
                || CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.Shift
                || CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.Ascension)
                spawnAfterAttack = false;
        }
        ++Cache.CurrentOpponentAttackCount;
        if (++Cache.CurrentOpponentAttackId >= CurrentOpponent.Attacks.Count)
            Cache.CurrentOpponentAttackId = 0;
        _opponentCooldownBar.UpdateContent(0, 1, Direction.Down);
        _opponentCooldownBar.ResetTilt();
        if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType != AttackType.ForcedPiece && _gameplayControler.ForcedPiece != null)
            Destroy(_gameplayControler.ForcedPiece);
        if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType != AttackType.Drill && Helper.TryFind(Constants.GoDrillTarget, out var tmpDrillTarget))
            Destroy(tmpDrillTarget);
        Destroy(_gameplayControler.ForcedPiece);
        StartOpponentCooldown();
        if (Character.ThornsPercent > 0)
        {
            var thornDamage = Mathf.RoundToInt(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.ThornsPercent));
            DamageOpponent(thornDamage == 0 ? 1 : thornDamage, _gameplayControler.CharacterInstanceBhv.gameObject);
        }
        return spawnAfterAttack;
    }

    protected override void FrameUpdate()
    {
        if (Paused || _gameplayControler.GameplayOnHold)
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
            ResetToOpponentGravity();
            _nextCooldownTick = Time.time + Constants.OpponentCooldownIncrement;
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
        if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece && _gameplayControler.ForcedPiece == null)
        {
            _gameplayControler.AttackForcedPiece(OpponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].Param1, CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].Param2);
            _gameplayControler.SetForcedPieceOpacity(Cache.CurrentOpponentCooldown, GetCurrentOpponentMaxCooldown());
        }
        if (_opponentOnCooldown && Time.time >= _nextCooldownTick)
        {
            Cache.CurrentOpponentCooldown += Constants.OpponentCooldownIncrement;
            _gameplayControler.SetForcedPieceOpacity(Cache.CurrentOpponentCooldown, GetCurrentOpponentMaxCooldown());
            UpdateCooldownBar(Direction.Up);
            SetNextCooldownTick();
        }
    }

    public void RestartOpponentCooldown()
    {
        Cache.CurrentOpponentCooldown = 0;
        UpdateCooldownBar(Direction.Down);
        _opponentOnCooldown = true;
        _nextCooldownTick = Time.time + Constants.OpponentCooldownIncrement;
        _gameplayControler.AttackIncoming = false;
    }

    private void UpdateCooldownBar(Direction direction)
    {
        _opponentCooldownBar.UpdateContent(Cache.CurrentOpponentCooldown, GetCurrentOpponentMaxCooldown(), direction);
    }

    public override void OnGameOver()
    {
        base.OnGameOver();
        _characterInstanceBhv.Die();
        if (Cache.CurrentGameMode == GameMode.TrainingFree
            || Cache.CurrentGameMode == GameMode.TrainingDummy)
            NavigationService.LoadBackUntil(Constants.CharSelScene);
        else
        {
            Cache.GameOverParams = $"{CurrentOpponent.Name}|{Run.CurrentRealm}|{Run.RealmLevel}";
            if (Run.CharacterEncounterAvailability)
                PlayerPrefsHelper.IncrementNumberRunWithoutCharacterEncounter();
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

    public override bool DamageOpponent(int amount, GameObject source, Realm? textRealm = null, bool attackLine = true)
    {
        var damageTextPosition = _opponentHpBar.transform.position + new Vector3(1.0f, 1.6f, 0.0f);
        if (Cache.SheleredAttacksCount > 0)
        {
            amount = 0;
            Instantiator.PopText($"{Constants.GetMaterial(this.CurrentOpponent.Realm, TextType.succubus3x5, TextCode.c43)}shelter", damageTextPosition);
            _soundControler.PlaySound(_idDodge);
            this.OpponentInstanceBhv.Dodge();
            Cache.SheleredAttacksCount--;
        }
        if (Cache.CurrentOpponentHp <= 0 && CurrentOpponent.IsDead)
            return false;
        if (Character.QuadDamage > 0 && Cache.CurrentListOpponentsId == 0 && _opponents.Count >= 4)
        {
            amount *= Character.QuadDamage;
            _characterInstanceBhv.Boost(Realm.None, 1.0f);
        }
        var realm = _gameplayControler.CharacterRealm;
        var sourcePosition = _characterInstanceBhv.transform.position;
        Piece piece = null;
        if (source != null)
        {
            sourcePosition = source.transform.position;
            piece = source.GetComponent<Piece>();
        }
        if (piece != null && piece.IsMimic)
        {
            realm = Helper.GetInferiorFrom(_gameplayControler.CharacterRealm);
            if (source.transform.position.x < 0)
                sourcePosition = new Vector3(0.0f, sourcePosition.y, 0.0f);
            else if (source.transform.position.x > 9)
                sourcePosition = new Vector3(9.0f, sourcePosition.y, 0.0f);
        }
        if (attackLine)
            Instantiator.NewAttackLine(sourcePosition, OpponentInstanceBhv.gameObject.transform.position, realm);
        OpponentInstanceBhv.TakeDamage();
        Cache.CurrentOpponentHp -= amount;
        CameraBhv.Bump(2);
        var attackText = "-" + amount;
        if (amount == 69)
            attackText = "nice";
        if (!_isCrit)
            PlayHit();
        else
        {
            var tmpCritIndicator = this.Instantiator.NewRhythmIndicator(Constants.ColorPlain);
            tmpCritIndicator.name = "TmpCritIndicator";
            tmpCritIndicator.transform.SetParent(_gameplayControler.PlayFieldBhv.transform);
            tmpCritIndicator.gameObject.AddComponent<FadeOnAppearanceBhv>().Init(0.075f, Constants.ColorPlain, recursiveChildren: true);
            attackText = $"</material>{attackText}!";
            Instantiator.PopText("!!!", _characterInstanceBhv.transform.position + new Vector3(-2f, 2.0f, 0.0f));
            _soundControler.PlaySound(_idCrit);
        }
        VibrationService.Vibrate();
        var poppingTexts = GameObject.FindGameObjectsWithTag(Constants.TagPoppingText);
        //Check for overlapping damage texts
        for (int i = 0; i < poppingTexts.Length; ++i)
        {
            if (Helper.VectorEqualsPrecision(poppingTexts[i].transform.position, damageTextPosition, 0.5f) && poppingTexts[i].transform.position.y >= damageTextPosition.y)
                damageTextPosition = poppingTexts[i].transform.position + new Vector3(0.0f, -1.1f);
        }
        var realmMaterial = "";
        if (textRealm == null || textRealm != Realm.None)
        {
            var tmpRealm = textRealm == null ? _gameplayControler.CharacterRealm : textRealm;
            realmMaterial = $"<material=\"{tmpRealm.ToString().ToLower()}.4.3\">";
        }
        Instantiator.PopText($"{realmMaterial}{attackText}", damageTextPosition);
        _opponentHpBar.UpdateContent(Cache.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Left);
        if (Cache.SlumberingDragoncrestInEffect)
        {
            StartOpponentCooldown();
            Cache.SlumberingDragoncrestInEffect = false;
        }
        if (Cache.CurrentOpponentHp <= 0)
        {
            KillOpponent();
            return true;
        }
        else
        {
            if (_opponentOnCooldown && Cache.CurrentOpponentCooldown < GetCurrentOpponentMaxCooldown())
            {
                if (CurrentOpponent.Immunity != Immunity.Cooldown)
                    Cache.CurrentOpponentCooldown -= Character.EnemyCooldownProgressionReducer;
                if (Cache.CurrentOpponentCooldown <= 0)
                    Cache.CurrentOpponentCooldown = 0;
                UpdateCooldownBar(Direction.Down);
            }
            SetNextCooldownTick();
            return false;
        }
    }

    public void KillOpponent()
    {
        _stunIcon.Hide();
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
        _gameplayControler.PlayFieldBhv.ShowSemiOpcaity(1);
        _gameplayControler.OpponentDeathScreen = true;
        _soundControler.PlaySound(_idOpponentDeath);
        var minHeight = 9.0f;
        var highestBlockY = _gameplayControler.GetHighestBlock();
        if (minHeight < highestBlockY)
            minHeight = highestBlockY + 2;
        Instantiator.PopText(CurrentOpponent.Name.ToLower() + " defeated!", new Vector2(4.5f, minHeight));
        OpponentInstanceBhv.Die();
        CurrentOpponent.IsDead = true;
        _opponentOnCooldown = false;
        Cache.CurrentOpponentCooldown = 0;
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
                bossHateBonusPercent = Mathf.RoundToInt(_realmTree.BossHate * Helper.MultiplierFromPercent(1.0f, this.Character.RealmTreeBoost));
            DamageOpponent(Mathf.RoundToInt(_characterAttack * Helper.MultiplierFromPercent(1.0f, bossHateBonusPercent)), lastPiece);
        }
        else if (_characterAttack < 0)
            Instantiator.PopText("missed", _characterInstanceBhv.transform.position + new Vector3(-3f, 0.0f, 0.0f));
        _characterAttack = 0;
        _isCrit = false;
    }

    private object AfterOpponentDeath()
    {
        var opponentIcon = GameObject.Find("Opponent" + Cache.CurrentListOpponentsId);
        opponentIcon.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/OpponentsIcons_" + ((_opponents[Cache.CurrentListOpponentsId].Realm.GetHashCode() * 2) + 1));
        opponentIcon.GetComponent<IconInstanceBhv>().Pop();
        ++Cache.CurrentListOpponentsId;
        if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece)
            Destroy(_gameplayControler.ForcedPiece);
        else if (CurrentOpponent.Attacks[Cache.CurrentOpponentAttackId].AttackType == AttackType.Drill)
        {
            var tmpDrillTarget = GameObject.Find(Constants.GoDrillTarget);
            if (tmpDrillTarget != null)
                Destroy(tmpDrillTarget);
        }
        if (Character.ItemCooldownReducerOnKill > 0 && PlayerPrefsHelper.GetCurrentItemName() != null)
        {
            Cache.CurrentItemCooldown -= Character.ItemCooldownReducerOnKill;
            _gameplayControler.UpdateItemAndSpecialVisuals();
        }
        if (Character.TrashAfterKill > 0)
            _gameplayControler.DeleteFromBottom(Character.TrashAfterKill);
        Cache.RandomizedAttackType = AttackType.None;
        Cache.HalvedCooldown = false;
        Cache.CurrentOpponentChangedRealm = Realm.None;
        NextOpponent();
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = false;
        _gameplayControler.PlayFieldBhv.ShowSemiOpcaity(0);
        _gameplayControler.OpponentDeathScreen = false;
        return true;
    }

    public override void OnPieceLocked(string pieceLetterTwist)
    {
        base.OnPieceLocked(pieceLetterTwist);
        if (string.IsNullOrEmpty(pieceLetterTwist))
            return;
        var incomingDamage = 0;
        if (CurrentOpponent.Weakness == Weakness.Twists)
        {
            _weaknessInstance.Pop();
            _soundControler.PlaySound(_idWeakness);
            incomingDamage += CurrentOpponent.DamageOnWeakness;
        }
        if (CurrentOpponent.Immunity == Immunity.Twists)
        {
            _immunityInstance.Pop();
            _soundControler.PlaySound(_idImmunity);
            incomingDamage = 0;
        }
        _characterAttack += incomingDamage;
    }

    public override void OnLinesCleared(int nbLines, bool isB2B, bool lastLockIsTwist)
    {
        base.OnLinesCleared(nbLines, isB2B, lastLockIsTwist);
        var incomingDamage = 0;
        if (nbLines > 0)
        {
            if (Character.DamoclesDamage > 0 && nbLines >= 4)
            {
                Cache.BonusDamage += Character.DamoclesDamage;
                _characterInstanceBhv.Boost(_gameplayControler.CharacterRealm, 0.25f);
            }

            incomingDamage = Character.GetAttack();
            if (nbLines == 1)
                incomingDamage += Character.SingleLineDamageBonus;
            if ((Character.DamageSmallLinesBonus > 0 || Character.DamageSmallLinesMalus > 0) && nbLines >= 1 && nbLines <= 2)
                incomingDamage = Mathf.RoundToInt(incomingDamage * Helper.MultiplierFromPercent(1.0f, Character.DamageSmallLinesBonus - Character.DamageSmallLinesMalus));
            if ((Character.DamageBigLinesBonus > 0 || Character.DamageBigLinesMalus > 0) && nbLines >= 3)
                incomingDamage = Mathf.RoundToInt(incomingDamage * Helper.MultiplierFromPercent(1.0f, Character.DamageBigLinesBonus - Character.DamageBigLinesMalus));
            if (!Cache.PactNoCrit && Helper.RandomDice100(Character.GetCriticalChancePercent()))
            {
                Cache.CumulativeCrit += Character.CumulativeCrit;
                incomingDamage += Mathf.RoundToInt(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.CritMultiplier));
                _isCrit = true;
            }
            else if (Character.CumulativeNotCrit > 0) // Not Critical
                Cache.CumulativeCrit += Character.CumulativeNotCrit;
            if (Helper.IsSuperiorByRealm(_gameplayControler.CharacterRealm, CurrentOpponent.Realm))
                incomingDamage = Mathf.RoundToInt(incomingDamage * Helper.MultiplierFromPercent(1.0f, Character.DamagePercentToInferiorRealm));
            incomingDamage *= nbLines;
            if (nbLines == 1 && Character.SingleLinesDamageOverride > 0)
                incomingDamage = Character.SingleLinesDamageOverride;
            else if (nbLines == 4 && Character.QuadrupleLinesDamageOverride > 0)
                incomingDamage = Character.QuadrupleLinesDamageOverride;
            if (CurrentOpponent.Weakness == Weakness.xLines && CurrentOpponent.XLineWeakness == nbLines)
            {
                _weaknessInstance.Pop();
                _soundControler.PlaySound(_idWeakness);
                incomingDamage += CurrentOpponent.DamageOnWeakness;
            }
            if (CurrentOpponent.Immunity == Immunity.xLines && CurrentOpponent.XLineImmunity == nbLines)
            {
                _immunityInstance.Pop();
                _soundControler.PlaySound(_idImmunity);
                incomingDamage = 0;
            }
            if (Cache.SlavWheelStreak > 0)
            {
                var slavBonus = Mathf.RoundToInt(incomingDamage * Helper.MultiplierFromPercent(0.0f, this.Character.SlavWheelDamagePercentBonus * Cache.SlavWheelStreak));
                if (slavBonus == 0)
                    slavBonus = 1;
                incomingDamage += slavBonus;
            }
            if (_gameplayControler.CharacterRealm == Realm.Earth && nbLines >= 4)
            {
                int linesDestroyed = Character.RealmPassiveEffect;
                linesDestroyed -= _gameplayControler.CheckForDarkRows(linesDestroyed);
                if (linesDestroyed > 0)
                    linesDestroyed -= _gameplayControler.CheckForWasteRows(linesDestroyed);
                if (linesDestroyed > 0)
                {
                    for (int i = 0; i < linesDestroyed; ++i)
                        _gameplayControler.CheckForLightRows();
                }

            }
            if (Character.OwlReduceSeconds > 0 && nbLines >= 4)
                Cache.CurrentOpponentCooldown -= Character.OwlReduceSeconds;

            //ELEMENTS STONES
            if (nbLines == 3)
            {
                if (Character.EarthStun > 0)
                    StunOpponent(Character.EarthStun);
                if (Character.WaterDamagePercent > 0)
                    WetMalusOpponent(Character.WaterDamagePercent, 4.0f);
                if (Character.FireDamagePercent > 0)
                    StartCoroutine(Burn(3));
                if (Character.WindTripleBonus > 0)
                {
                    Cache.TripleLineDamageBonus += Character.WindTripleBonus;
                    StartCoroutine(WindBoost());
                }
            }
        }
        if (lastLockIsTwist)
        {
            incomingDamage *= 2;
            if (Character.InstantLineClear > 0)
            {
                int linesDestroyed = Character.InstantLineClear;
                linesDestroyed -= _gameplayControler.CheckForDarkRows(linesDestroyed);
                if (linesDestroyed > 0)
                    _gameplayControler.CheckForWasteRows(linesDestroyed);
                _gameplayControler.ClearLineSpace();
            }
            if (_gameplayControler.CharacterRealm == Realm.Heaven)
            {
                Cache.SelectedCharacterSpecialCooldown -= Character.RealmPassiveEffect;
                Cache.CurrentItemCooldown -= Character.RealmPassiveEffect;
                _gameplayControler.UpdateItemAndSpecialVisuals();
            }
        }
        if (isB2B)
        {
            if (CurrentOpponent.Weakness == Weakness.Consecutive && nbLines >= 2)
            {
                _weaknessInstance.Pop();
                _soundControler.PlaySound(_idWeakness);
                incomingDamage += CurrentOpponent.DamageOnWeakness;
            }
            if (CurrentOpponent.Immunity == Immunity.Consecutive)
            {
                _immunityInstance.Pop();
                _soundControler.PlaySound(_idImmunity);
                incomingDamage = 0;
            }
        }
        _characterAttack += incomingDamage;
    }

    public IEnumerator Burn(int time)
    {
        yield return new WaitForSeconds(0.33f);
        if (time > 0)
        {
            var burnDamage = Mathf.RoundToInt(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.FireDamagePercent));
            DamageOpponent(burnDamage == 0 ? 1 : burnDamage, null, Realm.Hell, attackLine: false);
            StartCoroutine(Burn(time - 1));
        }
    }

    public IEnumerator WindBoost()
    {
        yield return new WaitForSeconds(0.25f);
        DamageOpponent(Cache.TripleLineDamageBonus, null, Realm.None, attackLine: false);
        _characterInstanceBhv.Boost(Realm.None, 1.0f);
    }

    public override void OnCombo(int nbCombo, int nbLines)
    {
        base.OnCombo(nbCombo, nbLines);
        if (_characterAttack < 0)
            return;
        var incomingDamage = 0;
        if (_gameplayControler.CharacterRealm == Realm.Hell)
            incomingDamage += Mathf.RoundToInt((Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, 10 * Character.RealmPassiveEffect) * (nbCombo - 1)) * nbLines);
        if (Cache.PactComboDamage > 0)
            incomingDamage += Mathf.RoundToInt((Cache.PactComboDamage * (nbCombo - 1))) * nbLines;
        if (CurrentOpponent.Weakness == Weakness.Combos)
        {
            _weaknessInstance.Pop();
            _soundControler.PlaySound(_idWeakness);
            incomingDamage += CurrentOpponent.DamageOnWeakness * (nbCombo - 1);
        }
        if (CurrentOpponent.Immunity == Immunity.Combos)
        {
            _immunityInstance.Pop();
            _soundControler.PlaySound(_idImmunity);
            incomingDamage = 0;
        }
        _characterAttack += incomingDamage;
    }

    public override void OnPerfectClear()
    {
        base.OnPerfectClear();
        if (_characterAttack < 0)
            return;
        else
            _characterAttack *= 2;
        if (Character.PerfectKills > 0)
            DamageOpponent(Character.PerfectKills, _gameplayControler.CharacterInstanceBhv.gameObject);
    }

    private void OnApplicationQuit()
    {
        //if (_isVictorious || _run == null)
        //    return;
        //if (Cache.CurrentGameMode == GameMode.Ascension
        //    || Cache.CurrentGameMode == GameMode.TrueAscension)
        //{
        //    _stepsService.ClearLootOnPos(_run.X, _run.Y, _run);
        //    _stepsService.SetVisionOnRandomStep(_run);
        //    _stepsService.GenerateAdjacentSteps(_run, Character, _currentStep);
        //}
    }
}
