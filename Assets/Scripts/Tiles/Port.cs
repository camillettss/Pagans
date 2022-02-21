using System.Collections.Generic;
using UnityEngine;

public class Port : MonoBehaviour
{
    public List<Boat> accessibleBoats => getAccessibleBoats();

    public void Choose()
    {
        GameController.Instance.OpenState(GameState.Port);
    }

    public List<Boat> getAccessibleBoats()
    {
        var res = new List<Boat>();
        foreach (var collider in Physics2D.OverlapCircleAll(Player.i.transform.position, 10f))
        {
            if (collider.TryGetComponent(out Boat boat))
            {
                res.Add(boat);
            }
        }
        return res;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Boat boat))
        {
            accessibleBoats.Add(boat);
        }

        if(collision.tag == "Player")
        {
            Player.i.activePort = this;
            GameController.Instance.portTip.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Boat boat))
        {
            accessibleBoats.Remove(boat);
        }

        if (collision.tag == "Player")
        {
            Player.i.activePort = null;
            GameController.Instance.portTip.SetActive(false);
        }
    }
}
