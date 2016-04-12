﻿using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.UI;

public class server : MonoBehaviour {
    const bool ON_PC = true;

    public GameObject canvasParent;
    public RectTransform canvas;
    public RectTransform cursor;
    public GameObject keyboard;
    public GameObject trackCanvas;
    
    int cursorSize = 40;
    string IP = "";
    int port = 1234;
    string message = "-1, -1";

    void OnGUI() {
        if (Network.peerType == NetworkPeerType.Server) {
            onServer();
        }
    }

    void startServer() {
        NetworkConnectionError error = Network.InitializeServer(12, port, false);
        switch (error) {
            case NetworkConnectionError.NoError:
                break;
            default:
                Debug.Log("Error:" + error);
                break;
        }
    }
    
    void onServer() {
        IP = Network.player.ipAddress;
        GUILayout.Box(IP);
    }

    [RPC]
    void reciveMessage(string msg, NetworkMessageInfo info) {
        message = msg;
    }

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (ON_PC) {
            GetComponent<client>().onClient();
        }

        switch (Network.peerType) {
            case NetworkPeerType.Disconnected:
                startServer();
                break;
            case NetworkPeerType.Client:
                break;
            case NetworkPeerType.Connecting:
                break;
            default:
                break;
        }

        moveCursor();
        fixCanvasWidth();
        rotateHead();
    }

    void rotateHead() {
        if (ON_PC) {
            if (Input.GetKey(KeyCode.W)) {
                canvasParent.transform.Rotate(Time.deltaTime * 10f, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.S)) {
                canvasParent.transform.Rotate(-Time.deltaTime * 10f, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.A)) {
                canvasParent.transform.Rotate(0f, -Time.deltaTime * 10f, 0f);
            }
            if (Input.GetKey(KeyCode.D)) {
                canvasParent.transform.Rotate(0f, Time.deltaTime * 10f, 0f);
            }
            if (Input.GetKey(KeyCode.R)) {
                canvasParent.transform.rotation = new Quaternion();
            }
        } else {
            canvasParent.transform.rotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
        }
    }

    void fixCanvasWidth() {
        canvas.sizeDelta = new Vector2(canvas.rect.height * Screen.width / Screen.height, canvas.rect.height);
    }

    void moveCursor() {
        cursor.localPosition = new Vector3(0f, 0f, 0f);

        if (message == "confirm") {
            keyboard.GetComponent<keyboard>().confirm();
        }
        else if (message != "untouch") {
            float x = float.Parse(message.Split(',')[0]);
            float y = float.Parse(message.Split(',')[1]);
            //Draw cursor
            float cursorX = (0.5f - x) * canvas.rect.width;
            float cursorY = (y - 0.5f) * canvas.rect.height;
            cursor.localPosition = new Vector3(cursorX, cursorY, 0f);

            //Draw line
            trackCanvas.GetComponent<trackCanvas>().drawLine(x, y);
        } else {
            trackCanvas.GetComponent<trackCanvas>().stopDrawing();
        }
    }
}
