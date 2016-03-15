using UnityEngine;
using System.Collections;

public class server : MonoBehaviour {
    public Texture cursor;
    int cursorSize = 40;
    string IP = "";
    int port = 1234;
    string message = "-1, -1";

    void OnGUI() {
        switch (Network.peerType) {
            case NetworkPeerType.Disconnected:
                startServer();
                break;
            case NetworkPeerType.Server:
                onServer();
                break;
            case NetworkPeerType.Client:
                break;
            case NetworkPeerType.Connecting:
                break;
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
        GUILayout.Box(message);
        float x = float.Parse(message.Split(',')[0]);
        float y = float.Parse(message.Split(',')[1]);
        if (x != -1 && y != -1) {
            x = x * Screen.width;
            y = (1 - y) * Screen.height;
            GUI.DrawTexture(new Rect(x, y, cursorSize, cursorSize), cursor);
        }
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
	
	}
}
