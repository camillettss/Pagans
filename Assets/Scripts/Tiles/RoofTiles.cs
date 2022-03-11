using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

[RequireComponent(typeof(TilemapCollider2D))]
public class RoofTiles : MonoBehaviour
{
    float invisibleAlpha = 0.0f;
    float visibleAlpha = 1.0f;

    const float innerLightIntensity = 0.3f;

    Tilemap tmap;

    private void Start()
    {
        tmap = GetComponent<Tilemap>();
    }

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            GameController.Instance.canHandleLight = false;
            float alpha = tmap.color.a;

            for(float t = 0; t<1; t+=Time.deltaTime/0.5f) // 0.5 è il tempo di fading
            {
                GameController.Instance.GameplayLight.intensity = Mathf.Lerp(GameController.Instance.GameplayLight.intensity, innerLightIntensity, t);
                tmap.color = new Color(tmap.color.r, tmap.color.g, tmap.color.b, Mathf.Lerp(alpha, invisibleAlpha, t));
                yield return null;
            }
        }
    }

    private IEnumerator OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            float alpha = tmap.color.a;
            float target = GameController.Instance.GameplayLight.intensity;

            for (float t = 0; t < 1; t += Time.deltaTime / 0.5f) // 0.5 è il tempo di fading
            {
                GameController.Instance.GameplayLight.intensity = Mathf.Lerp(GameController.Instance.GameplayLight.intensity, target, t);
                tmap.color = new Color(tmap.color.r, tmap.color.g, tmap.color.b, Mathf.Lerp(alpha, visibleAlpha, t));
                yield return null;
            }

            GameController.Instance.canHandleLight = true;
        }
    }
}
