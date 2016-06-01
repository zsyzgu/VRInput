using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour {
    string IP = "";
    int port = 1234;

    void Start() {

    }

    void Update() {
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
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            sendMessage("1");
        }
        if (Input.GetKeyUp(KeyCode.Alpha2)) {
            sendMessage("2");
        }
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        Debug.Log(message);
    }

    [RPC]
    void reciveMessage(string message, NetworkMessageInfo info) {
        NetworkPlayer sender = info.sender;
        if (sender.ToString() == "-1") {
            sender = Network.player;
        }

        if (sender != Network.player) {
            recvMessage(message);
        }
    }
}
