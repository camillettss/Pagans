using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraderController : MonoBehaviour, IEntity
{
    public string Name;

    [HideInInspector] public Inventory inventory;
    [SerializeField] GameObject signal;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    public void Interact(Player player)
    {
        GameController.Instance.dialogueBox.StartQuestionDialogue(
                new Dialogue(
                    new string[] { }
                    ),
                "vendi",
                "compra",
                () =>
                {
                    GameController.Instance.OpenState(GameState.Shop, this);
                    GameController.Instance.shopUI.switchToSell(true);
                },
                () =>
                {
                    GameController.Instance.OpenState(GameState.Shop, this);
                    GameController.Instance.shopUI.switchToSell(false);
                });
    }

    public void takeDamage(int dmg)
    {
        print("scemo demmerda");
    }

    public void ShowSignal()
    {
        if(!signal.activeSelf)
        {
            signal.SetActive(true);
        }
    }
}
