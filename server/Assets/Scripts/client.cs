using UnityEngine;
using System.Collections;

public class client : MonoBehaviour {
    string IP = "";
    int port = 1234;
    string message = "";

    void OnGUI() {
        if (Network.peerType == NetworkPeerType.Disconnected) {
            IP = GUILayout.TextArea(IP, GUILayout.Width(300), GUILayout.Height(50));

            if (GUILayout.Button("Connect to server", GUILayout.Width(300), GUILayout.Height(50))) {
                startConnect();
            }
        }
    }

    void startConnect() {
        NetworkConnectionError error = Network.Connect(IP, port);
        switch (error) {
            case NetworkConnectionError.NoError:
                break;
            default:
                Debug.Log("client:" + error);
                break;
        }
    }

    public void onClient() {
        if (Input.GetButtonUp("Fire1")) {
            message = "confirm";
        } else if (Input.GetButton("Fire1")) {
            message = (float)Input.mousePosition.x / Screen.width + ", " + (float)Input.mousePosition.y / Screen.height;
        } else {
            message = "untouch";
        }
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
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
        switch (Network.peerType) {
            case NetworkPeerType.Server:
                break;
            case NetworkPeerType.Client:
                onClient();
                break;
            case NetworkPeerType.Connecting:
                break;
            default:
                break;
        }
    }
}
