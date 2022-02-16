using UnityEngine;
using System.Collections;

public class TattooXRayGlasses : Tattoo
{
    private int _dodgeChance = 15;

    public TattooXRayGlasses()
    {
        Id = 15;
        Name = TattoosData.Tattoos[Id];
        Stat = 0;
        StatStr = "through";
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.XRay = true;
        character.DodgeChance += _dodgeChance;
    }

    public override string GetDescription()
    {
        return $"allows you to see your pieces' shadows {StatToString()} vision blocks, and gives you {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}+{_dodgeChance * Level}%{Constants.MaterialEnd} chance of dodging attacks.";
    }
}