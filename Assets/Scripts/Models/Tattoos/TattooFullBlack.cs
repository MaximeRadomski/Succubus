using UnityEngine;
using System.Collections;

public class TattooFullBlack : Tattoo
{
    public TattooFullBlack()
    {
        Id = 26;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Legendary;
        MaxLevel = 1;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.BonusLife += Stat;

        var tattoosFullStr = Mock.GetString(Constants.PpCurrentTattoos, Constants.PpSerializeDefault);
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

    public override string GetDescription()
    {
        return $"gives you {StatToString("+", " life")}, but takes all the remaining place on your body.";
    }
}