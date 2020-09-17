﻿using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Step
{
    public int X;
    public int Y;
    public Realm Realm;
    public StepType StepType;
    public bool Discovered;
    public bool LandLordVision;
    public LootType LootType;
    public int LootId;
    public List<Opponent> Opponents;

    public Step(int x, int y, Realm realm, StepType stepType, bool discovered, bool landLordVision, LootType lootType, int lootId, List<Opponent> opponents)
    {
        X = x;
        Y = y;
        Realm = realm;
        StepType = stepType;
        Discovered = discovered;
        LandLordVision = landLordVision;
        LootType = lootType;
        LootId = lootId;
        Opponents = opponents;
    }

    public Step(string parsedString)
    {
        X = int.Parse(parsedString.Substring(parsedString.IndexOf('X') + 1, 2));
        X = int.Parse(parsedString.Substring(parsedString.IndexOf('Y') + 1, 2));
        Realm = (Realm)int.Parse(parsedString.Substring(parsedString.IndexOf('R') + 1, 1));
        StepType = (StepType)int.Parse(parsedString.Substring(parsedString.IndexOf('S') + 1, 2));
        Discovered = int.Parse(parsedString.Substring(parsedString.IndexOf('D') + 1, 1)) == 1 ? true : false;
        LandLordVision = int.Parse(parsedString.Substring(parsedString.IndexOf('V') + 1, 1)) == 1 ? true : false;
        var lootLetter = parsedString.Substring(parsedString.IndexOf('V') + 2, 1);
        for (int i = 0; i < Helper.EnumCount<LootType>(); ++i)
        {
            if (lootLetter == ((LootType)i).ToString().Substring(0, 1))
                LootType = (LootType)i;
        }
        LootId = int.Parse(parsedString.Substring(parsedString.IndexOf(LootType.ToString()[0]) + 1, 2));
        //Opponents (ugly part)
        var realmOpponents = new List<Opponent>();
        if (Realm == Realm.Hell)
            realmOpponents = OpponentsData.HellOpponents;
        else if (Realm == Realm.Earth)
            realmOpponents = OpponentsData.EarthOpponents;
        else if (Realm == Realm.Heaven)
            realmOpponents = OpponentsData.HeavenOpponents;
        var nextOpponentIdStart = parsedString.IndexOf('O') + 1;
        Opponents = new List<Opponent>();
        while (nextOpponentIdStart <= parsedString.Length && parsedString[nextOpponentIdStart] != ';')
        {
            Opponents.Add(realmOpponents[int.Parse(parsedString.Substring(nextOpponentIdStart, 2))]);
            nextOpponentIdStart += 2;
        }
    }

    public string ToParsedString()
    {
        var stepStr = "";
        stepStr += "X" + X.ToString("00");
        stepStr += "Y" + Y.ToString("00");
        stepStr += "R" + Realm.GetHashCode().ToString("0");
        stepStr += "S" + StepType.GetHashCode().ToString("00");
        stepStr += "D" + (Discovered ? "1" : "0");
        stepStr += "V" + (LandLordVision ? "1" : "0");
        stepStr += LootType.ToString().Substring(0, 1) + LootId.ToString("00");
        stepStr += "O";
        foreach (var opponent in Opponents)
            stepStr += opponent.Id.ToString("00");
        return stepStr;
    }
}
