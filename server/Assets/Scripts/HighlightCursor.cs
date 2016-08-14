using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighlightCursor : MonoBehaviour {
    public Text defautWord;
    float viewTime = 0f;

	void Start () {
	
	}
	
	void Update () {
	    if (Input.GetButton("Fire1")) {
            GetComponent<Image>().color = Color.red;
        } else {
            GetComponent<Image>().color = Color.white;
        }

        if (viewTime > 0f) {
            viewTime -= Time.deltaTime;
            if (viewTime <= 0f) {
                viewTime = 0f;
                defautWord.text = "";
            }
        }
	}

    public void setDefaultWord(string word) {
        defautWord.text = word;
        viewTime = 1f;
    }

    public void clearDefaultWord() {
        defautWord.text = "";
        viewTime = 0f;
    }
}
