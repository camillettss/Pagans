using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FireLight : MonoBehaviour
{
    [SerializeField] float swing;

    Light2D Light;

    float count = 0;
    float timer = .1f;

    private void Start()
    {
        Light = GetComponent<Light2D>();
    }

    private void Update()
    {

        Light.pointLightInnerRadius = Mathf.Lerp(swing, -swing, count);

        count += Time.deltaTime;

        if (count >= timer)
        {
            swing = -swing;
            count = 0;
            timer = Random.Range(.4f, .8f);
        }
    }
}
