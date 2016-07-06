using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.IO;

public class Server : MonoBehaviour {
    static private Server server;
    static private float[] keyboardSize = {0.5f, 0.6f, 0.7f, 0.8f, 0.9f, 1.0f};
    static private float[] cursorSpeed = {1.0f, 1.25f, 1.5f, 1.75f, 2.0f};

    public Canvas canvas;
    public Text infoText;
    private int port = 1234;
    private string IP = "";
    
    public enum Method {
        normal = 0,
        baseline = 1,
        headOnly = 2,
        dwell = 3
    };
    private Method method;
    private int keyboardSizeIndex = 3;
    private int cursorSpeedIndex = 0;
    private bool inSession = false;

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
        if (Input.GetKeyUp(KeyCode.Alpha5)) {
            sendMessage("5");
        }
        if (Input.GetKeyUp(KeyCode.Alpha6)) {
            sendMessage("6");
        }
        if (Input.GetKeyUp(KeyCode.Alpha7)) {
            sendMessage("7");
        }
        if (Input.GetKeyUp(KeyCode.Alpha8)) {
            sendMessage("8");
        }
    }

    void outputInfo() {
        string info = "";
        info += "IP: " + IP + "\n";
        info += "size: " + getSize() + "\n";
        info += "mapping: " + getSpeed();

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

    static public void setMethod(Method method) {
        server.method = method;
    }

    static public Method getMethod() {
        return server.method;
    }

    private void setSize() {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        float size = keyboardSize[keyboardSizeIndex];
        rect.localScale = new Vector3(size, size, size);
        log("size " + size);
    }

    static public float getSize() {
        return keyboardSize[server.keyboardSizeIndex];
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
        }
    }

    static public bool canSpeedDown() {
        return server.cursorSpeedIndex - 1 >= 0;
    }

    static public void speedDown() {
        if (canSpeedDown()) {
            server.cursorSpeedIndex--;
        }
    }

    static public bool isInSession() {
        return server.inSession;
    }

    static public void startSession() {
        server.inSession = true;
        string sessionInfo = server.method.ToString() + "_" + getSize() + "_" + getSpeed();
        log("session " + sessionInfo);
    }

    static public void endSession() {
        server.inSession = false;
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        if (message == "1") {
            setMethod(Method.normal);
        }
        if (message == "2") {
            setMethod(Method.baseline);
        }
        if (message == "3") {
            setMethod(Method.headOnly);
        }
        if (message == "4") {
            setMethod(Method.dwell);
        }
        if (message == "5") {
            zoomIn();
        }
        if (message == "6") {
            zoomOut();
        }
        if (message == "7") {
            speedUp();
        }
        if (message == "8") {
            speedDown();
        }
    }

    [RPC]
    void reciveMessage(string message, NetworkMessageInfo info) {
        recvMessage(message);
    }
}
