using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBoxBhv : FrameRateBehavior
{
    public List<Sprite> TitleBackgroundSprites;
    public List<Sprite> MainTopBotSprites;
    public List<Sprite> MainMidSprites;

    private SoundControlerBhv _soundControler;
    private SpriteRenderer _picture;
    private SpriteRenderer _titleBackground;
    private SpriteRenderer _mainTop;
    private SpriteRenderer _mainMid;
    private SpriteRenderer _mainBot;
    private TMPro.TextMeshPro _title;
    private TMPro.TextMeshPro _content;
    private ButtonBhv _previousSentence;
    private ButtonBhv _nextSentence;

    private DialogSubject _subject;
    private DialogSubject _secondary;
    private DialogSubject _currentSubject;
    private List<string> _sentences;
    private int _sentencesId;
    private string _dialogLibelle;
    private int _maxLinePixels = 120;
    
    private bool _isTalking;
    private string[] _talkingSplit;
    private int _talkingSplitId;
    private int _talkingCharId;
    private int _talkingFramesDelay = 2;
    private int _talkingFramesProgress;

    private System.Func<bool> _resultAction;

    public void Init(string subjectName, string secondaryName, System.Func<bool> resultAction)
    {
        _resultAction = resultAction;

        _picture = transform.Find("Picture").GetComponent<SpriteRenderer>();
        _titleBackground = transform.Find("TitleBackground").GetComponent<SpriteRenderer>();
        _mainTop = transform.Find("MainTop").GetComponent<SpriteRenderer>();
        _mainMid = transform.Find("MainMid").GetComponent<SpriteRenderer>();
        _mainBot = transform.Find("MainBot").GetComponent<SpriteRenderer>();
        _title = transform.Find("Title").GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find("Content").GetComponent<TMPro.TextMeshPro>();
        (_previousSentence = transform.Find("ButtonPrev").GetComponent<ButtonBhv>()).EndActionDelegate = PrevSentence;
        (_nextSentence = transform.Find("ButtonNext").GetComponent<ButtonBhv>()).EndActionDelegate = NextSentence;
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();

        _dialogLibelle = secondaryName == null ? subjectName : $"{subjectName}|{secondaryName}";
        _subject = GetSubjectFromName(subjectName);
        _secondary = GetSubjectFromName(secondaryName);
        var tmpSentences = DialogData.DialogTree[_dialogLibelle];
        if (tmpSentences == null)
            _sentences = new List<string>() { "[Error]: Missing content" };
        else
            _sentences = tmpSentences[PlayerPrefsHelper.GetDialogProgress(_dialogLibelle)];

        _sentencesId = 0;
        UpdateCurrentSentence();
    }

    private DialogSubject GetSubjectFromName(string name)
    {
        if (name == null)
            return null;
        DialogSubject tmpSubject = null;
        var tmpOpponentSubject = OpponentsData.HellOpponents.Find(o => o.Name == name);
        if (tmpOpponentSubject == null)
            tmpOpponentSubject = OpponentsData.EarthOpponents.Find(o => o.Name == name);
        if (tmpOpponentSubject == null)
            tmpOpponentSubject = OpponentsData.HeavenOpponents.Find(o => o.Name == name);
        if (tmpOpponentSubject != null)
            tmpSubject = new DialogSubject() {
                Id = tmpOpponentSubject.Id,
                Name = tmpOpponentSubject.Name,
                Realm = tmpOpponentSubject.Realm,
                Type = SubjectType.Opponent,
                DialogId = _soundControler.SetSound($"Dialog{tmpOpponentSubject.DialogId.ToString("00")}"),
                DialogPitch = tmpOpponentSubject.DialogPitch
            };
        else
        {
            var tmpCharacterSubject = CharactersData.Characters.Find(c => c.Name == name);
            if (tmpCharacterSubject != null)
                tmpSubject = new DialogSubject() { Id = tmpCharacterSubject.Id,
                    Name = tmpCharacterSubject.Name,
                    Realm = tmpCharacterSubject.Realm,
                    Type = SubjectType.Character,
                    DialogId = _soundControler.SetSound($"Dialog{tmpCharacterSubject.DialogId.ToString("00")}"),
                    DialogPitch = tmpCharacterSubject.DialogPitch
                };
        }
        return tmpSubject;
    }

    private void UpdateCurrentSentence()
    {
        _currentSubject = _sentencesId % 2 == 0 ? _subject : _secondary;

        if (_currentSubject.Type == SubjectType.Opponent)
        {
            _picture.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{_currentSubject.Realm}Opponents_{_currentSubject.Id}");
            _picture.transform.position = new Vector3(-Mathf.Abs(_picture.transform.position.x), _picture.transform.position.y, 0);
            _picture.flipX = true;
            _titleBackground.flipX = true;
            _title.transform.position = new Vector3(-Mathf.Abs(_title.transform.position.x), _title.transform.position.y, 0);
        }
        else
        {
            _picture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _currentSubject.Id);
            _picture.transform.position = new Vector3(Mathf.Abs(_picture.transform.position.x), _picture.transform.position.y, 0);
            _picture.flipX = false;
            _titleBackground.flipX = false;
            _title.transform.position = new Vector3(Mathf.Abs(_title.transform.position.x), _title.transform.position.y, 0);
        }
        _title.text = _currentSubject.Name;
        _content.text = string.Empty;
        _isTalking = true;
        _talkingSplit = _sentences[_sentencesId].ToLower().Split(' ');
        _talkingSplitId = 0;
        _talkingCharId = 0;
        _talkingFramesProgress = 0;

        _titleBackground.sprite = TitleBackgroundSprites[_currentSubject.Realm.GetHashCode()];
        _mainTop.sprite = MainTopBotSprites[_currentSubject.Realm.GetHashCode()];
        _mainMid.sprite = MainMidSprites[_currentSubject.Realm.GetHashCode()];
        _mainBot.sprite = _mainTop.sprite;

        if (_sentencesId <= 0)
            _previousSentence.DisableButton();
        else if (_previousSentence.Disabled)
            _previousSentence.EnableButton();
    }

    protected override void FrameUpdate()
    {
        if (_isTalking)
        {
            if (_talkingFramesProgress >= _talkingFramesDelay)
            {
                _talkingFramesProgress = 0;
                Talk();
            }
            else
                ++_talkingFramesProgress;
        }
    }

    private void Talk()
    {
        _content.text += _talkingSplit[_talkingSplitId][_talkingCharId];
        switch (_talkingSplit[_talkingSplitId][_talkingCharId])
        {
            case '.': _talkingFramesProgress = -30;
                break;
            case ',':
                _talkingFramesProgress = -15;
                break;
        }
        if (_talkingCharId % 3 == 0) //Sound every 3 letters or new word (CharId == 0)
            _soundControler.PlaySound(_currentSubject.DialogId, _currentSubject.DialogPitch);

        ++_talkingCharId;
        if (_talkingCharId >= _talkingSplit[_talkingSplitId].Length)
        {
            _talkingCharId = 0;
            ++_talkingSplitId;
            var linesSplit = _content.text.Split('\n');
            if (_talkingSplitId < _talkingSplit.Length && PixelsOnLine($"{linesSplit[linesSplit.Length - 1]} {_talkingSplit[_talkingSplitId]}") > (float)_maxLinePixels)
                _content.text += '\n';
            else
                _content.text += ' ';
        }
        if (_talkingSplitId >= _talkingSplit.Length)
            _isTalking = false;
    }

    private float PixelsOnLine(string line)
    {
        float nbPixels = 0.0f;
        foreach (char character in line)
        {
            if (character == '.'
                || character == ','
                || character == '\''
                || character == '!'
                || character == ':'
                || character == ';')
                nbPixels += 2.0f;
            else if (character == ' ')
                nbPixels += 1.2f;
            else if (character == '('
                || character == ')'
                || character == '['
                || character == ']'
                || character == '`'
                || character == '|')
                nbPixels += 3.0f;
            else
                nbPixels += 4.0f;
        }
        return nbPixels;
    }

    private void OverrideTalk()
    {
        _isTalking = false;
        _content.text = _sentences[_sentencesId].ToLower();
    }

    private void PrevSentence()
    {
        if (_sentencesId - 1 < 0)
            return;
        --_sentencesId;
        UpdateCurrentSentence();
    }

    private void NextSentence()
    {
        if (_isTalking)
        {
            OverrideTalk();
            return;
        }

        ++_sentencesId;
        if (_sentencesId < _sentences.Count)
            UpdateCurrentSentence();
        else
            ExitDialogBox();
    }

    private void ExitDialogBox()
    {
        Constants.DecreaseInputLayer();
        _resultAction?.Invoke();
        Destroy(gameObject);
    }

    private class DialogSubject
    {
        public int Id;
        public string Name;
        public SubjectType Type;
        public Realm Realm;
        public int DialogId;
        public float DialogPitch;
    }

    private enum SubjectType
    {
        Opponent,
        Character
    }
}
