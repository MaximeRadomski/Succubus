using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicGameSceneBhv : GameSceneBhv
{
    private CharacterInstanceBhv _opponentInstanceBhv;
    private ResourceBarBhv _opponentHpBar;
    private ResourceBarBhv _opponentCooldownBar;
    private bool _opponentOnCooldown;
    private float _opponentCooldownStart;
    private int _opponentCurrentHp;
    private float _nextCooldownTick;
    private int _opponentAttackId;

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
        _opponentHpBar = GameObject.Find("OpponentHpBar").GetComponent<ResourceBarBhv>();
        _opponentCooldownBar = GameObject.Find("OpponentCooldownBar").GetComponent<ResourceBarBhv>();
        _opponentInstanceBhv = GameObject.Find(Constants.GoOpponentInstance).GetComponent<CharacterInstanceBhv>();
        _nextCooldownTick = Time.time - 1.0f;
        _opponentAttackId = 0;
        NextOpponent();
        _gameplayControler.GetComponent<GameplayControler>().StartGameplay(_currentOpponent.GravityLevel, Realm.Hell, Realm.Hell);
    }

    protected void NextOpponent()
    {
        _currentOpponent = _opponents[0];
        _opponentInstanceBhv.GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Opponents_" + _currentOpponent.Id);
        _opponentCurrentHp = _currentOpponent.HpMax;
        _opponentHpBar.UpdateContent(0, _currentOpponent.HpMax);
        _opponentHpBar.UpdateContent(_opponentCurrentHp, _currentOpponent.HpMax, Direction.Up);
        _opponentCooldownBar.UpdateContent(0, 1);
        StartOpponentCooldown();
    }

    private void StartOpponentCooldown()
    {
        _opponentOnCooldown = true;
        _opponentCooldownStart = Time.time;
        SetNextCooldownTick();
    }

    private void SetNextCooldownTick()
    {
        if (_nextCooldownTick >= _opponentCooldownStart + _currentOpponent.Cooldown)
        {
            _opponentOnCooldown = false;
            _nextCooldownTick = Time.time - 1.0f;
            Invoke(nameof(SetOpponentAttackReady), 1.0f);
        }
        else
            _nextCooldownTick = Time.time + 1.0f;
    }

    private void SetOpponentAttackReady()
    {
        _gameplayControler.AttackIncoming = true;
    }

    public override void OpponentAttack()
    {
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
            UpdateCooldownBar();
            SetNextCooldownTick();
        }
        if (_gameplayControler.AttackIncoming)
        {
            _opponentCooldownBar.Tilt();
        }
    }

    private void UpdateCooldownBar()
    {
        _opponentCooldownBar.UpdateContent((int)(_nextCooldownTick - _opponentCooldownStart), (int)_currentOpponent.Cooldown, Direction.Up);
    }

    override public void OnGameOver()
    {
        
    }

    public override void OnNewPiece()
    {
        if (_gameplayControler == null)
            return;
    }

    public override void OnPieceLocked(string pieceLetter)
    {
        base.OnPieceLocked(pieceLetter);
    }

    public override void OnLinesCleared(int nbLines, bool isB2B)
    {
        base.OnLinesCleared(nbLines, isB2B);
    }

    public override void OnCombo(int nbCombo)
    {
        base.OnCombo(nbCombo);
    }

    public override void OnPerfectClear()
    {
        base.OnPerfectClear();
    }
}
