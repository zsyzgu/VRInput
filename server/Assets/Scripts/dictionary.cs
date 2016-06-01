using UnityEngine;
using System.IO;
using System.Collections;

public class Dictionary : MonoBehaviour {
    // script on keyboard
    public const int MAX_WORD = 5000;
    public const int METRIC_SAMPLE = 20, POSLIST_SAMPLE = 50;

    public class Word {
        public float pri;
        public string word;
    }
    ArrayList lexicon = new ArrayList();
    ArrayList posList = new ArrayList();

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
    
	void Update () {
        
	}

    public ArrayList getWordList() {
        if (posList.Count == 0) {
            return new ArrayList();
        }

        ArrayList wordList = new ArrayList();

        if (posList.Count > POSLIST_SAMPLE) {
            ArrayList samplePosList = new ArrayList();
            for (int i = 0; i < POSLIST_SAMPLE; i++) {
                samplePosList.Add(posList[(i + 1) * posList.Count / POSLIST_SAMPLE - 1]);
            }
            posList = samplePosList;
        }

        for (int i = 0; i < lexicon.Count; i++) {
            ArrayList wordPosList = new ArrayList();
            string str = ((Word)lexicon[i]).word;
            for (int j = 0; j < str.Length; j++) {
                RectTransform key = transform.FindChild("key" + (char)(str[j] - 'a' + 'A')).GetComponent<RectTransform>();
                RectTransform canvas = transform.parent.GetComponent<RectTransform>();
                float x = key.localPosition.x / canvas.rect.width + 0.5f;
                float y = key.localPosition.y / canvas.rect.height + 0.5f;
                wordPosList.Add(new Vector2(x, y));
            }

            Word word = new Word();
            word.word = str;
            word.pri = calnPri(posList, wordPosList);
            wordList.Add(word);
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
        Server.log("pos " + pos.x + " " + pos.y);
        posList.Add(pos);
    }

    public void clearPos() {
        posList.Clear();
    }

    private float calnPri(ArrayList A, ArrayList B) {
        float ret = 0;

        float lenA = 0;
        for (int i = 0; i + 1 < A.Count; i++) {
            lenA += Vector2.Distance((Vector2)A[i], (Vector2)A[i + 1]);
        }
        lenA /= (METRIC_SAMPLE - 1);

        float lenB = 0;
        for (int i = 0; i + 1 < B.Count; i++) {
            lenB += Vector2.Distance((Vector2)B[i], (Vector2)B[i + 1]);
        }
        lenB /= (METRIC_SAMPLE - 1);

        int u = 0, v = 0;
        float leftA = 0, leftB = 0;
        for (int k = 0; k < METRIC_SAMPLE; k++) {
            while (u + 1 < A.Count && Vector2.Distance((Vector2)A[u], (Vector2)A[u + 1]) <= leftA) {
                leftA -= Vector2.Distance((Vector2)A[u], (Vector2)A[u + 1]);
                u++;
            }
            while (v + 1 < B.Count && Vector2.Distance((Vector2)B[v], (Vector2)B[v + 1]) <= leftB) {
                leftB -= Vector2.Distance((Vector2)B[v], (Vector2)B[v + 1]);
                v++;
            }
            Vector2 posA = (Vector2)A[u], posB = (Vector2)B[v];
            if (u + 1 < A.Count) {
                posA += ((Vector2)A[u + 1] - (Vector2)A[u]) * (leftA / Vector2.Distance((Vector2)A[u], (Vector2)A[u + 1]));
            }
            if (v + 1 < B.Count) {
                posB += ((Vector2)B[v + 1] - (Vector2)B[v]) * (leftB / Vector2.Distance((Vector2)B[v], (Vector2)B[v + 1]));
            }
            float dist = Vector2.Distance(posA, posB);
            ret += dist;
            leftA += lenA;
            leftB += lenB;
        }

        ret /= METRIC_SAMPLE;
        return ret;
    }
}

