using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroneBhv : MonoBehaviour
{
    private GameplayControler _gameplayControler;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _targetSpriteRenderer;
    private int _nbAttacks;
    private AttackType _attackType;
    private int _nbRows;
    private Realm _realm;

    public void Init(GameplayControler gameplayControler, Realm realm, int nbRows, AttackType rowType)
    {
        _attackType = rowType;
        _gameplayControler = gameplayControler;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Drone_" + realm.GetHashCode());
        _targetSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        _nbAttacks = 0;
        _nbRows = nbRows;
        _realm = realm;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        if (_gameplayControler == null)
            GetGameplayControler();
        if (_gameplayControler == null || _gameplayControler.CurrentPiece == null)
            return;
        if (IsTargeted(_gameplayControler.CurrentPiece))
            _targetSpriteRenderer.enabled = true;
        else
            _targetSpriteRenderer.enabled = false;
    }

    private bool IsTargeted(GameObject piece)
    {
        var rangeX = piece.GetComponent<Piece>().GetRangeX();
        //return Vector2.Distance(new Vector2(piece.transform.GetChild(0).position.x - 0.5f + piece.GetComponent<Piece>().XFromSpawn, 0.0f), new Vector2(transform.position.x, 0.0f)) < 1.5f;
        var x = Mathf.RoundToInt(transform.position.x);
        return x >= rangeX[0] && x <= rangeX[1];
    }

    public void OnPieceLocked(GameObject lockedCurrentPiece)
    {
        if (IsTargeted(lockedCurrentPiece))
        {
            if (_gameplayControler == null)
                GetGameplayControler();
            _gameplayControler.AfterSpawn = null;
            ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).PlayHit();
            Destroy(gameObject);
        }
        UpdateY();
    }

    public void UpdateY()
    {
        int highestOnX = _gameplayControler.GetHighestBlockOnX(Mathf.RoundToInt(transform.position.x));
        if (highestOnX >= transform.position.y)
            transform.position = new Vector3(transform.position.x, highestOnX + 1, 0.0f);
    }

    private void GetGameplayControler()
    {
        _gameplayControler = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameplayControler>();
    }

    public bool DroneAttackAfterSpawn(bool trueSpawn)
    {
        if (_nbAttacks == 0 || !trueSpawn)
        {
            ++_nbAttacks;
            return false;
        }
        if (_attackType == AttackType.WasteRow)
            _gameplayControler.AttackWasteRows(gameObject, _nbRows, _realm, 1);
        else if (_attackType == AttackType.LightRow)
            _gameplayControler.AttackLightRows(gameObject, _nbRows, _realm, 10);
        else if (_attackType == AttackType.EmptyRow)
            _gameplayControler.AttackEmptyRows(gameObject, _nbRows, _realm);
        else
            _gameplayControler.AttackDarkRows(gameObject, _nbRows, _realm);
        UpdateY();
        _gameplayControler.UpdateItemAndSpecialVisuals();
        return true;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (_gameplayControler == null)
            GetGameplayControler();
        if (_gameplayControler == null)
            return;
        _nbAttacks = 0;
        _gameplayControler.AfterSpawn = DroneAttackAfterSpawn;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
