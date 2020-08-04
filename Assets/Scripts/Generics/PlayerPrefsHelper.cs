using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class PlayerPrefsHelper : MonoBehaviour
{

    public static void SaveRun(Run run)
    {
        PlayerPrefs.SetString(Constants.PpRun, JsonUtility.ToJson(run));
    }

    public static Run GetRun()
    {
        var run = JsonUtility.FromJson<Run>(PlayerPrefs.GetString(Constants.PpRun, Constants.PpSerializeDefault));
        if (run == null)
            return null;
        return run;
    }

    public static void SaveBag(string bag)
    {
        PlayerPrefs.SetString(Constants.PpBag, bag);
    }

    public static string GetBag()
    {
        var bag = PlayerPrefs.GetString(Constants.PpBag, Constants.PpSerializeDefault);
        return bag;
    }

    public static void SaveHolder(string holder)
    {
        PlayerPrefs.SetString(Constants.PpHolder, holder);
    }

    public static string GetHolder()
    {
        var holder = PlayerPrefs.GetString(Constants.PpHolder, Constants.PpSerializeDefault);
        return holder;
    }

    public static void SaveTraining(int score, int level, int lines, int pieces)
    {
        PlayerPrefs.SetInt(Constants.PpTrainingScore, score);
        PlayerPrefs.SetInt(Constants.PpTrainingLevel, level);
        PlayerPrefs.SetInt(Constants.PpTrainingLines, lines);
        PlayerPrefs.SetInt(Constants.PpTrainingPieces, pieces);
    }

    public static List<int> GetTraining()
    {
        var results = new List<int>();
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingScore, 0));
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingLevel, 1));
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingLines, 0));
        results.Add(PlayerPrefs.GetInt(Constants.PpTrainingPieces, 0));
        return results;
    }

    public static void SaveTrainingHightScore(int score)
    {
        PlayerPrefs.SetInt(Constants.PpTrainingHighScore, score);
    }

    public static int GetTrainingHighScore()
    {
        var highScore = PlayerPrefs.GetInt(Constants.PpTrainingHighScore, 0);
        return highScore;
    }
}
