using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingSystem : MonoBehaviour
{
    private void Update()
    {
        foreach(var c in FadingSprite.Instances)
        {
            if(c.gameObject.activeSelf)
            {
                c.alpha = Mathf.SmoothDamp(c.alpha, c.targetAlpha, ref c.velocity, 0.1f, 1f);
                c.sp.color = new Color(1, 1, 1, c.alpha);
            }
        }
    }
}
