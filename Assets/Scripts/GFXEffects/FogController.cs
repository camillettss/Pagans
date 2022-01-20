using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FogController : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> FogGObjects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var fog in FogGObjects)
            fog.DOFade(1, 1f); 
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        foreach (var fog in FogGObjects)
            fog.DOFade(0, .3f);
    }
}
