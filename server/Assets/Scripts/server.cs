using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour {
    static private Server server;

    public Canvas canvas;
    public Text infoText;
    public GameObject output;
    public GameObject infoPanel;
    public GameObject lineGraph;
    private int port = 1234;

    private int phrasePerBlock = 0;
    private float[] keyboardSize;
    private float[] cursorSpeed;
    private int keyboardSizeIndex = 0;
    private int cursorSpeedIndex = 0;

    public enum Method {
        normal = 0,
        baseline = 1,
        headOnly = 2,
        dwell = 3
    };
    private Method method;
    private bool inSession = false;

    void Start() {
        server = this;
        loadSetting();
    }

    void loadSetting() {
        TextAsset textAsset = Resources.Load("setting") as TextAsset;

        string[] methods = textAsset.text.Split('\n');
        for (int i = 0; i < methods.Length; i++) {
            string[] values = methods[i].Split(' ');
            if (values[0] == "phrase") {
                phrasePerBlock = int.Parse(values[1]);
            }
            if (values[0] == "size") {
                keyboardSize = new float[values.Length - 2];
                keyboardSizeIndex = int.Parse(values[1]);
                for (int j = 2; j < values.Length; j++) {
                    keyboardSize[j - 2] = float.Parse(values[j]);
                }
            }
            if (values[0] == "speed") {
                cursorSpeed = new float[values.Length - 2];
                cursorSpeedIndex = int.Parse(values[1]);
                for (int j = 2; j < values.Length; j++) {
                    cursorSpeed[j - 2] = float.Parse(values[j]);
                }
            }
        }
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
    }

    static public string getIP() {
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
        if (Server.isInSession()) {
            server.sendMessage(Time.time + " " + message);
        }
    }

    static public int getPhrasePerBlock() {
        return server.phrasePerBlock;
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
        return server.keyboardSize[server.keyboardSizeIndex];
    }

    static public bool canZoomIn() {
        return server.keyboardSizeIndex + 1 < server.keyboardSize.Length;
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
        return server.cursorSpeed[server.cursorSpeedIndex];
    }

    static public bool canSpeedUp() {
        return server.cursorSpeedIndex + 1 < server.cursorSpeed.Length;
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
        server.infoPanel.SetActive(false);
        server.lineGraph.SetActive(false);

        string sessionInfo = server.method.ToString() + "_" + getSize() + "_" + getSpeed();
        log("session " + sessionInfo);
        server.output.GetComponent<Output>().updatePhrase();
    }

    static public void endSession() {
        server.inSession = false;
        server.infoPanel.SetActive(true);
        server.lineGraph.SetActive(true);
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
