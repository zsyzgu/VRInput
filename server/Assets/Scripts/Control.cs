﻿using UnityEngine;
using System.Collections;
using UnityEngine.VR;
using UnityEngine.UI;

public class Control : MonoBehaviour {
    const int FRAME_PER_SAMPLE = 5;

    public GameObject trackingSpace;
    public RectTransform canvas;
    public RectTransform cursor;
    public GameObject keyboard;
    public GameObject tracking;

    private bool mouseHidden = true;
    private float rotationY = 0f;
    private int frameCnt = 0;

    void OnGUI() {

    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        rotateHead();
    }

   bool aimPos(out Vector2 ret) {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo)) {
            Vector2 pos = (Vector2)(canvas.transform.worldToLocalMatrix * hitInfo.point);
            float x = pos.x / canvas.rect.width + 0.5f;
            float y = pos.y / canvas.rect.height + 0.5f;
            ret = new Vector2(x, y);
            return true;
        }
        ret = new Vector2();
        return false;
    }

    void rotateHead() {
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            //trackingSpace.transform.rotation (ON_PC) == InputTracking.GetLocalRotation(VRNode.CenterEye) (ON_VR)
            if (Input.GetKeyUp(KeyCode.H)) {
                mouseHidden ^= true;
            }

            if (mouseHidden) {
                Screen.lockCursor = true;

                float rotationX = trackingSpace.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * 5f;
                rotationY += Input.GetAxis("Mouse Y") * 5f; 
                rotationY = Mathf.Clamp(rotationY, -60f, 60f);
                trackingSpace.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

                if (Input.GetButton("Fire1")) {
                    headWriting();
                } else {
                    moveCursor();
                    confirm();
                }
            } else {
                Screen.lockCursor = false;
            }
        }
        else {
            if (Input.GetButton("Fire1")) {
                headWriting();
            } else {
                moveCursor();
                confirm();
            }
        }
    }

    void confirm() {
        if (tracking.GetComponent<Tracking>().stopDrawing()) {
            keyboard.GetComponent<Keyboard>().confirm();
        }
    }
    
    void headWriting() {
        Vector2 pos;
        if (aimPos(out pos) == false) {
            return;
        }

        moveCursor();

        if (frameCnt-- == 0) {
            frameCnt = FRAME_PER_SAMPLE;

            //Draw line
            tracking.GetComponent<Tracking>().addPos(pos.x, pos.y);
            //Record gesture input
            keyboard.GetComponent<Dictionary>().addPos(new Vector2(pos.x, pos.y));

        }
    }

    void moveCursor() {
        Vector2 pos;
        if (aimPos(out pos) == false) {
            return;
        }

        float cursorX = (pos.x - 0.5f) * canvas.rect.width;
        float cursorY = (pos.y - 0.5f) * canvas.rect.height;
        cursor.localPosition = new Vector3(cursorX, cursorY, 0f);
    }
}