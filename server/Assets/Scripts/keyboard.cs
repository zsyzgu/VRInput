using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class keyboard : MonoBehaviour {
    public RectTransform cursor;
    public Text screen;
    private RectTransform hoverKey = null;
    private ArrayList wordList;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        hoverKey = null;
        foreach (RectTransform key in transform) {
            if (cursorInsideKey(key)) {
                hoverKey = key;
                setKeyColor(key, Color.yellow);
            } else {
                setKeyColor(key, Color.white);
            }
        }
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

    string deleteWord(string text) {
        for (int i = text.Length - 2; i >= 0; i--) {
            if (text[i] == ' ') {
                return text.Substring(0, i + 1);
            }
        }
        return "";
    }

    public void confirm() {
        if (hoverKey != null) {
            if (hoverKey.name == "keyDelete") {
                screen.text = deleteWord(screen.text);
                GetComponent<dictionary>().clearPos();
                return;
            }
            if (hoverKey.name.Length >= 7 && hoverKey.name.Substring(0, 6) == "select") {
                screen.text = deleteWord(screen.text) + hoverKey.GetComponentInChildren<Text>().text;
                GetComponent<dictionary>().clearPos();
                return;
            }
        }

        wordList = GetComponent<dictionary>().getWordList();
        foreach (RectTransform key in transform) {
            if (key.name.Length >= 7 && key.name.Substring(0, 6) == "select") {
                int rank = key.name[6] - '0';
                if (rank < wordList.Count) {
                    key.GetComponentInChildren<Text>().text = ((dictionary.Word)wordList[rank]).word + " ";
                }
                else {
                    key.GetComponentInChildren<Text>().text = "";
                }
            }
        }
        if (wordList.Count > 0) {
            screen.text += ((dictionary.Word)wordList[0]).word + " ";
            GetComponent<dictionary>().clearPos();
        }
    }
}
