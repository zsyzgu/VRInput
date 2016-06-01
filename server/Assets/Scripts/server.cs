using UnityEngine;
using System.Collections;

public class Server : MonoBehaviour {
    public Canvas canvas;
    private int port = 1234;

    void Start() {

    }

    void Update() {
        switch (Network.peerType) {
            case NetworkPeerType.Disconnected:
                startServer();
                break;
            case NetworkPeerType.Server:
                break;
            case NetworkPeerType.Client:
                break;
            case NetworkPeerType.Connecting:
                break;
        }

        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            sendMessage("1");
        }
        if (Input.GetKeyUp(KeyCode.Alpha2)) {
            sendMessage("2");
        }
        if (Input.GetKeyUp(KeyCode.Alpha3)) {
            sendMessage("3");
        }
        if (Input.GetKeyUp(KeyCode.Alpha4)) {
            sendMessage("4");
        }
    }

    void startServer() {
        NetworkConnectionError error = Network.InitializeServer(12, port, false);
        switch (error) {
            case NetworkConnectionError.NoError:
                break;
            default:
                Debug.Log("Connect Error: " + error);
                break;
        }
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        if (message == "1") {

        }
        if (message == "2") {
           
        }
        if (message == "3") {
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.localScale = new Vector3(rect.localScale.x + 0.1f, rect.localScale.y + 0.1f, rect.localScale.z);
        }
        if (message == "4") {
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.localScale = new Vector3(rect.localScale.x - 0.1f, rect.localScale.y - 0.1f, rect.localScale.z);
        }
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

        recvMessage(message);
    }
}
