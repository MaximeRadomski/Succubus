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
}
