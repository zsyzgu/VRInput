using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    string IP = "";

	void Start () {
        IP = Server.getIP();
    }
	
	void Update () {
        if (Server.isInSession()) {
            gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
            outputInfo();
        }
    }

    void outputInfo() {
        string info = "";
        info += "IP: " + IP + "\n";
        info += "size: " + Server.getSize() + "\n";
        info += "mapping: " + Server.getSpeed();

        GetComponentInChildren<Text>().text = info;
    }
}
