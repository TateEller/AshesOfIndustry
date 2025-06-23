using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUIHandler : MonoBehaviour
{
    [SerializeField] private ResourcesSO resource;

    void Update()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ($"Water: {resource.Water}");
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ($"Wood: {resource.Wood}");
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ($"Fish: {resource.Fish}");
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = ($"Metal: {resource.Metal}");
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ($"Junk: {resource.Junk}");
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
