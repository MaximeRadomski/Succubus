using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AccountDto
{
    public string Id_PlayerName;
    public string Key_Password;
    public string SecretQuestion;
    public string SecretAnswer;

    public AccountDto(string playerName, string password, string secretquestion, string secretanswer)
    {
        Id_PlayerName = playerName;
        Key_Password = password;
        SecretQuestion = secretquestion;
        SecretAnswer = secretanswer;
    }
}
