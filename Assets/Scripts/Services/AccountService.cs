using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AccountService
{
    public static int MinPlayerNameCharacters = 3;
    public static int MinPasswordCharacters = 10;

    private static readonly string TableAccounts = "Accounts";

    public static void PutAccount(AccountDto Account, Action onResolved)
    {
        RestClient.Put<AccountDto>(DatabaseService.SetTableAndId(TableAccounts, Account.PlayerName), Account).Then(r =>
        {
            onResolved.Invoke();
        });
    }

    public static void GetAccount(string playerNameId, Action<AccountDto> resultAction)
    {
        RestClient.Get(DatabaseService.SetTableAndId(TableAccounts, playerNameId)).Then(returnValue =>
        {
            AccountDto account = null;
            if (!string.IsNullOrEmpty(returnValue.Text) && returnValue.Text != "null")
                account = JsonUtility.FromJson<AccountDto>(returnValue.Text);
            resultAction.Invoke(account);
        });
    }

    public static void CheckAccount(Instantiator instantiator, Action<AccountDto> thenAction)
    {
        instantiator.NewLoading();
        var lastSaved = PlayerPrefsHelper.GetLastSavedCredentials();
        if (lastSaved == null)
        {
            Helper.ResumeLoading();
            thenAction(null);
            return;
        }
        GetAccount(lastSaved.PlayerName, (account) =>
        {
            Helper.ResumeLoading();
            if (account == null || EncryptedPlayerPrefs.Md5WithKey(lastSaved.Password) != account.Password)
            {
                PlayerPrefsHelper.SaveLastSavedCredentials(null);
                instantiator.NewPopupYesNo("Error", "couldn't log you in with your saved credentials. please reconnect manually.", null, "Ok", (result) =>
                {
                    thenAction(null);
                    return false;
                });
            }
            else
                thenAction(lastSaved);
        });
    }
}
