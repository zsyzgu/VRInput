using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Server : MonoBehaviour {
    static private Server server;
    static private float[] keyboardSize = { 0.25f, 0.5f };

    public Canvas canvas;
    public Text infoText;
    private int port = 1234;
    private string IP = "";
    public StreamWriter sw;

    public bool tapOn = true;
    public bool bigKeyboard = true;
    public bool fastCursor = false;
    public bool singlePoint = false;

    void Start() {
        server = this;
        IP = getIP();

        FileInfo file = new FileInfo(Application.persistentDataPath + "\\" + "log.txt");
        sw = file.CreateText();
    }

    void OnDestroy() {
        sw.Close();
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
        server.sw.WriteLine(message);
    }

    static public bool isTapOn() {
        return server.tapOn;
    }

    static public void setTapOn() {
        server.tapOn ^= true;
        if (server.tapOn) {
            log("tap on");
        } else {
            log("tap off");
        }
    }

    static public bool isBigKeyboard() {
        return server.bigKeyboard;
    }

    static public void setBigKeyboard() {
        server.bigKeyboard ^= true;
        RectTransform rect = server.canvas.GetComponent<RectTransform>();
        float size = keyboardSize[(server.bigKeyboard) ? 1 : 0];
        rect.localScale = new Vector3(size, size, size);
        log("size " + size);
    }

    static public bool isFastCursor() {
        return server.fastCursor;
    }

    static public void setFastCursor() {
        server.fastCursor ^= true;
        if (server.fastCursor) {
            log("fastCursor on");
        } else {
            log("fastCursor off");
        }
    }

    static public bool isSinglePoint() {
        return server.singlePoint;
    }

    static public void setSinglePoint() {
        server.singlePoint ^= true;
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        if (message == "1") {
            setTapOn();
        }
        if (message == "2") {
            setBigKeyboard();
        }
        if (message == "3") {
            setFastCursor();
        }
    }

    [RPC]
    void reciveMessage(string message, NetworkMessageInfo info) {
        recvMessage(message);
    }
}
