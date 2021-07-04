using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPartitionBhv : MonoBehaviour
{
    private float _xOffset = -2.727f;
    private float _yMaxNoteOffset = 0.9285f;
    private float _containerPixelWidth = 50.0f;

    private int _id;
    private int _airLines;

    private List<KeyBinding> _keys = new List<KeyBinding>() { KeyBinding.Left, KeyBinding.SoftDrop, KeyBinding.Right };
    private List<Note> _notes;

    private GameplayControler _gameplayControler;
    private SoundControlerBhv _soundControler;
    private int _idCombo;

    public void Init(Realm opponentRealm, int nbPieces, Instantiator instantiator, GameplayControler gameplayControler, int airLines)
    {
        _gameplayControler = gameplayControler;
        _airLines = airLines;
        _soundControler = GameObject.Find(Constants.TagSoundControler).GetComponent<SoundControlerBhv>();
        _idCombo = _soundControler.SetSound("Combo");
        GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Music_{opponentRealm.GetHashCode()}");
        transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Music_{3 + opponentRealm.GetHashCode()}");
        var segment = _containerPixelWidth / nbPieces;
        var halfSegment = segment / 2;
        _notes = new List<Note>();

        for (int i = 0; i < nbPieces; ++i)
        {
            var y = Random.Range(0.0f, 12.0f);
            var x = (i * segment * Constants.Pixel) + (halfSegment * Constants.Pixel);
            var note = instantiator.NewPartitionNote(new Vector3(_xOffset + x, _yMaxNoteOffset - (y * Constants.Pixel), 0.0f) + transform.position);
            note.transform.SetParent(transform);
            var keybinding = _keys[Random.Range(0, _keys.Count)];
            var spriteId = 0;
            if (keybinding == KeyBinding.SoftDrop)
                spriteId = 1;
            else if (keybinding == KeyBinding.Right)
                spriteId = 2;
            if (spriteId == 2)
            {
                note.GetComponent<SpriteRenderer>().flipX = true;
                spriteId = 0;
            }
            var spriteRenderer = note.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Music_{8 + spriteId + (opponentRealm.GetHashCode() * 6)}");

            _notes.Add(new Note(spriteRenderer, keybinding, spriteId, opponentRealm, 2.0f - (0.1f * y)));
        }
        _id = -1;
        NextNote();
    }

    public void NextNote(KeyBinding pressedNote = KeyBinding.None)
    {
        if (_id >= _notes.Count)
            return;
        if (pressedNote != KeyBinding.None && pressedNote != _notes[_id].KeyBinding)
        {
            _gameplayControler.CurrentPiece.transform.position += new Vector3(0.0f, 1.0f, 0.0f);
            _gameplayControler.AttackEmptyRows((_gameplayControler.SceneBhv as ClassicGameSceneBhv).OpponentInstanceBhv.gameObject, _airLines, (_gameplayControler.SceneBhv as ClassicGameSceneBhv).CurrentOpponent.Realm);
            _gameplayControler.DropGhost();
            return;
        }

        if (_id >= 0)
        {
            _notes[_id].SpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Music_{10 + _notes[_id].SpriteId + (_notes[_id].Realm.GetHashCode() * 6)}");
            _soundControler.PlaySound(_idCombo, _notes[_id].Tone);
        }

        ++_id;
        if (_id < _notes.Count)
            _notes[_id].SpriteRenderer.sprite = Helper.GetSpriteFromSpriteSheet($"Sprites/Music_{6 + _notes[_id].SpriteId + (_notes[_id].Realm.GetHashCode() * 6)}");
        if (_id >= _notes.Count)
        {
            ((ClassicGameSceneBhv)_gameplayControler.SceneBhv).ResetToOpponentGravity();
            StartCoroutine(Helper.ExecuteAfterDelay(0.15f, () =>
            {
                Constants.IsEffectAttackInProgress = AttackType.None;
                Destroy(gameObject);
                return true;
            }, lockInputWhile: false));
        }
    }

    public KeyBinding GetLastKeyBinding()
    {
        if (_id - 1 < 0)
            return KeyBinding.None;
        return _notes[_id - 1].KeyBinding;
    }

    class Note
    {
        public SpriteRenderer SpriteRenderer;
        public KeyBinding KeyBinding;
        public int SpriteId;
        public Realm Realm;
        public float Tone;

        public Note(SpriteRenderer spriteRenderer, KeyBinding keyBinding, int spriteId, Realm realm, float tone)
        {
            SpriteRenderer = spriteRenderer;
            KeyBinding = keyBinding;
            SpriteId = spriteId;
            Realm = realm;
            Tone = tone;
        }
    }
}
