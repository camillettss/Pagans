using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class LibUI : MonoBehaviour, UIController
{
    [SerializeField] List<StoryBook> heroesBooks;

    [SerializeField] Transform categoriesContainer;
    [SerializeField] Transform booksContainer;

    [SerializeField] GameObject readingBook;
    [SerializeField] Text page1text;
    [SerializeField] Text page2text;

    [SerializeField] LibBookUI bookPrefab;

    int selected_category = 0;
    int selected_book = 0;
    int selected_page = 0;

    float change_direction = 0f;

    List<LibBookUI> slotUIs;

    [SerializeField] List<GameObject> keyTips_toFade; // struct: image(inner)text so fade children too

    private void OnDisable()
    {
        Player.i.playerInput.actions["Submit"].performed -= onSubmit;
        Player.i.playerInput.actions["Navigate"].performed -= onNavigate;
        Player.i.playerInput.actions["ExtraNavigation"].performed -= onExtraNav;
        Player.i.playerInput.actions["Cancel"].performed -= onCancel;

        Player.i.playerInput.SwitchCurrentActionMap("Player");
    }

    private void OnEnable()
    {
        Player.i.playerInput.SwitchCurrentActionMap("UI");

        Player.i.playerInput.actions["Submit"].performed += onSubmit;
        Player.i.playerInput.actions["Navigate"].performed += onNavigate;
        Player.i.playerInput.actions["ExtraNavigation"].performed += onExtraNav;
        Player.i.playerInput.actions["Cancel"].performed += onCancel;
    }

    private void Awake()
    {
        selected_category = 0;
        selected_book = 0;
        selected_page = 0;
    }

    IEnumerator changePage()
    {
        // start anim
        page1text.DOFade(0f, 0.2f);
        page2text.DOFade(0f, 0.2f);

        readingBook.GetComponent<Animator>().SetFloat("direction", change_direction);
        readingBook.GetComponent<Animator>().SetTrigger("change");

        yield return new WaitForSeconds(.3f);

        page1text.DOFade(1f, .2f);
        page2text.DOFade(1f, .2f);

        page1text.text = slotUIs[selected_book].book.Pages[selected_page].content;
        page2text.text = slotUIs[selected_book].book.Pages[selected_page + 1].content;
    }

    IEnumerator openBook()
    {
        slotUIs[selected_book].choose();

        categoriesContainer.GetComponent<Image>().DOFade(0f, 0.5f);

        foreach (Transform child in categoriesContainer)
            child.GetComponent<Text>().DOFade(0f, 0.5f); // fade categories

        foreach (var child in keyTips_toFade)
        {
            child.GetComponent<Image>().DOFade(0f, 0.5f); // fade tips
            child.transform.GetChild(0).GetComponent<Text>().DOFade(0f, 0.6f); // fade tips
        }

        yield return new WaitForSeconds(.8f);
        readingBook.SetActive(true);

        page1text.text = slotUIs[selected_book].book.Pages[selected_page].content;
        page2text.text = slotUIs[selected_book].book.Pages[selected_page+1].content;
    }

    List<StoryBook> getByCategory()
    {
        if (selected_category == 0)
            return Player.i.Recipes;

        else if (selected_category == 1)
            return Player.i.GodsBooks;

        else if (selected_category == 2)
            return Player.i.ElvesBooks;

        else if (selected_category == 3)
            return Player.i.MonstersBooks;

        else if (selected_category == 4)
            return Player.i.StoriesBooks;

        else
            return new List<StoryBook>();
    }

    void UpdateContents()
    {
        selected_book = 0;

        foreach (Transform child in booksContainer.transform)
            Destroy(child.gameObject);

        slotUIs = new List<LibBookUI>();

        var iter = getByCategory();

        if(iter.Count > 0)
        {
            foreach (var book in iter)
            {
                var ibook = Instantiate(bookPrefab, booksContainer);
                ibook.SetData(book);

                slotUIs.Add(ibook);
            }
        }
        else
        {
            // actually does nothing
            // 1.5.1: write ur own book.
            // 1.5.1? sarò arrivato alla 2.3.0 e ancora devo fixare bug coddio
        }

        selected_book = 0;
        UpdateBookSelection();
    }

    public void UpdateCategorySelection()
    {
        for(int i=0; i<categoriesContainer.childCount; i++)
        {
            if (i == selected_category)
                categoriesContainer.GetChild(i).GetComponent<Text>().color = GameController.Instance.selectedDefaultColor;
            else
                categoriesContainer.GetChild(i).GetComponent<Text>().color = GameController.Instance.unselectedDefaultColor;
        }

        UpdateContents();
    }

    void UpdateBookSelection()
    {
        if (booksContainer.childCount < 1)
            return;

        for(int i=0; i<slotUIs.Count; i++)
        {
            print($"actual i:{i} max iterations:{booksContainer.childCount}");
            if (i == selected_book)
                slotUIs[i].nameText.color = GameController.Instance.selectedOnBookColor;
            else
                slotUIs[i].nameText.color = GameController.Instance.unselectedDefaultColor;
        }
    }

    public void onSubmit(InputAction.CallbackContext ctx)
    {
        if (!readingBook.activeSelf) StartCoroutine(openBook());
    }

    public void onCancel(InputAction.CallbackContext ctx)
    {
        if (readingBook.activeSelf)
        {
            readingBook.SetActive(false);
            categoriesContainer.GetComponent<Image>().DOFade(1f, 0.1f);

            foreach (Transform child in categoriesContainer)
                child.GetComponent<Text>().DOFade(1f, 0.5f); // fade categories

            foreach (var child in keyTips_toFade)
            {
                child.GetComponent<Image>().DOFade(1f, 0.5f); // fade tips
                child.transform.GetChild(0).GetComponent<Text>().DOFade(1f, 0.6f); // fade tips
            }
        }
        else
        {
            GameController.Instance.state = GameState.FreeRoam;
            gameObject.SetActive(false);
            if (GameController.Instance.ActiveNPC != null)
            {
                GameController.Instance.ActiveNPC.canMove = true;
                GameController.Instance.ActiveNPC = null;
            }
        }
    }

    public void onNavigate(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>();

        if (!readingBook.activeSelf)
        {
            if (input == Vector2.down)
                selected_book += 3;
            else if(input == Vector2.up)
                selected_book -= 3;
            else if (input == Vector2.left)
                selected_book--;
            else if (input == Vector2.right)
                selected_book++;

            selected_book = Mathf.Clamp(selected_book, 0, booksContainer.childCount - 1);

            UpdateBookSelection();
        }
        else if (slotUIs[selected_book].book.Pages.Count > 2)
        {
            if (input.x > 0)
            {
                selected_page += 2;
                change_direction = 0f;
            }
            else if (input.x < 0)
            {
                selected_page -= 2;
                change_direction = 1f;

                selected_page = Mathf.Clamp(selected_page, 0, slotUIs[selected_book].book.Pages.Count - 3);

                StartCoroutine(changePage());
            }
        }
    }

    void onExtraNav(InputAction.CallbackContext ctx)
    {
        var input = ctx.ReadValue<Vector2>().y;

        if (input > 0)
            --selected_category;
        else if (input < 0)
            ++selected_category;

        selected_category = Mathf.Clamp(selected_category, 0, categoriesContainer.childCount - 1);

        UpdateCategorySelection();
    }
}