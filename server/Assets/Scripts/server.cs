using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.UI;

public class server : MonoBehaviour {
    public GameObject trackingSpace;
    public GameObject canvasParent;
    public RectTransform canvas;
    public RectTransform cursor;
    public GameObject keyboard;
    public GameObject trackCanvas;
    
    int cursorSize = 40;
    string IP = "";
    int port = 1234;
    string message = "untouch";

    void OnGUI() {
        if (Network.peerType == NetworkPeerType.Server) {
            onServer();
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
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            GetComponent<client>().onClient();
            if (message != "untouch") {
                float x = float.Parse(message.Split(',')[0]);
                float y = float.Parse(message.Split(',')[1]);
                message = (1 - x) + ", " + y;
            }
        }

        switch (Network.peerType) {
            case NetworkPeerType.Disconnected:
                startServer();
                break;
            case NetworkPeerType.Client:
                break;
            case NetworkPeerType.Connecting:
                break;
            default:
                break;
        }

        rotateHead();
        moveCursor();
        fixCanvasWidth();
    }

    void headWriting() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) {
            Vector2 pos = (Vector2)(canvas.transform.worldToLocalMatrix * hitInfo.point);
            float x = 1 - (pos.x / canvas.sizeDelta.x + 0.5f);
            float y = pos.y / canvas.sizeDelta.y + 0.5f;
            message = x + ", " + y;
        }
    }

    void rotateHead() {
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            //trackingSpace.transform.rotation (ON_PC) == InputTracking.GetLocalRotation(VRNode.CenterEye) (!ON_PC)
            if (Input.GetKey(KeyCode.W)) {
                trackingSpace.transform.Rotate(-Time.deltaTime * 50f, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.S)) {
                trackingSpace.transform.Rotate(+Time.deltaTime * 50f, 0f, 0f);
            }
            if (Input.GetKey(KeyCode.A)) {
                trackingSpace.transform.Rotate(0f, -Time.deltaTime * 50f, 0f);
            }
            if (Input.GetKey(KeyCode.D)) {
                trackingSpace.transform.Rotate(0f, +Time.deltaTime * 50f, 0f);
            }
            if (Input.GetKey(KeyCode.R)) {
                trackingSpace.transform.rotation = new Quaternion();
            }

            if (Network.connections.Length > 0) {
                canvasParent.transform.rotation = trackingSpace.transform.rotation;
            } else {
                if (Input.GetKey(KeyCode.H)) {
                    headWriting();
                } else {
                    //message = "untouch";
                }
            }
        } else {
            if (Network.connections.Length > 0) {
                canvasParent.transform.rotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
            } else {
                if (Input.GetButton("Fire1")) {
                    headWriting();
                } else {
                    message = "untouch";
                }
            }
        }
    }

    void fixCanvasWidth() {
        canvas.sizeDelta = new Vector2(canvas.rect.height * Screen.width / Screen.height, canvas.rect.height);
        canvas.GetComponent<BoxCollider>().size = new Vector3(canvas.sizeDelta.x, canvas.sizeDelta.y, 0.01f);
    }

    void moveCursor() {
        if (message != "untouch") {
            float x = float.Parse(message.Split(',')[0]);
            float y = float.Parse(message.Split(',')[1]);
            //Draw cursor
            float cursorX = (0.5f - x) * canvas.rect.width;
            float cursorY = (y - 0.5f) * canvas.rect.height;
            cursor.localPosition = new Vector3(cursorX, cursorY, 0f);

            //Draw line
            trackCanvas.GetComponent<trackCanvas>().drawLine(1 - x, y);

            //Record gesture input
            keyboard.GetComponent<dictionary>().addPos(new Vector2(1 - x, y));
        } else {
            if (trackCanvas.GetComponent<trackCanvas>().stopDrawing()) {
                keyboard.GetComponent<keyboard>().confirm();
            }
            cursor.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
}
