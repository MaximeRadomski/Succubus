using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsCameraBhv : MonoBehaviour
{
    private GameObject _panelGame;
    private GameObject _textureQuad;
    private Material _textureMaterial;
    private bool _hasInit;
    private float _quadOriginalScale;
    private Character _character;

    private Vector3 _intoxicatedPosition;
    private Vector2 _intoxicatedScale;

    private bool _isIntoxicated;

    private void Start()
    {
        if (!_hasInit)
            Init();
    }

    private void Init()
    {
        _panelGame = GameObject.Find("PanelGame");
        _textureQuad = transform.GetChild(0).gameObject;
        _textureMaterial = _textureQuad.GetComponent<Renderer>().material;
        _quadOriginalScale = _textureQuad.transform.localScale.x;
        _hasInit = true;
        _character = GameObject.Find(Constants.GoSceneBhvName).GetComponent<GameSceneBhv>().Character;
    }

    public void Reset()
    {
        if (!_hasInit)
            Init();

        transform.position = new Vector3(30.0f, 30.0f, transform.position.z);
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        _textureQuad.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        _textureQuad.transform.localScale = new Vector3(_quadOriginalScale, _quadOriginalScale, 1.0f);
        _textureMaterial.shader = Shader.Find("Sprites/Default");

        _isIntoxicated = false;

        gameObject.SetActive(false);
    }

    public void SetAttack(AttackType attackType, int param, int cooldown)
    {
        if (attackType == AttackType.MirrorMirror)
            SetMirrorMirror(param, cooldown);
        else if (attackType == AttackType.Intoxication)
            SetIntoxication(cooldown);
    }

    private void SetMirrorMirror(int axis, int cooldown)
    {
        transform.position = new Vector3(_panelGame.transform.position.x, _panelGame.transform.position.y, transform.position.z);
        float xScale = axis == 0 || axis == 2 ? -1.0f : 1.0f;
        float yScale = axis == 1 || axis == 2 ? -1.0f : 1.0f;
        transform.localScale = new Vector3(xScale, yScale, 1.0f);
        Constants.CurrentItemCooldown -= (int)(_character.ItemCooldownReducer * cooldown);
    }

    private void SetIntoxication(int cooldown)
    {
        _textureMaterial.shader = Shader.Find("FX/Flare");
        transform.position = new Vector3(_panelGame.transform.position.x, _panelGame.transform.position.y, transform.position.z);
        _intoxicatedPosition = _textureQuad.transform.localPosition;
        _intoxicatedScale = new Vector3(_quadOriginalScale, _quadOriginalScale, 1.0f);
        _isIntoxicated = true;
        Constants.CurrentItemCooldown -= (int)(_character.ItemCooldownReducer * (cooldown / 3));
    }

    private void Update()
    {
        if (_isIntoxicated)
            Intoxicated();
    }

    private void Intoxicated()
    {
        _textureQuad.transform.localPosition = Vector3.Lerp(_textureQuad.transform.localPosition, _intoxicatedPosition, 0.03f);
        _textureQuad.transform.localScale = Vector3.Lerp(_textureQuad.transform.localScale, _intoxicatedScale, 0.03f);
        if (Helper.VectorEqualsPrecision(_textureQuad.transform.localPosition, _intoxicatedPosition, 0.1f))
            _intoxicatedPosition = new Vector3(Random.Range(-2.0f, 2.0f), Random.Range(-2.0f, 2.0f), 0.0f);
        if (Helper.VectorEqualsPrecision(_textureQuad.transform.localScale, _intoxicatedScale, 0.1f))
            _intoxicatedScale = new Vector3(Random.Range(_quadOriginalScale - 2.0f, _quadOriginalScale + 2.0f), Random.Range(_quadOriginalScale - 2.0f, _quadOriginalScale + 2.0f), 1.0f);
    }
}
