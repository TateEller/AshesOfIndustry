using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExploreHandler : MonoBehaviour
{
    [SerializeField] private MenuManager menuMan;
    [SerializeField] private GameObject mainUI;
    [SerializeField] private GameObject battlePanel;
    [SerializeField] private ResourcesSO resources;
    [SerializeField] private DialogueSO dialogue;
    [SerializeField] private TextMeshProUGUI eventText;


    public void StartExploring()
    {
        menuMan.SlideOutMenu(mainUI);
        eventText.text = "...";
        menuMan.SlideInMenu(this.gameObject);

        StartCoroutine(ExploreCountdown());
    }

    private IEnumerator ExploreCountdown()
    {
        yield return new WaitForSeconds(3f);

        int encounter = Random.Range(19, 20);
        if (encounter < 11)
        {
            ItemEncounter();
        }
        else if (10 < encounter && encounter < 19)
        {
            //dialouge, lore, no outcome
            eventText.text = (dialogue.GetRandomLine());
        }
        else if (encounter == 19 || encounter == 20)
        {
            //battle
            menuMan.SlideOutMenu(this.gameObject);
            menuMan.SlideInMenu(battlePanel);
            //yield return new WaitForSeconds(1);
            Debug.Log("Battle");
            battlePanel.GetComponent<BattleManager>().StartBattle();
        }  


        StartCoroutine(WaitForExit());
    }

    private void ItemEncounter()
    {
        Resources ran = (Resources)Random.Range(0, System.Enum.GetValues(typeof(Resources)).Length);

        int amount = 0;
        while(amount == 0)
        {
            amount = Random.Range(-3, 6);
        }

        if(amount > 0)
        {
            eventText.text = ($"Gain {amount} {ran}");
        }
        else if (amount < 0)
        {
            eventText.text = ($"Lose {amount * -1} {ran}");
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
    private IEnumerator WaitForExit()
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.touchCount > 0);

        menuMan.SlideOutMenu(this.gameObject);
        menuMan.SlideInMenu(mainUI);
        eventText.text = "...";
    }

    public void BattleOver(string results)
    {
        eventText.text = results;

        menuMan.SlideOutMenu(battlePanel);
        menuMan.SlideInMenu(this.gameObject);

        StartCoroutine(WaitForExit());
    }
}
