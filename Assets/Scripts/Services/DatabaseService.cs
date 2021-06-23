using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatabaseService
{
    public static fsSerializer Serializer = new fsSerializer();

    private static readonly string ProjectId = "infidhells";
    private static readonly string DatabaseURL = $"https://{ProjectId}-default-rtdb.europe-west1.firebasedatabase.app/";

    public static string SetTableAndId(string tableName, string id)
    {
        return $"{DatabaseURL}{tableName}/{id}.json";
    }

    public static string SetTable(string tableName)
    {
        return $"{DatabaseURL}{tableName}.json";
    }
}
