using FullSerializer;
using Proyecto26;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighScoresService
{
    private static readonly string TableHighScores = "HighScores";

    public static void PutHighScore(HighScoreDto highScore, Action onResolved)
    {
        RestClient.Put<HighScoreDto>(DatabaseService.SetTableAndId(TableHighScores, highScore.PlayerName), highScore).Then(r =>
        {
            onResolved.Invoke();
        });
    }

    public static void PutHighScores(List<HighScoreDto> highScores, Action onResolved, int i = 0)
    {
        RestClient.Put<HighScoreDto>(DatabaseService.SetTableAndId(TableHighScores, highScores[i].PlayerName), highScores[i]).Then(r =>
        {
            if (++i < highScores.Count)
                PutHighScores(highScores, onResolved, i);
            else
                onResolved.Invoke();
        });
    }

    public static void GetHighScore(string playerNameId, Action<HighScoreDto> resultAction)
    {
        RestClient.Get(DatabaseService.SetTableAndId(TableHighScores, playerNameId)).Then(returnValue =>
        {
            HighScoreDto highScore = null;
            if (!string.IsNullOrEmpty(returnValue.Text) && returnValue.Text != "null")
                highScore = JsonUtility.FromJson<HighScoreDto>(returnValue.Text);
            resultAction.Invoke(highScore);
        });
    }

    public static void GetHighScores(int lastHighest, Action<List<HighScoreDto>> resultAction = null, int range = 5)
    {
        var param = $"?orderBy=\"Score\"&endAt={lastHighest}&limitToLast={range}";
        RestClient.Get(DatabaseService.SetTable(TableHighScores), param).Then(response =>
        {
            var responseJson = response.Text; // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                                              // to serialize more complex types (a Dictionary, in this case)
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            DatabaseService.Serializer.TryDeserialize(data, typeof(Dictionary<string, HighScoreDto>), ref deserialized);
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

    public static void CheckCloneScore(HighScoreDto localScore, Action<List<HighScoreDto>> thenAction)
    {
        RestClient.Get(DatabaseService.SetTable(TableHighScores), $"?orderBy=\"Score\"&startAt={localScore.Score}&limitToFirst=5").Then(response =>
        {
            if (response.Text == null || response.Text == "null")
            {
                thenAction(null);
                return;
            }
            var responseJson = response.Text; // Using the FullSerializer library: https://github.com/jacobdufault/fullserializer
                                              // to serialize more complex types (a Dictionary, in this case)
            var data = fsJsonParser.Parse(responseJson);
            object deserialized = null;
            DatabaseService.Serializer.TryDeserialize(data, typeof(Dictionary<string, HighScoreDto>), ref deserialized);
            var highScores = deserialized as Dictionary<string, HighScoreDto>;
            var listHighScores = new List<HighScoreDto>();
            foreach (var highScore in highScores)
            {
                listHighScores.Add(highScore.Value);
            }
            thenAction(listHighScores);
        });
    }

    /*
     * In order to get Range (let's say 8)
     * Page 0 => ?orderBy="Score"&limitToLast=8
     * Page 1 => ?orderBy="Score"&startAt={lowestHighScoreFromPreviousPage}&limitToFirst=8
     * 
     * Get a specific score:
     * https://infidhells-default-rtdb.europe-west1.firebasedatabase.app/HighScores.json?orderBy=%22PlayerName%22&startAt=%22Dasilver%22&limitToFirst=1
     */
}
