using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    [SerializeField] private ResourcesSO resources;

    public bool doesGenerate = false;
    
    [Tooltip("0-Water\n1-Wood\n2-Fish\n3-Metal\n4-Junk")]
    public int generateType = 0;

    public int cooldown = 60;
    private bool onCooldown = false;

    private void Update()
    {
        if (doesGenerate && !onCooldown)
        {
            onCooldown = true;

            Debug.Log("Generate");

            switch (generateType)
            {
                case (0):
                    resources.water++;
                    break;
                case (1):
                    resources.wood++;
                    break;
                case (2):
                    resources.fish++;
                    break;
                case (3):
                    resources.metal++;
                    break;
                case (4):
                    resources.junk++;
                    break;
            }

            StartCoroutine(WaitTimer());
        }
    }

    private IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
}
