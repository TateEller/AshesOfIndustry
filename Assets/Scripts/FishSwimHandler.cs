using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSwimHandler : MonoBehaviour
{
    [SerializeField] internal RectTransform waterRect;
    [SerializeField] internal RectTransform canvas;

    private void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        if (waterRect == null)
            waterRect = transform.parent.GetComponent<RectTransform>();

        RectTransform rect = GetComponent<RectTransform>();

        rect.SetParent(waterRect, false);

        Vector2 startPos, endPos;
        GetEdgePositions(out startPos, out endPos);

        rect.anchoredPosition = startPos;

        float swimSpeed = Random.Range(2f, 3f);
        LeanTween.move(rect, endPos, swimSpeed).setEase(LeanTweenType.linear).setOnComplete(() => Destroy(gameObject));
    }

    private void GetEdgePositions(out Vector2 start, out Vector2 end)
    {
        int sEdge = Random.Range(0, 4);
        int eEdge = Random.Range(0, 4);

        do { eEdge = Random.Range(0, 4); }
        while (eEdge == sEdge);

        start = GetRandomPositionsOffscreen(sEdge);
        end = GetRandomPositionsOffscreen(eEdge);
    }

    private Vector2 GetRandomPositionsOffscreen(int edge)
    {
        float width = canvas.rect.width;
        float height = canvas.rect.height;

        switch (edge)
        {
            case 0: //top
                return new Vector2(Random.Range(0, width), height);

            case 1: //left
                return new Vector2(0, Random.Range(0, height));

            case 2: //right
                return new Vector2(width, Random.Range(0, height));

            case 3: //bottom
            default:
                return new Vector2(Random.Range(0, width), 0);
        }
    }
}
