using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenderSwapCoffin : Item
{
    public ItemGenderSwapCoffin()
    {
        Id = 30;
        Name = ItemsData.Items[Id];
        Description = $"gives you {Highlight("+2 damage")} but also {Highlight("randomizes")} your character appearance and your special for the duration of the fight.";
        Rarity = Rarity.Legendary;
        Cooldown = 10;
    }

    protected override void Effect()
    {
        Cache.PactFlatDamage += 2;
        var id = Random.Range(0, 13);
        Character tmpChar;
        if (id == 12 || id == this._character.Id)
            tmpChar = CharactersData.CustomCharacters[1];
        else
            tmpChar = CharactersData.Characters[id];
        _gameplayControler.SceneBhv.TemporaryCharacter(tmpChar);        
        base.Effect();
    }
}
