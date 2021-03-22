using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicGameSceneBhv : GameSceneBhv
{
    private CharacterInstanceBhv _opponentInstanceBhv;
    private ResourceBarBhv _opponentHpBar;
    private ResourceBarBhv _opponentCooldownBar;
    private SpriteRenderer _opponentType;
    private bool _opponentOnCooldown;
    private float _nextCooldownTick;
    private IconInstanceBhv _weaknessInstance;
    private IconInstanceBhv _immunityInstance;

    private Run _run;
    private StepsService _stepsService;
    private Step _currentStep;

    private int _characterAttack;
    private int _cumulativeCrit;
    private bool _isCrit;
    private bool _isVictorious;

    private SoundControlerBhv _soundControler;
    private int _idOpponentDeath;
    private int _idOpponentAppearance;
    private int _idHit;
    private int _idCrit;
    private int _idWeakness;
    private int _idImmunity;

    void Start()
    {
        Init();
        _run = PlayerPrefsHelper.GetRun();
        _stepsService = new StepsService();
        if (Constants.CurrentGameMode == GameMode.TrainingDummy
            || Constants.CurrentGameMode == GameMode.TrainingFree)
        {
            _opponents = PlayerPrefsHelper.GetCurrentOpponents(new Run(Difficulty.Normal));
            Constants.ResetCurrentItemCooldown(Character, ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]));
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
        _cumulativeCrit = 0;
        _weaknessInstance = GameObject.Find("Weakness").GetComponent<IconInstanceBhv>();
        _immunityInstance = GameObject.Find("Immunity").GetComponent<IconInstanceBhv>();
        _opponentHpBar = GameObject.Find("OpponentHpBar").GetComponent<ResourceBarBhv>();
        _opponentCooldownBar = GameObject.Find("OpponentCooldownBar").GetComponent<ResourceBarBhv>();
        _opponentInstanceBhv = GameObject.Find(Constants.GoOpponentInstance).GetComponent<CharacterInstanceBhv>();
        _opponentInstanceBhv.AfterDeath = AfterOpponentDeath;
        _opponentType = GameObject.Find("OpponentType").GetComponent<SpriteRenderer>();
        _nextCooldownTick = Time.time + 3600;
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _idOpponentDeath = _soundControler.SetSound("OpponentDeath");
        _idOpponentAppearance = _soundControler.SetSound("OpponentAppearance");
        _idCrit = _soundControler.SetSound("Crit");
        _idHit = _soundControler.SetSound("Hit");
        _idWeakness = _soundControler.SetSound("Weakness");
        _idImmunity = _soundControler.SetSound("Immunity");
        GameObject.Find("InfoRealm").GetComponent<TMPro.TextMeshPro>().text = $"{Constants.MaterialHell_3_2B}realm:\n{ Constants.MaterialHell_4_3B}{ (_run?.CurrentRealm.ToString().ToLower() ?? Realm.Hell.ToString().ToLower())}\nlvl {_run?.RealmLevel.ToString() ?? "?"}";
        NextOpponent(sceneInit: true);
        _gameplayControler.GetComponent<GameplayControler>().StartGameplay(CurrentOpponent.GravityLevel, Character.Realm, _run?.CurrentRealm ?? Realm.Hell);
        
        Paused = true;
        Constants.InputLocked = true;
        if (Constants.NameLastScene == Constants.SettingsScene)
            AfterFightIntro();
        else
            Instantiator.NewFightIntro(new Vector3(CameraBhv.transform.position.x, CameraBhv.transform.position.y, 0.0f), Character, _opponents, AfterFightIntro);
    }

    private bool AfterFightIntro()
    {
        Constants.InputLocked = false;        
        Paused = false;
        OpponentAppearance();
        return true;
    }

    private void OpponentAppearance(float customY = 9.0f)
    {
        if (!_currentStep.LandLordVision
            && (DialogData.DialogTree.ContainsKey($"{CurrentOpponent.Name}|{Character.Name}")
            ||  DialogData.DialogTree.ContainsKey($"{CurrentOpponent.Name}|Any")))
        {
            Paused = true;
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
        CurrentOpponent = _opponents[Constants.CurrentListOpponentsId];
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
            _opponentInstanceBhv.Spawn();
            CameraBhv.Bump(4);
            var minHeight = 9.0f;
            var highestBlockY = _gameplayControler.GetHighestBlock();
            if (minHeight < highestBlockY)
                minHeight = highestBlockY + 1;
            if (!sceneInit)
                OpponentAppearance(minHeight);
        }
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{CurrentOpponent.Region}Opponents_{CurrentOpponent.Id}");
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
        StartOpponentCooldown(sceneInit);
    }

    private bool Victory()
    {
        PlayerPrefsHelper.SaveIsInFight(false);
        Paused = true;
        _isVictorious = true;
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
        _gameplayControler.CleanPlayerPrefs();

        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
        {
            LoadBackAfterVictory(false);
            return false;
        }

        _currentStep = _stepsService.GetStepOnPos(_run.X, _run.Y, _run.Steps);
        var loot = Helper.GetLootFromTypeAndId(_currentStep.LootType, _currentStep.LootId);
        _stepsService.ClearLootOnPos(_run.X, _run.Y, _run);
        if (_run.CurrentStep > Character.LandLordLateAmount)
            _stepsService.SetVisionOnRandomStep(_run);
        _stepsService.GenerateAdjacentSteps(_run, Character, _currentStep);
        _run.CurrentItemCooldown = Constants.CurrentItemCooldown;
        PlayerPrefsHelper.SaveRun(_run);
        if (loot.LootType == LootType.Character)
        {
            Instantiator.NewPopupYesNo("New Playable Character", "you unlocked a new playable character !", null, "Noice!", LoadBackAfterVictory);
            PlayerPrefsHelper.AddUnlockedCharacters((Character)loot);
        }
        else if (loot.LootType == LootType.Item)
        {
            var currentItem = PlayerPrefsHelper.GetCurrentItem();
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Items_" + ((Item)loot).Id);
            if (currentItem == null)
            {
                Instantiator.NewPopupYesNo("New Item", Constants.MaterialHell_4_3 + ((Item)loot).Name.ToLower() + Constants.MaterialHell_3_2 + " added to your gear", null, "Ok", LoadBackAfterVictory, sprite);
                PlayerPrefsHelper.SaveCurrentItem(((Item)loot).Name);
                Constants.ResetCurrentItemCooldown(Character, currentItem);
            }
            else if (currentItem.Id == ((Item)loot).Id)
            {
                Instantiator.NewPopupYesNo("Same Item", Constants.MaterialHell_3_2 + "well... this is awkward... you already use " + Constants.MaterialHell_4_3 + currentItem.Name.ToLower() + Constants.MaterialHell_3_2 + "...", null, "Oh...", LoadBackAfterVictory, sprite);
            }
            else
            {
                var content = Constants.MaterialHell_3_2 + "switch your " + Constants.MaterialHell_4_3 + currentItem.Name.ToLower() + Constants.MaterialHell_3_2 + " for " + Constants.MaterialHell_4_3 + ((Item)loot).Name.ToLower() + Constants.MaterialHell_3_2 + " ?";
                Instantiator.NewPopupYesNo("New Item", content, "No", "Yes", OnItemSwitch, sprite);
                object OnItemSwitch(bool result)
                {
                    if (result)
                    {
                        PlayerPrefsHelper.SaveCurrentItem(((Item)loot).Name);
                        Constants.ResetCurrentItemCooldown(Character, (Item)loot);
                    }
                    LoadBackAfterVictory(true);
                    return result;
                }
            }
        }
        else if (loot.LootType == LootType.Tattoo)
        {
            var nameToCheck = ((Tattoo)loot).Name.Replace(" ", "").Replace("'", "");
            var tattoos = PlayerPrefsHelper.GetCurrentTattoosString();
            var bodyPart = PlayerPrefsHelper.AddTattoo(((Tattoo)loot).Name);
            var sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Tattoos_" + ((Tattoo)loot).Id);
            if (bodyPart == BodyPart.MaxLevelReached)
            {
                Instantiator.NewPopupYesNo("Max Level", Constants.MaterialHell_4_3 + ((Tattoo)loot).Name.ToLower() + Constants.MaterialHell_3_2 + " reached its maximum level", null, "Damn...", LoadBackAfterVictory, sprite);
            }
            else 
            {
                if (!tattoos.Contains(nameToCheck))
                    Instantiator.NewPopupYesNo("New Tattoo", Constants.MaterialHell_4_3 + ((Tattoo)loot).Name.ToLower() + Constants.MaterialHell_3_2 + " has been inked \non your " + Constants.MaterialHell_4_3 + bodyPart.GetDescription().ToLower(), null, "Ouch!", LoadBackAfterVictory, sprite);
                else
                    Instantiator.NewPopupYesNo("Tattoo Upgrade", Constants.MaterialHell_3_2 + "your " + Constants.MaterialHell_4_3 + ((Tattoo)loot).Name.ToLower() + Constants.MaterialHell_3_2 + " power has been increased", null, "Noice", LoadBackAfterVictory, sprite);
                ((Tattoo)loot).ApplyToCharacter(Character);
                PlayerPrefsHelper.SaveRunCharacter(Character);
            }
        }
        else
        {
            LoadBackAfterVictory(false);
        }
        return true;
    }

    private object LoadBackAfterVictory(bool result)
    {
        Constants.CurrentMusicType = MusicType.Menu;
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            NavigationService.LoadBackUntil(Constants.CharSelScene);
        else
        {
            if (CurrentOpponent.Type == OpponentType.Boss)
            {
                _run.IncreaseLevel();
                PlayerPrefsHelper.SaveRun(_run);
                //DEBUG
                if (_run.CurrentRealm == Realm.Earth)
                {
                    PlayerPrefsHelper.ResetRun();
                    NavigationService.LoadNextScene(Constants.DemoEndScene);
                    return false;
                }
                //DEBUG
                NavigationService.LoadBackUntil(Constants.StepsAscensionScene);
            }
            else
            {
                NavigationService.LoadBackUntil(Constants.StepsScene);
            }
            
        }
        return result;
    }

    private void StartOpponentCooldown(bool sceneInit = false)
    {
        _opponentOnCooldown = true;
        if (!sceneInit)
            Constants.CurrentOpponentCooldown = 0;
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.Drill)
            _gameplayControler.AttackDrill(_opponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1);
        SetNextCooldownTick();
    }

    private void SetNextCooldownTick()
    {
        if (Constants.CurrentOpponentCooldown > CurrentOpponent.Cooldown)
        {
            _opponentOnCooldown = false;
            _nextCooldownTick = Time.time + 3600;
            _gameplayControler.AttackIncoming = true;
        }
        else
        {
            _nextCooldownTick = Time.time + 1.0f;
        }
    }

    public override bool OpponentAttack()
    {
        bool spawnAfterAttack = true;
        CameraBhv.Bump(2);
        _opponentInstanceBhv.Attack();
        _characterInstanceBhv.TakeDamage();
        _gameplayControler.OpponentAttack(
            CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType,
            CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1,
            CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param2,
            CurrentOpponent.Realm,
            _opponentInstanceBhv.gameObject);
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece
            || CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.Shift)
            spawnAfterAttack = false;
        if (++Constants.CurrentOpponentAttackId >= CurrentOpponent.Attacks.Count)
            Constants.CurrentOpponentAttackId = 0;
        _opponentCooldownBar.UpdateContent(0, 1, Direction.Down);
        _opponentCooldownBar.ResetTilt();
        StartOpponentCooldown();
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
    }

    private void HandleForcedPiece()
    {
        if (CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].AttackType == AttackType.ForcedPiece && _gameplayControler.ForcedPiece == null)
        {
            _gameplayControler.AttackForcedPiece(_opponentInstanceBhv.gameObject, CurrentOpponent.Realm, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param1, CurrentOpponent.Attacks[Constants.CurrentOpponentAttackId].Param2);
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

    public override void DamageOpponent(int amount, GameObject source)
    {
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
        Instantiator.NewAttackLine(sourcePosition, _opponentInstanceBhv.gameObject.transform.position, realm);
        _opponentInstanceBhv.TakeDamage();
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
        Instantiator.PopText($"<material=\"{Character.Realm.ToString().ToLower()}.4.3\">{attackText}", _opponentHpBar.transform.position + new Vector3(1.0f, 1.6f, 0.0f));
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, CurrentOpponent.HpMax, Direction.Left);
        if (Constants.CurrentOpponentHp <= 0)
        {
            _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
            _gameplayControler.PlayFieldBhv.ShowSemiOpcaity(1);
            _soundControler.PlaySound(_idOpponentDeath);
            var minHeight = 9.0f;
            var highestBlockY = _gameplayControler.GetHighestBlock();
            if (minHeight < highestBlockY)
                minHeight = highestBlockY + 2;
            Instantiator.PopText(CurrentOpponent.Name.ToLower() + " defeated!", new Vector2(4.5f, minHeight));
            _opponentInstanceBhv.Die();
            _opponentOnCooldown = false;
            Constants.CurrentOpponentCooldown = 0;
            UpdateCooldownBar(Direction.Down);
        }
        else
        {
            if (_opponentOnCooldown && Constants.CurrentOpponentCooldown < CurrentOpponent.Cooldown)
            {
                if (CurrentOpponent.Immunity != Immunity.Cooldown)
                    Constants.CurrentOpponentCooldown -= Character.EnemyCooldownProgressionReducer;
                else
                {
                    _immunityInstance.Pop();
                    _soundControler.PlaySound(_idImmunity);
                }
                if (Constants.CurrentOpponentCooldown <= 0)
                    Constants.CurrentOpponentCooldown = 0;
                UpdateCooldownBar(Direction.Down);
            }
            SetNextCooldownTick();
        }
    }

    public void PlayHit()
    {
        _soundControler.PlaySound(_idHit);
    }

    public override void OnNewPiece(GameObject lastPiece)
    {
        if (_characterAttack > 0)
        {
            DamageOpponent(_characterAttack, lastPiece);
        }
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
        if (nbLines > 0)
        {
            incomingDamages = Character.GetAttack();
            if (nbLines == 1)
                incomingDamages += Character.SingleLineDamageBonus;
            if (Helper.RandomDice100(Character.CritChancePercent + Character.CumulativeCrit))
            {
                _cumulativeCrit += Character.CumulativeCrit;
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
                _gameplayControler.CheckForDarkRows(Character.RealmPassiveEffect);
                _gameplayControler.CheckForWasteRows(Character.RealmPassiveEffect);
                
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
                _gameplayControler.UpdateItemAndSpecialVisuals();
            }
        }
        _characterAttack += incomingDamages;
    }

    public override void OnCombo(int nbCombo, int nbLines)
    {
        base.OnCombo(nbCombo, nbLines);
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
