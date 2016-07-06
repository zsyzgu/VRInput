using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class Client : MonoBehaviour {
    static private int PHRASES_ON_GUI = 10;
    private string IP = "";
    private string userName = "";
    private int port = 1234;
    private StreamWriter sw;
    private ArrayList messageList = new ArrayList();

    void Start() {
        IP = getIP();
        userName = "user" + Random.Range(100, 1000);
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
            userName = GUILayout.TextArea(userName, GUILayout.Width(300), GUILayout.Height(50));
            IP = GUILayout.TextArea(IP, GUILayout.Width(300), GUILayout.Height(50));

            if (GUILayout.Button("Connect to server", GUILayout.Width(300), GUILayout.Height(50))) {
                startConnect();
            }
        } else {
            string messages = "";
            int st = messageList.Count > 10 ? messageList.Count - 10 : 0;
            for (int i = st; i < messageList.Count; i++) {
                messages += (string)messageList[i] + "\n";
            }
            GUILayout.TextArea(messages, GUILayout.Width(500), GUILayout.Height(200));
        }
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
        if (Input.GetKeyUp(KeyCode.Alpha0)) {
            sendMessage("0");
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
        if (Input.GetKeyUp(KeyCode.Alpha9)) {
            sendMessage("9");
        }
    }

    void sendMessage(string message) {
        GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
    }

    void recvMessage(string message) {
        sw = new StreamWriter(userName + ".txt", true);
        sw.WriteLine(message);
        sw.Close();
        messageList.Add(message);
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
