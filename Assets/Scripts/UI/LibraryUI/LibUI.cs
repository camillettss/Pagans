using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LibUI : MonoBehaviour
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

    List<LibBookUI> slotUIs;

    Portal portal;

    private void Awake()
    {
        selected_category = 0;
        selected_book = 0;
    }

    public void setExitPortal(Portal portal)
    {
        this.portal = portal;
    }

    public void HandleUpdate()
    {
        int prev_sel = selected_book;
        int prev_cat = selected_category;

        if(Input.GetKeyDown(KeyCode.X)) // exit
        {
            if (readingBook.activeSelf)
            {
                readingBook.SetActive(false);
                categoriesContainer.GetComponent<Image>().DOFade(1f, 0.1f);
                foreach (Transform child in categoriesContainer)
                {
                    child.GetComponent<Text>().DOFade(1f, 0.1f);
                }
            }
            else
            {
                GameController.Instance.state = GameState.FreeRoam;
                gameObject.SetActive(false);
                //Player.i.transform.position = portal.transform.position; // make them collide so player will go back
            }
        }

        if(!readingBook.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
                selected_book += 3;
            if (Input.GetKeyDown(KeyCode.UpArrow))
                selected_book -= 3;
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                selected_book--;
            if (Input.GetKeyDown(KeyCode.RightArrow))
                selected_book++;

            if (Input.GetKeyDown(KeyCode.Tab))
                --selected_category;
            if (Input.GetKeyDown(KeyCode.LeftControl))
                ++selected_category;

            selected_category = Mathf.Clamp(selected_category, 0, categoriesContainer.childCount - 1);
            selected_book = Mathf.Clamp(selected_book, 0, booksContainer.childCount - 1);

            if (selected_category != prev_cat)
                UpdateCategorySelection();

            else if (selected_book != prev_sel)
                UpdateBookSelection();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                StartCoroutine(openBook());
            }
        }
    }

    IEnumerator openBook()
    {
        slotUIs[selected_book].choose();

        categoriesContainer.GetComponent<Image>().DOFade(0f, 0.5f);
        foreach (Transform child in categoriesContainer)
        {
            child.GetComponent<Text>().DOFade(0f, 0.5f);
        }

        yield return new WaitForSeconds(.8f);
        readingBook.SetActive(true);

        page1text.text = slotUIs[selected_book].story;
        page2text.text = "";
    }

    List<StoryBook> getByCategory()
    {
        if (selected_category == 0)
            return heroesBooks;

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
}
