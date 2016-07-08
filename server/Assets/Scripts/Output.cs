using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Output : MonoBehaviour {
    public GameObject keyboard;
    public Text phrasesField;
    public Text inputField;
    public AudioClip wordSound;
    public AudioClip phraseSound;
    public GameObject lineGraph;

    private string phrasesText = "";
    private string inputText = "";
    private Dictionary dictionary;
    private System.Random random = new System.Random();
    private string[] phrases = {};
    private int completedPhrases = -1;
    private List<float> sessionRate = new List<float>();
    private int sessionLetterCnt;
    private float sessionStartTime;

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
	    if (phrasesText == "") {
            updatePhrase();
        }
        updateOutput();
	}

    void updateOutput() {
        if (Time.fixedTime - Mathf.Floor(Time.fixedTime) < 0.5f) {
            inputField.text = inputText + "_";
        }
        else {
            inputField.text = inputText;
        }

        phrasesField.text = "";
        for (int i = 0; i < phrasesText.Length; i++) {
            if (i < inputText.Length) {
                if (phrasesText[i] == inputText[i]) {
                    phrasesField.text += "<color=green>" + phrasesText[i] + "</color>";
                } else {
                    phrasesField.text += "<color=red>" + phrasesText[i] + "</color>";
                }
            } else {
                phrasesField.text += phrasesText[i];
            }
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
    }

    public void delete() {
        if (inputText.Length - 1 >= 0 && inputText[inputText.Length - 1] == ' ') {
            //delete a word
            bool empty = true;

            for (int i = inputText.Length - 2; i >= 0; i--) {
                if (inputText[i] == ' ') {
                    inputText = inputText.Substring(0, i + 1);
                    empty = false;
                    break;
                }
            }

            if (empty) {
                inputText = "";
            }
        } else {
            //delete a letter
            if (inputText.Length - 1 >= 0) {
                inputText = inputText.Substring(0, inputText.Length - 1);
            }
        }

        GetComponent<AudioSource>().PlayOneShot(wordSound);
    }

    public void addWord(string str) {
        inputText += str + " ";
        GetComponent<AudioSource>().PlayOneShot(wordSound);

        if (inputText.Substring(0, inputText.Length - 1) == phrasesText) {
            updatePhrase();
        }
    }

    public void addChar(char ch) {
        inputText += ch;
        GetComponent<AudioSource>().PlayOneShot(wordSound);

        if (inputText == phrasesText) {
            updatePhrase();
        }
    }

    public void updatePhrase() {
        phrasesText = getPhrase();
        GetComponent<AudioSource>().PlayOneShot(phraseSound);

        if (Server.isInSession()) {
            //session begin
            if (completedPhrases == -1) {
                sessionStartTime = Time.time;
                sessionLetterCnt = 0;
            }
            completedPhrases++;
            
            if (completedPhrases < Server.getPhrasePerBlock()) {
                //each phrase
                for (int i = 0; i < phrasesText.Length; i++) {
                    if (char.IsLetter(phrasesText[i])) {
                        sessionLetterCnt++;
                    }
                }
            } else {
                //session end
                completedPhrases = -1;
                Server.endSession();
                showRate();
            }
        }

        inputText = "";
        Server.log("phrase " + phrasesText);
    }

    private void showRate() {
        float escapeTime = Time.time - sessionStartTime;
        float rate = sessionLetterCnt / escapeTime * 60 / 5;
        Debug.Log(escapeTime + ", " + sessionLetterCnt);

        sessionRate.Add(rate);
        LineGraphManager manager = lineGraph.GetComponent<LineGraphManager>();
        manager.setLine(sessionRate);
        manager.show();
    }
}
