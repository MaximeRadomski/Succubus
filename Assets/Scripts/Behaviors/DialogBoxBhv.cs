using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DialogBoxBhv : FrameRateBehavior
{
    public List<Sprite> TitleBackgroundSprites;
    public List<Sprite> MainTopBotSprites;
    public List<Sprite> MainMidSprites;
    public List<Sprite> Emojies;

    private SoundControlerBhv _soundControler;
    private InputControlerBhv _inputControlerBhv;
    private SpriteRenderer _picture;
    private SpriteRenderer _emoji;
    private SpriteRenderer _titleBackground;
    private SpriteRenderer _mainTop;
    private SpriteRenderer _mainMid;
    private SpriteRenderer _mainBot;
    private TMPro.TextMeshPro _title;
    private TMPro.TextMeshPro _content;
    private ButtonBhv _previousSentence;
    private ButtonBhv _nextSentence;
    private Transform _pelliculeTop;
    private Transform _pelliculeBot;
    private MusicControlerBhv _musicControler;

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
    private int _talkingFramesDelay = 1;
    private int _talkingFramesProgress;
    private int _pelliculeMove;

    private System.Func<bool> _resultAction;

    public void Init(Vector3 position, string subjectName, string secondaryName, System.Func<bool> resultAction, int? customid = null)
    {
        _resultAction = resultAction;

        transform.position = new Vector3(position.x, position.y, 0.0f);
        _picture = transform.Find("Picture").GetComponent<SpriteRenderer>();
        _emoji = transform.Find("Emoji").GetComponent<SpriteRenderer>();
        _titleBackground = transform.Find("TitleBackground").GetComponent<SpriteRenderer>();
        _mainTop = transform.Find("MainTop").GetComponent<SpriteRenderer>();
        _mainMid = transform.Find("MainMid").GetComponent<SpriteRenderer>();
        _mainBot = transform.Find("MainBot").GetComponent<SpriteRenderer>();
        _title = transform.Find("Title").GetComponent<TMPro.TextMeshPro>();
        _content = transform.Find("Content").GetComponent<TMPro.TextMeshPro>();
        (_previousSentence = transform.Find("ButtonPrev").GetComponent<ButtonBhv>()).EndActionDelegate = PrevSentence;
        (_nextSentence = transform.Find("ButtonNext").GetComponent<ButtonBhv>()).EndActionDelegate = NextSentence;
        if (SceneManager.GetActiveScene().name == Constants.ClassicGameScene && PlayerPrefsHelper.GetOrientation() == Direction.Horizontal)
        {
            _previousSentence.transform.position += new Vector3(-10.0f, 4.0f, 0.0f);
            _nextSentence.transform.position += new Vector3(10.0f, 4.0f, 0.0f);
        }
        _pelliculeTop = transform.Find("PelliculeTop");
        _pelliculeBot = transform.Find("PelliculeBot");
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _inputControlerBhv = GameObject.Find(Constants.GoInputControler).GetComponent<InputControlerBhv>();
        _musicControler = GameObject.Find(Constants.GoMusicControler)?.GetComponent<MusicControlerBhv>();

        _dialogLibelle = secondaryName == null ? subjectName : $"{subjectName}|{secondaryName}";
        _subject = GetSubjectFromName(subjectName);
        _secondary = GetSubjectFromName(secondaryName);
        List<List<string>> tmpSentences = null;
        if (!DialogsData.DialogTree.TryGetValue(_dialogLibelle, out tmpSentences))
        {
            _dialogLibelle = $"{subjectName}|Any";
            if (!DialogsData.DialogTree.TryGetValue(_dialogLibelle, out tmpSentences))
                _sentences = new List<string>() { "[Error]: Missing content... Go throw some rocks at at the dev!" };
        }
        if (tmpSentences != null)
        {
            var id = customid == null ? PlayerPrefsHelper.GetDialogProgress(_dialogLibelle) : customid.Value;
            if (id >= tmpSentences.Count)
                id = tmpSentences.Count - 1;
            else if (customid == null)
                PlayerPrefsHelper.SaveDialogProgress(_dialogLibelle, id + 1);
            _sentences = tmpSentences[id];
        }

        _musicControler.Play(Constants.DialogAudioClip);
        _sentencesId = 0;
        _pelliculeMove = 0;
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
                Region = tmpOpponentSubject.Region,
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
                    Region = tmpCharacterSubject.Realm,
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

        if (_currentSubject == null)
            ExitDialogBox();

        if (_currentSubject.Type == SubjectType.Opponent)
        {
            _picture.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{_currentSubject.Region}Opponents_{_currentSubject.Id}");
            _picture.transform.localPosition = new Vector3(-Mathf.Abs(_picture.transform.localPosition.x), _picture.transform.localPosition.y, 0);
            _picture.flipX = true;
            _emoji.transform.localPosition = new Vector3(-Mathf.Abs(_emoji.transform.localPosition.x), _emoji.transform.localPosition.y, 0);
            _titleBackground.flipX = true;
            _title.transform.localPosition = new Vector3(-Mathf.Abs(_title.transform.localPosition.x), _title.transform.localPosition.y, 0);
        }
        else
        {
            _picture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + _currentSubject.Id);
            _picture.transform.localPosition = new Vector3(Mathf.Abs(_picture.transform.localPosition.x), _picture.transform.localPosition.y, 0);
            _picture.flipX = false;
            _emoji.transform.localPosition = new Vector3(Mathf.Abs(_emoji.transform.localPosition.x), _emoji.transform.localPosition.y, 0);
            _titleBackground.flipX = false;
            _title.transform.localPosition = new Vector3(Mathf.Abs(_title.transform.localPosition.x), _title.transform.localPosition.y, 0);
        }
        _title.text = $"<material=\"Long{_currentSubject.Realm}.4.3\">{_currentSubject.Name}";
        _content.text = $"<material=\"{_currentSubject.Realm.ToString().ToLower()}.4.3\">";
        _isTalking = true;
        _talkingSplit = _sentences[_sentencesId].ToLower().Split(' ');
        _talkingSplitId = 0;
        _talkingCharId = 0;
        _talkingFramesProgress = 0;

        _titleBackground.sprite = TitleBackgroundSprites[_currentSubject.Realm.GetHashCode()];
        _mainTop.sprite = MainTopBotSprites[_currentSubject.Realm.GetHashCode()];
        _mainMid.sprite = MainMidSprites[_currentSubject.Realm.GetHashCode()];
        _mainBot.sprite = _mainTop.sprite;

        if (_sentences[_sentencesId].Contains("??")) _emoji.sprite = Emojies[0];
        else if (_sentences[_sentencesId].Contains("!!")) _emoji.sprite = Emojies[1];
        else if (_sentences[_sentencesId].Contains("...")) _emoji.sprite = Emojies[2];
        else _emoji.sprite = null;

        if (_emoji.sprite != null)
        {
            _emoji.color = (Color)Constants.GetColorFromRealm(_currentSubject.Realm, 4);
            _picture.GetComponent<IconInstanceBhv>().Pop(2.3f, 2.5f);
        }

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
        if (_pelliculeMove >= 10)
        {
            _pelliculeTop.transform.localPosition = new Vector3(0.0f, _pelliculeTop.transform.localPosition.y, 0.0f);
            _pelliculeBot.transform.localPosition = new Vector3(0.0f, _pelliculeBot.transform.localPosition.y, 0.0f);
            _pelliculeMove = 0;
        }
        else
        {
            _pelliculeTop.transform.localPosition = new Vector3(_pelliculeTop.transform.localPosition.x + (2 * Constants.Pixel), _pelliculeTop.transform.localPosition.y, 0.0f);
            _pelliculeBot.transform.localPosition = new Vector3(_pelliculeBot.transform.localPosition.x - (2 * Constants.Pixel), _pelliculeBot.transform.localPosition.y, 0.0f);
            ++_pelliculeMove;
        }
    }

    private void Talk()
    {
        _content.text += _talkingSplit[_talkingSplitId][_talkingCharId];
        switch (_talkingSplit[_talkingSplitId][_talkingCharId])
        {
            case '.': case '!': case '?': case ':': case ';': case ']':
                if (_talkingCharId == _talkingSplit[_talkingSplitId].Length - 1)
                    _talkingFramesProgress = -30;
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
            var linesSplit = _content.text.Substring(_content.text.IndexOf('>') + 1).Split('\n');
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
                nbPixels += 1.35f;
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
        _content.text = $"<material=\"{_currentSubject.Realm.ToString().ToLower()}.4.3\">{_sentences[_sentencesId].ToLower()}";
    }

    public void PrevSentence()
    {
        if (_sentencesId - 1 < 0)
            return;
        --_sentencesId;
        UpdateCurrentSentence();
        _inputControlerBhv.InitMenuKeyboardInputs(_previousSentence.transform.position + new Vector3(0.0f, 10.0f, 0.0f));
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
        {
            UpdateCurrentSentence();
            _inputControlerBhv.InitMenuKeyboardInputs(_nextSentence.transform.position + new Vector3(0.0f, 10.0f, 0.0f));
        }
        else
            ExitDialogBox();
    }

    private void ExitDialogBox()
    {
        _musicControler.ResetSceneLoadedMusic(manualReset : true);
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
        public Realm Region;
        public int DialogId;
        public float DialogPitch;
    }

    private enum SubjectType
    {
        Opponent,
        Character
    }
}
