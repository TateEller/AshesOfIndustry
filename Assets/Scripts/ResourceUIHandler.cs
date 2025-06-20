using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUIHandler : MonoBehaviour
{
    [SerializeField] private ResourcesSO resource;

    void Update()
    {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ($"Water: {resource.water}");
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ($"Wood: {resource.wood}");
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = ($"Fish: {resource.fish}");
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = ($"Metal: {resource.metal}");
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = ($"Junk: {resource.junk}");
    }

    public void ClearData()
    {
        resource.water = 0;
        resource.wood = 0;
        resource.fish = 0;
        resource.metal = 0;
        resource.junk = 0;
    }
}
