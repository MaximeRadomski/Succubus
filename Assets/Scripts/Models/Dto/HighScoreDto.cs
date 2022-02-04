using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HighScoreDto : Dto
{
    public string PlayerName;
    //Checksum
    public int Score;
    public int Level;
    public int Lines;
    public int Pieces;
    public int CharacterId;
    public string CreationDate;
    public bool Falsified;

    public HighScoreDto(string playerNameId, int score, int level, int lines, int pieces, int characterId, int type, string checkSum)
    {
        PlayerName = playerNameId;
        Score = score;
        Level = level;
        Lines = lines;
        Pieces = pieces;
        CharacterId = characterId;

        Type = type;
        Checksum = checkSum;

        CreationDate = Helper.DateFormat(DateTime.Now);
    }
}
