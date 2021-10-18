using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IEntity
{
    public int passcode;
    [SerializeField] Sprite openSprite;

    Sprite oldSprite;
    public bool isOpen = false;

    public void Open()
    {
        oldSprite = GetComponent<SpriteRenderer>().sprite;

        GetComponent<SpriteRenderer>().sprite = openSprite;
        GetComponent<BoxCollider2D>().enabled = false;
        unShowSignal();
        isOpen = true;

        GameController.Instance.dialogueBox.StartDialogue(new Dialogue(new string[]
        {
            "dove stai andando??",
            "pensavi avessi programmato anche la mappa dell'intera moria? HAHAHAAHA",
            "si",
            "effettivamente l'ho fatto ma devo ancora risolvere delle cose prima di renderla pubblica, passa nei prossimi aggiornamenti :/"
        }),
        () => 
        {
            StartCoroutine(close());
            GameController.Instance.state = GameState.FreeRoam;
        });
    }

    IEnumerator close()
    {
        yield return new WaitForSeconds(.5f);
        GetComponent<SpriteRenderer>().sprite = oldSprite;
        GetComponent<BoxCollider2D>().enabled = true;
    }

    Key findCorrectKey()
    {
        foreach(var key in Player.i.inventory.GetSlots(0)) // slots[0] = keys, 1-> cons
        {
            print(key.item.name);
            if (((Key)key.item).keycode == passcode)
                return (Key)(key.item);
        }
        return null;
    }

    public void ShowSignal()
    {
        // show signal only if player has this key in inventory.
        var key = findCorrectKey();
        if(key != null)
        {
            print("ora dovrei mostrare la UI");
            Player.i.keyToUse = key;
            Player.i.closeDoor = this;
            GameController.Instance.keyUI.Open(key);
        }
        else
        {
            print("no key found");
        }
        // if the user press Z while signal is showing call interact().
    }
    void unShowSignal()
    {
        if (GameController.Instance.keyUI.isActiveAndEnabled)
            GameController.Instance.keyUI.gameObject.SetActive(false);
    }

    public void Interact(Player player)
    {
        // check the key
    }

    public void takeDamage(int dmg)
    {

    }

    private void FixedUpdate()
    {
        if (isOpen)
            return;

        if (Player.i.isInRange(this))
            ShowSignal();
        else
            unShowSignal();
    }
}
