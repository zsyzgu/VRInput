using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class trackCanvas : MonoBehaviour {
    Texture2D texture;

    int brushRadius = 1;
    bool brushing = false;
    int lastPixelX = -1;
    int lastPixelY = -1;

    // Use this for initialization
    void Start () {
	    texture = (Texture2D)GetComponent<RawImage>().texture;
    }
	
	// Update is called once per frame
	void Update () {
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
    }

    void OnDestroy() {
        clearCanvas();
    }

    void clearCanvas() {
        for (int r = 0; r < texture.width; r++) {
            for (int c = 0; c < texture.height; c++) {
                texture.SetPixel(r, c, Color.clear);
            }
        }
        texture.Apply();
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
        texture.Apply();
    }

    public void drawLine(float x, float y) {
        int pixelX = (int)(texture.width * (1 - x));
        int pixelY = (int)(texture.height * y);

        //Check if clear the trackCanvas
        if (brushing == false) {
            clearCanvas();
        }

        //Draw trackCanvas
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

        brushing = true;
        lastPixelX = pixelX;
        lastPixelY = pixelY;
    }

    public void stopDrawing() {
        brushing = false;
    }
}
