using UnityEngine;
using System.Collections;

public class TattooUglySneaker : Tattoo
{
    public TattooUglySneaker()
    {
        Id = 86;
        Name = TattoosData.Tattoos[Id];
        Stat = 1;
        Rarity = Rarity.Common;
        MaxLevel = 4;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.SneakerSPecialBonus += Stat;
    }

    public override string GetDescription()
    {
        return $"your special loses {StatToString(after: Stat * Level == 1 ? " bonus cooldown point" : " bonus cooldown points")} for every quadruple lines.";
    }
}