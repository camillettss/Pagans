using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class LightShaderFX : MonoBehaviour
{
    SpriteRenderer sp;

    float duration = 50f;
    float timer = 0f;

    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (Player.i.transform.position.y - (transform.position.y - (sp.size.y / 2)) > 0)
        {
            sp.color = new Color(sp.color.r, sp.color.g, sp.color.b, Player.i.transform.position.y - (transform.position.y - (sp.size.y / 2)) / 10);
        }
        else
            sp.color = Color.white;
    }
}
