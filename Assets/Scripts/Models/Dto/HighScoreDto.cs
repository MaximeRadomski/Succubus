using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] // This makes the class able to be serialized into a JSON
public class HighScoreDto
{
    public string PlayerNameId;

    public int Score;

    public int CharacterId;

    public HighScoreDto(string playerNameId, int score, int characterId)
    {
        PlayerNameId = playerNameId;
        Score = score;
        CharacterId = characterId;
    }
}
