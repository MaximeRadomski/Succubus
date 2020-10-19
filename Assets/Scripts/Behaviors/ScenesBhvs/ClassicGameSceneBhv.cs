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
    private int _opponentAttackId;
    private IconInstanceBhv _weaknessInstance;
    private IconInstanceBhv _immunityInstance;

    private Run _run;
    private StepsService _stepsService;
    private Step _currentStep;

    private int _characterAttack;
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
        _currentStep = _stepsService.GetStepOnPos(_run.X, _run.Y, _run.Steps);
        if (Constants.CurrentGameMode == GameMode.TrainingDummy
            || Constants.CurrentGameMode == GameMode.TrainingFree)
        {
            _opponents = PlayerPrefsHelper.GetCurrentOpponents(null);
            Constants.ResetCurrentItemCooldown(Character, ItemsData.GetItemFromName(ItemsData.CommonItemsNames[2]));
        }
        else
        {
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
        GameObject.Find("InfoRealm").GetComponent<TMPro.TextMeshPro>().text = $"{Constants.MaterialHell_3_2B}realm:\n{ Constants.MaterialHell_4_3B}{ _run.CurrentRealm.ToString().ToLower()}\nlvl {_run.RealmLevel}";
        NextOpponent(sceneInit:true);
        _gameplayControler.GetComponent<GameplayControler>().StartGameplay(_currentOpponent.GravityLevel, Character.Realm, _run.CurrentRealm);
    }

    protected void NextOpponent(bool sceneInit = false)
    {
        if (Constants.CurrentListOpponentsId >= _opponents.Count)
        {
            Victory();
            return;
        }
        _currentOpponent = _opponents[Constants.CurrentListOpponentsId];
        _opponentInstanceBhv.Spawn();
        CameraBhv.Bump(4);
        _soundControler.PlaySound(_idOpponentAppearance);
        Instantiator.PopText(_currentOpponent.Kind.ToLower() + " appears!", new Vector2(4.5f, 15.0f), floatingTime:3.0f);
        _opponentAttackId = 0;
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/" + _run.CurrentRealm + "Opponents_" + _currentOpponent.Id);
        _opponentType.sprite = _currentOpponent.Type == OpponentType.Common ? null : Helper.GetSpriteFromSpriteSheet("Sprites/OpponentTypes_" + ((_currentOpponent.Realm.GetHashCode() * 3) + (_currentOpponent.Type.GetHashCode() - 1)));
        Constants.CurrentOpponentHp = Constants.CurrentOpponentHp <= 0 ? _currentOpponent.HpMax : Constants.CurrentOpponentHp;
        _weaknessInstance.SetVisible(_currentOpponent.Weakness != Weakness.None);
        _weaknessInstance.SetSkin(Helper.GetSpriteFromSpriteSheet("Sprites/WeaknessImmunity_" + (_currentOpponent.Realm.GetHashCode() * 2)));
        _immunityInstance.SetVisible(_currentOpponent.Immunity != Immunity.None);
        _immunityInstance.SetSkin(Helper.GetSpriteFromSpriteSheet("Sprites/WeaknessImmunity_" + (_currentOpponent.Realm.GetHashCode() * 2 + 1)));
        _opponentHpBar.UpdateContent(0, _currentOpponent.HpMax);
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, _currentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.UpdateContent(0, 1);
        _gameplayControler.SetGravity(_currentOpponent.GravityLevel + (_run.RealmLevel - 1));
        StartOpponentCooldown(sceneInit);
    }

    private void Victory()
    {
        Paused = true;
        _isVictorious = true;
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
        _gameplayControler.CleanPlayerPrefs();

        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
        {
            LoadBackAfterVictory(false);
            return;
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

    }

    private object LoadBackAfterVictory(bool result)
    {
        Constants.CurrentMusicType = MusicType.Menu;
        if (Constants.CurrentGameMode == GameMode.TrainingFree
            || Constants.CurrentGameMode == GameMode.TrainingDummy)
            NavigationService.LoadBackUntil(Constants.CharSelScene);
        else
            NavigationService.LoadBackUntil(Constants.StepsScene);
        return result;
    }

    private void StartOpponentCooldown(bool sceneInit = false)
    {
        _opponentOnCooldown = true;
        if (!sceneInit)
            Constants.CurrentOpponentCooldown = 0;
        if (_currentOpponent.Attacks[_opponentAttackId].AttackType == AttackType.Drill)
            _gameplayControler.AttackDrill(_opponentInstanceBhv.gameObject, _currentOpponent.Realm, _currentOpponent.Attacks[_opponentAttackId].Param1);
        SetNextCooldownTick();
    }

    private void SetNextCooldownTick()
    {
        if (Constants.CurrentOpponentCooldown > _currentOpponent.Cooldown)
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
            _currentOpponent.Attacks[_opponentAttackId].AttackType,
            _currentOpponent.Attacks[_opponentAttackId].Param1,
            _currentOpponent.Attacks[_opponentAttackId].Param2,
            _currentOpponent.Realm,
            _opponentInstanceBhv.gameObject);
        if (_currentOpponent.Attacks[_opponentAttackId].AttackType == AttackType.ForcedPiece)
            spawnAfterAttack = false;
        if (++_opponentAttackId >= _currentOpponent.Attacks.Count)
            _opponentAttackId = 0;
        _opponentCooldownBar.UpdateContent(0, 1, Direction.Down);
        _opponentCooldownBar.ResetTilt();
        StartOpponentCooldown();
        return spawnAfterAttack;
    }

    void Update()
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
        if (_currentOpponent.Attacks[_opponentAttackId].AttackType == AttackType.ForcedPiece && _gameplayControler.ForcedPiece == null)
        {
            _gameplayControler.AttackForcedPiece(_opponentInstanceBhv.gameObject, _currentOpponent.Realm, _currentOpponent.Attacks[_opponentAttackId].Param1, _currentOpponent.Attacks[_opponentAttackId].Param2);
            _gameplayControler.SetForcedPieceOpacity((float)Constants.CurrentOpponentCooldown, (float)_currentOpponent.Cooldown);
        }
        if (_opponentOnCooldown && Time.time >= _nextCooldownTick)
        {
            ++Constants.CurrentOpponentCooldown;
            _gameplayControler.SetForcedPieceOpacity((float)Constants.CurrentOpponentCooldown, (float)_currentOpponent.Cooldown);
            UpdateCooldownBar(Direction.Up);
            SetNextCooldownTick();
        }
    }

    private void UpdateCooldownBar(Direction direction)
    {
        _opponentCooldownBar.UpdateContent(Constants.CurrentOpponentCooldown, _currentOpponent.Cooldown, direction);
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
            PlayerPrefsHelper.ResetRun();
            NavigationService.LoadBackUntil(Constants.MainMenuScene);
        }
    }

    public override void DamageOpponent(int amount, GameObject source)
    {
        var realm = Character.Realm;
        var sourcePosition = source.transform.position;
        var piece = source.GetComponent<Piece>();
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
            _soundControler.PlaySound(_idHit);
        else
        {
            attackText += "!";
            _soundControler.PlaySound(_idCrit);
        }
        VibrationService.Vibrate();
        Instantiator.PopText(attackText, _opponentHpBar.transform.position + new Vector3(1.0f, 1.6f, 0.0f), !_isCrit ? ((Color)Constants.GetColorFromRealm(Character.Realm, 4)).ToHex() : Color.white.ToHex());
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, _currentOpponent.HpMax, Direction.Left);
        if (Constants.CurrentOpponentHp <= 0)
        {
            _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
            _gameplayControler.PlayFieldBhv.ShowSemiOpcaity(1);
            _soundControler.PlaySound(_idOpponentDeath);
            Instantiator.PopText(_currentOpponent.Kind.ToLower() + " defeated!", new Vector2(4.5f, 15.0f));
            _opponentInstanceBhv.Die();
            _opponentOnCooldown = false;
            Constants.CurrentOpponentCooldown = 0;
            UpdateCooldownBar(Direction.Down);
        }
        else
        {
            if (_opponentOnCooldown && Constants.CurrentOpponentCooldown < _currentOpponent.Cooldown)
            {
                if (_currentOpponent.Immunity != Immunity.Cooldown)
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
        if (_currentOpponent.Attacks[_opponentAttackId].AttackType == AttackType.ForcedPiece)
            Destroy(_gameplayControler.ForcedPiece);
        else if (_currentOpponent.Attacks[_opponentAttackId].AttackType == AttackType.Drill)
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
        if (_currentOpponent.Weakness == Weakness.Twists)
        {
            _weaknessInstance.Pop();
            _soundControler.PlaySound(_idWeakness);
            incomingDamages += _currentOpponent.DamagesOnWeakness;
        }
        if (_currentOpponent.Immunity == Immunity.Twists)
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
            if (Helper.RandomDice100(Character.CritChancePercent))
            {
                incomingDamages += (int)(Character.GetAttack() * Helper.MultiplierFromPercent(0.0f, Character.CritMultiplier));
                _isCrit = true;
            }
            if (Helper.IsSuperiorByRealm(Character.Realm, _currentOpponent.Realm))
                incomingDamages = (int)(incomingDamages * Helper.MultiplierFromPercent(1.0f, Character.DamagePercentToInferiorRealm));
            incomingDamages *= nbLines;
            if (_currentOpponent.Weakness == Weakness.xLines && _currentOpponent.XLineWeakness == nbLines)
            {
                _weaknessInstance.Pop();
                _soundControler.PlaySound(_idWeakness);
                incomingDamages += _currentOpponent.DamagesOnWeakness;
            }
            if (_currentOpponent.Immunity == Immunity.xLines && _currentOpponent.XLineImmunity == nbLines)
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
            if (_currentOpponent.Weakness == Weakness.Consecutive)
            {
                _weaknessInstance.Pop();
                _soundControler.PlaySound(_idWeakness);
                incomingDamages += _currentOpponent.DamagesOnWeakness;
            }
            if (_currentOpponent.Immunity == Immunity.Consecutive)
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
        if (_currentOpponent.Weakness == Weakness.Combos)
        {
            _weaknessInstance.Pop();
            _soundControler.PlaySound(_idWeakness);
            incomingDamages += _currentOpponent.DamagesOnWeakness * (nbCombo - 1);
        }
        if (_currentOpponent.Immunity == Immunity.Combos)
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
        if (_isVictorious)
            return;
        _stepsService.ClearLootOnPos(_run.X, _run.Y, _run);
        _stepsService.SetVisionOnRandomStep(_run);
        _stepsService.GenerateAdjacentSteps(_run, Character, _currentStep);
    }
}
