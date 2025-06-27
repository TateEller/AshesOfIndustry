using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueSO : ScriptableObject
{
    [SerializeField] private List<string> oneOffLines = new();
    [SerializeField] private List<string> loreLines = new();
    private List<string> usedLoreLines = new();

    public string GetRandomLine()
    {
        int ran = Random.Range(0, 3);

        switch (ran)
        {
            default:
            case (0):
                return oneOffLines[Random.Range(0, oneOffLines.Count)];
            case (1):
                if (loreLines.Count > 0)
                {
                    //if there are still lore lines
                    int ranLine = Random.Range(0, loreLines.Count);
                    string line = loreLines[ranLine];
                    usedLoreLines.Add(line);

                    //remove lore line so not repeating same lore
                    loreLines[ranLine] = loreLines[loreLines.Count - 1];
                    return line;
                }
                else return oneOffLines[Random.Range(0, oneOffLines.Count)];
        }
    }

    public void ResetLoreLines()
    {
        foreach(string line in usedLoreLines)
        {
            if (!loreLines.Contains(line))
            {
                loreLines.Add(line);
            }
        }

        usedLoreLines.Clear();
    }
}
