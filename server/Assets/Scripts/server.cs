using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.UI;

public class server : MonoBehaviour {
    public GameObject canvasParent;
    public RectTransform canvas;
    public RectTransform cursor;
    public GameObject keyboard;
    public GameObject picture;
    
    int cursorSize = 40;
    string IP = "";
    int port = 1234;
    string message = "-1, -1";

    int brushRadius = 1;
    bool brushing = false;
    int lastPixelX = -1;
    int lastPixelY = -1;

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
        //ENABLE server input
        GetComponent<client>().onClient();

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

        moveCursor();
        fixCanvasWidth();
    }

    void fixCanvasWidth() {
        canvas.sizeDelta = new Vector2(canvas.rect.height * Screen.width / Screen.height, canvas.rect.height);
        RectTransform pictureRect = picture.GetComponent<RectTransform>();
        pictureRect.sizeDelta = new Vector2(pictureRect.rect.height * Screen.width / Screen.height, pictureRect.rect.height);
    }

    void drawPoint(Texture2D texture, int pixelX, int pixelY) {
        for (int r = pixelX - brushRadius; r <= pixelX + brushRadius; r++) {
            for (int c = pixelY - brushRadius; c <= pixelY + brushRadius; c++) {
                if (0 <= r && r < texture.width && 0 <= c && c < texture.height) {
                    if ((pixelX - r) * (pixelX - r) + (pixelY - c) * (pixelY - c) <= brushRadius * brushRadius) {
                        texture.SetPixel(r, c, Color.red);
                    }
                }
            }
        }
    }

    void drawLine(float x, float y) {
        Texture2D texture = (Texture2D)picture.GetComponent<RawImage>().texture;
        int pixelX = (int)(texture.width * (1 - x));
        int pixelY = (int)(texture.height * y);

        //Check if clear the picture
        if (brushing == false) {
            for (int r = 0; r < texture.width; r++) {
                for (int c = 0; c < texture.height; c++) {
                    texture.SetPixel(r, c, Color.clear);
                }
            }
        }

        //Draw picture
        if (brushing) {
            int dx = pixelX - lastPixelX;
            int dy = pixelY - lastPixelY;
            int steps = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));
            if (steps > 0) {
                for (int i = 0; i <= steps; i++) {
                    drawPoint(texture, lastPixelX + dx * i / steps, lastPixelY + dy * i / steps);
                }
            }
        }
        drawPoint(texture, pixelX, pixelY);

        texture.Apply();
        brushing = true;
        lastPixelX = pixelX;
        lastPixelY = pixelY;
    }

    void moveCursor() {
        cursor.localPosition = new Vector3(0f, 0f, 0f);

        canvasParent.transform.rotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
        if (message == "confirm") {
            keyboard.GetComponent<keyboard>().confirm();
        }
        else if (message != "untouch") {
            float x = float.Parse(message.Split(',')[0]);
            float y = float.Parse(message.Split(',')[1]);
            //Draw cursor
            float cursorX = (0.5f - x) * canvas.rect.width;
            float cursorY = (y - 0.5f) * canvas.rect.height;
            cursor.localPosition = new Vector3(cursorX, cursorY, 0f);

            //Draw line
            drawLine(x, y);
        } else {
            if (brushing) {
                brushing = false;
            }
        }
    }
}
