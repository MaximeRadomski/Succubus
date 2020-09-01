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
    private bool _isResetingHit;
    private bool _isDying;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        _sceneBhv = GameObject.Find(Constants.GoSceneBhvName).GetComponent<SceneBhv>();
        _isAttacking = 0;
        _direction = transform.position.x > Camera.main.transform.position.x ? Direction.Right : Direction.Left;
        _originalPosition = transform.position;
        _endAttackPosition = _originalPosition + new Vector3(_direction == Direction.Left ? 2.0f : -2.0f, 0.0f, 0.0f);
        _originalScale = transform.localScale;
        _hitScale = new Vector3(0.8f, 1.15f, 1.0f);
        _hitPosition = new Vector3(0.0f, 0.5f, 0.0f);
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (_isAttacking > 0)
            DoAttack();
        else if (_isResetingHit)
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
        if (_isDying)
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
