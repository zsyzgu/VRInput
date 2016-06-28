using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Server : MonoBehaviour {
    static private Server server;
    static private float[] keyboardSize = {0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f};
    static private float[] cursorSpeed = {1.0f, 1.2f, 1.4f, 1.6f, 1.8f, 2.0f};

    public Canvas canvas;
    public Text infoText;
    private int port = 1234;
    private string IP = "";
    public StreamWriter sw;

    public bool tapOn = true;
    public bool fastCursor = false;
    public bool singlePoint = false;
    public int keyboardSizeIndex = 3;
    public int cursorSpeedIndex = 0;

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
        if (Input.GetKeyUp(KeyCode.Alpha5)) {
            sendMessage("5");
        }
        if (Input.GetKeyUp(KeyCode.Alpha6)) {
            sendMessage("6");
        }
    }

    void outputInfo() {
        string info = "";
        info += "IP : " + IP + "\n";
        RectTransform rect = canvas.GetComponent<RectTransform>();
        info += "size : " + rect.localScale.x + "\n";
        info += "speed : " + cursorSpeed[fastCursor ? 1 : 0];

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
    
    private void setSize() {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        float size = keyboardSize[keyboardSizeIndex];
        rect.localScale = new Vector3(size, size, size);
        log("size " + size);
    }

    static public bool canZoomIn() {
        return server.keyboardSizeIndex + 1 < keyboardSize.Length;
    }

    static public void zoomIn() {
        if (canZoomIn()) {
            server.keyboardSizeIndex++;
            server.setSize();
        }
    }

    static public bool canZoomOut() {
        return server.keyboardSizeIndex - 1 >= 0;
    }

    static public void zoomOut() {
        if (canZoomOut()) {
            server.keyboardSizeIndex--;
            server.setSize();
        }
    }

    static public float getSpeed() {
        return cursorSpeed[server.cursorSpeedIndex];
    }

    static public bool canSpeedUp() {
        return server.cursorSpeedIndex + 1 < cursorSpeed.Length;
    }

    static public void speedUp() {
        if (canSpeedUp()) {
            server.cursorSpeedIndex++;
            log("speed" + getSpeed());
        }
    }

    static public bool canSpeedDown() {
        return server.cursorSpeedIndex - 1 >= 0;
    }

    static public void speedDown() {
        if (canSpeedDown()) {
            server.cursorSpeedIndex--;
            log("speed" + getSpeed());
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
            setSinglePoint();
        }
        if (message == "3") {
            zoomIn();
        }
        if (message == "4") {
            zoomOut();
        }
        if (message == "5") {
            speedUp();
        }
        if (message == "6") {
            speedDown();
        }
    }

    [RPC]
    void reciveMessage(string message, NetworkMessageInfo info) {
        recvMessage(message);
    }
}
