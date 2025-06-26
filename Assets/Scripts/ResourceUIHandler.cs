using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUIHandler : MonoBehaviour
{
    [SerializeField] private ResourcesSO resource;
    [SerializeField] private TextMeshProUGUI[] resourceText = new TextMeshProUGUI[5];

    void Update()
    {
        resourceText[0].text = resource.Water.ToString();
        resourceText[1].text = resource.Wood.ToString();
        resourceText[2].text = resource.Fish.ToString();
        resourceText[3].text = resource.Metal.ToString();
        resourceText[4].text = resource.Junk.ToString();
    }

    public void ClearData()
    {
        resource.Water = 0;
        resource.Wood = 0;
        resource.Fish = 0;
        resource.Metal = 0;
        resource.Junk = 0;
    }
}
