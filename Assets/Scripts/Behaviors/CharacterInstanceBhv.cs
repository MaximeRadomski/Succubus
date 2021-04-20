using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInstanceBhv : FrameRateBehavior
{
    public System.Func<object> AfterDeath;
    public Vector3 OriginalPosition;

    private SceneBhv _sceneBhv;
    private SpecialParticleBhv _specialParticlesBhv;
    private BoostParticlesBhv _boostParticlesBhv;
    private Direction _direction;
    private Vector3 _idlePositionToReach;
    private Vector3 _originalScale;
    private Vector3 _attackPosition;
    private Vector3 _attackScale;
    private Vector3 _hitScale;
    private Vector3 _dodgeScale;
    private Vector3 _hitPosition;
    private Vector3 _dodgePosition;
    private Vector3 _spawnScale;
    private Vector3 _spawnPosition;
    private bool _attacking;
    private bool _resetAttacking;
    private bool _resetingTransform;
    private bool _isSpawning;
    private bool _isDying;
    private bool _dieOnReset;
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
        if (transform.childCount > 0)
        {
            _specialParticlesBhv = transform.GetChild(0).GetComponent<SpecialParticleBhv>();
            _boostParticlesBhv = transform.GetChild(1).GetComponent<BoostParticlesBhv>();
        }
        _direction = transform.position.x > Helper.GetMainCamera().transform.position.x ? Direction.Right : Direction.Left;
        OriginalPosition = transform.position;
        _originalScale = transform.localScale;
        _attackPosition = OriginalPosition + new Vector3(_direction == Direction.Left ? 2.0f : -2.0f, 0.0f, 0.0f);
        _attackScale = new Vector3(1.3f, 0.75f, 1.0f);
        _hitScale = new Vector3(0.7f, 1.3f, 1.0f);
        _dodgeScale = new Vector3(1.3f, 0.7f, 1.0f);
        _hitPosition = new Vector3(0.0f, 0.5f, 0.0f);
        _dodgePosition = new Vector3(0.0f, -0.5f, 0.0f);
        _spawnScale = new Vector3(0.5f, 1.5f, 1.0f);
        _spawnPosition = new Vector3(0.0f, 5.0f, 0.0f);
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _hasInit = true;
    }

    protected override void FrameUpdate()
    {
        if (_isSpawning)
            Spawning();
        else if (_isDying)
            Dying();
        else if (_attacking)
            Attacking();
        else if (_resetAttacking)
            ResetAttacking();
        else if (_resetingTransform)
            ResetingTransform();
        else
            Idle();
    }

    private void Idle()
    {
        transform.position = Vector2.Lerp(transform.position, _idlePositionToReach + OriginalPosition, 0.025f);
        if (Helper.VectorEqualsPrecision(transform.position, _idlePositionToReach + OriginalPosition, 0.01f))
            _idlePositionToReach = new Vector2(Random.Range(-2 * Constants.Pixel, 2 * Constants.Pixel), Random.Range(-2 * Constants.Pixel, 2 * Constants.Pixel));
    }

    private void ResetingTransform()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.2f);
        transform.position = Vector3.Lerp(transform.position, OriginalPosition, 0.2f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
        {
            transform.localScale = _originalScale;
            transform.position = OriginalPosition;
            _resetingTransform = false;
            if (_dieOnReset)
                Destroy(gameObject);
        }
    }

    private void Dying()
    {
        if (_spriteRenderer)
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorBlackTransparent, 0.05f);
        if (_spriteRenderer != null && Helper.FloatEqualsPrecision(_spriteRenderer.color.a, Constants.ColorBlackTransparent.a, 0.01f))
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                var spriteRenderer = transform.GetChild(i).GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                    spriteRenderer.color = Constants.ColorBlackTransparent;
            }
            _isDying = false;
            Constants.InputLocked = false;
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
        transform.position = Vector3.Lerp(transform.position, OriginalPosition, 0.2f);
        if (Helper.VectorEqualsPrecision(transform.localScale, _originalScale, 0.01f))
        {
            transform.localScale = _originalScale;
            transform.position = OriginalPosition;
            _isSpawning = false;
        }
    }

    public void Spawn()
    {
        if (_hasInit == false)
            Init();
        _spriteRenderer.color = Constants.ColorPlainTransparent;
        transform.localScale = new Vector3(_spawnScale.x, _spawnScale.y, _spawnScale.z);
        transform.position = OriginalPosition + _spawnPosition;
        _isSpawning = true;
    }

    public void Attack()
    {
        _attacking = true;
    }

    private void Attacking()
    {
        transform.position = Vector2.Lerp(transform.position, _attackPosition, 0.2f);
        transform.localScale = Vector3.Lerp(transform.localScale, _attackScale, 0.3f);
        if (Vector2.Distance(OriginalPosition, transform.position) > 1.5f)
        {
            _attacking = false;
            _resetAttacking = true;
        }
    }

    private void ResetAttacking()
    {
        transform.position = Vector2.Lerp(transform.position, OriginalPosition, 0.7f);
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.8f);
        if (Helper.VectorEqualsPrecision(transform.position, OriginalPosition, 0.01f))
        {
            _resetAttacking = false;
            transform.position = OriginalPosition;
            transform.localScale = _originalScale;
        }
    }

    public void TakeDamage()
    {
        transform.localScale = new Vector3(transform.localScale.x * _hitScale.x, _hitScale.y, _hitScale.z);
        transform.position = OriginalPosition + _hitPosition;
        _resetingTransform = true;
    }

    public void Dodge()
    {
        transform.localScale = new Vector3(transform.localScale.x * _dodgeScale.x, transform.localScale.y * _dodgeScale.y, _dodgeScale.z);
        transform.position = OriginalPosition + _dodgePosition;
        _resetingTransform = true;
    }

    public void GetOS()
    {
        transform.localScale = new Vector3(transform.localScale.x * _hitScale.x * 2, _hitScale.y * 2, _hitScale.z);
        transform.position = OriginalPosition + _hitPosition * 2;
        _dieOnReset = true;
        _resetingTransform = true;
    }

    public void Special(Realm realm)
    {
        _specialParticlesBhv?.Activate(realm);
        var specialMult = 2.0f;
        transform.localScale = new Vector3(transform.localScale.x * (_hitScale.x * specialMult), transform.localScale.y * (_hitScale.y * specialMult), _hitScale.z);
        transform.position = OriginalPosition + _hitPosition * 2;
        _resetingTransform = true;
    }

    public void Die()
    {
        _isDying = true;
        Constants.InputLocked = true;
    }

    public void Boost(Realm realm, float duration)
    {
        _boostParticlesBhv.Boost(realm, duration);
        var boostMult = 2.0f;
        transform.localScale = new Vector3(transform.localScale.x * (_hitScale.x * boostMult), transform.localScale.y * (_hitScale.y * boostMult), _hitScale.z);
        transform.position = OriginalPosition + _hitPosition * 2;
        _resetingTransform = true;
    }

    public void Malus(Realm realm, float duration)
    {
        _boostParticlesBhv.Malus(realm, duration);
        var malusMult = 0.5f;
        transform.localScale = new Vector3(transform.localScale.x * (_hitScale.x * malusMult), transform.localScale.y * (_hitScale.y * malusMult), _hitScale.z);
        transform.position = OriginalPosition + _hitPosition * 2;
        _resetingTransform = true;
    }
}
