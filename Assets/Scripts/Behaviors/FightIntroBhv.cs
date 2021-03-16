using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightIntroBhv : FrameRateBehavior
{
    private SpriteRenderer _characterPicture;
    private List<SpriteRenderer> _opponentsPictures;
    private float _characterXTarget;
    private List<float> _opponentsXTarget;

    private Vector3 _opponentOffset = new Vector3(2.0f, 4.0f, 0.0f);
    private float _startXOffset = 20;
    private float _lerpSpeed = 0.3f;
    private bool _animated;
    private int _opponentsIteration;

    private System.Func<bool> _resultAction;

    public void Init(Character character, List<Opponent> opponents, System.Func<bool> resultAction)
    {
        _resultAction = resultAction;

        _characterPicture = transform.Find("CharacterPicture").GetComponent<SpriteRenderer>();
        _characterPicture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + character.Id);
        _characterXTarget = _characterPicture.transform.position.x;
        _characterPicture.transform.position += new Vector3(_startXOffset, 0.0f, 0.0f);

        for (int i = 0; i < opponents.Count; ++i)
        {
            if (i == 0)
            {
                _opponentsPictures = new List<SpriteRenderer>();
                _opponentsXTarget = new List<float>();
                _opponentsPictures.Add(transform.Find("OpponentPicture").GetComponent<SpriteRenderer>());
                _opponentsPictures[0].sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{opponents[0].Region}Opponents_{opponents[0].Id}");
                _opponentsPictures[0].color = Constants.ColorPlainTransparent;
                _opponentsXTarget.Add(_opponentsPictures[0].transform.position.x);
                _opponentsPictures[0].transform.position += new Vector3(-_startXOffset, 0.0f, 0.0f);
                continue;
            }
            _opponentsPictures.Add(Instantiate(_opponentsPictures[0].gameObject, _opponentsPictures[0].transform.position, _opponentsPictures[0].transform.rotation).GetComponent<SpriteRenderer>());
            _opponentsPictures[i].gameObject.transform.SetParent(transform);
            _opponentsPictures[i].sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{opponents[i].Region}Opponents_{opponents[i].Id}");
            _opponentsPictures[i].color = Constants.ColorPlainTransparent;
            _opponentsPictures[i].sortingOrder = _opponentsPictures[0].sortingOrder - i;
            _opponentsPictures[i].transform.position = _opponentsPictures[i - 1].transform.position + _opponentOffset;
            _opponentsPictures[i].transform.localScale = _opponentsPictures[i].transform.localScale * (1.0f - (0.05f * i));
            _opponentsXTarget.Add(_opponentsPictures[i].transform.position.x + _startXOffset);
        }

        var regionColor = (Color)Constants.GetColorFromRealm(opponents[0].Region, 3);
        transform.Find("VS").GetComponent<SpriteRenderer>().color = regionColor;
        transform.Find("LeftLineShadow").GetComponent<SpriteRenderer>().color = regionColor;
        transform.Find("RightLineShadow").GetComponent<SpriteRenderer>().color = regionColor;

        _opponentsIteration = 0;
        _animated = true;
    }

    protected override void FrameUpdate()
    {
        if (_animated)
        {
            _characterPicture.transform.position = Vector3.Lerp(_characterPicture.transform.position, new Vector3(_characterXTarget, _characterPicture.transform.position.y, 0.0f), _lerpSpeed);
            _characterPicture.color = Color.Lerp(_characterPicture.color, Constants.ColorPlain, _lerpSpeed * 1.5f);

            for (int i = 0; i < _opponentsPictures.Count; ++i)
            {
                if (i * 5 > _opponentsIteration)
                    break;
                _opponentsPictures[i].transform.position = Vector3.Lerp(_opponentsPictures[i].transform.position, new Vector3(_opponentsXTarget[i], _opponentsPictures[i].transform.position.y, 0.0f), _lerpSpeed);
                _opponentsPictures[i].color = Color.Lerp(_opponentsPictures[i].color, Constants.ColorPlain, _lerpSpeed * 1.5f);
            }

            if (Helper.FloatEqualsPrecision(_opponentsPictures[_opponentsPictures.Count -1].transform.position.x, _opponentsXTarget[_opponentsXTarget.Count - 1], 0.01f))
            {
                Invoke(nameof(AfterIntro), 1.0f);
                _animated = false;
            }
            ++_opponentsIteration;
        }
    }

    private void AfterIntro()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke();
        Destroy(gameObject);
    }
}
