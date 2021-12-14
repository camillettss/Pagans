using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Boat : MonoBehaviour
{
    public string Name = "Boat";
    public Sprite icon;

    public void Mount()
    {
        Player.i.activeBoat = this;

        // disabilita i collide con i layer del mare e abilita quelli del background
        IEnumerable<Tilemap> query = from tm in FindObjectsOfType<Tilemap>()
                                     where tm.gameObject.name == "Background" || tm.gameObject.name == "Sea"
                                     select tm;

        foreach(var q in query)
        {
            try
            {
                q.GetComponent<TilemapCollider2D>().enabled = !q.GetComponent<TilemapCollider2D>().enabled;
            }
            catch
            {
                print("[ERR] this tilemap doesnt have a collider.");
            }
        }

        Player.i.transform.position = transform.position;
        Player.i.animator.SetBool("onBoat", true);
    }

    public void Dismount()
    {
        Player.i.activeBoat = null;

        // disabilita i collide con i layer del mare e abilita quelli del background
        IEnumerable<Tilemap> query = from tm in FindObjectsOfType<Tilemap>()
                                     where tm.gameObject.name == "Background" || tm.gameObject.name == "Sea"
                                     select tm;

        foreach (var q in query)
        {
            try
            {
                q.GetComponent<TilemapCollider2D>().enabled = !q.GetComponent<TilemapCollider2D>().enabled;
            }
            catch
            {
                print("[ERR] this tilemap doesnt have a collider.");
            }
        }


        Player.i.transform.position = new Vector3(transform.position.x + Player.i.animator.GetFloat("FacingHorizontal"), transform.position.y + Player.i.animator.GetFloat("FacingVertical"));
        Player.i.animator.SetBool("onBoat", false);
    }
}
