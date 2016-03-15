using UnityEngine;
using System.Collections;

public class selector : MonoBehaviour {
    void OnGUI() {
        if (GUILayout.Button("Server")) {
            GetComponent<server>().enabled = true;
            this.enabled = false;
        }
        if (GUILayout.Button("Client")) {
            GetComponent<client>().enabled = true;
            this.enabled = false;
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
