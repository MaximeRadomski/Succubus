using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreDto
{
    public string Id_PlayerName;
    public string Score;
    public string Level;
    public string Lines;
    public string Pieces;
    public string CharacterId;
    public string Key_Score;
    public string CreationDate;

    public HighScoreDto(string playerNameId, string score, string level, string lines, string pieces, string characterId, string encryptedScore)
    {
        Id_PlayerName = playerNameId;
        Score = score;
        Level = level;
        Lines = lines;
        Pieces = pieces;
        CharacterId = characterId;
        Key_Score = encryptedScore;

        CreationDate = DateTime.UtcNow.ToString();
    }
}
