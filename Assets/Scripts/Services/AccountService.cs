using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AccountService
{
    private static readonly string TableAccounts = "Accounts";

    public static void PutAccount(AccountDto Account, Action onResolved)
    {
        RestClient.Put<AccountDto>(DatabaseService.SetTableAndId(TableAccounts, Account.PlayerNameId), Account).Then(r =>
        {
            onResolved.Invoke();
        });
    }

    public static void GetAccount(string playerNameId, Action<AccountDto> resultAction)
    {
        RestClient.Get(DatabaseService.SetTableAndId(TableAccounts, playerNameId)).Then(returnValue =>
        {
            var Account = JsonUtility.FromJson<AccountDto>(returnValue.Text);
            resultAction.Invoke(Account);
        });
    }
}
