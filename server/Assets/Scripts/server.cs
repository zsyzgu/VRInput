using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;

public class Server : MonoBehaviour {
    static public int PHRASE_PER_BLOCK = 8;
    static public int BLOCK_PER_SESSION = 9;
    static public int phraseIndex = -1;
    static public int blockIndex = -1;
    static private Server server;

    public Canvas canvas;
    public Text infoText;
    public GameObject output;
    public GameObject infoPanel;
    public GameObject warning;
    private int port = 1234;
    
    private float[] keyboardSize = {0.4f, 0.6f, 0.8f};
    private float[] cursorSpeed = {1f, 1.5f, 2f};
    private int keyboardSizeIndex = 0;
    private int cursorSpeedIndex = 0;
    private int order = 0;

    public enum Method {
        normal = 0,
        baseline = 1,
        dwell = 2
    };
    private Method method;
    private bool inSession = false;
    private bool inputing = false;

    void Start() {
        server = this;
        //loadSetting();
    }

    /*void loadSetting() {
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
    }*/

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
        if (Server.isInputing()) {
            server.sendMessage(Time.time + " " + message);
        }
    }

    static public void setMethod(Method method) {
        server.method = method;
    }

    static public Method getMethod() {
        return server.method;
    }

    private void modifySize() {
        RectTransform rect = canvas.GetComponent<RectTransform>();
        float size = keyboardSize[keyboardSizeIndex];
        rect.localScale = new Vector3(size, size, size);
    }

    static public void setOrder(int v) {
        server.order = v;
        setSize(v);
        setSpeed(v);
    }

    static public int getOrder() {
        return server.order;
    }

    static public float getSize() {
        return server.keyboardSize[server.keyboardSizeIndex];
    }

    static public void setSize(int index) {
        if (0 <= index && index < server.keyboardSize.Length) {
            server.keyboardSizeIndex = index;
            server.modifySize();
        }
    }

    static public bool canZoomIn() {
        return server.keyboardSizeIndex + 1 < server.keyboardSize.Length;
    }

    static public void zoomIn() {
        if (canZoomIn()) {
            server.keyboardSizeIndex++;
            server.modifySize();
        }
    }

    static public bool canZoomOut() {
        return server.keyboardSizeIndex - 1 >= 0;
    }

    static public void zoomOut() {
        if (canZoomOut()) {
            server.keyboardSizeIndex--;
            server.modifySize();
        }
    }

    static public float getSpeed() {
        return server.cursorSpeed[server.cursorSpeedIndex];
    }

    static public void setSpeed(int index) {
        if (0 <= index && index < server.cursorSpeed.Length) {
            server.cursorSpeedIndex = index;
        }
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

    static public void play() {
        if (isInputing()) {
            server.inputing = false;
            Output.updateResult();
        } else {
            if (isInSession() == false) {
                server.startSession();
            }
        }
    }

    static public void accept() {
        server.inputing = true;
        server.output.GetComponent<Output>().updatePhrase();
        server.output.GetComponent<Output>().logPhrase();
    }

    static public void repeat() {
        server.inputing = true;
        server.output.GetComponent<Output>().clear();
    }

    private void startSession() {
        inSession = true;
        inputing = true;
        server.warning.GetComponent<Text>().text = "";

        output.GetComponent<Output>().updatePhrase();
        string sessionInfo = method.ToString() + "_" + getSize() + "_" + getSpeed();
        log("session " + sessionInfo);
        output.GetComponent<Output>().logPhrase();
    }

    static public void endSession() {
        server.inSession = false;
        server.inputing = false;
        if (blockIndex + 1 == BLOCK_PER_SESSION) {
            server.warning.GetComponent<Text>().text = "Session Finish! You can have a rest.";
        } else if (blockIndex + 1 > 0) {
            server.warning.GetComponent<Text>().text = "Block Finish! You can have a rest.";
        }
    }

    static public bool isInputing() {
        return server.inputing;
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        /*if (message == "1") {
            setMethod(Method.normal);
        }
        if (message == "2") {
            setMethod(Method.baseline);
        }
        if (message == "3") {
            setMethod(Method.dwell);
        }
        if (message == "4") {
            //setMethod(Method.headOnly);
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
        }*/
        if (message == "1") {
            setOrder(0);
        }
        if (message == "2") {
            setOrder(1);
        }
        if (message == "3") {
            setOrder(2);
        }
    }

    [RPC]
    void reciveMessage(string message, NetworkMessageInfo info) {
        recvMessage(message);
    }
}
