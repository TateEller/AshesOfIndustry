using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Resources { water, wood, fish, metal, junk }

[CreateAssetMenu]
public class ResourcesSO : ScriptableObject
{
    public Dictionary<Resources, int> resourceData = new();

    private int water = 0;
    private int wood = 0;
    private int fish = 0;
    private int metal = 0;
    private int junk = 0;  

    public int Water
    {
        get
        {
            resourceData.TryGetValue(Resources.water, out water);
            return water;
        }
        set
        {
            resourceData[Resources.water] = value;
            if (resourceData[Resources.water] < 0) resourceData[Resources.water] = 0;
        }
    }
    public int Wood
    {
        get
        {
            resourceData.TryGetValue(Resources.wood, out wood);
            return wood;
        }
        set
        {
            resourceData[Resources.wood] = value;
            if (resourceData[Resources.wood] < 0) resourceData[Resources.wood] = 0;
        }
    }
    public int Fish
    {
        get
        {
            resourceData.TryGetValue(Resources.fish, out fish);
            return fish;
        }
        set
        {
            resourceData[Resources.fish] = value;
            if (resourceData[Resources.fish] < 0) resourceData[Resources.fish] = 0;
        }
    }
    public int Metal
    {
        get
        {
            resourceData.TryGetValue(Resources.metal, out metal);
            return metal;
        }
        set
        {
            resourceData[Resources.metal] = value;
            if (resourceData[Resources.metal] < 0) resourceData[Resources.metal] = 0;
        }
    }
    public int Junk
    {
        get
        {
            resourceData.TryGetValue(Resources.junk, out junk);
            return junk;
        }
        set
        {
            resourceData[Resources.junk] = value;
            if (resourceData[Resources.junk] < 0) resourceData[Resources.junk] = 0;
        }
    }
}
