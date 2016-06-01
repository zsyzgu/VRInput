using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour {
    public RectTransform cursor;
    public GameObject outputScreen;

    private Output output;
    private RectTransform hoverKey = null;
    private ArrayList wordList = new ArrayList();
    private int selectNum = 0;
    private int page = 0;

    // Use this for initialization
    void Start () {
        output = outputScreen.GetComponent<Output>();
        calnSelectNum();
    }
	
	// Update is called once per frame
	void Update () {
        updateHover();
        updatePageButton();
    }

    void calnSelectNum() {
        foreach (RectTransform key in transform) {
            if (key.tag == "select") {
                selectNum++;
            }
        }
    }

    void updateHover() {
        hoverKey = null;
        foreach (RectTransform key in transform) {
            if (cursorInsideKey(key)) {
                hoverKey = key;
                setKeyColor(key, Color.yellow);
            }
            else {
                setKeyColor(key, Color.white);
            }
        }
    }

    void updatePageButton() {
        foreach (RectTransform key in transform) {
            if (key.name == "lastPage") {
                key.GetComponentInChildren<RawImage>().enabled = canLastPage();
            }
            if (key.name == "nextPage") {
                key.GetComponentInChildren<RawImage>().enabled = canNextPage();
            }
        }
    }

    bool canLastPage() {
        return page - 1 >= 0;
    }

    bool canNextPage() {
        return page + 1 < (wordList.Count + selectNum - 1) / selectNum;
    }

    void setKeyColor(RectTransform key, Color color) {
        var colors = key.GetComponent<Button>().colors;
        colors.normalColor = color;
        key.GetComponent<Button>().colors = colors;
    }

    bool cursorInsideKey(RectTransform key) {
        if (cursor.localPosition.x < key.localPosition.x - key.rect.width * key.localScale.x / 2 || cursor.localPosition.x > key.localPosition.x + key.rect.width * key.localScale.x / 2) return false;
        if (cursor.localPosition.y < key.localPosition.y - key.rect.height * key.localScale.y / 2 || cursor.localPosition.y > key.localPosition.y + key.rect.height * key.localScale.y / 2) return false;
        return true;
    }

    public void confirm() {
        if (hoverKey != null && hoverKey.tag == "delete") {
            output.deleteWord();
            page = 0;
            GetComponent<Dictionary>().clearPos();
            return;
        }

        if (hoverKey != null && hoverKey.tag == "select") {
            output.deleteWord();
            output.addWord(hoverKey.GetComponentInChildren<Text>().text);
            page = 0;
            GetComponent<Dictionary>().clearPos();
            clearSelect();
            return;
        }
        
        if (hoverKey != null && hoverKey.tag == "page") {
            if (hoverKey.name == "lastPage" && canLastPage()) {
                page--;
            }
            if (hoverKey.name == "nextPage" && canNextPage()) {
                page++;
            }
            drawSelect();
            return;
        }

        wordList = GetComponent<Dictionary>().getWordList();
        GetComponent<Dictionary>().clearPos();

        drawSelect();
        drawDefaultWord();
    }

    void drawSelect() {
        foreach (RectTransform key in transform) {
            if (key.tag == "select") {
                int rank = page * selectNum + (key.name[6] - '0');
                if (rank < wordList.Count) {
                    key.GetComponentInChildren<Text>().text = ((Dictionary.Word)wordList[rank]).word;
                } else {
                    key.GetComponentInChildren<Text>().text = "";
                }
            }
        }
    }

    void clearSelect() {
        foreach (RectTransform key in transform) {
            if (key.tag == "select") {
                key.GetComponentInChildren<Text>().text = "";
            }
        }
    }

    void drawDefaultWord() {
        if (wordList.Count > 0) {
            output.addWord(((Dictionary.Word)wordList[0]).word);
            page = 0;
        }
    }
}
