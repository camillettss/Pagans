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

        if(collision.collider.TryGetComponent(out IEntity npc))
        {
            print("shooting, from bullet");
            var bow = Player.i.inventory.Weapons[Player.i.inventory.equipedWeapon].item;
            print("ur bow is:" + bow);
            bow.Use(Player.i, npc, bow.GetLongDamage());

            /*
            print(FindObjectOfType<Player>().inventory.Equipment.Count);
            var eqArrow = FindObjectOfType<Player>().inventory.getEquiped("arrow").item;
            print($"arrow found: {eqArrow}");
            eqArrow.Use(FindObjectOfType<Player>(), npc, eqArrow.GetLongDamage()); // Usa la freccia equipaggiata
            */
        } 
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if(counter >= 3f)
            Destroy(gameObject);
    }
}
