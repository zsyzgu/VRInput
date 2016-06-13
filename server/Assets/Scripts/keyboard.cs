﻿using UnityEngine;
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

    public void confirm() {
        updateHover();

        if (hoverKey != null && hoverKey.tag == "delete") {
            Server.log("delete");
            output.deleteWord();
            page = 0;
            GetComponent<Dictionary>().clearPos();
            return;
        }

        if (hoverKey != null && hoverKey.tag == "select" && hoverKey.GetComponentInChildren<Text>().text != "") {
            string word = hoverKey.GetComponentInChildren<Text>().text;
            Server.log("select " + word);
            if (Server.tapIsOn()) {
                output.deleteWord();
            }
            output.addWord(word);
            page = 0;
            GetComponent<Dictionary>().clearPos();
            clearSelect();
            return;
        }
        
        if (hoverKey != null && hoverKey.tag == "page") {
            if (hoverKey.name == "lastPage" && canLastPage()) {
                Server.log("lastPage");
                page--;
            }
            if (hoverKey.name == "nextPage" && canNextPage()) {
                Server.log("nextPage");
                page++;
            }
            drawSelect();
            return;
        }

        wordList = GetComponent<Dictionary>().getWordList();
        GetComponent<Dictionary>().clearPos();

        drawSelect();
        if (Server.tapIsOn()) {
            //when tap is not on, user must select
            string defaultWord = drawDefaultWord();
            Server.log("endGesture " + defaultWord);
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
