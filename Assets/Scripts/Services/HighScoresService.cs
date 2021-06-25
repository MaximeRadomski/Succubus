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
        RestClient.Put<HighScoreDto>(DatabaseService.SetTableAndId(TableHighScores, highScore.Id_PlayerName), highScore).Then(r =>
        {
            onResolved.Invoke();
        });
    }

    public static void PutHighScores(List<HighScoreDto> highScores, Action onResolved, int i = 0)
    {
        RestClient.Put<HighScoreDto>(DatabaseService.SetTableAndId(TableHighScores, highScores[i].Id_PlayerName), highScores[i]).Then(r =>
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
            var highScore = JsonUtility.FromJson<HighScoreDto>(returnValue.Text);
            resultAction.Invoke(highScore);
        });
    }

    public static void GetHighScores(Action<List<HighScoreDto>> resultAction)
    {
        RestClient.Get(DatabaseService.SetTable(TableHighScores)).Then(response =>
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
}
