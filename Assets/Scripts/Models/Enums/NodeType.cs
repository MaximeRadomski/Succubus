using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public enum NodeType 
{
    [Title("Attack Boost")]
    [Description("+1 attack damage.")]
    AttackBoost,
    [Title("Cooldown Brake")]
    [Description("+0.666 second added to opponents cooldowns.")]
    CooldownBrake,
    [Title("Critical Precision")]
    [Description("+2% critical chance.")]
    CriticalPrecision,
    [Title("Posthumous Item")]
    [Description("-1 item's cooldown after a fight.")]
    PosthumousItem,
    [Title("Lock Delay")]
    [Description("+0.25 second lock delay.")]
    LockDelay,
    [Title("Life Roulette")]
    [Description("+50% chance of reviving once.")]
    LifeRoulette,
    [Title("Boss Hate")]
    [Description("+10% damages on bosses.")]
    BossHate,
    [Title("Shadowing")]
    [Description("+1 possible step per zone.")]
    Shadowing,
    [Title("Repentance")]
    [Description("+1 life.")]
    Repentance
}
