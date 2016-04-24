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

    void OnGUI() {

    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        fixCanvasWidth();
        rotateHead();
    }

    Vector2 aimPos() {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) {
            Vector2 pos = (Vector2)(canvas.transform.worldToLocalMatrix * hitInfo.point);
            float x = 1 - (pos.x / canvas.rect.width + 0.5f);
            float y = pos.y / canvas.rect.height + 0.5f;
            return new Vector2(x, y); ;
        }
        return new Vector2(-1, -1);
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
            }
            else {
                if (Input.GetKey(KeyCode.H)) {
                    headWriting(aimPos());
                }
                else {
                    moveCursor(aimPos());
                    confirm();
                }
            }
        }
        else {
            if (Network.connections.Length > 0) {
                canvasParent.transform.rotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
            }
            else {
                if (Input.GetButton("Fire1")) {
                    headWriting(aimPos());
                }
                else {
                    moveCursor(aimPos());
                    confirm();
                }
            }
        }
    }

    void confirm() {
        if (trackCanvas.GetComponent<trackCanvas>().stopDrawing()) {
            keyboard.GetComponent<keyboard>().confirm();
        }
    }

    void fixCanvasWidth() {
        canvas.sizeDelta = new Vector2(canvas.rect.height * Screen.width / Screen.height, canvas.rect.height);
        canvas.GetComponent<BoxCollider>().size = new Vector3(canvas.sizeDelta.x, canvas.sizeDelta.y, 0.01f);
    }

    void headWriting(Vector2 pos) {
        if (pos.x < 0) {
            return;
        }

        moveCursor(pos);

        //Draw line
        trackCanvas.GetComponent<trackCanvas>().drawLine(1 - pos.x, pos.y);

        //Record gesture input
        keyboard.GetComponent<dictionary>().addPos(new Vector2(1 - pos.x, pos.y));
    }

    void moveCursor(Vector2 pos) {
        float cursorX = (0.5f - pos.x) * canvas.rect.width;
        float cursorY = (pos.y - 0.5f) * canvas.rect.height;
        cursor.localPosition = new Vector3(cursorX, cursorY, 0f);
    }
}
