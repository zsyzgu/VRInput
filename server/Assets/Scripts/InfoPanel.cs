using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    string IP = "";
    static private float rate;
    static private float error;

	void Start () {
        IP = Server.getIP();
    }
	
	void Update () {
        outputInfo();
    }

    void outputInfo() {
        string info = "";
        info += "IP: " + IP + "\n";
        info += "Technique: " + Server.getMethod().ToString() + "\n";
        info += "Block: " + (Server.blockIndex + 1) + "/" + Server.BLOCK_PER_SESSION + "\n";
        info += "Phrase: " + (Server.phraseIndex + 1) + "/" + Server.PHRASE_PER_BLOCK + "\n";

        if (Server.isInSession() && Server.isInputing() == false) {
            info += "\n";
            info += "Speed: " + rate + " WPM\n";
            info += "Error: " + (error * 100) + "%\n";
        }

        GetComponentInChildren<Text>().text = info;
    }

    static public void setRate(float v) {
        rate = v;
    }

    static public void setError(float v) {
        error = v;
    }
}
