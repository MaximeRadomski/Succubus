using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogBoxBhv : FrameRateBehavior
{
    public List<Sprite> TitleBackgroundSprites;
    public List<Sprite> MainTopBotSprites;
    public List<Sprite> MainMidSprites;

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
    private List<string> _sentences;
    private int _sentencesId;
    private string _dialogLibelle;

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
            tmpSubject = new DialogSubject() { Id = tmpOpponentSubject.Id, Name = tmpOpponentSubject.Name, Realm = tmpOpponentSubject.Realm, Type = SubjectType.Opponent };
        else
        {
            var tmpCharacterSubject = CharactersData.Characters.Find(c => c.Name == name);
            if (tmpCharacterSubject != null)
                tmpSubject = new DialogSubject() { Id = tmpCharacterSubject.Id, Name = tmpCharacterSubject.Name, Realm = tmpCharacterSubject.Realm, Type = SubjectType.Character };
        }
        return tmpSubject;
    }

    private void UpdateCurrentSentence()
    {
        var currentSubject = _sentencesId % 2 == 0 ? _subject : _secondary;

        if (currentSubject.Type == SubjectType.Opponent)
        {
            _picture.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/{currentSubject.Realm}Opponents_{currentSubject.Id}");
            _picture.transform.position = new Vector3(-Mathf.Abs(_picture.transform.position.x), _picture.transform.position.y, 0);
            _picture.flipX = true;
            _titleBackground.flipX = true;
            _title.transform.position = new Vector3(-Mathf.Abs(_title.transform.position.x), _title.transform.position.y, 0);
        }
        else
        {
            _picture.sprite = Helper.GetSpriteFromSpriteSheet("Sprites/Characters_" + currentSubject.Id);
            _picture.transform.position = new Vector3(Mathf.Abs(_picture.transform.position.x), _picture.transform.position.y, 0);
            _picture.flipX = false;
            _titleBackground.flipX = false;
            _title.transform.position = new Vector3(Mathf.Abs(_title.transform.position.x), _title.transform.position.y, 0);
        }
        _title.text = currentSubject.Name;
        _content.text = _sentences[_sentencesId].ToLower();

        _titleBackground.sprite = TitleBackgroundSprites[currentSubject.Realm.GetHashCode()];
        _mainTop.sprite = MainTopBotSprites[currentSubject.Realm.GetHashCode()];
        _mainMid.sprite = MainMidSprites[currentSubject.Realm.GetHashCode()];
        _mainBot.sprite = _mainTop.sprite;

        if (_sentencesId <= 0)
            _previousSentence.DisableButton();
        else if (_previousSentence.Disabled)
            _previousSentence.EnableButton();
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
    }

    private enum SubjectType
    {
        Opponent,
        Character
    }
}
