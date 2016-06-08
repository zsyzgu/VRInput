using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour {
    static private Server server;
    public Canvas canvas;
    public Text infoText;
    public bool tapOn = true;
    private int port = 1234;
    private string IP = "";

    void Start() {
        server = this;
        IP = getIP();
    }

    string getIP() {
        IPHostEntry host;
        string localIP = "";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList) {
            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
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

        command();
        outputInfo();
    }

    void command() {
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

    void outputInfo() {
        string info = "";
        info += "IP : " + IP + "\n";
        RectTransform rect = canvas.GetComponent<RectTransform>();
        info += "SIZE : " + rect.localScale.x + "\n";
        info += "TAP : " + (tapOn ? "ON" : "OFF"); 

        infoText.text = info;
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

    static public void log(string message) {
        server.sendMessage(Time.time + " " + message);
    }

    static public bool tapIsOn() {
        return server.tapOn;
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        if (message == "1") {
            tapOn = true;
            log("tap on");
        }
        if (message == "2") {
            tapOn = false;
            log("tap off");
        }
        if (message == "3") {
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.localScale = new Vector3(rect.localScale.x + 0.1f, rect.localScale.y + 0.1f, rect.localScale.z);
            log("size " + rect.localScale.x);
        }
        if (message == "4") {
            RectTransform rect = canvas.GetComponent<RectTransform>();
            rect.localScale = new Vector3(rect.localScale.x - 0.1f, rect.localScale.y - 0.1f, rect.localScale.z);
            log("size " + rect.localScale.x);
        }
    }

    [RPC]
    void reciveMessage(string message, NetworkMessageInfo info) {
        recvMessage(message);
    }
}
