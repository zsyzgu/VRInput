using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tracking : MonoBehaviour {
    Texture2D texture;

    int brushRadius = 1;
    bool isDrawing = false;
    int lastPixelX = -1;
    int lastPixelY = -1;

    private LineRenderer lineRenderer;
    private int rendererCnt = 0;
    
    void Start () {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetColors(Color.yellow, Color.red);
        lineRenderer.SetWidth(0.1f, 0.1f);
        lineRenderer.SetVertexCount(rendererCnt);

        texture = (Texture2D)GetComponent<RawImage>().texture;
    }
	
	void Update () {
        GetComponent<RectTransform>().sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
    }

    void OnDestroy() {
        clearCanvas();
    }

    void clearCanvas() {
        rendererCnt = 0;
        lineRenderer.SetVertexCount(rendererCnt);
    }

    public void addPos(float x, float y) {
        if (isDrawing == false) {
            clearCanvas();
        }

        isDrawing = true;
        lineRenderer.SetVertexCount(++rendererCnt);
        lineRenderer.SetPosition(rendererCnt - 1, new Vector3(x * 5f - 2.5f, y * 5f - 2.5f, 5f));
    }

    public bool stopDrawing() {
        if (isDrawing) {
            isDrawing = false;
            return true;
        }
        return false;
    }
}
