using UnityEngine;
using System.IO;
using System.Collections;

public class Dictionary : MonoBehaviour {
    public Output output;

    public const int MAX_WORD = 5000;
    public const int METRIC_SAMPLE = 16;
    public const float DIST_THRESHOLD = 0.2f;

    public class Word {
        public float pri;
        public string word;
    }
    ArrayList lexicon = new ArrayList();
    ArrayList posList = new ArrayList();

    private float endPosY = 0;

    public class WordComparer : IComparer {
        public int Compare(object x, object y) {
            if (((Word)x).pri < ((Word)y).pri) {
                return -1;
            }
            if (((Word)x).pri > ((Word)y).pri) {
                return 1;
            }
            return 0;
        }
    }

	void Start () {
        inputLexicon();
        calnEndPosY();
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
            if (++cnt >= MAX_WORD) {
                break;
            }
        }
    }

    void calnEndPosY() {
        float keyTY = calnLetterPos('t').y;
        float keyGY = calnLetterPos('g').y;
        endPosY = keyGY + (keyTY - keyGY) * 2.5f;
    }
    
	void Update () {
        
	}

    Vector2 calnLetterPos(char ch) {
        RectTransform key = transform.FindChild("key" + char.ToUpper(ch)).GetComponent<RectTransform>();
        RectTransform canvas = transform.parent.GetComponent<RectTransform>();
        float x = key.localPosition.x / canvas.rect.width + 0.5f;
        float y = key.localPosition.y / canvas.rect.height + 0.5f;
        return new Vector2(x, y);
    }

    public ArrayList getWordList() {
        if (posList.Count == 0) {
            return new ArrayList();
        }

        ArrayList wordList = new ArrayList();
        ArrayList posSample = samplePos(posList);

        for (int i = 0; i < lexicon.Count; i++) {
            ArrayList wordPosList = new ArrayList();
            string str = ((Word)lexicon[i]).word;

            Vector2 beginPos = calnLetterPos(str[0]);
            Vector2 endPos = calnLetterPos(str[str.Length - 1]);
            if (Server.getMethod() == Server.Method.headOnly) {
                beginPos = calnLetterPos('g');
                endPos.y = endPosY;
            }

            if (Vector2.Distance((Vector2)posList[0], beginPos) > DIST_THRESHOLD) {
                continue;
            }
            if (Vector2.Distance((Vector2)posList[posList.Count - 1], endPos) > DIST_THRESHOLD) {
                continue;
            }

            if (Server.getMethod() == Server.Method.headOnly) {
                wordPosList.Add(beginPos);

                for (int j = 0; j < str.Length; j++) {
                    wordPosList.Add(calnLetterPos(str[j]));
                }

                wordPosList.Add(endPos);
            } else {
                for (int j = 0; j < str.Length; j++) {
                    wordPosList.Add(calnLetterPos(str[j]));
                }
            }

            Word word = new Word();
            word.word = str;
            ArrayList wordPosSample = samplePos(wordPosList);
            word.pri = calnPri(posSample, wordPosSample);
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
            output.GetComponent<Output>().gestureStart();

        }
        posList.Add(pos);
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

        for (int k = 0; k < METRIC_SAMPLE; k++) {
            float dist = Vector2.Distance((Vector2)A[k], (Vector2)B[k]);
            if (dist > DIST_THRESHOLD) {
                return -1;
            }
            ret += dist;
        }

        ret /= METRIC_SAMPLE;
        return ret;
    }
}

