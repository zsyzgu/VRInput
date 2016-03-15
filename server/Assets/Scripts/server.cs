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
        float x = float.Parse(message.Split(',')[0]);
        float y = float.Parse(message.Split(',')[1]);
        if (x != -1 && y != -1) {
            x = (1 - x) * 10 - 5;
            y = y * 10 - 5;
            //GUI.DrawTexture(new Rect(x, y, cursorSize, cursorSize), cursor);
            RectTransform cursor = GameObject.Find("Canvas/cursor").GetComponent<RectTransform>();
            cursor.position = new Vector3(x, y, 10f);
            GameObject sphere = GameObject.Find("Sphere");
            sphere.transform.position = new Vector3(x, y, 12f);
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
