using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class server : MonoBehaviour {
    public GameObject canvasParent;
    public RectTransform cursor;
    public GameObject keyboard;
    int cursorSize = 40;
    string IP = "";
    int port = 1234;
    string message = "-1, -1";

    void OnGUI() {
        //message from client
        /*if (Input.GetButtonUp("Fire1")) {
            message = "confirm";
        }
        else if (Input.GetButton("Fire1")) {
            message = (float)Input.mousePosition.x / Screen.width + ", " + (float)Input.mousePosition.y / Screen.height;
            GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
        }
        else {
            message = "untouch";
            GetComponent<NetworkView>().RPC("reciveMessage", RPCMode.All, message);
        }*/

        switch (Network.peerType) {
            case NetworkPeerType.Disconnected:
                startServer();
                break;
            case NetworkPeerType.Server:
                onServer();
                break;
            case NetworkPeerType.Client:
                break;
            case NetworkPeerType.Connecting:
                break;
        }
    }

    void startServer() {
        NetworkConnectionError error = Network.InitializeServer(12, port, false);
        switch (error) {
            case NetworkConnectionError.NoError:
                break;
            default:
                Debug.Log("Error:" + error);
                break;
        }
    }

    Vector3 rot = new Vector3(0.1f, 0f, 0f);
    void onServer() {
        IP = Network.player.ipAddress;
        GUILayout.Box(IP);
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
        cursor.localPosition = new Vector3(0f, 0f, 1f);

        canvasParent.transform.rotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
        if (message == "confirm") {
            keyboard.GetComponent<keyboard>().confirm();
        }
        else if (message != "untouch") {
            float x = float.Parse(message.Split(',')[0]);
            float y = float.Parse(message.Split(',')[1]);
            x = (1 - x) * 10 - 5;
            y = y * 10 - 5;
            cursor.localPosition = new Vector3(x, y, 1f);
        }
    }
}
