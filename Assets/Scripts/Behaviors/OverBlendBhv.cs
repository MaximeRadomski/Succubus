using UnityEngine;

public class OverBlendBhv : MonoBehaviour
{
    private GameObject _loading;
    private GameObject _loadingBorders;
    private SpriteRenderer _spriteRenderer;
    private TMPro.TextMeshPro _message;
    private System.Func<bool, object> _resultAction;

    private OverBlendType _overBlendType;
    private Vector3 _sourcePosition;
    private Vector3 _endPosition;
    private Vector3 _activePosition;
    private int _state; // 0:Start | 1:LoadingStart | 2: LoadingEnd | 3:End
    private float? _constantLoadingSpeed;
    private float _loadPercent;
    private float _halfSpriteSize;
    private bool _midActionDone;

    public void SetPrivates(OverBlendType overBlendType, string message, float? constantLoadingSpeed, System.Func<bool, object> resultAction, bool reverse)
    {
        Constants.InputLocked = true;
        DontDestroyOnLoad(gameObject);
        _overBlendType = overBlendType;
        _loadPercent = 0;
        if (reverse)
            _sourcePosition = new Vector3(-20.0f, 0.0f, 0.0f);
        else
            _sourcePosition = new Vector3(20.0f, 0.0f, 0.0f);
        _activePosition = new Vector3(0.0f, 0.0f, 0.0f);
        _endPosition = new Vector3(-_sourcePosition.x, 0.0f, 0.0f);
        _state = 0;
        _constantLoadingSpeed = constantLoadingSpeed;
        _resultAction = resultAction;
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Constants.ColorPlainTransparent;
        _loading = transform.Find("Loading").gameObject;
        _loadingBorders = transform.Find("LoadingBorders").gameObject;
        _halfSpriteSize = _loading.GetComponent<SpriteRenderer>().size.x / 2;
        if (constantLoadingSpeed != null)
            AddLoadingPercent(0.0f);
        else
        {
            _loading.GetComponent<SpriteRenderer>().enabled = false;
            _loadingBorders.GetComponent<SpriteRenderer>().enabled = false;
        }
        _message = transform.Find("Message").GetComponent<TMPro.TextMeshPro>();
        _message.text = message;
        _midActionDone = false;
        transform.position = _sourcePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == 0)
        {
            transform.position = Vector3.Lerp(transform.position, _activePosition, 0.25f);
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlain, 0.2f);
            if (Helper.FloatEqualsPrecision(transform.position.x, _activePosition.x, 0.5f))
            {
                transform.position = _activePosition;
                _spriteRenderer.color = Constants.ColorPlain;
                _state = 1;
                if (_overBlendType == OverBlendType.StartActionLoadingEnd)
                    _resultAction?.Invoke(true);
            }
        }
        else if (_state == 1)
        {
            AddLoadingPercent(_constantLoadingSpeed);
        }
        else if (_state == 2)
        {
            transform.position = Vector3.Lerp(transform.position, _endPosition, 0.25f);
            _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Constants.ColorPlainTransparent, 0.1f);
            if (Helper.FloatEqualsPrecision(transform.position.x, _endPosition.x, 0.01f))
            {
                transform.position = _endPosition;
                _spriteRenderer.color = Constants.ColorPlainTransparent;
                _state = 3;
            }
        }
        else if (_state == 3)
        {
            if (_overBlendType == OverBlendType.StartLoadingEndAction)
                _resultAction?.Invoke(true);
            ExitOverBlend();
        }
    }

    public void AddLoadingPercent(float? percentToAdd)
    {
        if (percentToAdd == null && _overBlendType == OverBlendType.StartLoadMidActionEnd)
        {
            _midActionDone = true;
            _resultAction?.Invoke(true);
            EndPercent();
            return;
        }
        _loadPercent += percentToAdd ?? 1.0f;
        _loading.transform.localScale = new Vector3(0.01f * _loadPercent, 1.0f, 1.0f);
        _loading.transform.position = new Vector3((_loading.transform.localScale.x * _halfSpriteSize) - _halfSpriteSize, _loading.transform.position.y, 0.0f);
        if (percentToAdd > 0.0f && (int)_loadPercent >= 100)
            EndPercent();
        else if (_overBlendType == OverBlendType.StartLoadMidActionEnd && !_midActionDone && _loadPercent >= 50.0f)
        {
            _midActionDone = true;
            _resultAction?.Invoke(true);
        }
    }

    public void EndPercent()
    {
        _state = 2;
        if (_overBlendType == OverBlendType.StartLoadingActionEnd)
            _resultAction?.Invoke(true);
    }

    public virtual void ExitOverBlend()
    {
        _state = -1;
        Constants.InputLocked = false;
        Destroy(gameObject);
    }
}
