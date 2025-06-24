using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleStatsSO pStats;
    [SerializeField] private GameObject playerShip;
    private Slider pHealthBar;

    [SerializeField] private GameObject enemyShip;
    private Slider eHealthBar;

    private Dictionary<stats, float> enemyStats = new();

    private int battlesWon = 0;

    [SerializeField] private ResourcesSO resources;
    [SerializeField] private ExploreHandler explorer;

    public void StartBattle()
    {
        pHealthBar = playerShip.GetComponentInChildren<Slider>();
        eHealthBar = enemyShip.GetComponentInChildren<Slider>();

        pHealthBar.maxValue = pStats.Health;
        pHealthBar.value = pStats.Health;

        GenerateRandomEnemy(battlesWon);

        //start battle
        StartCoroutine(PlayerAttackLoop());
        StartCoroutine(EnemyAttackLoop());
    }

    private void GenerateRandomEnemy(int diff)
    {
        float difficultyMod = 1 + (0.01f * diff);

        float health = Random.Range(10, 20);
        health *= difficultyMod;
        Mathf.RoundToInt(health);
        enemyStats[stats.Health] = health;
        eHealthBar.maxValue = health;
        eHealthBar.value = health;

        float dmg = Random.Range(2, 5);
        dmg *= difficultyMod;
        enemyStats[stats.Damage] = dmg;

        float speed = Random.Range(1, 3);
        speed *= difficultyMod;
        enemyStats[stats.AttackSpeed] = speed;


        Debug.Log($"Enemy stats: H-{health}, D-{dmg}, AS-{speed}");
    }
    private IEnumerator PlayerAttackLoop()
    {
        while(enemyStats[stats.Health] > 0 && pStats.Health > 0)
        {
            yield return new WaitForSeconds(pStats.AttackSpeed);
            enemyStats[stats.Health] -= pStats.Damage;
            eHealthBar.value = enemyStats[stats.Health];
            Debug.Log("Player attack");

            if (enemyStats[stats.Health] <= 0)
                EndBattle(true);
        }
    }
    private IEnumerator EnemyAttackLoop()
    {
        while (enemyStats[stats.Health] > 0 && pStats.Health > 0)
        {
            yield return new WaitForSeconds(enemyStats[stats.AttackSpeed]);
            pStats.Health -= enemyStats[stats.Damage];
            pHealthBar.value = pStats.Health;
            Debug.Log("Enemy attack");

            if (pStats.Health <= 0)
                EndBattle(false);
        }
    }

    private void EndBattle(bool playerWon)
    {
        StopAllCoroutines();

        if (playerWon)
        {
            battlesWon++;
            LootManager(2, 6);
        }
        else
        {
            LootManager(-4, -1);
        }
    }

    private void LootManager(int min, int max)
    {
        Resources ran = (Resources)Random.Range(0, System.Enum.GetValues(typeof(Resources)).Length);

        int amount = 0;
        while (amount == 0)
        {
            amount = Random.Range(min, max);
        }

        if (amount > 0)
        {
            explorer.BattleOver($"Gain {amount} {ran}");
        }
        else if (amount < 0)
        {
            explorer.BattleOver($"Lose {amount * -1} {ran}");
        }

        switch (ran)
        {
            case (Resources.water):
                resources.Water += amount;
                break;
            case (Resources.wood):
                resources.Wood += amount;
                break;
            case (Resources.fish):
                resources.Fish += amount;
                break;
            case (Resources.metal):
                resources.Metal += amount;
                break;
            case (Resources.junk):
                resources.Junk += amount;
                break;
        }
    }
}
