using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreDto
{
    public string Id_PlayerName;
    public int Score;
    public string Key_Score;
    public int CharacterId;

    public HighScoreDto(string playerNameId, int score, int characterId)
    {
        Id_PlayerName = playerNameId;
        Score = score;
        Key_Score = EncryptedPlayerPrefs.Md5WithKey(score.ToString());
        CharacterId = characterId;
    }
}
