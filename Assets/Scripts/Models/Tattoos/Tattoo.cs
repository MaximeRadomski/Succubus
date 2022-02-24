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

    protected string StatToString(string before = "", string after = "", float statMultiplier = 1.0f)
    {
        string stat;
        if (Stat != 0)
        {
            var tmpStat = Mathf.RoundToInt(Stat * statMultiplier);
            stat = (tmpStat * Level).ToString();
        }
        else
            stat = StatStr;
        return $"{Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}{before}{stat}{after}{Constants.MaterialEnd}";
    }

    protected void FillAllSpace()
    {
        var tattoosFullStr = Mock.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
        //DEBUG
        if (Constants.TattoosStringDebug != string.Empty)
            tattoosFullStr = Constants.TattoosStringDebug;
        //DEBUG
        var nameToAdd = Name.Replace(" ", "").Replace("'", "").Replace("-", "");
        var alreadyBodyPartsIds = Mock.GetString(Constants.PpCurrentBodyParts);
        if (alreadyBodyPartsIds == null)
            alreadyBodyPartsIds = "";
        var availablesPartsIds = PlayerPrefsHelper.GetRemainingAvailablesPartsIds(alreadyBodyPartsIds);
        for (int i = 0; i < availablesPartsIds.Length; i += 2)
        {
            var newBodyPartId = i / 2;
            var newBodyPartStr = availablesPartsIds.Substring(newBodyPartId * 2, 2);
            alreadyBodyPartsIds += newBodyPartStr;
            tattoosFullStr += nameToAdd + "L01B" + newBodyPartStr + ";";
        }
        PlayerPrefsHelper.AddMaxedOutTattoo(Id);
        Mock.SetString(Constants.PpCurrentBodyParts, alreadyBodyPartsIds);
        Mock.SetString(Constants.PpCurrentTattoos, tattoosFullStr);
    }
}
