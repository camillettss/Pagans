using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SacrificeUI : MonoBehaviour
{
    [SerializeField] Text itemName;
    [SerializeField] Text costDescription;
    [SerializeField] Image itemIcon;

    ItemBase item;

    int getItemExp(ItemBase item)
    {
        return (int)(item.price * Random.Range(0.5f, 1.5f));
    }

    public void Open(ItemBase item)
    {
        itemName.text = item.Name;
        costDescription.text = $"questo sacrificio ti darà {getItemExp(item)} punti esperienza, in cambio di {item.Name}";
        itemIcon.sprite = item.icon;
        gameObject.SetActive(true);
        this.item = item;
    }

    public void HandleUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Player.i.inventory.Remove(item);
            Player.i.experience += getItemExp(item);
            gameObject.SetActive(false);
            GameController.Instance.inventory2.UpdateView();
        }*/
    }
}
