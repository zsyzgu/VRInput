﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class keyboard : MonoBehaviour {
    public RectTransform cursor;
    public Text screen;
    private RectTransform hoverKey;

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
        if (cursor.position.x < key.position.x - key.rect.width * key.localScale.x / 2 || cursor.position.x > key.position.x + key.rect.width * key.localScale.x / 2) return false;
        if (cursor.position.y < key.position.y - key.rect.height * key.localScale.y / 2 || cursor.position.y > key.position.y + key.rect.height * key.localScale.y / 2) return false;
        return true;
    }

    public void confirm() {
        string str = hoverKey.GetComponentInChildren<Text>().text;
        if (str == "delete") {
            screen.text = "";
        } else {
            screen.text += str;
        }
    }
}
