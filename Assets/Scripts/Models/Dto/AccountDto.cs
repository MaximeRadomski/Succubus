using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccountDto : Dto
{
    //Checksum
    public string PlayerName;
    public string Password;
    public string SecretQuestion;
    public string SecretAnswer;
    public string CreationDate;

    public AccountDto(string playerName, string password, string secretquestion, string secretanswer, int type, string checkSum)
    {
        PlayerName = playerName;
        Password = password;
        SecretQuestion = secretquestion;
        SecretAnswer = secretanswer;

        Type = type;
        Checksum = checkSum;

        CreationDate = DateTime.UtcNow.ToString();
    }
}
