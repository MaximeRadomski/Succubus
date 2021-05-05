using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class ItemInnerStrength : Item
{
    public ItemInnerStrength()
    {
        Id = 5;
        Name = ItemsData.Items[Id];
        Description = $"{Highlight("quadruples")} your base attack damages for {Highlight("4 seconds")}.";
        Rarity = Rarity.Rare;
        Cooldown = 10;
    }

    private int _oldAttack;

    protected override object Effect()
    {
        _oldAttack = _character.GetAttackNoBoost();
        _character.BoostAttack += (_oldAttack * 3);
        _gameplayControler.CharacterInstanceBhv.Boost(_character.Realm, 4.0f);
        Task.Delay(4000).ContinueWith(t => AfterDelay());
        return base.Effect();
    }

    private void AfterDelay()
    {
        _character.BoostAttack -= _oldAttack;
    }
}