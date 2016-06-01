using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tracking : MonoBehaviour {
    int brushRadius = 1;
    bool isDrawing = false;
    int lastPixelX = -1;
    int lastPixelY = -1;

    private LineRenderer lineRenderer;
    private int rendererCnt = 0;
    
    void Start () {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetVertexCount(rendererCnt);
    }
	
	void Update () {
        RectTransform rect = GetComponentInParent<RectTransform>();
        lineRenderer.SetWidth(0.1f * rect.localScale.x, 0.1f * rect.localScale.y);
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

        RectTransform rect = GetComponentInParent<RectTransform>();
        float width = rect.rect.width * rect.localScale.x;
        float height = rect.rect.height * rect.localScale.y;
        isDrawing = true;
        lineRenderer.SetVertexCount(++rendererCnt);
        lineRenderer.SetPosition(rendererCnt - 1, new Vector3((x - 0.5f) * width, (y - 0.5f) * height, rect.transform.position.z - 0.01f));
    }

    public bool stopDrawing() {
        if (isDrawing) {
            isDrawing = false;
            return true;
        }
        return false;
    }
}
