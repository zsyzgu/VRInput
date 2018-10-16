using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Lexicon : MonoBehaviour {
    static private float SIGMA_X = 0.075f;
    static private float SIGMA_Y = 0.038f;
    static private float ST_VALUE = 0.2f;
    static private float EN_VALUE = 0.2f;
    public Output output;

    public const int MAX_WORD = 10000;
    public const int METRIC_SAMPLE = 50;
    public const float DIST_THRESHOLD = 0.1f;

    public class Word {
        public float pri;
        public string word;
    }
    ArrayList lexicon = new ArrayList();
    ArrayList posList = new ArrayList();
    ArrayList wordPosList = new ArrayList();
    static Dictionary<string, float> dict = new Dictionary<string, float>();

    public class WordComparer : IComparer {
        public int Compare(object x, object y) {
            if (((Word)x).pri < ((Word)y).pri) {
                return -1;
            }
            if (((Word)x).pri > ((Word)y).pri) {
                return 1;
            }
            if (Lexicon.getWordPri(((Word)x).word) > Lexicon.getWordPri(((Word)y).word)) {
                return -1;
            } else {
                return 1;
            }
        }
    }

	void Start () {
        inputLexicon();
	}

    void inputLexicon() {
        string[] lines = {};

        TextAsset textAsset = Resources.Load("lexicon") as TextAsset;
        lines = textAsset.text.Split('\n');

        int cnt = 0;
        foreach (string line in lines) {
            Word word = new Word();
            word.word = line.Split(' ')[0];
            word.pri = float.Parse(line.Split(' ')[1]);
            lexicon.Add(word);
            dict[word.word] = word.pri;
            ArrayList pos = new ArrayList();
            for (int i = 0; i < word.word.Length; i++) {
                pos.Add(calnLetterPos(word.word[i]));
            }
            wordPosList.Add(samplePos(pos));
            if (++cnt >= MAX_WORD) {
                break;
            }
        }
    }

	void Update () {
        
	}

    public Vector2 calnLetterPos(char ch) {
        RectTransform key = transform.Find("key" + char.ToUpper(ch)).GetComponent<RectTransform>();
        RectTransform canvas = transform.parent.GetComponent<RectTransform>();
        float x = key.localPosition.x / canvas.rect.width + 0.5f;
        float y = key.localPosition.y / canvas.rect.height + 0.5f;
        return new Vector2(x, y);
    }

    static public float getWordPri(string word) {
        return dict[word];
    }

    public ArrayList getGaussWordList() {
        ArrayList allWordList = new ArrayList();

        for (int i = 0; i < lexicon.Count; i++) {
            string str = ((Word)lexicon[i]).word;

            if (posList.Count != str.Length) {
                continue;
            }

            float sum = 0f;
            for (int j = 0; j < str.Length; j++) {
                //float x = Vector2.Distance(calnLetterPos(str[j]), (Vector2)posList[j]);
                Vector2 posA = calnLetterPos(str[j]);
                Vector2 posB = (Vector2)posList[j];
                float dx = Mathf.Abs(posA.x - posB.x);
                float dy = Mathf.Abs(posA.y - posB.y);
                sum = sum - 0.5f * (dx * dx / (SIGMA_X * SIGMA_X) + dy * dy / (SIGMA_Y * SIGMA_Y));
            }

            Word word = new Word();
            word.word = str;
            word.pri = -Mathf.Exp(sum) * ((Word)lexicon[i]).pri;
            allWordList.Add(word);
        }

        allWordList.Sort(new WordComparer());

        ArrayList wordList = new ArrayList();
        for (int i = 0; i < 4 && i < allWordList.Count; i++) {
            Word word = (Word)allWordList[i];
            word.pri = -word.pri;
            wordList.Add(word);
        }
        return wordList;
    }

    public ArrayList getWordList() {
        if (posList.Count == 0) {
            return new ArrayList();
        }

        ArrayList wordList = new ArrayList();
        ArrayList posSample = samplePos(posList);

        for (int i = 0; i < lexicon.Count; i++) {
            string str = ((Word)lexicon[i]).word;

            Vector2 beginPos = calnLetterPos(str[0]);
            Vector2 endPos = calnLetterPos(str[str.Length - 1]);

            if (calnOvalDist((Vector2)posList[0], beginPos, 0) > DIST_THRESHOLD) {
                continue;
            }
            if (calnOvalDist((Vector2)posList[posList.Count - 1], endPos, 1) > DIST_THRESHOLD) {
                continue;
            }

            Word word = new Word();
            word.word = str;
            word.pri = calnPri(posSample, (ArrayList)wordPosList[i]);
            if (word.pri >= 0) {
                wordList.Add(word);
            }
        }
        
        wordList.Sort(new WordComparer());
        
        return wordList;
    }

    public bool existWord(string word) {
        for (int i = 0; i < lexicon.Count; i++) {
            if (word == ((Word)lexicon[i]).word) {
                return true;
            }
        }
        return false;
    }

    public void addPos(Vector2 pos) {
        if (posList.Count == 0) {
            output.GetComponent<Output>().checkPhraseStart();
            if (Server.getMethod() == Server.Method.normal) {
                Server.log("gestureStart");
            }
        }
        Server.log("pos " + pos.x + " " + pos.y);
        posList.Add(pos);
    }

    public void deletePos() {
        if (posList.Count != 0) {
            posList.RemoveAt(posList.Count - 1);
        }
    }

    public void clearPos() {
        posList.Clear();
    }

    public ArrayList getPosList() {
        return posList;
    }

    private ArrayList samplePos(ArrayList list) {
        ArrayList sampleList = new ArrayList();

        float len = 0;
        for (int i = 0; i + 1 < list.Count; i++) {
            len += Vector2.Distance((Vector2)list[i], (Vector2)list[i + 1]);
        }
        len /= (METRIC_SAMPLE - 1);

        int u = 0;
        float left = 0;
        for (int k = 0; k < METRIC_SAMPLE; k++) {
            while (u + 1 < list.Count && Vector2.Distance((Vector2)list[u], (Vector2)list[u + 1]) <= left) {
                left -= Vector2.Distance((Vector2)list[u], (Vector2)list[u + 1]);
                u++;
            }
            Vector2 pos = (Vector2)list[u];
            if (u + 1 < list.Count) {
                pos += ((Vector2)list[u + 1] - (Vector2)list[u]) * (left / Vector2.Distance((Vector2)list[u], (Vector2)list[u + 1]));
            }
            sampleList.Add(pos);
            left += len;
        }

        return sampleList;
    }

    private float calnPri(ArrayList A, ArrayList B) {
        float ret = 0;
        
        for (int k = 1; k < METRIC_SAMPLE - 1; k++) {
            float dist = calnOvalDist((Vector2)A[k], (Vector2)B[k], 2);
            ret = ret + dist;
        }
        ret = ret / (METRIC_SAMPLE - 2);
        ret = ret * (1 - ST_VALUE - EN_VALUE);
        ret = ret + ST_VALUE * calnOvalDist((Vector2)A[0], (Vector2)B[0], 0);
        ret = ret + EN_VALUE * calnOvalDist((Vector2)A[METRIC_SAMPLE - 1], (Vector2)B[METRIC_SAMPLE - 1], 1);
        return ret;
    }

    private float calnOvalDist(Vector2 A, Vector2 B, int type) {
        float X = 0f;
        float Y = 0f;
        if (type == 0) {
            //start
            X = 0.2723f;
            Y = 0.1714f;
        } else if (type == 1) {
            //end
            X = 0.3058f;
            Y = 0.2052f;
        } else if (type == 2) {
            //middle
            X = 0.4027f;
            Y = 0.2366f;
        }
        float a = Mathf.Sqrt(X / Y);
        float b = Mathf.Sqrt(Y / X);
        return Mathf.Sqrt((A.x - B.x) * (A.x - B.x) / a / a + (A.y - B.y) * (A.y - B.y) / b / b);
    }
}

