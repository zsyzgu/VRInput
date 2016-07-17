using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Keyboard : MonoBehaviour {
    private const float DWELL_TIME = 0.4f;
    private const float EXTRA_DWELL_TIME = 0.4f;
    private const float DWELL_MOVE_SPEED = 5f;
    public Sprite cursorImage;
    public Sprite waitingCircleImage;
    public RectTransform cursor;
    public GameObject outputScreen;
    public GameObject mainCamera;

    private Output output;
    private RectTransform hoverKey = null;
    private RectTransform homeKey = null;
    private ArrayList wordList = new ArrayList();
    private int selectNum = 0;
    private int page = 0;
    private float dwellTimestamp;
    private bool dwellTimeout = false;
    private Vector2 lastCursorPos;
    
    void Start () {
        output = outputScreen.GetComponent<Output>();
        calnSelectNum();
        searchHomeKey();
    }
	
	void Update () {
        updateHover();
        updateDwell();

        foreach (RectTransform key in transform) {
            if (key.tag == "page") {
                if (key.name == "lastPage" && canLastPage() == false) {
                    setKeyColor(key, Color.gray);
                } else if (key.name == "nextPage" && canNextPage() == false) {
                    setKeyColor(key, Color.gray);
                } else {
                    setKeyColor(key, key == hoverKey ? Color.yellow : Color.white);
                }
            } else {
                setKeyColor(key, key == hoverKey ? Color.yellow : Color.white);
            }
        }
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
        foreach (RectTransform key in transform) {
            if (cursorInsideKey(key)) {
                if (hoverKey != key) {
                    hoverKey = key;
                }
                return;
            }
        }
        hoverKey = null;
    }

    void updateDwell() {
        if (Server.getMethod() == Server.Method.dwell) {
            if (Vector2.Distance(lastCursorPos, cursor.localPosition) / Time.deltaTime > DWELL_MOVE_SPEED) {
                dwellTimestamp = Time.time;
            }
            lastCursorPos = cursor.localPosition;

            dwellTimeout = false;
            float deltaTime = Time.time - dwellTimestamp;
            if (deltaTime >= DWELL_TIME / 8) {
                cursor.GetComponent<Image>().sprite = waitingCircleImage;
                cursor.GetComponent<Image>().fillAmount = deltaTime / DWELL_TIME;
                if (deltaTime >= DWELL_TIME) {
                    dwellTimestamp = Time.time + EXTRA_DWELL_TIME;
                    dwellTimeout = true;
                    confirm();
                }
            } else {
                cursor.GetComponent<Image>().sprite = cursorImage;
                cursor.GetComponent<Image>().fillAmount = 1f;
            }
        } else {
            cursor.GetComponent<Image>().sprite = cursorImage;
            cursor.GetComponent<Image>().fillAmount = 1f;
            dwellTimestamp = Time.time;
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
            if (key.tag == "select" || key.tag == "delete" || key.tag == "page") {
                if (cursorInsideKey(key)) {
                    return true;
                }
            }
        }
        return false;
    }

    void clearAllScreen() {
        Lexicon lexicon = GetComponent<Lexicon>();
        page = 0;
        lexicon.clearPos();
        wordList.Clear();
        drawSelect();
    }

    public void confirm() {
        Lexicon lexicon = GetComponent<Lexicon>();
        updateHover();

        if (hoverKey != null && hoverKey.tag == "delete") {
            Server.log("delete");
            output.delete();

            if (Server.getMethod() == Server.Method.baseline || Server.getMethod() == Server.Method.dwell) {
                if (output.lastChar() == ' ') {
                    clearAllScreen();
                } else {
                    lexicon.deletePos();
                    wordList = lexicon.getGaussWordList();
                    drawSelect();
                }
            } else {
                clearAllScreen();
            }
        } else if (hoverKey != null && hoverKey.tag == "select") {
            if (hoverKey.GetComponentInChildren<Text>().text != "") {
                string word = hoverKey.GetComponentInChildren<Text>().text;
                Server.log("select " + word);
                if (Server.getMethod() == Server.Method.baseline || Server.getMethod() == Server.Method.dwell) {
                    output.addChar(' ');
                }
                output.delete();
                output.addWord(word);
                page = 0;
                wordList.Clear();
                drawSelect();
            }
            lexicon.clearPos();
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
        } else {
            //hover on letter or symbol
            if (Server.getMethod() == Server.Method.baseline || (Server.getMethod() == Server.Method.dwell && dwellTimeout)) {
                if (hoverKey != null) {
                    char ch = hoverKey.GetComponentInChildren<Text>().text[0];
                    if (char.IsLetter(ch)) {
                        ch = char.ToLower(ch);
                    }
                    output.checkPhraseStart();
                    Server.log("letter " + ch);
                    output.addChar(ch);
                    /*char letter = output.getRespectiveLetter();
                    if (char.IsLetter(letter)) {
                        Vector2 letterPos = lexicon.calnLetterPos(letter);
                        Server.log("letterPos " + letterPos.x + " " + letterPos.y);
                    }*/

                    if (ch == ' ') {
                        lexicon.clearPos();
                        page = 0;
                        wordList.Clear();
                        drawSelect();
                    } else if (char.IsLetter(ch)) {
                        Vector2 pos;
                        mainCamera.GetComponent<MainControl>().aimPos(out pos);
                        lexicon.addPos(pos);
                        wordList = lexicon.getGaussWordList();
                        drawSelect();
                    }
                }
            } else {
                wordList = lexicon.getWordList();

                drawSelect();
                if (Server.getMethod() == Server.Method.normal) {
                    //when tap is not on, user must select
                    if (wordList.Count > 0) {
                        Server.log("gestureEnd " + ((Lexicon.Word)wordList[0]).word);
                        drawDefaultWord();
                    }
                }

                lexicon.clearPos();
            }
        }
    }

    void drawSelect() {
        foreach (RectTransform key in transform) {
            if (key.tag == "select") {
                int rank = page * selectNum + (Server.getMethod() == Server.Method.normal ? 1 : 0); //don't show default word
                float dist = Vector2.Distance(key.localPosition, cursor.localPosition);
                foreach (RectTransform otherKey in transform) {
                    if (otherKey.tag == "select" && otherKey != key) {
                        float otherDist = Vector2.Distance(otherKey.localPosition, cursor.localPosition);
                        if (otherDist < dist) {
                            rank++;
                        }
                    }
                }

                rank += page * selectNum;
                if (rank < wordList.Count) {
                    key.GetComponentInChildren<Text>().text = ((Lexicon.Word)wordList[rank]).word;
                } else {
                    key.GetComponentInChildren<Text>().text = "";
                }
            }
        }
    }

    void drawDefaultWord() {
        if (wordList.Count > 0) {
            output.addWord(((Lexicon.Word)wordList[0]).word);
            page = 0;
        }
    }
}
