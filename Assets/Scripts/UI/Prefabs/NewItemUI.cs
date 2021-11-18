using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewItemUI : MonoBehaviour
{
    [SerializeField] Text itemName;
    [SerializeField] Text itemDesc;
    [SerializeField] Image itemIcon;

    void UpdateView(ItemBase item)
    {
        itemName.text = item.Name;
        itemDesc.text = item.presentationDesc;

        itemIcon.sprite = item.icon;
    }

    public void Open(ItemBase item)
    {
        UpdateView(item);
        gameObject.SetActive(true);
        StartCoroutine(close());
    }

    IEnumerator close()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }
}
