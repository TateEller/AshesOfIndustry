using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingManager : MonoBehaviour
{
    [SerializeField] private ResourcesSO resources;

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
        if (Input.GetMouseButtonDown(0))
        {
            //testing
        }

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
            int ran = Random.Range(0, spawnOptions.Length); //get random spawn
            GameObject spawn = Instantiate(spawnOptions[ran], transform.GetChild(0));
            Button btn = spawn.GetComponent<Button>();
            switch (ran)    //assign button action
            {
                case (0):   //fish
                    btn.onClick.AddListener(() =>
                    {
                        Debug.Log("Collect Fish");
                        resources.fish++;
                        Destroy(spawn);
                    });
                    break;
                case (1):   //wood
                    btn.onClick.AddListener(() =>
                    {
                        Debug.Log("Collect Wood");
                        resources.wood++;
                        Destroy(spawn);
                    });
                    break;
                default:    //trash
                    btn.onClick.AddListener(() =>
                    {
                        Debug.Log("Collect Trash");
                        resources.junk++;
                        Destroy(spawn);
                    });
                    break;
            }

            //get positions
            Vector2 startPos, endPos;
            GetSwimmingPositions(out startPos, out endPos);

            RectTransform rect = spawn.GetComponent<RectTransform>();
            rect.position = startPos;

            //rotate to look in direction of swiming
            Vector2 direction = (endPos - startPos).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rect.rotation = Quaternion.Euler(0, 0, angle);
            
            float swimSpeed = Random.Range(4f, 5f); //high number is moving slower

            //MOVE :)
            LeanTween.move(spawn, endPos, swimSpeed).setEase(LeanTweenType.linear).setOnComplete(() => Destroy(spawn));
            
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    private void GetSwimmingPositions(out Vector2 sPos, out Vector2 ePos)
    {
        Vector2 start, end;

        RectTransform canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        float width = canvas.rect.width;
        float height = canvas.rect.height;

        switch (Random.Range(0, 4)) //where it starts
        {
            case 0: //top
                start = new Vector2(Random.Range(0, width), height);
                end = new Vector2(Random.Range(0, width), 0); //end bottom
                break;

            case 1: //left
                start = new Vector2(0, Random.Range(0, height));
                end = new Vector2(width, Random.Range(0, height));    //end right
                break;

            case 2: //right
                start = new Vector2(width, Random.Range(0, height));
                end = new Vector2(0, Random.Range(0, height));    //end left
                break;

            case 3: //bottom
            default:
                start = new Vector2(Random.Range(0, width), 0);
                end = new Vector2(Random.Range(0, width), height);    //end top
                break;
        }

        sPos = start;
        ePos = end;
    }

    private IEnumerator EndFishingGame()
    {
        foreach (RectTransform child in transform.GetChild(0).GetComponentInChildren<RectTransform>())
        {
            Destroy(child.gameObject);  //destroy any fishing items not collected
        }

        //display score

        yield return new WaitForSeconds(1.5f);

        MenuManager menus = transform.parent.GetComponent<MenuManager>();

        menus.SlideOutMenu(this.gameObject);
        menus.SlideInMenu(transform.parent.GetChild(0).gameObject); //should be MainUIButtons
    }
}
