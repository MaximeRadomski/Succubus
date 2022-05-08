using UnityEngine;
using System.Collections;

public class TattooXRayGlasses : Tattoo
{
    public TattooXRayGlasses()
    {
        Id = 15;
        Name = TattoosData.Tattoos[Id];
        Stat = 15;
        Rarity = Rarity.Rare;
        MaxLevel = 2;
    }

    public override void ApplyToCharacter(Character character)
    {
        character.DodgeChance += Stat;
        character.XRay = true;
    }

    protected override void CustomRemove(Character character)
    {
        character.XRay = false;
    }

    public override string GetDescription()
    {
        return $"allows you to see your pieces' shadows {Constants.GetMaterial(Realm.Hell, TextType.succubus3x5, TextCode.c43)}through{Constants.MaterialEnd} vision blocks, and gives you {StatToString("+", "%")} chance of dodging attacks.";
    }
}