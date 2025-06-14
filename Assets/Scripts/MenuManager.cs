using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    public void SlideInMenu(GameObject menu)
    {
        menu.SetActive(true);
        var rt = menu.GetComponent<RectTransform>();
        if (rt)
        {
            //set position offscreen
            var pos = rt.position;
            pos.y = -Screen.width / 2;  //slide in from bottom
            rt.position = pos;

            //move to center
            var tween = LeanTween.moveY(rt, 0, 1.5f);
            tween.setEase(LeanTweenType.easeInOutExpo);
            tween.setIgnoreTimeScale(true);
        }
    }
    public void SlideOutMenu(GameObject menu)
    {
        var rt = menu.GetComponent<RectTransform>();
        if (rt)
        {
            var tween = LeanTween.moveY(rt, -(Screen.width / 2), 0.5f); //slide out bottom
            tween.setEase(LeanTweenType.easeOutQuad);
            tween.setIgnoreTimeScale(true);
            tween.setOnComplete(() => { menu.SetActive(false); });
        }
    }
}
