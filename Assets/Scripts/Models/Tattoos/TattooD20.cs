using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class TattooD20 : Tattoo
{
    public TattooD20()
    {
        Id = 9;
        Name = TattoosData.Tattoos[Id];
        Stat = 4;
        Rarity = Rarity.Common;
        MaxLevel = 11;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.CritChancePercent += Stat;
    }

    public override string GetDescription()
    {
        return $"increase your critical chance\nby {StatToString(after: "%")}";
    }
}
