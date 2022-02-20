using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName ="Tools/new fishing pole")]
public class FishingPole : ItemBase
{
    [SerializeField] List<ItemBase> foundables;
    public override void Use(Player player)
    {
        var pos = new Vector3(player.transform.position.x + player.animator.GetFloat("FacingHorizontal"), player.transform.position.y + player.animator.GetFloat("FacingVertical"), 0);
        var collider = Physics2D.OverlapCircle(pos, 0.3f, player.seaLayer);
        if(collider!=null)
        {
            player.isFishing = true;
            player.canMove = false;

            GameController.Instance.ShowInfo("fishing...", () =>
            {
                StoryEventHandler.i.AddToInventory(foundables[Random.Range(0, foundables.Count - 1)]);

                player.isFishing = false;
                player.canMove = true;
            }, Random.Range(1, 3));
        }
    }

}
