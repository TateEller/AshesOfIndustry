using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject[] spawnOptions;

    public float timer = 5f;
    public int fishingHooks = 10;

    private bool gameRunning;
    public void StartFishingGame()
    {
        gameRunning = false;
        timer = 5f;
        StartCoroutine(fishingCountDown());
    }

    private IEnumerator fishingCountDown()
    {
        for (int c = 4; c > 0; c--)
        {
            timerText.text = c.ToString();
            yield return new WaitForSeconds(1f);
        }

        gameRunning = true;
        StartCoroutine(SpawnLoop());
    }

    private void Update()
    {
        if (!gameRunning) return;

        timer -= Time.deltaTime;
        int seconds = Mathf.FloorToInt(timer % 60);
        int milli = Mathf.FloorToInt((timer * 1000) % 1000);
        timerText.text = string.Format("{0:00}:{1:000}", seconds, milli);

        if (timer < 0)
        {
            StopCoroutine(SpawnLoop());
            gameRunning = false;
            timerText.text = ("00:000");
            StartCoroutine(EndFishingGame());
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (gameRunning)
        {
            int spawn = Random.Range(0, spawnOptions.Length);
            GameObject fish = Instantiate(spawnOptions[spawn], transform.GetChild(0));
            FishSwimHandler swimHandler = fish.GetComponent<FishSwimHandler>();
            if (swimHandler != null)
                swimHandler.waterRect = transform.GetChild(0).GetComponent<RectTransform>();


            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }

    }

    private IEnumerator EndFishingGame()
    {
        yield return new WaitForSeconds(1.5f);

        MenuManager menus = transform.parent.GetComponent<MenuManager>();

        menus.SlideOutMenu(this.gameObject);
        menus.SlideInMenu(transform.parent.GetChild(0).gameObject); //should be MainUIButtons
    }

    public void CollectFish()
    {
        Debug.Log("Collect Fish");
    }
    public void CollectWood()
    {
        Debug.Log("Collect Wood");
    }
}
