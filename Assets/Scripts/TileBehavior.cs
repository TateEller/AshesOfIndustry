using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    [SerializeField] private ResourcesSO resources;

    public bool doesGenerate = false;
    public bool randomGeneration = true;
    public Resources setGeneration;

    public int cooldown = 60;
    private bool onCooldown = false;

    private void Update()
    {
        if (doesGenerate && !onCooldown)
        {
            onCooldown = true;

            Debug.Log("Generate");

            if (randomGeneration)
            {
                //randomize the resource
                setGeneration = (Resources)Random.Range(0, System.Enum.GetValues(typeof(Resources)).Length);
            }

            switch (setGeneration)
            {
                case (Resources.water):
                    resources.Water++;
                    break;
                case (Resources.wood):
                    resources.Wood++;
                    break;
                case (Resources.fish):
                    resources.Fish++;
                    break;
                case (Resources.metal):
                    resources.Metal++;
                    break;
                case (Resources.junk):
                    resources.Junk++;
                    break;
            }

            Debug.Log($"Generate {setGeneration}");
            StartCoroutine(WaitTimer());
        }
    }

    private IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(cooldown);
        onCooldown = false;
    }
}
