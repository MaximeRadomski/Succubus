using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassicGameSceneBhv : GameSceneBhv
{
    private CharacterInstanceBhv _opponentInstanceBhv;
    private TMPro.TextMeshPro _weaknessText;
    private ResourceBarBhv _opponentHpBar;
    private ResourceBarBhv _opponentCooldownBar;
    private bool _opponentOnCooldown;
    //private float _opponentCooldownStart;
    //private float _opponentCooldownEnd;
    private float _nextCooldownTick;
    private int _opponentAttackId;

    private int _characterAttack;

    private SoundControlerBhv _soundControler;
    private int _characterVoice;

    private int _opponentVoice;

    void Start()
    {
        Init();
        _opponents = PlayerPrefsHelper.GetCurrentOpponents();
        if (_opponents.Count == 1)
            GameObject.Find("Enemies").GetComponent<TMPro.TextMeshPro>().text = "enemy";
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
        _weaknessText = GameObject.Find("Weakness").GetComponent<TMPro.TextMeshPro>();
        _opponentHpBar = GameObject.Find("OpponentHpBar").GetComponent<ResourceBarBhv>();
        _opponentCooldownBar = GameObject.Find("OpponentCooldownBar").GetComponent<ResourceBarBhv>();
        _opponentInstanceBhv = GameObject.Find(Constants.GoOpponentInstance).GetComponent<CharacterInstanceBhv>();
        _opponentInstanceBhv.AfterDeath = AfterOpponentDeath;
        _nextCooldownTick = Time.time - 1.0f;
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        NextOpponent();
        _characterVoice = _soundControler.SetSound("CharVoice" + Character.Id.ToString("00"));
        _gameplayControler.GetComponent<GameplayControler>().StartGameplay(_currentOpponent.GravityLevel, Realm.Hell, Realm.Hell);
    }

    protected void NextOpponent()
    {
        if (Constants.CurrentListOpponentsId >= _opponents.Count)
        {
            NavigationService.LoadPreviousScene();
            return;
        }
        _currentOpponent = _opponents[Constants.CurrentListOpponentsId];
        _opponentAttackId = 0;
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/" + _currentOpponent.Realm + "Opponents_" + _currentOpponent.Id);
        Constants.CurrentOpponentHp = Constants.CurrentOpponentHp <= 0 ? _currentOpponent.HpMax : Constants.CurrentOpponentHp;
        _weaknessText.text = _currentOpponent.Weakness != Weakness.None ? _currentOpponent.Weakness.GetDescription().ToLower() : "";
        if (_currentOpponent.Weakness == Weakness.xLines)
            _weaknessText.text = _currentOpponent.XLineWeakness + _weaknessText.text;
        _opponentHpBar.UpdateContent(0, _currentOpponent.HpMax);
        _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, _currentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.UpdateContent(0, 1);
        _gameplayControler.SetGravity(_currentOpponent.GravityLevel);
        _opponentVoice = _soundControler.SetSound("OppoVoice" + _currentOpponent.Realm.ToString() + _currentOpponent.Id.ToString("00"));
        StartOpponentCooldown();
    }

    private void StartOpponentCooldown()
    {
        _opponentOnCooldown = true;
        Constants.CurrentOpponentCooldown = 0;
        SetNextCooldownTick();
    }

    private void SetNextCooldownTick()
    {
        if (Constants.CurrentOpponentCooldown >= _currentOpponent.Cooldown)
        {
            _opponentOnCooldown = false;
            _nextCooldownTick = Time.time - 1.0f;
            Invoke(nameof(SetOpponentAttackReady), 1.0f);
        }
        else
        {
            _nextCooldownTick = Time.time + 1.0f;
        }
    }

    private void SetOpponentAttackReady()
    {
        _gameplayControler.AttackIncoming = true;
    }

    public override void OpponentAttack()
    {
        _opponentInstanceBhv.Attack();
        _characterInstanceBhv.TakeDamage();
        if (_opponentAttackId >= _currentOpponent.Attacks.Count)
            _opponentAttackId = 0;
        _gameplayControler.OpponentAttack(
            _currentOpponent.Attacks[_opponentAttackId].AttackType,
            _currentOpponent.Attacks[_opponentAttackId].NbAttackRows,
            _currentOpponent.Attacks[_opponentAttackId].AttackParam,
            _currentOpponent.Realm);
        ++_opponentAttackId;
        _gameplayControler.AttackIncoming = false;
        _opponentCooldownBar.UpdateContent(0, 1, Direction.Down);
        _opponentCooldownBar.ResetTilt();
        StartOpponentCooldown();
    }

    void Update()
    {
        if (_opponentOnCooldown && Time.time >= _nextCooldownTick)
        {
            ++Constants.CurrentOpponentCooldown;
            UpdateCooldownBar(Direction.Up);
            SetNextCooldownTick();
        }
        if (_gameplayControler.AttackIncoming)
        {
            _opponentCooldownBar.Tilt();
        }
    }

    private void UpdateCooldownBar(Direction direction)
    {
        _opponentCooldownBar.UpdateContent(Constants.CurrentOpponentCooldown, _currentOpponent.Cooldown, direction);
    }

    override public void OnGameOver()
    {
        base.OnGameOver();
        NavigationService.LoadPreviousScene();
        _characterInstanceBhv.Die();
    }

    public override void OnNewPiece()
    {
        if (_characterAttack > 0)
        {
            _opponentInstanceBhv.TakeDamage();
            Constants.CurrentOpponentHp -= _characterAttack;
            Instantiator.PopText("-" + _characterAttack, _opponentHpBar.transform.position + new Vector3(1.0f, 1.6f, 0.0f));
            _opponentHpBar.UpdateContent(Constants.CurrentOpponentHp, _currentOpponent.HpMax, Direction.Left);
            if (Constants.CurrentOpponentHp <= 0)
            {
                _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = true;
                _opponentInstanceBhv.Die();
                _opponentOnCooldown = false;
                Constants.CurrentOpponentCooldown = 0;
                UpdateCooldownBar(Direction.Down);
            }
            else
            {
                if (_opponentOnCooldown && Constants.CurrentOpponentCooldown < _currentOpponent.Cooldown)
                {
                    Constants.CurrentOpponentCooldown -= 1 + Character.EnemyCooldownProgressionReducer;
                    if (Constants.CurrentOpponentCooldown <= 0)
                        Constants.CurrentOpponentCooldown = 0;
                    UpdateCooldownBar(Direction.Down);
                }                
                SetNextCooldownTick();
            }
        }
        _characterAttack = 0;
    }

    private object AfterOpponentDeath()
    {
        GameObject.Find("Opponent" + Constants.CurrentListOpponentsId).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/OpponentsIcons_" + ((_opponents[Constants.CurrentListOpponentsId].Realm.GetHashCode() * 2) + 1));
        ++Constants.CurrentListOpponentsId;
        NextOpponent();
        _gameplayControler.CurrentPiece.GetComponent<Piece>().IsLocked = false;
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().color = Constants.ColorPlain;
        return true;
    }

    public override void OnPieceLocked(string pieceLetter)
    {
        base.OnPieceLocked(pieceLetter);
        if (string.IsNullOrEmpty(pieceLetter))
            return;
        var incomingDamages = 0;
        if (_currentOpponent.Weakness == Weakness.Twists)
            incomingDamages += _currentOpponent.DamagesOnWeakness;
        if (_currentOpponent.Immunity == Immunity.Twists)
            incomingDamages = 0;
        _characterAttack += incomingDamages;
    }

    public override void OnLinesCleared(int nbLines, bool isB2B)
    {
        base.OnLinesCleared(nbLines, isB2B);
        var incomingDamages = 0;
        if (nbLines > 0)
        {
            incomingDamages = Character.Attack;
            if (Helper.IsSuperiorByRealm(Character.Realm, _currentOpponent.Realm))
                incomingDamages = (int)(incomingDamages * Helper.MultiplierFromPercent(1.0f, Character.DamagePercentToInferiorRealm));
            else if (Helper.IsSuperiorByRealm(_currentOpponent.Realm, Character.Realm))
                incomingDamages -= (int)(_characterAttack * Helper.MultiplierFromPercent(1.0f, -Character.DamagePercentToInferiorRealm));
            incomingDamages *= nbLines;
            if (_currentOpponent.Weakness == Weakness.xLines && _currentOpponent.XLineWeakness == nbLines)
                incomingDamages += _currentOpponent.DamagesOnWeakness;
            if (_currentOpponent.Immunity == Immunity.xLines && _currentOpponent.XLineImmunity == nbLines)
                incomingDamages = 0;
            if (Character.Realm == Realm.Earth && nbLines == 4)
                _gameplayControler.CheckForGarbageRows(Character.RealmPassiveEffect);
        }
        if (isB2B)
        {
            if (_currentOpponent.Weakness == Weakness.Consecutive)
                incomingDamages += _currentOpponent.DamagesOnWeakness;
            if (_currentOpponent.Immunity == Immunity.Consecutive)
                incomingDamages = 0;
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
            incomingDamages += (int)((Character.Attack * Helper.MultiplierFromPercent(0.0f, 10 * Character.RealmPassiveEffect) + (nbCombo - 2)) * nbLines);
        if (_currentOpponent.Weakness == Weakness.Combos)
            incomingDamages += _currentOpponent.DamagesOnWeakness * (nbCombo - 1);
        if (_currentOpponent.Immunity == Immunity.Combos)
            incomingDamages = 0;
        _characterAttack += incomingDamages;
    }

    public override void OnPerfectClear()
    {
        base.OnPerfectClear();
    }
}
