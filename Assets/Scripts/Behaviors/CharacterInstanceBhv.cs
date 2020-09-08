using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstanceBhv : MonoBehaviour
{
    public System.Func<object> AfterDeath;

    private SceneBhv _sceneBhv;
    private int _isAttacking;
    private Vector3 _originalPosition;
    private Vector3 _endAttackPosition;
    private Direction _direction;
    private Vector3 _originalScale;
    private Vector3 _hitScale;
    private Vector3 _hitPosition;
    private Vector3 _spawnScale;
    private Vector3 _spawnPosition;
    private bool _isResetingHit;
    private bool _isSpawning;
    private bool _isDying;
    private SpriteRenderer _spriteRenderer;

    private bool _hasInit;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        if (_hasInit)
            return;
        _sceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
        _isAttacking = 0;
        _direction = transform.position.x > Camera.main.transform.position.x ? Direction.Right : Direction.Left;
        _originalPosition = transform.position;
        _endAttackPosition = _originalPosition + new Vector3(_direction == Direction.Left ? 2.0f : -2.0f, 0.0f, 0.0f);
        _originalScale = transform.localScale;
        _hitScale = new Vector3(0.8f, 1.15f, 1.0f);
        _hitPosition = new Vector3(0.0f, 0.5f, 0.0f);
        _spawnScale = new Vector3(0.5f, 1.5f, 1.0f);
        _spawnPosition = new Vector3(0.0f, 5.0f, 0.0f);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hasInit = true;
    }

    void Update()
    {
        if (_isAttacking > 0)
            DoAttack();
        else if (_isResetingHit)
        {
            ResetingHit();
        }
        if (_isDying)
        {
            Dying();
        }
        else if (_isSpawning)
        {
            Spawning();
        }
    }

    private void ResetingHit()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.2f);
        transform.position = Vector3.Lerp(transform.position, _originalPosition, 0.2f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
        {
            transform.localScale = _originalScale;
            transform.position = _originalPosition;
            _isResetingHit = false;
        }
    }

    private void Dying()
    {
        if (_spriteRenderer)
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorTransparent, 0.05f);
        if (_spriteRenderer != null && Helper.FloatEqualsPrecision(_spriteRenderer.color.a, Constants.ColorTransparent.a, 0.01f))
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                _spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
                _spriteRenderer.color = Constants.ColorTransparent;
            }
            _isDying = false;
            AfterDeath?.Invoke();
        }
    }

    private void Spawning()
    {
        if (_spriteRenderer)
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlain, 0.2f);
        if (_spriteRenderer != null && Helper.FloatEqualsPrecision(_spriteRenderer.color.a, Constants.ColorPlain.a, 0.01f))
            _spriteRenderer.color = Constants.ColorPlain;
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.2f);
        transform.position = Vector3.Lerp(transform.position, _originalPosition, 0.2f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
        {
            transform.localScale = _originalScale;
            transform.position = _originalPosition;
            _isSpawning = false;
        }
    }

    public void Spawn()
    {
        if (_hasInit == false)
            Init();
        _spriteRenderer.color = Constants.ColorPlainTransparent;
        transform.localScale = new Vector3(_spawnScale.x, _spawnScale.y, _spawnScale.z);
        transform.position = _originalPosition + _spawnPosition;
        _isSpawning = true;
    }

    public void Attack()
    {
        _isAttacking = 1;
    }

    private void DoAttack()
    {
        if (_isAttacking == 1)
            transform.position = Vector2.Lerp(transform.position, _endAttackPosition, 0.2f);
        else
            transform.position = Vector2.Lerp(transform.position, _originalPosition, 0.7f);
        if (_isAttacking == 1 && Vector2.Distance(_originalPosition, transform.position) > 1.5f)
        {
            _isAttacking = 2;
        }
        else if (_isAttacking == 2 && Helper.VectorEqualsPrecision(transform.position, _originalPosition, 0.01f))
        {
            _isAttacking = 0;
            transform.position = _originalPosition;
        }
    }

    public void TakeDamage()
    {
        transform.localScale = new Vector3(transform.localScale.x * _hitScale.x, _hitScale.y, _hitScale.z);
        transform.position = _originalPosition + _hitPosition;
        _isResetingHit = true;
    }

    public void Die()
    {
        _isDying = true;
    }
}
