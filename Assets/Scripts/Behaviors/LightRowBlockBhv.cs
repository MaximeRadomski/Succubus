using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRowBlockBhv : MonoBehaviour
{
    public bool IsGate;
    public int NbRows;
    public int Cooldown;
    public TMPro.TextMeshPro CooldownText;

    public bool IsOverOrDecreaseCooldown()
    {
        UpdateCooldownText(--Cooldown);
        if (Cooldown > 0)
            return false;
        return true;
    }

    public void UpdateCooldownText(int cooldown)
    {
        CooldownText.text = (cooldown).ToString();
    }
}
