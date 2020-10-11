using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tattoo : Loot
{
    public int Id;
    public string Name;
    public int Stat;
    public string StatStr;
    public int Level = 1;
    public int MaxLevel;
    public BodyPart BodyPart;

    public abstract string GetDescription();
    public abstract void ApplyToCharacter(Character character);

    public Tattoo()
    {
        LootType = LootType.Tattoo;
    }

    protected string StatToString(string before = "", string after = "")
    {
        string stat;
        if (Stat != 0)
            stat = (Stat * Level).ToString();
        else
            stat = StatStr;
        return $"{Constants.MaterialHell_4_3}{before}{stat}{after}{Constants.MaterialEnd}";
    }
}
