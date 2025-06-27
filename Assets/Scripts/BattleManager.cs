using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleStatsSO pStats;
    [SerializeField] private GameObject playerShip;
    private Slider pHealthBar;

    [SerializeField] private Sprite[] enemyOptions;
    [SerializeField] private GameObject enemyShip;
    private Slider eHealthBar;

    private Dictionary<stats, float> enemyStats = new();

    public int battlesWon;

    [SerializeField] private ResourcesSO resources;
    [SerializeField] private ExploreHandler explorer;
    [SerializeField] private GameObject explosion;

    private void Awake()
    {
        playerShip = transform.GetChild(0).gameObject;
        enemyShip = transform.GetChild(1).gameObject;

        pHealthBar = playerShip.GetComponentInChildren<Slider>();
        eHealthBar = enemyShip.GetComponentInChildren<Slider>();
    }
    public void StartBattle()
    {
        pHealthBar.maxValue = pStats.Health;
        pHealthBar.value = pStats.Health;

        battlesWon = PlayerPrefs.GetInt("BattlesWon", 0);

        GenerateRandomEnemy(battlesWon);

        //start battle
        StartCoroutine(PlayerAttackLoop());
        StartCoroutine(EnemyAttackLoop());
    }

    private void GenerateRandomEnemy(int diff)
    {
        float difficultyMod = 1 + (0.01f * diff);

        //set random stats
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

        //update sprite
        enemyShip.transform.GetChild(1).GetComponent<Image>().sprite = enemyOptions[Random.Range(0, enemyOptions.Length)];

        Debug.Log($"Enemy stats: H-{health}, D-{dmg}, AS-{speed}");
    }

    //do attack animation
    //deal dmg
    //wait cooldown
    //repeat

    private IEnumerator PlayerAttackLoop()
    {
        while(enemyStats[stats.Health] > 0 && pStats.Health > 0)
        {
            yield return new WaitForSeconds(pStats.AttackSpeed);
            GameObject temp = Instantiate(explosion, enemyShip.transform);
            Destroy(temp, 0.5f);

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
            GameObject temp = Instantiate(explosion, playerShip.transform);
            Destroy(temp, 0.5f);

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
            PlayerPrefs.SetInt("BattlesWon", PlayerPrefs.GetInt("BattlesWon") + 1);
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
