using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Tools/new axe")]
public class Axe : ItemBase
{
    [SerializeField] bool hasUsagesLimit = false;
    [MyBox.ConditionalField(nameof(hasUsagesLimit))] [SerializeField] int maxUsages;
    [SerializeField] int usagesCount = 0;

    private void Awake()
    {
        usagesCount = 0;
    }

    public override void Use(Player player)
    {
        Debug.Log($"usages; {usagesCount}");
        player.animator.SetTrigger("useAxe");
        player.inventory.StartCoroutine(cut());
    }

    void Break()
    {
        Player.i.inventory.Unequip(1); // questo dovrebbe essere equipaggiato, quindi disequipaggia
        GameController.Instance.hotbar.UpdateItems();
        Player.i.inventory.Remove(this);
    }

    IEnumerator cut()
    {
        yield return new WaitForSeconds(0.3f);

        var tree = Player.i.GetFrontalCollider(Player.i.farmingLayer);

        if (tree != null)
        {
            do
            {
                if (tree.TryGetComponent(out Tree tempTree))
                    tempTree.Cut();
                else if (tree.TryGetComponent(out CuttableTile cuttable))
                    cuttable.takeDamage(1);

                else break;
                
                if (hasUsagesLimit)
                {
                    usagesCount++;
                    Debug.Log(usagesCount);
                    if (usagesCount >= maxUsages)
                        Break();
                }
            }
            while (false);
        }
    }

    public override void Use(Player player, IEntity npc, int damage = 1)
    {

    }
}
