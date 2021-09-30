using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] GameObject hitEffectPrefab;

    float counter = 0;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Instantiate(hitEffectPrefab, transform.position, transform.rotation);
        Destroy(gameObject);

        if(collision.collider.TryGetComponent(out NPCController npc))
        {
            print(FindObjectOfType<Player>().inventory.Equipment.Count);
            var eqArrow = FindObjectOfType<Player>().inventory.getEquiped("arrow").item;
            print($"arrow found: {eqArrow}");
            eqArrow.Use(FindObjectOfType<Player>(), npc, eqArrow.GetLongDamage()); // Usa la freccia equipaggiata
        }
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if(counter >= 3f)
            Destroy(gameObject);
    }
}
