using UnityEngine;
using System.Collections;

public class client : MonoBehaviour {
    string IP = "";
    int port = 1234;
    string message = "";

    void OnGUI() {
        switch (Network.peerType) {
            case NetworkPeerType.Disconnected:
                startConnect();
                break;
            case NetworkPeerType.Server:
                break;
            case NetworkPeerType.Client:
                onClient();
                break;
            case NetworkPeerType.Connecting:
                break;
        }
    }

    void startConnect() {
        IP = GUILayout.TextArea(IP);
        
        if (GUILayout.Button("Connect to server")) {
            NetworkConnectionError error = Network.Connect(IP, port);
            switch (error) {
                case NetworkConnectionError.NoError:
                    break;
                default:
                    Debug.Log("client:" + error);
                    break;
            }
        }
    }

    void onClient() {
        GUILayout.Box(message);
        if (Input.GetButton("Fire1")) {
            message = (float)Input.mousePosition.x / Screen.width + ", " + (float)Input.mousePosition.y / Screen.height;
            GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
        } else {
            message = "-1, -1";
            GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
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
