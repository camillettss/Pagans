using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosingUI : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] Text choosablePrefab;
    List<Text> slotUIs = new List<Text>();

    public void UpdateItems()
    {
        foreach (Transform child in content.transform)
            Destroy(child.gameObject);

        slotUIs = new List<Text>();
        foreach(var wp in Player.i.inventory.Weapons)
        {
            var obj = Instantiate(choosablePrefab, content.transform);
            obj.text = wp.item.Name;

            slotUIs.Add(obj);
        }
    }

    public void HandleUpdate()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
        }
    }
}
