using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CauldronUI : MonoBehaviour
{
    [SerializeField] GameObject content; // scrollview
    [SerializeField] Image itemIcon;
    [SerializeField] GameObject TextPrefab;
    [SerializeField] GameObject fireUI;
    [SerializeField] Image scrollView;

    int selected = 0;
    List<Text> slotUIs;

    Cauldron cauldron;

    bool done = false;

    public void SetSource(Cauldron cauldron)
    {
        this.cauldron = cauldron;
    }

    private void Awake()
    {
        fireUI.SetActive(false);
    }

    public void HandleUpdate()
    {
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            gameObject.SetActive(false);
            GameController.Instance.state = GameState.FreeRoam;
        }

        var prev = selected;

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selected;
        if (Input.GetKeyDown(KeyCode.UpArrow))
            --selected;

        selected = Mathf.Clamp(selected, 0, slotUIs.Count - 1);

        if(selected != prev)
        {
            UpdateSelection();
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Perform();
        }*/
    }

    public void UpdateContents()
    {
        if(!cauldron.isCooking)
        {
            itemIcon.rectTransform.localPosition = new Vector3(0, 80, 0);

            foreach (Transform child in content.transform)
                Destroy(child.gameObject);

            slotUIs = new List<Text>();

            foreach (var consumable in Player.i.inventory.GetByBookmark(2)[0])
            {
                var t = Instantiate(TextPrefab, content.transform);
                t.GetComponent<Text>().text = consumable.item.Name;

                slotUIs.Add(t.GetComponent<Text>());
            }

            UpdateSelection();
        }
        else // sta(va) cucinando qualcosa //(o aveva finito ma non era ritirato)
        {
            itemIcon.rectTransform.localPosition = Vector3.zero;
            itemIcon.sprite = cauldron.ingredient.icon;
        }
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i == selected)
                slotUIs[i].color = GameController.Instance.selectedDefaultColor;
            else
                slotUIs[i].color = GameController.Instance.unselectedDefaultColor;
        }

        itemIcon.sprite = Player.i.inventory.GetByBookmark(2)[0][selected].item.icon;
    }

    void Perform()
    {
        print($"using: {slotUIs[selected].text}.");
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        while(itemIcon.transform.localPosition.y > 0)
        {
            itemIcon.rectTransform.localPosition = new Vector3(
                itemIcon.transform.localPosition.x,
                itemIcon.transform.localPosition.y - 4,
                0
            );

            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(.1f);

        fireUI.SetActive(true);

        StartCoroutine(cauldron.Cook(Player.i.inventory.GetByBookmark(2)[0][selected].item));
        scrollView.DOFade(0f, .4f);

        foreach (Transform child in content.transform)
            Destroy(child.gameObject);
    }

}
