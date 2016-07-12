using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HighlightCursor : MonoBehaviour {
	void Start () {
	
	}
	
	void Update () {
	    if (Input.GetButton("Fire1")) {
            GetComponent<Image>().color = Color.red;
        } else {
            GetComponent<Image>().color = Color.white;
        }
	}
}
