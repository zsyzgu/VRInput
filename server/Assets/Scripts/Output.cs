using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Output : MonoBehaviour {
    public GameObject keyboard;
    public Text phrasesField;
    public Text inputField;

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
        for (int i = inputField.text.Length - 2; i >= 0; i--) {
            if (inputField.text[i] == ' ') {
                inputField.text = inputField.text.Substring(0, i + 1);
                return;
            }
        }
        inputField.text = "";
    }

    public void addWord(string str) {
        inputField.text += str + " ";
        
        if (inputField.text.Substring(0, inputField.text.Length - 1) == phrasesField.text) {
            if (Server.tapIsOn() || keyboard.GetComponent<Keyboard>().cursorInsideKeyboard()) {
                //wait for select when tap is not on
                phrasesField.text = getPhrase();
                inputField.text = "";
            }
        }
    }
}
