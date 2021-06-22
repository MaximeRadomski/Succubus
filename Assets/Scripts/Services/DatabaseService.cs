using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatabaseService
{
    private static fsSerializer serializer = new fsSerializer();
    private const string projectId = "infidhells"; // You can find this in your Firebase project settingsprivate
    static readonly string databaseURL = $"https://infidhells-default-rtdb.europe-west1.firebasedatabase.app/";//$"https://{projectId}.firebaseio.com/";

    public static void PostHighScore(HighScoreDto highScore, Func<bool, object> resultAction)
    {
        RestClient.Put<HighScoreDto>($"{databaseURL}highScores/{highScore.PlayerNameId}.json", highScore).Then(response =>
        {
            resultAction.Invoke(true);
        });
    }
    

    public static void GetHighScore(string playerNameId, Func<HighScoreDto, object> resultAction)
    {
        RestClient.Get($"{databaseURL}highScores/{playerNameId}.json").Then(returnValue =>
        {
            var highScore = JsonUtility.FromJson<HighScoreDto>(returnValue.Text);
            resultAction.Invoke(highScore);
        });
    }
    
    public static void GetHighScores(Func<List<HighScoreDto>, object> resultAction)
    {
        RestClient.Get($"{databaseURL}highScores.json").Then(response =>
        {
            var responseJson = response.Text; // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                                              // to serialize more complex types (a Dictionary, in this case)
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            serializer.TryDeserialize(data, typeof(Dictionary<string, HighScoreDto>), ref deserialized);
            var highScores = deserialized as Dictionary<string, HighScoreDto>;
            var listHighScores = new List<HighScoreDto>();
            foreach (var highScore in highScores)
            {
                listHighScores.Add(highScore.Value);
            }
            listHighScores.Sort((a, b) => b.Score.CompareTo(a.Score)); //a.Compare(b) -> Ascending    |   b.Compare(a) -> Descending
            resultAction.Invoke(listHighScores);
        });
    }
}
