using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSelectorBhv : MonoBehaviour
{
    private bool _moving;
    private bool _instantMove;
    private bool _clickIng;
    private bool _resetingScaleAndOpacity;
    private GameObject _targetGameObject;
    private Vector3 _targetOffset;
    private float _targetHalfWidth;
    private float _targetHalfHeight;
    private float _clickScale;
    private float _clickOpacity;


    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Reset();
    }

    public void Reset(Vector3? resetPosition = null)
    {
        if (resetPosition != null)
            this.transform.position = resetPosition.Value;
        else
            this.transform.position = new Vector3(-30, 30.0f, 0.0f);
        foreach (Transform child in transform)
            child.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }

    public void MoveTo(GameObject target)
    {
        var boxCollider = target.GetComponent<BoxCollider2D>();
        _targetGameObject = target;
        _targetOffset = new Vector3(boxCollider.offset.x, boxCollider.offset.y, 0.0f);
        _targetHalfWidth = boxCollider.size.x / 2;
        _targetHalfHeight = boxCollider.size.y / 2;
        _moving = true;
        if (Vector2.Distance(transform.position, _targetGameObject.transform.position + _targetOffset) > 20.0f)
            _instantMove = true;
    }

    private void Update()
    {
        if (_moving)
            Moving();
    }

    private void Moving()
    {
        if (!_instantMove)
            transform.position = Vector3.Lerp(transform.position, _targetGameObject.transform.position + _targetOffset, 0.5f);
        else
            transform.position = _targetGameObject.transform.position + _targetOffset;
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
        if (Helper.VectorEqualsPrecision(transform.position, _targetGameObject.transform.position + _targetOffset, 0.01f))
        {
            transform.position = _targetGameObject.transform.position + _targetOffset;
            _moving = false;
            _instantMove = false;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
