using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour {
    public RectTransform cursor;
    public GameObject outputScreen;

    private Output output;
    private RectTransform hoverKey = null;
    private RectTransform homeKey = null;
    private ArrayList wordList = new ArrayList();
    private int selectNum = 0;
    private int page = 0;

    // Use this for initialization
    void Start () {
        output = outputScreen.GetComponent<Output>();
        calnSelectNum();
        searchHomeKey();
    }
	
	// Update is called once per frame
	void Update () {
        updateKeysColor();
    }

    void calnSelectNum() {
        foreach (RectTransform key in transform) {
            if (key.tag == "select") {
                selectNum++;
            }
        }
    }

    void searchHomeKey() {
        foreach (RectTransform key in transform) {
            if (key.name == "keyG") {
                homeKey = key;
            }
        }
    }

    void updateHover() {
        hoverKey = null;
        foreach (RectTransform key in transform) {
            if (cursorInsideKey(key)) {
                hoverKey = key;
            }
        }
    }

    void updateKeysColor() {
        updateHover();
        foreach (RectTransform key in transform) {
            if (key.tag == "page") {
                if (key.name == "lastPage" && canLastPage() == false) {
                    setKeyColor(key, Color.gray);
                } else if (key.name == "nextPage" && canNextPage() == false) {
                    setKeyColor(key, Color.gray);
                } else {
                    setKeyColor(key, key == hoverKey ? Color.yellow : Color.white);
                }
            } else if (key.tag == "control") {
                if (key == hoverKey) {
                    setKeyColor(key, Color.yellow);
                } else {
                    if (key.name == "tapOn") {
                        setKeyColor(key, Server.isTapOn() ? Color.white : Color.gray);
                    } else if (key.name == "bigKeyboard") {
                        setKeyColor(key, Server.isBigKeyboard() ? Color.white : Color.gray);
                    } else if (key.name == "fastCursor") {
                        setKeyColor(key, Server.isFastCursor() ? Color.white : Color.gray);
                    }
                }
            } else {
                setKeyColor(key, key == hoverKey ? Color.yellow : Color.white);
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
    
    bool allPosInsideHoverKey() {
        if (hoverKey == null) {
            return false;
        }
        RectTransform canvas = transform.parent.GetComponent<RectTransform>();
        float x = hoverKey.localPosition.x / canvas.rect.width + 0.5f;
        float y = hoverKey.localPosition.y / canvas.rect.height + 0.5f;
        float w = hoverKey.rect.width * hoverKey.localScale.x / canvas.rect.width;
        float h = hoverKey.rect.height * hoverKey.localScale.y / canvas.rect.height;

        ArrayList posList = GetComponent<Dictionary>().getPosList();
        for (int i = 0; i < posList.Count; i++) {
            Vector2 pos = (Vector2)posList[i];
            if (pos.x < x - w / 2 || pos.x > x + w / 2) return false;
            if (pos.y < y - h / 2 || pos.y > y + h / 2) return false;
        }
        return true;
    }

    bool cursorInsideKey(RectTransform key) {
        if (cursor.localPosition.x < key.localPosition.x - key.rect.width * key.localScale.x / 2) return false;
        if (cursor.localPosition.x > key.localPosition.x + key.rect.width * key.localScale.x / 2) return false;
        if (cursor.localPosition.y < key.localPosition.y - key.rect.height * key.localScale.y / 2) return false;
        if (cursor.localPosition.y > key.localPosition.y + key.rect.height * key.localScale.y / 2) return false;
        return true;
    }

    public bool cursorInsideKeyboard() {
        RectTransform board = gameObject.GetComponent<RectTransform>();
        if (cursor.localPosition.x < board.localPosition.x - board.rect.width * board.localScale.x / 2) return false;
        if (cursor.localPosition.x > board.localPosition.x + board.rect.width * board.localScale.x / 2) return false;
        if (cursor.localPosition.y < board.localPosition.y - board.rect.height * board.localScale.y / 2) return false;
        if (cursor.localPosition.y > board.localPosition.y + board.rect.height * board.localScale.y / 2) return false;
        return true;
    }

    public bool cursorInsideHomeKey() {
        return cursorInsideKey(homeKey);
    }

    public bool cursorInsideCmdKeys() {
        foreach (RectTransform key in transform) {
            if (key.tag == "select" || key.tag == "delete" || key.tag == "page" || key.tag == "control") {
                if (cursorInsideKey(key)) {
                    return true;
                }
            }
        }
        return false;
    }

    public void confirm() {
        Dictionary dictionary = GetComponent<Dictionary>();
        updateHover();

        if (hoverKey != null && hoverKey.tag == "delete") {
            Server.log("delete");
            output.delete();
            page = 0;
            dictionary.clearPos();
            wordList.Clear();
            drawSelect();
        } else if (hoverKey != null && hoverKey.tag == "select" && hoverKey.GetComponentInChildren<Text>().text != "") {
            string word = hoverKey.GetComponentInChildren<Text>().text;
            Server.log("select " + word);
            if (Server.isTapOn()) {
                output.delete();
            }
            output.addWord(word);
            page = 0;
            dictionary.clearPos();
            clearSelect();
        } else if (hoverKey != null && hoverKey.tag == "page") {
            if (hoverKey.name == "lastPage" && canLastPage()) {
                Server.log("lastPage");
                page--;
            }
            if (hoverKey.name == "nextPage" && canNextPage()) {
                Server.log("nextPage");
                page++;
            }
            drawSelect();
        } else if (hoverKey != null && hoverKey.tag == "control") {
            if (hoverKey.name == "tapOn") {
                Server.setTapOn();
            } else if (hoverKey.name == "bigKeyboard") {
                Server.setBigKeyboard();
            } else if (hoverKey.name == "fastCursor") {
                Server.setFastCursor();
            }
            dictionary.clearPos();
        } else {
            //hover on letter or symbol
            //TODO: what if !isTapOn
            if (Server.isTapOn() && allPosInsideHoverKey()) {
                char ch = hoverKey.GetComponentInChildren<Text>().text[0];
                if (char.IsLetter(ch)) {
                    ch = char.ToLower(ch);
                }
                output.addChar(ch);
                Server.log("singlePoint" + ch);
            } else {
                wordList = dictionary.getWordList();

                drawSelect();
                if (Server.isTapOn()) {
                    //when tap is not on, user must select
                    string defaultWord = drawDefaultWord();
                    Server.log("endGesture " + defaultWord);
                }
            }
            dictionary.clearPos();
        }
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

    string drawDefaultWord() {
        string word = "";
        if (wordList.Count > 0) {
            word = ((Dictionary.Word)wordList[0]).word;
            output.addWord(word);
            page = 0;
        }
        return word;
    }
}
