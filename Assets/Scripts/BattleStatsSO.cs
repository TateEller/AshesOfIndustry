using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum stats { Health, Damage, AttackSpeed }

[CreateAssetMenu]
public class BattleStatsSO : ScriptableObject
{
    public Dictionary<stats, float> playerStats = new();

    private void OnEnable()
    {
        if (playerStats.Count == 0)
        {
            playerStats[stats.Health] = 15;
            playerStats[stats.Damage] = 5;
            playerStats[stats.AttackSpeed] = 1.5f;
        }
    }

    public float Health
    {
        get
        {
            return playerStats[stats.Health];
        }
        set
        {
            playerStats[stats.Health] = value;
        }
    }
    public float Damage
    {
        get
        {
            return playerStats[stats.Damage];
        }
        set
        {
            playerStats[stats.Damage] = value;
        }
    }
    public float AttackSpeed
    {
        get
        {
            return playerStats[stats.AttackSpeed];
        }
        set
        {
            playerStats[stats.AttackSpeed] = value;
        }
    }
}
