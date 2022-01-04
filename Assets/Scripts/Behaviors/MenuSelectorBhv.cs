using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectorBhv : FrameRateBehavior
{
    private GameObject _targetGameObject;
    private SoundControlerBhv _soundControler;
    private CameraBhv _mainCameraBhv;

    private bool _moving;
    private bool _instantMove;
    private bool _reseted;
    private bool _clickIng;
    private bool _resetingScaleAndOpacity;
    private Vector3 _targetOffset;
    private float _targetHalfWidth;
    private float _targetHalfHeight;
    private Vector3 _originalScale = new Vector3(1.0f, 1.0f, 1.0f);
    private Vector3 _clickScale = new Vector3(0.8f, 0.8f, 1.0f);

    private int _followingFrames;
    private int _maxFollowingFrames = 10;

    private int _moveId;


    void Start()
    {
        Init();
    }

    private void Init()
    {
#if UNITY_ANDROID
        return;
#else
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        GetSoundControler();
#endif
    }

    private void GetSoundControler()
    {
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        if (_soundControler != null)
            _moveId = _soundControler.SetSound("LeftRightDown");
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Reset();
    }

    public void Reset(Vector3? resetPosition = null)
    {
        if (_reseted)
            return;
        if (_mainCameraBhv == null ||_mainCameraBhv.gameObject == null)
            _mainCameraBhv = Helper.GetMainCamera().GetComponent<CameraBhv>();
        if (resetPosition != null)
            this.transform.position = resetPosition.Value;
        else
            this.transform.position = new Vector3(-10, 30.0f, 0.0f) + _mainCameraBhv.transform.position;
        foreach (Transform child in transform)
        {
            child.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            var spriteRenderer = child.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Constants.ColorPlainTransparent;
        }
        _reseted = true;
        _resetingScaleAndOpacity = true;
    }

    override protected void FrameUpdate()
    {
        if (_clickIng)
            Clicking();
        else if (_moving)
            Moving();
        else if (_resetingScaleAndOpacity)
            ResetingScaleAndOpacity();
    }

    public void MoveTo(GameObject target, bool soundMuted = false)
    {
        if (target == null)
            return;
        if (_soundControler == null)
            GetSoundControler();
        if (_soundControler != null && !soundMuted)
            _soundControler.PlaySound(_moveId);
        var boxCollider = target.GetComponent<BoxCollider2D>();
        _targetGameObject = target;
        _targetOffset = new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0.0f);
        _targetHalfWidth = (boxCollider.size.x / 2) + boxCollider.edgeRadius;
        _targetHalfHeight = (boxCollider.size.y / 2) + boxCollider.edgeRadius;
        _moving = true;
        _followingFrames = 0;
        if (_reseted == true)
            _instantMove = true;
        _reseted = false;
    }

    private void Moving()
    {
        if (_targetGameObject == null)
        {
            _moving = false;
            ResetingScaleAndOpacity();
            return;
        }
        if (!_instantMove)
            transform.position = Vector3.Lerp(transform.position, _targetGameObject.transform.position + _targetOffset, 0.5f);
        else
        {
            transform.position = _targetGameObject.transform.position + _targetOffset;
            foreach (Transform child in transform)
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.color = Constants.ColorPlain;
            }
        }
        foreach (Transform child in transform)
        {
            float localX;
            float localY;
            if (child.name.Contains("Top"))
                localY = _targetHalfHeight;
            else
                localY = -_targetHalfHeight;
            if (child.name.Contains("Right"))
                localX = _targetHalfWidth;
            else
                localX = -_targetHalfWidth;
            if (!_instantMove)
                child.transform.localPosition = Vector3.Lerp(child.transform.localPosition, new Vector3(localX, localY, 0.0f), 0.5f);
            else
                child.transform.localPosition = new Vector3(localX, localY, 0.0f);
        }
        if (_followingFrames > _maxFollowingFrames && Helper.VectorEqualsPrecision(transform.position, _targetGameObject.transform.position + _targetOffset, 0.01f))
        {
            transform.position = _targetGameObject.transform.position + _targetOffset;
            _moving = false;
            _instantMove = false;
        }
        ++_followingFrames;
    }

    public void Click(GameObject selectedGameObject = null)
    {
        if (selectedGameObject != null)
            MoveTo(selectedGameObject, soundMuted: true);
        _clickIng = true;
    }

    private void Clicking()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _clickScale, 0.5f);
        foreach (Transform child in transform)
        {
            var spriteRenderer = child.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Constants.ColorPlainTransparent, 0.5f);
        }
        if (Helper.FloatEqualsPrecision(transform.localScale.x, _clickScale.x, 0.02f))
        {
            transform.localScale = _clickScale;
            foreach (Transform child in transform)
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.color = Constants.ColorPlainTransparent;
            }
            _clickIng = false;
            _resetingScaleAndOpacity = true;
        }
    }

    private void ResetingScaleAndOpacity()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, _originalScale, 0.25f);
        foreach (Transform child in transform)
        {
            var spriteRenderer = child.GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Constants.ColorPlain, 0.25f);
        }
        if (Helper.FloatEqualsPrecision(transform.localScale.x, _originalScale.x, 0.02f))
        {
            transform.localScale = _originalScale;
            foreach (Transform child in transform)
            {
                var spriteRenderer = child.GetComponent<SpriteRenderer>();
                spriteRenderer.color = Constants.ColorPlain;
            }
            _resetingScaleAndOpacity = false;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
