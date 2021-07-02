using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatabaseService
{
    public static fsSerializer Serializer = new fsSerializer();

    private static readonly string DatabaseURL = $"https://{Application.productName}/";
    private static readonly string SystemTable = "System";
    private static readonly string LogsTable = "Logs";
    private static readonly string BlackList = "BlackList";

    public static string SetTableAndId(string tableName, string id)
    {
        return $"{DatabaseURL}{tableName}/{id}";
    }

    public static string SetTable(string tableName)
    {
        return $"{DatabaseURL}{tableName}";
    }

    public static void GetLastUpdatedVersion(Action<string> thenAction)
    {
        RestClient.Get(SetTableAndId(SystemTable, "LastUpdatedVersion")).Then(returnValue =>
        {
            string version = null;
            if (!string.IsNullOrEmpty(returnValue.Text) && returnValue.Text != "null")
                version = returnValue.Text.Replace("\"", "");
            thenAction.Invoke(version);
        });
    }

    public static void SendErrorBody(string id, object content)
    {
        id = id.Replace(" ", "").Replace("/", "-");
        RestClient.Put<object>(SetTableAndId(LogsTable, $"Error_{id}"), content).Then(r =>
        {
            var name = "";
            if (content is AccountDto account)
                name = account.PlayerName;
            else if (content is HighScoreDto highScore)
                name = highScore.PlayerName;
            RestClient.Put<object>(SetTableAndId(BlackList, name), content).Then(r =>
            {

            });
        });
    }
}
