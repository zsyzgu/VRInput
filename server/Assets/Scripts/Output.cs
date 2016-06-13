using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Output : MonoBehaviour {
    public GameObject keyboard;
    public Text phrasesField;
    public Text inputField;

    private string inputText = "";
    private Dictionary dictionary;
    private System.Random random = new System.Random();
    private string[] phrases = {};

	void Start () {
        dictionary = keyboard.GetComponent<Dictionary>();

        TextAsset textAsset = Resources.Load("phrases") as TextAsset;
        phrases = textAsset.text.Split('\n');

        //null char at the end !!
        for (int i = 0; i < phrases.Length; i++) {
            if (phrases[i].Length != 0 && char.IsLetter(phrases[i][phrases[i].Length - 1]) == false) {
                phrases[i] = phrases[i].Substring(0, phrases[i].Length - 1);
            }
        }
    }
	
	void Update () {
	    if (phrasesField.text == "") {
            Server.log("phraseUpdate");
            phrasesField.text = getPhrase();
        }
        if (Time.fixedTime - Mathf.Floor(Time.fixedTime) < 0.5f) {
            inputField.text = inputText + "_";
        } else {
            inputField.text = inputText;
        }
	}

    string getPhrase() {
        int cnt = 0;

        for (int i = random.Next(phrases.Length); ; i = random.Next(phrases.Length)) {
            string phrase = phrases[i];
            string[] words = phrase.Split(' ');

            bool check = true;
            foreach (string word in words) {
                if (!dictionary.existWord(word)) {
                    check = false;
                }
            }

            if (check || ++cnt == 20) {
                return phrase;
            }
        }

        return "";
    }

    public void deleteWord() {
        for (int i = inputText.Length - 2; i >= 0; i--) {
            if (inputText[i] == ' ') {
                inputText = inputText.Substring(0, i + 1);
                return;
            }
        }
        inputText = "";
    }

    public void addWord(string str) {
        inputText += str + " ";
        
        if (inputText.Substring(0, inputText.Length - 1) == phrasesField.text) {
            phrasesField.text = getPhrase();
            inputText = "";
        }
    }

    public void addChar(char ch) {
        inputText += ch;

        if (inputText == phrasesField.text) {
            phrasesField.text = getPhrase();
            inputText = "";
        }
    }
}
