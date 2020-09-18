using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TattooPower : Tattoo
{
    public override void Init(Character character, GameplayControler gameplayControler)
    {
        base.Init(character, gameplayControler);
        Id = 0;
        Name = "Power";
        Stat = 10;
        Description = "You deal " + StatToString("+", "%") + "";
    }

    public override void ApplyToCharacter(Character character)
    {
        base.ApplyToCharacter(character);
        character.DamagePercentBonus += Stat;
    }
}
