using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class Output : MonoBehaviour {
    static private Output output;
    public GameObject keyboard;
    public Text phrasesField;
    public Text inputField;
    public AudioClip wordSound;
    public AudioClip phraseSound;

    private string phrasesText = "";
    private string inputText = "";
    private Lexicon lexicon;
    private System.Random random = new System.Random();
    private string[] phrases = {};
    private bool[] phraseUsed = {};
    private float phraseStartTime;

    void Start () {
        output = this;
        lexicon = keyboard.GetComponent<Lexicon>();

        TextAsset textAsset = Resources.Load("phrases") as TextAsset;
        phrases = textAsset.text.Split('\n');
        phraseUsed = new bool[phrases.Length];

        //null char at the end !!
        for (int i = 0; i < phrases.Length; i++) {
            phraseUsed[i] = false;
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
        } else {
            inputField.text = inputText;
        }
        
        if (Server.isInSession()) {
            phrasesField.text = phrasesText;
        } else {
            phrasesField.text = "";
        }
        /*for (int i = 0; i < phrasesText.Length; i++) {
            if (i < inputText.Length) {
                if (phrasesText[i] == inputText[i]) {
                    phrasesField.text += "<color=green>" + phrasesText[i] + "</color>";
                } else {
                    phrasesField.text += "<color=red>" + phrasesText[i] + "</color>";
                }
            } else {
                phrasesField.text += phrasesText[i];
            }
        }*/
    }

    string getPhrase() {
        int cnt = 0;

        for (int i = random.Next(phrases.Length); ; i = random.Next(phrases.Length)) {
            if (phraseUsed[i]) {
                continue;
            }

            string phrase = phrases[i];
            string[] words = phrase.Split(' ');

            bool check = true;
            foreach (string word in words) {
                if (!lexicon.existWord(word)) {
                    check = false;
                }
            }

            if (check || ++cnt == 20) {
                phraseUsed[i] = true;
                return phrase;
            }
        }
    }

    public char lastChar() {
        if (inputText.Length == 0) {
            return ' ';
        }
        return inputText[inputText.Length - 1];
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

    public void clear() {
        while (inputText != "") {
            delete();
        }
        GetComponent<AudioSource>().PlayOneShot(wordSound);
    }

    public void addWord(string str) {
        inputText += str + " ";
        GetComponent<AudioSource>().PlayOneShot(wordSound);
        /*if (inputText.Substring(0, inputText.Length - 1) == phrasesText) {
            updatePhrase();
        }*/
    }

    public void addChar(char ch) {
        inputText += ch;
        GetComponent<AudioSource>().PlayOneShot(wordSound);
        /*if (inputText == phrasesText) {
            updatePhrase();
        }*/
    }

    public void checkPhraseStart() {
        if (inputText == "") {
            phraseStartTime = Time.time;
        }
    }

    public void updatePhrase() {
        phrasesText = getPhrase();
        GetComponent<AudioSource>().PlayOneShot(phraseSound);

        if (Server.isInSession()) {
            Server.phraseIndex++;
            if (Server.phraseIndex == 0) {
                Server.blockIndex++;
            }
            if (Server.phraseIndex == Server.PHRASE_PER_BLOCK) {
                //session end
                Server.phraseIndex = -1;
                Server.endSession();
            }
        }

        inputText = "";
        Server.log("phrase " + phrasesText);
    }

    public float calnError() {
        string A = inputText;
        string B = phrasesText;
        while (A.Length > 0 && A[A.Length - 1] == ' ') {
            A = A.Substring(0, A.Length - 1);
        }

        int[,] f = new int[A.Length + 1, B.Length + 1];
        for (int i = 0; i <= A.Length; i++) {
            for (int j = 0; j <= B.Length; j++) {
                if (i == 0 && j == 0) {
                    f[i, j] = 0;
                } else {
                    f[i, j] = A.Length;
                }
                if (i - 1 >= 0) {
                    f[i, j] = Mathf.Min(f[i, j], f[i - 1, j] + 1);
                }
                if (j - 1 >= 0) {
                    f[i, j] = Mathf.Min(f[i, j], f[i, j - 1] + 1);
                }
                if (i - 1 >= 0 && j - 1 >= 0) {
                    if (A[i - 1] == B[j - 1]) {
                        f[i, j] = Mathf.Min(f[i, j], f[i - 1, j - 1]);
                    } else {
                        f[i, j] = Mathf.Min(f[i, j], f[i - 1, j - 1] + 1);
                    }
                }
            }
        }

        return (float)f[A.Length, B.Length] / A.Length;
    }

    static public void updateResult() {
        int letterCnt = 0;
        for (int i = 0; i < output.inputText.Length; i++) {
            if (char.IsLetter(output.inputText[i])) {
                letterCnt++;
            }
        }
        float deltaTime = Time.time - output.phraseStartTime;
        float rate = (float)letterCnt / deltaTime * 60 / 5;
        float error = output.calnError();
        InfoPanel.setRate(rate);
        InfoPanel.setError(error);
    }
}
