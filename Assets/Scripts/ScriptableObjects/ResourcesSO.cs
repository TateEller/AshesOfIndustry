using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Resources { water, wood, fish, metal, junk }

[CreateAssetMenu]
public class ResourcesSO : ScriptableObject
{
    //public Dictionary<Resources, int> resourceData = new();

    public int Water
    {
        get
        {
            return PlayerPrefs.GetInt("Water");
        }
        set
        {
            PlayerPrefs.SetInt("Water", value);

            if (PlayerPrefs.GetInt("Water") < 0)
                PlayerPrefs.SetInt("Water", 0);

            PlayerPrefs.Save();
        }
    }
    public int Wood
    {
        get
        {
            return PlayerPrefs.GetInt("Wood");
        }
        set
        {
            PlayerPrefs.SetInt("Wood", value);

            if (PlayerPrefs.GetInt("Wood") < 0)
                PlayerPrefs.SetInt("Wood", 0);

            PlayerPrefs.Save();
        }
    }
    public int Fish
    {
        get
        {
            return PlayerPrefs.GetInt("Fish");
        }
        set
        {
            PlayerPrefs.SetInt("Fish", value);

            if (PlayerPrefs.GetInt("Fish") < 0)
                PlayerPrefs.SetInt("Fish", 0);

            PlayerPrefs.Save();
        }
    }
    public int Metal
    {
        get
        {
            return PlayerPrefs.GetInt("Metal");
        }
        set
        {
            PlayerPrefs.SetInt("Metal", value);

            if (PlayerPrefs.GetInt("Metal") < 0)
                PlayerPrefs.SetInt("Metal", 0);

            PlayerPrefs.Save();
        }
    }
    public int Junk
    {
        get
        {
            return PlayerPrefs.GetInt("Junk");
        }
        set
        {
            PlayerPrefs.SetInt("Junk", value);

            if (PlayerPrefs.GetInt("Junk") < 0)
                PlayerPrefs.SetInt("Junk", 0);

            PlayerPrefs.Save();
        }
    }
}
